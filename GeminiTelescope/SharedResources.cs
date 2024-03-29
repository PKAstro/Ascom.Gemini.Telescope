//
// ================
// Shared Resources
// ================
//
// This class is a container for all shared resources that may be needed
// by the drivers served by the Local Server. 
//
// NOTES:
//
//	* ALL DECLARATIONS MUST BE STATIC HERE!! INSTANCES OF THIS CLASS MUST NEVER BE CREATED!
//
// Written by:	Bob Denny	29-May-2007
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;

namespace ASCOM.GeminiTelescope
{
    public class SharedResources
    {
        
        private static int s_z;

        //Constant Definitions
        public static string TELESCOPE_PROGRAM_ID = "ASCOM.GeminiTelescope.Telescope";  //Key used to store the settings
        public static string FOCUSER_PROGRAM_ID = "ASCOM.GeminiTelescope.Focuser";  //Key used to store the settings
        public static string REGISTRATION_VERSION = "1";
        public static int GEMINI_INSTANCE_NUMBER = 1;

        public static int GEMINI_POLLING_INTERVAL = 1000;             //Seconds to use for Polling Gemini status

        public static string TELESCOPE_DRIVER_DESCRIPTION = "Gemini Telescope ASCOM Driver .NET";
        public static string TELESCOPE_DRIVER_NAME = "Gemini Telescope .NET";
        public static string TELESCOPE_DRIVER_INFO = "Gemini Telescope Driver";

        public static string FOCUSER_DRIVER_DESCRIPTION = "Gemini Focuser ASCOM Driver .NET";
        public static string FOCUSER_DRIVER_NAME = "Gemini Focuser .NET";
        public static string FOCUSER_DRIVER_INFO = "Gemini Focuser Driver V1";

        public static uint ERROR_BASE = 0x80040400;
        public static uint SCODE_NO_TARGET_COORDS = ERROR_BASE + 0x404;
        public static string MSG_NO_TARGET_COORDS = "Target coordinates have not yet been set";
        public static uint SCODE_VAL_OUTOFRANGE = ERROR_BASE + 0x405;
        public static string MSG_VAL_OUTOFRANGE = "The property value is out of range";
        public static uint SCODE_PROP_NOT_SET = ERROR_BASE + 0x403;
        public static string MSG_PROP_NOT_SET = "The property has not yet been set";
        public static uint INVALID_AT_PARK = ERROR_BASE + 0x404;
        public static string MSG_INVALID_AT_PARK = "Invalid while parked";

        public static uint INVALID_WHILE_SLEWING = ERROR_BASE + 0x408;
        public static string MSG_INVALID_WHILE_SLEWING = "Guide commands are invalid while slewing";

        public static string MSG_GEMINI_VERSION = "Gemini level 4 or greater is required";
        public static uint SCODE_GEMINI_VERSION = ERROR_BASE + 0x407;

        public static uint SCODE_NOT_IMPLEMENTED = ERROR_BASE + 0x400;
        public static string MSG_NOT_IMPLEMENTED = "Not implemented";

        public static uint SCODE_INVALID_VALUE = ERROR_BASE + 0x401;
        public static string MSG_INVALID_VALUE = "Invalid value";

        public static uint SCODE_NOT_CONNECTED = ERROR_BASE + 0x406;
        public static string MSG_NOT_CONNECTED = "Gemini is not responding. Is it connected?";

        public static uint SCODE_TIME_NOTSET = ERROR_BASE + 0x407;
        public static string MSG_TIME_NOTSET = "Failed to set Gemini time";

        public static string DEAULT_PROFILE = "GeminiDefaultProfile.gp";

        //Astronomy Releated Constants
        public static double DEG_RAD = Math.PI / 180;
        public static double RAD_DEG = 57.2957795;
        public static double HRS_RAD = 0.2617993881;
        public static double RAD_HRS = 3.81971863;
        public static double EARTH_ANG_ROT_DEG_MIN = 0.25068447733746215; //Angular rotation of earth in degrees/min

        public static double INVALID_DOUBLE = -33333333;    // invalid number, or value not set

        public static int MAXIMUM_ERRORS = 5   ;      // max communication errors tolerated within specified interval
        public static int MAXIMUM_ERROR_INTERVAL = 20000;   // in msecs. 
        public static int RECOVER_SLEEP = 3000;         // how long to sleep while waiting to recover from errors

        // slew of 180 degrees at 400x sidereal will take 108 seconds
        // since Gemini is often unresponsive during a slew, 110 seconds is the longest
        // interval we'll wait for a response before giving up and disconnecting:
        public static int MAXIMUM_DISCONNECT_TIME = 110; // (seconds)

        public static int PULSEGUIDE_POLLING_INTERVAL = 20000; // how often to refresh mount parameters when doing pulse-guiding

        public static int DAYS_TO_KEEP_LOGS = 10;     // how many days to keep logs: older logs will be deleted on start-up

        private SharedResources() { }							// Prevent creation of instances

        static SharedResources()								// Static initialization
        {
            
            s_z = 0;
        }

        //
        // Public access to shared resources
        //

        // Shared serial port 
        public static int z { get { return s_z++; } }



        /// <summary>
        /// bring window on top of others in the z-order, without making it top-most.
        /// </summary>
        /// <param name="window"></param>
        public static void SetTopWindow(System.Windows.Forms.Control window)
        {

            Win32API.SetWindowPos(window.Handle, Win32API.HWND_TOPMOST, 0, 0, 0, 0, Win32API.SWP_NOMOVE | Win32API.SWP_NOSIZE);
            Win32API.SetWindowPos(window.Handle, Win32API.HWND_NOTTOPMOST, 0, 0, 0, 0, Win32API.SWP_NOMOVE | Win32API.SWP_NOSIZE);
            Win32API.SetForegroundWindow(window.Handle);
        }

        public static IEnumerable<Control> GetAll(Control control)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl))
                                      .Concat(controls);
                                      
        }

        public static void SetInstance(System.Windows.Forms.Form f)
        {
            
            if (GEMINI_INSTANCE_NUMBER == 2)
            {
                f.Text += $" #{GEMINI_INSTANCE_NUMBER}";
                f.BackColor = System.Drawing.Color.FromArgb(0, 0, 96);
                foreach(System.Windows.Forms.Control c in GetAll(f as Control))
                {
                    if (c.BackColor == System.Drawing.Color.Black)
                        c.BackColor = f.BackColor;
                }
            }
        }
    }
}
