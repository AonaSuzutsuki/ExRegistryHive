using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ExRegistryHiveLib
{
    public class ExRegistry
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegLoadKey(uint hKey, string lpSubKey, string lpFile);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegUnLoadKey(uint hKey, string lpSubKey);
        [DllImport("advapi32.dll")]
        private static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int tokenhandle);
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentProcess();
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int handle);
        [DllImport("advapi32.dll")]
        private static extern int AdjustTokenPrivileges(int tokenhandle, bool disableprivs, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, int PreivousState, int Returnlength);
        [DllImport("advapi32.dll")]
        private static extern int LookupPrivilegeValue(string lpsystemname, string lpname, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);


        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public int LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public UInt32 Attributes;
        }

        static int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        static uint SE_PRIVILEGE_ENABLED = 0x00000002;
        static int TOKEN_QUERY = 0x00000008;

        public enum ExRegistryKey : uint
        {
            //HKEY_CLASSES_ROOT = 0x80000000,
            //HKEY_CURRENT_USER = 0x80000001,
            HKEY_LOCAL_MACHINE = 0x80000002,
            HKEY_USERS = 0x80000003,
            //HKEY_CURRENT_CONFIG = 0x80000005
        }

        /// <summary>
        /// Load RegistryHive from file.
        /// </summary>
        /// <param name="hivename">RegistryHive name</param>
        /// <param name="filepath">RegistyHive filepath</param>
        /// <param name="rkey">Registrykey</param>
        /// <returns>When loading is failed, return false. When loading is succeeded, return true.</returns>
        public static bool LoadHive(string hivename, string filepath, ExRegistryKey rkey)
        {
            int tokenHandle = 0;
            OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle);

            LUID sebLuid = new LUID();
            LookupPrivilegeValue(null, "SeBackupPrivilege", ref sebLuid);
            TOKEN_PRIVILEGES sebTokenp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Privileges = new LUID_AND_ATTRIBUTES[]
                {
                    new LUID_AND_ATTRIBUTES()
                    {
                        Luid = sebLuid,
                        Attributes = SE_PRIVILEGE_ENABLED
                    }
                }
            };
            AdjustTokenPrivileges(tokenHandle, false, ref sebTokenp, 0, 0, 0);

            LUID serLuid = new LUID();
            LookupPrivilegeValue(null, "SeRestorePrivilege", ref serLuid);
            TOKEN_PRIVILEGES serTokenp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Privileges = new LUID_AND_ATTRIBUTES[]
                {
                    new LUID_AND_ATTRIBUTES()
                    {
                        Luid = serLuid,
                        Attributes = SE_PRIVILEGE_ENABLED
                    }
                }
            };
            AdjustTokenPrivileges(tokenHandle, false, ref serTokenp, 0, 0, 0);
            CloseHandle(tokenHandle);

            int rtn = RegLoadKey((uint)rkey, hivename, filepath);

            return rtn == 0;
        }

        /// <summary>
        /// Unload RegistryHive.
        /// </summary>
        /// <param name="hivename">RegistryHive name</param>
        /// <param name="rkey">Registrykey</param>
        /// <returns>When unloading is failed, return false. When unloading is succeeded, return true.</returns>
        public static bool UnloadHive(string hivename, ExRegistryKey rkey)
        {
            int rtn = RegUnLoadKey((uint)rkey, hivename);

            return rtn == 0;
        }
    }
}
