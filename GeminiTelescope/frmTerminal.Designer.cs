namespace ASCOM.GeminiTelescope
{
    partial class frmTerminal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTerminal));
            this.txtTerm = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtTerm
            // 
            this.txtTerm.BackColor = System.Drawing.Color.Black;
            this.txtTerm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTerm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTerm.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTerm.ForeColor = System.Drawing.Color.White;
            this.txtTerm.Location = new System.Drawing.Point(0, 0);
            this.txtTerm.Multiline = true;
            this.txtTerm.Name = "txtTerm";
            this.txtTerm.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTerm.Size = new System.Drawing.Size(694, 329);
            this.txtTerm.TabIndex = 0;
            this.txtTerm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtTerm_KeyUp);
            // 
            // frmTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 329);
            this.Controls.Add(this.txtTerm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTerminal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gemini Command Terminal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTerminal_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTerm;
    }
}