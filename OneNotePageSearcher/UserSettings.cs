using System;
using System.Windows.Forms;
using System.Configuration;

namespace OneNotePageSearcher
{
    public partial class UserSettings : Form
    {

        private string viewMode;

        public UserSettings()
        {
            InitializeComponent();
            Shown += disableParentForm;
            FormClosed += enableParentForm;
            initSettings();
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

        private void initSettings()
        {
            this.viewMode = ReadSetting("view_mode") ?? "tree";
            if (this.viewMode == "tree") this.treeViewRatioButton.Checked = true;
            else this.listViewRatioButton.Checked = true;
        }

        private void confirmSaveSettingsBtn_Click(object sender, EventArgs e)
        {
            if (this.treeViewRatioButton.Checked) this.viewMode = "tree";
            else this.viewMode = "table";

            AddUpdateAppSettings("view_mode", viewMode);
            this.Owner.Enabled = true;
            this.Close();
        }
    }
}
