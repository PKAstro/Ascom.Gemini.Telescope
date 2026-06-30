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
    public partial class frmSiderealRate : Form
    {

        public double siderealRate;
        public frmSiderealRate()
        {
            InitializeComponent();
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtSidereal.Text, out siderealRate))
                DialogResult = DialogResult.OK;
            else MessageBox.Show("Invalid rate specified");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

        }

        private void frmSiderealRate_Load(object sender, EventArgs e)
        {
            txtSidereal.Text = $"{siderealRate}";
        }
    }
}
