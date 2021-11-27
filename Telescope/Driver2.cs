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


namespace ASCOM.GeminiTelescope2
{
    //
    // Your driver's ID is ASCOM.Telescope.Telescope
    //
    // The Guid attribute sets the CLSID for ASCOM.Telescope.Telescope
    // The ClassInterface/None addribute prevents an empty interface called
    // _Telescope from being created and used as the [default] interface
    //


    [ASCOM.DeviceId("ASCOM.GeminiTelescope.Telescope2", DeviceName = "ASCOM.GeminiTelescope.Telescope2")]
    [ASCOM.ServedClassNameAttribute("Gemini Telescope .NET 2")]
    [Guid("19d618b3-329a-44e3-aa91-9ce6e148eb48")]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class Telescope : ASCOM.GeminiTelescope.Telescope1
    {
    }


}

namespace ASCOM.GeminiTelescope
{
    public partial class Telescope1 : ReferenceCountedObjectBase, IGeminiTelescope
    {
    }
}