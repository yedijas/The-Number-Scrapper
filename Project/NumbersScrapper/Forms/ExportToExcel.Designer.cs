namespace NumbersScrapper.Forms
{
    partial class ExportToExcel
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnSome = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.lstYear = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Year";
            // 
            // btnSome
            // 
            this.btnSome.Location = new System.Drawing.Point(12, 195);
            this.btnSome.Name = "btnSome";
            this.btnSome.Size = new System.Drawing.Size(260, 23);
            this.btnSome.TabIndex = 2;
            this.btnSome.Text = "Export Year";
            this.btnSome.UseVisualStyleBackColor = true;
            this.btnSome.Click += new System.EventHandler(this.btnSome_Click);
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(12, 12);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(260, 23);
            this.btnAll.TabIndex = 3;
            this.btnAll.Text = "Export All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // lstYear
            // 
            this.lstYear.FormattingEnabled = true;
            this.lstYear.Location = new System.Drawing.Point(12, 54);
            this.lstYear.Name = "lstYear";
            this.lstYear.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstYear.Size = new System.Drawing.Size(260, 95);
            this.lstYear.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Select Path";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 169);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(260, 20);
            this.txtPath.TabIndex = 6;
            this.txtPath.Click += new System.EventHandler(this.txtPath_Click);
            // 
            // ExportToExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 232);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstYear);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnSome);
            this.Controls.Add(this.label1);
            this.Name = "ExportToExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExportToExcel";
            this.Load += new System.EventHandler(this.ExportToExcel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSome;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.ListBox lstYear;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.FolderBrowserDialog folderDlg;
    }
}