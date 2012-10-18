namespace ASCOM.GeminiTelescope
{
    partial class frmPEC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPEC));
            this.chkEnablePEC = new System.Windows.Forms.CheckBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.pbSaveAs = new System.Windows.Forms.Button();
            this.pbLoad = new System.Windows.Forms.Button();
            this.chkDataAvailable = new System.Windows.Forms.CheckBox();
            this.chkTraining = new System.Windows.Forms.CheckBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.cbSaveAs = new System.Windows.Forms.ComboBox();
            this.cbLoad = new System.Windows.Forms.ComboBox();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.percent = new System.Windows.Forms.Label();
            this.cbPECMax = new System.Windows.Forms.ComboBox();
            this.SetPECMax = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pbClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkEnablePEC
            // 
            this.chkEnablePEC.AutoCheck = false;
            this.chkEnablePEC.AutoSize = true;
            this.chkEnablePEC.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.chkEnablePEC.Location = new System.Drawing.Point(17, 22);
            this.chkEnablePEC.Name = "chkEnablePEC";
            this.chkEnablePEC.Size = new System.Drawing.Size(94, 17);
            this.chkEnablePEC.TabIndex = 30;
            this.chkEnablePEC.Text = "PEC Playback";
            this.chkEnablePEC.UseVisualStyleBackColor = false;
            this.chkEnablePEC.Click += new System.EventHandler(this.chkEnablePEC_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(16, 96);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(113, 24);
            this.cmdOK.TabIndex = 31;
            this.cmdOK.Text = "Start PEC Training";
            this.cmdOK.UseVisualStyleBackColor = false;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            this.cmdOK.Validating += new System.ComponentModel.CancelEventHandler(this.cmdOK_Validating);
            // 
            // pbSaveAs
            // 
            this.pbSaveAs.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pbSaveAs.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.pbSaveAs.Location = new System.Drawing.Point(16, 126);
            this.pbSaveAs.Name = "pbSaveAs";
            this.pbSaveAs.Size = new System.Drawing.Size(113, 24);
            this.pbSaveAs.TabIndex = 33;
            this.pbSaveAs.Text = "Save PEC As";
            this.pbSaveAs.UseVisualStyleBackColor = false;
            this.pbSaveAs.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbLoad
            // 
            this.pbLoad.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pbLoad.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.pbLoad.Location = new System.Drawing.Point(16, 153);
            this.pbLoad.Name = "pbLoad";
            this.pbLoad.Size = new System.Drawing.Size(113, 24);
            this.pbLoad.TabIndex = 34;
            this.pbLoad.Text = "Load PEC";
            this.pbLoad.UseVisualStyleBackColor = false;
            this.pbLoad.Click += new System.EventHandler(this.pbLoad_Click);
            // 
            // chkDataAvailable
            // 
            this.chkDataAvailable.AutoCheck = false;
            this.chkDataAvailable.AutoSize = true;
            this.chkDataAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.chkDataAvailable.Location = new System.Drawing.Point(17, 45);
            this.chkDataAvailable.Name = "chkDataAvailable";
            this.chkDataAvailable.Size = new System.Drawing.Size(119, 17);
            this.chkDataAvailable.TabIndex = 35;
            this.chkDataAvailable.Text = "PEC Data Available";
            this.chkDataAvailable.UseVisualStyleBackColor = false;
            // 
            // chkTraining
            // 
            this.chkTraining.AutoCheck = false;
            this.chkTraining.AutoSize = true;
            this.chkTraining.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.chkTraining.Location = new System.Drawing.Point(17, 70);
            this.chkTraining.Name = "chkTraining";
            this.chkTraining.Size = new System.Drawing.Size(144, 17);
            this.chkTraining.TabIndex = 36;
            this.chkTraining.Text = "PEC Training In Progress";
            this.chkTraining.UseVisualStyleBackColor = false;
            // 
            // lbStatus
            // 
            this.lbStatus.BackColor = System.Drawing.Color.Black;
            this.lbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.ForeColor = System.Drawing.Color.White;
            this.lbStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbStatus.Location = new System.Drawing.Point(144, 96);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(208, 24);
            this.lbStatus.TabIndex = 32;
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbSaveAs
            // 
            this.cbSaveAs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbSaveAs.FormattingEnabled = true;
            this.cbSaveAs.Location = new System.Drawing.Point(144, 129);
            this.cbSaveAs.Name = "cbSaveAs";
            this.cbSaveAs.Size = new System.Drawing.Size(208, 21);
            this.cbSaveAs.TabIndex = 37;
            // 
            // cbLoad
            // 
            this.cbLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoad.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbLoad.FormattingEnabled = true;
            this.cbLoad.Location = new System.Drawing.Point(144, 156);
            this.cbLoad.Name = "cbLoad";
            this.cbLoad.Size = new System.Drawing.Size(208, 21);
            this.cbLoad.TabIndex = 38;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(16, 210);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(246, 23);
            this.progress.Step = 1;
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progress.TabIndex = 39;
            this.progress.Visible = false;
            // 
            // percent
            // 
            this.percent.BackColor = System.Drawing.Color.Black;
            this.percent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.percent.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percent.ForeColor = System.Drawing.Color.White;
            this.percent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.percent.Location = new System.Drawing.Point(268, 210);
            this.percent.Name = "percent";
            this.percent.Size = new System.Drawing.Size(83, 24);
            this.percent.TabIndex = 40;
            this.percent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.percent.Visible = false;
            // 
            // cbPECMax
            // 
            this.cbPECMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPECMax.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbPECMax.FormattingEnabled = true;
            this.cbPECMax.Location = new System.Drawing.Point(144, 183);
            this.cbPECMax.Name = "cbPECMax";
            this.cbPECMax.Size = new System.Drawing.Size(208, 21);
            this.cbPECMax.TabIndex = 42;
            // 
            // SetPECMax
            // 
            this.SetPECMax.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SetPECMax.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SetPECMax.Location = new System.Drawing.Point(16, 180);
            this.SetPECMax.Name = "SetPECMax";
            this.SetPECMax.Size = new System.Drawing.Size(113, 24);
            this.SetPECMax.TabIndex = 41;
            this.SetPECMax.Text = "Set PEC Period";
            this.SetPECMax.UseVisualStyleBackColor = false;
            this.SetPECMax.Click += new System.EventHandler(this.SetPECMax_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(358, 129);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 23);
            this.button1.TabIndex = 43;
            this.button1.Text = "PE";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // pbClear
            // 
            this.pbClear.Location = new System.Drawing.Point(254, 70);
            this.pbClear.Name = "pbClear";
            this.pbClear.Size = new System.Drawing.Size(97, 23);
            this.pbClear.TabIndex = 44;
            this.pbClear.Text = "Clear PEC";
            this.pbClear.UseVisualStyleBackColor = true;
            this.pbClear.Click += new System.EventHandler(this.pbClear_Click);
            // 
            // frmPEC
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(397, 245);
            this.Controls.Add(this.pbClear);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbPECMax);
            this.Controls.Add(this.SetPECMax);
            this.Controls.Add(this.percent);
            this.Controls.Add(this.cbLoad);
            this.Controls.Add(this.cbSaveAs);
            this.Controls.Add(this.chkTraining);
            this.Controls.Add(this.chkDataAvailable);
            this.Controls.Add(this.pbLoad);
            this.Controls.Add(this.pbSaveAs);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkEnablePEC);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPEC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Configure PEC";
            this.Load += new System.EventHandler(this.frmPassThroughPortSetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        public System.Windows.Forms.CheckBox chkEnablePEC;
        private System.Windows.Forms.Button pbSaveAs;
        private System.Windows.Forms.Button pbLoad;
        public System.Windows.Forms.CheckBox chkDataAvailable;
        public System.Windows.Forms.CheckBox chkTraining;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.ComboBox cbSaveAs;
        private System.Windows.Forms.ComboBox cbLoad;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label percent;
        private System.Windows.Forms.ComboBox cbPECMax;
        private System.Windows.Forms.Button SetPECMax;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button pbClear;
    }
}