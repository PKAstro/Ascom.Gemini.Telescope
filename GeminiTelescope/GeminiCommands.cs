﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.GeminiTelescope
{

    /// <summary>
    /// A single command description for the table of exception commands
    /// </summary>
  
    public delegate string CustomCommand(string cmd, int timeout);

    internal class GeminiCommand
    {
        /// <summary>
        /// What type of a return this command expects
        /// </summary>
        internal enum ResultType 
        {
            NoResult,       // no result expected
            HashChar,       // hash-terminated ('#') string
            ZeroOrHash,     // character '0' (zero) or a hash ('#') terminated string
            OneOrHash,      // character '1' (one) or a hash ('#') terminated string
            NumberofChars,   // specific number of characters
            ZeroOrTwoHash,   // :SC command
            Custom           // custom implementation of a command
        }

        internal GeminiCommand(ResultType type, int chars) : this(type, chars, false)
        {
        }

        internal GeminiCommand(ResultType type, int chars, bool bUpdateStatus)
        {
            Type = type;
            Chars = chars;
            UpdateStatus = bUpdateStatus;
        }

        internal GeminiCommand(ResultType type, int chars, bool bUpdateStatus, CustomCommand cmdDelegate) : 
          this(type, chars, bUpdateStatus)
        {
            CustomDelegate = cmdDelegate;
        }

        public ResultType Type; // expected return type
        public int Chars;   // expected number of characters if Type=NumberofChars
        public bool UpdateStatus; // this command changes mount status, and an update to polled variables should follow immediately

        public CustomCommand CustomDelegate;
    }

    /// <summary>
    /// Class contains a dictionary of all the commands with "exceptional" return values
    ///   each Gemini command that expects a non-standard return (for example, one not terminated by '#' character)
    ///   is listed here, along with the type of return required.
    ///   
    /// </summary>
    static class GeminiCommands
    {
        /// <summary>
        /// Dictionary of all the supported Geimini commands and their expected result types
        ///   anything not in this table will either be expected not to return a value, or
        ///   return a '#' terminated string (by default)
        ///   
        /// </summary>
        public static Dictionary<string, GeminiCommand> Commands = new Dictionary<string, GeminiCommand>();
        static GeminiCommands()
        {
            Commands.Add("\x6", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":Cm", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":CM", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            
            Commands.Add(":F+", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 
            Commands.Add(":F-", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 
            Commands.Add(":FQ", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 
            Commands.Add(":FF", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 
            Commands.Add(":FM", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 
            Commands.Add(":FS", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0)); 

            Commands.Add(":GA", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GB", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GC", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":Gc", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GD", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GE", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GG", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":Gg", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GH", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0)); 
            Commands.Add(":GL", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));

            Commands.Add(":Gm", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GM", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GN", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GO", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GP", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GR", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GS", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":Gt", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GV", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GVD", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));

            Commands.Add(":GVN", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GVP", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":GVT", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));

            Commands.Add(":Gv", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1)); //N/T/G/C/S/! are possible return values
            Commands.Add(":GZ", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":hP", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":hC", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":hZ", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":hN", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":hW", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":h?", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1)); //0/1/2

            Commands.Add(":MA", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 0, true)); //0 or # terminated string

            Commands.Add(":MF", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0, true));
            Commands.Add(":ML", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":Ml", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":Mf", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 0, true));
            Commands.Add(":MM", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 0, true));
            Commands.Add(":MS", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 0, true));
            Commands.Add(":Me", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Mw", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Mn", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Ms", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));

            Commands.Add(":mi", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":mm", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":Ma", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":Mi", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":Mg", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":OC", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":OI", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":ON", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":OR", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":OS", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":Oc", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":Od", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":On", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":Or", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(":Os", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":p0", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":p1", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":p2", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":p3", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));

            Commands.Add(":P", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 14)); // "LOW  PRECISION" or "HIGH PRECISION"

            Commands.Add(":U", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":Q", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Qe", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Qw", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Qn", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));
            Commands.Add(":Qs", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0, true));

            Commands.Add(":RC", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":RG", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":RM", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":RS", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":Sa", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));                        
  
            Commands.Add(":SB", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            
            // :SC is special case, returns '0' if invalid, or
            // two strings '1Updating planetary data#' followed by '<24 blanks>#'
            Commands.Add(":SC", new GeminiCommand(GeminiCommand.ResultType.ZeroOrTwoHash, 0, true));
            Commands.Add(":SE", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":SG", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1, true));
            Commands.Add(":SL", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1, true));
            Commands.Add(":Sd", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":SM", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":SN", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":SO", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":SP", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1, true));
            Commands.Add(":Sg", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":Sp", new GeminiCommand(GeminiCommand.ResultType.OneOrHash, 1));
            Commands.Add(":Sr", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":St", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1, true));
            Commands.Add(":Sw", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":Sz", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));

            Commands.Add(":W", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            // enter/exit second hand controller mode
            Commands.Add(":HC1", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":HC0", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            // commands to press various buttons and button combinations on the hand controller, :k@ means nothing's pressed
            Commands.Add(":kD", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":kA", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":kH", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":kB", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":k@", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":kP", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            Commands.Add(":kI", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));

            Commands.Add(":HI", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            
            // request controller button status: returns @# if nothing's pressed
            Commands.Add(":T", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));

            // enter, exit menu mode
            Commands.Add(":HM", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            Commands.Add(":Hm", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            
            // commands >181: and >182: are exceptions -- they set value, and return value
            // documentation is wrong: >181: returns '0#' and >182: returns '1'
            // >509: returns just a '#'
            Commands.Add(">181:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">182:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">509:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">511:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">221:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">222:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">411:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add(">412:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
            Commands.Add("<226:", new GeminiCommand(GeminiCommand.ResultType.Custom, 0, false, new CustomCommand(GeminiHardware.Instance.TimeToLimitL4)));

        }

        /// <summary>
        /// this adds commands present in Gemini II (Level 5)
        /// This is only called if L5 is detected
        /// </summary>
        public static void GeminiCommandsL5()
        {
            // if we didn't add these L5 commands yet, do this now
            if (!Commands.ContainsKey(":OO"))
            {
                //Commands.Add("<97:", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 6));
                Commands.Remove("<226:");    //delete custom implementation used for L4 mounts, this is implemented in firmware in L5
                Commands.Add(":GW", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
                Commands.Add(":Gw", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
                Commands.Add(":Gu", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 2));
                Commands.Add(":GI", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
                Commands.Add(":OO", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
                Commands.Add(":Oo", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));

                // L5 fixes the return values for these, so need to remove them from the list of special cases:
                Commands.Remove(">181:");
                Commands.Remove(">182:");
                Commands.Remove(">509:");
                Commands.Remove(">511:");
                Commands.Remove(">221:");
                Commands.Remove(">222:");
                Commands.Remove(">411:");
                Commands.Remove(">412:");
                Commands.Add(":u", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
                Commands.Add(":W?", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
                Commands.Add(":S0", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
                Commands.Add("<91:", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
                Commands.Add("<551", new GeminiCommand(GeminiCommand.ResultType.NoResult, 0));
            }
        }

        /// <summary>
        /// this adds commands present in Gemini II (Level 6)
        /// This is only called if L6 is detected
        /// </summary>
        public static void GeminiCommandsL6()
        {
            // if we didn't add these L6 commands yet, do this now
            if (!Commands.ContainsKey(":Mp"))
            {
                Commands.Add(":MP", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 0));
                Commands.Add(":Mp", new GeminiCommand(GeminiCommand.ResultType.ZeroOrHash, 1));
                Commands.Add(":Gl#", new GeminiCommand(GeminiCommand.ResultType.HashChar, 0));
                Commands.Add(":Sl", new GeminiCommand(GeminiCommand.ResultType.NumberofChars, 1));
            }
        }
    }
}
