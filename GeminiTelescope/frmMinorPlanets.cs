using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using MathNet;
using MathNet.Numerics.Interpolation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
//using System.Threading;

namespace ASCOM.GeminiTelescope
{
    public partial class frmMinorPlanets : Form
    {
        private bool customAdded = false;

        HorizonsAPI.Ephemeris currentObject = null;

        Timer tmUpdateDesc = null;

        TimeSpan timeOffset = TimeSpan.FromSeconds(0);
        internal bool supported
        {
            get {

                if (GeminiHardware.Instance.dVersion >= 5.0) return true;
#if DEBUG
                return true;
#endif
                return false;
            }
        }
        internal bool connected
        {
            get
            {
                if (GeminiHardware.Instance.Connected) return true;
#if DEBUG
                return true;
#endif
                return false;

            }
        }
        bool _tracking = false;

        Timer tmUpdateTrack = null;

        //https://en.wikipedia.org/wiki/Earth%27s_rotation
        const double SiderealRate = 15.041068640261926; // 360 / 23 h 56 m 4.09053083288s 

        double maxSlewRate = 0; //deg per second

        double RAOffset, DECOffset;

        internal bool tracking {

            get {
                return _tracking;
            }
            set
            {
                if (value)
                {
                    if (currentObject == null) {
                        tmUpdateTrack.Stop();
                        _tracking = false;
                        lblStatus.Text = "No object to track!";
                        SetButtonState();
                        GeminiHardware.Instance.Trace.Exit("MP:tracking", "No object to track!");

                        return;
                    }

                    previous_stepsPerDegree = -1;    // recompute mount parameters at the start of tracking

                

                    double dRA, dDec, ra, dec;
                    bool rateAvailable = currentObject.GetCurrent(currentTimeUTC, out ra, out dec, out dRA, out dDec);

                    GeminiHardware.Instance.Trace.Info(2, "MP:tracking", $"GetCurrent={rateAvailable}", ra.ToString(), dec.ToString(), dRA.ToString(), dDec.ToString());

                    if (rateAvailable)
                    {
                        if (tmUpdateTrack == null)
                        {
                            tmUpdateTrack = new Timer();
                            tmUpdateTrack.Tick += TmUpdateTrack_Tick;
                        }
                        tmUpdateTrack.Stop();

                        maxSlewRate = SlewRate()*2;

                        RAOffset = GeminiHardware.Instance.RightAscension - ra; // starting offset
                        DECOffset = GeminiHardware.Instance.Declination - dec;

                        GeminiHardware.Instance.Trace.Info(2, "MP:tracking", "Offsets", RAOffset.ToString(), DECOffset.ToString());

                        VariableMoveAxisRA((SiderealRate - dRA) / 3600.0, true);
                        VariableMoveAxisDEC(-dDec/3600, true);

                        double drate = currentObject.maxRateChange; //arcsec/sec
                        drate = (0.5625 / 4) / drate;  //assume g11 step size at 4x resolution
                        drate /= 2; // update twice per interval

                        drate = Math.Floor(drate);

                        if (drate < 0.5) drate = 0.5;

                        if (drate > 1) drate = 2;
                        tmUpdateTrack.Interval = (int)(1000 * drate);
                        tmUpdateTrack.Start();
                        GeminiHardware.Instance.HorizonsTracking = true;
                        _tracking = true;                        
                        GeminiHardware.Instance.Trace.Info(2, "MP:tracking", "Interval", tmUpdateTrack.Interval.ToString());

                    }
                }
                else
                {
                    tmUpdateTrack.Stop();
                    GeminiHardware.Instance.HorizonsTracking = false;
                    _tracking = false;
                    txtDEC.Text = "";
                    txtRA.Text = "";
                    VariableMoveAxisDEC(0);
                    VariableMoveAxisRA(0);
                    GeminiHardware.Instance.DoCommand(">131:", false); // Set Sidereal
                    lblStatus.Text = "Not tracking.";
                    GeminiHardware.Instance.Trace.Info(2, "MP:tracking", "Stopped");
                }

                SetButtonState();
            }
        }

        private void updateErrors(double target_ra, double target_dec, out double e_ra, out double e_dec)
        {
            e_ra = e_dec = 0;
            if (supported && connected)
            {
                double ra = GeminiHardware.Instance.RightAscension;
                double dec = GeminiHardware.Instance.Declination;
                txtRAError.Text = GeminiHardware.Instance.m_Util.HoursToHMS(e_ra = target_ra - ra, ":", ":", "");
                txtDECError.Text = GeminiHardware.Instance.m_Util.DegreesToDMS(e_dec = target_dec - dec, ":",":","");

                GeminiHardware.Instance.Trace.Info(2, "MP:updateErrors", txtRAError.Text, txtDECError.Text);
            }
        }


        private void TmUpdateTrack_Tick(object sender, EventArgs e)
        {
            double dRA, dDec, ra, dec;
            ra = dec = dRA = dDec = 0;
            bool? rateAvailable = currentObject?.GetCurrent(currentTimeUTC, out ra, out dec, out dRA, out dDec);

            GeminiHardware.Instance.Trace.Info(2, "MP:UpdateTrack", $"GetCurrent={rateAvailable}", ra.ToString(), dec.ToString(), dRA.ToString(), dDec.ToString());

            if (currentObject == null || !(bool)rateAvailable)
            {
                tracking = false;
                tmUpdateTrack.Stop();
                return;
            }

            ra += RAOffset;
            dec += DECOffset;

            double err_ra = 0, err_dec = 0;
            updateErrors(ra, dec, out err_ra, out err_dec);

            if (ckClosedLoop.Checked)
            {
               // err_ra *= 360.0 / 24.0;

                GeminiHardware.Instance.Trace.Info(2, "MP:UpdateTrack", $"SideOfPier", GeminiHardware.Instance.m_SideOfPier);
                if (GeminiHardware.Instance.m_SideOfPier == "W")
                    err_dec = -err_dec;

                //err_ra *= 0.2;
                err_dec *= 0.2;
            }
            else
            {
                err_dec = err_ra = 0;
                GeminiHardware.Instance.Trace.Info(2, "MP:UpdateTrack", "ClosedLoop turned OFF");
            }


            //GeminiHardware.Instance.Trace.Info(2, "MP:UpdateTrack", $"SideOfPier", GeminiHardware.Instance.m_SideOfPier);
            if (GeminiHardware.Instance.m_SideOfPier == "E")
                dDec = -dDec;
            //dRA = -dRA;
            var t = new Task(() =>
            {
                VariableMoveAxisRA((SiderealRate - dRA) / 3600.0 - err_ra);
                VariableMoveAxisDEC(dDec/3600 - err_dec);
                this.BeginInvoke(new Action(() => lblStatus.Text = $"Tracking {currentObject.name}"));
            });
            t.Start();


            txtRA.Text = GeminiHardware.Instance.m_Util.HoursToHMS(ra, ":", ":", "");
            txtDEC.Text = GeminiHardware.Instance.m_Util.DegreesToDMS(dec, ":", ":", "");
            txtdRA.Text = $"{dRA:0.000000}";
            txtdDEC.Text = $"{dDec:0.000000}";

            GeminiHardware.Instance.Trace.Exit("MP:UpdateTrack", txtRA.Text, txtDEC.Text, txtdRA.Text, txtdDEC.Text);
        }

        public frmMinorPlanets()
        {
            InitializeComponent();
        }

        string[] mp_Names = null;

        private void frmMinorPlanets_Load(object sender, EventArgs e)
        {
            if (mp_Names == null)
            {
                frmProgress.Initialize(0, 100, "Load JPL Catalog...", null);
                frmProgress.ShowProgress(this);

                string path = "";
                try
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASCOM\\" + SharedResources.TELESCOPE_DRIVER_NAME;
                    System.IO.Directory.CreateDirectory(path);
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASCOM\\" + SharedResources.TELESCOPE_DRIVER_NAME + "\\Catalogs";
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {
                }

                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASCOM\\" + SharedResources.TELESCOPE_DRIVER_NAME + "\\Catalogs";


                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.FileInfo[] files = di.GetFiles("jpl*.txt");

                mp_Names = new string[0];

                foreach (var fi in files)
                {
                    try
                    {
                        string cn = fi.Name.Substring(0, fi.Name.Length - 4);
                        frmProgress.Update(30, "Loading " + cn + "...");
                        var lines = File.ReadAllLines(fi.FullName);
                        mp_Names = mp_Names.Union(lines).ToArray();
                    }
                    catch { }
                }

                var scout = GetScoutNames();
                if (scout != null) mp_Names = mp_Names.Union(scout).ToArray();

                frmProgress.Update(30, "Verifying PC clock");
                ComputeTimeDifference();

                frmProgress.HideProgress();


                GeminiHardware.Instance.OnConnect += new ConnectDelegate(OnConnect);

                if (!connected) lblStatus.Text = "Gemini not connected";
                else EnsureSupported();

                tmUpdateDesc = new Timer();
                tmUpdateDesc.Interval = 5000;
                tmUpdateDesc.Tick += TmUpdateDesc_Tick;
                tmUpdateDesc.Start();

                CleanUpEphemeris();

                LoadEphemeris(null);    //load last saved eph, if available
            }

            SetButtonState();

            SharedResources.SetInstance(this);
        }

        private List<string> GetScoutNames()
        {
            try
            {
                var r = getWeb($"{urlScoutGet}");
                if (r != null)
                    return HorizonsAPI.Parse.ExtractScoutAllObjects(r);
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Error("GetScoutNames", ex);
                lblStatus.Text = "Can't fetch SCOUT NEO objects";
            }
            return new List<string>();

        }

        private void EnsureSupported()
        {
            if (connected && !supported)
            {
                MessageBox.Show($"Your Gemini L{GeminiHardware.Instance.dVersion} does not support custom object tracking. Please upgrade to L6 or higher!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (connected && (GeminiHardware.Instance.m_Refraction || GeminiHardware.Instance.m_Precession))
            {
                var r = MessageBox.Show($"Gemini settings indicate J2000 or refraction calculation is on. These settings need to change for proper tracking using this tool.\r\n\r\nChange them now?", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (r == DialogResult.Yes)
                {
                    GeminiHardware.Instance.m_Refraction = false;
                    GeminiHardware.Instance.Precession = false;
                }
            }

        }

        private void ComputeTimeDifference()
        {
            try
            {
                var tm = new NistClock();

                var diff = tm.MeasureSystemClockError();
#if DEBUG
                diff = TimeSpan.FromSeconds(0);
#endif
                timeOffset = diff;

                txtDesc.Text += $"Measured PC time offset: {diff:d\\ \\d\\a\\y\\s\\ \\ hh\\:mm\\:ss\\.fff} seconds\r\n";

                GeminiHardware.Instance.Trace.Exit("MP:ComputeTimeDifference", txtDesc.Text);

                if (Math.Abs(diff.TotalSeconds) > 5)
                {
                    MessageBox.Show($"Your PC clock is off by {diff:d\\ \\d\\a\\y\\s\\ \\ hh\\:mm\\:ss\\.f} seconds.\r\n\r\nPlease update before proceeding.", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch { }
        }

        private void TmUpdateDesc_Tick(object sender, EventArgs e)
        {
            if (currentObject != null && this.Visible)
            {
                UpdateDescription(currentObject);
            }
        }

        DateTime currentTimeUTC
        {
            get
            {
                return DateTime.UtcNow + timeOffset; ;
            }
        }

        static string prev_name = "";

        Timer tmTextChange = null;

        static string current_name = "";

        private void txtObject_TextChanged(object sender, EventArgs e)
        {
            current_name = txtObject.Text;
            if (txtObject.Text == "[PASTE]")
            {
                string t = Clipboard.GetText();
                if (t.Contains("\n"))   // assume TLE
                {
                    tle = t;
                    ProcessTLE(sender, e);
                    return;
                }
                else
                {
                    txtObject.Text = t;
                    return;
                }
            }

            if (tmTextChange == null)
            {
                tmTextChange = new Timer();
                tmTextChange.Interval = 200;
                tmTextChange.Tick += TmTextChange_Tick;
            }
            tmTextChange.Stop();
            tmTextChange.Start();

            return;
        }

        private void TmTextChange_Tick(object sender, EventArgs e)
        {
            tmTextChange.Stop();
            try
            {

                string name = current_name;
                if (name.Length < 2 || prev_name == name) return;


                //txtObject.SelectionStart = name.Length;
                int idx = txtObject.SelectionStart;

                Cursor.Current = Cursors.Default;


                var n = name.ToLower();
                var q = mp_Names.Where(x => x.ToLower().Contains(n));
                //txtObject.DataSource = null;
                //txtObject.DataSource = q.ToList();

                SuspendDrawing(txtObject);
                if (q.Count() > 0 || txtObject.Items.Count > 0)
                {
                    txtObject.Items.Clear();
                    txtObject.Items.AddRange(q.ToArray());
                }
                //txtObject.SelectionStart = name.Length;
                prev_name = name;
                txtObject.Text = name;
                if (q.Count() == 1 && q.First().ToLower() == n)
                    txtObject.DroppedDown = false;
                else
                if (q.Count() > 0)
                    txtObject.DroppedDown = true;
                else
                    txtObject.DroppedDown = false;

                if (idx <= txtObject.Text.Length)
                    txtObject.SelectionStart = idx;

                ResumeDrawing(txtObject);
            }
            catch (Exception ex)
            {
            }
        }

        void SetButtonState()
        {
            bool visible = false;

            if (currentObject != null)
                visible = currentObject.Visible(currentTimeUTC);

            btnGoto.Enabled = (visible && connected);

            btnTrack.Enabled = (supported && visible && !tracking && connected);
            btnTrack.Text = tracking ? "Tracking" : "Start Track";
            btnStop.Enabled = (visible && tracking && connected);

            btnRaw.Enabled = ((currentObject != null && !string.IsNullOrEmpty(currentObject.raw)) || last_result != "");

            btnTLE.Enabled = (!tracking || !connected);
           
        }

        void OnConnect(bool bConnect, int clients)
        {
            this.BeginInvoke(new Action(() =>
            {
                SetButtonState();

                if (this.Visible && connected && !supported)
                {
                    MessageBox.Show($"Your Gemini L{GeminiHardware.Instance.dVersion} does not support custom object tracking. L6 or later is required.", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                lblStatus.Text = bConnect ? "Gemini Connected!" : "Gemini not connected";
            }));
        }

        private double ConditionHA(double HA)
        {
            double retVal = HA; // Set the return value to the received HA
            if (HA <= -12.0) retVal += 24.0; // It is less or equal to -12 so convert to a positive value
            if (HA > 12.0) retVal -= 24.0; // It is greater than 12.0 so convert to a negative value
            return retVal;
        }

        private void btnTrack_Click(object sender, EventArgs e)
        {
            tracking = true;
        }
    
        private void txtRA_Enter(object sender, EventArgs e)
        {
        }

        private void txtDEC_Enter(object sender, EventArgs e)
        {
        }

        private void frmMinorPlanets_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }

        string urlHorizonsLookup = @"https://ssd.jpl.nasa.gov/api/horizons_lookup.api";
        string urlHorizonsGet = @"https://ssd.jpl.nasa.gov/api/horizons.api";
        string urlScoutGet = @"https://ssd-api.jpl.nasa.gov/scout.api";
        class mb_object
        {
            public string name;
            public string spkid;
            public string uid;
            public override string ToString()
            {
                return $"{name}: spkid={spkid}/uid={uid}";
            }
        }

       
        private string FindHorizons(string name)
        {
            GeminiHardware.Instance.Trace.Info(2, "MP:FindHorizons", name);
            string r = "";
            
            lblStatus.Text = $"Downloading ephemeris for {name}...";
            lblStatus.Update();
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                bool sb = !name.Contains("[MB]");
                var eph = LoadEphemeris(name, sb);
 
                    //txtObject.Items.Clear();
                if (string.IsNullOrEmpty(eph) || eph.Contains("No matches found."))
                {
                    r = getWeb($"{urlHorizonsLookup}?sstr='{HttpUtility.UrlEncode(name)}'");

                    HorizonsAPI.Lookup res = JsonConvert.DeserializeObject<HorizonsAPI.Lookup>(r);
                    if (res == null || res.result == null || res.result.Count == 0)
                    {
                        if (name.Contains("/"))
                        {
                            var nn = name.Split('/');
                            r = getWeb($"{urlHorizonsLookup}?sstr='{HttpUtility.UrlEncode(nn[0].Trim())}'");
                            res = JsonConvert.DeserializeObject<HorizonsAPI.Lookup>(r);
                            if (res == null || res.result == null || res.result.Count == 0)
                            {
                                MessageBox.Show("No matching objects found at JPL Horizons!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return "";
                            }
                        }
                    }
                    var list = (res.result.Select(x => new mb_object
                    {
                        name = x.name,
                        spkid = x.spkid,
                        uid = getUid(x.name)
                    }).ToArray());


                    return LoadEphemeris(list.FirstOrDefault(), true);
                }
                else
                    return eph;
            }
            catch { }
            finally
            {
                lblStatus.Text = "";
                Cursor.Current = Cursors.Default;

            }
            return r;
        }

        private void txtObject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (txtObject.SelectedIndex < 0 && txtObject.Items.Count > 0)
            //    txtObject.SelectedIndex = 0;
        }

        private string LoadEphemeris(string n, bool sb)
        {
            var idx = n.IndexOf(' ');
            if (n.ToLower().Contains("[sc]"))
            {
                return LoadScoutEphemeris(n.Replace("[SC]","").Replace("[sc]", "").Trim(' '));
            }
            if (idx > 0 && (char.IsDigit(n[0]) || n[0]=='-'))
            {
                n = n.Substring(0, idx) + (sb? "%3B" : "");
                return LoadEphemeris(new mb_object() { name = n, spkid = n, uid = n }, false);
            }
            return LoadEphemeris(new mb_object() { name = n, spkid = n, uid = n }, false);
        }

        private string LoadScoutEphemeris(string n)
        {
            try
            {
                txtDesc.Text = "";
                var cmd = n;

                //https://ssd-api.jpl.nasa.gov/scout.api?tdes=P10vY9r&eph-start=2016-09-25T18:00:00&eph-stop=2016-09-26T06:00:00&eph-step=2h

                var u = $"{urlScoutGet}?tdes={cmd}";
                u += $"&eph-start={DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", GeminiHardware.Instance.m_GeminiCulture)}";
                u += $"&eph-stop={DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ss", GeminiHardware.Instance.m_GeminiCulture)}";
                u += $"&eph-step=1h";

                GeminiHardware.Instance.Trace.Info(2, "MP:LoadScoutEphemeris", u);

                var r = getWeb(u);
                last_result = r;

                return r;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Failed to get JPL Horizons ephemeris!\r\n\r\n" + ex.Message, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                GeminiHardware.Instance.Trace.Info(2, "MP:LoadEphermis", $"Error: {ex.Message}");

            }
            return null;
        }

        private string dbl(double d)
        {
            return d.ToString(GeminiHardware.Instance.m_GeminiCulture);
        }

        string last_result;

        private string LoadEphemeris(mb_object obj, bool spk)
        {
            try
            {
                txtDesc.Text = "";
                string tle_text = "";
                var cmd = spk ? $"DES={obj.spkid}" : obj.name;
                if (obj.name.StartsWith("[TLE] "))
                {
                    cmd = "TLE";
                    tle_text = $"&TLE='{tle}'";
                }

                var u = $"{urlHorizonsGet}?format=text&CSV_FORMAT=YES&COMMAND='{cmd}'{tle_text}";
                u += $"&MAKE_EPHEM=YES&EPHEM_TYPE=OBSERVER&CENTER=coord";
                u += $"&STEP_SIZE=1m&COORD_TYPE=GEODETIC&OBJ_DATA=NO&EXTRA_PREC=YES";
                u += $"&SITE_COORD='{dbl(GeminiHardware.Instance.Longitude)},{dbl(GeminiHardware.Instance.Latitude)},{dbl(GeminiHardware.Instance.Elevation / 1000)}'";
                u += $"&START_TIME='{DateTime.UtcNow.ToString("yyyy-MMM-dd HH:mm:ss.fff", GeminiHardware.Instance.m_GeminiCulture)}'";
                u += $"&STOP_TIME='{DateTime.UtcNow.AddHours(24).ToString("yyyy-MMM-dd HH:mm:ss.fff", GeminiHardware.Instance.m_GeminiCulture)}'";
                u += $"&TIME_DIGITS=SECONDS&QUANTITIES='1,2,3,4,29'";

                GeminiHardware.Instance.Trace.Info(2, "MP:LoadEphermis", u);

                var r = getWeb(u);
                last_result = r;
                
                return r;
            }
            catch (Exception ex) {

                MessageBox.Show("Failed to get JPL Horizons ephemeris!\r\n\r\n"+ex.Message, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                GeminiHardware.Instance.Trace.Info(2, "MP:LoadEphermis", $"Error: {ex.Message}");

            }
            return null;
        }

        private string getUid(string name)
        {
            if (char.IsDigit(name[0]))
            {
                var s = name.Split(' ');
                if (s.Length > 1)
                    return s[0];
            }

            return "";
        }

        string getWeb(string url)
        {
            int retries = 3;
retry:
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = null;

            request.Headers = new WebHeaderCollection();
            request.Headers.Add("Accept-Language: en-US");

            // all the following is to mitigate 'connection unexpectedly closed' error on Windows 7
            request.KeepAlive = false;
            System.Net.ServicePointManager.Expect100Continue = false;
            if (GeminiHardware.Instance.BypassProxy) request.Proxy = null;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;


            request.Timeout = 20000;
            request.ReadWriteTimeout = 20000;
            request.ContinueTimeout = 20000;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                if (retries-- > 0)
                {
                    System.Threading.Thread.Sleep(100);
                    goto retry; // Still Windows 7 retry mitigation, since connection errors occur randomly
                }
                GeminiHardware.Instance.OnError?.Invoke("Minor Planet", ex.Message);
                return null;
            }

            return getWebResult(response, 20000); 
        }

        private string getWebResult(HttpWebResponse response, int timeout)
        {
            if (response == null) return null;

            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = GeminiHardware.Instance.m_BinaryEncoding.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            response.Close();
            response = null;
            return sb.ToString();
        }

        internal bool isScout { get { return current_name.ToLower().Contains("[sc]"); } }

        private void btnFind_Click(object sender, EventArgs e)
        {

try_again:
            if (tracking)
            {
                GeminiHardware.Instance.Trace.Info(2, "MP:btnFind", "Can't change object selection while tracking");
                MessageBox.Show("Can't change object selection while tracking", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }
            txtDesc.Clear();
            SetButtonState();
            currentObject = null;
            var r = FindHorizons(current_name);
            if (string.IsNullOrEmpty(r) || r.StartsWith("{\"error\":"))
            {
                lblStatus.Text = $"Object not found: {current_name}";
                return;
            }
            if (r.Contains("Matching small-bodies:")) //multiple matches, need to disambiguate
            {
                GeminiHardware.Instance.Trace.Info(2, "MP:btnFind", $"Multiple matches in JPL Horizons database! Please pick a unique ID or name.");
                lblStatus.Text = $"Multiple objects found: {current_name}";

                var choice = HorizonsAPI.Parse.ExtractMultiple(r);
                if (choice != null && choice.Count > 0)
                {
                    frmJPLMultiple f = new frmJPLMultiple();
                    f.choices = choice;
                    var res = f.ShowDialog(this);
                    if (res == DialogResult.OK)
                    {
                        txtObject.Text = f.recordNumber;
                        goto try_again;
                    }
                }
                
                MessageBox.Show($"Multiple matches in JPL Horizons database! Please pick a unique ID or name.", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }
            else if (string.IsNullOrEmpty(r) || r.StartsWith("{\"count\":\"0\"") || r.Trim(' ', '\n').EndsWith("\"count\":\"0\"}"))
            {
                GeminiHardware.Instance.Trace.Info(2, "MP:btnFind", $"Object '{current_name}' not found in JPL Horizons database");
                lblStatus.Text = $"Object not found: {current_name}";
                MessageBox.Show($"Object '{current_name}' not found in JPL Horizons database", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                HorizonsAPI.Ephemeris eph = null;

                if (isScout && !r.Contains("API SOURCE: NASA/JPL Horizons API"))
                {
                    eph = HorizonsAPI.Parse.ExtractScout(r);
                    AdjustScoutEphemeris(eph);
                }
                else
                    eph = HorizonsAPI.Parse.Extract(r);

                currentObject = eph;

                UpdateDescription(eph);

                SaveEphemeris(txtObject.Text, r);
                lblStatus.Text = $"Ephemeris loaded for: {current_name}";

            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Info(2, "MP:btnFind", $"Error processing ephemeris: {ex.Message}");
                MessageBox.Show($"Error processing ephemeris!\r\n\r\n{ex.Message}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            SetButtonState();

        }

        private void AdjustScoutEphemeris(HorizonsAPI.Ephemeris eph)
        {
            eph.name = current_name;

            foreach(var t in eph.data)
            {

                //convert to JNow from J2000 (Scout)
                double ra = t.ra / 360 * 24;    //convert to hours
                double dec = t.dec;

                AstronomyFunctions.FromJ2000(ref ra, ref dec);
                
                t.ra = t.ra_apparent = ra / 24 * 360.0;
                t.dec = t.dec_apparent = dec;

                // SCOUT data doesn't include alt/az for this location -- compute it from RA/DEC
                t.alt = AstronomyFunctions.CalculateAltitude(t.ra, t.dec, GeminiHardware.Instance.Latitude, GeminiHardware.Instance.Longitude);
                t.az = AstronomyFunctions.CalculateAzimuth(t.ra, t.dec, GeminiHardware.Instance.Latitude, GeminiHardware.Instance.Longitude);
                t.elevation = t.alt;
            }
        }

        private void SaveEphemeris(string fname, string ephemeris)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path = Path.Combine(path, "GeminiNET");

            Directory.CreateDirectory(path);
            path = Path.Combine(path, "ephemeris");
            Directory.CreateDirectory(path);

            fname = fname.Replace('/', '_').Replace('\\', '_').Replace(':', '_') + ".eph";

            DirectoryInfo di = new DirectoryInfo(path);

            // delete files older than 24 hours
            var delete = di.EnumerateFiles("*.eph").Where(x => x.LastWriteTime < DateTime.Now - TimeSpan.FromHours(24));
            for (int i = 0; i < delete.Count(); ++i)
            {
                var x = delete.ElementAt(i);
                try
                {
                    File.Delete(x.FullName);                   
                }
                catch { }
            }
            path = Path.Combine(path, fname);
            try
            {
                File.WriteAllText(path, ephemeris);
            } catch (Exception ex)
            {
                MessageBox.Show($"Failed to save ephemeris to {path}:\r\n\r\n{ex.Message}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        void CleanUpEphemeris()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path = Path.Combine(path, "GeminiNET");

            Directory.CreateDirectory(path);
            path = Path.Combine(path, "ephemeris");
            Directory.CreateDirectory(path);

            DirectoryInfo di = new DirectoryInfo(path);

            // delete files older than 24 hours
            var delete = di.EnumerateFiles("*.eph").Where(x => x.LastWriteTime < DateTime.Now - TimeSpan.FromHours(24));
            for (int i = 0; i < delete.Count(); ++i)
            {
                var x = delete.ElementAt(i);
                try
                {
                    File.Delete(x.FullName);
                }
                catch { }
            }
        }

        private void LoadEphemeris(string fname)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path = Path.Combine(path, "GeminiNET");

            Directory.CreateDirectory(path);
            path = Path.Combine(path, "ephemeris");
            Directory.CreateDirectory(path);
            string ephText = "";

            if (fname == null)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                var latest = di.EnumerateFiles("*.eph").Where(x => x.LastWriteTime > DateTime.Now - TimeSpan.FromHours(24)).OrderByDescending(x => x.LastWriteTime).FirstOrDefault();
                if (latest != null)
                    path = latest.FullName;
            }
            else path = fname;

            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            try
            {
                ephText = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Failed to read ephemeris";
                MessageBox.Show($"Failed to read ephemeris from {path}:\r\n\r\n{ex.Message}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (ephText != "")
            {
                try
                {
                    txtObject.Text = "";
                    HorizonsAPI.Ephemeris eph = null;
                    current_name = Path.GetFileNameWithoutExtension(path);

                    if (isScout && !ephText.Contains("API SOURCE: NASA/JPL Horizons API"))
                    {
                        eph = HorizonsAPI.Parse.ExtractScout(ephText);
                        AdjustScoutEphemeris(eph);
                    }
                    else
                        eph = HorizonsAPI.Parse.Extract(ephText);

                    currentObject = eph;
                    UpdateDescription(eph);

                    lblStatus.Text = $"Ephemeris loaded for: {current_name}";

                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Info(2, "MP:btnFind", $"Error processing ephemeris: {ex.Message}");
                    lblStatus.Text = $"Failed to load ephemeris: {current_name}";

                    MessageBox.Show($"Error processing ephemeris!\r\n\r\n{ex.Message}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                SetButtonState();
            }
        }


        private void UpdateDescription(HorizonsAPI.Ephemeris eph)
        {

            DateTime tStart = eph.StartVisible(DateTime.UtcNow);

            string visible;

            if (tStart == DateTime.MaxValue)
            {
                visible = "NOT visible for 24 hours";
            }
            else
            {
                DateTime tEnd = eph.EndVisible(tStart);

                TimeSpan ts = tEnd - tStart;

                if (tStart - DateTime.UtcNow < TimeSpan.FromSeconds(30))
                    visible = $"NOW for {ts:hh\\h\\ mm\\m}";
                else
                    visible = $"in {tStart - DateTime.UtcNow:hh\\h\\ mm\\m} for {ts:hh\\h\\ mm\\m}";
            }

            txtDesc.Text = $"Name:          {eph.name,-20}\r\nConstellation: {eph.data[0].constellation,-20}     Visibile: {visible}\r\n\r\n";

            DateTime when;
            double currentEl = 0;
            double mxe = eph.MaxElevation(DateTime.UtcNow, out when, out currentEl);
            TimeSpan maxVis = when - DateTime.UtcNow;

            string visIn;
            if (maxVis < TimeSpan.FromSeconds(60))
                visIn = "NOW";
            else
                visIn = $"in {maxVis:hh\\:mm} hours";


            txtDesc.Text += $"Max Elevation:    {mxe:0}° {visIn}   Current: {currentEl:0}°\r\n";
            //txtDesc.Text += $"Date Start:    {eph.start,-20}\r\nEnd:           {eph.stop,-20}\r\n";
            txtDesc.Text += $"\r\nMin dRA:  {eph.data.Min(x => x.dra),15:0.00000}   Max dRA: {eph.data.Max(x => x.dra),15:0.00000}\r\n";
            txtDesc.Text += $"Min dDEC: {eph.data.Min(x => x.ddec),15:0.00000}   Max dDEC:{eph.data.Max(x => x.ddec),15:0.00000}\r\n\r\n";
            txtDesc.Text += $"Max dRate:{eph.maxRateChange,15:0.00000} arcsec/sec^2\r\n\r\n";
            txtDesc.Text += $"Time: {DateTime.Now,15}   Time UTC: {DateTime.UtcNow,15}\r\n";
            GeminiHardware.Instance.Trace.Info(3, "MP:UpdateDescription", txtDesc.Text);

        }

        string tle = "";
        private void pbTLE_Click(object sender, EventArgs e)
        {
            frmTLE frm = new frmTLE();
            frm.tle = tle;
            var r = frm.ShowDialog();
            if (r == DialogResult.OK)
            {
                tle = frm.tle;
                ProcessTLE(sender, e);
            }

        }

        private void ProcessTLE(object sender, EventArgs e)
        {
            GeminiHardware.Instance.Trace.Info(3, "MP:ProcessTLE", tle);
            var l = tle.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (l.Length > 0)
            {
                if (!l[0].StartsWith("1 "))
                    txtObject.Text = $"[TLE] {l[0].Trim()}";
                else
                    txtObject.Text = $"[TLE] Unspecified Object";

                btnFind_Click(sender, e);
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        private void frmMinorPlanets_ChangeUICues(object sender, UICuesEventArgs e)
        {
            SetButtonState();
        }

        private void txtObject_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => btnFind_Click(sender, e)));
        }

        private void pbRaw_Click(object sender, EventArgs e)
        {
            frmMPRaw frm = new frmMPRaw();

            if (isScout)
            {
                string json = currentObject == null ? last_result : currentObject.raw;
                var obj = JsonConvert.DeserializeObject(json);
                frm.data = JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            else
                frm.data = currentObject == null? last_result : currentObject.raw;
            
            if (frm.data == null) return;
            frm.ShowDialog();

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tracking = false;
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            if (currentObject == null || !connected)
                return;
            double dRA, dDec, ra, dec;
            bool rateAvailable = currentObject.GetCurrent(currentTimeUTC, out ra, out dec, out dRA, out dDec);
            if (rateAvailable)
            {
                this.BeginInvoke(new Action(() => SlewToCoordinates(ra, dec)));
                
            }
        }

        object lockUpdate = new object();

        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            btnGoto.Enabled = false;
                try
                {
                    lblStatus.Text = $"Slewing to {currentObject.name}";
                    lblStatus.Update();


                    GeminiHardware.Instance.Trace.Enter("MP:SlewToCoordinates", RightAscension, Declination);

                    GeminiHardware.Instance.TargetRightAscension = RightAscension;
                    GeminiHardware.Instance.TargetDeclination = Declination;

                    GeminiHardware.Instance.Velocity = "S";
                    GeminiHardware.Instance.SlewEquatorial();

                    var t = new Task(() =>
                    {
                        GeminiHardware.Instance.WaitForSlewToEnd();
                        GeminiHardware.Instance.Trace.Exit("MP:SlewToCoordinates", RightAscension, Declination);
                        this.BeginInvoke(new Action(()=>lblStatus.Text = "Goto completed!"));
                    });

                  t.Start();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Goto failed: {ex.Message}:\r\n\r\n{ex.GetType().Name}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lblStatus.Text = "Goto failed";
                }
                SetButtonState();          
        }

        private void VariableMoveAxisDEC(double rate, bool initial = false)
        {
            if (!System.Threading.Monitor.TryEnter(lockUpdate, 1)) return;

            try
            {
                if (GeminiHardware.Instance.dVersion < 6)
                {
                    AdjustCometRateDEC(rate, initial);
                    return;
                }

                GeminiHardware.Instance.Trace.Enter("MP:MoveAxisDEC", rate);

                if (rate == 0) // stop motion
                {
                    GeminiHardware.Instance.DoCommandResult(">454:0", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                }
                else
                {
                    if (Math.Abs(rate) > maxSlewRate) rate = maxSlewRate * Math.Sign(rate); //don't exceed max rates

                    double spd = stepsPerDegree();
                    int divisor = (int)Math.Round(12e6 / (spd * rate));

                    if (!initial)
                    {
                        // if reversing direction, set divisor to 0, and correct on the next iteration
                        if (divisor < 0 && previousDECDivisor > 0 || divisor > 0 && previousDECDivisor < 0)
                            divisor = 0;

                        if (divisor == previousDECDivisor) return;   //already set
                    }

                    string[] cmds = new string[]
                    {
                    $">452:{divisor}",
                    ">454:1"
                    };

                    previousDECDivisor = divisor;

                    GeminiHardware.Instance.Trace.Info(4, "MP:MoveAxisDEC", spd, divisor);

                    GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                    //GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
                    //GeminiHardware.Instance.Velocity = "S"; //slew, as per ASCOM spec
                }
                GeminiHardware.Instance.Trace.Exit("MP:MoveAxisDEC", rate);
            }
            finally
            {
                System.Threading.Monitor.Exit(lockUpdate);
            }
        }

        private void AdjustCometRateDEC(double rate, bool initial)
        {
            GeminiHardware.Instance.Trace.Enter("MP:AdjustCometRateDEC", rate);

            if (rate == 0) // stop motion
            {
                GeminiHardware.Instance.DoCommandResult(">412:0", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
            }
            else
            {
                if (Math.Abs(rate) > maxSlewRate) rate = maxSlewRate * Math.Sign(rate); //don't exceed max rates

                if (GeminiHardware.Instance.m_SideOfPier == "E")
                    rate = -rate;

                double spd = stepsPerDegree();
                int divisor = (int)Math.Round(12e6 / (spd * rate));

                if (!initial)
                {
                    // if reversing direction, set divisor to 0, and correct on the next iteration
                    if (divisor < 0 && previousDECDivisor > 0 || divisor > 0 && previousDECDivisor < 0)
                        divisor = 0;

                    if (divisor == previousDECDivisor) return;   //already set
                }

                string[] cmds = new string[]
                {
                    $">412:{divisor}",                   
                };

                previousDECDivisor = divisor;

                GeminiHardware.Instance.Trace.Info(4, "MP:AdjustCometRateDEC", spd, divisor);

                GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                //GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
            }
            GeminiHardware.Instance.Trace.Exit("MP:AdjustCometRateDEC", rate);

        }

        private bool TrackingState = true;  //set when MoveAxis is initiated, restored when it ended
        private string RADivisors = "";     //set by MoveAxis before variable rate is set to be restored later
        private int previousRADivisor = 0;
        private int previousDECDivisor = 0;

        private void VariableMoveAxisRA(double rate, bool initial = false)
        {
            if (!System.Threading.Monitor.TryEnter(lockUpdate, 1)) return;

            try
            {
                if (GeminiHardware.Instance.dVersion < 6)
                {
                    AdjustCometRateRA(rate, initial);
                    return;
                }
                GeminiHardware.Instance.Trace.Enter("MP:MoveAxisRA", rate);
                if (rate == 0)
                {
                    // stop motion
                    GeminiHardware.Instance.DoCommandResult(">453:0", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                    if (RADivisors != "")
                        GeminiHardware.Instance.DoCommandResult(">451:" + RADivisors, GeminiHardware.Instance.MAX_TIMEOUT, false);

                    RADivisors = "";

                    if (!TrackingState)
                    {
                        GeminiHardware.Instance.DoCommandResult(":hN", GeminiHardware.Instance.MAX_TIMEOUT, false);
                        GeminiHardware.Instance.WaitForVelocity("N", GeminiHardware.Instance.MAX_TIMEOUT);
                        GeminiHardware.Instance.Tracking = false;
                    }
                }
                else
                {
                    TrackingState = GeminiHardware.Instance.Tracking;

                    if (Math.Abs(rate) > maxSlewRate) rate = maxSlewRate * Math.Sign(rate); //don't exceed max rates

                    //save
                    if (initial)
                        RADivisors = GeminiHardware.Instance.DoCommandResult("<451:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                    double spd = stepsPerDegree();
                    int divisor = (int)Math.Round(12e6 / (spd * rate));

                    if (!initial)
                    {
                        // if reversing direction, set divisor to 0, and correct on the next iteration
                        if (divisor < 0 && previousRADivisor > 0 || divisor > 0 && previousRADivisor < 0)
                            divisor = 0;

                        if (divisor == previousRADivisor) return;   //already set
                    }
                    string[] cmds = new string[]
                    {
                    $">451:{divisor}",
                    ">453:1"
                    };

                    previousRADivisor = divisor;
                    GeminiHardware.Instance.Trace.Info(4, "MP:MoveAxisRA", spd, divisor);

                    GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                    //GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
                    //GeminiHardware.Instance.Velocity = "S"; //slew, as per ASCOM spec
                }
                GeminiHardware.Instance.Trace.Exit("MP:MoveAxisRA", rate);
            }
            finally
            {
                System.Threading.Monitor.Exit(lockUpdate);
            }
        }

        private void AdjustCometRateRA(double rate, bool initial)
        {
           
            GeminiHardware.Instance.Trace.Enter("MP:AdjustCometRateRA", rate);
            if (rate == 0)
            {
                // stop motion
                GeminiHardware.Instance.DoCommandResult(">411:0", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
              
                RADivisors = "";

                if (!TrackingState)
                {
                    GeminiHardware.Instance.DoCommandResult(":hN", GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.WaitForVelocity("N", GeminiHardware.Instance.MAX_TIMEOUT);
                    GeminiHardware.Instance.Tracking = false;
                }
            }
            else
            {
                TrackingState = GeminiHardware.Instance.Tracking;

                if (Math.Abs(rate) > maxSlewRate) rate = maxSlewRate * Math.Sign(rate); //don't exceed max rates

                double spd = stepsPerDegree();
                int divisor = (int)Math.Round(12e6 / (spd * rate));

                if (!initial)
                {

                    // set comet tracking rate
                    GeminiHardware.Instance.DoCommandResult(">137:", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);

                    // if reversing direction, set divisor to 0, and correct on the next iteration
                    if (divisor < 0 && previousRADivisor > 0 || divisor > 0 && previousRADivisor < 0)
                        divisor = 0;

                    if (divisor == previousRADivisor) return;   //already set
                }
                string[] cmds = new string[]
                {
                    $">411:{divisor}",
                };

                previousRADivisor = divisor;
                GeminiHardware.Instance.Trace.Info(4, "MP:AdjustCometRateRA", spd, divisor);

                GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                //GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
            }
            GeminiHardware.Instance.Trace.Exit("MP:AdjustCometRateRA", rate);

        }

        double previous_stepsPerDegree = -1;

        private double stepsPerDegree()
        {
            if (previous_stepsPerDegree > 0) return previous_stepsPerDegree;

            string[] mountpar = null;
            double spd = 0;

            GeminiHardware.Instance.DoCommandResult(new string[] { "<21:", "<23:", "<25:" }, GeminiHardware.Instance.MAX_TIMEOUT / 2, false, out mountpar);

            if (mountpar != null)
            {
                string wormGearRatio = mountpar[0];
                string spurGearRatio = mountpar[1];
                string encoderResolution = mountpar[2];

                if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                    spd = (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution)) / 360.0;
            }
            if (spd == 0) throw new TimeoutException("Can't calculate stepsPerDegree");

            previous_stepsPerDegree = spd;

            GeminiHardware.Instance.Trace.Exit("MP:stepsPerDegree", spd.ToString());

            return spd;
        }


        private double SlewRate()
        {

            string[] get_rates = { "<170:" };
            string[] result = null;

            GeminiHardware.Instance.DoCommandResult(get_rates, 3000, false, out result);

            // if didn't get a result or one of the results timed out, throw an error:
            if (result == null) throw new TimeoutException("MP SlewRate");

            foreach (string s in result)
                if (s == null) throw new TimeoutException("MP SlewRate");

            double rate = 0;
            if (!GeminiHardware.Instance.m_Util.StringToDouble(result[0], out rate))
                throw new TimeoutException("MP SlewRate");
            rate = rate * SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0;  // convert to rate in deg/sec

            GeminiHardware.Instance.Trace.Exit("MP:SlewRate", rate.ToString());
            return rate;
        }



    internal void TimeChanged()
        {
            GeminiHardware.Instance.Trace.Enter("MP:TimeChanged");
            ComputeTimeDifference();
        }

        private void btnJPL_Click(object sender, EventArgs e)
        {
            string name = txtObject.Text;
            bool scout = false;
            if (name.Contains("[MB]"))
            {
                MessageBox.Show($"JPL/Horizons website may not have information for this major body object:\r\n\r\n{name}", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (name.ToLower().Contains("[sc]"))
            {
                scout = true;
            }

            if (currentObject != null)
                name = currentObject.name;

            
            int idx = name.IndexOf('(');
            if (idx > 0)
            {
                int idx2 = name.IndexOf(')', idx);
                name = name.Substring(idx + 1, idx2 - idx-1);
            }
            if (!scout)
                System.Diagnostics.Process.Start($"https://ssd.jpl.nasa.gov/tools/sbdb_lookup.html#/?sstr={HttpUtility.UrlEncode(name.Trim())}&view=VOSPA");
            else
            {
                name = name.Replace("[SC]", "").Replace("[sc]", "").Trim();
                System.Diagnostics.Process.Start($"https://cneos.jpl.nasa.gov/scout/#/object/{HttpUtility.UrlEncode(name)}");
            }

        }

        private void pbFile_Click(object sender, EventArgs e)
        {
            CleanUpEphemeris();

            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path = Path.Combine(path, "GeminiNET");

            Directory.CreateDirectory(path);
            path = Path.Combine(path, "ephemeris");
            Directory.CreateDirectory(path);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "GeminiNET Ephemeris (*.eph)|*.eph|All Files (*.*)|*.*";
            dlg.FilterIndex = 0;
            dlg.CheckFileExists = true;
            dlg.Title = "Import JPL/Horizons Ephemeris from a File";
            dlg.InitialDirectory = path;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadEphemeris(dlg.FileName);
            }

        }

        private void txtObject_DropDownClosed(object sender, EventArgs e)
        {
            if (current_name != "" && txtObject.Items.Count == 0)
            {
                txtObject.Items.Add(current_name);
            }
        }
    }

}
