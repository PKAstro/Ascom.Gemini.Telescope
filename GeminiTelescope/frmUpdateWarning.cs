using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{
    public partial class frmUpdateWarning : Form
    {
        public frmUpdateWarning()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pbDont_Click(object sender, EventArgs e)
        {

        }

        private void pbDo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
