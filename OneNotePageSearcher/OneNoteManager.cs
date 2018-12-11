using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.OneNote;
using System.IO;
using System.Text;

namespace OneNotePageSearcher
{
    internal class OneNoteManager
    {
        private static readonly XNamespace One = "http://schemas.microsoft.com/office/onenote/2013/onenote";

        private NetLuceneProvider lucene;

        private Application oneNote = new Application();

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
            oneNote.GetHierarchy(sampleNotebookUrl, HierarchyScope.hsPages, out outputXML);
            pageList = XDocument.Parse(outputXML).Descendants(One + "Page");
        }

        /// <summary>
        /// Add or update index in lucene.
        /// </summary>
        /// <param name="useCache">Indicate use previous cache or do a clean index.</param>
        public void BuildIndex(bool useCache, string indexMode)
        {
            progressRate = 0;
            string outputXML;
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            pageList = XDocument.Parse(outputXML).Descendants(One + "Page");
            lucene.SetWorkingDirectory();
            if (useCache)
            {
                AddIndexByTime(indexMode);
            }
            else
            {
                AddAllIndex(indexMode);
            }
            lucene.CloseWriter();
        }

        /// <summary>
        /// Just update lucene index.
        /// </summary>
        private void AddIndexByTime(string indexMode)
        {
            lucene.SetUpWriter(false);
            GetUpdateIndexID(out HashSet<string> deletedID, out HashSet<string> updateID);
            if (isDebug) Console.WriteLine("Begining Index Process...");
            foreach (var id in deletedID)
            {
                if (isDebug) Console.WriteLine("Deleting: " + id);
                lucene.DeleteDocumentByID(id);
            }
            isIndexing = true;
            AddIndexFromID(updateID, indexMode);
            isIndexing = false;
            var currentTime = String.Format("{0:u}", DateTime.UtcNow);
            UserSettings.AddUpdateAppSettings("LastIndexTime", currentTime);
        }

        /// <summary>
        /// Add all index and invalidates cache.
        /// </summary>
        private void AddAllIndex(string indexMode)
        {
            lucene.SetUpWriter(true);
            HashSet<String> updateID = new HashSet<string>();
            foreach (var n in pageList)
            {
                updateID.Add(n.Attribute("ID").Value);
            }
            AddIndexFromID(updateID, indexMode);
        }

        /// <summary>
        /// Update/Create current lucene index by paragraph id. Due to out-of-date
        /// </summary>
        /// <param name="updateID">An iterable contains all paragraph id should be updated</param>
        private void AddIndexFromID(HashSet<String> updateID, string indexMode)
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
                        if (indexMode == GlobalVar.IndexByParagraphMode) IndexByParagraph(id);
                        else IndexByDocument(id);
                    }
                    catch (System.Runtime.InteropServices.COMException e)
                    {
                        int code = e.HResult;
                        if (isDebug) Console.WriteLine("Exception: Error Code is " + code);
                    }
                }
            }
        }

        /// <summary>
        /// Create index by content of paragraph in page.
        /// </summary>
        /// <param name="pageID"></param>
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

        /// <summary>
        /// Create index by whole content in page.
        /// </summary>
        /// <param name="pageID"></param>
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

        public List<SearchResult> Search(string query)
        {
            return lucene.Search(query);
        }

        /// <summary>
        /// Send page id(from index), open requested page.
        /// </summary>
        /// <param name="id"></param>
        public bool OpenPage(string pageID, string paraID = "NULL")
        {
            try
            {
                if (paraID == "NULL")
                {
                    oneNote.NavigateTo(pageID);
                }
                else
                {
                    oneNote.NavigateTo(pageID, paraID);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send page id(from index), get page title.
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns>page title</returns>
        public string GetPageTitle(string pageId)
        {
            IEnumerable<XElement> pages =
                (from el in pageList
                 where el.Attribute("ID").Value == pageId
                 select el);
            if (pages.Any()) return pages.First().Attribute("name").Value;
            else return "Default Title";
        }

        /// <summary>
        /// Get a list of page id from current onenote content.
        /// </summary>
        /// <returns></returns>
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
            // DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffK")
            string oldTime = UserSettings.ReadSetting("LastIndexTime") ?? lucene.GetLastUpdatedTime();
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

        public void setIndexDirectory(string indexDir)
        {
            this.lucene._indexPath = indexDir;
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

            data = data.Replace("&nbsp;", " ");
            var document = new HtmlDocument();
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
