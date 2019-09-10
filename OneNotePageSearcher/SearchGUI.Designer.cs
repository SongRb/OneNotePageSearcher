using System.Windows.Forms;

namespace OneNotePageSearcher
{
    partial class SearchGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchGUI));
            this.inputField = new System.Windows.Forms.GroupBox();
            this.indexButton = new System.Windows.Forms.Button();
            this.searchButton = new System.Windows.Forms.Button();
            this.queryBox = new System.Windows.Forms.TextBox();
            this.outputField = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.etaLabel = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.indexProgressBar = new System.Windows.Forms.ProgressBar();
            this.resultGridView = new System.Windows.Forms.DataGridView();
            this.exitButton = new System.Windows.Forms.Button();
            this.optionButton = new System.Windows.Forms.Button();
            this.inputField.SuspendLayout();
            this.outputField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // inputField
            // 
            this.inputField.Controls.Add(this.indexButton);
            this.inputField.Controls.Add(this.searchButton);
            this.inputField.Controls.Add(this.queryBox);
            this.inputField.Location = new System.Drawing.Point(10, 13);
            this.inputField.Name = "inputField";
            this.inputField.Size = new System.Drawing.Size(552, 60);
            this.inputField.TabIndex = 7;
            this.inputField.TabStop = false;
            // 
            // indexButton
            // 
            this.indexButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indexButton.Location = new System.Drawing.Point(447, 18);
            this.indexButton.Margin = new System.Windows.Forms.Padding(4);
            this.indexButton.Name = "indexButton";
            this.indexButton.Size = new System.Drawing.Size(88, 30);
            this.indexButton.TabIndex = 6;
            this.indexButton.Text = "Index";
            this.indexButton.UseVisualStyleBackColor = true;
            this.indexButton.Click += new System.EventHandler(this.IndexButtonClick);
            // 
            // searchButton
            // 
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(353, 18);
            this.searchButton.Margin = new System.Windows.Forms.Padding(4);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(88, 30);
            this.searchButton.TabIndex = 5;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // queryBox
            // 
            this.queryBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.queryBox.Location = new System.Drawing.Point(17, 21);
            this.queryBox.Margin = new System.Windows.Forms.Padding(4);
            this.queryBox.Name = "queryBox";
            this.queryBox.Size = new System.Drawing.Size(294, 22);
            this.queryBox.TabIndex = 4;
            // 
            // outputField
            // 
            this.outputField.Controls.Add(this.treeView1);
            this.outputField.Controls.Add(this.etaLabel);
            this.outputField.Controls.Add(this.progressLabel);
            this.outputField.Controls.Add(this.indexProgressBar);
            this.outputField.Controls.Add(this.resultGridView);
            this.outputField.Location = new System.Drawing.Point(10, 79);
            this.outputField.Name = "outputField";
            this.outputField.Size = new System.Drawing.Size(552, 319);
            this.outputField.TabIndex = 7;
            this.outputField.TabStop = false;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(17, 19);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(518, 279);
            this.treeView1.TabIndex = 9;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            // 
            // etaLabel
            // 
            this.etaLabel.AutoSize = true;
            this.etaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.etaLabel.Location = new System.Drawing.Point(16, 110);
            this.etaLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.etaLabel.Name = "etaLabel";
            this.etaLabel.Size = new System.Drawing.Size(123, 16);
            this.etaLabel.TabIndex = 9;
            this.etaLabel.Text = "Remaining Time: ∞ ";
            this.etaLabel.Visible = false;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressLabel.Location = new System.Drawing.Point(16, 65);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(74, 16);
            this.progressLabel.TabIndex = 8;
            this.progressLabel.Text = "Initializing...";
            this.progressLabel.Visible = false;
            // 
            // indexProgressBar
            // 
            this.indexProgressBar.Location = new System.Drawing.Point(18, 153);
            this.indexProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.indexProgressBar.Name = "indexProgressBar";
            this.indexProgressBar.Size = new System.Drawing.Size(518, 30);
            this.indexProgressBar.TabIndex = 7;
            this.indexProgressBar.Visible = false;
            // 
            // resultGridView
            // 
            this.resultGridView.AllowUserToAddRows = false;
            this.resultGridView.AllowUserToDeleteRows = false;
            this.resultGridView.AllowUserToResizeRows = false;
            this.resultGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.resultGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultGridView.Location = new System.Drawing.Point(17, 19);
            this.resultGridView.Margin = new System.Windows.Forms.Padding(4);
            this.resultGridView.Name = "resultGridView";
            this.resultGridView.ReadOnly = true;
            this.resultGridView.Size = new System.Drawing.Size(518, 279);
            this.resultGridView.TabIndex = 3;
            this.resultGridView.Visible = false;
            this.resultGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGridView_CellContentDoubleClick);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(497, 404);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(66, 24);
            this.exitButton.TabIndex = 7;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // optionButton
            // 
            this.optionButton.Location = new System.Drawing.Point(10, 404);
            this.optionButton.Name = "optionButton";
            this.optionButton.Size = new System.Drawing.Size(66, 24);
            this.optionButton.TabIndex = 8;
            this.optionButton.Text = "Option";
            this.optionButton.UseVisualStyleBackColor = true;
            this.optionButton.Click += new System.EventHandler(this.optionButton_Click);
            // 
            // SearchGUI
            // 
            this.AcceptButton = this.searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 440);
            this.Controls.Add(this.optionButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.outputField);
            this.Controls.Add(this.inputField);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "SearchGUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Search OneNote Content";
            this.inputField.ResumeLayout(false);
            this.inputField.PerformLayout();
            this.outputField.ResumeLayout(false);
            this.outputField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox inputField;
        private System.Windows.Forms.Button indexButton;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.TextBox queryBox;
        private System.Windows.Forms.GroupBox outputField;
        private System.Windows.Forms.Label etaLabel;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar indexProgressBar;
        private System.Windows.Forms.DataGridView resultGridView;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button optionButton;
        private TreeView treeView1;
    }
}