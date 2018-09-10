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
            this.AppearanceGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewRatioButton
            // 
            this.treeViewRatioButton.AutoSize = true;
            this.treeViewRatioButton.Location = new System.Drawing.Point(6, 19);
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
            this.listViewRatioButton.Location = new System.Drawing.Point(6, 42);
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
            this.AppearanceGroup.Size = new System.Drawing.Size(200, 100);
            this.AppearanceGroup.TabIndex = 2;
            this.AppearanceGroup.TabStop = false;
            this.AppearanceGroup.Text = "Appearance";
            // 
            // confirmSaveSettingsBtn
            // 
            this.confirmSaveSettingsBtn.Location = new System.Drawing.Point(197, 126);
            this.confirmSaveSettingsBtn.Name = "confirmSaveSettingsBtn";
            this.confirmSaveSettingsBtn.Size = new System.Drawing.Size(75, 23);
            this.confirmSaveSettingsBtn.TabIndex = 3;
            this.confirmSaveSettingsBtn.Text = "Save";
            this.confirmSaveSettingsBtn.UseVisualStyleBackColor = true;
            this.confirmSaveSettingsBtn.Click += new System.EventHandler(this.confirmSaveSettingsBtn_Click);
            // 
            // UserSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.Controls.Add(this.confirmSaveSettingsBtn);
            this.Controls.Add(this.AppearanceGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserSettings";
            this.Text = "UserSettings";
            this.AppearanceGroup.ResumeLayout(false);
            this.AppearanceGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton treeViewRatioButton;
        private System.Windows.Forms.RadioButton listViewRatioButton;
        private System.Windows.Forms.GroupBox AppearanceGroup;
        private System.Windows.Forms.Button confirmSaveSettingsBtn;
    }
}