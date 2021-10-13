namespace ASCOM.GeminiTelescope
{
    partial class frmCollector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCollector));
            this.lbCollect = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbCancel = new System.Windows.Forms.Button();
            this.pbCollect = new System.Windows.Forms.Button();
            this.ckAll = new System.Windows.Forms.CheckBox();
            this.pbRecord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbCollect
            // 
            this.lbCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCollect.CheckOnClick = true;
            this.lbCollect.FormattingEnabled = true;
            this.lbCollect.Items.AddRange(new object[] {
            "Network and PC Configuration",
            "Gemini status",
            "Gemini log files",
            "Gemini PEC table",
            "ASCOM platform settings",
            "ASCOM all profile and settings from Gemini",
            "ASCOM latest two log files"});
            this.lbCollect.Location = new System.Drawing.Point(15, 40);
            this.lbCollect.Name = "lbCollect";
            this.lbCollect.Size = new System.Drawing.Size(431, 139);
            this.lbCollect.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select information to collect into a Zip file";
            // 
            // pbCancel
            // 
            this.pbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pbCancel.Location = new System.Drawing.Point(371, 190);
            this.pbCancel.Name = "pbCancel";
            this.pbCancel.Size = new System.Drawing.Size(75, 30);
            this.pbCancel.TabIndex = 2;
            this.pbCancel.Text = "Close";
            this.pbCancel.UseVisualStyleBackColor = true;
            this.pbCancel.Click += new System.EventHandler(this.pbCancel_Click);
            // 
            // pbCollect
            // 
            this.pbCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCollect.Location = new System.Drawing.Point(236, 190);
            this.pbCollect.Name = "pbCollect";
            this.pbCollect.Size = new System.Drawing.Size(118, 30);
            this.pbCollect.TabIndex = 3;
            this.pbCollect.Text = "Collect and Save...";
            this.pbCollect.UseVisualStyleBackColor = true;
            this.pbCollect.Click += new System.EventHandler(this.pbCollect_Click);
            // 
            // ckAll
            // 
            this.ckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckAll.AutoSize = true;
            this.ckAll.Checked = true;
            this.ckAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckAll.Location = new System.Drawing.Point(15, 190);
            this.ckAll.Name = "ckAll";
            this.ckAll.Size = new System.Drawing.Size(119, 17);
            this.ckAll.TabIndex = 4;
            this.ckAll.Text = "Check/Uncheck all";
            this.ckAll.UseVisualStyleBackColor = true;
            this.ckAll.CheckedChanged += new System.EventHandler(this.ckAll_CheckedChanged);
            // 
            // pbRecord
            // 
            this.pbRecord.Location = new System.Drawing.Point(308, 11);
            this.pbRecord.Name = "pbRecord";
            this.pbRecord.Size = new System.Drawing.Size(138, 23);
            this.pbRecord.TabIndex = 5;
            this.pbRecord.Text = "Record Activity";
            this.pbRecord.UseVisualStyleBackColor = true;
            this.pbRecord.VisibleChanged += new System.EventHandler(this.pbRecord_VisibleChanged);
            this.pbRecord.Click += new System.EventHandler(this.pbRecord_Click);
            // 
            // frmCollector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 227);
            this.Controls.Add(this.pbRecord);
            this.Controls.Add(this.ckAll);
            this.Controls.Add(this.pbCollect);
            this.Controls.Add(this.pbCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbCollect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCollector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gemini Troubleshooting Reporter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCollector_FormClosing);
            this.Load += new System.EventHandler(this.frmCollector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lbCollect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pbCancel;
        private System.Windows.Forms.Button pbCollect;
        private System.Windows.Forms.CheckBox ckAll;
        private System.Windows.Forms.Button pbRecord;
    }
}