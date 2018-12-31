using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExRegistryHiveLib.ExRegistry;

namespace ExRegistryHiveLib
{
    public class FailedToLoadHiveException : Exception
    {

    }

    public class FailedToUnLoadHiveException : Exception
    {

    }

    public class FailedToCloseKeyException : Exception
    {

    }



    public interface ISubKey : IDisposable
    {
        void SetValue(string key, int value);
        void SetValue(string key, string value);
        int GetValue(string key);
        string GetStringValue(string key);
        void DeleteValue(string valueName);
        void DeleteKey(string keyName);
    }

    public class SubKey : ISubKey
    {

        public SubKey(IntPtr hkey)
        {
            Hkey = hkey;
        }
        
        public IntPtr Hkey { get; }

        public void SetValue(string key, int value)
        {
            ExSetValue(key, value, Hkey);
        }

        public void SetValue(string key, string value)
        {
            ExSetValue(key, value, Hkey);
        }

        public int GetValue(string key)
        {
            return ExGetInt32Value(key, Hkey);
        }

        public string GetStringValue(string key)
        {
            return ExGetStringValue(key, Hkey);
        }

        public void DeleteValue(string valueName)
        {
            ExDeleteValue(valueName, Hkey);
        }

        public void DeleteKey(string keyName)
        {
            ExDeleteKey(keyName, Hkey);
        }

        public void Dispose()
        {
            if (!ExCloseKey(Hkey))
                throw new FailedToCloseKeyException();
        }
    }

    public class RegistryHive : IDisposable
    {
        private RegistryHive() { }

        public RegistryHive(string hiveFilePath, string hiveName, ExRegistryKey targetKey)
        {
            this.hiveName = hiveName;
            this.targetKey = targetKey;


            if (!ExLoadHive(hiveName, hiveFilePath, targetKey))
                throw new FailedToLoadHiveException();
        }

        private readonly string hiveName;
        private readonly ExRegistryKey targetKey;

        public ISubKey OpenKey(string subKeyName)
        {
            var ptr = ExOpenKey(targetKey, subKeyName);
            if (!ptr.Equals(IntPtr.Zero))
                return new SubKey(ptr);
            return null;
        }

        public ISubKey CreateKey(string subKeyName)
        {
            var ptr = ExCreateKey(targetKey, subKeyName);
            if (!ptr.Equals(IntPtr.Zero))
                return new SubKey(ptr);
            return null;
        }

        public void Dispose()
        {
            if (!ExUnloadHive(hiveName, targetKey))
                throw new FailedToUnLoadHiveException();
        }
    }
}
