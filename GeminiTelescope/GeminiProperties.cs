﻿//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Gemini Properties
//
// Description:	This implements property source for Gemini
//
// Author:		(pk) Paul Kanevsky <paul@pk.darkhorizons.org>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 08-SEP-2009  pk  1.0.0   Added full complement of modeling, custom mount, and other Gemini property
//                          Implemented profile saving and editing
// --------------------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ASCOM.GeminiTelescope
{
    public class GeminiProperties : Component, INotifyPropertyChanged
    {

        private SerializableDictionary<string, object> mProfile = new SerializableDictionary<string, object>();

        
        public SerializableDictionary<string, object> Profile
        {
            get { return mProfile; }
            set { mProfile = value; }
        }

        public class ItemList
        {
            public ItemList(int _id, string _name)
            {
                id = _id;
                name = _name;
            }

            public override string ToString()
            {
                return name;
            }
            
            public int id;

            public string name;
        }

        static public string[] Mount_names = {"Custom", "GM-8", "G-11", "HGM-200", "MI-250", "Titan", "Titan50", "G11T", "GM811"};
        static public string[] Geometry_names = { "GEM", "Alt/Az" };
        static public string[] TrackingRate_names = { "Sidereal", "King Rate", "Lunar", "Solar", "Terrestrial", "Closed Loop", "Comet Rate" };
        static public string[] HandController_names = { "Visual", "Photo", "All Speeds" };
        static public string[] Brightness_names = { "100%", "53%", "40%", "27%", "20%", "13%", "6.6%"  };
        static public string[] Buzzer_states = { "On", "Off" };
        static public string[] Brightness_namesL5 = {"100%", "53%", "40%", "27%", "20%", "13%", "6.6%", "0%", "-6.6%", "-13%", "-20%"};
        /// <summary>
        /// true if these properties have not been sync'ed with Gemini yet
        /// false if no changes since the last sync (to Gemini) or since props were downloaded from Gemini:
        /// </summary>
        public bool IsDirty { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public GeminiProperties()
        {
        }


        /// <summary>
        /// Erase all entries in the current profile
        /// </summary>
        public void ClearProfile()
        {
            mProfile.Clear();
            RTProfile.Clear();
        }

        public void ClearProfile(List<string> props)
        {
            foreach (string s in props)
            {
                if (mProfile.ContainsKey(s))
                    mProfile.Remove(s);
                RTProfile.Clear();
            }
        }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
                catch { }
            }
        }
        
        private string get_Prop(string s)
        {
            if (!GeminiHardware.Instance.Connected) return null;

            string res = null;
            try
            {
                res = GeminiHardware.Instance.DoCommandResult(s, 2000, false);
                if (res == null) throw new TimeoutException();
            }
            catch
            {
                throw new TimeoutException();
                //return null;
            }
            return res;
        }


        private Dictionary<string, bool> WaitList = new Dictionary<string, bool>();
        private Dictionary<string, string> RTProfile = new Dictionary<string, string>();


        void OnHardwarePropertyUpdate(string sCmd, string sResult)
        {
            lock (RTProfile)
            {
                if (sResult != null)
                    RTProfile[sCmd] = sResult;
                WaitList[sCmd] = false;
            }
        }

        private string get_PropAsync(string sCmd)
        {
            string res = null;

            if (!GeminiHardware.Instance.Connected) return null;
            lock (RTProfile)
            {
                if (RTProfile.ContainsKey(sCmd))
                    res = RTProfile[sCmd] as string;

                if (!WaitList.ContainsKey(sCmd) || !WaitList[sCmd])
                {
                    if (RTProfile.ContainsKey(sCmd))
                    {
                        GeminiHardware.Instance.DoCommandAsync(sCmd, 5000, new HardwareAsyncDelegate(OnHardwarePropertyUpdate), false);
                        WaitList[sCmd] = true;
                    }
                    else
                    {
                        res = GeminiHardware.Instance.DoCommandResult(sCmd, GeminiHardware.Instance.MAX_TIMEOUT, false);    //first time, get the value non-async
                        RTProfile[sCmd] = res;
                    }
                }
            }
            return res;
        }



        private int get_int_Prop(string s)
        {
            string prop = get_Prop(s);
            int val;
            if (!int.TryParse(prop, out val)) return 0;
            return val;
        }

        private double get_double_Prop(string s)
        {
            string prop = get_Prop(s);
            double val;
            if (!double.TryParse(prop, System.Globalization.NumberStyles.Float, GeminiHardware.Instance.m_GeminiCulture, out val)) return 0;
            return val;
        }

        private bool get_bool_Prop(string s)
        {
            string prop = get_Prop(s);
            bool val;
            if (!bool.TryParse(prop, out val)) return false;
            return val;
        }

        private string get_string_Prop(string s)
        {
            string prop = get_Prop(s);
            return prop??"";
        }

        private DateTime get_time_Prop(string s)
        {
            string prop = get_Prop(s);
            DateTime val;
            if (!DateTime.TryParseExact(prop, "HH:mm:ss", new System.Globalization.DateTimeFormatInfo(), System.Globalization.DateTimeStyles.AssumeLocal, out val)) return DateTime.Now;
            return val;
        }

        private object get_Profile(string s, object def_value)
        {
            if (mProfile.ContainsKey(s)) return mProfile[s];
            return def_value;
        }

        // Real-time properites by-pass profile and go directly to Gemini hardware:
#region Real-Time Properties

        public string GeminiVersion
        {
            get
            {
                string res = get_PropAsync(":GVN");
                if (res == null) return null;

                return string.Format("L{0} v{1}.{2}", GeminiHardware.Instance.Version.Substring(0, 1), GeminiHardware.Instance.Version.Substring(1, 1), GeminiHardware.Instance.Version.Substring(2,1));
            }
        }

        public string MountType
        {
            get
            {
                string res = get_PropAsync("<0:");
                int idx = 0;
                if (!int.TryParse(res, out idx) || idx >= Mount_names.Length) return null;
                return Mount_names[idx];
            }
        }


        public string MountGeometry
        {
            get
            {
                if (GeminiHardware.Instance.dVersion >= 5.1)
                {
                    string res = get_PropAsync("<700:");
                    int idx = 0;
                    if (!int.TryParse(res, out idx)) return Geometry_names[0];
                    return Geometry_names[idx - 700];
                }
                else
                    return Geometry_names[0];
            }
        }


        public string LocalTime
        {
            get
            {
                string res = get_PropAsync(":GL");
                res = GeminiHardware.Instance.m_Util.HoursToHMS(GeminiHardware.Instance.m_Util.HMSToHours(res), ":", ":");
                return res;
            }
        }

        public string LocalDate
        {
            get
            {
                string res = get_PropAsync(":GC");
                if (res != null)
                {
                    DateTime tm = DateTime.ParseExact(res, "MM/dd/yy", new System.Globalization.DateTimeFormatInfo()); // Parse to a local datetime using the given format                    
                    return tm.ToShortDateString();
                }
                return null;
            }
        }


        public string TrackingRate
        {
            get
            {
                string res = get_PropAsync("<130:");
                int rate ;
                if (!int.TryParse(res, out rate)) return null;
                return TrackingRate_names[rate-131];
            }
        }

        public string TimeZone
        {
            get
            {
                if (!GeminiHardware.Instance.Connected) return null;
                // gemini stores timezone with sign reversed from UTC:
                return "UTC" + (GeminiHardware.Instance.UTCOffset > 0? " - " : " + ") + Math.Abs(GeminiHardware.Instance.UTCOffset).ToString();
            }
        }


        public string HandControllerMode
        {
            get
            {
                string prop = get_PropAsync("<160:");
                int hc;
                if (!int.TryParse(prop, out hc)) return null; 
                return HandController_names[hc - 161];
            }
        }

        public string GotoSlewRate
        {
            get
            {
                string prop = get_PropAsync("<140:");
                int rate;
                if (!int.TryParse(prop, out rate)) return null;
                return rate.ToString() + "x";
            }
        }

        public string ManualSlewRate
        {
            get
            {
                string prop = get_PropAsync("<120:");
                int rate;
                if (!int.TryParse(prop, out rate)) return null;
                return rate.ToString() + "x";
            }
        }

        public string GuideRate
        {
            get
            {
                string prop = get_PropAsync("<150:");
                double rate;
                if (!double.TryParse(prop, System.Globalization.NumberStyles.Float, GeminiHardware.Instance.m_GeminiCulture, out rate)) return null;
                return rate.ToString("0.0") + "x";
            }
        }


        public string CenteringRate
        {
            get
            {
                string prop = get_PropAsync("<170:");
                int rate;
                if (!int.TryParse(prop, out rate)) return null;
                return rate.ToString() + "x";
            }
        }

        public string PECStatus
        {
            get
            {
                string prop = get_PropAsync("<509:");
                int stat;
                if (!int.TryParse(prop, out stat)) return null;
                return (stat & 1) == 0 ? "PEC OFF" : "PEC ON";
            }
        }

        public string RA
        {
            get
            {
                if (!GeminiHardware.Instance.Connected) return null;
                double ra = GeminiHardware.Instance.RightAscension;
                return GeminiHardware.Instance.m_Util.HoursToHMS(ra, ":", ":", "");
            }
        }

        public string DEC
        {
            get
            {
                if (!GeminiHardware.Instance.Connected) return null;
                double dec = GeminiHardware.Instance.Declination;
                return GeminiHardware.Instance.m_Util.DegreesToDMS(dec, ":", ":", "");
            }
        }

        public string ALT
        {
            get
            {
                if (!GeminiHardware.Instance.Connected) return null;
                double alt = GeminiHardware.Instance.Altitude;
                return GeminiHardware.Instance.m_Util.DegreesToDMS(alt, ":", ":", "");
            }
        }

        public string AZ
        {
            get
            {
                if (!GeminiHardware.Instance.Connected) return null;
                double az = GeminiHardware.Instance.Azimuth;
                return GeminiHardware.Instance.m_Util.DegreesToDMS(az, ":", ":", "");
            }
        }

        public bool IsConnected
        {
            get { return GeminiHardware.Instance.Connected; }
        }

#endregion

#region Advanced Properties

        [Sequence(1)]
        public string MountTypeSetting
        {
            get { return (string)get_Profile("MountTypeSetting", Mount_names[2]); }
            set { mProfile["MountTypeSetting"] = value; IsDirty = true; }
        }

        private string MountTypeSetting_Gemini
        {
            get { return MountType ?? Mount_names[2]; }
            set
            {
                for (int i = 0; i < Mount_names.Length; ++i)
                    if (Mount_names[i].Equals((string)value))
                        GeminiHardware.Instance.DoCommandResult(">" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(2)]
        public string MountGeometrySetting
        {
            get { return (string)get_Profile("MountGeometrySetting", Geometry_names[0]); }
            set { mProfile["MountGeometrySetting"] = value; IsDirty = true; }
        }

        private string MountGeometrySetting_Gemini
        {
            get { return MountGeometry ?? Geometry_names[0]; }
            set
            {
                if (GeminiHardware.Instance.dVersion >= 5.1)
                {
                    for (int i = 0; i < Geometry_names.Length; ++i)
                        if (Geometry_names[i].Equals((string)value))
                            GeminiHardware.Instance.DoCommandResult(">" + (700 + i).ToString() + ":", GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }
        }



        [Sequence(99)]
        public string LEDBrightness
        {
            get { return (string)get_Profile("LEDBrightness", Brightness_names[0]); }
            set { mProfile["LEDBrightness"] = value; IsDirty = true;  }
        }

        private string LEDBrightness_Gemini
        {
            get {
                int res = get_int_Prop(":GB");
                string [] bn = Brightness_names;
                if (GeminiHardware.Instance.GeminiLevel > 4)
                    bn  = Brightness_namesL5;
                if (res <= bn.Length) return res.ToString();
                else
                    return bn[res];
            }
            set {
                string [] bn = Brightness_names;
                if (GeminiHardware.Instance.GeminiLevel > 4)
                    bn  = Brightness_namesL5;

                for (int i = 0; i < bn.Length; ++i)
                    if (bn[i].Equals(value))
                        GeminiHardware.Instance.DoCommandResult(":SB" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(99)]
        public string Buzzer
        {
            get { return (string)get_Profile("Buzzer", Buzzer_states[0]); }
            set { mProfile["Buzzer"] = value; IsDirty = true; }
        }

        private string Buzzer_Gemini
        {
            get
            {
                string[] bn = Buzzer_states;

                if (GeminiHardware.Instance.GeminiLevel < 6 || !GeminiHardware.Instance.Connected)
                    return bn[0];   //always On             
                int res = 0;

                try
                {
                    res = get_int_Prop("<60:");
                    if (res < 0 || res >= bn.Length)
                        return bn[0];   //always On            
                } catch { } 
                return bn[res];
            }
            set
            {
                string[] bn = Buzzer_states;
                if (GeminiHardware.Instance.GeminiLevel < 6)
                    return; 

                for (int i = 0; i < bn.Length; ++i)
                    if (bn[i].Equals(value))
                        GeminiHardware.Instance.DoCommandResult(">60:" + i.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }




        [Sequence(99)]
        public bool SyncDoesAlign
        {
            get { return (bool)get_Profile("SyncDoesAlign", false); }
            set { mProfile["SyncDoesAlign"] = value; IsDirty = true; }
        }


        private bool SyncDoesAlign_Gemini
        {
            get { return GeminiHardware.Instance.SwapSyncAdditionalAlign; }
            set { GeminiHardware.Instance.SwapSyncAdditionalAlign = value; }
        }

        // must be set before DoesRefraction!
        [Sequence(98)]
        public bool DoesPrecession
        {
            get { return (bool) get_Profile("DoesPrecession", false); }
            set { mProfile["DoesPrecession"] = value; IsDirty = true; }
        }

        private bool m_Precession;

        private bool DoesPrecession_Gemini
        {
            get { return GeminiHardware.Instance.Precession; }
            set { m_Precession  = value; }
        }


        [Sequence(99)]
        public bool DoesRefraction
        {
            get { return (bool) get_Profile("DoesRefraction", false); }
            set { mProfile["DoesRefraction"] = value; IsDirty = true; }
        }

        private bool DoesRefraction_Gemini
        {
            get { return GeminiHardware.Instance.Refraction; }
            set { GeminiHardware.Instance.SetPrecessionRefraction(m_Precession, value); }
        }

        [Sequence(99)]
        public DateTime AlarmTime
        {
            get { return (DateTime) get_Profile("AlarmTime", DateTime.Now); }
            set { mProfile["AlarmTime"] = value; IsDirty = true; }
        }


        private DateTime AlarmTime_Gemini
        {
            get { return get_time_Prop(":GE"); }
            set { GeminiHardware.Instance.DoCommandResult(":SE" + value.ToString("HH:mm:ss"), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(99)]
        public bool AlarmSet
        {
            get { return (bool) get_Profile("AlarmSet", false); }
            set { mProfile["AlarmSet"] = value; IsDirty = true; }
        }


        private bool AlarmSet_Gemini
        {
            get
            {
                int res = get_int_Prop("<180:");
                if (res == 182) return true;
                return false;
            }
            set
            {
                string prop  =GeminiHardware.Instance.DoCommandResult(value ? ">182:" : ">181:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(20)]
        public string WestSafetyLimit
        {
            get {
                double deg = (double)get_Profile("WestSafetyLimitDegrees", 0);
                return string.Format("Western Safety Limit: {0:0}°{1:00}", Math.Truncate(deg), Math.Truncate((deg - Math.Truncate(deg)) * 60));            
            }
            set { mProfile["WestSafetyLimit"] = value; IsDirty = true; }
        }
/*
        private string WestSafetyLimit_Gemini
        {
            get
            {
                string res = get_Prop("<220:");
                int d = 0, m = 0;
                try
                {
                    // east        west
                    //<ddd>d<mm>;<ddd>d<mm>
                    d = int.Parse(res.Substring(7, 3));
                    m = int.Parse(res.Substring(11, 2));
                }
                catch { }

                return string.Format("Western Safety Limit: {0:0}°{1:00}", d, m);
            }
        }
*/

        [Sequence(20)]
        public string EastSafetyLimit
        {
            get {
                double deg = (double)get_Profile("EastSafetyLimitDegrees", 0);
                return string.Format("Eastern Safety Limit: {0:0}°{1:00}", Math.Truncate(deg), Math.Truncate((deg - Math.Truncate(deg)) * 60));
            }
            set { mProfile["EastSafetyLimit"] = value; IsDirty = true; }
        }
/*
        private string EastSafetyLimit_Gemini
        {
            get
            {
                string res = get_Prop("<220:");
                int d = 0, m = 0;
                try
                {
                    // east        west
                    //<ddd>d<mm>;<ddd>d<mm>
                    d = int.Parse(res.Substring(0, 3));
                    m = int.Parse(res.Substring(4, 2));
                }
                catch { }
                return string.Format("Eastern Safety Limit: {0:0}°{1:00}", d, m);
            }
        }
 */

        [Sequence(20)]
        public double WestSafetyLimitDegrees
        {
            get { return (double)get_Profile("WestSafetyLimitDegrees", 0); }
            set { mProfile["WestSafetyLimitDegrees"] = value; IsDirty = true; }
        }

        private double WestSafetyLimitDegrees_Gemini
        {
            get
            {
                string res = get_Prop("<220:");
                int d = 0, m = 0;
                try
                {
                    // east        west
                    //<ddd>d<mm>;<ddd>d<mm>
                    d = int.Parse(res.Substring(7, 3));
                    m = int.Parse(res.Substring(11, 2));
                }
                catch { }

                return ((double)d) + ((double)m / 60.0);
            }
            set
            {
                if (value <= 0) return; // value not set -- don't update the mount!

                string cmd = ">222:" + string.Format("{0:000}d{1:00}", Math.Truncate(value), (value - Math.Truncate(value)) * 60.0);
                GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }


        [Sequence(20)]
        public double EastSafetyLimitDegrees
        {
            get { return (double)get_Profile("EastSafetyLimitDegrees", 0); }
            set { mProfile["EastSafetyLimitDegrees"] = value; IsDirty = true; }
        }

        private double EastSafetyLimitDegrees_Gemini
        {
            get
            {
                string res = get_Prop("<220:");
                int d = 0, m = 0;
                try
                {
                    // east        west
                    //<ddd>d<mm>;<ddd>d<mm>
                    d = int.Parse(res.Substring(0, 3));
                    m = int.Parse(res.Substring(4, 2));
                }
                catch { }
                return ((double)d) + ((double)m / 60.0);
            }
            set
            {
                if (value <= 0) return; // value not set -- don't update the mount!

                string cmd = ">221:" + string.Format("{0:000}d{1:00}", Math.Truncate(value), (value - Math.Truncate(value)) * 60.0);
                GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(2)]
        public double Latitude
        {
            get { return (double)get_Profile("Latitude", 0.0); }
            set { mProfile["Latitude"] = value; IsDirty = true; }
        }

        private double Latitude_Gemini
        {
            get
            {
                return GeminiHardware.Instance.Latitude;
            }
            set
            {
                GeminiHardware.Instance.SetLatitude(value);
            }
        }

        [Sequence(3)]
        public double Longitude
        {
            get { return (double)get_Profile("Longitude", 0.0); }
            set { mProfile["Longitude"] = value; IsDirty = true; }
        }

        private double Longitude_Gemini
        {
            get
            {
                return GeminiHardware.Instance.Longitude;
            }
            set
            {
                GeminiHardware.Instance.SetLongitude(value);
            }
        }

        [Sequence(4)]
        public int UTC_Offset
        {
            get { return (int)get_Profile("UTC_Offset", 0); }
            set { mProfile["UTC_Offset"] = value; IsDirty = true; }
        }

        private int UTC_Offset_Gemini
        {
            get
            {
                return GeminiHardware.Instance.UTCOffset;
            }
            set
            {
                GeminiHardware.Instance.UTCOffset = value;
            }
        }

        [Sequence(20)]
        public double GotoLimitDegrees
        {
            get { return (double)get_Profile("GotoLimitDegrees", 0.0); }
            set { mProfile["GotoLimitDegrees"] = value; IsDirty = true; }
        }

        private double GotoLimitDegrees_Gemini
        {
            get
            {
                string res = get_Prop("<223:");
                int d = 0, m = 0;
                try
                {
                    //<ddd>d<mm>
                    d = int.Parse(res.Substring(0, 3));
                    m = int.Parse(res.Substring(4, 2));
                }
                catch { }
                return ((double)d) + ((double)m / 60.0);
            }

            set
            {
                string cmd = ">223:" + string.Format("{0:000}d{1:00}", Math.Truncate(value), (value - Math.Truncate(value)) * 60.0);
                GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
            }

        }

        [Sequence(20)]
        public double FlipPointEast
        {
            get { return (double)get_Profile("FlipPointEast", 0.0); }
            set { mProfile["FlipPointEast"] = value; IsDirty = true; }
        }

        private double FlipPointEast_Gemini
        {
            get
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                {

                    string res = get_Prop("<227:");
                    int d = 0, m = 0;
                    try
                    {
                        //<ddd>d<mm>
                        d = int.Parse(res.Substring(0, 3));
                        m = int.Parse(res.Substring(4, 2));
                    }
                    catch { }
                    return ((double)d) + ((double)m / 60.0);
                }
                else return 0;
            }
            set
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                {

                    string cmd = ">227:" + string.Format("{0:000}d{1:00}", Math.Truncate(value), (value - Math.Truncate(value)) * 60.0);
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }
        }

        [Sequence(20)]
        public bool FlipPointsEnabled
        {
            get { return (bool)get_Profile("FlipPointEnabled", false); }
            set { mProfile["FlipPointEnabled"] = value; IsDirty = true; }
        }

        private bool FlipPointsEnabled_Gemini
        {
            get
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                {

                    int res = get_int_Prop("<229:");
                    return res!=0;
                }
                else return false;
            }
            set
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                {

                    string cmd = ">229:" + (value ? "3" : "0"); // turn on both or none
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }
        }

        [Sequence(20)]
        public double FlipPointWest
        {
            get { return (double)get_Profile("FlipPointWest", 0.0); }
            set { mProfile["FlipPointWest"] = value; IsDirty = true; }
        }

        private double FlipPointWest_Gemini
        {
            get
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                { 
                    string res = get_Prop("<228:");
                    int d = 0, m = 0;
                    try
                    {
                        //<ddd>d<mm>
                        d = int.Parse(res.Substring(0, 3));
                        m = int.Parse(res.Substring(4, 2));
                    }
                    catch { }
                    return ((double)d) + ((double)m / 60.0);
                }
                else
                    return 0;
            }

            set
            {
                if (GeminiHardware.Instance.GeminiLevel >= 6)
                {
                    string cmd = ">228:" + string.Format("{0:000}d{1:00}", Math.Truncate(value), (value - Math.Truncate(value)) * 60.0);
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }

        }






        [Sequence(20)]
        public bool PEC_Is_On
        {
            get { return (bool)get_Profile("PEC_Is_On", false);}
            set { mProfile["PEC_Is_On"] = value; IsDirty = true;} 

        }

        private bool PEC_Is_On_Gemini
        {
            get {
                string prop = get_Prop("<509:");
                int stat;
                if (!int.TryParse(prop, out stat)) return false;
                return (stat & 1) == 0 ? false : true;
            }

            set {              
                string prop = get_Prop("<509:");
                if (prop == "0" || prop == null) prop = get_Prop("<509:");

                int stat;
                if (!int.TryParse(prop, out stat)) return ;
                stat = (stat & 0xfe) | (value? 1 : 0);
                GeminiHardware.Instance.DoCommandResult(">509:" + stat.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);                
            }
        }




        [Sequence(20)]
        public string HandController
        {
            get {return (string)get_Profile("HandController", HandController_names[0]);}
            set { mProfile["HandController"] = value; IsDirty = true; }
        }

        private string HandController_Gemini
        {
            get { return HandControllerMode ?? HandController_names[0]; }
            set {
                for (int i = 0; i < HandController_names.Length; ++i)
                    if (HandController_names[i].Equals((string)value))
                        GeminiHardware.Instance.DoCommandResult(">" + (i + 161).ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public string TrackingRateMode
        {
            get { return (string)get_Profile("TrackingRateMode", TrackingRate_names[0]); }
            set { mProfile["TrackingRateMode"] = value; IsDirty = true; }
        }

        private string TrackingRateMode_Gemini
        {
            get { return this.TrackingRate?? TrackingRate_names[0]; }
            set {
                for (int i = 0; i < TrackingRate_names.Length; ++i)
                    if (TrackingRate_names[i].Equals(value))
                        GeminiHardware.Instance.DoCommandResult(">" + (i + 131).ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(10)]
        public int TrackingDivisorRA
        {
            get { return (int)get_Profile("TrackingDivisorRA", 56096); }
            set { mProfile["TrackingDivisorRA"] = value; IsDirty = true; }
        }

        private int TrackingDivisorRA_Gemini
        {
            get { return get_int_Prop("<411:"); }
            set { 
                if (TrackingRateMode=="Comet Rate")
                    GeminiHardware.Instance.DoCommandResult(">411:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);  
            }
        }

        [Sequence(10)]
        public int TrackingDivisorDEC
        {
            get { return (int)get_Profile("TrackingDivisorDEC", 0); }
            set { mProfile["TrackingDivisorDEC"] = value; IsDirty = true; }
        }

       
        private int TrackingDivisorDEC_Gemini
        {
            get { return get_int_Prop("<412:"); }
            set {
                if (TrackingRateMode == "Comet Rate")
                    GeminiHardware.Instance.DoCommandResult(">412:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(9)]
        public int ManualSlewSpeed
        {
            get { return (int)get_Profile("ManualSlewSpeed", 800); }
            set { mProfile["ManualSlewSpeed"] = value; IsDirty = true; }
        }

        private int ManualSlewSpeed_Gemini
        {
            get { return get_int_Prop("<120:");  }
            set { GeminiHardware.Instance.DoCommandResult(">120:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }


        [Sequence(9)]
        public int GotoSlewSpeed
        {
            get { return (int)get_Profile("GotoSlewSpeed", 800); }
            set { mProfile["GotoSlewSpeed"] = value; IsDirty = true; }
        }

        private int GotoSlewSpeed_Gemini
        {
            get { return get_int_Prop("<140:"); }
            set { GeminiHardware.Instance.DoCommandResult(">140:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(9)]
        public int CenteringSpeed
        {
            get { return (int)get_Profile("CenteringSpeed", 20); }
            set { mProfile["CenteringSpeed"] = value; IsDirty = true; }
        }

        private int CenteringSpeed_Gemini
        {
            get { return get_int_Prop("<170:"); }
            set { GeminiHardware.Instance.DoCommandResult(">170:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(9)]
        public double GuideSpeed
        {
            get { return (double)get_Profile("GuideSpeed", 0.5); }
            set { mProfile["GuideSpeed"] = value; IsDirty = true; }
        }

        private double GuideSpeed_Gemini
        {
            get { return get_double_Prop("<150:"); }
            set { GeminiHardware.Instance.DoCommandResult(">150:" + value.ToString("0.0",GeminiHardware.Instance.m_GeminiCulture), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        public int SlewSettleTime
        {
            get { return (int)get_Profile("SlewSettleTime", 0); }
            set { mProfile["SlewSettleTime"] = value; IsDirty = true; }
        }

        private int SlewSettleTime_Gemini
        {
            get { return GeminiHardware.Instance.SlewSettleTime; }
            set { GeminiHardware.Instance.SlewSettleTime = value; IsDirty = true; }
        }


        [Sequence(7)]
        public int WormGearRatioRA
        {
            get { return (int)get_Profile("WormGearRatioRA", 0); }
            set { mProfile["WormGearRatioRA"] = value; IsDirty = true; }
        }

        private int WormGearRatioRA_Gemini
        {
            get { return get_int_Prop("<21:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">21:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(7)]
        public int WormGearRatioDEC
        {
            get { return (int)get_Profile("WormGearRatioDEC", 0); }
            set { mProfile["WormGearRatioDEC"] = value; IsDirty = true; }
        }

        private int WormGearRatioDEC_Gemini
        {
            get { return get_int_Prop("<22:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">22:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int SpurGearRatioRA
        {
            get { return (int)get_Profile("SpurGearRatioRA", 0); }
            set { mProfile["SpurGearRatioRA"] = value; IsDirty = true; }
        }

        private int SpurGearRatioRA_Gemini
        {
            get { return get_int_Prop("<23:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">23:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int SpurGearRatioDEC
        {
            get { return (int)get_Profile("SpurGearRatioDEC", 0); }
            set { mProfile["SpurGearRatioDEC"] = value; IsDirty = true; }
        }

        private int SpurGearRatioDEC_Gemini
        {
            get { return get_int_Prop("<24:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">24:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int ServoEncoderResolutionRA
        {
            get { return (int)get_Profile("ServoEncoderResolutionRA", 0); }
            set { mProfile["ServoEncoderResolutionRA"] = value; IsDirty = true; }
        }

        private int ServoEncoderResolutionRA_Gemini
        {
            get { return get_int_Prop("<25:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">25:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int ServoEncoderResolutionDEC
        {
            get { return (int)get_Profile("ServoEncoderResolutionDEC", 0); }
            set { mProfile["ServoEncoderResolutionDEC"] = value; IsDirty = true; }
        }

        private int ServoEncoderResolutionDEC_Gemini
        {
            get { return get_int_Prop("<26:"); }
            set {
                if (MountTypeSetting == "Custom")
                    GeminiHardware.Instance.DoCommandResult(">26:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int StepsPerWormRevolutionRA
        {
            get { return (int)get_Profile("StepsPerWormRevolutionRA", 0); }
            set { mProfile["StepsPerWormRevolutionRA"] = value; IsDirty = true; }
        }

        private int StepsPerWormRevolutionRA_Gemini
        {
            get { return get_int_Prop("<27:"); }
            set
            {
                //if (MountTypeSetting == "Custom")
                //    GeminiHardware.Instance.DoCommandResult(">27:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(7)]
        public int StepsPerWormRevolutionDEC
        {
            get { return (int)get_Profile("StepsPerWormRevolutionDEC", 0); }
            set { mProfile["StepsPerWormRevolutionDEC"] = value; IsDirty = true; }
        }

        private int StepsPerWormRevolutionDEC_Gemini
        {
            get { return get_int_Prop("<28:"); }
            set {
                //if (MountTypeSetting == "Custom")
                //    GeminiHardware.Instance.DoCommandResult(">28:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }


        [Sequence(20)]
        public int ServoEncoderMultiplierRA
        {
            get { return (int)get_Profile("ServoEncoderMultiplierRA", 1); }
            set { mProfile["ServoEncoderMultiplierRA"] = value; IsDirty = true; }
        }

        private int ServoEncoderMultiplierRA_Gemini
        {
            get
            {
                if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
                    return (get_int_Prop("<401:") & 1) == 1 ? 4 : 1;
                return 1;
            }
            set
            {
                if (GeminiHardware.Instance.ServoFirmwareVersion(true) >= 2)
                {
                    int r = (get_int_Prop("<401:") & 2) | (value == 1 ? 0 : 1);
                    GeminiHardware.Instance.DoCommandResult(">401:" + r.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }
        }

        [Sequence(20)]
        public int ServoEncoderMultiplierDEC
        {
            get { return (int)get_Profile("ServoEncoderMultiplierDEC", 1); }
            set { mProfile["ServoEncoderMultiplierDEC"] = value; IsDirty = true; }
        }

        private int ServoEncoderMultiplierDEC_Gemini
        {
            get {
                if (GeminiHardware.Instance.ServoFirmwareVersion(false) >= 2)
                    return (get_int_Prop("<401:") & 2) == 2? 4 : 1;
                return 1;               
            }
            set
            {
                if (GeminiHardware.Instance.ServoFirmwareVersion(false) >= 2)
                {
                    int r = (get_int_Prop("<401:") & 1) | (value == 1 ? 0 : 2);
                    GeminiHardware.Instance.DoCommandResult(">401:" + r.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
                }
            }
        }


        [Sequence(7)]
        public bool UseEncoders
        {
            get { return (bool)get_Profile("UseEncoders", false); }
            set { mProfile["UseEncoders"] = value; IsDirty = true; }
        }


        private bool UseEncoders_Gemini
        {
            get
            {
                int res = get_int_Prop("<10:");
                if (res == 11) return true;
                return false;
            }
            set { GeminiHardware.Instance.DoCommandResult(value ? ">11:" : ">13:", GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }



        [Sequence(7)]
        public bool UseLimitSwitches
        {
            get { return (bool)get_Profile("UseLimitSwitches", false); }
            set { mProfile["UseLimitSwitches"] = value; IsDirty = true; }
        }


        private bool UseLimitSwitches_Gemini
        {
            get
            {
                int res = get_int_Prop("<10:");
                if (res == 14) return true;
                return false;
            }
            set { GeminiHardware.Instance.DoCommandResult(value ? ">14:" : ">15:", GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }


        [Sequence(7)]
        public int EncoderResolutionRA
        {
            get { return (int)get_Profile("EncoderResolutionRA", 0); }
            set { mProfile["EncoderResolutionRA"] = value; IsDirty = true; }
        }

        private int EncoderResolutionRA_Gemini
        {
            get { return get_int_Prop("<100:"); }
            set { GeminiHardware.Instance.DoCommandResult(">100:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(7)]
        public int EncoderResolutionDEC
        {
            get { return (int)get_Profile("EncoderResolutionDEC", 0); }
            set { mProfile["EncoderResolutionDEC"] = value; IsDirty = true; }
        }

        private int EncoderResolutionDEC_Gemini
        {
            get { return get_int_Prop("<110:"); }
            set { GeminiHardware.Instance.DoCommandResult(">110:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }


        [Sequence(10)]
        public int TVC
        {
            get { return (int)get_Profile("TVC", 0); }
            set { mProfile["TVC"] = value; IsDirty = true; }
        }

        private int TVC_Gemini
        {
            get { return get_int_Prop("<200:"); }
            set { GeminiHardware.Instance.DoCommandResult(">200:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(10)]
        public int PECOnAuto
        {
            get { return (int)get_Profile("PECOnAuto", 0); }
            set { mProfile["PECOnAuto"] = value; IsDirty = true; }
        }

        private int PECOnAuto_Gemini
        {
            get
            {
                if (GeminiHardware.Instance.dVersion >= 5.2)
                    return get_int_Prop("<508:");
                else
                    return 0;
            }
            set
            {
                if (GeminiHardware.Instance.dVersion >= 5.2)
                    GeminiHardware.Instance.DoCommandResult(">508:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
            }
        }

        [Sequence(13)]
        public int ModelA
        {
            get { return (int)get_Profile("ModelA", 0); }
            set { mProfile["ModelA"] = value; IsDirty = true; }
        }

        private int ModelA_Gemini
        {
            get { return get_int_Prop("<201:"); }
            set { GeminiHardware.Instance.DoCommandResult(">201:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); } 
        }

        [Sequence(13)]
        public int ModelE
        {
            get { return (int)get_Profile("ModelE", 0); }
            set { mProfile["ModelE"] = value; IsDirty = true; }
        }

        private int ModelE_Gemini
        {
            get { return get_int_Prop("<202:"); }
            set { GeminiHardware.Instance.DoCommandResult(">202:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelNP
        {
            get { return (int)get_Profile("ModelNP", 0); }
            set { mProfile["ModelNP"] = value; IsDirty = true; }
        }

        private int ModelNP_Gemini
        {
            get { return get_int_Prop("<203:"); }
            set { GeminiHardware.Instance.DoCommandResult(">203:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelNE
        {
            get { return (int)get_Profile("ModelNE", 0); }
            set { mProfile["ModelNE"] = value; IsDirty = true; }
        }

        private int ModelNE_Gemini
        {
            get { return get_int_Prop("<204:"); }
            set { GeminiHardware.Instance.DoCommandResult(">204:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelIH
        {
            get { return (int)get_Profile("ModelIH", 0); }
            set { mProfile["ModelIH"] = value; IsDirty = true; }
        }

        private int ModelIH_Gemini
        {
            get { return get_int_Prop("<205:"); }
            set { GeminiHardware.Instance.DoCommandResult(">205:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelID
        {
            get { return (int)get_Profile("ModelID", 0); }
            set { mProfile["ModelID"] = value; IsDirty = true; }
        }

        private int ModelID_Gemini
        {
            get { return get_int_Prop("<206:"); }
            set { GeminiHardware.Instance.DoCommandResult(">206:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelFR
        {
            get { return (int)get_Profile("ModelFR", 0); }
            set { mProfile["ModelFR"] = value; IsDirty = true; }
        }

        private int ModelFR_Gemini
        {
            get { return get_int_Prop("<207:"); }
            set { GeminiHardware.Instance.DoCommandResult(">207:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelFD
        {
            get { return (int)get_Profile("ModelFD", 0); }
            set { mProfile["ModelFD"] = value; IsDirty = true; }
        }

        private int ModelFD_Gemini
        {
            get { return get_int_Prop("<208:"); }
            set { GeminiHardware.Instance.DoCommandResult(">208:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelCF
        {
            get { return (int)get_Profile("ModelCF", 0); }
            set { mProfile["ModelCF"] = value; IsDirty = true; }
        }

        private int ModelCF_Gemini
        {
            get { return get_int_Prop("<209:"); }
            set { GeminiHardware.Instance.DoCommandResult(">209:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }

        [Sequence(13)]
        public int ModelTF
        {
            get { return (int)get_Profile("ModelTF", 0); }
            set { mProfile["ModelTF"] = value; IsDirty = true; }
        }

        private int ModelTF_Gemini
        {
            get { return get_int_Prop("<211:"); }
            set { GeminiHardware.Instance.DoCommandResult(">211:" + value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false); }
        }


        [Sequence(16)]
        public bool SavePEC
        {
            get { return (bool)get_Profile("SavePEC", false); }
            set
            {
                mProfile["SavePEC"] = value; IsDirty = true;  
                if (!value)
                    if (GeminiHardware.Instance.Connected && Profile.ContainsKey("PECTable"))
                        Profile.Remove("PECTable"); // delete it: the user doesn't want pec data
            }
        }

        private bool SavePEC_Gemini
        {
            get { return (bool)get_Profile("SavePEC", false); }
            set {
                if (value == SavePEC) return;   // was already set to true
                mProfile["SavePEC"] = value;
                if (value) PECTable = PECTable_Gemini;  // get the table from Gemini
                else
                    PECTable = new SerializableDictionary<int, string>();
            }

        }

        /// <summary>
        /// this doesn't participate in data binding, and so is marked private
        /// </summary>
        private SerializableDictionary<int, string> PECTable
        {
            get { 
                    
                object pec = get_Profile("PECTable", null);
                if (pec!=null)
                    return SerializableDictionary<int, string>.Parse((string)pec);
                return null;
            }
            set { mProfile["PECTable"] = value.ToString(); IsDirty = true; }
        }

        private SerializableDictionary<int, string> PECTable_Gemini
        {
            get
            {
                Cursor.Current = Cursors.WaitCursor;
                SerializableDictionary<int, string> pec = new SerializableDictionary<int, string>();
                int MaxPEC = get_int_Prop("<503:");
                double incr = 1;
                if (MaxPEC != 0) incr = 100.0 / MaxPEC;

                for (int i = 0; i < MaxPEC; )
                {

                    string val = get_string_Prop("<511:" + i.ToString());
                    string[] parts = val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    pec.Add(i, parts[0]+";"+i.ToString() + ";" + parts[1]);
                    i += int.Parse(parts[1]);
                    frmProgress.Update(int.Parse(parts[1])*incr, null);
                }
                Cursor.Current = Cursors.Default;
                return pec;
            }
            set
            {
                Cursor.Current = Cursors.WaitCursor;
                double incr = 1;
                if (value.Count != 0) incr = 100.0 / value.Count;


                List<string> batch = new List<string>();

                foreach (KeyValuePair<int, string> kp in value)
                {
                    if (batch.Count >= GeminiHardware.Instance.MaxCommands*2)
                    {
                        GeminiHardware.Instance.DoCommandResult(batch.ToArray(), GeminiHardware.Instance.MAX_TIMEOUT, false);
                        batch.Clear();
                    }
                    frmProgress.Update(incr, null);
                    batch.Add(">511:" + kp.Value.ToString());
//                    GeminiHardware.Instance.DoCommandResult(">511:" + kp.Value.ToString(), GeminiHardware.Instance.MAX_TIMEOUT, false);
                }

                if (batch.Count > 0)
                {
                    GeminiHardware.Instance.DoCommandResult(batch.ToArray(), GeminiHardware.Instance.MAX_TIMEOUT, false);
                    batch.Clear();
                }

                byte pec = GeminiHardware.Instance.PECStatus;
                if (pec != 0xff)
                {
                    GeminiHardware.Instance.PECStatus = (byte)(pec | (32 + 2));      // PEC data is available
                }
                Cursor.Current = Cursors.Default;
            }
        }

#endregion


        /// <summary>
        /// Synchronize current profile with Gemini
        /// </summary>
        /// <param name="write">if true, write current profile settings to Gemini,
        /// if false, read the current settings from Gemini
        /// </param>
        /// <returns>success or failure</returns>
        public bool SyncWithGemini(bool write)
        {
            GeminiHardware.Instance.Trace.Enter("GeminiProps:SyncWithGemini", write);

            if (!SyncWithGemini(write, null)) return false;

            if (SavePEC && write)       // reading of PEC occurs when trying to save the profile
            {
                frmProgress.Initialize(0, 100, write ? "Sending PEC data to Gemini" : "Getting PEC Data from Gemini", null);
                frmProgress.ShowProgress(null);

                // save PEC table if requested:
                if (write)
                {
                    if (PECTable!=null)
                        PECTable_Gemini = PECTable;
                }
                else
                {
                    PECTable = PECTable_Gemini;
                }
                frmProgress.HideProgress();
            }
            else if (GeminiHardware.Instance.Connected && !write && Profile.ContainsKey("PECTable"))
                Profile.Remove("PECTable");

            IsDirty = false;    // props are in sync: we downloaded them from Gemini, or uploaded them to Gemini

            GeminiHardware.Instance.Trace.Exit("GeminiProps:SyncWithGemini", write);
            return true;
        }

        private class SequenceCompare : IComparer
        {
            //compare sequence attributes of two properties
            // if attribute is missing its sequence is assumed to be 99
            int IComparer.Compare(Object x, Object y)
            {
                PropertyInfo px = x as PropertyInfo;
                PropertyInfo py = y as PropertyInfo;

                object[] ax = px.GetCustomAttributes(typeof(Sequence), false);
                object[] ay = py.GetCustomAttributes(typeof(Sequence), false);
                int ix = 99;
                int iy = 99;
                if (ax != null && ax.Length > 0) ix = ((Sequence)ax[0]).Seq;
                if (ay != null && ay.Length > 0) iy = ((Sequence)ay[0]).Seq;
                return (ix > iy)? 1 : ((ix < iy)? -1 : 0);
            }

        }


        /// <summary>
        /// Synchronize values from profile to/from Gemini, but only for properties
        /// contained in 'properties' list, or all properties if 'properties'==null
        /// </summary>
        /// <param name="write"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool SyncWithGemini(bool write, List<string> properties)
        {
            GeminiHardware.Instance.Trace.Enter("GeminiProps:SyncWithGemini", write, properties);
            
            if (!GeminiHardware.Instance.Connected)
            {
                GeminiHardware.Instance.Trace.Exit("GeminiProps:SyncWithGemini", false, "mount not connected");
                return false;
            }

            PropertyInfo[] ps = typeof(GeminiProperties).GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance);
            if (write && !ValidateProperties(ps)) return false;

            frmProgress.Initialize(0, 100, write ? "Sending Settings to Gemini" : "Getting Settings from Gemini", null);
            frmProgress.ShowProgress(null);

         
            //sort all properties in the order of their 'Sequence' attribute
            // missing sequence attribute means that sequence number is 99
            Array.Sort(ps, new SequenceCompare());
            
            double incr = 1;

            if (ps.Length != 0) incr = 100.0 / ps.Length;

            bool bSuccess = true;

            foreach (PropertyInfo p in ps)
            {
                string name = p.Name;

                frmProgress.Update(incr, null);

                // if properties list specified, and this one is not in it, skip it:
                if (properties != null && !properties.Contains(name)) continue;

                PropertyInfo pGemini = typeof(GeminiProperties).GetProperty(p.Name + "_Gemini", BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance);

                // for each public property in this class
                // find an equivalent propert with _Gemini appended to it,
                // and synchronize it with the public property.
                // public properties read/write to the Profile dictionary only
                // private properties that end with _Gemini read/write to Gemini directly
                try
                {
                    if (pGemini != null)
                        if (write && pGemini.GetSetMethod(true) != null && p.GetGetMethod() != null)
                            pGemini.SetValue(this, p.GetValue(this, null), null);
                        else if (!write && p.GetSetMethod() != null && pGemini.GetGetMethod(true) != null)
                            p.SetValue(this, pGemini.GetValue(this, null), null);
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                    bSuccess = false;
                    //break;
                }
            }

            frmProgress.HideProgress();
            GeminiHardware.Instance.Trace.Exit("GeminiProps:SyncWithGemini", write);
            return bSuccess;
        }

        static bool butc_asked = false;
        static bool bgeo_asked = false;

        private bool ValidateProperties(PropertyInfo[] ps)
        {
            List<string> warning = new List<string>();
            bool blat = false, blon = false, butc = false;

            int utc = 0;

            foreach (PropertyInfo p in ps)
            {
                string name = p.Name;

                PropertyInfo pGemini = typeof(GeminiProperties).GetProperty(p.Name + "_Gemini", BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance);

                try
                {

                    if (pGemini != null)
                        if (pGemini.GetSetMethod(true) != null && p.GetGetMethod() != null)
                            switch (p.Name)
                            {
                                case "Latitude":
                                    {
                                        double lat = (double)p.GetValue(this, null);

                                        if (Math.Round(lat, 5) == 34.08967)
                                            blat = true;
                                        break;
                                    }
                                case "Longitude":
                                    {
                                        double lon = (double)p.GetValue(this, null);

                                        if (Math.Round(lon, 5) == -118.343)
                                            blon = true;
                                        break;

                                    }
                                case "UTC_Offset":
                                    {
                                        utc = (int)p.GetValue(this, null);

                                        if ((int)(TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)).TotalHours != -utc)
                                            butc = true;
                                        break;
                                    }
                            }
                }
                catch (Exception ex)
                {
                    GeminiHardware.Instance.Trace.Except(ex);
                    return true;
                }
            }



            string err = "";

            if (blon && blat && !bgeo_asked)
            {
                err = "Profile settings that you are about to send to Gemini appear to contain some default values instead of actual settings. Do you still want to send this to Gemini?\r\n\r\n(For example, check geo locationand UTC offset)";
                bgeo_asked = true;
            }
            else if (butc && !butc_asked)
            {
                err = $"Settings that you are about to send to Gemini specify UTC Offset of ({-utc}) that doesn't match your PC ({(int)(TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)).TotalHours}). Do you still want to update Gemini?";
                butc_asked = true;
            }

            GeminiHardware.Instance.Trace.Error(err);
            if (err!="")
            {
                var r = MessageBox.Show(err, SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
                if (r != DialogResult.Yes)
                {
                    GeminiHardware.Instance.Trace.Error("Not updating");
                    return false;
                }
            }
            GeminiHardware.Instance.Trace.Error("Updating");
            return true;
        }




        /// <summary>
        /// Read/write the Profile dictionary to disk
        /// </summary>
        /// <param name="write">true to write, false to read</param>
        /// <param name="FileName">full path to the file to read/write</param>
        /// <returns></returns>
        public bool Serialize(bool write, string FileName)
        {
            // default profile is being read/written to if FileName is null:
            if (FileName == null)
            {
                string path = "";
                try
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\ASCOM\\" + SharedResources.TELESCOPE_DRIVER_NAME;
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {
                }

                FileName = path + "\\" + SharedResources.DEAULT_PROFILE;

                if (!write && !File.Exists(FileName)) return false;
            }

            GeminiHardware.Instance.Trace.Enter("GeminiProps:Serialize", write, FileName);

            // get PEC from Gemini if PEC is not in the current profile, but user wants it saved!
            if (write && SavePEC && !Profile.ContainsKey("PECTable"))
            {
                frmProgress.Initialize(0, 100, "Getting PEC data from Gemini", null);
                frmProgress.ShowProgress(null);
                PECTable = PECTable_Gemini;
                frmProgress.HideProgress();
            }
            else if (GeminiHardware.Instance.Connected && !SavePEC && Profile.ContainsKey("PECTable"))
                Profile.Remove("PECTable");

            try
            {
                if (write)
                {
                    TextWriter writer = new StreamWriter(FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
                    serializer.Serialize(writer, mProfile);
                    writer.Close();
                }
                else
                {
                    TextReader reader = new StreamReader(FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
                    mProfile = (SerializableDictionary<string, object>)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                GeminiHardware.Instance.Trace.Error("GeminiProperties.Serialize", write, FileName, ex.ToString(), ex.Message);
                return false;
            }

            GeminiHardware.Instance.Trace.Exit("GeminiProps:Serialize", write, FileName);

            return true;
        }

    }

    /// <summary>
    /// SerializableDictionary is a serializable generic dictionary class that .NET should've implemented
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable, ICloneable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        { 
            return null;
        }

        /// <summary>
        /// deep copy of the dictionary
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            MemoryStream ms = new MemoryStream();

            TextWriter writer = new StreamWriter(ms);
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<TKey, TValue>));
            serializer.Serialize(writer, this);

            ms.Seek(0, SeekOrigin.Begin);

            TextReader reader = new StreamReader(ms);
            object res = (object)serializer.Deserialize(reader);
            writer.Close();
            reader.Close();
            return res;
        }

        /// <summary>
        /// serialize into a string (xml)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            MemoryStream ms = new MemoryStream();

            TextWriter writer = new StreamWriter(ms);
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<TKey, TValue>));
            serializer.Serialize(writer, this);
            ms.Flush();
            long len = ms.Position;
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[len];
            ms.Read(buf, 0, (int)len);
            ms.Close();
            return ASCIIEncoding.ASCII.GetString(buf);

        }

        /// <summary>
        /// deserialize from string (xml)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public  static SerializableDictionary<TKey, TValue> Parse(string s)
        {
            MemoryStream ms = new MemoryStream();

            ms.Write(ASCIIEncoding.ASCII.GetBytes(s), 0, s.Length);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            TextReader reader = new StreamReader(ms);
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<TKey, TValue>));

            object res = (object)serializer.Deserialize(reader);
            reader.Close();
            return (SerializableDictionary<TKey, TValue>)res;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();

            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }



    public class Sequence : Attribute
    {
        private int m_seq;

        public Sequence(int seq)
        {
            m_seq = seq;
        }

        public int Seq
        {
            get { return m_seq; }
        }
    }
}
