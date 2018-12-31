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
            if (!ExRegistry.LoadHive(hivename, "ex.hive", ExRegistry.ExRegistryKey.HKEY_USERS))
            {
                Console.WriteLine("Failed to load.");
            }
            else
            {
                using (var baseKey = Registry.Users.OpenSubKey(string.Format("{0}\\{1}", hivename, "test_value")))
                {
                    var dwardStr = baseKey.GetValue("test_dward").ToString();
                    Console.WriteLine("{0}: {1}", "test_dward", dwardStr);
                }

                if (!ExRegistry.UnloadHive(hivename, ExRegistry.ExRegistryKey.HKEY_USERS))
                    Console.WriteLine("Failed to unload.");
            }
            
            Console.ReadLine();
        }
    }
}
