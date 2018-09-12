﻿using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OneNotePageSearcher
{
    public partial class UserSettings : Form
    {
        public string viewMode;
        public string indexPath;
        public bool useCache;
        public string indexMode;

        public UserSettings()
        {
            InitializeComponent();
            this.MaximumSize = new Size(this.Width * 2, this.Height);
            this.MinimumSize = new Size(this.Width, this.Height);
            Shown += disableParentForm;
            FormClosed += enableParentForm;
            reloadSettings();
        }

        private void enableParentForm(object sender, EventArgs e)
        {
            this.Owner.Enabled = true;
        }

        private void disableParentForm(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Owner.Enabled = false;
        }

        public static string ReadSetting(string key)
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

        public static void AddUpdateAppSettings(string key, string value)
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

        public void reloadSettings()
        {
            this.viewMode = ReadSetting("view_mode") ?? "tree";
            if (this.viewMode == "tree") this.treeViewRatioButton.Checked = true;
            else this.listViewRatioButton.Checked = true;

            this.indexPath = ReadSetting("index_path") ??  Path.GetFullPath(Path.GetDirectoryName(Application.ExecutablePath)+ "\\LuceneIndex");
            Directory.CreateDirectory(this.indexPath);
            this.currentIndexPathLabel.Text = "Index path:\n" + this.indexPath;

            var useCacheStr = ReadSetting("use_cache") ?? "true";

            if (useCacheStr == "true")
            {
                this.useCache = true;
                this.idxTimeBtn.Checked = true;
            }
            else
            {
                this.useCache = false;
                this.idxTimeBtn.Checked = false;
            }
            this.indexMode = ReadSetting("index_mode") ?? GlobalVar.IndexByParagraphMode;
            if (this.indexMode == GlobalVar.IndexByParagraphMode) this.idxParaBtn.Checked = true;
            else this.idxPageBtn.Checked = true;
        }

        private void confirmSaveSettingsBtn_Click(object sender, EventArgs e)
        {
            if (this.treeViewRatioButton.Checked) this.viewMode = GlobalVar.TreeViewMode;
            else this.viewMode = GlobalVar.ListViewMode;

            AddUpdateAppSettings("view_mode", viewMode);
            this.Owner.Enabled = true;
            this.Close();
        }

        private void indexDirBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.indexPath = folderBrowserDialog1.SelectedPath;
                currentIndexPathLabel.Text = "Local index path:\n"+folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
