using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Reflection;

namespace TestService
{
    public partial class TestService : ServiceBase
    {
        private EventLog m_EventLog;
        private Thread m_CheckThread = new Thread(ThCheckAppServer);
        EventWaitHandle m_CloseEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        bool m_IsPaused = false;


        public TestService()
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

            Trace("OnStart:0");
            Start(false);

            Trace("OnStart:1");
            m_CheckThread.Start(this);
            Trace("OnStart:2");
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

        protected void EventLogMessage(String message, EventLogEntryType type)
        {
            if (m_EventLog == null)
                return;
            try
            {
                m_EventLog.WriteEntry(message, type);
            }
            catch (ArgumentException)
            {
                m_EventLog.WriteEntry(String.Format("ArgumentException len = {0}\n{1}",
                                            message.Length, message.Substring(0, 100)),
                                        EventLogEntryType.Error);
            }
        }
        internal static void Trace(String s)
        {
            String mess = String.Format("Th {0}| Time {1}| {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, s);
            System.Diagnostics.Trace.WriteLine(mess);
        }

        private static void ThCheckAppServer(Object obj)
        {
            Trace("ThCheckAppServer:0");
            TestService srvc = (TestService)obj;
            Trace("ThCheckAppServer:1");

            while (!srvc.CloseEvent.WaitOne(30000, false))
            {
                Trace("ThCheckAppServer:check");
                srvc.Check(srvc.CloseEvent);
                Trace("ThCheckAppServer:check:end");
            }
        }
    }
}
