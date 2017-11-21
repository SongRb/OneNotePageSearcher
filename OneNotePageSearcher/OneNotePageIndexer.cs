using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.Office.Interop.OneNote;
using System.IO;

namespace OneNotePageSearcher
{
    internal class OneNotePageIndexer
    {
        private static readonly XNamespace One = "http://schemas.microsoft.com/office/onenote/2013/onenote";

        private NetLuceneProvider lucene;

        private Microsoft.Office.Interop.OneNote.Application oneNote = new Microsoft.Office.Interop.OneNote.Application();

        public double progressRate=0;

        public bool canceled = false;

        public string currentPageTitle = "";

        public XDocument allPageInfo;

        public OneNotePageIndexer()
        {
            lucene = new NetLuceneProvider();

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

            int totalCount = pageList.Count();
            Console.WriteLine("All Count"+totalCount);
            double count = 0;
            foreach (var n in pageList)
            {
                count += 1;
                progressRate = count / totalCount;
                currentPageTitle = n.Attribute("name").Value;
                Console.Write("Adding " + currentPageTitle);
                var pageID = n.Attribute("ID").Value;
                String xmlString;
                oneNote.GetPageContent(pageID, out xmlString);
                var doc = XDocument.Parse(xmlString);
                var des = doc.Descendants(One + "OE");
                Console.WriteLine("\t" + des.Count() +" Paragraphs");

                var documentList = new List<Tuple<string, string>>();
                foreach (var el in des)
                {
                    var parId = el.Attribute("objectID");
                    var text = el.Element(One + "T");

                    if (parId == null || text == null || text.Value.Length <= 2) continue;
                    var id = parId.Value + " " + pageID;
                    var par = RemoveUnwantedTags(text.Value);
                    //Console.WriteLine(id + "\t" + par + "\n");
                    documentList.Add(new Tuple<string, string>(id, par));
                }
                lucene.AddDocument(documentList);
            }
        }

        public void BuildIndex()
        {
            progressRate = 0;
            //string outputXML;
            //oneNote.GetHierarchy(null, HierarchyScope.hsPages, out outputXML);
            lucene.SetWorkingDirectory();
            lucene.SetUpWriter();
            string outputXML = System.IO.File.ReadAllText(@"D:\\Sample.xml");
            BuildIndex(outputXML);
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
            var pageId = id.Split(' ')[1];
            var objectId = id.Split(' ')[0];

            oneNote.NavigateTo(pageId, objectId);
        }

        /// <summary>
        /// Send id in index, return requested page title
        /// </summary>
        public string GetPageTitle(string id)
        {
            var pageId = id.Split(' ')[1];
            IEnumerable<XElement> pages=
                (from el in allPageInfo.Descendants(One+"Page")
                 where el.Attribute("ID").Value == pageId
                 select el);
            return pages.First().Attribute("name").Value;
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
