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

## Requirement
1. Windows Vista SP2 or Windows 7 SP1 or later  
2. .Net Framework 4.5.1  

## Licence
Public Domain

## Author
[AonaSuzutsuki](https://github.com/AonaSuzutsuki)
