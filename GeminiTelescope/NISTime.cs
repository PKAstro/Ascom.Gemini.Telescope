using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOM.GeminiTelescope
{

    public class NistClock
    {

        //http://tf.nist.gov/tf-cgi/servers.cgi
        string[] TimeServers = {

            "pool.ntp.org",
            "time-a.timefreq.bldrdoc.gov",
            "time-b.timefreq.bldrdoc.gov",
            "time-c.timefreq.bldrdoc.gov",
            "time-a.nist.gov",
            "time-b.nist.gov",
            "time.nist.gov",
            //"time-nw.nist.gov",
            //"nist1-ny.ustiming.org",
            //"nist1-dc.ustiming.org",
            //"nist1.columbiacountyga.gov",
            //"nist1-chi.ustiming.org"
            };

        public double LastError = 0;

        static DateTime dtLastUpdate = DateTime.MinValue;

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public int Attributes;
        }


        //Adjust token privilege
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int AdjustTokenPrivileges(IntPtr tokenhandle, int disableprivs,
        [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength,
        IntPtr PreivousState, IntPtr Returnlength);

        [DllImport("Advapi32.dll")]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);
        [DllImport("Advapi32.dll")]
        public static extern bool LookupPrivilegeValue(string SystemName, string Name, ref LUID luid);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        private static extern bool GetSystemTimeAdjustment(
            out int timeAdjustment, out int timeIncrement, out bool timeAdjustmentDisabled);

        [DllImport("kernel32.dll")]
        private static extern bool SetSystemTimeAdjustment(
            int timeAdjustment, bool timeAdjustmentDisabled);

        public NistClock()
        {
            //if (ConfigurationManager.AppSettings.Count > 0)
            //{
            //    TimeServers = new string[ConfigurationManager.AppSettings.Count];
            //    for (int i = 0; i < ConfigurationManager.AppSettings.Count; ++i)
            //        TimeServers[i] = ConfigurationManager.AppSettings[i];
            //}
        }

        public NistClock(IPAddress _timeServerIpAddress)
        {
            timeServerIpAddress = _timeServerIpAddress;
        }

        public bool PrintDiagnosticMessages
        {
            get { return printDiagnosticMessages; }
            set { printDiagnosticMessages = value; }
        }


        public static Socket ConnectToserver(IPEndPoint endPoint, int timeoutMs)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // do not block - do not want to be forced to wait on (too- long) timeout 
                socket.Blocking = false;

                // initiate connection - will cause exception because we're not  blocking 
                socket.Connect(endPoint);
                return socket;
            }
            catch (SocketException socketException)
            {
                // check if this exception is for the expected 'Would Block' error 
                if (socketException.ErrorCode != 10035)
                {
                    socket.Close();
                    // the error is not 'Would Block', so propogate the exception 
                    throw;
                }
                // if we get here, the error is 'Would Block' so just continue  execution   } 
                // wait until connected or we timeout 
                int timeoutMicroseconds = timeoutMs * 1000;
                if (socket.Poll(timeoutMicroseconds, SelectMode.SelectWrite) == false)
                {
                    // timed out 
                    socket.Close();
                    throw new Exception("The host failed to connect");
                }

                // *** AT THIS POINT socket.Connected SHOULD EQUAL TRUE BUT  IS FALSE!  ARGH! 
                // set socket back to blocking 
                socket.Blocking = true;
                return socket;
            }

        }


        public delegate void OnProgressDelegate(int pos, int ev);
        public event OnProgressDelegate OnProgress;

        private string QueryDaytimeString(out TimeSpan roundTripTime)
        {
            // Use RFC-867 protocol to query time from NIST time server...

            int count = 0;
            Stopwatch stopWatch = new Stopwatch();

            while (true)
            {
                ++count;

                IPEndPoint hostEndPoint = new IPEndPoint(timeServerIpAddress, 13);

                using (Socket socket = ConnectToserver(hostEndPoint, 1000))
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    int numberOfBytes = socket.Receive(buffer);
                    stopWatch.Stop();

                    if (numberOfBytes == 51)
                    {
                        roundTripTime = stopWatch.Elapsed;
                        string daytimeString = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, numberOfBytes).Trim();
                        return daytimeString;
                    }
                    else
                    {
                        if (printDiagnosticMessages)
                        {
                            Console.WriteLine("Attempt #{0}:  byte count received = {1}", count, numberOfBytes);
                        }
                    }
                }
                if (count > 2) throw new Exception("connection failed");
                System.Threading.Thread.Sleep(100);
            }
        }

        private static char[] SeparatorArray = new char[] { ' ' };

        // In USA, we use "." as decimal point (e.g. pi = 3.141592)
        // but in Europe, they use commas (pi = 3,141592).
        // The following is needed for parsing double with "." delimiter for users with non-US defaults:
        private static System.Globalization.CultureInfo EnglishUSACulture = new System.Globalization.CultureInfo("en-US");

        private static DateTime ParseDaytimeProtocol(string daytimeString)
        {
            // 54708 08-08-30 18:53:02 50 0 0 770.8 UTC(NIST) *
            //   0       1        2     3 4 5   6      7      8
            // http://tf.nist.gov/service/its.htm
            // See "Daytime Protocol (RFC-867)"

            string[] resultTokens = daytimeString.Split(SeparatorArray, StringSplitOptions.RemoveEmptyEntries);
            if (resultTokens[7] != "UTC(NIST)" || resultTokens[8] != "*")
            {
                throw new ApplicationException(string.Format("Invalid RFC-867 daytime protocol string: '{0}'", daytimeString));
            }

            if (resultTokens[5] != "0") throw new Exception("server is not accurate");

            int mjd = int.Parse(resultTokens[0]);  // "JJJJ is the Modified Julian Date (MJD). The MJD has a starting point of midnight on November 17, 1858."
            DateTime now = new DateTime(1858, 11, 17);
            now = now.AddDays(mjd);

            string[] timeTokens = resultTokens[2].Split(':');
            int hours = int.Parse(timeTokens[0]);
            int minutes = int.Parse(timeTokens[1]);
            int seconds = int.Parse(timeTokens[2]);
            double millis = double.Parse(resultTokens[6], EnglishUSACulture);     // this is speculative: official documentation seems out of date!

            now = now.AddHours(hours);
            now = now.AddMinutes(minutes);
            now = now.AddSeconds(seconds);
            now = now.AddMilliseconds(-millis);

            return now;
        }

        public DateTime GetCorrectedSystemLocalTime()
        {
            TimeSpan diff = MeasureSystemClockError();
            DateTime corrected = DateTime.Now + diff;
            return corrected;
        }



        public TimeSpan MeasureSystemClockError()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            int nbr = 0;

            TimeSpan ts = DateTime.Now - dtLastUpdate;

            // wait some if this was executed within last 10 seconds
            if (Math.Abs(ts.TotalMilliseconds) < 10000)
            {
                Thread.Sleep(ts.Milliseconds);
            }

            if (OnProgress != null)
            {
                OnProgress(TimeServers.Length, 0);
            }

            TimeSpan total = TimeSpan.Zero;
            List<TimeSpan> spans = new List<TimeSpan>();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < TimeServers.Length; ++i)
            {

                string name = TimeServers[i];

                if (OnProgress != null)
                {
                    OnProgress(i + 1, 1);
                }

                Task t = new Task( ()=>
                {
                    try
                    {

                        IPAddress[] ips = null;

                        ips = System.Net.Dns.GetHostAddresses(name);

                        if (ips == null) throw new Exception();

                        timeServerIpAddress = ips[0];
                        TimeSpan delay = PingIt(timeServerIpAddress);
                        TimeSpan err = _MeasureSystemClockError();
                        lock (spans)
                        {
                            spans.Add(err);

                            total += err;
                            nbr++;
                        }
                    }
                    catch { }
                });
                tasks.Add(t);
                t.Start();
            }

            Task.WaitAll(tasks.ToArray(), 4000);

            dtLastUpdate = DateTime.Now;

            if (nbr > 4)    // remove outliers from both ends, after sorting:
            {

                Array.Sort<TimeSpan>(spans.ToArray(), 0, nbr);
                List<TimeSpan> lst = new List<TimeSpan>(spans);

                total = TimeSpan.Zero;
                for (int i = 1; i < nbr - 1; ++i)
                    total += spans[i];

                double err = 0;
                double av = total.Milliseconds / (nbr - 2);

                for (int i = 1; i < nbr - 1; ++i)
                {
                    int ms = spans[i].Milliseconds;
                    err += (ms - av) * (ms - av);
                }
                nbr -= 2;
                LastError = Math.Sqrt(err / nbr);
            }
            else if (nbr == 0)
                throw new Exception("Cannot reach any of the NIST servers -- check internet access and proxy settings");


            if (OnProgress != null)
            {
                OnProgress(0, 2);
            }

            /*
                        if (nbr == 0)
                        {
                            TimeSpan tt = TimeSpan.Zero;
                            GetHTTPTimeError(); // make sure proxy, etc. is initialized, ignore first time
                            for(int i=0; i<5; ++i)
                                tt +=GetHTTPTimeError();
                            return TimeSpan.FromMilliseconds(tt.TotalMilliseconds / 5);
                        }
                        else
             */

            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            {
                TimeSpan diff = TimeSpan.FromMilliseconds(total.TotalMilliseconds / nbr);
                return diff;
            }


        }

        private TimeSpan GetHTTPTimeError()
        {
            string url = "http://tycho.usno.navy.mil/cgi-bin/timer.pl";

            WebRequest req = WebRequest.Create(url);

            Stopwatch stopWatch = new Stopwatch();

            WebResponse response = req.GetResponse();

            stopWatch.Stop();

            System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
            string str = reader.ReadLine();

            while (str != null)
            {
                if (str.Contains("UTC"))
                {
                    string header = response.Headers["Date"];
                    DateTime header_date = DateTime.ParseExact(header, "ddd, d MMM yyyy HH:mm:ss GMT", System.Globalization.CultureInfo.InvariantCulture);

                    string[] tokens = str.Split(new char[] { ' ', '.', ',', '<', '>', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens[4] == "UTC")
                    {
                        DateTime tm = DateTime.ParseExact(tokens[1] + " " + tokens[2] + " " + header_date.Year.ToString() + " " + tokens[3], "MMM dd yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        TimeSpan halfTrip = TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds / 2.0);
                        TimeSpan diff = tm + halfTrip - DateTime.Now.ToUniversalTime();
                        return diff;

                    }
                }
                Console.WriteLine(str);
                str = reader.ReadLine();
            }
            throw new Exception("no http access");
        }


        private TimeSpan _MeasureSystemClockError()
        {
            TimeSpan roundTrip;
            string daytimeString = QueryDaytimeString(out roundTrip);
            double halfTripMilliseconds = roundTrip.TotalMilliseconds / 2.0;
            TimeSpan halfTrip = new TimeSpan(0, 0, 0, 0, (int)halfTripMilliseconds);

            DateTime nist = NistClock.ParseDaytimeProtocol(daytimeString) + halfTrip;
            DateTime here = DateTime.Now.ToUniversalTime();
            TimeSpan diff = (nist - here);

            return diff;
        }

        // msec_per_min is the clock drift to correct, in msec per minute
        public static bool AdjustClockDrift(double msec_per_min)
        {

            const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
            const int SE_PRIVILEGE_ENABLED = 0x00000002;
            //            const int TOKEN_QUERY = 0x0008;
            //            const int TOKEN_ADJUST_PRIVILEGES = 0x0020;

            IntPtr tokenHandle = IntPtr.Zero;
            IntPtr pHandle = GetCurrentProcess();
            bool b = OpenProcessToken(pHandle, 32, ref tokenHandle);
            if (b)
                try
                {
                    LUID luid = new LUID();
                    bool ret;
                    string privilegeName = SE_SYSTEMTIME_NAME;
                    ret = LookupPrivilegeValue(null, privilegeName, ref luid);
                    if (ret)
                    {

                        TOKEN_PRIVILEGES tkNew = new TOKEN_PRIVILEGES();
                        tkNew.Privileges = new LUID_AND_ATTRIBUTES[1];
                        TOKEN_PRIVILEGES tkOld = new TOKEN_PRIVILEGES();
                        tkOld.Privileges = new LUID_AND_ATTRIBUTES[1];

                        tkNew.PrivilegeCount = 1;
                        tkNew.Privileges[0].Attributes =
                        SE_PRIVILEGE_ENABLED;
                        tkNew.Privileges[0].Luid = luid;
                        int luidSize = Marshal.SizeOf(typeof(TOKEN_PRIVILEGES));

                        if (AdjustTokenPrivileges(tokenHandle, 0, ref tkNew,
                        luidSize, IntPtr.Zero, IntPtr.Zero) != 0)
                        {
                            int timeAdjustment, timeIncrement;
                            bool timeAdjustmentDisabled;

                            if (GetSystemTimeAdjustment(out timeAdjustment, out timeIncrement, out timeAdjustmentDisabled))
                            {
                                double period = ((double)timeIncrement * 1e-7); // clock period in seconds
                                double drift = (msec_per_min) / 1000.0 / 60.0;  // drift in sec/sec
                                drift = drift * period * 1e7;
                                if (!timeAdjustmentDisabled) drift += timeAdjustment;
                                else drift += timeIncrement;

                                if (SetSystemTimeAdjustment((int)(drift + 0.5), false))
                                {
                                    if (GetSystemTimeAdjustment(out timeAdjustment, out timeIncrement, out timeAdjustmentDisabled))
                                    {
                                    }

                                }

                            }



                        }
                        else throw new Exception("Cannot adjust PC clock -- insufficient permissions");


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed: " + ex.Message, "NISTSync");
                }

            return false;
        }

        public TimeSpan SynchronizeLocalClock()
        {
            TimeSpan diff = MeasureSystemClockError();
            DateTime corrected = DateTime.Now + diff;
            NistClock.SetTimeLocal(corrected);
            return diff;
        }

        // Code snippet for setting system time found at:
        // http://www.codeguru.com/forum/archive/index.php/t-246724.html

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime([In] ref SYSTEMTIME st);   // http://msdn.microsoft.com/en-us/library/ms724942(VS.85).aspx

        public static void SetTimeLocal(DateTime local)
        {

            SetTimeUtc(local.ToUniversalTime());
        }

        public static void SetTimeUtc(DateTime utc)
        {
            SYSTEMTIME st = new SYSTEMTIME();  // http://msdn.microsoft.com/en-us/library/ms724950(VS.85).aspx
            st.wYear = (short)utc.Year;
            st.wMonth = (short)utc.Month;
            st.wDayOfWeek = 0;     // Microsoft documentation (see URL above) says: "The wDayOfWeek member of the SYSTEMTIME structure is ignored."
            st.wDay = (short)utc.Day;
            st.wHour = (short)utc.Hour;
            st.wMinute = (short)utc.Minute;
            st.wSecond = (short)utc.Second;
            st.wMilliseconds = (short)utc.Millisecond;

            if (!SetSystemTime(ref st))
            {
                throw new ApplicationException("Error setting time.");
            }
        }

        private IPAddress timeServerIpAddress;
        private byte[] buffer = new byte[256];
        private bool printDiagnosticMessages = false;


        public static TimeSpan PingIt(IPAddress ip)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions(128, true);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000;
            PingReply reply = pingSender.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                return TimeSpan.FromMilliseconds(reply.RoundtripTime);
            }
            return TimeSpan.Zero;
        }
    }
}
