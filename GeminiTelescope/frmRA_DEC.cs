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
    public partial class frmRA_DEC : Form
    {
        private bool customAdded = false;

        public frmRA_DEC()
        {
            InitializeComponent();
        }

        private void frmRA_DEC_Load(object sender, EventArgs e)
        {

            if (frmUserCatalog.m_Objects == null)
            {
                frmProgress.Initialize(0, 100, "Load Catalogs...", null);
                frmProgress.ShowProgress(this);

                frmUserCatalog uc = new frmUserCatalog();
                uc.PopulateCatalogs();
                uc.Dispose();
                frmProgress.HideProgress();
            }
            GeminiHardware.Instance.OnConnect += new ConnectDelegate(OnConnect);
            txtObject.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtObject.AutoCompleteMode = AutoCompleteMode.Suggest;
            System.Windows.Forms.AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
            coll.AddRange(frmUserCatalog.m_Objects.Keys.ToArray<string>());

            txtObject.AutoCompleteCustomSource = coll;
            SetButtonState();
            chkJ2000.Checked = true;
        }


        private string FindObject(string o)
        {
            string key = null;
            if (frmUserCatalog.m_Objects.ContainsKey(o)) key = o;
            else
                if (frmUserCatalog.m_Objects.ContainsKey(o.ToUpper()))
                    key = o.ToUpper();
                else
                    if (frmUserCatalog.m_Objects.ContainsKey(o.ToLower()))
                        key = o.ToLower();

            return key;
        }

        private void txtObject_TextChanged(object sender, EventArgs e)
        {
            string key = FindObject(txtObject.Text);

            if (key!=null)
            {

                CatalogObject obj = frmUserCatalog.m_Objects[key];

                // update custom catalog entry if RA/DEC are different?
                //if (obj.Catalog == "Custom")
                //{
                //    double ra = GeminiHardware.Instance.m_Util.HMSToHours(txtRA.Text);
                //    double dec = GeminiHardware.Instance.m_Util.DMSToDegrees(txtDEC.Text);
                //    if ((ra!=0 || dec != 0) && (ra != obj.RA.RA || dec != obj.DEC.DEC))
                //    {
                //        obj.RA = new RACoord(ra);
                //        obj.DEC = new DECCoord(dec);
                //        customAdded = true;
                //    }
                //}

                if (ckRA.Checked)
                {
                    var ra = obj.RA.RA;

                    double sidereal_time = GeminiHardware.Instance.SiderealTime;
                    var ha = ConditionHA(sidereal_time - ra);
                    txtRA.Text = (Math.Sign(ha) < 0 ? "-" : "+") + GeminiHardware.Instance.m_Util.HoursToHMS(ha);
                }
                else
                   txtRA.Text = obj.RA.ToString(":", ":");
                txtDEC.Text = (Math.Sign(obj.DEC.DEC)>=0? "+":"-") + obj.DEC.ToString(":", ":");
                chkJ2000.Checked = true;    //catalogs are all in J2000
                lbCatalog.Text = obj.Catalog;
            }
            else
            {
                if (GeminiHardware.Instance.Connected)
                {
                    if (ckRA.Checked)
                        txtRA.Text = GeminiHardware.Instance.m_Util.HoursToHMS(GeminiHardware.Instance.HourAngle, ":", ":", "");
                    else
                        txtRA.Text = GeminiHardware.Instance.m_Util.HoursToHMS(GeminiHardware.Instance.RightAscension, ":", ":", "");
                    txtDEC.Text = (Math.Sign(GeminiHardware.Instance.Declination)>=0? "+":"-") + GeminiHardware.Instance.m_Util.DegreesToDMS(GeminiHardware.Instance.Declination, ":", ":", ".0");
                    if (GeminiHardware.Instance.Precession)
                        chkJ2000.Checked = true;
                    else
                        chkJ2000.Checked = false;
                    lbCatalog.Text = "Custom";
                }
                else
                {
                    //txtRA.Text = GeminiHardware.Instance.m_Util.HoursToHMS(0, ":", ":", ".0");
                    //txtDEC.Text = "+" + GeminiHardware.Instance.m_Util.DegreesToDMS(0, ":", ":", "");
                    //chkJ2000.Checked = true;
                    lbCatalog.Text = "Custom";
                }
            }

        }

        void SetButtonState()
        {
            btnGoto.Enabled = GeminiHardware.Instance.Connected;
            btnSync.Enabled = GeminiHardware.Instance.Connected;
            btnAddAlign.Enabled = GeminiHardware.Instance.Connected;
        }

        void OnConnect(bool bConnect, int clients)
        {
            SetButtonState();
        }

        private double ConditionHA(double HA)
        {
            double retVal = HA; // Set the return value to the received HA
            if (HA <= -12.0) retVal += 24.0; // It is less or equal to -12 so convert to a positive value
            if (HA > 12.0) retVal -= 24.0; // It is greater than 12.0 so convert to a negative value
            return retVal;
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            double ra = GeminiHardware.Instance.m_Util.HMSToHours(txtRA.Text);
            if (ckRA.Checked) //convert from HA
            {
                double sidereal_time = GeminiHardware.Instance.SiderealTime;
                ra = ConditionHA(sidereal_time - ra);
                if (ra >= 24) ra -= 24;
                if (ra < 0) ra += 24;
            }
            double dec =  GeminiHardware.Instance.m_Util.DMSToDegrees(txtDEC.Text);

            if (ra==0 && dec==0)
            {
                MessageBox.Show("Please select an object or enter RA/DEC coordinates!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            CatalogObject obj = null;
            string key = FindObject(txtObject.Text);
            if (key != null)
                obj = frmUserCatalog.m_Objects[key];
            else
            {
                if (!chkJ2000.Checked && !ckRA.Checked)
                {
                    ra = ra / 24 * 360.0;   // ra needs to be in degrees
                    AstronomyFunctions.ToJ2000(ref ra, ref dec);
                }
                key = frmUserCatalog.AddCustom(txtObject.Text, ra, dec);

                if (key != null) obj = frmUserCatalog.m_Objects[key];
                else
                    return;

                txtObject.Text = key;
                customAdded = true;
            }

            obj.GetCoords(out ra, out dec); //convert to correct epoch/refraction settings

            GeminiHardware.Instance.TargetRightAscension = ra;
            GeminiHardware.Instance.TargetDeclination = dec;
            GeminiHardware.Instance.TargetName = string.Format("{0} {1}", obj.Name, obj.Catalog);

            try
            {
                if (sender == btnGoto)
                    GeminiHardware.Instance.SlewEquatorial();
                else if (sender == btnSync)
                {
                    GeminiHardware.Instance.SyncEquatorial();
                    GeminiHardware.Instance.ReportAlignResult(((Button)sender).Text);
                }
                else
                {
                    GeminiHardware.Instance.AlignEquatorial();
                    GeminiHardware.Instance.ReportAlignResult(((Button)sender).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Gemini reported an error: " + ex.Message, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (customAdded) frmUserCatalog.SaveCustom();    // in case new RA/DEC objects were added
            customAdded = false;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtObject_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkJ2000_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtRA_Enter(object sender, EventArgs e)
        {
        }

        private void txtDEC_Enter(object sender, EventArgs e)
        {
        }

        private void frmRA_DEC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (customAdded) frmUserCatalog.SaveCustom();    // in case new RA/DEC objects were added
        }

        private void pbCatalog_Click(object sender, EventArgs e)
        {
            ((frmMain)this.Owner).DoCatalogManagerDialog();
        }

        private void ckRA_CheckedChanged(object sender, EventArgs e)
        {
            //chkJ2000.Checked = false;
            if (ckRA.Checked)
            {
                ckRA.Text = "HA:";

                var t = txtRA.Text;

                var ra = GeminiHardware.Instance.m_Util.HMSToHours(t);
                double sidereal_time = GeminiHardware.Instance.SiderealTime;
                var ha = ConditionHA(sidereal_time - ra);
           
                txtRA.Mask = "#00:00:00";
                txtRA.Text = (Math.Sign(ha)<0? "-" : "+") + GeminiHardware.Instance.m_Util.HoursToHMS(ha);
                
            }
            else
            {
                ckRA.Text = "RA:";

                var t = txtRA.Text;           

                var ha = GeminiHardware.Instance.m_Util.HMSToHours(t);
                double sidereal_time = GeminiHardware.Instance.SiderealTime;
                var ra = sidereal_time - (ha );
                if (ra >= 24) ra -= 24;
                if (ra < 0) ra += 24;
                txtRA.Mask = "00:00:00";
                txtRA.Text =  GeminiHardware.Instance.m_Util.HoursToHMS(ra);
        
            }
        }
    }

}
