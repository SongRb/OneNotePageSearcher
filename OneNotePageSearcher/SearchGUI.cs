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
        List<Tuple<string, string, string, float>> resList;
        OneNoteManager oneNoteManager;

        BackgroundWorker bgWorker;

        UserSettings userSettings;

        public SearchGUI(bool isDebug)
        {
            oneNoteManager = new OneNoteManager(isDebug);
            InitializeComponent();
            InitializeResultGridView();
            InitializeBackgroundWorker();
            userSettings = new UserSettings();
            oneNoteManager.setIndexDirectory(userSettings.indexPath);
        }

        private void InitializeResultGridView()
        {
            resultTable.Columns.Add(new DataColumn("Content", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Score", typeof(float)));
            resultTable.Columns.Add(new DataColumn("Title", typeof(string)));
            resultGridView.DataSource = resultTable;
            resultGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void InitializeBackgroundWorker()
        {
            bgWorker = new BackgroundWorker();

            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            bgWorker.DoWork += DoWork;
            bgWorker.ProgressChanged += ProgressChanged;
            bgWorker.RunWorkerCompleted += WorkerCompleted;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Full text search index successfully built!");
            bgWorker.CancelAsync();
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            indexProgressBar.Value = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    oneNoteManager.setIndexDirectory(this.userSettings.indexPath);
                    oneNoteManager.BuildIndex(this.userSettings.useCache, this.userSettings.indexMode);
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
                        double tmp = oneNoteManager.progressRate + 0.0001;
                        int i = (int)(tmp * 100);

                        if (i == 100) break;

                        var eta = (watch.ElapsedMilliseconds - elapsedMs) * (1 - tmp) / tmp;
                        if (!oneNoteManager.isIndexing) elapsedMs = watch.ElapsedMilliseconds;
                        progressLabel.Invoke((MethodInvoker)(() =>
                        {
                            if (oneNoteManager.isIndexing) progressLabel.Text = "Adding " + oneNoteManager.currentPageTitle;
                            else progressLabel.Text = "Purging caches";
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
                }
            ));
            backgroundThread.Start();
            guiThread.Start();
            backgroundThread.Join();
            guiThread.Join();
            backgroundWorker.ReportProgress(100);
        }

        private void ResultGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!oneNoteManager.OpenPage(resList[e.RowIndex].Item1, resList[e.RowIndex].Item2))
            {
                MessageBox.Show("Requested page not found, please reindex the page!");
            }
        }

        void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tuple = (Tuple<string, string, string, double>)e.Node.Tag;
            if (!oneNoteManager.OpenPage(tuple.Item1, tuple.Item2 ?? "NULL"))
            {
                MessageBox.Show("Requested page not found, please reindex the page!");
            }
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            progressLabel.Hide();
            etaLabel.Hide();
            Console.WriteLine("Query: " + queryBox.Text);
            resList = oneNoteManager.Search(queryBox.Text);
            if (userSettings.viewMode == GlobalVar.TreeViewMode)
            {
                AddResultToTree();
            }
            else
            {
                AddResultToTable();
            }
        }

        private void AddResultToTable()
        {
            resultGridView.Show();
            treeView1.Hide();
            resultTable.Rows.Clear();
            for (var i = 0; i < resList.Count; i++)
            {
                resultTable.Rows.Add(resList[i].Item3.Substring(0, Math.Min(10, resList[i].Item3.Length)), resList[i].Item4, oneNoteManager.GetPageTitle(resList[i].Item1));
            }
            resultGridView.Show();
        }

        private void AddResultToTree()
        {
            Dictionary<string, List<Tuple<string, string, double>>> tree = new Dictionary<string, List<Tuple<string, string, double>>>();
            for (var i = 0; i < resList.Count; i++)
            {
                if(!tree.ContainsKey(resList[i].Item1)) tree[resList[i].Item1] = new List<Tuple<string, string, double>>();
                tree[resList[i].Item1].Add(new Tuple<string, string, double>(resList[i].Item2, resList[i].Item3, resList[4].Item4));
            }
            treeView1.Show();
            foreach(var k in tree)
            {
                List<TreeNode> lst = new List<TreeNode>();
                TreeNode tn;
                foreach (var t in k.Value)
                {
                    tn = new TreeNode(t.Item2.Substring(0, Math.Min(t.Item2.Length, 30))+"...");
                    tn.Tag = new Tuple<string, string, string, double>(k.Key, t.Item1, t.Item2, t.Item3);
                    lst.Add(tn);
                }
                tn = new TreeNode(oneNoteManager.GetPageTitle(k.Key), lst.ToArray());
                tn.Tag = new Tuple<string, string, string, double>(k.Key, null, null, 0);
                treeView1.Nodes.Add(tn);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            bool isDebug = false;
            if (args.Length == 1) isDebug = true;
            Application.Run(new SearchGUI(isDebug));
        }

        private void IndexButtonClick(object sender, EventArgs e)
        {
            resultGridView.Hide();
            treeView1.Hide();
            indexProgressBar.Show();
            etaLabel.Show();
            progressLabel.Show();
            bgWorker.RunWorkerAsync();
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

        private void optionButton_Click(object sender, EventArgs e)
        {
            userSettings.Owner = this;
            userSettings.Show();
            userSettings = new UserSettings();
        }
    }


    public static class GlobalVar
    {
        public const string TreeViewMode = "tree";
        public const string ListViewMode = "list";
        public const string IndexByPageMode = "page";
        public const string IndexByParagraphMode = "paragraph";
    }
}

