//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Gemini driver for Telescope
//
// Description:	
//
// Implements:	ASCOM Telescope interface version: 2.0 (now 3.0)
// Author:		(rbt) Robert Turner <robert@robertturnerastro.com>
//              (pk)  Paul Kanevsky <paul@pk.darkhorizons.org>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 15-JUL-2009	rbt	1.0.0	Initial edit, from ASCOM Telescope Driver template
// 08-JUL-2009  pk  1.0.1   Full implementation of ITelescope interface, passing Conform test.
// 29-MAR-2010  pk  1.0.3   Moved CommandXXX methods to their proper location in the interface specification
//                          modified TrackingRates private 'pos' field to be non-static
// 10-SEP-2011  pk  1.0.23  implement ITelescope v3 interface
// --------------------------------------------------------------------------------
//
using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using ASCOM;
using ASCOM.Utilities;
using ASCOM.Interface;
using ASCOM.GeminiTelescope;

using System.IO;
using System.Windows.Forms;

/// <summary>
/// Need to add CommandNative to standard ITelescope interface for backward compatibility with
/// old Gemini ASCOM driver
/// </summary>
/// 
[Guid("EF0C67AD-A9D3-4F7B-A635-CD2095517633")]
[TypeLibType(4288)]
public interface IGeminiTelescope
{
    [DispId(101)]
    AlignmentModes AlignmentMode { get; }
    [DispId(102)]
    double Altitude { get; }
    [DispId(103)]
    double ApertureArea { get; }
    [DispId(104)]
    double ApertureDiameter { get; }
    [DispId(105)]
    bool AtHome { get; }
    [DispId(106)]
    bool AtPark { get; }
    [DispId(107)]
    double Azimuth { get; }
    [DispId(108)]
    bool CanFindHome { get; }
    [DispId(109)]
    bool CanPark { get; }
    [DispId(110)]
    bool CanPulseGuide { get; }
    [DispId(111)]
    bool CanSetDeclinationRate { get; }
    [DispId(112)]
    bool CanSetGuideRates { get; }
    [DispId(113)]
    bool CanSetPark { get; }
    [DispId(115)]
    bool CanSetPierSide { get; }
    [DispId(114)]
    bool CanSetRightAscensionRate { get; }
    [DispId(116)]
    bool CanSetTracking { get; }
    [DispId(117)]
    bool CanSlew { get; }
    [DispId(118)]
    bool CanSlewAltAz { get; }
    [DispId(119)]
    bool CanSlewAltAzAsync { get; }
    [DispId(120)]
    bool CanSlewAsync { get; }
    [DispId(121)]
    bool CanSync { get; }
    [DispId(122)]
    bool CanSyncAltAz { get; }
    [DispId(123)]
    bool CanUnpark { get; }
    [DispId(124)]
    bool Connected { get; set; }
    [DispId(125)]
    double Declination { get; }
    [DispId(126)]
    double DeclinationRate { get; set; }
    [DispId(127)]
    string Description { get; }
    [DispId(128)]
    bool DoesRefraction { get; set; }
    [DispId(129)]
    string DriverInfo { get; }
    [DispId(130)]
    string DriverVersion { get; }
    [DispId(131)]
    EquatorialCoordinateType EquatorialSystem { get; }
    [DispId(132)]
    double FocalLength { get; }
    [DispId(133)]
    double GuideRateDeclination { get; set; }
    [DispId(134)]
    double GuideRateRightAscension { get; set; }
    [DispId(135)]
    short InterfaceVersion { get; }
    [DispId(136)]
    bool IsPulseGuiding { get; }
    [DispId(137)]
    string Name { get; }
    [DispId(138)]
    double RightAscension { get; }
    [DispId(139)]
    double RightAscensionRate { get; set; }
    [DispId(140)]
    PierSide SideOfPier { get; set; }
    [DispId(141)]
    double SiderealTime { get; }
    [DispId(142)]
    double SiteElevation { get; set; }
    [DispId(143)]
    double SiteLatitude { get; set; }
    [DispId(144)]
    double SiteLongitude { get; set; }
    [DispId(145)]
    bool Slewing { get; }
    [DispId(146)]
    short SlewSettleTime { get; set; }
    [DispId(147)]
    double TargetDeclination { get; set; }
    [DispId(148)]
    double TargetRightAscension { get; set; }
    [DispId(149)]
    bool Tracking { get; set; }
    [DispId(150)]
    DriveRates TrackingRate { get; set; }
    [DispId(151)]
    ITrackingRates TrackingRates { get; }
    [DispId(152)]
    DateTime UTCDate { get; set; }

    [DispId(401)]
    void AbortSlew();
    [DispId(402)]
    IAxisRates AxisRates(TelescopeAxes Axis);
    [DispId(403)]
    bool CanMoveAxis(TelescopeAxes Axis);
    [DispId(404)]
    PierSide DestinationSideOfPier(double RightAscension, double Declination);
    [DispId(405)]
    void FindHome();
    [DispId(406)]
    void MoveAxis(TelescopeAxes Axis, double Rate);
    [DispId(407)]
    void Park();
    [DispId(408)]
    void PulseGuide(GuideDirections Direction, int Duration);
    [DispId(409)]
    void SetPark();
    [DispId(410)]
    void SetupDialog();
    [DispId(411)]
    void SlewToAltAz(double Azimuth, double Altitude);
    [DispId(412)]
    void SlewToAltAzAsync(double Azimuth, double Altitude);
    [DispId(413)]
    void SlewToCoordinates(double RightAscension, double Declination);
    [DispId(414)]
    void SlewToCoordinatesAsync(double RightAscension, double Declination);
    [DispId(415)]
    void SlewToTarget();
    [DispId(416)]
    void SlewToTargetAsync();
    [DispId(417)]
    void SyncToAltAz(double Azimuth, double Altitude);
    [DispId(418)]
    void SyncToCoordinates(double RightAscension, double Declination);
    [DispId(419)]
    void SyncToTarget();
    [DispId(420)]
    void Unpark();
    [DispId(421)]
    void CommandBlind(string Command, [DefaultParameterValue(false)] bool Raw);
    [DispId(422)]
    bool CommandBool(string Command, [DefaultParameterValue(false)]bool Raw);
    [DispId(423)]
    string CommandString(string Command, [DefaultParameterValue(false)]bool Raw);
    [DispId(424)]
    string CommandNative(string Command);
  

    //ITelescope v3 additions
    [DispId(429)]
    void Dispose();
    [DispId(430)]
    ArrayList SupportedActions { get; }
    [DispId(431)]
    string Action(string ActionName, string ActionParameters);



    [DispId(500)]
    PierSide PhysicalSideOfPier { get; set; }

}

namespace ASCOM.GeminiTelescope
{
    //
    // Your driver's ID is ASCOM.Telescope.Telescope
    //
    // The Guid attribute sets the CLSID for ASCOM.Telescope.Telescope
    // The ClassInterface/None addribute prevents an empty interface called
    // _Telescope from being created and used as the [default] interface
    //

    public partial class Telescope1 : ReferenceCountedObjectBase, IGeminiTelescope
    {
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        //private static string s_csDriverID = "ASCOM.Telescope.Telescope";
        // TODO Change the descriptive string for your driver then remove this line
        //private static string s_csDriverDescription = "Telescope Telescope";

        //
        // Driver private data (rate collections)
        //
        private AxisRates[] m_AxisRates = null;
        //private TrackingRates m_TrackingRates;

        private bool m_FoundHome = false;

        private bool m_AsyncSlewStarted = false;

        private bool m_Connected = false;


        // CACHED values for PulseGuide command:

        // last time pulse guide fetched guide speed and
        // calculated number of ticks per second:
        DateTime m_LastPulseGuideUpdate = DateTime.MinValue;
        double m_GuideRateStepsPerMilliSecond = 0;
        double m_GuideRateStepsPerMilliSecondEast = 0;
        double m_GuideRateStepsPerMilliSecondWest = 0;
        double m_TrackRate = 0;
        double m_GuideRA = 0;

        //
        // Constructor - Must be public for COM registration!
        //
        public Telescope1()
        {
            //m_TrackingRates = new TrackingRates();
        }


        ~Telescope1()
        {
            if (m_Connected)
            {
                m_Connected = false;
                GeminiHardware.Instance.Connected = false;
            }
        }


#region Private Code

        /// <summary>
        /// Processes a command string that comes from outside the driver and may have its leading 
        /// colon missing. It appends one if necessary which ensures correct processing elsewhere 
        /// in the driver
        /// </summary>
        /// <param name="cmd">The command string to be checked for leading colon</param>
        /// <returns>A command string with leading colon where required.</returns>
        internal string PrepareCommand(string cmd)
        {
            switch (cmd.Substring(0, 1))
            {
                case "<": //Do nothing
                    break;
                case ">": //Do nothing
                    break;
                case ":": //Do nothing
                    break;
                case "\x6": //special case
                    break;
                default: //Prepend :
                    cmd = ":" + cmd;
                    break;
            }
            return cmd;
        }

        private bool IsConnected
        {
            get
            {
                // if Gemini controller was disconnected, remove the reference count and mark this telescope object as disconnected:
                if (m_Connected && !GeminiHardware.Instance.Connected)
                {
                    GeminiHardware.Instance.Connected = false;
                    m_Connected = false;
                }
                return m_Connected && GeminiHardware.Instance.Connected;
            }
        }

        private void AssertConnect()
        {
            if (!IsConnected) throw new ASCOM.NotConnectedException();
        }

#endregion
        //
        // PUBLIC COM INTERFACE ITelescope IMPLEMENTATION
        //

#region ITelescope Members

        public void AbortSlew()
        {
            GeminiHardware.Instance.Trace.Enter("IT:AbortSlew");
            AssertConnect();
            if (GeminiHardware.Instance.AtHome || GeminiHardware.Instance.AtPark)
                throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);
            GeminiHardware.Instance.AbortSlewSync();
            GeminiHardware.Instance.Trace.Exit("IT:AbortSlew");
        }

        public AlignmentModes AlignmentMode
        {

            get
            {

                AlignmentModes res = AlignmentModes.algGermanPolar;
                if (GeminiHardware.Instance.AltAzMode)
                    res = AlignmentModes.algAltAz;
                GeminiHardware.Instance.Trace.Enter("IT:AlignmentMode.Get", res);

                return res;

            }
        }

        public double Altitude
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                GeminiHardware.Instance.Trace.Enter("IT:Altitude.Get", GeminiHardware.Instance.Altitude);
                return GeminiHardware.Instance.Altitude;
            }
        }

        public double ApertureArea
        {
            get
            {
                try
                {
                    double area = Math.PI * ((ApertureDiameter / 2.0) * (ApertureDiameter / 2.0));
                    double obstruction = double.Parse(GeminiHardware.Instance.OpticsObstruction.Split('~')[GeminiHardware.Instance.OpticsValueIndex])/1000; //in meters
                    area -= Math.PI * ((obstruction / 2.0) * (obstruction / 2.0));
                    GeminiHardware.Instance.Trace.Enter("IT:ApertureArea.Get", area);
                    return area; // in meters
                }
                catch { return 0; }
            }
        }

        public double ApertureDiameter
        {
            get
            {
                try
                {
                    GeminiHardware.Instance.Trace.Enter("IT:ApertureDiameter.Get", GeminiHardware.Instance.ApertureDiameter);
                    double aperturediameter;
                    double.TryParse(GeminiHardware.Instance.ApertureDiameter.Split('~')[GeminiHardware.Instance.OpticsValueIndex], out aperturediameter);
                    return aperturediameter / 1000; // in meters
                }
                catch { return 0; }
            }
        }

        public bool AtHome
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                GeminiHardware.Instance.Trace.Enter("IT:AtHome.Get", m_FoundHome);
                return m_FoundHome;
            }
        }

        public bool AtPark
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                GeminiHardware.Instance.Trace.Enter("IT:AtPark.Get", GeminiHardware.Instance.AtPark);
                return GeminiHardware.Instance.AtPark;
            }
        }

        public IAxisRates AxisRates(TelescopeAxes Axis)
        {
            AssertConnect();
            GeminiHardware.Instance.Trace.Enter("IT:AxisRates");

            if (m_AxisRates == null)
            {
                if (GeminiHardware.Instance.Connected)
                {
                    m_AxisRates = new AxisRates[3];
                    m_AxisRates[0] = new AxisRates(TelescopeAxes.axisPrimary);
                    m_AxisRates[1] = new AxisRates(TelescopeAxes.axisSecondary);
                    m_AxisRates[2] = new AxisRates(TelescopeAxes.axisTertiary);
                }
                else
                    return null;
            }

            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                    return m_AxisRates[0];
                case TelescopeAxes.axisSecondary:
                    return m_AxisRates[1];
                case TelescopeAxes.axisTertiary:
                    return m_AxisRates[2];
                default:
                    return null;
            }
        }

        public double Azimuth
        {
            get
            {
                AssertConnect();

                GeminiHardware.Instance.Trace.Enter("IT:Azimuth.Get", GeminiHardware.Instance.Azimuth);
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                return GeminiHardware.Instance.Azimuth;
            }
        }

        public bool CanFindHome
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanFindHome.Get", true);
                return true;
            }
        }

        public bool CanMoveAxis(TelescopeAxes Axis)
        {
            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                    GeminiHardware.Instance.Trace.Enter("IT:CanMoveAxis.Get", Axis, true);
                    return true;
                case TelescopeAxes.axisSecondary:
                    GeminiHardware.Instance.Trace.Enter("IT:CanMoveAxis.Get", Axis, true);
                    return true;
                case TelescopeAxes.axisTertiary:
                    GeminiHardware.Instance.Trace.Enter("IT:CanMoveAxis.Get", Axis, false);
                    return false;
                default:
                    GeminiHardware.Instance.Trace.Enter("IT:CanMoveAxis.Get", Axis, false);
                    return false;
            }
        }

        public bool CanPark
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanPark.Get", true);
                return true;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanPulseGuide.Get", true);
                return true;
            }
        }

        public bool CanSetDeclinationRate
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetDeclinationRate.Get", true);
                return true;
            }
        }

        public bool CanSetGuideRates
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetGuideRates.Get", true);
                return true;
            }
        }

        public bool CanSetPark
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetPark.Get", true);
                return true;
            }
        }

        public bool CanSetPierSide
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetPierSide.Get", true);
                return true;
            }
        }

        public bool CanSetRightAscensionRate
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetRightAscensionRate.Get", true);
                return true;
            }
        }

        public bool CanSetTracking
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSetTracking.Get", true);
                return true;
            }
        }

        public bool CanSlew
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSlew.Get", true);
                return true;
            }
        }

        public bool CanSlewAltAz
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSlewAltAz.Get", true);
                return true;
            }
        }

        public bool CanSlewAltAzAsync
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSlewAltAzAsync.Get", true);
                return true;
            }
        }

        public bool CanSlewAsync
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSlewAsync.Get", true);
                return true;
            }
        }

        public bool CanSync
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSync.Get", true);
                return true;
            }
        }

        public bool CanSyncAltAz
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanSyncAltAz.Get", false);
                return false;
            }
        }

        public bool CanUnpark
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:CanUnpark.Get", true);
                return true;
            }
        }

        public string CommandNative(string Command)
        {
            GeminiHardware.Instance.Trace.Enter("IT:CommandNative", Command);
            AssertConnect();

            if (Command == String.Empty) throw new ASCOM.InvalidValueException("CommandNative", Command, "valid Gemini command");
            string result = GeminiHardware.Instance.DoCommandResult(Command, 1000, false);

            if (result == null) return null;
            else
                if (result.EndsWith("#")) return result.Substring(result.Length - 1);

            GeminiHardware.Instance.Trace.Exit("IT:CommandNative", Command, result);
            return result;
        }

        public void CommandBlind(string Command, bool Raw)
        {
            GeminiHardware.Instance.Trace.Enter("IT:CommandBlind", Command, Raw);
            AssertConnect();

            if (Command == String.Empty) throw new ASCOM.InvalidValueException("CommandBlind", Command, "valid Gemini command");
            if (!Raw) Command = PrepareCommand(Command); // Add leading colon if required
            GeminiHardware.Instance.DoCommandResult(Command, GeminiHardware.Instance.MAX_TIMEOUT, Raw);
            GeminiHardware.Instance.Trace.Exit("IT:CommandBlind", Command);
        }

        public bool CommandBool(string Command, bool Raw)
        {
            GeminiHardware.Instance.Trace.Enter("IT:CommandBool", Command, Raw);
            AssertConnect();

            if (Command == "") throw new InvalidValueException("CommandBool", "", "valid Gemini command");
            if (!Raw) Command = PrepareCommand(Command); // Add leading colon if required
            string result = GeminiHardware.Instance.DoCommandResult(Command, GeminiHardware.Instance.MAX_TIMEOUT, Raw);
            if (result == null) throw new TimeoutException("CommandString: " + Command);

            bool bRes = (result != null && result.StartsWith("1"));
            GeminiHardware.Instance.Trace.Exit("IT:CommandBool", Command, bRes);
            return bRes;
        }

        public string CommandString(string Command, bool Raw)
        {
            GeminiHardware.Instance.Trace.Enter("IT:CommandString", Command, Raw);
            AssertConnect();

            if (Command == String.Empty) throw new ASCOM.InvalidValueException("CommandString", Command, "valid Gemini command");
            if (!Raw) Command = PrepareCommand(Command); // Add leading colon if required
            string result = GeminiHardware.Instance.DoCommandResult(Command, GeminiHardware.Instance.MAX_TIMEOUT, Raw);
            if (result == null) throw new TimeoutException("CommandString: " + Command);
            
            if (!Raw && result.EndsWith("#")) return result.Substring(1, result.Length - 1);//Added Start value substring parameter and handling of Raw values
            GeminiHardware.Instance.Trace.Exit("IT:CommandString", Command, result);
            return result;
        }

        public bool Connected
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:Connected.Get", IsConnected);
                return IsConnected;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:Connected.Set", value);
                GeminiHardware.Instance.Connected = value;
                if (value && !GeminiHardware.Instance.Connected) throw new ASCOM.Utilities.Exceptions.SerialPortInUseException("Connect");
                m_Connected = value;

                // reset some state variables so they are 
                // queried from the mount next time they are needed:
                m_LastPulseGuideUpdate = DateTime.MinValue;
                m_GuideRateStepsPerMilliSecond = 0;
                GeminiHardware.Instance.Trace.Exit("IT:Connected.Set", value);
            }
        }

        public double Declination
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                double res = GeminiHardware.Instance.Declination;  
                
                // adjust output to J2000 if that's the setting when below L6
                if (GeminiHardware.Instance.Precession && GeminiHardware.Instance.dVersion < 6)
                {
                    GeminiHardware.Instance.Trace.Info(2, "Converting DEC to J2000", res);
                    double ra = GeminiHardware.Instance.RightAscension;
                    AstronomyFunctions.ToJ2000(ref ra, ref res);
                }
                GeminiHardware.Instance.Trace.Enter("IT:Declination.Get", res);
                return res;
            }
        }



        /// <summary>
        /// The declination rate, default = 0.0
        /// this is for Gemini Level 5 or greater that uses a different meaning
        /// for the declination rate divisor than previous levels (12Mhz, no prescalers)
        /// </summary>
        public double L5DeclinationRate
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("L5DeclinationRate.Get");
                AssertConnect();
                string rateDivisor = GeminiHardware.Instance.DoCommandResult("<412:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<22:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<24:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<26:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (rateDivisor != null && spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                {

                    double frequency = 12000000;
                    double drateDivisor = double.Parse(rateDivisor);

                    if (drateDivisor == 0) return 0;    // no divisor, so no tracking

                    double stepsPerSecond = (frequency) / drateDivisor;
                    double arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                    double rate = arcSecondsPerStep * stepsPerSecond;

                    GeminiHardware.Instance.Trace.Exit("L5DeclinationRate.Get", rate);
                    return Math.Round(rate, 4);
                }
                else
                    throw new TimeoutException("L5DeclinationRate");
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("L5DeclinationRate.Set", value);
                AssertConnect();

                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<22:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<24:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<26:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                {

                    double frequency = 12000000;


                    double arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                    double stepsPerSecond = value / arcSecondsPerStep;

                    int rateDivisor = (int)((frequency) / stepsPerSecond + 0.5);

                  
                    if (value != 0)
                        GeminiHardware.Instance.DoCommand(">137:", false); // set Comet tracking rate so we can adjust DEC rate

                    string cmd = ">412:" + rateDivisor.ToString();
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.Trace.Exit("L5DeclinationRate.Set", value);
                }
                else throw new TimeoutException("DeclinationRate");
            }
        }




        /// <summary>
        /// Set comet-tracking declination rate. 
        /// arcseconds per second, default = 0.0
        /// </summary>
        public double DeclinationRate
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:DeclinationRate.Get");
                AssertConnect();

                if (GeminiHardware.Instance.GeminiLevel >= 5)
                    return L5DeclinationRate;

                string rateDivisor = GeminiHardware.Instance.DoCommandResult("<412:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<22:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<24:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<26:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (rateDivisor != null && spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                {

                    double rate = 0.0;

                    double rd = double.Parse(rateDivisor);
                    if (rd != 0)
                    {
                        double stepsPerSecond = 22.8881835938 / double.Parse(rateDivisor);
                        double arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                        rate = arcSecondsPerStep * stepsPerSecond;
                    }

                    GeminiHardware.Instance.Trace.Exit("IT:DeclinationRate.Get", rate);
                    return Math.Round(rate, 4);
                }
                else
                    throw new TimeoutException("DeclinationRate");
            }

            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:DeclinationRate.Set", value);

                AssertConnect();

                if (value == 0)
                {
                    GeminiHardware.Instance.DoCommandResult(">412:0", GeminiHardware.Instance.MAX_TIMEOUT, false);
                    return;
                }

                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<22:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<24:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<26:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                {
                    double arcSecondsPerStep = 1296000.00 / (double.Parse(wormGearRatio) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));
                    double stepsPerSecond = value / arcSecondsPerStep;

                    // L4
                    //1500000/22.881835938 = 65536
                    // 1500000/65536 = 22.881835938
                    // L5
                    // 12000000 / 2147483647

                    double step = GeminiHardware.Instance.GeminiLevel >= 5 ? 12000000.0 : 1500000.0;

                    int divisor = (int)(step / stepsPerSecond + 0.5);
                    if (GeminiHardware.Instance.GeminiLevel  < 5 && (divisor < -65535 || divisor > 65535))
                        throw new InvalidValueException("DeclinationRate", value.ToString(), "Rate is not implemented");

                    if (value != 0)
                        GeminiHardware.Instance.DoCommand(">137:", false); // set Comet tracking rate so we can adjust DEC rate

                    string cmd = ">412:" + divisor.ToString();
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.Trace.Exit("IT:DeclinationRate.Set", value);
                }
                else throw new TimeoutException("DeclinationRate");
            }
        }

        public string Description
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:Description.Get", SharedResources.TELESCOPE_DRIVER_DESCRIPTION);
                return SharedResources.TELESCOPE_DRIVER_DESCRIPTION;
            }
        }

#if false
        public PierSide DestinationSideOfPier(double RightAscension, double Declination)
        {
            throw new ASCOM.MethodNotImplementedException("DestinationSideOfPier"); // Was PropertyNotImplementedException
        }


#else

        // PK: southern hemisphere needs testing!
        public PierSide DestinationSideOfPier(double RightAscension, double Declination)
        {
            if (!GeminiHardware.Instance.ReportPierSide)
                throw new ASCOM.MethodNotImplementedException("DestinationSideOfPier"); // Was PropertyNotImplementedException

            string res = "RA: " + RightAscension.ToString() + " Dec: " + Declination.ToString();
            GeminiHardware.Instance.Trace.Enter("IT:DestinationSideOfPier", res);

            AssertConnect();

            // Get the Western goto limit
            res = GeminiHardware.Instance.DoCommandResult("<223:", GeminiHardware.Instance.MAX_TIMEOUT, false);

            int d = 0, m = 0;
            try
            {
                //<ddd>d<mm>
                d = int.Parse(res.Substring(0, 3));
                m = int.Parse(res.Substring(4, 2));
            }
            catch
            {
                GeminiHardware.Instance.Trace.Exit("IT:DestinationSideOfPier", PierSide.pierUnknown, "Error: Unable to get West Goto Limit");
                return PierSide.pierUnknown;
            }
            double gotolimit = ((double)d) + ((double)m / 60.0);

            int de, me = 0;

            // Get current safety limit
            res = GeminiHardware.Instance.DoCommandResult("<220:", GeminiHardware.Instance.MAX_TIMEOUT, false);
            try
            {
                // east        west
                //<ddd>d<mm>;<ddd>d<mm>
                d = int.Parse(res.Substring(7, 3));
                m = int.Parse(res.Substring(11, 2));
                de = int.Parse(res.Substring(0, 3));
                me = int.Parse(res.Substring(4, 2));

            }
            catch
            {
                GeminiHardware.Instance.Trace.Exit("IT:DestinationSideOfPier", PierSide.pierUnknown, "Error: Unable to get Safety Limits");
                return PierSide.pierUnknown;
            }
#if false
            double west_limit = ((double)d) + ((double)m / 60.0);
            west_limit -= 90;

            // if goto limit is set to zero, this means it's 2.5 degrees from west safety limit:
            if (gotolimit == 0)
            {
                gotolimit = ((double)d) + ((double)m / 60.0) - 2.5;
            }
            //gotolimit is now number of degrees from cwd position
            // PWGS No longer the case, it is referenced from the West limit. Was: gotolimit -= 90;    // degrees from meridian
            double east_limit = 90 - (de + (double)me / 60.0);

            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "GoTo Limit, West Limit, East Limit Degrees:", gotolimit, west_limit, east_limit);
            east_limit = east_limit / 360 * 24 - 12;
            west_limit = west_limit / 360 * 24;
            gotolimit = gotolimit / 360 * 24;

            double hour_angle = (GeminiHardware.Instance.SiderealTime) - RightAscension;

            if (GeminiHardware.Instance.SideOfPier == "E") hour_angle -= 12;

            // normalize to -12..12 hours:
            if (hour_angle < -12) hour_angle = 24 + hour_angle;
            if (hour_angle > 12) hour_angle = hour_angle - 24;
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "GoTo Limit, West Limit, East Limit, Hour Angle Hours:", gotolimit, west_limit, east_limit, hour_angle);
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "SideOfPier:", GeminiHardware.Instance.SideOfPier);

            // Default is 'we don't know!'
            PierSide retVal = PierSide.pierUnknown;
            bool flip = false;

            if ((hour_angle >= gotolimit && hour_angle - 12 >= east_limit && GeminiHardware.Instance.SideOfPier == "W"))
                flip = true;
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "Flip W:", flip);

            if ((hour_angle < east_limit && hour_angle + 12 <= west_limit && GeminiHardware.Instance.SideOfPier == "E"))
                flip = true;
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "Flip E:", flip);

            retVal = SideOfPier;    //current side of pier

            // flip it, if flip is required:
            if (flip)
                if (retVal == PierSide.pierEast) retVal = PierSide.pierWest;
                else
                    if (retVal == PierSide.pierWest) retVal = PierSide.pierEast;
#endif
            double west_limit = (double)d + ((double)m / 60.0);
            double east_limit = ((double)de + (double)me / 60.0);

            // if goto limit is set to zero, this means it's 2.5 degrees from west safety limit:
            if (gotolimit == 0) gotolimit = 2.5;

            // Convert goto limit from an offset from the west limit into an offset from the CWD position
            gotolimit = west_limit - gotolimit;

            // Convert the limits into degree offsets from the meridian
            east_limit = 90.0 - east_limit;
            west_limit = west_limit - 90.0;
            gotolimit = gotolimit - 90.0;
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "GoTo Limit, West Limit, East Limit Meridian offset (degrees):", gotolimit, west_limit, east_limit);

            // Convert the limits into hour angle offsets from the meridian
            east_limit = east_limit * 24.0 / 360.0;
            west_limit = west_limit * 24.0 / 360.0;
            gotolimit = gotolimit * 24.0 / 360.0;
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "GoTo Limit, West Limit, East Limit Meridian offset (hours):", gotolimit, west_limit, east_limit);

            // Get sidereal time, scope and destination hour angles
            double sidereal_time = GeminiHardware.Instance.SiderealTime;
            double scope_hour_angle = ConditionHA(sidereal_time - GeminiHardware.Instance.RightAscension);
            double destination_hour_angle = ConditionHA(sidereal_time - RightAscension);
            GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "SiderealTime, ScopeHA, DesinationHA (hours):", sidereal_time, scope_hour_angle, destination_hour_angle);

            PierSide retVal = PierSide.pierUnknown;
            // Now determine which side we will be on start with where we are. Region A is PierSide.PierWest, Region C is PierSidef.pierEast
            if (scope_hour_angle < east_limit) // Start Region A
            {
                if (destination_hour_angle < east_limit) // Destination Region A
                {
                    retVal = PierSide.pierWest; // No flip, always pierWest
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region A to Region A, Retval: ", retVal.ToString());
                }
                else if (destination_hour_angle > gotolimit) // Destination Region C
                {
                    if (SideOfPier == PierSide.pierEast) retVal = PierSide.pierWest; // Will flip
                    else retVal = PierSide.pierEast;
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region A to Region C, Retval: ", retVal.ToString());
                }
                else // Destination Region B
                {
                    retVal = SideOfPier; // No flip
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region A to Region B, Retval: ", retVal.ToString());
                }
            }
            else if (scope_hour_angle > gotolimit) // Start Region C
            {
                if (destination_hour_angle < east_limit) // Destination Region A
                {
                    if (SideOfPier == PierSide.pierEast) retVal = PierSide.pierWest; // Will flip
                    else retVal = PierSide.pierEast;
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region C to Region A, Retval: ", retVal.ToString());
                }
                else if (destination_hour_angle > gotolimit) // Destination Region C
                {
                    retVal = PierSide.pierEast; // No flip, always pierEast
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region C to Region C, Retval: ", retVal.ToString());
                }
                else // Destination Region B
                {
                    retVal = SideOfPier; // No flip
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region C to Region B, Retval: ", retVal.ToString());
                }
            }
            else // Start Region B
            {
                if (destination_hour_angle < east_limit) // Destination Region A
                {
                    // May flip
                    if (SideOfPier == PierSide.pierWest) retVal = PierSide.pierWest; // No flip as already in correct state
                    else retVal = PierSide.pierEast; // Will flip because not in correct state for destination
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region B to Region A, Retval: ", retVal.ToString(), SideOfPier);
                }
                else if (destination_hour_angle > gotolimit) // Destination Region C
                {
                    // May flip
                    if (SideOfPier == PierSide.pierEast) retVal = PierSide.pierEast; // No flip as already in correct state
                    else retVal = PierSide.pierWest; // Will flip because not in correct state for destination
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region B to Region C, Retval: ", retVal.ToString(), SideOfPier);
                }
                else // Destination Region B
                {
                    retVal = SideOfPier; // No flip
                    GeminiHardware.Instance.Trace.Info(1, "IT:DestinationSideOfPier", "From Region B to Region B, Retval: ", retVal.ToString());
                }
            }



#if false
            // Swap sides for Southern Hemisphere
            if (GeminiHardware.Instance.SouthernHemisphere)
            {
                GeminiHardware.Instance.Trace.Info(4, "IT:DestinationSideOfPier", "Southern Hemisphere");
                if (retVal == PierSide.pierEast) retVal = PierSide.pierWest;
                else if (retVal == PierSide.pierWest) retVal = PierSide.pierEast;
            }
#endif
            GeminiHardware.Instance.Trace.Exit("IT:DestinationSideOfPier", retVal);
            return retVal;
        }

#endif

        public bool DoesRefraction
        {
            get
            {
                AssertConnect();
                bool bRef = GeminiHardware.Instance.Refraction;
                GeminiHardware.Instance.Trace.Enter("IT:DoesRefraction.Get", bRef);
                return bRef;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:DoesRefraction.Set", value);
                AssertConnect();
                GeminiHardware.Instance.Refraction = value;
            }
        }

        public string DriverInfo
        {
            get
            {

                Version GeminiVersion = Assembly.GetEntryAssembly().GetName().Version;

                FileInfo oMyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                DateTime oBuildDate = oMyFile.LastWriteTime;
                string res = SharedResources.TELESCOPE_DRIVER_INFO + " Version " + GeminiVersion.ToString() + " dated " + oBuildDate.ToLongDateString() + " " + oBuildDate.ToLongTimeString();

                GeminiHardware.Instance.Trace.Enter("IT:DriverInfo.Get", res);
                return res;

            }
        }

        public string DriverVersion
        {
            get
            {
                Version GeminiVersion = Assembly.GetEntryAssembly().GetName().Version;
                string res = GeminiVersion.ToString(2); //Return just the major and minor version numbers
                GeminiHardware.Instance.Trace.Enter("IT:DriverVersion.Get", res);
                return res;
            }
        }

        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {

                AssertConnect();
                EquatorialCoordinateType res = GeminiHardware.Instance.Precession ? EquatorialCoordinateType.equJ2000 : EquatorialCoordinateType.equLocalTopocentric;
                GeminiHardware.Instance.Trace.Enter("IT:EquatorialSystem.Get", res);
                return res;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:EquatorialSystem.Set", value);
                AssertConnect();
                if (value == EquatorialCoordinateType.equLocalTopocentric)
                    GeminiHardware.Instance.Precession = false;
                else
                    if (value == EquatorialCoordinateType.equJ2000)
                        GeminiHardware.Instance.Precession = true;
                    else
                        throw new InvalidValueException("EquatorialSystem", value.ToString(), "equLocalTopocentric (1), or equJ2000 (2)");
                GeminiHardware.Instance.Trace.Exit("IT:EquatorialSystem.Set", value);
            }

        }

        public void FindHome()
        {
            GeminiHardware.Instance.Trace.Enter("IT:FindHome");
            AssertConnect();

            if (GeminiHardware.Instance.AtPark)
                throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            if (GeminiHardware.Instance.AtHome) return;
            /*
                        GeminiHardware.Instance.DoCommandResult(":hP", GeminiHardware.Instance.MAX_TIMEOUT, false);
                        GeminiHardware.Instance.WaitForHomeOrPark("Home");
                        GeminiHardware.Instance.DoCommandResult(":hW", GeminiHardware.Instance.MAX_TIMEOUT, false); //resume tracking, as FindHome isn't supposed to stop the mount
                        GeminiHardware.Instance.WaitForVelocity("TG", GeminiHardware.Instance.MAX_TIMEOUT);
             */

            m_FoundHome = GeminiHardware.Instance.DoHome();

            GeminiHardware.Instance.Trace.Exit("IT:FindHome");
        }

        public double FocalLength
        {
            get
            {
                try
                {
                    GeminiHardware.Instance.Trace.Enter("IT:Altitude.Get", GeminiHardware.Instance.FocalLength);
                    double focallength;
                    double.TryParse(GeminiHardware.Instance.FocalLength.Split('~')[GeminiHardware.Instance.OpticsValueIndex], out focallength);
                    return focallength / 1000; // focal length in meters
                }
                catch { return 0; }
            }
        }


        /// <summary>
        /// Same guide rate as RightAscension;
        /// </summary>
        public double GuideRateDeclination
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:GuideRateDeclination.Get", GuideRateRightAscension);
                AssertConnect();
                return GuideRateRightAscension;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:GuideRateDeclination.Set", value);
                AssertConnect();
                GuideRateRightAscension = value;
                GeminiHardware.Instance.Trace.Exit("IT:GuideRateDeclination.Set", value);
            }
        }

        /// <summary>
        /// Get/Set guiding rate in degrees/second
        /// Actual Gemini rates are 0.2 - 0.8x Sidereal
        /// </summary>
        public double GuideRateRightAscension
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:GuideRateRightAscesion.Get");
                AssertConnect();

                string result = GeminiHardware.Instance.DoCommandResult("<150:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                if (result == null) throw new TimeoutException("GuideRateRightAscention");
                double res = double.Parse(result, GeminiHardware.Instance.m_GeminiCulture) * SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0;
                GeminiHardware.Instance.Trace.Exit("IT:GuideRateRightAscesion.Get", res);
                return res;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:GuideRateRightAscesion.Set", value);

                double val = value / (SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0);
                if (val < 0.2 || val > 0.8) throw new InvalidValueException("GuideRate", value.ToString(), "");
                string cmd = ">150:" + val.ToString("0.0", GeminiHardware.Instance.m_GeminiCulture);    //internationalization issues?
                GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.Trace.Exit("IT:GuideRateRightAscesion.Set", value);
            }
        }

        public short InterfaceVersion
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:InterfaceVersion.Get", 2);
                return 2;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10);  // allow some delay for apps that query in a tight loop
                bool res = GeminiHardware.Instance.IsPulseGuiding;
                GeminiHardware.Instance.Trace.Enter("IT:IsPulseGuiding.Get", res);
                return res;
            }
        }

        private bool TrackingState = true;  //set when MoveAxis is initiated, restored when it ended
        private string RADivisors = "";     //set by MoveAxis before variable rate is set to be restored later

        public void MoveAxis(TelescopeAxes Axis, double Rate)
        {
            GeminiHardware.Instance.Trace.Enter("IT:MoveAxis", Axis, Rate);

            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);


            if (GeminiHardware.Instance.dVersion >= 6 && GeminiHardware.Instance.VariableMoveAxis)
            {
                VariableMoveAxis(Axis, Rate);
                return;
            }

            string[] cmds = { null, null };

            switch (Axis)
            {
                case TelescopeAxes.axisPrimary: //RA
                    if (Rate < 0) cmds[1] = ":Me";
                    else if (Rate > 0)
                        cmds[1] = ":Mw";
                    else
                    {
                        GeminiHardware.Instance.DoCommandResult(new string[] { ":Qe", ":Qw" }, GeminiHardware.Instance.MAX_TIMEOUT / 2, false); //stop motion in RA
                        GeminiHardware.Instance.WaitForVelocity("T", GeminiHardware.Instance.MAX_TIMEOUT);
                        if (!TrackingState)
                        {
                            GeminiHardware.Instance.DoCommandResult(":hN", GeminiHardware.Instance.MAX_TIMEOUT, false);
                            GeminiHardware.Instance.WaitForVelocity("N", GeminiHardware.Instance.MAX_TIMEOUT);
                            GeminiHardware.Instance.Tracking = false;
                        }
                        GeminiHardware.Instance.Trace.Exit("IT:MoveAxis", Axis, Rate);
                        return;
                    }
                    break;
                case TelescopeAxes.axisSecondary: //DEC
                    if (Rate < 0) cmds[1] = ":Ms";
                    else if (Rate > 0)
                        cmds[1] = ":Mn";
                    else
                    {
                        GeminiHardware.Instance.DoCommandResult(new string[] { ":Qn", ":Qs" }, GeminiHardware.Instance.MAX_TIMEOUT / 2, false); //stop motion in DEC
                        GeminiHardware.Instance.WaitForVelocity("T", GeminiHardware.Instance.MAX_TIMEOUT);
                        GeminiHardware.Instance.Trace.Exit("IT:MoveAxis", Axis, Rate);
                        if (!TrackingState)
                        {
                            GeminiHardware.Instance.DoCommandResult(":hN", GeminiHardware.Instance.MAX_TIMEOUT, false);
                            GeminiHardware.Instance.WaitForVelocity("N", GeminiHardware.Instance.MAX_TIMEOUT);
                            GeminiHardware.Instance.Tracking = false;
                        }
                        return;
                    }
                    break;
                default:
                    throw new ASCOM.InvalidValueException("MoveAxis", Axis.ToString(), "Primary,Secondary");
            }

            Rate = Math.Abs(Rate);

            const double RateTolerance = 1e-5;  // 1e-6 is 0.036 arcseconds/second

            // find the rate in the list of rates. The position will determine if it's
            // guiding, slewing, or centering rate:
            int cnt = 0;
            foreach (Rate r in AxisRates(Axis))
            {
                if (r.Minimum >= Rate - RateTolerance && r.Minimum <= Rate + RateTolerance) // use tolerance to ensure doubles compare properly
                    break;
                cnt++;
            }

            switch (cnt)
            {
                case 0: // slew rate
                    cmds[0] = ":RS"; break;
                case 1: // center rate
                    cmds[0] = ":RC"; break;
                case 2: // guide rate
                    cmds[0] = ":RG"; break;

                default:
                    throw new ASCOM.InvalidValueException("MoveAxis", Axis.ToString(), "guiding, centering, or slewing speeds");
            }

            TrackingState = GeminiHardware.Instance.Tracking;

            GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
            GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
            GeminiHardware.Instance.Velocity = "S"; //slew, as per ASCOM spec
            GeminiHardware.Instance.Trace.Exit("IT:MoveAxis", Axis, Rate);
        }

        private void VariableMoveAxis(TelescopeAxes axis, double rate)
        {
            switch(axis)
            {
                case TelescopeAxes.axisPrimary: VariableMoveAxisRA(rate); break;
                case TelescopeAxes.axisSecondary: VariableMoveAxisDEC(rate); break;
                default:
                    throw new ASCOM.InvalidValueException("MoveAxis", axis.ToString(), "Primary,Secondary");
            }
        }

        private void VariableMoveAxisDEC(double rate)
        {
            GeminiHardware.Instance.Trace.Enter("IT:MoveAxisDEC", rate);

            if (rate == 0) // stop motion
            {
                GeminiHardware.Instance.DoCommandResult(">454:0", GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
            }
            else
            {
                double spd = stepsPerDegree();
                int divisor = (int)Math.Round(12e6 / (spd * rate));
                string[] cmds = new string[]
                {
                    $">452:{divisor}",
                    ">454:1"
                };

                GeminiHardware.Instance.Trace.Info(4, "IT:MoveAxisDEC", spd, divisor);

                GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
                GeminiHardware.Instance.Velocity = "S"; //slew, as per ASCOM spec
            }
            GeminiHardware.Instance.Trace.Exit("IT:MoveAxisDEC", rate);

        }

       

        private void VariableMoveAxisRA(double rate)
        {
            GeminiHardware.Instance.Trace.Enter("IT:MoveAxisRA", rate);
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

                //save
                RADivisors = GeminiHardware.Instance.DoCommandResult("<451:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                double spd = stepsPerDegree();
                int divisor = (int)Math.Round(12e6 / (spd * rate));
                string[] cmds = new string[]
                {
                    $">451:{divisor}",
                    ">453:1"
                };

                GeminiHardware.Instance.Trace.Info(4, "IT:MoveAxisRA", spd, divisor);

                GeminiHardware.Instance.DoCommandResult(cmds, GeminiHardware.Instance.MAX_TIMEOUT / 2, false);
                GeminiHardware.Instance.WaitForVelocity("GCS", GeminiHardware.Instance.MAX_TIMEOUT);
                GeminiHardware.Instance.Velocity = "S"; //slew, as per ASCOM spec
            }
            GeminiHardware.Instance.Trace.Exit("IT:MoveAxisRA", rate);
        }


        private double stepsPerDegree()
        {
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
            return spd;
        }

        public string Name
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:Name.Get", SharedResources.TELESCOPE_DRIVER_NAME);
                return SharedResources.TELESCOPE_DRIVER_NAME;
            }
        }

        public void Park()
        {
            GeminiHardware.Instance.Trace.Enter("IT:Park");
            AssertConnect();

            if (GeminiHardware.Instance.AtPark) return;  // already there

            // synchronous with this thread, don't return until done:
            GeminiHardware.Instance.DoPark(GeminiHardware.Instance.ParkPosition);

            GeminiHardware.Instance.Trace.Exit("IT:Park");
        }

        /// <summary>
        /// PulseGuide for Gemini L5 and above
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Duration"></param>
        public void PulseGuideL5(GuideDirections Direction, int Duration)
        {
            GeminiHardware.Instance.Trace.Enter("IT:PulseGuide5", Direction, Duration);

            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);
            if (internalSlewing)
            {
                GeminiHardware.Instance.Trace.Info(2, "IT:PulseGuide5", SharedResources.MSG_INVALID_WHILE_SLEWING);
                return; // throw new DriverException(SharedResources.MSG_INVALID_WHILE_SLEWING, (int)SharedResources.INVALID_WHILE_SLEWING);
            }

            string cmd = String.Empty;

            switch (Direction)
            {
                case GuideDirections.guideEast:
                    cmd = ":Mge";
                    break;
                case GuideDirections.guideNorth:
                    cmd = ":Mgn";
                    break;
                case GuideDirections.guideSouth:
                    cmd = ":Mgs";
                    break;
                case GuideDirections.guideWest:
                    cmd = ":Mgw";
                    break;
            }

            if (Duration > 60000 || Duration <= 0)  // too large or zero/negative...
                throw new InvalidValueException("PulseGuide5", Duration.ToString(), "1..60000");


            string c = cmd + Duration.ToString();

            // Set time for pulse guide command to be started (used by IsPulseGuiding property)
            // IsPulseGuiding will report true until this many milliseconds elapse.
            // After this time, IsPulseGuiding will query the mount for tracking speed
            // to return the proper status. This is necessary because Gemini doesn't immediately
            // set 'G' or 'C' tracking rate when pulse-guiding command is issued and continues to track
            // for a little while. Use 1/2 of the total duration or 100 milliseconds, whichever is greater:
            GeminiHardware.Instance.EndOfPulseGuide = Math.Max(Duration / 2, 100);

            GeminiHardware.Instance.Velocity = "G";
            GeminiHardware.Instance.DoCommandResult(c, Duration + GeminiHardware.Instance.MAX_TIMEOUT, false);

            if (!GeminiHardware.Instance.AsyncPulseGuide)
                GeminiHardware.Instance.WaitForVelocity("TN", Duration + GeminiHardware.Instance.MAX_TIMEOUT); // shouldn't take much longer than 'Duration', right?

            GeminiHardware.Instance.Trace.Exit("IT:PulseGuide5", Direction, Duration, GeminiHardware.Instance.AsyncPulseGuide);
        }

        /// <summary>
        /// Send pulse-guide commands to the mount in the required direction, for the required duration
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Duration"></param>
        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            if (GeminiHardware.Instance.GeminiLevel > 4)
            {
                PulseGuideL5(Direction, Duration);
                return;
            }

            if (!GeminiHardware.Instance.PrecisionPulseGuide)
            {
                OldPulseGuide(Direction, Duration);
                return;
            }


            GeminiHardware.Instance.Trace.Enter("IT:PulseGuide", Direction, Duration);

            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            // don't update mount parameters each time a guide command is issued: this will slow things down while guiding
            // do it on a polling interval:
            if (DateTime.Now - m_LastPulseGuideUpdate > TimeSpan.FromMilliseconds(SharedResources.PULSEGUIDE_POLLING_INTERVAL) ||
                m_GuideRateStepsPerMilliSecond == 0)
            {
                string[] mountpar = null;

                GeminiHardware.Instance.DoCommandResult(new string[] { "<21:", "<23:", "<25:" }, GeminiHardware.Instance.MAX_TIMEOUT / 2, false, out mountpar);

                if (mountpar != null)
                {
                    string wormGearRatio = mountpar[0];
                    string spurGearRatio = mountpar[1];
                    string encoderResolution = mountpar[2];

                    // compute actual tracking rate, including any offset, in arcsecs/second

                    m_TrackRate = (this.RightAscensionRate * 0.9972695677 + SharedResources.EARTH_ANG_ROT_DEG_MIN * 60);

                    if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null)
                    {

                        double StepsPerDegree = (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution)) / 360.0;
                        m_GuideRA = GuideRateRightAscension;

                        m_GuideRateStepsPerMilliSecond = StepsPerDegree * m_GuideRA / 1000;  // guide rate in encoder ticks per milli-second

                        m_GuideRateStepsPerMilliSecondEast = StepsPerDegree * (m_TrackRate / 3600 - m_GuideRA) / 1000;
                        m_GuideRateStepsPerMilliSecondWest = StepsPerDegree * (m_TrackRate / 3600 + m_GuideRA) / 1000;
                        m_LastPulseGuideUpdate = DateTime.Now;

                        GeminiHardware.Instance.Trace.Info(3, "PulseGuide Param", m_GuideRateStepsPerMilliSecond, m_GuideRateStepsPerMilliSecondEast, m_GuideRateStepsPerMilliSecondWest);
                    }
                }
            }

            if (m_GuideRateStepsPerMilliSecond == 0) // never did get the rate! 
                throw new ASCOM.InvalidValueException(SharedResources.MSG_INVALID_VALUE);

            string cmd = String.Empty;

            //switch (Direction)
            //{
            //    case GuideDirections.guideEast:
            //        cmd = ":Mie";
            //        break;
            //    case GuideDirections.guideNorth:
            //        cmd = ":Min";
            //        break;
            //    case GuideDirections.guideSouth:
            //        cmd = ":Mis";
            //        break;
            //    case GuideDirections.guideWest:
            //        cmd = ":Miw";
            //        break;
            //}


            int maxduration = (int)(255 / m_GuideRateStepsPerMilliSecond);

            int prescaler = 1;

            switch (Direction)
            {
                case GuideDirections.guideEast:
                    cmd = ":Mge";
                    maxduration = (int)(255 / m_GuideRateStepsPerMilliSecondEast);
                    // perhaps a bug in Gemini: the prescaler value used for East guiding rate 
                    // needs to be reversed for West guiding rate.

                    // actual divisor:
                    //prescaler = (int)(1500 / m_GuideRateStepsPerMilliSecondWest);

                    //// prescaler needed to fit into 16 bits:
                    //prescaler = (prescaler / 65536) + 1;

                    // adjust duration to account for prescaler:
                    Duration *= prescaler;
                    break;
                case GuideDirections.guideNorth:
                    cmd = ":Mgn";
                    break;
                case GuideDirections.guideSouth:
                    cmd = ":Mgs";
                    break;
                case GuideDirections.guideWest:
                    maxduration = (int)(255 / m_GuideRateStepsPerMilliSecondWest);

                    // factor is due to different step speed in West direction: (1+g)/(1-g):
                    double fact = (1 + m_GuideRA / (SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0)) / (1 - m_GuideRA / (SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0));

                    Duration = (int)(Duration / fact);

                    // perhaps a bug in Gemini: the prescaler value used for East guiding rate 
                    // needs to be reversed for West guiding rate.

                    // actual divisor:
                    prescaler = (int)(1500 / m_GuideRateStepsPerMilliSecondEast);

                    // prescaler needed to fit into 16 bits:
                    prescaler = (prescaler / 65536) + 1;
                    // adjust duration to account for prescaler:
                    Duration *= prescaler;

                    cmd = ":Mgw";
                    break;
            }

            // max duration is rounded to whole seconds, as per Rene G.:
            maxduration = ((int)(maxduration / 1000)) * 1000;

            //System.Windows.Forms.MessageBox.Show("Max duration: " + maxduration.ToString() + "\r\n" +
            //        "Guide Rate: " + (m_GuideRA / (SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0)).ToString() + "\r\n" +
            //        "East Steps/Sec: " + (m_GuideRateStepsPerMilliSecondEast * 1000).ToString() + "\r\n" +
            //        "West Steps/Sec: " + (m_GuideRateStepsPerMilliSecondWest * 1000).ToString() + "\r\n" +
            //        "Prescaler     : " + prescaler.ToString());


            GeminiHardware.Instance.Trace.Info(3, "PulseGuide MaxDuration", maxduration);

            int totalSteps = (int)(Duration * m_GuideRateStepsPerMilliSecond + 0.5); // total encoder ticks 
            GeminiHardware.Instance.Trace.Info(4, "IT:PulseGuide Ticks", totalSteps);
            GeminiHardware.Instance.Trace.Info(4, "IT:PulseGuide MaxDur", maxduration);

            if (Duration > 60000 || Duration < 0)  // too large or negative...
                throw new InvalidValueException("PulseGuide", Duration.ToString(), "0..60000");

            if (totalSteps <= 0) return;    //too small a move (less than 1 encoder tick)

            int count = Duration;

            for (int idx = 0; count > 0; ++idx)
            {
                int d = (count > maxduration ? maxduration : count);
                string c = cmd + d.ToString();

                // Set time for pulse guide command to be started (used by IsPulseGuiding property)
                // IsPulseGuiding will report true until this many milliseconds elapse.
                // After this time, IsPulseGuiding will query the mount for tracking speed
                // to return the proper status. This is necessary because Gemini doesn't immediately
                // set 'G' or 'C' tracking rate when pulse-guiding command is issued and continues to track
                // for a little while. Use 1/2 of the total duration or 100 milliseconds, whichever is greater:
                GeminiHardware.Instance.EndOfPulseGuide = Math.Max(d / 2, 100);

                GeminiHardware.Instance.DoCommandResult(c, Duration + GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.Velocity = "G";
                count -= d;


                if (!GeminiHardware.Instance.AsyncPulseGuide || count > 0)
                    GeminiHardware.Instance.WaitForVelocity("TN", Duration + GeminiHardware.Instance.MAX_TIMEOUT); // shouldn't take much longer than 'Duration', right?
            }
            GeminiHardware.Instance.Trace.Exit("IT:PulseGuide", Direction, Duration, totalSteps, GeminiHardware.Instance.AsyncPulseGuide);
        }

        /// <summary>
        /// Use pc timing to execute pulse-guiding commands instead of Gemini precision-guiding commands
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Duration"></param>
        private void OldPulseGuide(GuideDirections Direction, int Duration)
        {
            GeminiHardware.Instance.Trace.Enter("IT:OldPulseGuide", Direction, Duration, GeminiHardware.Instance.AsyncPulseGuide);
            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            if (internalSlewing)
            {
                GeminiHardware.Instance.Trace.Info(2, "IT:OldPulseGuide", SharedResources.MSG_INVALID_WHILE_SLEWING);
                return; // throw new DriverException(SharedResources.MSG_INVALID_WHILE_SLEWING, (int)SharedResources.INVALID_WHILE_SLEWING);
            }

            if (Duration > 60000 || Duration < 0)  // too large or negative...
                throw new InvalidValueException("PulseGuide", Duration.ToString(), "0..60000");

            if (Duration == 0) return;

            string[] cmd = new string[2];

            cmd[0] = ":RG";

            switch (Direction)
            {
                case GuideDirections.guideEast:
                    cmd[1] = ":Me";
                    break;
                case GuideDirections.guideNorth:
                    cmd[1] = ":Mn";
                    break;
                case GuideDirections.guideSouth:
                    cmd[1] = ":Ms";
                    break;
                case GuideDirections.guideWest:
                    cmd[1] = ":Mw";
                    break;
            }

            // Set time for pulse guide command to be started (used by IsPulseGuiding property)
            // IsPulseGuiding will report true until this many milliseconds elapse.
            // After this time, IsPulseGuiding will query the mount for tracking speed
            // to return the proper status. This is necessary because Gemini doesn't immediately
            // set 'G' or 'C' tracking rate when pulse-guiding command is issued and continues to track
            // for a little while. Use the total duration:
            GeminiHardware.Instance.EndOfPulseGuide = Duration;

            GeminiHardware.Instance.Velocity = "G";

            GeminiHardware.Instance.DoPulseCommand(cmd, Duration, GeminiHardware.Instance.AsyncPulseGuide);

            GeminiHardware.Instance.Trace.Exit("IT:OldPulseGuide", Direction, Duration, GeminiHardware.Instance.AsyncPulseGuide);
        }

        public double RightAscension
        {

            get
            {
                AssertConnect();

                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)
                double res = GeminiHardware.Instance.RightAscension;

                // adjust output to J2000 if that's the setting and level below L6:
                if (GeminiHardware.Instance.Precession && GeminiHardware.Instance.dVersion < 6)
                {
                    GeminiHardware.Instance.Trace.Info(2, "Converting RA to J2000", res);
                    double dec = GeminiHardware.Instance.Declination;
                    AstronomyFunctions.ToJ2000(ref res, ref dec);
                }
                GeminiHardware.Instance.Trace.Enter("IT:RightAscention.Get", res);
                return res;
            }
        }

        /// <summary>
        /// The right ascension tracking rate offset from sidereal (seconds per sidereal second, default = 0.0)
        /// </summary>
        public double RightAscensionRate
        {

            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:RightAscensionRate.Get");
                AssertConnect();
                string rateDivisor = GeminiHardware.Instance.DoCommandResult("<411:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string mountType = GeminiHardware.Instance.DoCommandResult("<0:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                double preScaler = 1;

                if (rateDivisor != null && spurGearRatio != null && wormGearRatio != null && encoderResolution != null && mountType != null)
                {

                    double frequency = 0;

                    if (GeminiHardware.Instance.GeminiLevel < 5)
                    {
                        if ((mountType == "1" || mountType == "5")) preScaler = 2;
                        frequency = 1500000;
                    }
                    else
                    {
                        frequency = 12000000;
                    }

                    double drateDivisor = double.Parse(rateDivisor);
                    if (drateDivisor == 0) return 0;

                    double stepsPerSecond = (frequency / preScaler) / drateDivisor;
                    double arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                    double rate = arcSecondsPerStep * stepsPerSecond;

                    double sidereal = 15.041112373821779; // computed from Gemini divisor - actual sidereal rate of tracking
                    double offsetRate = (rate - sidereal) / 0.9972695677;

                    GeminiHardware.Instance.Trace.Exit("IT:RightAscensionRate.Get", offsetRate);
                    return Math.Round(offsetRate, 4);
                }
                else
                    throw new TimeoutException("RightAscensionRate");
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:RightAscensionRate.Set", value);
                AssertConnect();

                // default case, set sidereal tracking and return:
                if (value == 0)
                {
                    TrackingRate = DriveRates.driveSidereal;
                    return;
                }

                string wormGearRatio = GeminiHardware.Instance.DoCommandResult("<21:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string spurGearRatio = GeminiHardware.Instance.DoCommandResult("<23:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string encoderResolution = GeminiHardware.Instance.DoCommandResult("<25:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                string mountType = GeminiHardware.Instance.DoCommandResult("<0:", GeminiHardware.Instance.MAX_TIMEOUT, false);

                double preScaler = 1;

                if (spurGearRatio != null && wormGearRatio != null && encoderResolution != null && mountType != null)
                {

                    double frequency = 0;

                    if (GeminiHardware.Instance.GeminiLevel < 5)
                    {
                        // GM-8, Titan have prescaler of 2:
                        if ((mountType == "1" || mountType == "5")) preScaler = 2;
                        frequency = 1500000;
                    }
                    else
                    {
                        frequency = 12000000;
                    }

                    double sidereal = 15.041112373821779; // computed from Gemini divisor - actual sidereal rate of tracking

                    double offsetRate = value * 0.9972695677 + sidereal; //arcseconds per second

                    double arcSecondsPerStep = 1296000.00 / (Math.Abs(double.Parse(wormGearRatio)) * double.Parse(spurGearRatio) * double.Parse(encoderResolution));

                    double stepsPerSecond = offsetRate / arcSecondsPerStep;

                    int rateDivisor = (int)((frequency / preScaler) / stepsPerSecond + 0.5);

                    if (GeminiHardware.Instance.GeminiLevel < 5)
                        if (rateDivisor < 256 || rateDivisor > 65535) throw new InvalidValueException("RightAscensionRate", value.ToString(), "Rate cannot be implemented");

                    if (value != 0)
                        GeminiHardware.Instance.DoCommand(">137:", false); // set Comet tracking rate so we can adjust RA rate

                    string cmd = ">411:" + rateDivisor.ToString();
                    GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.Trace.Exit("IT:RightAscensionRate.Set", value);
                }
                else throw new TimeoutException("RightAscensionRate");
            }
        }

        public void SetPark()
        {
            AssertConnect();
            GeminiHardware.Instance.ParkAlt = GeminiHardware.Instance.Altitude;
            GeminiHardware.Instance.ParkAz = GeminiHardware.Instance.Azimuth;
            GeminiHardware.Instance.ParkPosition = GeminiHardwareBase.GeminiParkMode.SlewAltAz;
            GeminiHardware.Instance.Profile = null;
            GeminiHardware.Instance.Trace.Exit("IT:SetPark", GeminiHardware.Instance.ParkAlt, GeminiHardware.Instance.ParkAz);
        }

        public void SetupDialog()
        {
            GeminiHardware.Instance.Trace.Enter("IT:SetupDialog");

            //if (GeminiHardware.Instance.Connected)
            //{
            //    throw new DriverException("The hardware is connected, cannot do SetupDialog()",
            //                        unchecked(ErrorCodes.DriverBase + 4));
            //}
            GeminiTelescope.m_MainForm.DoTelescopeSetupDialog();
            GeminiHardware.Instance.Trace.Exit("IT:SetupDialog");
        }

        public PierSide SideOfPier
        {
            get
            {
                AssertConnect();
                if (!GeminiHardware.Instance.ReportPierSide)
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", "ReportPierSide is false, throwing PropertyNotImplementedException");
                    throw new PropertyNotImplementedException("SideOfPier", false); // PWGS - was return PierSide.pierUnknown;
                }

                double mech_declination = 0;

                try
                {
                    mech_declination = GeminiHardware.Instance.MechanicalDeclination;
                }
                catch (Exception x)
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierUnknown, x.ToString());
                    return PierSide.pierUnknown;
                }

                if (Math.Abs(mech_declination) > 90)
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierEast, mech_declination);
                    return PierSide.pierEast;
                }
                else
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierWest, mech_declination);
                    return PierSide.pierWest;
                }
            }

            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Set", value);
                AssertConnect();

                // PWGS - If we aren't reporting SideOfPier then it shouldn't be possible to set it either
                if (!GeminiHardware.Instance.ReportPierSide)
                {
                    GeminiHardware.Instance.Trace.Exit("IT:SideOfPier.Set", "ReportPierSide is false, throwing PropertyNotImplementedException");
                    throw new PropertyNotImplementedException("SideOfPier", true);
                }

                if ((value == PierSide.pierEast && GeminiHardware.Instance.SideOfPier == "W") || (value == PierSide.pierWest && GeminiHardware.Instance.SideOfPier == "E"))
                {
                    try
                    {
                        GeminiHardware.Instance.IgnoreErrors = true;

                        string res = GeminiHardware.Instance.DoMeridianFlip();

                        if (res == null) throw new TimeoutException("SideOfPier");
                        if (res.StartsWith("1")) throw new ASCOM.InvalidOperationException("Object below horizon");
                        if (res.StartsWith("4")) throw new ASCOM.InvalidOperationException("Position unreachable");
                        if (res.StartsWith("3")) throw new ASCOM.InvalidOperationException("Manual control");

                        GeminiHardware.Instance.WaitForVelocity("S", GeminiHardware.Instance.MAX_TIMEOUT);
                        GeminiHardware.Instance.WaitForVelocity("TN", -1);  // :Mf is asynchronous, wait until done
                    }
                    finally
                    {
                        GeminiHardware.Instance.IgnoreErrors = false;
                    }
                }
                GeminiHardware.Instance.Trace.Exit("IT:SideOfPier.Set", value);

            }
        }

        /// <summary>
        /// custom property for Gemini -- report physical side of pier
        /// </summary>
        public PierSide PhysicalSideOfPier
        {
            get
            {
                AssertConnect();
                if (!GeminiHardware.Instance.ReportPierSide) return PierSide.pierUnknown;

                if (GeminiHardware.Instance.SideOfPier == "E")
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierEast);
                    return PierSide.pierEast;
                }
                else if (GeminiHardware.Instance.SideOfPier == "W")
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierWest);
                    return PierSide.pierWest;
                }
                else
                {
                    GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Get", PierSide.pierUnknown);
                    return PierSide.pierUnknown;
                }
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:SideOfPier.Set", value);
                AssertConnect();

                if ((value == PierSide.pierEast && GeminiHardware.Instance.SideOfPier == "W") || (value == PierSide.pierWest && GeminiHardware.Instance.SideOfPier == "E"))
                {

                    GeminiHardware.Instance.Trace.Info(4, "IT:SideOfPier.Set", "Before DoMeridianFlip");

                    string res = GeminiHardware.Instance.DoMeridianFlip();

                    GeminiHardware.Instance.Trace.Info(4, "IT:SideOfPier.Set",  "After DoMeridianFlip", res);

                    if (res == null) throw new TimeoutException("SideOfPier");
                    if (res.StartsWith("1")) throw new ASCOM.InvalidOperationException("Object below horizon");
                    if (res.StartsWith("4")) throw new ASCOM.InvalidOperationException("Position unreachable");
                    if (res.StartsWith("3")) throw new ASCOM.InvalidOperationException("Manual control");

                    GeminiHardware.Instance.Trace.Info(4, "IT:SideOfPier.Set", "Wait for slew", res);
                    GeminiHardware.Instance.WaitForVelocity("S", GeminiHardware.Instance.MAX_TIMEOUT);
                    GeminiHardware.Instance.Trace.Info(4, "IT:SideOfPier.Set", "Wait for tracking", res);
                    GeminiHardware.Instance.WaitForVelocity("TN", -1);  // :Mf is asynchronous, wait until done
                }
                GeminiHardware.Instance.Trace.Exit("IT:SideOfPier.Set", value);

            }
        }



        public double SiderealTime
        {
            get
            {
                AssertConnect();
                double res = GeminiHardware.Instance.SiderealTime;
                GeminiHardware.Instance.Trace.Enter("IT:SiderealTime.Get", res);
                return res;
            }
        }

        public double SiteElevation
        {
            get { return GeminiHardware.Instance.Elevation; }
            set
            {
                if (value < -300 || value > 10000)
                {
                    throw new InvalidValueException("SiteElevation", value.ToString(), "-300...10000");
                }
                GeminiHardware.Instance.Elevation = value;
            }
        }

        public double SiteLatitude
        {
            get
            {

                AssertConnect();
                double res = GeminiHardware.Instance.Latitude;
                GeminiHardware.Instance.Trace.Enter("IT:SiteLatitude.Get", res);
                return res;
            }
            set
            {

                GeminiHardware.Instance.Trace.Enter("IT:SiteLatitude.Set", value);
                AssertConnect();

                if (value < -90 || value > 90)
                {
                    throw new ASCOM.InvalidValueException("SiteLatitude", value.ToString(), "-90..90");
                }
                GeminiHardware.Instance.SetLatitude(value);
                GeminiHardware.Instance.Trace.Exit("IT:SiteLatitude.Set", value);
            }
        }

        public double SiteLongitude
        {
            get
            {
                AssertConnect();
                double res = GeminiHardware.Instance.Longitude;
                GeminiHardware.Instance.Trace.Enter("IT:SiteLongitude.Get", res);
                return res;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:SiteLongitude.Set", value);
                AssertConnect();

                if (value < -180 || value > 180)
                {
                    throw new ASCOM.InvalidValueException("SiteLongitude", value.ToString(), "-180..180");
                }
                GeminiHardware.Instance.SetLongitude(value);
                GeminiHardware.Instance.Trace.Exit("IT:SiteLongitude.Set", value);
            }
        }

        public short SlewSettleTime
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:SlewSettleTime.Get", GeminiHardware.Instance.SlewSettleTime);
                return (short)GeminiHardware.Instance.SlewSettleTime;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:SlewSettleTime.Set", value);

                if (value < 0 || value > 100) throw new ASCOM.InvalidValueException("SlewSettleTime", value.ToString(), "0-100 seconds");
                GeminiHardware.Instance.SlewSettleTime = (int)value;
                GeminiHardware.Instance.Trace.Exit("IT:SlewSettleTime.Set", value);
            }
        }

        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToAltAz", Azimuth, Altitude);
            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            GeminiHardware.Instance.TargetAzimuth = Azimuth;
            GeminiHardware.Instance.TargetAltitude = Altitude;
            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewHorizon();
            GeminiHardware.Instance.WaitForSlewToEnd();
            GeminiHardware.Instance.Trace.Exit("IT:SlewToAltAz", Azimuth, Altitude);
        }

        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToAltAzAsync", Azimuth, Altitude);

            AssertConnect();
            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            GeminiHardware.Instance.TargetAzimuth = Azimuth;
            GeminiHardware.Instance.TargetAltitude = Altitude;
            if (internalSlewing) GeminiHardware.Instance.AbortSlewSync();
            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewHorizonAsync();
            GeminiHardware.Instance.WaitForVelocity("SC", GeminiHardware.Instance.MAX_TIMEOUT);
            m_AsyncSlewStarted = true;
            GeminiHardware.Instance.Trace.Exit("IT:SlewToAltAzAsync", Azimuth, Altitude);
        }

        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToCoordinates", RightAscension, Declination);
            AssertConnect();

            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            GeminiHardware.Instance.TargetRightAscension = RightAscension;
            GeminiHardware.Instance.TargetDeclination = Declination;
            if (internalSlewing) GeminiHardware.Instance.AbortSlewSync();

            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewEquatorial();

            GeminiHardware.Instance.WaitForSlewToEnd();
            GeminiHardware.Instance.Trace.Exit("IT:SlewToCoordinates", RightAscension, Declination);
        }

        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToCoordinatesAsync", RightAscension, Declination);
            AssertConnect();

            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            GeminiHardware.Instance.TargetRightAscension = RightAscension;
            GeminiHardware.Instance.TargetDeclination = Declination;
            if (internalSlewing) GeminiHardware.Instance.AbortSlewSync();

            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewEquatorialAsync();
            //            GeminiHardware.Instance.WaitForVelocity("SC", GeminiHardware.Instance.MAX_TIMEOUT);
            GeminiHardware.Instance.WaitForVelocity("SC", GeminiHardware.Instance.MAX_TIMEOUT);
            m_AsyncSlewStarted = true;
            GeminiHardware.Instance.Trace.Exit("IT:SlewToCoordinatesAsync", RightAscension, Declination);
        }

        public void SlewToTarget()
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToTarget", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);
            AssertConnect();

            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            if (internalSlewing) GeminiHardware.Instance.AbortSlewSync();
            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewEquatorial();
            GeminiHardware.Instance.WaitForSlewToEnd();
            GeminiHardware.Instance.Trace.Exit("IT:SlewToTarget", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);

        }

        public void SlewToTargetAsync()
        {
            GeminiHardware.Instance.Trace.Enter("IT:SlewToTargetAsync", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);
            AssertConnect();

            if (GeminiHardware.Instance.AtPark) throw new InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            if (internalSlewing)  GeminiHardware.Instance.AbortSlewSync();
            GeminiHardware.Instance.Velocity = "S";
            GeminiHardware.Instance.SlewEquatorialAsync();
            GeminiHardware.Instance.WaitForVelocity("SC", GeminiHardware.Instance.MAX_TIMEOUT);
            m_AsyncSlewStarted = true;
            GeminiHardware.Instance.Trace.Exit("IT:SlewToTargetAsync", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);
        }

        public bool Slewing
        {
            get
            {
                bool res = internalSlewing;
                GeminiHardware.Instance.Trace.Enter("IT:Slewing.Get", res);
                return res;
            }
        }

        public bool internalSlewing
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)

                if (GeminiHardware.Instance.Velocity == "S" || GeminiHardware.Instance.Velocity == "C")
                {
                    return true;
                }
                else
                {
                    if (m_AsyncSlewStarted) // need to wait out the slewsettletime here...
                    {
                        System.Threading.Thread.Sleep((GeminiHardware.Instance.SlewSettleTime + 2) * 1000);
                        m_AsyncSlewStarted = false;
                    }
                    return false;
                }
            }
        }


        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SyncToAltAz", Azimuth, Altitude);

            throw new ASCOM.MethodNotImplementedException("SyncToAltAz");   // Gemini doesn't process Sync to Alt/Az in GEM mode

            AssertConnect();

            GeminiHardware.Instance.SyncHorizonCoordinates(Azimuth, Altitude);

            GeminiHardware.Instance.Trace.Exit("IT:SyncToAltAz", Azimuth, Altitude);
        }

        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            GeminiHardware.Instance.Trace.Enter("IT:SyncToCoordinates", RightAscension, Declination);

            AssertConnect();
            if (GeminiHardware.Instance.AtPark)
                throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            GeminiHardware.Instance.SyncToEquatorialCoords(RightAscension, Declination);
            GeminiHardware.Instance.Trace.Exit("IT:SyncToCoordinates", RightAscension, Declination);
        }

        public void SyncToTarget()
        {
            GeminiHardware.Instance.Trace.Enter("IT:SyncToTarget", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);
            AssertConnect();

            if (GeminiHardware.Instance.AtPark)
                throw new ASCOM.InvalidOperationException(SharedResources.MSG_INVALID_AT_PARK);

            if (TargetDeclination == SharedResources.INVALID_DOUBLE || TargetRightAscension == SharedResources.INVALID_DOUBLE)
                throw new ASCOM.InvalidOperationException(SharedResources.MSG_PROP_NOT_SET);
            GeminiHardware.Instance.SyncEquatorial();
            GeminiHardware.Instance.Trace.Exit("IT:SyncToTarget", GeminiHardware.Instance.TargetRightAscension, GeminiHardware.Instance.TargetDeclination);
        }

        public double TargetDeclination
        {
            get
            {
                AssertConnect();
                double val = GeminiHardware.Instance.TargetDeclination;
                GeminiHardware.Instance.Trace.Enter("IT:TargetDeclination.Get", val);

                if (val == SharedResources.INVALID_DOUBLE)
                    throw new ASCOM.ValueNotSetException("TargetDeclination");
                return val;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:TargetDeclination.Set", value);
                AssertConnect();
                if (value < -90 || value > 90)
                {
                    throw new ASCOM.InvalidValueException("TargetDeclination", value.ToString(), "-90..90");
                }
                GeminiHardware.Instance.TargetDeclination = value;
            }
        }

        public double TargetRightAscension
        {
            // TODO Replace this with your implementation
            get
            {

                AssertConnect();
                double val = GeminiHardware.Instance.TargetRightAscension;
                GeminiHardware.Instance.Trace.Enter("IT:TargetRightAscension.Get", val);

                if (val == SharedResources.INVALID_DOUBLE)
                    throw new ValueNotSetException("TargetRightAscension");
                return val;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:TargetRightAscension.Set", value);

                AssertConnect();
                if (value < 0 || value > 24)
                {
                    throw new InvalidValueException("TargetRightAscension", value.ToString(), "0..24");
                }
                GeminiHardware.Instance.TargetRightAscension = value;
            }
        }

        public bool Tracking
        {
            get
            {
                AssertConnect();
                System.Threading.Thread.Sleep(10); // since this is a polled property, don't let the caller monopolize the cpu in a tight loop (StaryNights!)                
                bool res = GeminiHardware.Instance.Tracking;
                GeminiHardware.Instance.Trace.Enter("IT:Tracking.Get", res);
                return res;
            }
            set
            {
                GeminiHardware.Instance.Trace.Enter("IT:Tracking.Set", value);

                AssertConnect();
                if (value /*&& !GeminiHardware.Instance.Tracking*/)
                {
                    GeminiHardware.Instance.DoCommandResult(":hW", GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.WaitForVelocity("TG", GeminiHardware.Instance.MAX_TIMEOUT);
                    GeminiHardware.Instance.Tracking = true;
                }
                if (!value /*&& GeminiHardware.Instance.Tracking*/)
                {
                    GeminiHardware.Instance.DoCommandResult(":hN", GeminiHardware.Instance.MAX_TIMEOUT, false);
                    GeminiHardware.Instance.WaitForVelocity("N", GeminiHardware.Instance.MAX_TIMEOUT);
                    GeminiHardware.Instance.Tracking = false;
                }
                GeminiHardware.Instance.Trace.Exit("IT:Tracking.Set", value);
            }
        }


        public DriveRates TrackingRate
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:TrackingRate.Get");
                AssertConnect();

                string res = GeminiHardware.Instance.DoCommandResult("<130:", GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.Trace.Exit("IT:TrackingRate.Get", res);
                switch (res)
                {
                    case "131": return DriveRates.driveSidereal;
                    case "132": return DriveRates.driveKing;
                    case "133": return DriveRates.driveLunar;
                    case "134": return DriveRates.driveSolar;
                    case null: throw new TimeoutException("Get TrackingRate");
                    default: throw new ASCOM.PropertyNotImplementedException("TrackingRate for custom rate", false);
                }
            }
            set
            {

                GeminiHardware.Instance.Trace.Enter("IT:TrackingRate.Set", value);
                AssertConnect();

                string cmd = "";

                switch (value)
                {
                    case DriveRates.driveSidereal: cmd = ">131:"; break;
                    case DriveRates.driveKing: cmd = ">132:"; break;
                    case DriveRates.driveLunar: cmd = ">133:"; break;
                    case DriveRates.driveSolar: cmd = ">134:"; break;
                    default:
                        throw new ASCOM.InvalidValueException("TrackingRate.Set", value.ToString(), "Sidereal,King,Lunar,Solar");
                }
                GeminiHardware.Instance.DoCommandResult(cmd, GeminiHardware.Instance.MAX_TIMEOUT, false);
                GeminiHardware.Instance.Trace.Exit("IT:TrackingRate.Set", value);
            }
        }

        public ITrackingRates TrackingRates
        {
            get
            {
                GeminiHardware.Instance.Trace.Enter("IT:TrackingRates.Get");
                return new TrackingRates(); // changed for Platform 6 conform: previous version returned a single static enumerator that never got reset
            }
        }

        public DateTime UTCDate
        {

            get
            {
                AssertConnect();
                DateTime res = GeminiHardware.Instance.UTCDate;
                GeminiHardware.Instance.Trace.Enter("IT:UTCDate.Get", res);
                return res;
            }
            set
            {
                AssertConnect();
                GeminiHardware.Instance.Trace.Enter("IT:UTCDate.Set", value);
                GeminiHardware.Instance.UTCDate = value;
                GeminiHardware.Instance.Trace.Exit("IT:UTCDate.Set", value);
            }
        }

        public void Unpark()
        {
            GeminiHardware.Instance.Trace.Enter("IT:Unpark");
            AssertConnect();

            GeminiHardware.Instance.DoCommandResult(":hW", GeminiHardware.Instance.MAX_TIMEOUT, false);
            GeminiHardware.Instance.WaitForVelocity("T", GeminiHardware.Instance.MAX_TIMEOUT);
            GeminiHardware.Instance.Trace.Exit("IT:Unpark");
        }

#endregion


        public string Action(string ActionName, string ActionParameters)
        {
            //throw new ASCOM.ActionNotImplementedException(ActionName);
            throw new ASCOM.MethodNotImplementedException("Action");
        }

        public ArrayList SupportedActions
        {
            get { return new ArrayList(); }
        }

        public void Dispose()
        {
        }


        /// <summary>
        /// Condition an hour angle value into the range -11.99999:0.0:+12.0 range
        /// </summary>
        /// <param name="HA"></param>
        /// <returns></returns>
        private double ConditionHA(double HA)
        {
            double retVal = HA; // Set the return value to the received HA
            if (HA <= -12.0) retVal += 24.0; // It is less or equal to -12 so convert to a positive value
            if (HA > 12.0) retVal -= 24.0; // It is greater than 12.0 so convert to a negative value
            return retVal;
        }

    }

    //
    // The Rate class implements IRate, and is used to hold values
    // for AxisRates. You do not need to change this class.
    //
    // The Guid attribute sets the CLSID for ASCOM.Telescope.Rate
    // The ClassInterface/None addribute prevents an empty interface called
    // _Rate from being created and used as the [default] interface
    //
    [Guid("84f0df4b-8d54-41d6-be96-1b8a3c98ef03")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Rate : IRate
    {
        private double m_dMaximum = 0;
        private double m_dMinimum = 0;

        //
        // Default constructor - Internal prevents public creation
        // of instances. These are values for AxisRates.
        //
        internal Rate(double Minimum, double Maximum)
        {
            m_dMaximum = Maximum;
            m_dMinimum = Minimum;
        }

#region IRate Members

        public double Maximum
        {
            get { return m_dMaximum; }
            set { m_dMaximum = value; }
        }

        public double Minimum
        {
            get { return m_dMinimum; }
            set { m_dMinimum = value; }
        }

#endregion
    }

    //
    // AxisRates is a strongly-typed collection that must be enumerable by
    // both COM and .NET. The IAxisRates and IEnumerable interfaces provide
    // this polymorphism. 
    //
    // The Guid attribute sets the CLSID for ASCOM.Telescope.AxisRates
    // The ClassInterface/None addribute prevents an empty interface called
    // _AxisRates from being created and used as the [default] interface
    //
    [Guid("c9809d46-a7aa-4876-8631-47222a34707f")]
    [ClassInterface(ClassInterfaceType.None)]
    public class AxisRates : IAxisRates, IEnumerable
    {
        private TelescopeAxes m_Axis;
        private Rate[] m_Rates;

        //
        // Constructor - Internal prevents public creation
        // of instances. Returned by Telescope.AxisRates.
        //
        internal AxisRates(TelescopeAxes Axis)
        {
            m_Axis = Axis;
            //
            // This collection must hold zero or more Rate objects describing the 
            // rates of motion ranges for the Telescope.MoveAxis() method
            // that are supported by your driver. It is OK to leave this 
            // array empty, indicating that MoveAxis() is not supported.
            //
            // Note that we are constructing a rate array for the axis passed
            // to the constructor. Thus we switch() below, and each case should 
            // initialize the array for the rate for the selected axis.
            //

            if (Axis == TelescopeAxes.axisTertiary)
            {
                m_Rates = new Rate[0];
                return;
            }

            // goto slew, centering, and guiding speeds from the mount
            string[] get_rates = { "<140:", "<170:", "<150:" };
            string[] result = null;

            GeminiHardware.Instance.DoCommandResult(get_rates, 3000, false, out result);

            // if didn't get a result or one of the results timed out, throw an error:
            if (result == null) throw new TimeoutException("AxisRates");
            foreach (string s in result)
                if (s == null) throw new TimeoutException("AxisRates");

            switch (Axis)
            {
                case TelescopeAxes.axisPrimary:
                case TelescopeAxes.axisSecondary:
                    m_Rates = new Rate[result.Length];
                    for (int idx = 0; idx < result.Length; ++idx)
                    {
                        double rate = 0;
                        if (!GeminiHardware.Instance.m_Util.StringToDouble(result[idx], out rate))
                            throw new TimeoutException("AxisRates");
                        rate = rate * SharedResources.EARTH_ANG_ROT_DEG_MIN / 60.0;  // convert to rate in deg/sec
                        m_Rates[idx] = new Rate(rate, rate);
                    }

                    // add variable rate from 0 to slew for satellite tracking
                    if (GeminiHardware.Instance.dVersion >= 6 && GeminiHardware.Instance.VariableMoveAxis)
                    {
                        Array.Resize(ref m_Rates, m_Rates.Length + 1);
                        int idx = result.Length;
                        m_Rates[idx] = new Rate(0, m_Rates[0].Maximum); 
                    }

                    break;
            }
        }

#region IAxisRates Members

        public int Count
        {
            get { return m_Rates.Length; }
        }

        public IEnumerator GetEnumerator()
        {
            return m_Rates.GetEnumerator();
        }

        public IRate this[int Index]
        {
            get { return (IRate)m_Rates[Index - 1]; }	// 1-based
        }

#endregion

    }

    //
    // TrackingRates is a strongly-typed collection that must be enumerable by
    // both COM and .NET. The ITrackingRates and IEnumerable interfaces provide
    // this polymorphism. 
    //
    // The Guid attribute sets the CLSID for ASCOM.Telescope.TrackingRates
    // The ClassInterface/None addribute prevents an empty interface called
    // _TrackingRates from being created and used as the [default] interface
    //
    [Guid("e1108c19-e0bb-472e-8bb2-29263f331971")]
    [ClassInterface(ClassInterfaceType.None)]
    public class TrackingRates : ITrackingRates, IEnumerable, IEnumerator
    {
        private DriveRates[] m_TrackingRates;
        private int _pos = -1;

        //
        // Default constructor - Internal prevents public creation
        // of instances. Returned by Telescope.AxisRates.
        //
        internal TrackingRates()
        {
            //
            // This array must hold ONE or more DriveRates values, indicating
            // the tracking rates supported by your telescope. The one value
            // (tracking rate) that MUST be supported is driveSidereal!
            //
            m_TrackingRates = new DriveRates[] { DriveRates.driveSidereal, DriveRates.driveKing, DriveRates.driveLunar, DriveRates.driveSolar };
        }

#region ITrackingRates Members

        public int Count
        {
            get { return m_TrackingRates.Length; }
        }

        public IEnumerator GetEnumerator()
        {
            return this as IEnumerator;
        }


        public DriveRates this[int Index]
        {
            get { return m_TrackingRates[Index - 1]; }	// 1-based
        }
#endregion

#region IEnumerator implementation

        public bool MoveNext()
        {
            if (++_pos >= m_TrackingRates.Length) return false;
            return true;
        }

        public void Reset()
        {
            _pos = -1;
        }

        public object Current
        {
            get
            {
                if (_pos < 0 || _pos >= m_TrackingRates.Length) throw new System.InvalidOperationException();
                return m_TrackingRates[_pos];
            }
        }

#endregion
    }
}
