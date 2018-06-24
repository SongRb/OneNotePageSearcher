using System;
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
        List<Tuple<string, string, float>> resList;
        OneNoteManager oneNotePageIndexer;

        BackgroundWorker backgroundWorker;

        public SearchGUI(bool isDebug)
        {
            oneNotePageIndexer = new OneNoteManager(isDebug);
            InitializeComponent();
            InitializeResultGridView();
            InitializeBackgroundWorker();
        }

        private void InitializeResultGridView()
        {
            resultTable.Columns.Add(new DataColumn("Content", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Score", typeof(float)));
            resultTable.Columns.Add(new DataColumn("Source", typeof(string)));
            resultGridView.DataSource = resultTable;
            resultGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            return;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            indexProgressBar.Value = e.ProgressPercentage;
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

                    var elapsedMs = watch.ElapsedMilliseconds;
                    while (true)
                    {
                        Thread.Sleep(100);
                        double tmp = oneNotePageIndexer.progressRate + 0.0001;
                        int i = (int)(tmp * 100);

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

                        backgroundWorker.ReportProgress(i);

                        if (backgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            backgroundWorker.ReportProgress(0);
                            return;
                        }
                    }
                    watch.Stop();
                    progressLabel.Invoke((MethodInvoker)(() =>
                    {
                        progressLabel.Text = "Finshed!";
                    }));
                    etaLabel.Invoke((MethodInvoker)(() =>
                    {
                        etaLabel.Text = "";
                    }));
                    indexProgressBar.Invoke((MethodInvoker)(() =>
                    {
                        indexProgressBar.Hide();
                    }));
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

        private void ResultGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            oneNotePageIndexer.OpenPage(resList[e.RowIndex].Item1);
        }

        public void CreateNewRow()
        {
            resultTable.Rows.Add(1, "2", "3", 4, "5");
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            progressLabel.Visible = false;
            etaLabel.Visible = false;
            Console.WriteLine("Query: "+queryBox.Text);
            resList = oneNotePageIndexer.Search(queryBox.Text);

            resultTable.Rows.Clear();
            for (var i = 0; i < 20 && i < resList.Count; i++)
            {
                // TODO Add Page Title into index
                resultTable.Rows.Add(resList[i].Item2.Substring(0,Math.Min(10, resList[i].Item2.Length)), resList[i].Item3, oneNotePageIndexer.GetPageTitle(resList[i].Item1));
            }
            resultGridView.Show();
        }

        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            bool isDebug = false;
            if(args.Length==1) isDebug = true;
            Application.Run(new SearchGUI(isDebug));
        }

        private void IndexButtonClick(object sender, EventArgs e)
        {
            indexProgressBar.Show();
            etaLabel.Show();
            progressLabel.Show();
            backgroundWorker.RunWorkerAsync();
            progressLabel.Text = "Finished";
            etaLabel.Text = "";
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will close down the whole application. Confirm?", "Close Application", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void QueryBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Do something
                SearchButtonClick(sender, e);
            }
        }
    }
}

