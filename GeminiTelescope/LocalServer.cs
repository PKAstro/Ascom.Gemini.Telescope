//
// GeminiTelescope Local COM Server
//
// This is the core of a managed COM Local Server, capable of serving
// multiple instances of multiple interfdaces, within a single
// executable. This implementes the equivalent functionality of VB6
// which has been extensively used in ASCOM for drivers that provide
// multiple interfaces to multiple clients (e.g. Meade Telescope
// and Focuser) as well as hubs (e.g., POTH).
//
// Written by: Robert B. Denny (Version 1.0.1, 29-May-2007)
//
//
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using ASCOM.Utilities;
using Microsoft.Win32;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Diagnostics;
using ASCOM;

namespace ASCOM.GeminiTelescope
{
    public class GeminiTelescope
    {

        #region Access to kernel32.dll, user32.dll, and ole32.dll functions
        [Flags]
        enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        [Flags]
        enum COINIT : uint
        {
            /// Initializes the thread for multi-threaded object concurrency.
            COINIT_MULTITHREADED = 0x0,
            /// Initializes the thread for apartment-threaded object concurrency. 
            COINIT_APARTMENTTHREADED = 0x2,
            /// Disables DDE for Ole1 support.
            COINIT_DISABLE_OLE1DDE = 0x4,
            /// Trades memory for speed.
            COINIT_SPEED_OVER_MEMORY = 0x8
        }

        [Flags]
        enum REGCLS : uint
        {
            REGCLS_SINGLEUSE = 0,
            REGCLS_MULTIPLEUSE = 1,
            REGCLS_MULTI_SEPARATE = 2,
            REGCLS_SUSPENDED = 4,
            REGCLS_SURROGATE = 8
        }


        // CoInitializeEx() can be used to set the apartment model
        // of individual threads.
        [DllImport("ole32.dll")]
        static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

        // CoUninitialize() is used to uninitialize a COM thread.
        [DllImport("ole32.dll")]
        static extern void CoUninitialize();

        // PostThreadMessage() allows us to post a Windows Message to
        // a specific thread (identified by its thread id).
        // We will need this API to post a WM_QUIT message to the main 
        // thread in order to terminate this application.
        [DllImport("user32.dll")]
        static extern bool PostThreadMessage(uint idThread, uint Msg, UIntPtr wParam,
            IntPtr lParam);

        // GetCurrentThreadId() allows us to obtain the thread id of the
        // calling thread. This allows us to post the WM_QUIT message to
        // the main thread.
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        #endregion

        #region Private Data
        private static uint m_uiMainThreadId;					// Stores the main thread's thread id.
        private static int m_iObjsInUse;						// Keeps a count on the total number of objects alive.
        private static int m_iServerLocks;						// Keeps a lock count on this application.
        public static bool m_bComStart;						// True if server started by COM (-embedding)
        public static frmMain m_MainForm = null;				// Reference to our main form
        private static ArrayList m_ComObjectAssys;				// Dynamically loaded assemblies containing served COM objects
        private static ArrayList m_ComObjectTypes;				// Served COM object types
        private static ArrayList m_ClassFactories;				// Served COM object class factories
        private static string s_appId = "{fbe30603-14ea-45ed-96d5-98c5e00f9c1c}";	// Our AppId
        private static bool m_UnregisterASCOM = false;

        #endregion

        // This property returns the main thread's id.
        public static uint MainThreadId { get { return m_uiMainThreadId; } }

        // Used to tell if started by COM or manually
        public static bool StartedByCOM { get { return m_bComStart; } }


        #region Server Lock, Object Counting, and AutoQuit on COM startup
        // Returns the total number of objects alive currently.
        public static int ObjectsCount
        {
            get
            {
                lock (typeof(GeminiTelescope))
                {
                    return m_iObjsInUse;
                }
            }
        }

        // This method performs a thread-safe incrementation of the objects count.
        public static int CountObject()
        {
            // Increment the global count of objects.
            return Interlocked.Increment(ref m_iObjsInUse);
        }

        // This method performs a thread-safe decrementation the objects count.
        public static int UncountObject()
        {
            // Decrement the global count of objects.
            return Interlocked.Decrement(ref m_iObjsInUse);
        }

        // Returns the current server lock count.
        public static int ServerLockCount
        {
            get
            {
                lock (typeof(GeminiTelescope))
                {
                    return m_iServerLocks;
                }
            }
        }

        // This method performs a thread-safe incrementation the 
        // server lock count.
        public static int CountLock()
        {
            // Increment the global lock count of this server.
            return Interlocked.Increment(ref m_iServerLocks);
        }

        // This method performs a thread-safe decrementation the 
        // server lock count.
        public static int UncountLock()
        {
            // Decrement the global lock count of this server.
            return Interlocked.Decrement(ref m_iServerLocks);
        }

        // AttemptToTerminateServer() will check to see if the objects count and the server 
        // lock count have both dropped to zero.
        //
        // If so, and if we were started by COM, we post a WM_QUIT message to the main thread's
        // message loop. This will cause the message loop to exit and hence the termination 
        // of this application. If hand-started, then just trace that it WOULD exit now.
        //
        public static void ExitIf()
        {
            lock (typeof(GeminiTelescope))
            {
                if ((ObjectsCount <= 0) && (ServerLockCount <= 0))
                {
                    if (m_bComStart)
                    {
                        UIntPtr wParam = new UIntPtr(0);
                        IntPtr lParam = new IntPtr(0);
                        PostThreadMessage(MainThreadId, 0x0012, wParam, lParam);
                    }
                }
            }
        }
        #endregion

        // -----------------
        // PRIVATE FUNCTIONS
        // -----------------

        #region Dynamic Driver Assembly Loader
        //
        // Load the assemblies that contain the classes that we will serve
        // via COM. These will be located in the subfolder ServedClasses
        // below our executable. The code below takes care of the situation
        // where we're running in the VS.NET IDE, allowing the ServedClasses
        // folder to be in the solution folder, while we are executing in
        // the GeminiTelescope\bin\Debug subfolder.
        //
        private static bool LoadComObjectAssemblies(bool all_instances = false)
        {
            m_ComObjectAssys = new ArrayList();
            m_ComObjectTypes = new ArrayList();

            string assyPath = Assembly.GetEntryAssembly().Location;
            int i = assyPath.LastIndexOf(@"\GeminiTelescope\bin\");						// Look for us running in IDE
            if (i == -1) i = assyPath.LastIndexOf('\\');
            assyPath = assyPath.Remove(i, assyPath.Length - i) + "\\GeminiTelescopeServedClasses";

            DirectoryInfo d = new DirectoryInfo(assyPath);
            foreach (FileInfo fi in d.GetFiles("*.dll"))
            {
                string aPath = fi.FullName;
                string fqClassName = fi.Name.Replace(fi.Extension, "");						// COM class FQN
                //
                // First try to load the assembly and get the types for
                // the class and the class facctory. If this doesn't work ????
                //
                try
                {
                    Assembly so = Assembly.LoadFrom(aPath);
                    //PWGS Get the types in the assembly
                    Type[] types = so.GetTypes();
                    foreach (Type type in types)
                    {
                        // PWGS Now checks the type rather than the assembly
                        // Check to see if the type has the ServedClassName attribute, only use it if it does.
                        MemberInfo info = type;

                        object[] attrbutes = info.GetCustomAttributes(typeof(ServedClassNameAttribute), false);
                        if (attrbutes.Length > 0)
                        {
                            
                            if (!all_instances)
                               if ((SharedResources.GEMINI_INSTANCE_NUMBER == 1 && !type.FullName.Contains(".GeminiTelescope.")) ||
                                (SharedResources.GEMINI_INSTANCE_NUMBER == 2 && !type.FullName.Contains(".GeminiTelescope2.")))
                                continue;   // skip if this is not the instance that serves this gemini number

                            //MessageBox.Show("Adding Type: " + type.Name + " " + type.FullName);
                                m_ComObjectTypes.Add(type); //PWGS - much simpler
                            m_ComObjectAssys.Add(so);
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                    // Probably an attempt to load a Win32 DLL (i.e. not a .net assembly)
                    // Just swallow the exception and continue to the next item.
                    continue;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to load served COM class assembly " + fi.Name + " - " + e.Message,
                        "test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

            }
            return true;
        }
        #endregion

        #region COM Registration and Unregistration
        //
        // Test if running elevated
        //
        private static bool IsAdministrator
        {
            get
            {
                WindowsIdentity i = WindowsIdentity.GetCurrent();
                WindowsPrincipal p = new WindowsPrincipal(i);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        //
        // Elevate by re-running ourselves with elevation dialog
        //
        private static void ElevateSelf(string arg)
        {
            ProcessStartInfo si = new ProcessStartInfo();
            si.Arguments = arg;
            si.WorkingDirectory = Environment.CurrentDirectory;
            si.FileName = Application.ExecutablePath;
            si.Verb = "runas";
            try { Process.Start(si); }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("The test was not " + (arg == "/register" ? "registered" : "unregistered") +
                    " because you did not allow it.", "test", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return;
        }

        //
        // Do everything to register this for COM. Never use REGASM on
        // this exe assembly! It would create InProcServer32 entries 
        // which would prevent proper activation!
        //
        // Using the list of COM object types generated during dynamic
        // assembly loading, it registers each one for COM as served by our
        // exe/local server, as well as registering it for ASCOM. It also
        // adds DCOM info for the local server itself, so it can be activated
        // via an outboiud connection from TheSky.
        //
        private static void RegisterObjects()
        {
            if (!IsAdministrator)
            {
                ElevateSelf("/register");
                return;
            }
            //
            // If reached here, we're running elevated
            //

            LoadComObjectAssemblies(true);  //reload all assemblies, not just instance-specific ones, so we can register them
            Assembly assy = Assembly.GetExecutingAssembly();
            Attribute attr = Attribute.GetCustomAttribute(assy, typeof(AssemblyTitleAttribute));
            string assyTitle = ((AssemblyTitleAttribute)attr).Title;
            attr = Attribute.GetCustomAttribute(assy, typeof(AssemblyDescriptionAttribute));
            string assyDescription = ((AssemblyDescriptionAttribute)attr).Description;

            //
            // Local server's DCOM/AppID information
            //
            try
            {
                //
                // HKCR\APPID\appid
                //
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey("APPID\\" + s_appId))
                {
                    key.SetValue(null, assyDescription);
                    key.SetValue("AppID", s_appId);
                    key.SetValue("AuthenticationLevel", 1, RegistryValueKind.DWord);
                }
                //
                // HKCR\APPID\exename.ext
                //
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(string.Format("APPID\\{0}",
                            Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf('\\') + 1))))
                {
                    key.SetValue("AppID", s_appId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while registering the server:\n" + ex.ToString(),
                        "test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            finally
            {
            }

            //
            // For each of the driver assemblies
            //
            foreach (Type type in m_ComObjectTypes)
            {
                bool bFail = false;
                try
                {
                    //
                    // HKCR\CLSID\clsid
                    //
                    string clsid = Marshal.GenerateGuidForType(type).ToString("B");
                    string progid = Marshal.GenerateProgIdForType(type);
                    //PWGS Generate device type from the Class name
                    string deviceType = type.Name;

                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(string.Format("CLSID\\{0}", clsid)))
                    {
                        key.SetValue(null, progid);						// Could be assyTitle/Desc??, but .NET components show ProgId here
                        key.SetValue("AppId", s_appId);
                        using (RegistryKey key2 = key.CreateSubKey("Implemented Categories"))
                        {
                            key2.CreateSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
                        }
                        using (RegistryKey key2 = key.CreateSubKey("ProgId"))
                        {
                            key2.SetValue(null, progid);
                        }
                        key.CreateSubKey("Programmable");
                        using (RegistryKey key2 = key.CreateSubKey("LocalServer32"))
                        {
                            key2.SetValue(null, Application.ExecutablePath);
                        }
                    }
                    //
                    // HKCR\CLSID\progid
                    //
                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(progid))
                    {
                        key.SetValue(null, assyTitle);
                        using (RegistryKey key2 = key.CreateSubKey("CLSID"))
                        {
                            key2.SetValue(null, clsid);
                        }
                    }
                    //
                    // ASCOM 
                    //
                    assy = type.Assembly;

                    // Pull the display name from the ServedClassName attribute.
                    attr = Attribute.GetCustomAttribute(type, typeof(ServedClassNameAttribute)); //PWGS Changed to search type for attribute rather than assembly
                    string chooserName = ((ServedClassNameAttribute)attr).DisplayName ?? "MultiServer";
                    using (var P = new ASCOM.Utilities.Profile())
                    {
                        P.DeviceType = deviceType;
                        P.Register(progid, chooserName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while registering the server:\n" + ex.ToString(),
                            "test", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    bFail = true;
                }
                finally
                {
                }
                if (bFail) break;
            }
        }

        //
        // Remove all traces of this from the registry. 
        //
        // **TODO** If the above does AppID/DCOM stuff, this would have
        // to remove that stuff too.
        //
        private static void UnregisterObjects()
        {
            if (!IsAdministrator)
            {
                ElevateSelf("/unregister");
                return;
            }

            //
            // Local server's DCOM/AppID information
            //
            Registry.ClassesRoot.DeleteSubKey(string.Format("APPID\\{0}", s_appId), false);
            Registry.ClassesRoot.DeleteSubKey(string.Format("APPID\\{0}",
                    Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf('\\') + 1)), false);

            //
            // For each of the driver assemblies
            //
            foreach (Type type in m_ComObjectTypes)
            {
                string clsid = Marshal.GenerateGuidForType(type).ToString("B");
                string progid = Marshal.GenerateProgIdForType(type);
                string deviceType = type.Name;
                //
                // Best efforts
                //
                //
                // HKCR\progid
                //
                Registry.ClassesRoot.DeleteSubKey(String.Format("{0}\\CLSID", progid), false);
                Registry.ClassesRoot.DeleteSubKey(progid, false);
                //
                // HKCR\CLSID\clsid
                //
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}\\Implemented Categories\\{{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}}", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}\\Implemented Categories", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}\\ProgId", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}\\LocalServer32", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}\\Programmable", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(String.Format("CLSID\\{0}", clsid), false);

                if (m_UnregisterASCOM) // Added to ensure that we don't unregister during a driver upgrade
                {
                    try
                    {
                        //
                        // ASCOM
                        //
                        using (var P = new ASCOM.Utilities.Profile())
                        {
                            P.DeviceType = deviceType;
                            P.Unregister(progid);
                        }
                    }
                    catch (Exception) { }
                }
            }
        }
        #endregion

        #region Class Factory Support
        //
        // On startup, we register the class factories of the COM objects
        // that we serve. This requires the class facgtory name to be
        // equal to the served class name + "ClassFactory".
        //
        private static bool RegisterClassFactories()
        {
            m_ClassFactories = new ArrayList();
            foreach (Type type in m_ComObjectTypes)
            {
                ClassFactory factory = new ClassFactory(type);					// Use default context & flags
                m_ClassFactories.Add(factory);
                if (!factory.RegisterClassObject())
                {
                    MessageBox.Show("Failed to register class factory for " + type.Name,
                        "GeminiTelescope", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            ClassFactory.ResumeClassObjects();									// Served objects now go live
            return true;
        }

        private static void RevokeClassFactories()
        {
            ClassFactory.SuspendClassObjects();									// Prevent race conditions
            foreach (ClassFactory factory in m_ClassFactories)
                factory.RevokeClassObject();
        }
        #endregion

        #region Command Line Arguments
        //
        // ProcessArguments() will process the command-line arguments
        // If the return value is true, we carry on and start this application.
        // If the return value is false, we terminate this application immediately.
        //
        private static bool ProcessArguments(string[] args)
        {
            bool bRet = true;

            //
            //**TODO** -Embedding is "ActiveX start". Prohibit non_AX starting?
            //
            if (args.Length > 0)
            {

                switch (args[0].ToLower())
                {
                    case "-embedding":
                        m_bComStart = true;										// Indicate COM started us
                        break;

                    case "-register":
                    case "/register":
                    case "-regserver":											// Emulate VB6
                    case "/regserver":
                        RegisterObjects();										// Register each served object
                        bRet = false;
                        break;

                    case "-unregister":
                    case "/unregister":
                    case "-unregserver":										// Emulate VB6
                    case "/unregserver":
                        m_UnregisterASCOM = true;
                        UnregisterObjects();									//Unregister each served object
                        bRet = false;
                        break;
                    case "-unregcom":
                    case "/unregcom":
                        m_UnregisterASCOM = false;
                        UnregisterObjects();									//Unregister each served object leave ASCOM profile
                        bRet = false;
                        break;
                    default:
                        MessageBox.Show("Unknown argument: " + args[0] + "\nValid are : -register, -unregister and -embedding",
                            "GeminiTelescope", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                }
            }
            else
                m_bComStart = false;

            return bRet;
        }
        #endregion

        #region SERVER ENTRY POINT (main)
        //
        // ==================
        // SERVER ENTRY POINT
        // ==================
        //
        [STAThread]
        static void Main(string[] args)
        {

            Mutex mut = null;


            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                mut = new Mutex(false, SharedResources.TELESCOPE_PROGRAM_ID + "_Mutex");
                if (!mut.WaitOne(500))
                {
                    var a = MessageBox.Show("Another instance of " + SharedResources.TELESCOPE_DRIVER_NAME + " is already running.\r\n\r\nWould you like to start another instance to connect to a different Gemini unit?", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (a == DialogResult.Yes)
                    { //starting another instance
                        SharedResources.TELESCOPE_PROGRAM_ID = "ASCOM.GeminiTelescope2.Telescope";
                        SharedResources.TELESCOPE_DRIVER_NAME += " #2";
                        SharedResources.DEAULT_PROFILE = "GeminiDefaultProfile2.gp";
                        SharedResources.GEMINI_INSTANCE_NUMBER = 2;

                        mut = new Mutex(false, SharedResources.TELESCOPE_PROGRAM_ID + "_Mutex");
                        if (!mut.WaitOne(500))
                        {
                            MessageBox.Show("Another instance of " + SharedResources.TELESCOPE_DRIVER_NAME + " is already running.\r\nPlease close it before starting this one.", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                        return;
                }
            }
            catch
            {
            }


            if (!LoadComObjectAssemblies()) return;						// Load served COM class assemblies, get types

            if (!ProcessArguments(args)) return;						// Register/Unregister

            // Initialize critical member variables.
            m_iObjsInUse = 0;
            m_iServerLocks = 0;
            m_uiMainThreadId = GetCurrentThreadId();
            Thread.CurrentThread.Name = "Main Thread";
  
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            m_MainForm = new frmMain();
            if (m_bComStart && !GeminiHardware.Instance.ShowHandbox) m_MainForm.WindowState = FormWindowState.Minimized;

            // Register the class factories of the served objects
            RegisterClassFactories();

            // Start up the garbage collection thread.
            GarbageCollection GarbageCollector = new GarbageCollection(1000);
            Thread GCThread = new Thread(new ThreadStart(GarbageCollector.GCWatch));
            GCThread.Name = "Garbage Collection Thread";
            GCThread.Start();

            //
            // Start the message loop. This serializes incoming calls to our
            // served COM objects, making this act like the VB6 equivalent!
            //
            Application.Run(m_MainForm);

            // Revoke the class factories immediately.
            // Don't wait until the thread has stopped before
            // we perform revocation!!!
            RevokeClassFactories();
            if (mut != null) mut.ReleaseMutex();

            // Now stop the Garbage Collector thread.
            GarbageCollector.StopThread();
            GarbageCollector.WaitForThreadToStop();


        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (m_MainForm != null)
                {
                    if (GeminiHardware.Instance.Trace != null)
                    {
                        Exception ex = e.ExceptionObject as Exception;
                        if (ex != null)
                        {
                            GeminiHardware.Instance.Trace.Except(ex);
                            GeminiHardware.Instance.Trace.Error("CurrentDomain_Exception", ex.Message, ex.Source, ex.StackTrace, ex.InnerException);
                        }
                        MessageBox.Show(SharedResources.TELESCOPE_DRIVER_NAME + " has encountered an error and must now close\r\n\r\n" + ex.ToString(), SharedResources.TELESCOPE_DRIVER_NAME + "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                if (m_MainForm != null)
                {
                    if (GeminiHardware.Instance.Trace != null)
                    {
                        Exception ex = e.Exception as Exception;
                        if (ex != null)
                        {
                            GeminiHardware.Instance.Trace.Except(ex);
                            GeminiHardware.Instance.Trace.Error("ThreadException", ex.Message, ex.Source, ex.StackTrace, ex.InnerException);
                            MessageBox.Show(SharedResources.TELESCOPE_DRIVER_NAME + " has encountered an error and must now close\r\n\r\n" + ex.ToString(), SharedResources.TELESCOPE_DRIVER_NAME + "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
            finally
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        #endregion
    }
}
