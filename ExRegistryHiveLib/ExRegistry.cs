using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;

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

    internal class ExRegistry
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegLoadKey(uint hKey, string lpSubKey, string lpFile);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegUnLoadKey(uint hKey, string lpSubKey);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegSetValueEx(IntPtr hKey, string lpValueName, int Reserved, int dwType, IntPtr lpData, int cbData);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref RegistryValueKind lpType, IntPtr lpData, ref int lpcbData);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref RegistryValueKind lpType, StringBuilder lpData, ref int lpcbData);

        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegDeleteValue(IntPtr hKey, string lpValueName);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegDeleteKey(IntPtr hKey, string lpSubKey);

        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, ref IntPtr phkResult);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int RegCreateKeyEx(IntPtr hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, int samDesired, IntPtr lpSecurityAttributes, ref IntPtr phkResult, ref int lpdwDisposition);
        [DllImport("advapi32.dll")]
        private static extern int RegCloseKey(IntPtr hKey);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();
        [DllImport("advapi32.dll")]
        private static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr tokenhandle);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi)]
        private static extern int LookupPrivilegeValue(string lpsystemname, string lpname, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);
        [DllImport("advapi32.dll")]
        private static extern int AdjustTokenPrivileges(IntPtr tokenhandle, bool disableprivs, [MarshalAs(UnmanagedType.Struct)]ref TOKEN_PRIVILEGES Newstate, int bufferlength, IntPtr PreivousState, int Returnlength);
        

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

        private const int REG_OPTION_NON_VOLATILE = 0x00000000;

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
            if (!EnabledPrivilege())
                return false;

            return RegLoadKey((uint)rkey, hivename, filepath) == 0;
        }

        private static bool EnabledPrivilege()
        {
            IntPtr tokenHandle = IntPtr.Zero;
            if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, ref tokenHandle) == 0)
                return false;
            
            LUID serLuid = new LUID();
            if (LookupPrivilegeValue(null, "SeRestorePrivilege", ref serLuid) == 0)
            {
                CloseHandle(tokenHandle);
                return false;
            }

            TOKEN_PRIVILEGES serTokenp = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Luid = serLuid,
                Attributes = SE_PRIVILEGE_ENABLED
            };
            if (AdjustTokenPrivileges(tokenHandle, false, ref serTokenp, 0, IntPtr.Zero, 0) == 0)
            {
                CloseHandle(tokenHandle);
                return false;
            }

            CloseHandle(tokenHandle);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rkey"></param>
        /// <param name="subKeyName"></param>
        /// <returns></returns>
        public static IntPtr ExOpenKey(ExRegistryKey rkey, string subKeyName)
        {
            var ptr = IntPtr.Zero;
            RegOpenKeyEx(new IntPtr((int)rkey), subKeyName, 0, KEY_ALL_ACCESS, ref ptr);
            return ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rkey"></param>
        /// <param name="subKeyName"></param>
        /// <returns></returns>
        public static IntPtr ExCreateKey(ExRegistryKey rkey, string subKeyName)
        {
            int lpdwDisposition = 0; // REG_CREATED_NEW_KEY(0x00000001L)  REG_OPENED_EXISTING_KEY(0x00000002L)
            var ptr = IntPtr.Zero;
            RegCreateKeyEx(new IntPtr((int)rkey), subKeyName, 0, null, REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, IntPtr.Zero, ref ptr, ref lpdwDisposition);
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
        public static bool ExSetValue(string key, int value, IntPtr rkey)
        {
            var size = Marshal.SizeOf(value.GetType());
            var pData = Marshal.AllocHGlobal(size);
            Marshal.WriteInt32(pData, value);

            var rtn = RegSetValueEx(rkey, key, 0, (int)RegistryValueKind.DWord, pData, size);
            Marshal.Release(pData);
            return rtn == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public static bool ExSetValue(string key, string value, IntPtr rkey)
        {
            var size = value.Length + 1;
            var pData = Marshal.StringToHGlobalAnsi(value);

            var rtn = RegSetValueEx(rkey, key, 0, (int)RegistryValueKind.String, pData, size);
            Marshal.Release(pData);
            return rtn == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public static int ExGetInt32Value(string key, IntPtr rkey)
        {
            RegistryValueKind lpType = 0;
            var ptr = Marshal.AllocHGlobal(4);
            int size = 4;
            RegQueryValueEx(rkey, key, 0, ref lpType, ptr, ref size);

            var rtn = Marshal.ReadInt32(ptr);
            Marshal.Release(ptr);
            return rtn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public static string ExGetStringValue(string key, IntPtr rkey)
        {
            RegistryValueKind lpType = 0;
            var sb = new StringBuilder(1024);
            int size = 64;
            RegQueryValueEx(rkey, key, 0, ref lpType, sb, ref size);
            
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public static bool ExDeleteValue(string valueName, IntPtr rkey)
        {
            return RegDeleteValue(rkey, valueName) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public static bool ExDeleteKey(string keyName, IntPtr rkey)
        {
            return RegDeleteKey(rkey, keyName) == 0;
        }

        /// <summary>
        /// Close registry key.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <returns>When closing is failed, return false. When is succeeded, return true.</returns>
        public static bool ExCloseKey(IntPtr key)
        {
            return RegCloseKey(key) == 0;
        }

        /// <summary>
        /// Unload RegistryHive.
        /// </summary>
        /// <param name="hivename">RegistryHive name</param>
        /// <param name="rkey">Registrykey</param>
        /// <returns>When unloading is failed, return false. When is succeeded, return true.</returns>
        public static bool ExUnloadHive(string hivename, ExRegistryKey rkey)
        {
            return RegUnLoadKey((uint)rkey, hivename) == 0;
        }
    }
}
