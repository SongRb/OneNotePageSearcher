﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace OneNotePageSearcher
{
    public partial class SearchGUI : Form
    {
        DataTable resultTable = new DataTable();
        NetLuceneProvider lucene = new NetLuceneProvider();
        List<Tuple<string, string, float>> resList;
        OneNotePageIndexer oneNotePageIndexer = new OneNotePageIndexer();

        BackgroundWorker backgroundWorker;

        public SearchGUI()
        {
            //oneNotePageIndexer.Main();
            InitializeComponent();
            InitializeResultGridView();
            InitializeBackgroundWorker();
        }

        private void InitializeResultGridView()
        {
            resultTable.Columns.Add(new DataColumn("Content", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Score", typeof(float)));
            resultTable.Columns.Add(new DataColumn("Source", typeof(string)));
            ResultGridView.DataSource = resultTable;
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();

            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            backgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler
                    (ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (WorkerCompleted);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("HHHHHHHHH");
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    oneNotePageIndexer.BuildIndex();
                }

            ));
            Thread guiThread = new Thread(
                new ThreadStart(() =>
                {

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    // the code that you want to measure comes here

                    var elapsedMs = watch.ElapsedMilliseconds;
                    while (true)
                    {
                        Thread.Sleep(2000);
                        //int i = (int)oneNotePageIndexer.progressRate * 1000;
                        double tmp = oneNotePageIndexer.progressRate + 0.0001;
                        int i = (int)(tmp * 100);
                        //MessageBox.Show(i.ToString());
                        if (i == 100) break;

                        var eta = (watch.ElapsedMilliseconds - elapsedMs) * (1 - tmp) / tmp;

                        progressLabel.Invoke((MethodInvoker)(() =>
                        {
                            progressLabel.Text = "Adding " + oneNotePageIndexer.currentPageTitle;
                        }));

                        etaLabel.Invoke((MethodInvoker)(() =>
                        {
                            etaLabel.Text = "Remaining Time: " + Math.Round(eta / 1000, 2, MidpointRounding.AwayFromZero) + "s";
                        }));


                        // Periodically report progress to the main thread so that it can
                        // update the UI.  In most cases you'll just need to send an
                        // integer that will update a ProgressBar
                        backgroundWorker.ReportProgress(i);
                        // Periodically check if a cancellation request is pending.
                        // If the user clicks cancel the line
                        // m_AsyncWorker.CancelAsync(); if ran above.  This
                        // sets the CancellationPending to true.
                        // You must check this flag in here and react to it.
                        // We react to it by setting e.Cancel to true and leaving
                        if (backgroundWorker.CancellationPending)
                        {
                            // Set the e.Cancel flag so that the WorkerCompleted event
                            // knows that the process was cancelled.
                            e.Cancel = true;
                            backgroundWorker.ReportProgress(0);
                            return;
                        }
                    }
                    watch.Stop();

                    MessageBox.Show("Full text search index successfully built!");
                }
            ));
            backgroundThread.Start();
            guiThread.Start();
            backgroundThread.Join();
            guiThread.Join();
            //Report 100% completion on operation completed
            backgroundWorker.ReportProgress(100);
        }

        private void ResultGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            oneNotePageIndexer.OpenPage(resList[e.RowIndex].Item1);
        }

        public void CreateNewRow()
        {
            resultTable.Rows.Add(1, "2", "3", 4, "5");
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            resList = lucene.Search(queryBox.Text);
            resultTable.Rows.Clear();
            for (var i = 0; i < 20 && i < resList.Count; i++)
            {
                // TODO Add Page Title into index
                resultTable.Rows.Add(resList[i].Item2, resList[i].Item3, oneNotePageIndexer.GetPageTitle(resList[i].Item1));
            }
            ResultGridView.Show();
        }

        public static void Main(string[] args)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(new SearchGUI());
        }

        private void IndexButtonClick(object sender, EventArgs e)
        {
            progressBar1.Show();
            etaLabel.Show();
            progressLabel.Show();
            backgroundWorker.RunWorkerAsync();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
