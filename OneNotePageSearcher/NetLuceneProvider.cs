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
        private readonly string _indexPath = "LuceneIndex";
        private int _maxDoc;
        private readonly Analyzer _analyzer;

        private IndexWriter writer;

        private int _searchLimit = 1000;

        private Directory directory;

        public NetLuceneProvider()
        {
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
        }

        public void SetWorkingDirectory()
        {
            directory = FSDirectory.Open(_indexPath);
        }

        public void SetUpIndexer()
        {
            var indexReader = IndexReader.Open(directory, true);
            _maxDoc = indexReader.MaxDoc;
            Console.WriteLine(_maxDoc);
            indexReader.Dispose();
            directory.Dispose();

        }

        public void SetUpWriter()
        {
            writer = new IndexWriter(directory, _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
        }


        public void CloseWriter()
        {
            writer.Optimize();
            //Close the writer
            writer.Flush(true, true, true);
            writer.Dispose();
            directory.Dispose();
        }

        public void AddDocument(List<Tuple<string, string>> documentList)
        {
            Directory directory = FSDirectory.Open(_indexPath);

            foreach (var docPair in documentList)
            {
                AddTextToIndex(docPair.Item1, docPair.Item2);
            }


        }

        //private static void Main(string[] args)
        //{
        //    var netLuceneProvider = new NetLuceneProvider();
        //    var docList = new List<Tuple<string, string>>();

        //    docList.Add(new Tuple<string, string>("1", "科学"));
        //    docList.Add(new Tuple<string, string>("2", "中国科学院"));
        //    docList.Add(new Tuple<string, string>("3", "国科大"));
        //    docList.Add(new Tuple<string, string>("4", "科技是第一发展力"));
        //    docList.Add(new Tuple<string, string>("5", "语文和数学科目"));
        //    docList.Add(new Tuple<string, string>("6", "特搜科"));

        //    netLuceneProvider.SetUpIndexer();
        //    netLuceneProvider.AddDocument(docList);

        //    var result = netLuceneProvider.Search("科");
        //    foreach (var resTuple in result)
        //    {
        //        Console.WriteLine(resTuple.ToString());
        //    }


        //    Console.ReadLine();
        //}

        public List<Tuple<string, string, float>> Search(string q)
        {
            //Setup searcher
            Directory directory = FSDirectory.Open(_indexPath);
            var searcher = new IndexSearcher(directory);
            var parser = new QueryParser(Version.LUCENE_30, "postBody", _analyzer);

            var query = parser.Parse(q);
            var hits = searcher.Search(query, _searchLimit);

            Console.WriteLine(hits.TotalHits);

            var results = hits.TotalHits;

            var resultList = new List<Tuple<string, string, float>>();

            Console.WriteLine("Found {0} results", results);
            for (var i = 0; i < results && i< _searchLimit; i++)
            {
                var doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                var score = hits.ScoreDocs[i].Score;

                resultList.Add(new Tuple<string, string, float>(
                    doc.Get("id"), doc.Get("postBody"), score
                    ));
            }


            //Clean up everything
            searcher.Dispose();
            directory.Dispose();
            return resultList;
        }



        private void AddTextToIndex(string id, string text)
        {
            var doc = new Document();
            doc.Add(new Field("id", id, Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
            doc.Add(new Field("postBody", text, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
        }
    }
} 

 
