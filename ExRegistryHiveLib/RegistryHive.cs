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
        IntPtr Hkey { get; }

        void SetValue(string key, int value);
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
            ExSetValue(key, value, Hkey, Microsoft.Win32.RegistryValueKind.DWord);
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

        public void Dispose()
        {
            if (!ExUnloadHive(hiveName, targetKey))
                throw new FailedToUnLoadHiveException();
        }
    }
}
