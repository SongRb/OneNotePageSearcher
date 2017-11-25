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

        private Microsoft.Office.Interop.OneNote.Application oneNote = new Microsoft.Office.Interop.OneNote.Application();

        public double progressRate=0;
        double count = 0;
        double totalCount = 0;

        public bool canceled = false;

        public string currentPageTitle = "";

        public XDocument allPageInfo;

        public OneNoteManager()
        {
            lucene = new NetLuceneProvider(false);

            string outputXML;
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            allPageInfo = XDocument.Parse(outputXML);
        }

        /// <summary>
        /// Take a xml string which follows the standard of 
        /// </summary>
        /// <param name="outputXML"></param>
        public void BuildIndex(string outputXML)
        {
            var info = XDocument.Parse(outputXML);
            var pageList = info.Descendants(One + "Page");

            AddIndex(pageList);
        }

        public void BuildIndexByTime()
        {
            HashSet<String> deletedID;
            HashSet<String> updatedID;
            GetUpdateIndexID(out deletedID, out updatedID);
            Console.WriteLine("Begining Index Process...");
            foreach(var id in deletedID)
            {
                Console.WriteLine("Deleting: " + id);
                lucene.DeleteDocumentByID(id);
            }

            foreach (var id in updatedID)
            {
                Console.WriteLine("Adding: " + id);
                lucene.DeleteDocumentByID(id);
                string xmlString;
                try
                {
                    oneNote.GetPageContent(id, out xmlString);
                    var doc = XDocument.Parse(xmlString);
                    IndexByDocument(id, doc);
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    int code = e.HResult;
                    Console.WriteLine("Exception: Error Code is " + code);
                }

            }





        }

        private void AddIndex(IEnumerable<XElement> pageList)
        {
            totalCount = pageList.Count();
            Console.WriteLine("All Count: " + totalCount);

            foreach (var n in pageList)
            {
                count += 1;
                progressRate = count / totalCount;
                currentPageTitle = n.Attribute("name").Value;
                //string title = currentPageTitle;
                Console.Write("Adding " + currentPageTitle);
                var pageID = n.Attribute("ID").Value;
                Console.WriteLine(pageID);
                string xmlString;
                try
                {
                    oneNote.GetPageContent(pageID, out xmlString);
                    var doc = XDocument.Parse(xmlString);
                    //IndexByParagraph(pageID, doc);
                    IndexByDocument(pageID, doc);
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    int code = e.HResult;
                    Console.WriteLine("Exception: Error Code is " + code);
                }

            }
        }

        private void IndexByParagraph(string pageID, XDocument doc)
        {
            var des = doc.Descendants(One + "OE");
            Console.WriteLine("\t" + des.Count() + " Paragraphs");

            var documentList = new List<Tuple<string, string>>();
            // Index title as a paragraph
            int paragraphCount = des.Count();
            if(paragraphCount>0)
            {
                foreach (var el in des)
                {
                    var parId = el.Attribute("objectID");
                    var text = el.Element(One + "T");

                    if (parId == null || text == null) continue;
                    var id = pageID + " " + parId.Value;
                    var par = RemoveUnwantedTags(text.Value);
                    //Console.WriteLine(id + "\t" + par + "\n");
                    documentList.Add(new Tuple<string, string>(id, par));
                }
            }

            lucene.AddDocumentList(documentList);
        }

        private string GetTextFromNode()
        {
            throw new NotImplementedException();
        }

        private void IndexByDocument(string pageID, XDocument doc)
        {
            var des = doc.Descendants(One + "OE");
            StringBuilder sb = new StringBuilder("");

            foreach (var el in des)
            {
                var textNode = el.Element(One + "T");
                string text;
                if (textNode == null)
                {
                    var image = el.Element(One + "Image");
                    if (image == null) continue;
                    var altAttr = image.Attribute("alt");
                    if (altAttr == null) continue;
                    text = altAttr.Value;
                    //Console.WriteLine(text);
                }
                else
                {
                    text = textNode.Value;
                }
                sb.AppendLine(text);
            }
            lucene.AddDocument(new Tuple<string,string>(pageID, RemoveUnwantedTags(sb.ToString())));
        }

        public void BuildIndex()
        {
            progressRate = 0;
            string outputXML;
            oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            lucene.SetWorkingDirectory();
            lucene.SetUpWriter(false);
            //outputXML = System.IO.File.ReadAllText(@"D:\\Sample.xml");
            //BuildIndex(outputXML);
            BuildIndexByTime();
            lucene.CloseWriter();
        }

        public List<Tuple<string, string, float>> Search(string query)
        {
            return lucene.Search(query);
        }

        /// <summary>
        /// Send id in index, open requested page.
        /// </summary>
        /// <param name="id"></param>
        public void OpenPage(string id)
        {
            var idList = id.Split(' ');

            if(idList.Length==1)
            {
                oneNote.NavigateTo(idList[0]);
            }
            else
            {
                oneNote.NavigateTo(idList[0], idList[1]);
            }

        }

        /// <summary>
        /// Send id in index, return requested page title
        /// </summary>
        public string GetPageTitle(string id)
        {
            var pageId = id.Split(' ')[0];
            IEnumerable<XElement> pages=
                (from el in allPageInfo.Descendants(One+"Page")
                 where el.Attribute("ID").Value == pageId
                 select el);
            return pages.First().Attribute("name").Value;
        }

        public HashSet<String> GetAllPageId()
        {
            var pageList = allPageInfo.Descendants(One + "Page");
            var idSet = new HashSet<String>();
            foreach(var n in pageList)
            {
                idSet.Add(n.Attribute("ID").Value);
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
            var legacyID = lucene.GetAllValuesByField("id");
            var legacyPageID = new HashSet<String>();
            indexIDToDelete = new HashSet<String>();
            indexIDToCreate = new HashSet<String>();


            var pageList = allPageInfo.Descendants(One + "page");

            foreach(var n in legacyID)
            {
                legacyPageID.Add(n.Split(' ')[0]);
            }
            // We want to find page that is already deleted
            foreach (var id in legacyID)
            {
                if(!currentID.Contains(id))
                {
                    indexIDToDelete.Add(id);
                }
            }

            // We also want to find page that is updated and created
            string oldTime= "2017-11-18T08:56:47.000Z";
            //var indexIDToCreate = new HashSet<String>();
            foreach (var n in pageList)
            {
                // Last Modified Time older than index time
                if(CompareTimeByString(n.Attribute("lastModifiedTime").Value, oldTime))
                {
                    indexIDToCreate.Add(n.Attribute("ID").Value);
                }
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
    }
}
