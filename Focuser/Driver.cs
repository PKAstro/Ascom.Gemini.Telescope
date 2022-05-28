//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Focuser driver for Gemini
//
// Description:	Gemini controlled focuser
//
// Implements:	ASCOM Focuser interface version: 1.0
// Author:		(rbt) Robert Turner <robert@robertturnerastro.com>
//              (pk)  Paul Kanevsky <pk.darkhorizons.org>
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 15-JUL-2009	rbt	1.0.0	Initial edit, from ASCOM Focuser Driver template
// 30-JUL-2009  pk  1.0.1   Basic driver and setup implementation
// --------------------------------------------------------------------------------
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Utilities;
using ASCOM.Interface;
using ASCOM.GeminiTelescope;
using System.Configuration;

namespace ASCOM.GeminiTelescope
{    
    //
    // Your driver's ID is ASCOM.Focuser.Focuser
    //
    // The Guid attribute sets the CLSID for ASCOM.Focuser.Focuser
    // The ClassInterface/None addribute prevents an empty interface called
    // _Focuser from being created and used as the [default] interface
    //


    [ASCOM.DeviceId("ASCOM.GeminiTelescope.Focuser", DeviceName = "ASCOM.GeminiTelescope.Focuser")]
    [ASCOM.ServedClassNameAttribute("Gemini Focuser .NET")]

    [Guid("3a22c443-4e46-4504-8cef-731095e51e1f")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Focuser : ReferenceCountedObjectBase, ASCOM.Interface.IFocuser
    {

        bool m_Connected = false;
        private ASCOM.Utilities.Util m_Util;

        

        int m_PreviousMove = 0;

        

        int m_Position = 0;

        System.Timers.Timer tmrFocus = new System.Timers.Timer();
        
        enum FocuserState {
            Backlash,
            Focusing,
            Braking,
            None
        };
        
        FocuserState m_State = FocuserState.None;

        //
        // Constructor - Must be public for COM registration!
        //
        public Focuser()
        {
            m_Util = new ASCOM.Utilities.Util();
            tmrFocus.AutoReset = true;
            tmrFocus.Elapsed += new System.Timers.ElapsedEventHandler(tmrFocus_Elapsed);
                // Changed to work with new timer interface
        }

        /// <summary>
        /// Executed on a timer when waiting to complete a focuser move
        /// </summary>
        void tmrFocus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrFocus.Stop();
            int val = m_PreviousMove;

            GeminiHardware.Instance.Trace.Enter("Focuser:Timer", m_State.ToString(), m_PreviousMove, m_Position);

            // if we were taking up backlash prior to this,
            // move on to actual focusing command:
            if (m_State == FocuserState.Backlash)
            {
                m_State = FocuserState.Focusing;

                if ((val > 0 && !GeminiHardware.Instance.ReverseDirection) || (val < 0 && GeminiHardware.Instance.ReverseDirection))
                    GeminiHardware.Instance.DoCommand(":F+", false);
                else
                    GeminiHardware.Instance.DoCommand(":F-", false);
                tmrFocus.Interval = (GeminiHardware.Instance.StepSize * Math.Abs(val));
                tmrFocus.Start();
                GeminiHardware.Instance.Trace.Exit("Focuser:Timer", m_State.ToString(), m_PreviousMove, m_Position);
                return;
            }
            // if we are done with focusing, check if braking is enabled
            // and execute a break maneuver (move in the opposite direction for a bit)
            else if (m_State == FocuserState.Focusing)
            {
                if (GeminiHardware.Instance.BrakeSize > 0)
                {
                    // move in the opposite direction for specified step*interval
                    if ((val > 0 && !GeminiHardware.Instance.ReverseDirection) || (val < 0 && GeminiHardware.Instance.ReverseDirection))
                        GeminiHardware.Instance.DoCommand(":F-", false);
                    else
                        GeminiHardware.Instance.DoCommand(":F+", false);

                    tmrFocus.Interval = (GeminiHardware.Instance.StepSize * GeminiHardware.Instance.BrakeSize);
                    tmrFocus.Start();
                    GeminiHardware.Instance.Trace.Exit("Focuser:Timer", m_State.ToString(), m_PreviousMove, m_Position);
                    return;
                }
            }

            // at this point, we're done focusing!
            Halt();
            m_Position += m_PreviousMove * GeminiHardware.Instance.StepSize;  // new position 
            GeminiHardware.Instance.Trace.Exit("Foc:Timer", m_State.ToString(), m_PreviousMove, m_Position);
        }


        //
        // PUBLIC COM INTERFACE IFocuser IMPLEMENTATION
        //

        #region IFocuser Members

        /// <summary>
        /// Gemini doesn't support absolute focusers, but we'll fake it by keeping track of current position
        /// based on executed commands
        /// </summary>
        public bool Absolute
        {
            get {
                GeminiHardware.Instance.Trace.Enter("IF:Absolute.Get", GeminiHardware.Instance.AbsoluteFocuser);                
                return GeminiHardware.Instance.AbsoluteFocuser; }
        }

        /// <summary>
        /// Stop focusing
        /// </summary>
        public void Halt()
        {
            GeminiHardware.Instance.Trace.Enter("Foc:Halt", m_State.ToString());
            tmrFocus.Stop();
            m_State = FocuserState.None;

            GeminiHardware.Instance.DoCommand(":FQ", false);
        }

        /// <summary>
        /// Is the focuser moving?
        /// </summary>
        public bool IsMoving
        {
            get {
                GeminiHardware.Instance.Trace.Enter("IF:IsMoving", (m_Connected && m_State!=FocuserState.None).ToString());
                
                return m_Connected && m_State!=FocuserState.None;  }
        }

        /// <summary>
        /// Get/Set connection property
        /// </summary>
        public bool Link
        {
            get { 
                return GeminiHardware.Instance.Connected && m_Connected;  
            }
            set {
                if (value && !m_Connected) 
                {
                    GeminiHardware.Instance.Trace.Enter("IF:Link", value);
                    GeminiHardware.Instance.Connected = true;
                    if (!GeminiHardware.Instance.Connected)
                        throw new InvalidOperationException("Cannot connect to Gemini Focuser");
                    else
                    {
                        m_State  = FocuserState.None;

                        // set the desired focuser speed:
                        string sCmd = ":FS";
                        if (GeminiHardware.Instance.Speed == 2) sCmd = ":FM";
                        else
                            if (GeminiHardware.Instance.Speed == 3) sCmd = ":FF";
                        GeminiHardware.Instance.DoCommand(sCmd, false);

                        m_Connected = true;
                    }
                }
                else
                    if (m_Connected) {
                        tmrFocus.Stop();
                        GeminiHardware.Instance.Connected = false;
                        m_Connected = false;
                        m_State = FocuserState.None;
                    }
                GeminiHardware.Instance.Trace.Exit("IF:Link", m_Connected);
            }
        }

        

        /// <summary>
        /// Maximum focuser increment
        /// </summary>
        public int MaxIncrement
        {
            get {
                GeminiHardware.Instance.Trace.Enter("IF:MaxIncrement", GeminiHardware.Instance.MaxIncrement.ToString());                
                return GeminiHardware.Instance.MaxIncrement; }
        }

        /// <summary>
        /// Maximum focuser step
        /// </summary>
        public int MaxStep
        {
            get {
                GeminiHardware.Instance.Trace.Enter("IF:MaxStep", GeminiHardware.Instance.MaxIncrement.ToString());                                
                return GeminiHardware.Instance.MaxStep; }
        }

        /// <summary>
        /// Move focuser by a number of steps
        /// </summary>
        /// <param name="val">val is the number of steps, + or -, + means out, - means in</param>        
        public void Move(int val)
        {

            GeminiHardware.Instance.Trace.Enter("IF:Move", val.ToString());                

            if (m_State != FocuserState.None) Halt();


            if (GeminiHardware.Instance.AbsoluteFocuser)
                val = val * GeminiHardware.Instance.StepSize - m_Position; // how far to move from current position
            else
                val = val * GeminiHardware.Instance.StepSize;

            val /= GeminiHardware.Instance.StepSize;



            // limit the move to max increment setting
            if (Math.Abs(val) > GeminiHardware.Instance.MaxIncrement)
                val = GeminiHardware.Instance.MaxIncrement * Math.Sign(val);

            GeminiHardware.Instance.Trace.Info(3, "Move Value", val.ToString());

            if (val == 0)
            {
                GeminiHardware.Instance.Trace.Exit("IF:Move", tmrFocus.Enabled);
                return;
            }

            if (GeminiHardware.Instance.BacklashDirection != 0 && Math.Sign(GeminiHardware.Instance.BacklashDirection) == Math.Sign(val))
            {
                m_State = FocuserState.Backlash;
                m_PreviousMove = val;

                if ((val > 0 && !GeminiHardware.Instance.ReverseDirection) || (val < 0 && GeminiHardware.Instance.ReverseDirection))
                    GeminiHardware.Instance.DoCommand(":F+",false);
                else
                    GeminiHardware.Instance.DoCommand(":F-", false);

                tmrFocus.Interval = (GeminiHardware.Instance.StepSize * GeminiHardware.Instance.BacklashSize);
                GeminiHardware.Instance.Trace.Info(3, "Setting Backlash", tmrFocus.Interval.ToString());

                tmrFocus.Start();
            }
            else
            {
                m_State = FocuserState.Focusing;
                m_PreviousMove = val;
                if ((val > 0 && !GeminiHardware.Instance.ReverseDirection) || (val < 0 && GeminiHardware.Instance.ReverseDirection))
                    GeminiHardware.Instance.DoCommand(":F+", false);
                else
                    GeminiHardware.Instance.DoCommand(":F-", false);

                tmrFocus.Interval = (GeminiHardware.Instance.StepSize * Math.Abs(val));
                GeminiHardware.Instance.Trace.Info(3, "Focuser", tmrFocus.Interval.ToString());
                tmrFocus.Start();

            }
            GeminiHardware.Instance.Trace.Exit("IF:Move", tmrFocus.Enabled);                 
        }

        /// <summary>
        /// Current focuser position
        /// </summary>
        public int Position
        {
            get {


                if (GeminiHardware.Instance.AbsoluteFocuser)
                {
                    GeminiHardware.Instance.Trace.Enter("IF:Position.Get", m_Position / GeminiHardware.Instance.StepSize);                
                    return m_Position / GeminiHardware.Instance.StepSize;
                }
                else
                    throw new ASCOM.PropertyNotImplementedException("Position", false);
            }
        }

        /// <summary>
        /// Bring up setup dialog
        /// </summary>
        public void SetupDialog()
        {
            //if (GeminiHardware.Instance.Connected)
            //{
            //    throw new DriverException("The hardware is connected, cannot do SetupDialog()",
            //                        unchecked(ErrorCodes.DriverBase + 4));
            //}
            GeminiHardware.Instance.Trace.Enter("IF:SetupDialog");                
            GeminiTelescope.m_MainForm.DoFocuserSetupDialog(); ;
            GeminiHardware.Instance.Trace.Exit("IF:SetupDialog");
        }

        /// <summary>
        /// step size for the focuser, in micro-seconds
        /// </summary>
        public double StepSize
        {
            get {
                GeminiHardware.Instance.Trace.Enter("IF:StepSize.Get", GeminiHardware.Instance.StepSize);                                
                return GeminiHardware.Instance.StepSize;  }
        }

        /// <summary>
        /// Temp compensation is not supported by Gemini
        /// </summary>
        public bool TempComp
        {
            get { return false; }
            set { throw new PropertyNotImplementedException("TempComp", false); }
        }

        /// <summary>
        /// Temp compensation is not available
        /// </summary>
        public bool TempCompAvailable
        {
            get { return false;  }
        }

        /// <summary>
        /// Not supported by Gemini
        /// </summary>
        public double Temperature
        {
            get { throw new PropertyNotImplementedException("Temperature", false); }
        }

        #endregion
    }


}
