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
    public partial class frmMPRaw : Form
    {
        public string data { get; set; } = "";

        public frmMPRaw()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
           
        }

        private void frmMPRaw_Load(object sender, EventArgs e)
        {
            txtData.Text = data.Replace("\n", "\r\n");
            txtData.SelectionStart = 0;
            txtData.SelectionLength = 0;
        }
    }
}
