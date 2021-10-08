using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;

namespace ASCOM.GeminiTelescope
{
    public partial class frmCollector : Form
    {
        string Folder;
        string log;

        public frmCollector()
        {
            InitializeComponent();
        }

        private void frmCollector_Load(object sender, EventArgs e)
        {
            try
            {
                if (!GeminiHardware.Instance.Connected)
                    GeminiHardware.Instance.Connected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gemini is not connected!\r\nOnly information from the PC will be included in the report.", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            for (int i = 0; i < lbCollect.Items.Count; ++i)
                lbCollect.SetItemChecked(i, true);
        }

        private void pbCollect_Click(object sender, EventArgs e)
        {
            if (lbCollect.CheckedIndices.Count == 0)
            {
                MessageBox.Show("Please select at least one item!", SharedResources.TELESCOPE_DRIVER_NAME, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            Folder = System.IO.Path.GetTempPath() + "GeminiCollector";
            if (Directory.Exists(Folder)) Directory.Delete(Folder, true);
            Directory.CreateDirectory(Folder);

            log = $"{SharedResources.TELESCOPE_DRIVER_NAME} v{Application.ProductVersion} Troubleshooting Report\r\n{DateTime.Now.ToString("o")}\r\n";
            log += $"Selected: {String.Join(", ", lbCollect.CheckedItems.OfType<string>())}\r\n";
            log += $"Temp folder: {Folder}\r\n";

            ComputerInfo();

            for (int i = 0; i < lbCollect.Items.Count; ++i)
            {
                if (lbCollect.GetItemChecked(i))
                {
                    try
                    {
                        switch (i)
                        {
                            case 0: Network(); break;
                            case 1: Status(); break;
                            case 2: GeminiSettings(); break;
                            case 3: GeminiPEC(); break;
                            case 4: ASCOMPlatform(); break;
                            case 5: ASCOMProfile(); break;
                            case 6: ASCOMLog(); break;
                        }
                    }
                    catch (Exception ex)
                    {
                        log += $"Collector Error ({i}): {ex.Message}\r\n";
                    }
                }
            }

            log += "Collection completed\r\n";

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.FileName = SharedResources.TELESCOPE_DRIVER_INFO + " Report.zip";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = path;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.DefaultExt = "zip";
            saveFileDialog.Filter = "Report Zip File (*.zip)|*.zip;";
            saveFileDialog.Title = "Save Report File";
            DialogResult res = saveFileDialog.ShowDialog(this);

            if (res != DialogResult.OK) return;

            log += $"Saving report to {saveFileDialog.FileName}\r\n";
            log += $"Collection completed {DateTime.Now.ToString("o")}\r\n";
            File.WriteAllText(Folder + "\\collector.log", log);

            if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
            try {
                ZipFile.CreateFromDirectory(Folder, saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Troubleshooting report was not saved! Error: {ex.Message}");
                log += $"Save report error: {ex.Message}\r\n";
            }


            try
            {
                Process.Start("explorer.exe", $"/select,\"{saveFileDialog.FileName}\"");
            } catch { }

            DialogResult = DialogResult.OK;
        }

        private void ComputerInfo()
        {
           
        }

        private void ASCOMLog()
        {
            log += $"Collecting Logs from ASCOM...";
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ASCOM";

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.DirectoryInfo[] dirs = di.GetDirectories("Logs *").OrderByDescending(x => x.LastWriteTime).ToArray();
                if (dirs.Length > 0)
                {
                    var fis = dirs[0].GetFiles("ASCOM." + SharedResources.TELESCOPE_DRIVER_NAME + "*.*").OrderByDescending(x => x.LastWriteTime).Take(2);

                    foreach (var fi in fis)
                    {
                        File.Copy(fi.FullName, Folder + "\\" + fi.Name);
                        log += $"File: {fi.FullName}...";
                    }
                }

            }
            catch (Exception ex)
            {
                log += $"ASCOMLog Error: {ex.Message}";
            }

            log += "\r\n";

        }

        private void ASCOMProfile()
        {
            log += $"Collecting Gemini Settings from ASCOM...";
            if (GeminiHardware.Instance.Connected)
            {
                GeminiProperties props = new GeminiProperties();

                props.SavePEC = (lbCollect.GetItemChecked(3)); //if pec is selected, add it into the profile
                if ((GeminiHardware.Instance.PECStatus & 3) == 0) // no pec programmed
                    props.SavePEC = false;

                if (props.SyncWithGemini(false))
                {
                    props.Serialize(true, Folder + "\\Gemini_Profile.xml");
                    log += $"Done";
                }
                else
                {
                    log += $"Sync not done";
                }
            }
            else
                log += $"Gemini not connected";

            log += $"\r\n";
        }

        private void ASCOMPlatform()
        {
            log += $"Collecting ASCOM Platfrom Profile Settings...";
            var profile = GeminiHardware.Instance.m_Profile.GetProfileXML(SharedResources.TELESCOPE_PROGRAM_ID);
            File.WriteAllText(Folder + "\\ASCOM_Platform_Settings.xml", profile);
            log += $" written to {(Folder + "\\ASCOM_Platform_Settings.xml")}, size={profile.Length}\r\n";
        }

        private void GeminiPEC()
        {

        }

        private void GeminiSettings()
        {
            log += $"Collecting Gemini Logs...";

            if (!GeminiHardware.Instance.IsEthernetConnected)
                log += $"Ethernet is not connected, can't download files from Gemini!...";

            if (GeminiHardware.Instance.dVersion >= 5 && GeminiHardware.Instance.IsEthernetConnected)
            {
                log += $" Gemini version {GeminiHardware.Instance.dVersion}...";
                using (MyWebClient webClient = new MyWebClient())
                {
                    webClient.Credentials = new NetworkCredential(GeminiHardware.Instance.m_EthernetUser, GeminiHardware.Instance.m_EthernetPassword);

                    try
                    {
                        webClient.Timeout = 5000;
                        webClient.Encoding = System.Text.Encoding.UTF8;
                        try
                        {
                            webClient.DownloadFile($"ftp://{GeminiHardware.Instance.m_EthernetIP}/LOGS/RA_Enc.log", Folder + "\\Gemini_RA_Enc.log");
                            log += $"Downloaded RA_Enc.log...";
                        }
                        catch (Exception ex) {
                            log += $"Error RA_Enc.log {ex.Message}...";
                        }
                        try
                        {
                            webClient.DownloadFile($"ftp://{GeminiHardware.Instance.m_EthernetIP}/LOGS/Gemini.log", Folder + "\\Gemini.log");
                            log += $"Downloaded Gemini.log...";
                        }
                        catch (Exception ex)
                        {
                            log += $"Error Gemini.log {ex.Message}...";
                        }

                        try
                        {
                            webClient.DownloadFile($"ftp://{GeminiHardware.Instance.m_EthernetIP}/LOGS/POINTING.DAT", Folder + "\\Gemini_POINTING.DAT");
                            log += $"Downloaded POINTING.DAT...";
                        }
                        catch (Exception ex)
                        {
                            log += $"Error POINTING.DAT {ex.Message}...";
                        }

                        log += $"Done";
                    }

                    catch (Exception ex)
                    {
                        log += $"Error {ex.Message}";
                    }

                    log += "\r\n";
                }

            }
        }

        private void Status()
        {
            log += $"Collecting Gemini Status...";

            string status = "";
            if (GeminiHardware.Instance.Connected)
                status += $"Gemini Already Connected\r\n";
            else
            {
                status += $"Gemini is NOT Connected\r\n";
            }

            status += $"PC Civil Time={(DateTime.Now.ToString("o", CultureInfo.InvariantCulture))} PC Time Zone Offset={TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)}\r\n";

            log += status;

            if (GeminiHardware.Instance.Connected)
            {
                var util = GeminiHardware.Instance.m_Util;
                GeminiHardware.Instance.UpdatePolledVariables(true);
                //GeminiHardware.Instance.UpdatePolledVariables(true); // do twice to get all the variables
                status += $"Gemini version: {GeminiHardware.Instance.dVersion}\r\nBoot Mode={GeminiHardware.Instance.m_BootMode.ToString()}\r\n";
                status += $"Firmware={GeminiHardware.Instance.DoCommandResult(":GVD", 2000, false)} {GeminiHardware.Instance.DoCommandResult(":GVT", 2000, false)}\r\n";
                status += $"Mount Type={GeminiHardware.Instance.DoCommandResult("<0", 2000, false)}\r\n";
                status += $"Use Ethernet={GeminiHardware.Instance.m_EthernetPort}\r\nEthernet IP={GeminiHardware.Instance.m_EthernetIP}\r\nCOM Port={GeminiHardware.Instance.m_ComPort}\r\nScan COM Ports={GeminiHardware.Instance.m_ScanCOMPorts}\r\nUDP Port={GeminiHardware.Instance.m_UDPPort}\r\n";
                if (GeminiHardware.Instance.dVersion >= 5.0)
                {
                    status += $"IP={GeminiHardware.Instance.DoCommandResult("<801", 2000, false)}\r\nMask={GeminiHardware.Instance.DoCommandResult("<802", 2000, false)}\r\n";
                    status += $"Gateway={GeminiHardware.Instance.DoCommandResult("<803", 2000, false)}\r\nDNS={GeminiHardware.Instance.DoCommandResult("<804", 2000, false)}\r\n";
                    status += $"DHCP={GeminiHardware.Instance.DoCommandResult("<805", 2000, false)}\r\nMAC={GeminiHardware.Instance.DoCommandResult("<818", 2000, false)}\r\n";

                    status += $"Startup Code={GeminiHardware.Instance.DoCommandResult("<96", 2000, false)}\r\n";
                    status += $"Status Check={GeminiHardware.Instance.DoCommandResult("<99", 2000, false)}\r\n";
                    status += $"Steps to Western Limit={GeminiHardware.Instance.DoCommandResult("<225", 2000, false)}\r\n";
                    status += $"Time to Western Limit={GeminiHardware.Instance.DoCommandResult("<226", 2000, false)}\r\n";
                }
                if (GeminiHardware.Instance.dVersion >= 5.2)
                {
                    status += $"Main Voltage={GeminiHardware.Instance.DoCommandResult("<321", 2000, false)}\r\n";
                    status += $"Battery Voltage={GeminiHardware.Instance.DoCommandResult("<322", 2000, false)}\r\n";
                }
                status += $"Send Advanced Settings={GeminiHardware.Instance.m_SendAdvancedSettings}\r\n";
                status += $"Set Site={GeminiHardware.Instance.m_UseDriverSite}\r\nSet Time={GeminiHardware.Instance.m_UseDriverTime}\r\n";

                status += $"Latitude={util.DegreesToDMS(GeminiHardware.Instance.m_Latitude)}\r\n";
                status += $"Longitude={util.DegreesToDMS(GeminiHardware.Instance.m_Longitude)}\r\n";
                status += $"Time Zone Offset={GeminiHardware.Instance.UTCOffset}\r\n";
                double local = 0;
                double.TryParse(GeminiHardware.Instance.DoCommandResult(":GL", 2000, false), out local);
                status += $"Civil Time={util.HoursToHMS(local)}\r\n";
                status += $"HA={util.HoursToHMS(GeminiHardware.Instance.HourAngle)}\r\n";
                status += $"RA={util.HoursToHMS(GeminiHardware.Instance.m_RightAscension)}\r\n";
                status += $"DEC={util.DegreesToDMS(GeminiHardware.Instance.m_Declination)}\r\n";
                status += $"Azimuth={util.DegreesToDMS(GeminiHardware.Instance.m_Azimuth)}\r\n";
                status += $"Altitude={util.DegreesToDMS(GeminiHardware.Instance.Altitude)}\r\n";
                status += $"Velocity={GeminiHardware.Instance.Velocity}\r\n";
                status += $"Speed={ GeminiHardware.Instance.m_Speed}\r\n";
                status += $"Sidereal Time={util.HoursToHMS(GeminiHardware.Instance.m_SiderealTime)}\r\nSouthern Hemisphere={GeminiHardware.Instance.m_SouthernHemisphere}\r\n";
                status += $"Tracking={GeminiHardware.Instance.m_Tracking}\r\nNudge from Safety={GeminiHardware.Instance.m_NudgeFromSafety}\r\n";

                status += $"Eastern Safety Limit={GeminiHardware.Instance.DoCommandResult("<221", 2000, false)}\r\n";
                status += $"Western Safety Limit={GeminiHardware.Instance.DoCommandResult("<222", 2000, false)}\r\n";
                status += $"Goto Limit={GeminiHardware.Instance.DoCommandResult("<223", 2000, false)}\r\n";
                status += $"AtHome={GeminiHardware.Instance.m_AtHome}\r\nAtPark={GeminiHardware.Instance.m_AtPark}\r\n";
                status += $"Status Byte={GeminiHardware.Instance.m_GeminiStatusByte}\r\nAt Safety Limit={GeminiHardware.Instance.m_SafetyNotified}\r\n";
                status += $"Side of Pier={GeminiHardware.Instance.m_SideOfPier}\r\n";

                status += $"Park Position={GeminiHardware.Instance.m_ParkPosition}\r\nPark Alt={GeminiHardware.Instance.m_ParkAlt}\r\nPark Az={GeminiHardware.Instance.m_ParkAz}\r\n";
                status += $"Park State={GeminiHardware.Instance.m_ParkState}\r\n";
                status += $"Precession={GeminiHardware.Instance.m_Precession}\r\nRefraction={GeminiHardware.Instance.m_Refraction}\r\nPrecision Pulse Guide={GeminiHardware.Instance.m_PrecisionPulseGuide}\r\n";
                status += $"Slew Settle Time ={GeminiHardware.Instance.m_SlewSettleTime}\r\n";

                status += $"Encoder resolution RA={GeminiHardware.Instance.DoCommandResult("<100", 2000, false)}\r\n";
                status += $"Encoder resolution DEC={GeminiHardware.Instance.DoCommandResult("<110", 2000, false)}\r\n";
                status += $"Tracking rate={GeminiHardware.Instance.DoCommandResult("<130", 2000, false)}\r\n";
                status += $"TVC={GeminiHardware.Instance.DoCommandResult("<200", 2000, false)}\r\n";
                status += $"Curent PEC Counter={GeminiHardware.Instance.DoCommandResult("<503", 2000, false)}\r\n";
                status += $"PEC Max={GeminiHardware.Instance.DoCommandResult("<503", 2000, false)}\r\n";
                status += $"PEC Status={GeminiHardware.Instance.DoCommandResult("<509", 2000, false)}\r\n";

                status += $"Slew Speed={GeminiHardware.Instance.DoCommandResult("<120", 2000, false)}\r\n";
                status += $"Goto Speed={GeminiHardware.Instance.DoCommandResult("<140", 2000, false)}\r\n";              
                status += $"Guide Speed={GeminiHardware.Instance.DoCommandResult("<150", 2000, false)}\r\n";
            }

            File.WriteAllText(Folder + "\\gemini_status.log", status);
            log += $" written to {(Folder + "\\gemini_status.log")}, size={status.Length}\r\n";
        }

        private void Network()
        {
            log += $"Collecting Network settings...";
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "ipconfig.exe";
            p.StartInfo.Arguments = "/all";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "systeminfo.exe";
            //p.StartInfo.Arguments = "/all";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string output2 = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            output += "\r\n\r\n--------------------------SYSINFO-----------------\r\n\r\n" + output2;

            File.WriteAllText(Folder + "\\network.log", output);
            log += $" written to {(Folder + "\\network.log")}, size={output.Length}\r\n";
        }



        void getGeminiLogFiles()
        {



        }



         private class MyWebClient : WebClient
        {
            public int Timeout { get; set; }
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);

                w.Timeout = Timeout;
                //((HttpWebRequest)w).ReadWriteTimeout = Timeout;
                return w;
            }
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
