//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Gemini Telescope Hardware
//
// Description:	This implements a simulated Telescope Hardware
//
// Implements:	ASCOM Telescope interface version: 2.0
// Author:		(rbt) Robert Turner <robert@robertturnerastro.com>
//              (pk) Paul Kanevsky <paul@pk.darkhorizons.org>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------------
// 24-MAY-2011	pk	1.0.19	Initial edit, created for Gemini-2 L5 functionality overrides
// --------------------------------------------------------------------------------------
//

using System;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Timers;
using System.IO.Ports;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;    
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using ASCOM.GeminiTelescope.Properties;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ASCOM.GeminiTelescope
{

    public delegate void SiteChangedDelegate();
    public delegate void TimeChangedDelegate();
    public delegate void MountChangedDelegate();
    public delegate void DisplayChangedDelegate();
    public delegate void ModelChangedDelegate();
    public delegate void SpeedChangedDelegate();


    /// <summary>
    /// Class encapsulating all communications with Gemini L5
    /// </summary>
    public partial class Gemini5Hardware :  GeminiHardwareBase
    {
        // for L5, Gemini returns a set of bytes to indicate internal
        // variable changes. Six revision characters
        // 1: Site,
        // 2: Date/Time,
        // 3: Mount Parameter,
        // 4: Display content,
        // 5: Modelling parameters,
        // 6: Speeds

        internal string m_PreviousUpdateState = "000000";
        internal string m_CurrentUpdateState = "000000";

        public event SiteChangedDelegate OnSiteChanged;
        public event TimeChangedDelegate OnTimeChanged;
        public event MountChangedDelegate OnMountChanged;
        public event DisplayChangedDelegate OnDisplayChanged;
        public event ModelChangedDelegate OnModelChanged;
        public event SpeedChangedDelegate OnSpeedChanged;

    


        public IPEndPoint UDP_endpoint = null;
        public UdpClient UDP_client = null;    

        public Gemini5Hardware() : base()
        {
        }

        internal override void SendStartUpCommands()
        {

            if (GeminiLevel >= 5)
            {

                Trace.Info(2, "Setting DOUBLE PRECISION");
                DoCommandResult(":u", MAX_TIMEOUT, false);
                DoublePrecision = true;

                Trace.Info(2, "Setting extended checksum for L5");
                DoCommandResult(">91:1", MAX_TIMEOUT, false);

                string res = DoCommandResult("<91:", MAX_TIMEOUT, false);
                if (res == "1")
                    m_ChecksumMask = 0xff;


                if (GeminiHardware.Instance.dVersion >= 5.1) //set preferred stop mode for level 5.1 and greater
                {
                    GeminiHardware.Instance.GeminiStopMode = GeminiHardware.Instance.m_GeminiStopMode;
                }           
            } 

            base.SendStartUpCommands();
            //Setting double-precision mode:
        }

        public void ResyncEthernet()
        {
            Trace.Enter(2, "ResyncEthernet");
            if (UDP && this.Connected)
            {
                try
                {
//                    if (UDP_client != null)
                    {
                        DisconnectToEthernet();
//                        UDP_client.Close();
                    }

                    //UDP_client = new UdpClient(UDPPort);
                    for (int i = 0; i < 3; ++i)
                    {
                        try
                        {
                            ConnectToEthernet();
                            System.Threading.Thread.Sleep(500);
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.Error("ResyncEthernet", ex.Message);
                }

            }
        }
            
        public void DisconnectToEthernet()
        {
            Trace.Enter(2, "DisconnectToEthernet");
            if (UDP)
            {

                try
                {
                    UDP_endpoint = null;
                    if (UDP_client != null)
                    {
                        UDP_client.Close();
              
                        UDP_client = null;
                    }
                }
                catch (Exception ex)
                {
                    Trace.Error("DisconnectToEthernet", ex.Message);
                }
            }
            Trace.Exit(2, "DisconnectToEthernet");
        }

        public bool IsEthernetConnected
        {
            get
            {
                return EthernetPort && (!UDP || (UDP_client != null));
            }
        }

        public bool ConnectToEthernet()
        {
            Trace.Enter("ConnectToEthernet");
            if (UDP && UDP_client==null)
            {

                System.Threading.Thread.Sleep(500);
                Trace.Info(2, "UDP connection", GeminiDHCPName, UDPPort);

                IPAddress addr = null;


                if (this.UseDHCP)
                {
                    Trace.Info(2, "Looking up DHCP name", GeminiDHCPName);
                    IPAddress[] addresslist = Dns.GetHostAddresses(this.GeminiDHCPName);
                    if (addresslist == null || addresslist.Length == 0)
                        throw new Exception("Network name not found: " + GeminiDHCPName);
                    addr = addresslist[0];


                }
                else
                {
                    Trace.Info(2, "Using IP address", EthernetIP);
                    System.Net.IPAddress ip;
                    if (!System.Net.IPAddress.TryParse(EthernetIP, out ip))
                        ip = System.Net.IPAddress.Parse("192.168.000.111");
                    addr = ip;
                }

                Trace.Info(2, "UDP IP address", addr.ToString());

                DisconnectToEthernet();

                UDP_endpoint = new IPEndPoint(addr, UDPPort);
                UDP_client = new UdpClient(UDPPort);
                Trace.Info(2, "Initialized UDP endpoint and client");
            }

            Trace.Exit("ConnectToEthernet", true);
            return true;
        }

        /// <summary>
        /// Called when the last command datagram contained no commands
        /// producing a return value. Gemini send an ACK packet when this is the case,
        /// so we check for ACK. If a timeout occurs and 
        /// </summary>
        /// <returns></returns>
        internal override bool GetSyncOnEmptyReturn()
        {
            if (EthernetPort && UDP)
            {
                Trace.Enter("GetSyncOnEmptyReturn");
                string r = getUDPCommandResult(2000);
                if (r == null || r.Length == 0 || r[0] != 0x06) // not an ACK or a timeout
                {
                    Trace.Enter("GetSyncOnEmptyReturn", r, false);
                    Resync();
                    return false;
                }
                Trace.Exit("GetSyncOnEmptyReturn", true);
            }

            return true;
        }


        public override int MaxCommands
        {
            get
            {
                if (GeminiLevel >= 5 && EthernetPort)
                    return 15;
                else
                    return base.MaxCommands;
            }
        }

        internal override void UpdatePolledVariables(bool bUpdateAll)
        {
            if (GeminiLevel >= 5)
            {
                // use macro command 00 to get all the values in one shot if this is Level 5 using UDP:
                if (EthernetPort)
                {
                    if (UDP)
                        __UpdatePolledVariablesUDP();
                    else
                        __UpdatePolledVariablesSerial();
                    return;
                }
            }

            Trace.Enter("UpdatePolledVariables5", bUpdateAll);
            _UpdatePolledVariables();
            //two calls needed to fetch all the variables:
            if (bUpdateAll) _UpdatePolledVariables();
            Trace.Exit("UpdatePolledVariables5", bUpdateAll);
        }


        internal override void _UpdatePolledVariables()
        {
            if (GeminiLevel >= 5)
            {
                if (!CanPoll)
                {
                    Thread.Sleep(SharedResources.GEMINI_POLLING_INTERVAL / 2);
                    return; //don't tie up the serial port while pulse guiding -- timing is critical!
                }

                Trace.Enter("_UpdatePolledVariables5");
                try
                {
                    CommandItem command;
                    int timeout = 3000; // polling should not hold up the queue for too long
                    DiscardInBuffer(); //clear all received data
                    Transmit(CompleteNativeCommand("<97:"));
                    command = new CommandItem("<97:", timeout, true);
                    string change = GetCommandResult(command);
                    if (change != null && change.Length == 6)
                    {
                        m_CurrentUpdateState = change;
                    }
                }
                catch { }

                if (m_CurrentUpdateState != m_PreviousUpdateState) ProcessUpdates();

            }
            base._UpdatePolledVariables();
        }

        static byte [] macro_req = new byte[] { 0x05, 00, 00, 00 };
        public string macro = "";

        private void __UpdatePolledVariablesUDP()
        {

            Trace.Enter("_UpdatePolledVariablesUDP");

            lock (m_SerialPort)
            {
                try
                {
                    DiscardInBuffer(); //clear all received data
                    GeminiHardware.Instance.TransmitUDP(macro_req);
                }
                catch
                {
                    lock (m_CommandQueue)
                    {
                        GeminiHardware.Instance.ResyncEthernet();
                    }
                    TransmitUDP(macro_req);
                }
            }

            macro = getUDPCommandResult(2000);
            if (macro == null)
            {
                Trace.Error("timeout", "UpdatePolledVariablesUDP");
                return;
            }

            Trace.Info(4, "UDP Macro 0 result", macro);
            _UpdatePolledVariablesFromMacro(macro);
            Trace.Exit("_UpdatePolledVariablesUDP");
        }

        private void __UpdatePolledVariablesSerial()
        {

            Trace.Enter("_UpdatePolledVariablesSerial");

            macro = "";

            //lock (m_SerialPort)
            {
                try
                {
                    DiscardInBuffer(); //clear all received data
                    string cmd = CompleteNativeCommand("<81:");
                    Transmit(cmd);
                    CommandItem command = new CommandItem("<81:", 2000, true);
                    macro = GetCommandResult(command);
                }
                catch
                {
                    return;
                }
            }

            if (macro == null)
            {
                Trace.Error("timeout", "UpdatePolledVariablesSerial");
                return;
            }

            Trace.Info(4, "Serial Macro 0 result", macro);
            _UpdatePolledVariablesFromMacro(macro);
            Trace.Exit("_UpdatePolledVariablesSerial");
        }


        
        
        private void _UpdatePolledVariablesFromMacro(string macro)
        {
            string[] res = macro.Split(';');

            Trace.Enter("_UpdatePolledVariablesFromacro", macro);

            /*
                0 PRA 2591823;
                1 PDEC 2592000;
                2 RA 19.467428;
                3 DEC +0.179764;
                4 HA
             
                5 Az 270.204063;
                6 El -28.246181;
                7 Gv N;
                8 GW N;
                9 Gw N;
                10 Gm E;
                11 GS 3.350518;
                12 h? 0;
                13 PEC 0;
                14 Western limit 50963.891895;
                15 <99: 1;
                16 B076D900;
             * 
             */

            try
            {
                //m_PollUpdateCount++;

                //Get RA and DEC etc

                string trc = "";

                string RA = res[2];
                string DEC = res[3];
                string HA = res[4];
                string ALT = res[6];
                string AZ = res[5];
                string V = res[7];
                if (RA != null) m_RightAscension = m_Util.HMSToHours(RA);
                if (DEC != null) m_Declination = m_Util.DMSToDegrees(DEC);
                if (ALT != null) m_Altitude = m_Util.DMSToDegrees(ALT);
                if (AZ != null) m_Azimuth = m_Util.DMSToDegrees(AZ);
                if (V != null) m_Velocity = V;
                if (HA != null) m_HourAngle = m_Util.HMSToHours(HA);

                trc = "RA=" + RA + ", DEC=" + DEC + "ALT=" + ALT + "HA=" + HA + " AZ=" + AZ + " Velocity=" + Velocity;

                string ST = res[11];
                string SOP = res[10];

                string STATUS = res[15];
                string HOME = res[12];

                if (Velocity == "N") m_Tracking = false;
                else
                    m_Tracking = true;


                m_SiderealTime = m_Util.HMSToHours(ST);
                m_SideOfPier = SOP;

                m_ParkState = HOME;

                if ((m_ParkWasExecuted || m_AtPark || m_AtHome) && Velocity != "N") //unparked!
                {
                    m_AtPark = false;
                    m_AtHome = false;
                    LastParkOperation = "";
                    m_ParkWasExecuted = false;
                }

                if (STATUS != null)
                {
                    int.TryParse(STATUS, out m_GeminiStatusByte);

                    // if reached safety limit, send out one notification 
                    if ((m_GeminiStatusByte & 16) != 0 && !m_SafetyNotified)
                    {
                        if (OnSafetyLimit != null) OnSafetyLimit();
                        m_SafetyNotified = true;
                    }
                    else if ((m_GeminiStatusByte & 16) == 0) m_SafetyNotified = false;
                }

                trc += " SOP=" + SOP + " HOME=" + HOME + " Status=" + m_GeminiStatusByte.ToString();

                double TOLIMIT = 0;
                if (m_Util.StringToDouble(res[14], out TOLIMIT))
                    TimeToWestLimit = TOLIMIT;


                byte pec = 0;
                if (byte.TryParse(res[13], out pec))
                    m_PECStatus = pec;

                string change = res[16];
                if (change != null && change.Length == 8)
                {
                    m_CurrentUpdateState = change;
                }



                if (m_CurrentUpdateState != m_PreviousUpdateState) ProcessUpdates();

                Trace.Info(4, trc);
                m_LastUpdate = System.DateTime.Now;
            }
            catch (Exception e)
            {
                Trace.Except(e);
                m_SerialPort.DiscardOutBuffer();
                DiscardInBuffer();
            }

            Trace.Exit("_UpdatePolledVariablesFromMacro");
        }




        /// <summary>
        /// Fire update delegates if Gemini5 returned a change in status flag
        /// </summary>
        private void ProcessUpdates()
        {
            if (m_CurrentUpdateState[0] != m_PreviousUpdateState[0] && OnSiteChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnSiteChanged()));
            if (m_CurrentUpdateState[1] != m_PreviousUpdateState[1] && OnTimeChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnTimeChanged()));
            if (m_CurrentUpdateState[2] != m_PreviousUpdateState[2] && OnMountChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnMountChanged()));
            if (m_CurrentUpdateState[3] != m_PreviousUpdateState[3] && OnDisplayChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnDisplayChanged()));
            if (m_CurrentUpdateState[4] != m_PreviousUpdateState[4] && OnModelChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnDisplayChanged()));
            if (m_CurrentUpdateState[5] != m_PreviousUpdateState[5] && OnSpeedChanged != null)
                ThreadPool.QueueUserWorkItem(new WaitCallback(a => OnSpeedChanged()));
            m_PreviousUpdateState = m_CurrentUpdateState;
        }


        private double m_TimeToWestLimit = double.NaN;

        public override double TimeToWestLimit
        {
            get
            {
                if (EthernetPort && UDP) // this is part of a polled macro command when using UDP
                {
                    if (m_TimeToWestLimit == double.NaN) throw new TimeoutException();
                    return m_TimeToWestLimit;
                }
                else return base.TimeToWestLimit;
            }
            set
            {
                m_TimeToWestLimit = value;
            }
        }

        private byte m_PECStatus = 0;

        /// <summary>
        /// Set/Get PEC Status byte from Gemini
        ///  PEC status. Decimal
        ///     1: PEC active,
        ///     2: freshly trained (not yet altered) PEC data are available as current PEC data,
        ///     4: PEC training in progress,
        ///     8: PEC training was just completed,
        ///     16: PEC training will start soon,
        ///   0xff: failed to get status
        /// </summary>
        public override byte PECStatus
        {
            get
            {
                if (EthernetPort && UDP) // this is part of a polled macro command when using UDP
                    return m_PECStatus;
                else
                    return base.PECStatus;
            }
            set
            {
                DoCommand(">509:" + value.ToString(), false);
            }
        }

        public void SetHomePosition()
        {
            Trace.Enter(2, "SetHomePosition");

            if (Connected)
               DoCommand(":hH#", false);

            Trace.Exit(2, "SetHomePosition");

        }

        private double m_HourAngle = 0;

        public override double HourAngle
        {
            get
            {
                if (EthernetPort && UDP) // this is part of a polled macro command when using UDP
                    return m_HourAngle;
                else
                    return base.HourAngle;
            }

            set {
                m_HourAngle = value;
            }
        }

    }
}