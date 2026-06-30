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
    public partial class frmTLE : Form
    {
        public string tle { get; set; } = "";

        public frmTLE()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            tle = txtTLE.Text;
        }

        private void frmTLE_Load(object sender, EventArgs e)
        {
            txtTLE.Text = tle;
        }
    }
}
