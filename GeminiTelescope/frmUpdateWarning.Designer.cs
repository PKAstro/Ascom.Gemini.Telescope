namespace ASCOM.GeminiTelescope
{
    partial class frmUpdateWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateWarning));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ckDontAskAgain = new System.Windows.Forms.CheckBox();
            this.pbDont = new System.Windows.Forms.Button();
            this.pbDo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(21, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(431, 104);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "WARNING!\r\n\r\nYou\'ve enabled the setting to update Gemini configuration on connect." +
    " \r\nAll Gemini settings will be overwritten with the settings from your PC.\r\n\r\nAr" +
    "e you sure you want to do this?";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ckDontAskAgain
            // 
            this.ckDontAskAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckDontAskAgain.AutoSize = true;
            this.ckDontAskAgain.Location = new System.Drawing.Point(21, 155);
            this.ckDontAskAgain.Name = "ckDontAskAgain";
            this.ckDontAskAgain.Size = new System.Drawing.Size(100, 17);
            this.ckDontAskAgain.TabIndex = 2;
            this.ckDontAskAgain.Text = "Don\'t ask again";
            this.ckDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // pbDont
            // 
            this.pbDont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDont.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pbDont.Location = new System.Drawing.Point(271, 147);
            this.pbDont.Name = "pbDont";
            this.pbDont.Size = new System.Drawing.Size(78, 30);
            this.pbDont.TabIndex = 0;
            this.pbDont.Text = "Don\'t update";
            this.pbDont.UseVisualStyleBackColor = true;
            this.pbDont.Click += new System.EventHandler(this.pbDont_Click);
            // 
            // pbDo
            // 
            this.pbDo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDo.Location = new System.Drawing.Point(374, 147);
            this.pbDo.Name = "pbDo";
            this.pbDo.Size = new System.Drawing.Size(78, 30);
            this.pbDo.TabIndex = 1;
            this.pbDo.Text = "Update";
            this.pbDo.UseVisualStyleBackColor = true;
            this.pbDo.Click += new System.EventHandler(this.pbDo_Click);
            // 
            // frmUpdateWarning
            // 
            this.AcceptButton = this.pbDo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.pbDont;
            this.ClientSize = new System.Drawing.Size(464, 184);
            this.Controls.Add(this.pbDo);
            this.Controls.Add(this.pbDont);
            this.Controls.Add(this.ckDontAskAgain);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdateWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Gemini Settings?";
            this.Load += new System.EventHandler(this.frmUpdateWarning_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button pbDont;
        private System.Windows.Forms.Button pbDo;
        internal System.Windows.Forms.CheckBox ckDontAskAgain;
    }
}