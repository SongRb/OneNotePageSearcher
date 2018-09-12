using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;


namespace OneNotePageSearcher
{
    internal class NetLuceneProvider
    {
        public string _indexPath;

        private readonly string _indexByDocumentPath = "LuceneIndex";

        private int _maxDoc;
        private readonly Analyzer _analyzer;

        private IndexWriter writer;

        private int _searchLimit = 1000;

        private Directory indexDirectory = null;

        public bool debug;

        public NetLuceneProvider(bool overwrite)
        {
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
        }

        public void SetWorkingDirectory()
        {
            indexDirectory = FSDirectory.Open(_indexPath);
        }

        public void SetUpReader()
        {
            SetWorkingDirectory();
            var indexReader = IndexReader.Open(indexDirectory, true);
            _maxDoc = indexReader.MaxDoc;
            if (debug) Console.WriteLine(_maxDoc);
            indexReader.Dispose();
            indexDirectory.Dispose();
        }

        public void SetUpWriter(bool overwrite = true)
        {
            SetWorkingDirectory();
            try
            {
                writer = new IndexWriter(indexDirectory, _analyzer, overwrite, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            catch (Exception e)
            {
                writer = new IndexWriter(indexDirectory, _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        public void CloseWriter()
        {
            writer.Optimize();
            writer.Flush(true, true, true);
            writer.Dispose();
            indexDirectory.Dispose();
        }

        public void AddDocumentList(List<Tuple<string, string, string>> documentList)
        {
            foreach (var docPair in documentList)
            {
                AddTextToIndex(docPair.Item1, docPair.Item2, docPair.Item3);
            }
        }

        public void AddDocument(Tuple<string, string, string> doc)
        {
            AddTextToIndex(doc.Item1, doc.Item2, doc.Item3);
        }

        public void DeleteDocumentByID(string id)
        {
            writer.DeleteDocuments(new Term("pageID", id));
        }

        public HashSet<String> GetAllValuesByField(string field_name)
        {
            IndexReader reader = IndexReader.Open(indexDirectory, true);
            TermEnum terms = reader.Terms();
            HashSet<String> uniqueTerms = new HashSet<String>();
            while (terms.Next())
            {
                var term = terms.Term;
                if (term.Field.Equals(field_name))
                {
                    uniqueTerms.Add(term.Text);
                }
            }
            return uniqueTerms;
        }

        private void Main(string[] args)
        {
            var lucene = new NetLuceneProvider(true);
            lucene.SetWorkingDirectory();
            if (debug) Console.WriteLine(String.Join(",", lucene.GetAllValuesByField("id")));
            if (debug) Console.Read();
        }

        public List<Tuple<string, string, string, float>> Search(string q)
        {
            SetWorkingDirectory();
            var resultList = new List<Tuple<string, string, string, float>>();
            try
            {
                var searcher = new IndexSearcher(indexDirectory);
                var parser = new QueryParser(Version.LUCENE_30, "postBody", _analyzer);

                var query = parser.Parse(q);
                var hits = searcher.Search(query, _searchLimit);

                if (debug) Console.WriteLine(hits.TotalHits);

                var results = hits.TotalHits;

                if (debug) Console.WriteLine("Found {0} results", results);
                for (var i = 0; i < results && i < _searchLimit; i++)
                {
                    var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                    var score = hits.ScoreDocs[i].Score;

                    resultList.Add(new Tuple<string, string, string, float>(
                        doc.Get("pageID"), doc.Get("paraID"), doc.Get("postBody"), score
                        ));
                }

                //Clean up everything
                searcher.Dispose();
                indexDirectory.Dispose();
            }
            catch (Exception e)
            {

            }
            return resultList;
        }

        private void AddTextToIndex(string pageID, string paraID, string text)
        {
            var doc = new Document();
            doc.Add(new Field("pageID", pageID, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("paraID", paraID, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("postBody", text, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
        }
    }
}


