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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(string key, string value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetValue(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetStringValue(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueName"></param>
        void DeleteValue(string valueName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
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
            var ptr = ExOpenSubKey(targetKey, subKeyName);
            if (!ptr.Equals(IntPtr.Zero))
                return new SubKey(ptr);
            return null;
        }

        public ISubKey CreateKey(string subKeyName)
        {
            var ptr = ExCreateSubKey(targetKey, subKeyName);
            if (!ptr.Equals(IntPtr.Zero))
                return new SubKey(ptr);
            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (!ExUnloadHive(hiveName, targetKey))
                    throw new FailedToUnLoadHiveException();

                disposedValue = true;
            }
        }

        ~RegistryHive()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
