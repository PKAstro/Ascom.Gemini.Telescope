using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{
    partial class frmMinorPlanets
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMinorPlanets));
            this.label1 = new System.Windows.Forms.Label();
            this.btnGoto = new System.Windows.Forms.Button();
            this.txtRA = new System.Windows.Forms.MaskedTextBox();
            this.txtDEC = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtObject = new System.Windows.Forms.ComboBox();
            this.btnTrack = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnTLE = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtdDEC = new System.Windows.Forms.MaskedTextBox();
            this.txtdRA = new System.Windows.Forms.MaskedTextBox();
            this.btnRaw = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnJPL = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDECError = new System.Windows.Forms.MaskedTextBox();
            this.txtRAError = new System.Windows.Forms.MaskedTextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.ckClosedLoop = new System.Windows.Forms.CheckBox();
            this.pbFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Object Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGoto
            // 
            this.btnGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoto.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnGoto.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnGoto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnGoto.Location = new System.Drawing.Point(727, 158);
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(75, 53);
            this.btnGoto.TabIndex = 4;
            this.btnGoto.Text = "Go To";
            this.btnGoto.UseVisualStyleBackColor = true;
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // txtRA
            // 
            this.txtRA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRA.BackColor = System.Drawing.Color.Black;
            this.txtRA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRA.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtRA.ForeColor = System.Drawing.Color.White;
            this.txtRA.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtRA.Location = new System.Drawing.Point(91, 248);
            this.txtRA.Margin = new System.Windows.Forms.Padding(0);
            this.txtRA.Name = "txtRA";
            this.txtRA.PromptChar = '0';
            this.txtRA.Size = new System.Drawing.Size(109, 20);
            this.txtRA.TabIndex = 2;
            this.txtRA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRA.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            this.txtRA.Enter += new System.EventHandler(this.txtRA_Enter);
            // 
            // txtDEC
            // 
            this.txtDEC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDEC.BackColor = System.Drawing.Color.Black;
            this.txtDEC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDEC.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtDEC.ForeColor = System.Drawing.Color.White;
            this.txtDEC.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtDEC.Location = new System.Drawing.Point(91, 274);
            this.txtDEC.Margin = new System.Windows.Forms.Padding(0);
            this.txtDEC.Name = "txtDEC";
            this.txtDEC.PromptChar = '0';
            this.txtDEC.Size = new System.Drawing.Size(109, 20);
            this.txtDEC.TabIndex = 3;
            this.txtDEC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDEC.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            this.txtDEC.Enter += new System.EventHandler(this.txtDEC_Enter);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(13, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "DEC:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtObject
            // 
            this.txtObject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObject.BackColor = System.Drawing.Color.Black;
            this.txtObject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtObject.Font = new System.Drawing.Font("Arial", 11.25F);
            this.txtObject.ForeColor = System.Drawing.Color.White;
            this.txtObject.FormattingEnabled = true;
            this.txtObject.Location = new System.Drawing.Point(91, 20);
            this.txtObject.Name = "txtObject";
            this.txtObject.Size = new System.Drawing.Size(531, 25);
            this.txtObject.TabIndex = 1;
            this.txtObject.SelectedIndexChanged += new System.EventHandler(this.txtObject_SelectedIndexChanged);
            this.txtObject.SelectionChangeCommitted += new System.EventHandler(this.txtObject_SelectionChangeCommitted);
            this.txtObject.DropDownClosed += new System.EventHandler(this.txtObject_DropDownClosed);
            this.txtObject.TextChanged += new System.EventHandler(this.txtObject_TextChanged);
            // 
            // btnTrack
            // 
            this.btnTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTrack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnTrack.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnTrack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTrack.Location = new System.Drawing.Point(628, 236);
            this.btnTrack.Name = "btnTrack";
            this.btnTrack.Size = new System.Drawing.Size(75, 53);
            this.btnTrack.TabIndex = 5;
            this.btnTrack.Text = "Track";
            this.btnTrack.UseVisualStyleBackColor = true;
            this.btnTrack.Click += new System.EventHandler(this.btnTrack_Click);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnFind.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnFind.Location = new System.Drawing.Point(628, 20);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 26);
            this.btnFind.TabIndex = 14;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtDesc
            // 
            this.txtDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDesc.BackColor = System.Drawing.Color.Black;
            this.txtDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDesc.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtDesc.ForeColor = System.Drawing.Color.White;
            this.txtDesc.Location = new System.Drawing.Point(16, 58);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ReadOnly = true;
            this.txtDesc.Size = new System.Drawing.Size(687, 153);
            this.txtDesc.TabIndex = 15;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.btnStop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStop.Location = new System.Drawing.Point(727, 236);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 53);
            this.btnStop.TabIndex = 16;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnTLE
            // 
            this.btnTLE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTLE.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnTLE.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnTLE.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTLE.Location = new System.Drawing.Point(727, 23);
            this.btnTLE.Name = "btnTLE";
            this.btnTLE.Size = new System.Drawing.Size(75, 23);
            this.btnTLE.TabIndex = 17;
            this.btnTLE.Text = "TLE...";
            this.btnTLE.UseVisualStyleBackColor = true;
            this.btnTLE.Click += new System.EventHandler(this.pbTLE_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 225);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Current Coordinates";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(223, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Current Rates (\"/sec)";
            // 
            // txtdDEC
            // 
            this.txtdDEC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtdDEC.BackColor = System.Drawing.Color.Black;
            this.txtdDEC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtdDEC.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtdDEC.ForeColor = System.Drawing.Color.White;
            this.txtdDEC.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtdDEC.Location = new System.Drawing.Point(222, 274);
            this.txtdDEC.Margin = new System.Windows.Forms.Padding(0);
            this.txtdDEC.Name = "txtdDEC";
            this.txtdDEC.PromptChar = '0';
            this.txtdDEC.Size = new System.Drawing.Size(109, 20);
            this.txtdDEC.TabIndex = 20;
            this.txtdDEC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtdDEC.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            // 
            // txtdRA
            // 
            this.txtdRA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtdRA.BackColor = System.Drawing.Color.Black;
            this.txtdRA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtdRA.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtdRA.ForeColor = System.Drawing.Color.White;
            this.txtdRA.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtdRA.Location = new System.Drawing.Point(222, 248);
            this.txtdRA.Margin = new System.Windows.Forms.Padding(0);
            this.txtdRA.Name = "txtdRA";
            this.txtdRA.PromptChar = '0';
            this.txtdRA.Size = new System.Drawing.Size(109, 20);
            this.txtdRA.TabIndex = 19;
            this.txtdRA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtdRA.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            // 
            // btnRaw
            // 
            this.btnRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRaw.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRaw.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRaw.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnRaw.Location = new System.Drawing.Point(727, 51);
            this.btnRaw.Name = "btnRaw";
            this.btnRaw.Size = new System.Drawing.Size(75, 23);
            this.btnRaw.TabIndex = 24;
            this.btnRaw.Text = "Raw Data...";
            this.btnRaw.UseVisualStyleBackColor = true;
            this.btnRaw.Click += new System.EventHandler(this.pbRaw_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(12, 249);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "RA:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnJPL
            // 
            this.btnJPL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJPL.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnJPL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnJPL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnJPL.Location = new System.Drawing.Point(727, 80);
            this.btnJPL.Name = "btnJPL";
            this.btnJPL.Size = new System.Drawing.Size(75, 23);
            this.btnJPL.TabIndex = 26;
            this.btnJPL.Text = "JPL View...";
            this.btnJPL.UseVisualStyleBackColor = true;
            this.btnJPL.Click += new System.EventHandler(this.btnJPL_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(363, 225);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Coordinate Error";
            // 
            // txtDECError
            // 
            this.txtDECError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDECError.BackColor = System.Drawing.Color.Black;
            this.txtDECError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDECError.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtDECError.ForeColor = System.Drawing.Color.White;
            this.txtDECError.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtDECError.Location = new System.Drawing.Point(360, 274);
            this.txtDECError.Margin = new System.Windows.Forms.Padding(0);
            this.txtDECError.Name = "txtDECError";
            this.txtDECError.PromptChar = '0';
            this.txtDECError.Size = new System.Drawing.Size(109, 20);
            this.txtDECError.TabIndex = 28;
            this.txtDECError.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDECError.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            // 
            // txtRAError
            // 
            this.txtRAError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRAError.BackColor = System.Drawing.Color.Black;
            this.txtRAError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRAError.Font = new System.Drawing.Font("Lucida Console", 9.75F);
            this.txtRAError.ForeColor = System.Drawing.Color.White;
            this.txtRAError.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.txtRAError.Location = new System.Drawing.Point(360, 248);
            this.txtRAError.Margin = new System.Windows.Forms.Padding(0);
            this.txtRAError.Name = "txtRAError";
            this.txtRAError.PromptChar = '0';
            this.txtRAError.Size = new System.Drawing.Size(109, 20);
            this.txtRAError.TabIndex = 27;
            this.txtRAError.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRAError.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9.75F);
            this.lblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStatus.Location = new System.Drawing.Point(0, 306);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(829, 28);
            this.lblStatus.TabIndex = 34;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ckClosedLoop
            // 
            this.ckClosedLoop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ckClosedLoop.AutoSize = true;
            this.ckClosedLoop.Location = new System.Drawing.Point(491, 254);
            this.ckClosedLoop.Name = "ckClosedLoop";
            this.ckClosedLoop.Size = new System.Drawing.Size(131, 30);
            this.ckClosedLoop.TabIndex = 35;
            this.ckClosedLoop.Text = "Auto-correct RA/DEC\r\n(turn off when guiding)\r\n";
            this.ckClosedLoop.UseVisualStyleBackColor = true;
            // 
            // pbFile
            // 
            this.pbFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.pbFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pbFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbFile.Location = new System.Drawing.Point(727, 120);
            this.pbFile.Name = "pbFile";
            this.pbFile.Size = new System.Drawing.Size(75, 23);
            this.pbFile.TabIndex = 36;
            this.pbFile.Text = "From File...";
            this.pbFile.UseVisualStyleBackColor = true;
            this.pbFile.Click += new System.EventHandler(this.pbFile_Click);
            // 
            // frmMinorPlanets
            // 
            this.AcceptButton = this.btnGoto;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(829, 333);
            this.Controls.Add(this.pbFile);
            this.Controls.Add(this.ckClosedLoop);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtDECError);
            this.Controls.Add(this.txtRAError);
            this.Controls.Add(this.btnJPL);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnRaw);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtdDEC);
            this.Controls.Add(this.txtdRA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnTLE);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.btnTrack);
            this.Controls.Add(this.txtObject);
            this.Controls.Add(this.txtDEC);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtRA);
            this.Controls.Add(this.btnGoto);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMinorPlanets";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Minor Planets & Small Objects";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMinorPlanets_FormClosing);
            this.Load += new System.EventHandler(this.frmMinorPlanets_Load);
            this.ChangeUICues += new System.Windows.Forms.UICuesEventHandler(this.frmMinorPlanets_ChangeUICues);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGoto;
        private System.Windows.Forms.MaskedTextBox txtRA;
        private System.Windows.Forms.MaskedTextBox txtDEC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox txtObject;
        private System.Windows.Forms.Button btnTrack;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnTLE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox txtdDEC;
        private System.Windows.Forms.MaskedTextBox txtdRA;
        private System.Windows.Forms.Button btnRaw;
        private Label label7;
        private Button btnJPL;
        private Label label11;
        private MaskedTextBox txtDECError;
        private MaskedTextBox txtRAError;
        private Label lblStatus;
        private CheckBox ckClosedLoop;
        private Button pbFile;

        class CCombo : ComboBox
        {
            private const int WM_PASTE = 0x0302;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg != WM_PASTE)
                {
                    // Handle all other messages normally
                    base.WndProc(ref m);
                }
                else
                {
                    this.Text = "[PASTE]";
                }
            }
        }


    }
}