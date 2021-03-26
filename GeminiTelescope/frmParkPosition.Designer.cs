namespace ASCOM.GeminiTelescope
{
    partial class frmParkPosition
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmParkPosition));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbCWD = new System.Windows.Forms.RadioButton();
            this.rbAltAz = new System.Windows.Forms.RadioButton();
            this.rbHome = new System.Windows.Forms.RadioButton();
            this.pbGetPos = new System.Windows.Forms.Button();
            this.txtAlt = new System.Windows.Forms.TextBox();
            this.rbNoSlew = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAz = new System.Windows.Forms.TextBox();
            this.pbOK = new System.Windows.Forms.Button();
            this.pbCancel = new System.Windows.Forms.Button();
            this.chkUnparkMode = new System.Windows.Forms.CheckBox();
            this.pbSetHome = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbCWD);
            this.groupBox1.Controls.Add(this.rbAltAz);
            this.groupBox1.Controls.Add(this.rbHome);
            this.groupBox1.Controls.Add(this.pbGetPos);
            this.groupBox1.Controls.Add(this.txtAlt);
            this.groupBox1.Controls.Add(this.rbNoSlew);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtAz);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rbCWD
            // 
            resources.ApplyResources(this.rbCWD, "rbCWD");
            this.rbCWD.Name = "rbCWD";
            this.rbCWD.TabStop = true;
            this.rbCWD.UseVisualStyleBackColor = true;
            // 
            // rbAltAz
            // 
            resources.ApplyResources(this.rbAltAz, "rbAltAz");
            this.rbAltAz.Name = "rbAltAz";
            this.rbAltAz.TabStop = true;
            this.rbAltAz.UseVisualStyleBackColor = true;
            // 
            // rbHome
            // 
            resources.ApplyResources(this.rbHome, "rbHome");
            this.rbHome.Name = "rbHome";
            this.rbHome.TabStop = true;
            this.rbHome.UseVisualStyleBackColor = true;
            // 
            // pbGetPos
            // 
            resources.ApplyResources(this.pbGetPos, "pbGetPos");
            this.pbGetPos.ForeColor = System.Drawing.Color.White;
            this.pbGetPos.Name = "pbGetPos";
            this.pbGetPos.UseVisualStyleBackColor = true;
            this.pbGetPos.Click += new System.EventHandler(this.pbGetPos_Click);
            // 
            // txtAlt
            // 
            resources.ApplyResources(this.txtAlt, "txtAlt");
            this.txtAlt.Name = "txtAlt";
            // 
            // rbNoSlew
            // 
            resources.ApplyResources(this.rbNoSlew, "rbNoSlew");
            this.rbNoSlew.Name = "rbNoSlew";
            this.rbNoSlew.TabStop = true;
            this.rbNoSlew.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtAz
            // 
            resources.ApplyResources(this.txtAz, "txtAz");
            this.txtAz.Name = "txtAz";
            // 
            // pbOK
            // 
            resources.ApplyResources(this.pbOK, "pbOK");
            this.pbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.pbOK.ForeColor = System.Drawing.Color.White;
            this.pbOK.Name = "pbOK";
            this.pbOK.UseVisualStyleBackColor = true;
            this.pbOK.Click += new System.EventHandler(this.pbOK_Click);
            // 
            // pbCancel
            // 
            resources.ApplyResources(this.pbCancel, "pbCancel");
            this.pbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pbCancel.ForeColor = System.Drawing.Color.White;
            this.pbCancel.Name = "pbCancel";
            this.pbCancel.UseVisualStyleBackColor = true;
            // 
            // chkUnparkMode
            // 
            this.chkUnparkMode.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.chkUnparkMode, "chkUnparkMode");
            this.chkUnparkMode.Name = "chkUnparkMode";
            this.chkUnparkMode.UseVisualStyleBackColor = true;
            // 
            // pbSetHome
            // 
            resources.ApplyResources(this.pbSetHome, "pbSetHome");
            this.pbSetHome.ForeColor = System.Drawing.Color.White;
            this.pbSetHome.Name = "pbSetHome";
            this.pbSetHome.UseVisualStyleBackColor = true;
            this.pbSetHome.Click += new System.EventHandler(this.pbSetHome_Click);
            // 
            // frmParkPosition
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.pbSetHome);
            this.Controls.Add(this.chkUnparkMode);
            this.Controls.Add(this.pbCancel);
            this.Controls.Add(this.pbOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmParkPosition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmParkPosition_FormClosed);
            this.Load += new System.EventHandler(this.frmParkPosition_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pbOK;
        private System.Windows.Forms.Button pbCancel;
        private System.Windows.Forms.Button pbGetPos;
        private System.Windows.Forms.TextBox txtAz;
        private System.Windows.Forms.TextBox txtAlt;
        private System.Windows.Forms.RadioButton rbAltAz;
        private System.Windows.Forms.RadioButton rbHome;
        private System.Windows.Forms.RadioButton rbNoSlew;
        private System.Windows.Forms.RadioButton rbCWD;
        private System.Windows.Forms.CheckBox chkUnparkMode;
        private System.Windows.Forms.Button pbSetHome;
    }
}