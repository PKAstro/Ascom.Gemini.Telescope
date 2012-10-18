using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ASCOM.GeminiTelescope
{
    public partial class frmPECChart : Form
    {
        public DateTime[] X = null;
        public float[] Y= null;

        private static int count = 1;


        public frmPECChart()
        {
            InitializeComponent();
            this.Text = "PEC Chart " + count.ToString();
            count++;
        }



        internal void UpdateData()
        {
            chrt.Series["Series1"].Points.Clear();
            chrt.ChartAreas[0].AxisX.LabelStyle.Format = "m:ss";
            chrt.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            chrt.Series["Series1"].ChartType = SeriesChartType.Line;

            chrt.ChartAreas[0].AxisY.Title = "arcsecs";
            chrt.ChartAreas[0].AxisX.Title = "time";

            for (int i = 0; i < X.Length; ++i)
            {
                chrt.Series["Series1"].Points.AddXY(X[i], Y[i]);
            }
        }

        private void chrt_Click(object sender, EventArgs e)
        {

        }
    }
}
