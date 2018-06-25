using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.OneNote;
using System.IO;
using System.Text;
using System.Configuration;

namespace OneNotePageSearcher
{
    internal class OneNoteManager
    {
        private static readonly XNamespace One = "http://schemas.microsoft.com/office/onenote/2013/onenote";

        private NetLuceneProvider lucene;

        private Application oneNote = new Microsoft.Office.Interop.OneNote.Application();

        public double progressRate = 0;
        double count = 0;
        double totalCount = 0;

        public bool canceled = false;
        public bool isDebug = false;
        public string currentPageTitle = "";
        public bool isIndexing = false;
        string sampleNotebookUrl = null;

        private IEnumerable<XElement> pageList;

        public OneNoteManager(bool isDebug)
        {
            this.isDebug = isDebug;
            lucene = new NetLuceneProvider(false);
            lucene.debug = this.isDebug;

            string outputXML;
            if (this.isDebug) sampleNotebookUrl = "{E9ACF59B-250A-0A88-1083-AD27FB56155D}{1}{B0}";
            oneNote.GetHierarchy(sampleNotebookUrl, HierarchyScope.hsPages, out outputXML);
            pageList = XDocument.Parse(outputXML).Descendants(One + "Page");
        }


        public void BuildIndex()
        {
            progressRate = 0;
            string outputXML;
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            pageList = XDocument.Parse(outputXML).Descendants(One + "Page");
            lucene.SetWorkingDirectory();
            AddIndexByTime();
            lucene.CloseWriter();
        }

        public void AddIndexByTime()
        {
            lucene.SetUpWriter(false);
            HashSet<String> deletedID;
            HashSet<String> updateID;
            GetUpdateIndexID(out deletedID, out updateID);
            if (isDebug) Console.WriteLine("Begining Index Process...");
            foreach (var id in deletedID)
            {
                if (isDebug) Console.WriteLine("Deleting: " + id);
                lucene.DeleteDocumentByID(id);
            }
            isIndexing = true;
            AddIndexFromID(updateID);
            isIndexing = false;
            var currentTime = String.Format("{0:u}", DateTime.UtcNow);
            AddUpdateAppSettings("LastIndexTime", currentTime);
        }

        private void AddIndex()
        {
            lucene.SetUpWriter(true);
            HashSet<String> updateID = new HashSet<string>();
            foreach (var n in pageList)
            {
                updateID.Add(n.Attribute("ID").Value);
            }
            AddIndexFromID(updateID);
        }

        public void AddIndexFromID(HashSet<String> updateID)
        {
            totalCount = updateID.Count();
            if (totalCount == 0) progressRate = 1;
            else
            {
                foreach (var id in updateID)
                {
                    if (isDebug) Console.WriteLine("Adding: " + id);
                    lucene.DeleteDocumentByID(id);
                    count += 1;
                    progressRate = count / totalCount;
                    try
                    {
                        IndexByParagraph(id);
                    }
                    catch (System.Runtime.InteropServices.COMException e)
                    {
                        int code = e.HResult;
                        if (isDebug) Console.WriteLine("Exception: Error Code is " + code);
                    }
                }
            }
        }

        private void IndexByParagraph(string pageID)
        {
            string xmlString;
            oneNote.GetPageContent(pageID, out xmlString);

            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            currentPageTitle = GetPageTitle(pageID);
            if (isDebug) Console.WriteLine("\t" + des.Count() + " Paragraphs");

            if (des.Count() > 0)
            {
                foreach (var el in des)
                {
                    var paraID = el.Attribute("objectID").Value;
                    var text = GetTextFromNode(el);
                    if (text == null) continue;
                    var paragraphText = RemoveUnwantedTags(text);
                    lucene.AddDocument(new Tuple<string, string, string>(pageID, paraID, paragraphText));
                }
            }
        }

        private void IndexByDocument(string pageID)
        {
            string xmlString;
            oneNote.GetPageContent(pageID, out xmlString);
            var des = XDocument.Parse(xmlString).Descendants(One + "OE");
            StringBuilder sb = new StringBuilder("");

            foreach (var el in des)
            {
                var text = GetTextFromNode(el);
                if (text == null) continue;
                var paragraphText = RemoveUnwantedTags(text);
                if (isDebug)
                {
                    using (StreamWriter w = File.AppendText("log.txt"))
                    {
                        Log(paragraphText, w);
                    }
                }
                sb.AppendLine(paragraphText);
            }
            lucene.AddDocument(new Tuple<string, string, string>(pageID, "NULL", sb.ToString()));
        }

        private string GetTextFromNode(XElement el)
        {
            var textNode = el.Element(One + "T");
            string text;
            if (textNode == null)
            {
                var image = el.Element(One + "Image");
                if (image == null) return null;
                var altAttr = image.Attribute("alt");
                if (altAttr == null) return null;
                text = altAttr.Value;
            }
            else
            {
                text = textNode.Value;
            }
            return text.ToString();
        }

        public List<Tuple<string, string, string, float>> Search(string query)
        {
            return lucene.Search(query);
        }

        /// <summary>
        /// Send id in index, open requested page.
        /// </summary>
        /// <param name="id"></param>
        public void OpenPage(string pageID, string paraID = "NULL")
        {
            if (paraID == "NULL")
            {
                oneNote.NavigateTo(pageID);
            }
            else
            {
                oneNote.NavigateTo(pageID, paraID);
            }
        }

        /// <summary>
        /// Send id in index, return requested page title
        /// </summary>
        public string GetPageTitle(string pageId)
        {
            IEnumerable<XElement> pages =
                (from el in pageList
                 where el.Attribute("ID").Value == pageId
                 select el);
            if (pages.Any()) return pages.First().Attribute("name").Value;
            else return "Default Title";
        }

        public HashSet<String> GetAllPageId()
        {
            var idSet = new HashSet<String>();
            foreach (var n in pageList)
            {
                idSet.Add(n.Attribute("ID").Value);
                if (isDebug) Console.WriteLine(n.Attribute("lastModifiedTime").Value);
            }
            return idSet;
        }

        // If t1 is older than t2, return true, otherwise return false
        private bool CompareTimeByString(string t1, string t2)
        {
            return DateTime.Parse(t1) >= DateTime.Parse(t2);
        }

        public void GetUpdateIndexID(out HashSet<String> indexIDToDelete, out HashSet<String> indexIDToCreate)
        {
            var currentID = GetAllPageId();
            var legacyID = lucene.GetAllValuesByField("pageID");
            indexIDToDelete = new HashSet<String>();
            indexIDToCreate = new HashSet<String>();

            // We want to find page that is already deleted
            foreach (var id in legacyID)
            {
                if (!currentID.Contains(id))
                {
                    indexIDToDelete.Add(id);
                }
            }

            // We also want to find page that is updated and created
            string oldTime = ReadSetting("LastIndexTime") ?? "1978-06-18T08:56:47.000Z";
            if (isDebug) Console.WriteLine(oldTime);
            foreach (var n in pageList)
            {
                // Last Modified Time is after index time
                if (CompareTimeByString(n.Attribute("lastModifiedTime").Value, oldTime))
                {
                    if (isDebug) Console.WriteLine(n.Attribute("lastModifiedTime").Value);
                    indexIDToCreate.Add(n.Attribute("ID").Value);
                }
            }
        }

        static string ReadSetting(string key)
        {
            string result;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                result = null;
            }
            return result;
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        public void Main()
        {
            string outputXML = System.IO.File.ReadAllText(@"test.xml");
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            StreamWriter sw = new StreamWriter("D:\\Sample.xml");
            sw.WriteLine(outputXML);
            sw.Close();
        }

        internal static string RemoveUnwantedTags(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;


            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(data);

            var acceptableTags = new String[] { };
            try
            {
                var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    var parentNode = node.ParentNode;

                    if (acceptableTags.Contains(node.Name) || node.Name == "#text") continue;
                    var childNodes = node.SelectNodes("./*|./text()");

                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);
                }

                return document.DocumentNode.InnerHtml;
            }
            catch (ArgumentNullException)
            {
                return data;
            }


        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine(logMessage);
        }
    }
}
