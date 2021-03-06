﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.GeminiTelescope
{
    public class Tracer
    {
        private int m_Level = 0;
        private ASCOM.Utilities.TraceLogger m_Log;
        public void Start(string name, int level)
        {
            try
            {
                if (m_Log == null)
                {
                    m_Log = new ASCOM.Utilities.TraceLogger(null, name);
                    CleanUpLogs();
                }
                m_Log.Enabled = true;
                m_Level = level;
                m_Log.LogMessage("Log started: " + name, "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " " + DateTime.Now.ToUniversalTime().ToString());
            }
            catch { }
        }

        private void CleanUpLogs()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ASCOM";

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.DirectoryInfo [] dirs = di.GetDirectories("Logs *");
                foreach (System.IO.DirectoryInfo dir in dirs)
                {
                    // if more than 10-days old, delete old logs
                    if (DateTime.Now - dir.LastWriteTime > TimeSpan.FromDays(SharedResources.DAYS_TO_KEEP_LOGS))
                    {
                        CleanUpFolder(dir);
                    }
                }

            }
            catch { }
        }

        private void CleanUpFolder(System.IO.DirectoryInfo dir)
        {

            System.IO.FileInfo [] fis = dir.GetFiles("ASCOM." + SharedResources.TELESCOPE_DRIVER_NAME + "*.*");
            foreach (System.IO.FileInfo fi in fis)
            {
                try
                {
                    if (DateTime.Now  - fi.LastWriteTime > TimeSpan.FromDays(SharedResources.DAYS_TO_KEEP_LOGS))
                    {
                        fi.Delete();
                    }
                }
                catch { }
            }

            // remove the folder if there are no files left:
            fis = dir.GetFiles();
            if (fis == null || fis.Length == 0)
            {
                try
                {
                    dir.Delete();
                }
                catch { }
            }
        }



        public void Enter(int level, string function, params object[] par)
        {
            if (m_Log!=null && m_Log.Enabled && level <= m_Level) 
            {
                lock (m_Log)
                {
                    m_Log.LogStart(function, "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " [Enter] ");
                    for (int i = 0; i < par.Length; ++i)
                    {
                        object o = par[i];
                        m_Log.LogContinue((o??"null") + (i != par.Length - 1 ? ", " : ""));
                    }
                    m_Log.LogFinish("");
                }
            }
        }

        public void Enter(string function, params object[] par)
        {
            Enter(2, function, par);
        }

        public void Exit(int level, string function, params object[] result)
        {
            if (m_Log != null && m_Log.Enabled && level <= m_Level)
            {
                lock (m_Log)
                {

                    m_Log.LogStart(function, "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " [Exit] ");
                    for (int i = 0; i < result.Length; ++i)
                    {
                        object o = result[i];
                        m_Log.LogContinue((o ?? "null") + (i != result.Length - 1 ? ", " : ""));
                    }
                    m_Log.LogFinish("");
                }
            }
        }

        public void Exit(string function, params object[] result)
        {
            Exit(2, function, result);
        }

        public void Except(Exception e)
        {
            if (m_Log != null && m_Log.Enabled)
            {
                lock (m_Log)
                {
                    m_Log.LogStart("Exception", "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " [Error]: ");
                    m_Log.LogFinish(e.ToString());
                }
            }
        }

        public void Error(string s, params object[] values)
        {
             if (m_Log != null && m_Log.Enabled)
            {
                lock (m_Log)
                {

                    m_Log.LogStart(s, "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " [Error]: ");
                    for (int i = 0; i < values.Length; ++i)
                    {
                        object o = values[i];
                        m_Log.LogContinue((o ?? "null") + (i != values.Length - 1 ? ", " : ""));
                    }
                    m_Log.LogFinish("");
                }
            }
        }

        public void Info(int level, string s, params object[] values)
        {
            if (m_Log != null && m_Log.Enabled && level <= m_Level)
            {
                lock (m_Log)
                {

                    m_Log.LogStart(s, "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " [" + level.ToString() + "]: ");
                    for (int i = 0; i < values.Length; ++i)
                    {
                        object o = values[i];
                        m_Log.LogContinue((o ?? "null") + (i != values.Length - 1 ? ", " : ""));
                    }
                    m_Log.LogFinish("");
                }
            }
        }

        public void Stop()
        {
            if (m_Log != null && m_Log.Enabled)
            {
                lock (m_Log)
                {

                    m_Log.LogMessage("Log closed", "TID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " " + DateTime.Now.ToUniversalTime().ToString());
                    m_Log.Enabled = false;
                }
            }
        }
    }
}
