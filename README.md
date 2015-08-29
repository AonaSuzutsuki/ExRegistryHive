ExRegistryHive
====

Overview

## Description
This project is that it load RegistryHive with C#.

## Demo
.Build and Debug on Visual Studio  
1. Load ExRegistryHive.sln.  
2. Start Debug.  
3. Click LoadHive and select "SOFTWARE" registry file.  
4. Click GetReg and show ProductName.  
5. Click UnLoadHive.  
  
.Build and Debug using cmd.exe  
1. cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319  
  (cd C:\Windows\Microsoft.NET\Framework\v4.0.30319)  
2. msbuild.exe /p:Configuration=Debug "[SavedPath]\RegistryHive\ExRegistryHive.csproj"  
3. [SavedPath]\RegistryHive\bin\Debug\RegistryHive.exe  
4. Click LoadHive and select "SOFTWARE" registry file.  
5. Click GetReg and show ProductName.  
6. Click UnLoadHive.

## Usage
.`LoadHive(string hivename, string filename, ExRegistryKey rkey)`  
Load RegistryHive from file.  
*hivename* = When it load registryhive, it is imported with this name.  
*filename* = Filepath of Registryhive.  
*rkey* = It load Registryhive in this.  
 -> ExRegistry.ExRegistryKey.HKEY_CLASSES_ROOT  
 -> ExRegistry.ExRegistryKey.HKEY_CURRENT_USER  
 -> ExRegistry.ExRegistryKey.HKEY_LOCAL_MACHINE  
 -> ExRegistry.ExRegistryKey.HKEY_USERS  
 -> ExRegistry.ExRegistryKey.HKEY_CURRENT_CONFIG  
 
 .`UnLoadHive(string hivename, ExRegistryKey rkey)`  
 *hivename* = It unload registryhive of this name. 
 *rkey* = It unload Registryhive from this.  
 -> ExRegistry.ExRegistryKey.HKEY_CLASSES_ROOT  
 -> ExRegistry.ExRegistryKey.HKEY_CURRENT_USER  
 -> ExRegistry.ExRegistryKey.HKEY_LOCAL_MACHINE  
 -> ExRegistry.ExRegistryKey.HKEY_USERS  
 -> ExRegistry.ExRegistryKey.HKEY_CURRENT_CONFIG  


.Get a key from Registryhive  
`RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(hivename + "\\Microsoft\\Windows NT\\CurrentVersion");`  
`Console.WriteLine("ProductName : " + baseKey.GetValue("ProductName"));`  
`baseKey.Close();`  
Keyname is "[hivename]\\[Keypath]".  
This is about the same as the normal way.  

## Requirement
1. Windows Vista SP2 or Windows 7 SP1 or later  
2. .Net Framework 4.5.1  

## Licence
Public Domain

## Author
[AonaSuzutsuki](https://github.com/AonaSuzutsuki)
