using System;
using System.Collections.Generic;
using System.IO;
using AnvizLibrary;
using System.Data.SqlClient;
using System.Collections.Specialized;
using TimeAttendance;

namespace TestConsole
{
    class Program
    {
        public static string con_str = AnvizProject.Properties.Settings.Default.con_str;
        public static StringCollection devices = AnvizProject.Properties.Settings.Default.devices;
        public static void TestConnection(string con_str) // Тест соединения к БД
        {
            using (SqlConnection con = new SqlConnection())
            {
                Console.WriteLine("MSSQL Credentials: {0}", con_str);
                try
                {
                    con.ConnectionString = con_str;
                    con.Open();
                    Console.WriteLine("Connection opened");
                }
                catch (SqlException e)
                {
                    string err_str = "TestConnection-> Failed. " + e.Message;
                    Console.WriteLine(err_str);
                    AnvizLibraryClass.LogTxt(err_str);
                }

            }

        }
        
        static void Main(string[] args)
        {
            TestConnection(con_str);
            Dictionary<string, string> test_input = new Dictionary<string, string>(4);
            test_input.Add("person_id", "test_id1");
            test_input.Add("time", "2015-01-01 21:01:01:000");
            test_input.Add("stat", "test_stat");
            test_input.Add("scanner_id", "test_id");
            /*Это тестовый ассоциативник, который запишется только один раз.
            У нас n устройств, значит, следующий тест запишет в БД только одну запись,
            остальные проигнорирует, так как разница дат < 3 секунд
            person_id И scanner_id в бд - varchar, поэтому для удобства они здесь
            не числовые
            */
            for (int i = 0; i < devices.Count; i++)
            {
                //string ip = devices[i].Substring(devices[i].IndexOf(':') + 1);
                //int id = Convert.ToInt32(devices[i].Remove(devices[i].IndexOf(":")));
                //if (!AnvizLibraryClass.ConnectLAN(ip, id))
                //{
                //    continue;
                //}
                //AnvizLibraryClass.GetDataFromDevice();
                //AnvizNew.CKT_Disconnect();
                AnvizLibraryClass.PutDataToSqlDbTest(ref test_input, con_str);
                                
            }
            Console.WriteLine("End of the test. Press any key");
            Console.ReadKey();
        }
    }
}
