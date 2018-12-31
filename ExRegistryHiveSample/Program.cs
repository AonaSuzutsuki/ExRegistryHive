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
            var subKeyName2 = string.Format("{0}\\{1}", hivename, "test_value2");

            using (var registryHive = new ExRegistryHiveLib.RegistryHive("ex.hive", hivename, ExRegistryKey.HKEY_USERS))
            {
                using (var exBaseKey = registryHive.OpenKey(subKeyName))
                {
                    exBaseKey.SetValue("test_int", 12);
                    exBaseKey.SetValue("test_str", "test");

                    Console.WriteLine(subKeyName);
                    Console.WriteLine(" {0}: {1}", "test_dward", exBaseKey.GetValue("test_dward").ToString());
                    Console.WriteLine(" {0}: {1}", "test_int", exBaseKey.GetValue("test_int").ToString());
                    Console.WriteLine(" {0}: {1}", "test_str", exBaseKey.GetStringValue("test_str").ToString());

                    exBaseKey.DeleteValue("test_str");
                }

                using (var exBaseKey = registryHive.CreateKey(subKeyName2))
                {
                    exBaseKey.SetValue("test_int", 12);
                    exBaseKey.SetValue("test_str", "test");

                    Console.WriteLine(subKeyName2);
                    Console.WriteLine(" {0}: {1}", "test_int", exBaseKey.GetValue("test_int").ToString());
                    Console.WriteLine(" {0}: {1}", "test_str", exBaseKey.GetStringValue("test_str").ToString());
                }
                
                using (var exBaseKey = registryHive.CreateKey(hivename))
                {
                    exBaseKey.DeleteKey("test_value2");
                }
            }

            Console.ReadLine();
        }
    }
}
