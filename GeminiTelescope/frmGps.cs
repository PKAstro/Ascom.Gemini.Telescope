//tabs=4
// --------------------------------------------------------------------------------
//
// GPS query window
//
// Description:	
//
// Author:		(rbt) Robert Turner <robert@robertturnerastro.com>
//              (pk)  Paul Kanevsky <paul@pk.darkhorizons.org>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 15-JUL-2009	rbt	1.0.0	Initial implementation
// 29-MAR-2010  pk  1.0.3   Changed GPS Lat/Long/Elevation data to proper local
//                          decimal separator
// 16-MAY-2010  mc  1.0.7   Added EventHandlers for InvalidData and DataTimeout
// --------------------------------------------------------------------------------
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using ASCOM.GeminiTelescope.Properties;

namespace ASCOM.GeminiTelescope
{
    
    public delegate void FormDelegate(string latitude, string longitude, string elevation);
    public delegate void StatusDelegate(string status, Boolean blankFields, int icon);
    public delegate void TimeUpdateDelegate(DateTime tm);

    public partial class frmGps : Form
    {
        
        private NmeaInterpreter interpreter = new NmeaInterpreter();
        private string m_Latitude;
        private string m_Longitude;
        private string m_Elevation;
        
        public struct SystemTime

        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Millisecond;
        };

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        public extern static bool Win32SetSystemTime(ref SystemTime sysTime);

        public frmGps()
        {
            InitializeComponent();
            GeminiHardware.Instance.Trace.Enter("frmGPS");

            comboBoxComPort.Items.Add("");
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboBoxComPort.Items.Add(s);
            }
            buttonQuery.Text = Resources.Query;
            labelStatus.Text = Resources.Status + ":";
            labelStatusData.Text = "";
            labelLatitude.Text = Resources.Latitude + ":";
            labelLatitudeData.Text = "";
            labelLongitude.Text = Resources.Longitude + ":";
            labelLongitudeData.Text = "";
            labelElevation.Text = Resources.Elevation + ":";
            labelElevationData.Text = "";
            labelDateTime.Text = Resources.GPSDateTime + ":";
            labelDateTimeData.Text = "";
            GeminiHardware.Instance.Trace.Exit("frmGPS");
        }

        private void frmGps_Load(object sender, EventArgs e)
        {
            GeminiHardware.Instance.Trace.Enter("frmGPS_Load");
            interpreter.PositionReceived += new NmeaInterpreter.PositionReceivedEventHandler(interpreter_PositionReceived);
            interpreter.DateTimeChanged +=new NmeaInterpreter.DateTimeChangedEventHandler(interpreter_DateTimeChanged);
            interpreter.FixLost += new NmeaInterpreter.FixLostEventHandler(interpreter_FixLost);
            interpreter.FixObtained += new NmeaInterpreter.FixObtainedEventHandler(interpreter_FixObtained);
            interpreter.InvalidData += new NmeaInterpreter.InvalidDataEventHandler(interpreter_InvalidData);
            interpreter.DataTimeout += new NmeaInterpreter.DataTimeoutEventHandler(interpreter_DataTimeout);
            GeminiHardware.Instance.Trace.Exit("frmGPS_Load");
            SharedResources.SetInstance(this);
        }
        private void ProcessForm(string latitude, string longitude, string elevation)
        {
            GeminiHardware.Instance.Trace.Enter("ProcessForm", latitude, longitude, elevation);

            m_Latitude = latitude.Substring(1);
            m_Longitude = longitude.Substring(1);

            System.Globalization.CultureInfo oldCulture = GeminiHardware.Instance.m_Util.CurrentCulture;

            GeminiHardware.Instance.m_Util.CurrentCulture = GeminiHardware.Instance.m_GeminiCulture;    //"en-US" culture

            //// GPS data contains '.' as the decimal separator. To make ASCOM conversion functions work for the current locale,
            //// need to replace '.' with the correct local decimal separator [pk: 2010-03-29]
            string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            GeminiHardware.Instance.Trace.Info(4, "NumberDecimalSeparator = ", sep );

            if (sep != ".")
            {
                //m_Latitude = m_Latitude.Replace(".", sep);
                //m_Longitude = m_Longitude.Replace(".", sep);
                elevation = elevation.Replace(".", sep);
            }

            if (latitude.Substring(0, 1) == "S") m_Latitude = "-" + m_Latitude;
            if (longitude.Substring(0, 1) == "W") m_Longitude = "-" + m_Longitude;

            try
            {
 
                labelLatitudeData.Text =
                    GeminiHardware.Instance.m_Util.DegreesToDMS(GeminiHardware.Instance.m_Util.DMSToDegrees(m_Latitude));
                labelLongitudeData.Text =
                    GeminiHardware.Instance.m_Util.DegreesToDMS(GeminiHardware.Instance.m_Util.DMSToDegrees(m_Longitude));
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Except(ex);
            }

            GeminiHardware.Instance.m_Util.CurrentCulture = oldCulture; //restore original 

            if (elevation != SharedResources.INVALID_DOUBLE.ToString()) labelElevationData.Text = elevation;

            m_Elevation = elevation;

            GeminiHardware.Instance.Trace.Exit("ProcessForm", latitude, longitude, elevation);
        }
        private void interpreter_PositionReceived(string latitude, string longitude, string elevation)
        {
            GeminiHardware.Instance.Trace.Enter("frmGPS PositionReceived", latitude, longitude, elevation);

            FormDelegate message = new FormDelegate(ProcessForm);
            this.BeginInvoke(message, new Object[] { latitude, longitude, elevation });
            GeminiHardware.Instance.Trace.Exit("frmGPS PositionReceived");
        }

        private void ProcessStatus(string status, Boolean blankFields, int icon)
        {
            GeminiHardware.Instance.Trace.Enter("ProcessStatus", status, blankFields, icon);

            labelStatusData.Text = status;
            if (blankFields)
            {
                labelLatitudeData.Text = "";
                labelLongitudeData.Text = "";
                labelElevationData.Text = "";
                labelDateTimeData.Text = "";
            }
            if (icon == 1) //not connected
            {
                pictureBox1.Image = Resources.no_satellite;
            }
            else if (icon == 2)
            {
                pictureBox1.Image = Resources.no_fix_satellite;
            }
            else if (icon == 3)
            {
                pictureBox1.Image = Resources.satellite;
            }
            GeminiHardware.Instance.Trace.Exit("ProcessStatus");
        }


        private void setTime(System.DateTime dateTime)
        {
            GeminiHardware.Instance.Trace.Enter("setTime", dateTime.ToString());

            labelDateTimeData.Text = dateTime.ToString();
            if (checkBoxUpdateClock.Checked)
            {
                SystemTime updatedTime = new SystemTime();
                updatedTime.Year = (ushort)dateTime.ToUniversalTime().Year;
                updatedTime.Month = (ushort)dateTime.ToUniversalTime().Month;
                updatedTime.Day = (ushort)dateTime.ToUniversalTime().Day;
                updatedTime.Hour = (ushort)dateTime.ToUniversalTime().Hour;
                updatedTime.Minute = (ushort)dateTime.ToUniversalTime().Minute;
                updatedTime.Second = (ushort)dateTime.ToUniversalTime().Second;
                Win32SetSystemTime(ref updatedTime);
            }

            GeminiHardware.Instance.Trace.Exit("setTime" );
        }

        private void interpreter_DateTimeChanged(System.DateTime dateTime)
        {
            GeminiHardware.Instance.Trace.Enter("DateTimeChanged", dateTime);
            this.BeginInvoke(new TimeUpdateDelegate(setTime), dateTime);
        }

        public double Latitude
        {
            get
            {
                double lat = 0;
                try
                {
                    lat = GeminiHardware.Instance.m_Util.DMSToDegrees(m_Latitude);
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                }
                GeminiHardware.Instance.Trace.Enter("get_Latitude", lat);
                return lat;
            }
        }
        public double Longitude
        {
            get
            {
                double log = 0;
                try
                {
                    log = GeminiHardware.Instance.m_Util.DMSToDegrees(m_Longitude);
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                }

                GeminiHardware.Instance.Trace.Enter("get_Longitude", log);

                return log;
            }
        }
        public string Elevation { get { return m_Elevation; } }

        public string ComPort
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("get_ComPort", comboBoxComPort.SelectedItem.ToString());
                return comboBoxComPort.SelectedItem.ToString();
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("set_ComPort", value);
                try
                {
                    comboBoxComPort.SelectedItem = value;
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                }
//                GeminiHardware.Instance.Trace.Exit("set_ComPort", comboBoxComPort.SelectedItem.ToString());
            }
        }

        public string BaudRate
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("get_BaudRate", comboBoxBaudRate.SelectedItem.ToString());
                return comboBoxBaudRate.SelectedItem.ToString();
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("set_BaudRate",value);
                comboBoxBaudRate.SelectedItem = value;
                GeminiHardware.Instance.Trace.Exit("set_BaudRate", comboBoxBaudRate.SelectedItem.ToString());
            }
        }
        public bool UpdateClock
        {
            get { return checkBoxUpdateClock.Checked; }
            set { checkBoxUpdateClock.Checked = value; }
        }
        
        private void buttonQuery_Click(object sender, EventArgs e)
        {
            GeminiHardware.Instance.Trace.Enter("buttonQuery");


            if (buttonQuery.Text == Resources.Query)
            {
                if (comboBoxComPort.SelectedItem == null) { MessageBox.Show(Resources.SelectCOMPort); return; }
                try
                {

                    interpreter.ComPort = comboBoxComPort.SelectedItem.ToString();
                    interpreter.BaudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());
                    interpreter.Connected = true;
                    buttonQuery.Text = Resources.Stop;
                    labelStatusData.Text = Resources.WaitingForData;
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                    MessageBox.Show(ex.Message);
                }
                
            }
            else
            {
                try
                {
                    buttonQuery.Text = Resources.Query;
                    interpreter.Connected = false;
                    pictureBox1.Image = Resources.no_satellite;
                    labelStatusData.Text = "";
                }
                catch (Exception ex1)
                {
                    GeminiHardware.Instance.Trace.Except(ex1);
                }
            }
            GeminiHardware.Instance.Trace.Exit("buttonQuery");
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            GeminiHardware.Instance.Trace.Enter("cmdOK_Click");

            try
            {

                interpreter.Connected = false;

            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Except(ex);

            }
            GeminiHardware.Instance.Trace.Exit("cmdOK_Click");
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {

                interpreter.Connected = false;

            }
            catch { }
        }

        private void interpreter_FixLost()
        {
            GeminiHardware.Instance.Trace.Enter("FixLost");

            StatusDelegate message = new StatusDelegate(ProcessStatus);
            this.BeginInvoke(message, new Object[] { global::ASCOM.GeminiTelescope.Properties.Resources.GPSNoFix, true, 2 });
            GeminiHardware.Instance.Trace.Exit("FixLost");

        }
        private void interpreter_FixObtained()
        {
            GeminiHardware.Instance.Trace.Enter("FixObtained");

            StatusDelegate message = new StatusDelegate(ProcessStatus);
            this.BeginInvoke(message, new Object[] { global::ASCOM.GeminiTelescope.Properties.Resources.DataOK, false, 3 });
            GeminiHardware.Instance.Trace.Exit("FixObtained");
        }

        private void interpreter_InvalidData()
        {
            GeminiHardware.Instance.Trace.Enter("InvalidData");

            StatusDelegate message = new StatusDelegate(ProcessStatus);
            this.BeginInvoke(message, new Object[] { global::ASCOM.GeminiTelescope.Properties.Resources.InvalidDataReceived, true, 2 });
            GeminiHardware.Instance.Trace.Exit("InvalidData");
        }

        private void interpreter_DataTimeout()
        {
            GeminiHardware.Instance.Trace.Enter("DataTimeout");

            StatusDelegate message = new StatusDelegate(ProcessStatus);
            this.BeginInvoke(message, new Object[] { global::ASCOM.GeminiTelescope.Properties.Resources.WaitingForData, true, 1 });
            GeminiHardware.Instance.Trace.Exit("DataTimeout");
        }

        private void frmGps_FormClosing(object sender, FormClosingEventArgs e)
        {
            GeminiHardware.Instance.Trace.Enter("frmGps_Closing");

            interpreter.Connected = false;
            interpreter = null;
            GeminiHardware.Instance.Trace.Exit("frmGps_Closing");

        }

    }
}
