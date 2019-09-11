using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Cn.Smart;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;
using Lucene.Net.QueryParsers.Analyzing;

namespace OneNotePageSearcher
{
    internal class NetLuceneProvider
    {
        public string _indexPath;
        private int _maxDoc;
        private readonly Analyzer _analyzer;

        private IndexWriter writer;

        private int _searchLimit = 1000;

        private Directory indexDirectory = null;

        private LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        public bool debug;

        public NetLuceneProvider(bool overwrite)
        {
            _analyzer = new SmartChineseAnalyzer(AppLuceneVersion, true);
        }

        public void SetWorkingDirectory()
        {
            indexDirectory = FSDirectory.Open(_indexPath);
        }

        public void SetUpReader()
        {
            SetWorkingDirectory();
            var indexReader = DirectoryReader.Open(indexDirectory);
            _maxDoc = indexReader.MaxDoc;
            if (debug) Console.WriteLine(_maxDoc);
            indexReader.Dispose();
            indexDirectory.Dispose();
        }

        public void SetUpWriter(bool overwrite = true)
        {
            SetWorkingDirectory();
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, _analyzer);
            indexConfig.OpenMode = overwrite ? OpenMode.CREATE : OpenMode.CREATE_OR_APPEND;

            writer = new IndexWriter(indexDirectory, indexConfig);
        }

        public void CloseWriter()
        {
            writer.Flush(true, true);
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
            HashSet<String> uniqueTerms = new HashSet<string>();
            if (DirectoryReader.IndexExists(indexDirectory))
            {
                var reader = DirectoryReader.Open(indexDirectory);
                for (int i = 0; i < reader.MaxDoc; i++)
                {
                    HashSet<string> field_names = new HashSet<string>();
                    field_names.Add(field_name);
                    Document doc = reader.Document(i, field_names);
                    String field_value = doc.Get(field_name);
                    uniqueTerms.Add(field_value);

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

        public List<SearchResult> Search(string q)
        {
            SetWorkingDirectory();
            var resultList = new List<SearchResult>();
            var indexReader = DirectoryReader.Open(indexDirectory);
            var searcher = new IndexSearcher(indexReader);
            var parser = new AnalyzingQueryParser(AppLuceneVersion, "postBody", _analyzer);

            var query = parser.Parse(q);
            var hits = searcher.Search(query, _searchLimit);

            if (debug) Console.WriteLine(hits.TotalHits);

            var results = hits.TotalHits;

            if (debug) Console.WriteLine("Found {0} results", results);
            for (var i = 0; i < results && i < _searchLimit; i++)
            {
                var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                var score = hits.ScoreDocs[i].Score;

                resultList.Add(
                    new SearchResult(
                        doc.Get("pageID"), doc.Get("paraID"), doc.Get("postBody"), score
                    )
                );
            }

            //Clean up everything
            //searcher.Dispose();
            indexDirectory.Dispose();

            return resultList;
        }

        private void AddTextToIndex(string pageID, string paraID, string text)
        {
            var doc = new Document();
            doc.Add(new StringField("pageID", pageID, Field.Store.YES));
            doc.Add(new StringField("paraID", paraID, Field.Store.YES));
            doc.Add(new TextField("postBody", text, Field.Store.YES));
            writer.AddDocument(doc);
        }

        public string GetLastUpdatedTime()
        {
            var fileList = new DirectoryInfo(_indexPath).GetFiles();
            return fileList.Any() ? fileList.Min(file => file.LastWriteTime).ToString("yyyy-MM-ddTHH:mm:ss.fffK") : "1978-06-18T08:56:47.000Z";
        }
    }
}


