using System.Windows.Forms;

namespace OneNotePageSearcher
{
    partial class UserSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserSettings));
            this.treeViewRatioButton = new System.Windows.Forms.RadioButton();
            this.listViewRatioButton = new System.Windows.Forms.RadioButton();
            this.AppearanceGroup = new System.Windows.Forms.GroupBox();
            this.confirmSaveSettingsBtn = new System.Windows.Forms.Button();
            this.indexGroup = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.idxPageBtn = new System.Windows.Forms.RadioButton();
            this.idxParaBtn = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.idxCleanBtn = new System.Windows.Forms.RadioButton();
            this.idxTimeBtn = new System.Windows.Forms.RadioButton();
            this.currentIndexPathLabel = new System.Windows.Forms.Label();
            this.indexDirBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.AppearanceGroup.SuspendLayout();
            this.indexGroup.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewRatioButton
            // 
            this.treeViewRatioButton.AutoSize = true;
            this.treeViewRatioButton.Location = new System.Drawing.Point(3, 15);
            this.treeViewRatioButton.Name = "treeViewRatioButton";
            this.treeViewRatioButton.Size = new System.Drawing.Size(70, 17);
            this.treeViewRatioButton.TabIndex = 0;
            this.treeViewRatioButton.TabStop = true;
            this.treeViewRatioButton.Text = "TreeView";
            this.treeViewRatioButton.UseVisualStyleBackColor = true;
            // 
            // listViewRatioButton
            // 
            this.listViewRatioButton.AutoSize = true;
            this.listViewRatioButton.Location = new System.Drawing.Point(3, 38);
            this.listViewRatioButton.Name = "listViewRatioButton";
            this.listViewRatioButton.Size = new System.Drawing.Size(64, 17);
            this.listViewRatioButton.TabIndex = 1;
            this.listViewRatioButton.TabStop = true;
            this.listViewRatioButton.Text = "ListView";
            this.listViewRatioButton.UseVisualStyleBackColor = true;
            // 
            // AppearanceGroup
            // 
            this.AppearanceGroup.Controls.Add(this.treeViewRatioButton);
            this.AppearanceGroup.Controls.Add(this.listViewRatioButton);
            this.AppearanceGroup.Location = new System.Drawing.Point(12, 12);
            this.AppearanceGroup.Name = "AppearanceGroup";
            this.AppearanceGroup.Size = new System.Drawing.Size(93, 67);
            this.AppearanceGroup.TabIndex = 2;
            this.AppearanceGroup.TabStop = false;
            this.AppearanceGroup.Text = "Appearance";
            // 
            // confirmSaveSettingsBtn
            // 
            this.confirmSaveSettingsBtn.Location = new System.Drawing.Point(268, 126);
            this.confirmSaveSettingsBtn.Name = "confirmSaveSettingsBtn";
            this.confirmSaveSettingsBtn.Size = new System.Drawing.Size(75, 23);
            this.confirmSaveSettingsBtn.TabIndex = 3;
            this.confirmSaveSettingsBtn.Text = "Save";
            this.confirmSaveSettingsBtn.UseVisualStyleBackColor = true;
            this.confirmSaveSettingsBtn.Click += new System.EventHandler(this.confirmSaveSettingsBtn_Click);
            // 
            // indexGroup
            // 
            this.indexGroup.Controls.Add(this.panel2);
            this.indexGroup.Controls.Add(this.panel1);
            this.indexGroup.Location = new System.Drawing.Point(123, 12);
            this.indexGroup.Name = "indexGroup";
            this.indexGroup.Size = new System.Drawing.Size(220, 67);
            this.indexGroup.TabIndex = 4;
            this.indexGroup.TabStop = false;
            this.indexGroup.Text = "Index";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.idxPageBtn);
            this.panel2.Controls.Add(this.idxParaBtn);
            this.panel2.Location = new System.Drawing.Point(104, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(110, 48);
            this.panel2.TabIndex = 7;
            // 
            // idxPageBtn
            // 
            this.idxPageBtn.AutoSize = true;
            this.idxPageBtn.Location = new System.Drawing.Point(3, 2);
            this.idxPageBtn.Name = "idxPageBtn";
            this.idxPageBtn.Size = new System.Drawing.Size(79, 17);
            this.idxPageBtn.TabIndex = 5;
            this.idxPageBtn.TabStop = true;
            this.idxPageBtn.Text = "Index Page";
            this.idxPageBtn.UseVisualStyleBackColor = true;
            // 
            // idxParaBtn
            // 
            this.idxParaBtn.AutoSize = true;
            this.idxParaBtn.Location = new System.Drawing.Point(3, 25);
            this.idxParaBtn.Name = "idxParaBtn";
            this.idxParaBtn.Size = new System.Drawing.Size(103, 17);
            this.idxParaBtn.TabIndex = 6;
            this.idxParaBtn.TabStop = true;
            this.idxParaBtn.Text = "Index Paragraph";
            this.idxParaBtn.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.idxCleanBtn);
            this.panel1.Controls.Add(this.idxTimeBtn);
            this.panel1.Location = new System.Drawing.Point(6, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(92, 48);
            this.panel1.TabIndex = 0;
            // 
            // idxCleanBtn
            // 
            this.idxCleanBtn.AutoSize = true;
            this.idxCleanBtn.Location = new System.Drawing.Point(3, 25);
            this.idxCleanBtn.Name = "idxCleanBtn";
            this.idxCleanBtn.Size = new System.Drawing.Size(81, 17);
            this.idxCleanBtn.TabIndex = 1;
            this.idxCleanBtn.TabStop = true;
            this.idxCleanBtn.Text = "Clean Index";
            this.idxCleanBtn.UseVisualStyleBackColor = true;
            // 
            // idxTimeBtn
            // 
            this.idxTimeBtn.AutoSize = true;
            this.idxTimeBtn.Location = new System.Drawing.Point(3, 2);
            this.idxTimeBtn.Name = "idxTimeBtn";
            this.idxTimeBtn.Size = new System.Drawing.Size(74, 17);
            this.idxTimeBtn.TabIndex = 0;
            this.idxTimeBtn.TabStop = true;
            this.idxTimeBtn.Text = "Fast Index";
            this.idxTimeBtn.UseVisualStyleBackColor = true;
            // 
            // currentIndexPathLabel
            // 
            this.currentIndexPathLabel.AutoSize = true;
            this.currentIndexPathLabel.Location = new System.Drawing.Point(15, 95);
            this.currentIndexPathLabel.Name = "currentIndexPathLabel";
            this.currentIndexPathLabel.Size = new System.Drawing.Size(0, 13);
            this.currentIndexPathLabel.TabIndex = 8;
            this.currentIndexPathLabel.MouseHover += new System.EventHandler(this.OnMouseHover);
            this.currentIndexPathLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.currentIndexPathLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // indexDirBtn
            // 
            this.indexDirBtn.Location = new System.Drawing.Point(12, 126);
            this.indexDirBtn.Name = "indexDirBtn";
            this.indexDirBtn.Size = new System.Drawing.Size(75, 23);
            this.indexDirBtn.TabIndex = 9;
            this.indexDirBtn.Text = "Select";
            this.indexDirBtn.UseVisualStyleBackColor = true;
            this.indexDirBtn.Click += new System.EventHandler(this.indexDirBtn_Click);
            // 
            // UserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 161);
            this.Controls.Add(this.currentIndexPathLabel);
            this.Controls.Add(this.indexDirBtn);
            this.Controls.Add(this.indexGroup);
            this.Controls.Add(this.confirmSaveSettingsBtn);
            this.Controls.Add(this.AppearanceGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserSettings";
            this.Text = "UserSettings";
            this.AppearanceGroup.ResumeLayout(false);
            this.AppearanceGroup.PerformLayout();
            this.indexGroup.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton treeViewRatioButton;
        private System.Windows.Forms.RadioButton listViewRatioButton;
        private System.Windows.Forms.GroupBox AppearanceGroup;
        private System.Windows.Forms.Button confirmSaveSettingsBtn;
        private System.Windows.Forms.GroupBox indexGroup;
        private System.Windows.Forms.RadioButton idxPageBtn;
        private System.Windows.Forms.RadioButton idxParaBtn;
        private System.Windows.Forms.RadioButton idxTimeBtn;
        private System.Windows.Forms.RadioButton idxCleanBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button indexDirBtn;
        private System.Windows.Forms.Label currentIndexPathLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}