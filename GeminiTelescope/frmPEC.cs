using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace ASCOM.GeminiTelescope
{
    public partial class frmPEC : Form
    {
        Timer tmrUpdate = new Timer();

        int PECMax = 0;
        int PECOneWorm = 0;

        public frmPEC()
        {
            InitializeComponent();
        }

        private void frmPassThroughPortSetup_Load(object sender, EventArgs e)
        {
            UpdateFiles();
            UpdateStatus();
            tmrUpdate.Tick += new EventHandler(tmrUpdate_Tick);
            tmrUpdate.Interval = 1000;
            tmrUpdate.Start();
            cbSaveAs.Text = "CurrPEC.pec";

        }

        void tmrUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }


        string[] FTPFileList(string path)
        {
            if (!GeminiHardware.Instance.Connected || !GeminiHardware.Instance.EthernetPort)
                return null;
            
            string[] files = null;

            try
            {
                string uri = GeminiHardware.Instance.EthernetIP;

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + GeminiHardware.Instance.EthernetIP + "/" + path);

                request.Credentials = new NetworkCredential(GeminiHardware.Instance.EthernetUser, GeminiHardware.Instance.EthernetPassword);

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Timeout = 5000;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string r = reader.ReadToEnd();

                if (r != null)
                    files = r.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Except(ex);
            }


            string pecmax = GeminiHardware.Instance.DoCommandResult("<503:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int.TryParse(pecmax, out PECMax);

            string wormPeriod = GeminiHardware.Instance.DoCommandResult("<27:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int.TryParse(wormPeriod, out PECOneWorm);
            cbPECMax.Items.Clear();

            int pos = -1;
            int j =1;
            for (int i = PECOneWorm; i <= 25600; i+=PECOneWorm, j++)
            {
                cbPECMax.Items.Add(j.ToString("00") + " x Worm  " + i.ToString() + " steps");
                if (i == PECMax) pos = j-1;
            }
            cbPECMax.SelectedIndex = pos;
            return files;

        }

        private void UpdateFiles()
        {

            cbSaveAs.Items.Clear();
            string sLoad = cbLoad.Text;

            cbLoad.Items.Clear();

            string[] files = FTPFileList( "PEC/*" );
            if (files != null)
            {
                cbLoad.Items.AddRange(files);
                cbSaveAs.Items.AddRange(files);
            }

            cbLoad.Text = sLoad;

            //try
            //{
            //    byte[] newFileData = request.DownloadData(serverUri.ToString());
            //    string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
            //    Console.WriteLine(fileString);
            //}
            //catch (WebException e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            //return true;

        }


        private void UpdateStatus()
        {
            if (!GeminiHardware.Instance.Connected)
            {
                pbLoad.Enabled = false;
                pbSaveAs.Enabled = false;
                cmdOK.Enabled = false;
            }
            else
            {
                byte pec = GeminiHardware.Instance.PECStatus;
                chkEnablePEC.Checked = ((pec & 1) != 0);
                chkTraining.Checked = ((pec & (4 | 16)) !=0);
                chkDataAvailable.Checked = ((pec & 32) != 0);

                if ((pec & 4) == 0 && progress.Visible)
                {
                    progress.Visible = false;
                    percent.Visible = false;
                    cmdOK.Text = "Start PEC Training";
                }

                if ((pec & 8) != 0)
                    lbStatus.Text = "Training Completed";
                else
                    if ((pec & 4) != 0)
                    {
                        lbStatus.Text = "Training in progress";
                        UpdateCounter();
                    }
                    else
                        if ((pec & 16) != 0)
                            lbStatus.Text = "Training will start soon";
                        else
                            if ((pec & 1) != 0)
                                lbStatus.Text = "PEC playback enabled";
                            else if ((pec & 32) != 0)
                                lbStatus.Text = "PEC Data available";
                            else
                                lbStatus.Text = "";            

                if (GeminiHardware.Instance.GeminiLevel < 5)
                {
                    pbLoad.Enabled = false;
                    pbSaveAs.Enabled = false;
                    cmdOK.Enabled = false;
                }
                else if (!GeminiHardware.Instance.EthernetPort)
                {
                    pbLoad.Enabled = false;
                    pbSaveAs.Enabled = false;
                }

            }

        }

        private void UpdateCounter()
        {
            int pos = 0;
            try
            {
                string pecstep = GeminiHardware.Instance.DoCommandResult("<501:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (int.TryParse(pecstep, out pos))
                {
                    if (pos > 0)
                    {
                        if (!progress.Visible)
                        {
                            progress.Maximum = PECMax;
                            progress.Visible = true;
                            percent.Visible = true;
                            percent.Text = "0%";
                        }
                        progress.Value = pos;
                        int p = ((100 * pos) / PECMax);
                        percent.Text = p.ToString() + "%";
                    }
                }
            }
            catch { }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            // if waiting for PEC training or PEC training in progress, the user wants to stop!
            if ((GeminiHardware.Instance.PECStatus & (4 + 16)) != 0)
            {
                GeminiHardware.Instance.DoCommand(">535:", false);
                cmdOK.Text = "Start PEC Training";
                
            }
            else
            {
                GeminiHardware.Instance.DoCommand(">530:", false);
                cmdOK.Text = "STOP PEC Training";

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!GeminiHardware.Instance.Connected)
            {
                return;
            }
            if (cbSaveAs.Text == "")
            {
                cbSaveAs.Text = "CurrPEC.pec";
            }

            if ((GeminiHardware.Instance.PECStatus & 32) == 0)
            {
                MessageBox.Show("No PEC data available to save. Please train PEC first.", "No PEC Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (cbLoad.Items.Contains(cbSaveAs.Text))
            {
                DialogResult res = MessageBox.Show("PEC file " + cbSaveAs.Text + " already exists.\r\nDo you want to overwrite it?", "PEC Overwrite Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (res != DialogResult.Yes)
                    return;
            }

            GeminiHardware.Instance.DoCommand(">551:\\PEC\\" + cbSaveAs.Text, false);
            MessageBox.Show("Saved PEC file " + cbSaveAs.Text, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateFiles();
        }

        private void chkEnablePEC_Click(object sender, EventArgs e)
        {
            chkEnablePEC.Checked = !chkEnablePEC.Checked;

            byte pec = GeminiHardware.Instance.PECStatus;
            if (pec != 0xff)
            {
                pec = (byte)((pec & 0xfe) | (chkEnablePEC.Checked ? 1 : 0));
                GeminiHardware.Instance.PECStatus = pec;
            }

        }

        private void cmdOK_Validating(object sender, CancelEventArgs e)
        {
        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            if (!GeminiHardware.Instance.Connected)
            {
                return;
            }
            if (string.IsNullOrEmpty(cbLoad.SelectedItem as string))
            {
                MessageBox.Show("Please select a data file to load, first!", SharedResources.TELESCOPE_DRIVER_NAME,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string fn = cbLoad.SelectedItem as string;

            GeminiHardware.Instance.DoCommand("<551:\\PEC\\" + cbLoad.Text, false);

            MessageBox.Show("Loaded PEC file "  + cbLoad.Text, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateFiles();
        }

        private void SetPECMax_Click(object sender, EventArgs e)
        {
            if (cbPECMax.SelectedIndex >= 0)
            {
                int pos = (cbPECMax.SelectedIndex + 1) * PECOneWorm;
                GeminiHardware.Instance.DoCommand(">503:" + pos.ToString(), false);
                UpdateFiles();
                UpdateStatus();

                MessageBox.Show("PEC period is set to " + (cbPECMax.SelectedItem.ToString()), SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            frmProgress.Initialize(0, 100, "Getting PEC Data from Gemini", null);
            frmProgress.ShowProgress(null);

            List<float> Y = new List<float>();
            List<DateTime> X = new List<DateTime>();
     
            float y = 0;
            DateTime x = new DateTime(2000,1,1,1,0,0);

            X.Add(x);
            Y.Add(y);

            string guide = GeminiHardware.Instance.DoCommandResult("<502:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            double guideRate = 0.5;            
            GeminiHardware.Instance.m_Util.StringToDouble(guide, out guideRate);

            string rateDivisor = GeminiHardware.Instance.DoCommandResult("<411:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            double arcSecondsPerStep = 0;
            double stepsPerSecond  = 0;
            if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
            {

                double frequency = 12000000;
                stepsPerSecond = (frequency) / double.Parse(rateDivisor);
                arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));
            }


            for (int i = 0; i < PECMax; )
            {
                string res = GeminiHardware.Instance.DoCommandResult("<511:" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (res != null)
                {
                    string[] p = res.Split(';');
                    int rep = 0, v = 0;

                    int.TryParse(p[0], out v);
                    int.TryParse(p[1], out rep);
                    if (rep <= 0) break;

                    if (v == 1) v = -1;
                    else
                        if (v == 8) v = 1;
                        else v = 0;

                    x = x.AddSeconds((double)rep / stepsPerSecond);
                    i += rep;
                    y += (float)((rep * guideRate * v * (v > 0 ? 1.0 / (1.0 + guideRate) : 1.0 / (1.0 - guideRate))) * arcSecondsPerStep);
                    X.Add(x);
                    Y.Add(y);

                    frmProgress.Update((i*100)/PECMax, null);
                }
                else break;
            }

            frmPECChart pecChart = new frmPECChart();
            pecChart.X = X.ToArray();
            pecChart.Y = Y.ToArray();
            pecChart.UpdateData();
            frmProgress.HideProgress();

            pecChart.Show(this);
        }

        private void pbClear_Click(object sender, EventArgs e)
        {
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v & (~32)).ToString(), false);
                UpdateStatus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string res = GeminiHardware.Instance.DoCommandResult("<509:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int v = 0;
            if (int.TryParse(res, out v))
            {
                GeminiHardware.Instance.DoCommand(">509:" + (v | 33).ToString(), false);
                UpdateStatus();
            }

        }

    }
}
