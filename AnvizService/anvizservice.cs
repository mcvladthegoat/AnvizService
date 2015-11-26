using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.Data.SqlClient;
using TimeAttendance;
using System.Runtime.InteropServices;
using AnvizLibrary;


namespace AnvizProject
{
    public partial class AnvizService : ServiceBase
    {
        public static StringCollection devices = AnvizProject.Properties.Settings.Default.devices;
        public static string con_str = AnvizProject.Properties.Settings.Default.con_str;

        private static EventLog m_EventLog;
        private Thread m_CheckThread = new Thread(ThCheckAppServer);
        EventWaitHandle m_CloseEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        bool m_IsPaused = false;


        public AnvizService()
        {
            InitializeComponent();
        }

        public EventWaitHandle CloseEvent
        {
            get { return m_CloseEvent; }
        }
        
        #region Handlers
        protected override void OnStart(string[] args)
        {
            this.RequestAdditionalTime(10000);

            if (EventLog.SourceExists(this.ServiceName))
            {
                m_EventLog = new EventLog();
                m_EventLog.Source = this.ServiceName;
            }

            AnvizLibraryClass.Trace("OnStart:0");
            Start(false);

            AnvizLibraryClass.Trace("OnStart:1");
            m_CheckThread.Start(this);
            AnvizLibraryClass.Trace("OnStart:2");
        }




        protected override void OnStop()
        {
            m_CloseEvent.Set();

            while ((m_CheckThread != null) && m_CheckThread.IsAlive)
            {
                this.RequestAdditionalTime(2000);
                System.Threading.Thread.Sleep(1000);
            }
            
            Stop(false);
        }

        protected override void OnPause()
        {
            lock (m_CheckThread)
            {
                m_IsPaused = true;
            }
        }

        protected override void OnContinue()
        {
            lock (m_CheckThread)
            {
                m_IsPaused = false;
            }
        }
        
        protected override void OnShutdown()
        {
        }
        
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
        }

        #endregion

        protected void Start(bool isNoServiceCall)
        {
            // todo add code
            //TestConnection(AnvizService.Properties.Settings.Default.con_str);
        }

        protected void Stop(bool isNoServiceCall)
        {
            // todo add code
        }

        protected void Check(EventWaitHandle CloseEvent)
        {
            lock (m_CheckThread)
            {
                if (m_IsPaused)
                    return;
            }

            // todo add code
        }



        private static void ThCheckAppServer(Object obj)
        {
            AnvizLibraryClass.Trace("ThCheckAppServer:0");
            AnvizService srvc = (AnvizService)obj;
            AnvizLibraryClass.Trace("ThCheckAppServer:1");

            while (!srvc.CloseEvent.WaitOne(30000, false))
            {
                AnvizLibraryClass.Trace("ThCheckAppServer:check");
                
                for(int i=0;i<devices.Count;i++)
                {
                    string ip = devices[i].Substring(devices[i].IndexOf(':') + 1);
                    int id = Convert.ToInt32(devices[i].Remove(devices[i].IndexOf(":")));
                    if( !AnvizLibraryClass.ConnectLAN(ip, id) )
                    {
                        continue;
                    }
                    AnvizLibraryClass.GetDataFromDevice();
                    AnvizNew.CKT_Disconnect();
                }
                srvc.Check(srvc.CloseEvent);
                AnvizLibraryClass.Trace("ThCheckAppServer:check:end");
            }
        }

    }
}
