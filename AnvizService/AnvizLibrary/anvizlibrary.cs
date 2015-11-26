using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using TimeAttendance;
using System.Diagnostics;
using System.Threading;
using System.Data.SqlClient;
namespace AnvizLibrary
{
    public class AnvizLibraryClass
    {
        [DllImport("Kernel32.dll")]
        public static extern bool RtlMoveMemory(ref AnvizNew.CLOCKINGRECORD Destination, int Source, int Length);
        [DllImport("Kernel32.dll")]
        public static extern bool RtlMoveMemory(ref AnvizNew.PERSONINFO Destination, int Source, int Length);
        [DllImport("Kernel32.dll")]
        public static extern bool RtlMoveMemory(ref AnvizNew.PERSONINFOEX Destination, int Source, int Length);
        [DllImport("Kernel32.dll")]
        public static extern bool RtlMoveMemory(ref int Destination, int Source, int Length);
        [DllImport("Kernel32.dll")]
        public static extern bool RtlMoveMemory(ref byte Destination, int Source, int Length);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref AnvizNew.SYSTEMTIME lpSystemTime);

        private static int ReaderNo;
        private static string ReaderIpAddress;
        static AnvizNew.CLOCKINGRECORD clocking = new AnvizNew.CLOCKINGRECORD();

        public static string con_str;

        public static EventLog m_EventLog;
        
        public static EventWaitHandle m_CloseEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        public bool m_IsPaused = false;

        public static bool ConnectLAN(string ReaderIpAddress_, int id)
        {
            ReaderNo = id;
            ReaderIpAddress = ReaderIpAddress_;
            try
            {
                int ret = AnvizNew.CKT_RegisterNet(ReaderNo, ReaderIpAddress);
                if (ret == 0)
                {
                    string err_str = "ConnectLAN id:" + ReaderNo + "  ip: "
                        + ReaderIpAddress + " -> Connection Failed";
                    Trace(err_str);
                    EventLogMessage(err_str, EventLogEntryType.Error);
                    Console.WriteLine("ConnectLAN -> " + err_str);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace(ex.Message);
                EventLogMessage("ConnectLAN -> " + ex.Message, EventLogEntryType.Error);
                Console.WriteLine("ConnectLAN -> " + ex.Message);
                return false;
            }
        }

        public static void PutDataToSqlDbTest(ref Dictionary<string, string> input, string con_str)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = con_str;
            con.Open();

            string q = String.Format("DECLARE @result int;" +
                "SET @result = DATEDIFF(s,(SELECT MAX(time) FROM AnvizCheckins WHERE p_id = @person_id), @time); "+
                "IF EXISTS(SELECT @result where @result  > 3 OR @result is null) " +
                "INSERT INTO AnvizCheckins(p_id, time, stat, s_id) " +
                "VALUES(@person_id, @time, @stat, @scanner_id)");
            try
            {
                using (SqlCommand cmd = new SqlCommand(q, con))
                {
                    foreach (KeyValuePair<string, string> cursor in input)
                    {
                        cmd.Parameters.AddWithValue("@" + cursor.Key, cursor.Value);
                    }
                    
                    int affected_rows = cmd.ExecuteNonQuery();
                    if (affected_rows > 0)
                    {
                        AnvizLibraryClass.Trace("Affected rows: " + affected_rows);
                        Console.WriteLine("Affected rows: {0}", affected_rows);
                    }
                }
            }
            catch (SqlException e)
            {
                Trace("PutDataToSqlDbTest -> " + e.Message);
                EventLogMessage("PutDataToSqlDbTest -> " + e.Message, EventLogEntryType.Error);
                Console.WriteLine("PutDataToSqlDbTest -> " + e.Message);
            }
            con.Close();
        }

        public static void LogTxt(string input)
        {
            StreamWriter err_log = new StreamWriter("Log_TestConsole_Anviz.txt", true);
            err_log.WriteLine("{0} | {1}", DateTime.Now, input);
            err_log.Close();
        }

        public static void GetDataFromDevice()
        {
            int pLongRun = new int();
            if (AnvizNew.CKT_GetClockingNewRecordEx(ReaderNo, ref pLongRun) != 1)
            {
                return;
            }
            while (true)
            {
                int RecordCount = new int();
                int RetCount = new int();
                int pClockings = new int();
                int ret = AnvizNew.CKT_GetClockingRecordProgress(pLongRun,
                    ref RecordCount, ref RetCount, ref pClockings);
                if (ret != 0)
                {

                    int ptemp = Marshal.SizeOf(clocking);
                    int tempptr = pClockings;

                    for (int i = 0; i < RetCount; i++)
                    {
                        RtlMoveMemory(ref clocking, pClockings, ptemp);
                        pClockings = pClockings + ptemp;
                        if (clocking.PersonID < 0)
                        {
                            continue;
                        }
                        Dictionary<string, string> output = new Dictionary<string, string>();
                        output.Add("person_id", clocking.PersonID.ToString());
                        output.Add("time", Encoding.Default.GetString(clocking.Time).ToString());
                        output.Add("stat", clocking.Stat.ToString());
                        output.Add("scanner_id", clocking.ID.ToString());
                        PutDataToSqlDbTest(ref output, con_str); //!
                    }
                    if (tempptr != 0)
                    {
                        AnvizNew.CKT_FreeMemory(tempptr);
                    }
                    if (ret == 1)
                    {
                        break;
                    }
                }
            }
        }

        protected static void EventLogMessage(String message, EventLogEntryType type)
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
        public static void Trace(String s)
        {
            String mess = String.Format("Th {0}| Time {1}| {2}",
                Thread.CurrentThread.ManagedThreadId, DateTime.Now, s);
            System.Diagnostics.Trace.WriteLine(mess);
        }

    }

}
