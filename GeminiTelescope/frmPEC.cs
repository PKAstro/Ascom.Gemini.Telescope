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
using System.Linq;

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
            SharedResources.SetInstance(this);
            //TestPECFile();
        }

        void tmrUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        internal string FTPGetFile(string path)
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

                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Timeout = 5000;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string r = reader.ReadToEnd();

                reader.Close();
                response.Close();
                return r;
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Except(ex);
            }
            return null;
        }

        internal string[] FTPFileList(string path)
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


 
            return files;

        }

        private void updatePECSettings()
        {
            if (!GeminiHardware.Instance.Connected) return;

            for (int i = 0; i < 3; ++i)
            {
                string pecmax = GeminiHardware.Instance.DoCommandResult("<503:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (int.TryParse(pecmax, out PECMax)) break;
            }

            string wormPeriod = GeminiHardware.Instance.DoCommandResult("<27:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            int.TryParse(wormPeriod, out PECOneWorm);
            if (PECOneWorm == 0) PECOneWorm = 6400;

            cbPECMax.Items.Clear();

            int pos = -1;
            int j = 1;

            int max = GeminiHardware.Instance.GeminiLevel < 5? PECMax : 25600;
            if (GeminiHardware.Instance.GeminiLevel > 5)
            {
                // no limit on PEC table in L6
                max = PECOneWorm * 4;   // up to 4x worm cycle
            }

            for (int i = PECOneWorm; i <= max; i += PECOneWorm, j++)
            {
                cbPECMax.Items.Add(j.ToString("00") + " x Worm  " + i.ToString() + " steps");
                if (i == PECMax) pos = j - 1;
            }
            if (pos < 0)
            {
                pos = 0;
                PECMax = PECOneWorm;
            }
            cbPECMax.SelectedIndex = pos;
        }

        private void UpdateFiles()
        {

            cbSaveAs.Items.Clear();
            string sLoad = cbLoad.Text;

            cbLoad.Items.Clear();

            string[] files = FTPFileList("PEC/*");
            if (files != null)
            {
                cbLoad.Items.AddRange(files);
                cbSaveAs.Items.AddRange(files);
            }

            cbLoad.Text = sLoad;

            updatePECSettings();

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
            chkAutoTurnOnPEC.Checked = GeminiHardware.Instance.AutoStartPEC;
            if (!GeminiHardware.Instance.Connected)
            {
                pbLoad.Enabled = false;
                pbSaveAs.Enabled = false;
                cmdOK.Enabled = false;
            }
            else
            {
                if (GeminiHardware.Instance.m_CommandQueue.Count > 20)
                {
                    GeminiHardware.Instance.AddOneMoreError();
                    return;
                }
                byte pec = GeminiHardware.Instance.PECStatus;
                chkEnablePEC.Checked = ((pec & 1) != 0);
                chkTraining.Checked = ((pec & (4 | 16)) != 0);
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
                MessageBox.Show("Please select a data file to load, first!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string fn = cbLoad.SelectedItem as string;

            GeminiHardware.Instance.DoCommand("<551:\\PEC\\" + cbLoad.Text, false);

            MessageBox.Show("Loaded PEC file " + cbLoad.Text, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
#if false
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
            if (guideRate < 0.2 || guideRate > 0.9)
            {
                GeminiHardware.Instance.Trace.Error("frmPEC Error", $"Guide rate is invalid: {guide}, using 0.5 instead");
                guideRate = 0.5;
            }
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

            if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
            {
                string raMultiplier = GeminiHardware.Instance.DoCommandResult("<401:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                int m = 0;
                int.TryParse(raMultiplier, out m);
                if ((m & 1) != 0)
                {
                    stepsPerSecond *= 4;        //account for 4x multiplier
                    arcSecondsPerStep /= 4;
                }
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
            if (PECOneWorm == 0) PECOneWorm = 6400;
            pecChart.currentPE = new PEClass()
            {
                FileName = "gemini://" + cbLoad.Text,
                X = X.ToArray(),
                Y = Y.ToArray(),

                WormPeriodSteps = PECMax,
                WormPeriods = PECMax / PECOneWorm,
                StepX = (1.0 / stepsPerSecond)
            };

#endif

            frmPECChart pecChart = new frmPECChart();
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

        private void chkEnablePEC_CheckedChanged(object sender, EventArgs e)
        {

        }

        const double SiderealRate = 15.041068558362671; // 360 / 23 h 56 m 04.091s 

        private void chkAutoTurnOnPEC_Click(object sender, EventArgs e)
        {
            chkAutoTurnOnPEC.Checked = !chkAutoTurnOnPEC.Checked;
            GeminiHardware.Instance.AutoStartPEC = chkAutoTurnOnPEC.Checked;
        }

        class PEC
        {
            public uint pos;
            public uint count;
            public uint timer;
        }

        List<PEC> PECList = new List<PEC>();
        PEC[] PECarray = null;
        uint PECmax = 0;       
        uint RA_SidTracking_Timer = 448770;
        int PEC_Tracking_Timer = 0;
        int PECTrackDivisor = 0;
        int PECEncoder = 1;
        int PECOffset = 0;
        private double arcsecsPerStep;

        void TestPECFile()
        {
            string f = FTPGetFile("PEC/Titan_x_1.pec");
            if (f == null) return;

            var lines = f.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            PECList = new List<PEC>();
            for (int i=0; i<lines.Length; ++i)
            {
                if (lines[i].StartsWith("#") || !lines[i].Contains(","))
                {
                    if (lines[i].StartsWith("PECMax"))
                    {
                        PECmax = uint.Parse(lines[i].Substring(7));
                    }
                    else
                    if (lines[i].StartsWith("#PECTrackDivisor"))
                    {
                        PECTrackDivisor = int.Parse(lines[i].Substring(17));
                        RA_SidTracking_Timer = (uint)PECTrackDivisor;
                    }
                    else if (lines[i].StartsWith("#0x Mode") ||
                        lines[i].StartsWith("#1x Mode"))
                    {
                        PECEncoder = 1;
                    }
                    else if (lines[i].StartsWith("#4x Mode"))
                    {
                        PECEncoder = 4;
                    }
                    else if (lines[i].StartsWith("PECWormOffset"))
                    {
                        PECOffset = int.Parse(lines[i].Substring(14));
                    }
                }
                else
                {
                    var t = lines[i].Split(new char[] { '\t', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    uint pos = uint.Parse(t[0]);
                    uint rep = uint.Parse(t[1]);
                    uint div = uint.Parse(t[2]);
                    PECList.Add(new PEC() { pos = pos, count = rep, timer = div });
                }
            }
            PECarray = PECList.ToArray();

            EliminatePECDrift();


            arcsecsPerStep = SiderealRate / (12e6 / PECTrackDivisor);
            double stepsPerWormCycle = PECMax;
            double secsPerStep = 1.0 / (12e6 / PECTrackDivisor);
            var pec = new PEClass()
            {
            };

            List<DateTime> xx = new List<DateTime>();
            List<float> yy = new List<float>();
            var startTime = new DateTime(2000, 1, 1, 1, 0, 0);

            for (int i=0; i<PECList.Count; ++i)
            {
                var x = PECList[i].pos * secsPerStep;
                xx.Add(startTime.AddSeconds(x));
                var y = (12e6 / PECList[i].timer - 12e6 / PECTrackDivisor) * PECList[i].count * arcsecsPerStep - SiderealRate;
                yy.Add((float)y);
            }
        }

        void EliminatePECDrift()
        {
            uint i, count_sum;
            double divisor_sum, sids, bals;
            //	volatile long long int test;
            count_sum = 0;
            divisor_sum = 0.0;

            for (i = 0; i < PECarray.Length; i++)
            {
                count_sum += PECarray[i].count;
                divisor_sum += PECarray[i].count * (PECarray[i].timer + 1);
            }

            sids = (double)((double)RA_SidTracking_Timer + 1.0) * (double)PECmax;  // total of sid. tracking steps CLOCK ticks
            bals = sids - (double)divisor_sum;      // minus sum of guiding steps ticks intervals = balancing steps
            i = PECmax - count_sum;    // number of "normal" tracking steps at balanced PEC timer rate
                                            //	ram_var.PEC_Tracking_Timer = (lli_g / i) - 1;	// distribute the remaining divisors to the normal steps 
            PEC_Tracking_Timer = (int)((((double)bals + 0.5) / (double)i) - 0.5);   // distribute the remaining divisors to the normal steps 

            //	// check the result
            //	test = sids - divisor_sum - i * (ram_var.PEC_Tracking_Timer +1);	// should be 0, ideally
            //	if (test > i/2)										// rounding up the tracking divisor?
            //		ram_var.PEC_Tracking_Timer++;		// yes, slower
            //	else  
            //		if ((test < 0) && (llabs(test) > i/2))
            //			ram_var.PEC_Tracking_Timer--;	// rounding downwards: faster
        }

        private void pbClearCal_Click(object sender, EventArgs e)
        {
            if (!GeminiHardware.Instance.Connected)
            {
                return;
            }
            if (string.IsNullOrEmpty(cbLoad.SelectedItem as string))
            {
                MessageBox.Show("Please select a data file to delete, first!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string fn = cbLoad.SelectedItem as string;

            var r = GeminiHardware.Instance.DoCommandResult(new string[] { ">912:\\PEC\\" + cbLoad.Text }, GeminiHardware.Instance.MAX_TIMEOUT, false);
            if (r != "0")
            {
                MessageBox.Show("Deleted file " + cbLoad.Text, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unable to delete file " + cbLoad.Text, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateFiles();
        }
    }
}
