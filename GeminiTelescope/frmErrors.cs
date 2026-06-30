using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{
    public partial class frmErrors : Form
    {
        string _err = "";

        Timer tmrUpdate = new Timer();

        public string err
        {
            get { return _err; }
            set
            {
                if (_err == value) return;
                _err = value;
                txtErrors.Text = "";
                txtErrors.AppendText(err);
            }
        }

        public frmErrors()
        {
            InitializeComponent();
        }

  
        private void frmErrors_Load(object sender, EventArgs e)
        {
            tmrUpdate.Interval = 1000;
            tmrUpdate.Tick += TmrUpdate_Tick;
            tmrUpdate.Start();
            //txtErrors.Text = err;
        }

        private void TmrUpdate_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                err = GeminiHardware.Instance.m_AllErrors;
            }
        }

        private void frmErrors_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmMain frm = Application.OpenForms[0] as frmMain;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return; //exiting
            e.Cancel = true;
            this.Hide();
        }
    }
}
