using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace ExRegistryHiveLib
{
    public enum ExRegistryKey : uint
    {
        //HKEY_CLASSES_ROOT = 0x80000000,
        //HKEY_CURRENT_USER = 0x80000001,
        HKEY_LOCAL_MACHINE = 0x80000002,
        HKEY_USERS = 0x80000003,
        //HKEY_CURRENT_CONFIG = 0x80000005
    }

    public class ExRegistry
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegLoadKey(uint hKey, string lpSubKey, string lpFile);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegUnLoadKey(uint hKey, string lpSubKey);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegSetValueEx(IntPtr hKey, string lpValueName, int Reserved, int dwType, IntPtr lpData, int cbData);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, ref IntPtr phkResult);
        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(IntPtr hKey);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentProcess();
        [DllImport("advapi32.dll")]
        private static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int tokenhandle);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int handle);
        [DllImport("advapi32.dll")]
        private static extern int AdjustTokenPrivileges(int tokenhandle, bool disableprivs, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, IntPtr PreivousState, int Returnlength);
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
            public int PrivilegeCount;
            public LUID Luid;
            public int Attributes;
        }

        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;

        private const int KEY_ALL_ACCESS = 0xF003F;
        private const int KEY_CREATE_LINK = 0x0020;
        private const int KEY_CREATE_SUB_KEY = 0x0004;
        private const int KEY_ENUMERATE_SUB_KEYS = 0x0008;
        private const int KEY_EXECUTE = 0x20019;
        private const int KEY_NOTIFY = 0x0010;
        private const int KEY_QUERY_VALUE = 0x0001;
        private const int KEY_READ = 0x20019;
        private const int KEY_SET_VALUE = 0x0002;
        private const int KEY_WOW64_32KEY = 0x0200;
        private const int KEY_WOW64_64KEY = 0x0100;
        private const int KEY_WRITE = 0x20006;

        /// <summary>
        /// Load RegistryHive from file.
        /// </summary>
        /// <param name="hivename">RegistryHive name</param>
        /// <param name="filepath">RegistyHive filepath</param>
        /// <param name="rkey">Registrykey</param>
        /// <returns>When loading is failed, return false. When loading is succeeded, return true.</returns>
        public static bool ExLoadHive(string hivename, string filepath, ExRegistryKey rkey)
        {
            int tokenHandle = 0;
            OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, ref tokenHandle);

            //LUID sebLuid = new LUID();
            //LookupPrivilegeValue(null, "SeBackupPrivilege", ref sebLuid);
            //TOKEN_PRIVILEGES sebTokenp = new TOKEN_PRIVILEGES
            //{
            //    PrivilegeCount = 1,
            //    Luid = sebLuid,
            //    Attributes = SE_PRIVILEGE_ENABLED
            //};
            //AdjustTokenPrivileges(tokenHandle, false, ref sebTokenp, 0, IntPtr.Zero, 0);

            LUID serLuid = new LUID();
            LookupPrivilegeValue(null, "SeRestorePrivilege", ref serLuid);
            TOKEN_PRIVILEGES serTokenp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Luid = serLuid,
                Attributes = SE_PRIVILEGE_ENABLED
            };
            AdjustTokenPrivileges(tokenHandle, false, ref serTokenp, 0, IntPtr.Zero, 0);
            CloseHandle(tokenHandle);

            int rtn = RegLoadKey((uint)rkey, hivename, filepath);

            return rtn == 0;
        }

        public static IntPtr ExOpenKey(ExRegistryKey rkey, string subKeyName)
        {
            IntPtr ptr = IntPtr.Zero;
            RegOpenKeyEx(new IntPtr((int)rkey), subKeyName, 0, 0x20006, ref ptr);
            return ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="rkey"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExSetValue(string key, int value, IntPtr rkey, RegistryValueKind type)
        {
            var size = Marshal.SizeOf(typeof(int));
            var pData = Marshal.AllocHGlobal(size);
            Marshal.WriteInt32(pData, value);

            int rtn = RegSetValueEx(rkey, key, 0, (int)type, pData, size);
            return rtn == 0;
        }

        /// <summary>
        /// Close registry key.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>When closing is failed, return false. When is succeeded, return true.</returns>
        public static bool ExCloseKey(IntPtr key)
        {
            int rtn = RegCloseKey(key);
            return rtn == 0;
        }

        /// <summary>
        /// Unload RegistryHive.
        /// </summary>
        /// <param name="hivename">RegistryHive name</param>
        /// <param name="rkey">Registrykey</param>
        /// <returns>When unloading is failed, return false. When is succeeded, return true.</returns>
        public static bool ExUnloadHive(string hivename, ExRegistryKey rkey)
        {
            int rtn = RegUnLoadKey((uint)rkey, hivename);

            return rtn == 0;
        }
    }
}
