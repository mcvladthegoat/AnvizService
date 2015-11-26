using System;
using System.Runtime.InteropServices;

namespace TimeAttendance
{
    public class AnvizNew
    {
        // Consts
        public const long CKT_ERROR_INVPARAM = -1;
        public const long CKT_ERROR_NETDAEMONREADY = -1;
        public const long CKT_ERROR_CHECKSUMERR = -2;
        public const long CKT_ERROR_MEMORYFULL = -1;
        public const long CKT_ERROR_INVFILENAME = -3;
        public const long CKT_ERROR_FILECANNOTOPEN = -4;
        public const long CKT_ERROR_FILECONTENTBAD = -5;
        public const long CKT_ERROR_FILECANNOTCREATED = -2;
        public const long CKT_ERROR_NOTHISPERSON = -1;

        public const long CKT_RESULT_OK = 1;
        public const long CKT_RESULT_ADDOK = 1;
        public const long CKT_RESULT_HASMORECONTENT = 2;

        // Types
        [StructLayout(LayoutKind.Sequential, Size = 26, CharSet = CharSet.Ansi), Serializable]
        public struct NETINFO
        {
            public int ID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] IP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Mask;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Gateway;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] ServerIP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] MAC;
        }

        [StructLayout(LayoutKind.Sequential, Size = 164, CharSet = CharSet.Ansi), Serializable]
        public struct CKT_KQState
        {
            public int num;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 160)]
            public byte[] kqmsg;   //0 To 9, 0 To 15
        }

        [StructLayout(LayoutKind.Sequential, Size = 16, CharSet = CharSet.Ansi), Serializable]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [StructLayout(LayoutKind.Sequential, Size = 18, CharSet = CharSet.Ansi), Serializable]
        public struct DATETIMEINFO
        {
            public int ID;
            public ushort Year;
            public byte Month;
            public byte Day;
            public byte Hour;
            public byte Minute;
            public byte Second;
        }

        [StructLayout(LayoutKind.Sequential, Size = 48, CharSet = CharSet.Ansi), Serializable]
        public struct PERSONINFO
        {
            public int PersonID;            //人员编号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Password;         //密码
            public int CardNo;              //卡号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] Name;             //姓名
            public int Dept;                //部门
            public int Group;               //组号
            public int KQOption;            //考勤比对标志
            //0：独立（指纹，卡，密码都可以）
            //1：卡+指纹
            //2：密码+指纹
            //3：卡+密码
            //4：工号+指纹 
            public int FPMark;              //位0 = 1表示已登记指纹1，位1 = 1表示已登记指纹2
                                            //例如FPMark=3(00000011)表示已登记指纹1和2，
                                            //FPMark=１表示已登记指纹1，FPMark=2表示已登记指纹2
            public int Other;               //特殊信息 =0 普通人员, =1 管理员
        }

        [StructLayout(LayoutKind.Sequential, Size = 100, CharSet = CharSet.Ansi), Serializable]
        public struct PERSONINFOEX
        {
            public int PersonID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Password;
            public int CardNo;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] Name;
            public int Dept;
            public int Group;
            public int KQOption;
            public int FPMark;
            public int Other;
        }

        [StructLayout(LayoutKind.Sequential, Size = 40, CharSet = CharSet.Ansi), Serializable]
        public struct CLOCKINGRECORD
        {
            public int ID;
            public int PersonID;
            public int Stat;
            public int BackupCode;
            public int WorkType;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Time;
        }

        [StructLayout(LayoutKind.Sequential, Size = 24, CharSet = CharSet.Ansi), Serializable]
        public struct CKT_PictureFileHead
        {
            public int ID;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] STime;
        }

        [StructLayout(LayoutKind.Sequential, Size = 60, CharSet = CharSet.Ansi), Serializable]
        public struct DEVICEINFO
        {
            public int ID;
            public int MajorVersion;
            public int MinorVersion;
            public int SpeakerVolume;
            public int Parameter;
            public int DefaultAuth;
            public int FixWGHead;
            public int WGOption;
            public int AutoUpdateAllow;
            public int KQRepeatTime;
            public int RealTimeAllow;
            public int RingAllow;
            public int LockDelayTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] AdminPassword;
        }

        [StructLayout(LayoutKind.Sequential, Size = 12, CharSet = CharSet.Ansi), Serializable]
        public struct RINGTIME
        {
            public int hour;
            public int minute;
            public int week;
        }
        /*
                [StructLayout(LayoutKind.Explicit, Pack = 1)]
                public struct TIMESECT
                {
                    [FieldOffset(0)]
                    public byte bHour;
                    [FieldOffset(1)]
                    public byte bMinute;
                    [FieldOffset(2)]
                    public byte eHour;
                    [FieldOffset(3)]
                    public byte eMinute;
                }
        */

        [StructLayout(LayoutKind.Sequential, Size = 4, CharSet = CharSet.Ansi), Serializable]
        public struct TIMESECT
        {
            public byte bHour;
            public byte bMinute;
            public byte eHour;
            public byte eMinute;
        }

        [StructLayout(LayoutKind.Sequential, Size = 76, CharSet = CharSet.Ansi), Serializable]
        public struct CKT_MessageInfo
        {
            public int PersonID;
            public int sYear;
            public int sMon;
            public int sDay;
            public int eYear;
            public int eMon;
            public int eDay;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] msg;
        }

        [StructLayout(LayoutKind.Sequential, Size = 28, CharSet = CharSet.Ansi), Serializable]
        public struct CKT_MessageHead
        {
            public int PersonID;
            public int sYear;
            public int sMon;
            public int sDay;
            public int eYear;
            public int eMon;
            public int eDay;
        }

        [StructLayout(LayoutKind.Sequential, Size = 108, CharSet = CharSet.Ansi), Serializable]
        public struct GPRSinfo
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] APN;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] ServerIP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Port;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] LocalIP;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] UserName;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 18)]
            public byte[] Password;
            public byte Mode;
            //public byte Reserved;
        }

        // Routines
        //[DllImport("tc400.dll", ExactSpelling = true)] 

        [DllImport("tc400.dll")]
        public static extern int CKT_FreeMemory(int address);


        [DllImport("tc400.dll")]
        public static extern int CKT_RegisterSno(int Sno, int ComPort);

        [DllImport("tc400.dll")]
        public static extern int CKT_RegisterNet(int Sno, String Addr);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetCounts(int Sno, ref int pPersonCount, ref int pFPCount, ref int pClockingsCount);

        [DllImport("tc400.dll")]
        public static extern int CKT_ClearClockingRecord(int Sno, int type, int count);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingRecordEx(int Sno, ref int ppLongRun);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingNewRecordEx(int Sno, ref int ppLongRun);

        [DllImport("tc400.dll")]
        public static extern int CKT_GetClockingRecordProgress(int pLongRun, ref int pRecCount, ref int pRetCount, ref int ppPersons);


        [DllImport("tc400.dll")]
        public static extern void CKT_Disconnect();

        [DllImport("tc400.dll")]
        public static extern int CKT_ReadRealtimeClocking(ref int ppClockings);

    }
}
