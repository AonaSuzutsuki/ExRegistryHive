using ExRegistryHiveLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExRegistryHiveSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var hivename = "test_hive";
            var subKeyName = string.Format("{0}\\{1}", hivename, "test_value");

            using (var registryHive = new ExRegistryHiveLib.RegistryHive("ex.hive", hivename, ExRegistryKey.HKEY_USERS))
            {
                using (var exBaseKey = registryHive.OpenKey(subKeyName))
                {
                    exBaseKey.SetValue("test_int", 12);
                }

                using (var baseKey = Registry.Users.OpenSubKey(subKeyName))
                {
                    Console.WriteLine("{0}: {1}", "test_dward", baseKey.GetValue("test_dward").ToString());
                    Console.WriteLine("{0}: {1}", "test_int", baseKey.GetValue("test_int").ToString());
                }
            }

            //if (!ExRegistry.ExLoadHive(hivename, "ex.hive", ExRegistry.ExRegistryKey.HKEY_USERS))
            //{
            //    Console.WriteLine("Failed to load.");
            //}
            //else
            //{
            //    try
            //    {

            //        using (var baseKey = Registry.Users.OpenSubKey(subKeyName))
            //        {
            //            var subkey = ExRegistry.ExOpenKey(ExRegistry.ExRegistryKey.HKEY_USERS, subKeyName);
            //            ExRegistry.ExSetValue("test_int", 12, subkey, RegistryValueKind.DWord);
            //            Console.WriteLine("{0}: {1}", "test_dward", baseKey.GetValue("test_dward").ToString());
            //            Console.WriteLine("{0}: {1}", "test_int", baseKey.GetValue("test_int").ToString());
            //            ExRegistry.ExCloseKey(subkey);
            //        }
            //    }
            //    finally
            //    {
            //        if (!ExRegistry.ExUnloadHive(hivename, ExRegistry.ExRegistryKey.HKEY_USERS))
            //            Console.WriteLine("Failed to unload.");
            //    }
            //}

            Console.ReadLine();
        }
    }
}
