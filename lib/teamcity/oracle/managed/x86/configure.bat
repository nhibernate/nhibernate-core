@ECHO OFF
REM
REM configure.bat
REM
REM This .bat file configures ODP.NET, Managed Driver
REM

REM determine if the configuration is on a 32-bit or 64-bit OS
set ODAC_CFG_PREFIX=Wow6432Node\
if (%PROCESSOR_ARCHITECTURE%) == (x86) if (%PROCESSOR_ARCHITEW6432%) == () set ODAC_CFG_PREFIX=

  
REM Configure machine.config for ODP.NET, Managed Driver's configuration file section handler and client factory
echo.
echo OraProvCfg /action:config /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" /set:settings\TNS_ADMIN:"%~dp0..\..\..\network\admin"
OraProvCfg /action:config /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" /set:settings\TNS_ADMIN:"%~dp0..\..\..\network\admin" 


REM Configure machine.config for ODP.NET, Managed Driver's Performance Counter
echo.
echo OraProvCfg /action:register /product:odpm /component:perfcounter /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" 
OraProvCfg /action:register /product:odpm /component:perfcounter /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" 


REM Place the ODP.NET, Managed Driver assemblies into the GAC
echo.
echo OraProvCfg /action:gac /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll"        
OraProvCfg /action:gac /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll"        
echo.
echo OraProvCfg /action:gac /providerpath:"Oracle.ManagedDataAccessDTC.dll" 
OraProvCfg /action:gac /providerpath:"Oracle.ManagedDataAccessDTC.dll" 


REM Enable intelli-sense for ODP.NET, Managed Provider configuration section
echo.
echo xcopy ..\common\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd "%VS100COMNTOOLS%\..\..\Xml\Schemas" /R /Y
if defined VS100COMNTOOLS xcopy ..\common\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd "%VS100COMNTOOLS%\..\..\Xml\Schemas" /R /Y
echo xcopy ..\common\Oracle.ManagedDataAccess.Catalog.xml "%VS100COMNTOOLS%\..\..\Xml\Schemas" /R /Y
if defined VS100COMNTOOLS xcopy ..\common\Oracle.ManagedDataAccess.Catalog.xml "%VS100COMNTOOLS%\..\..\Xml\Schemas" /R /Y
echo.
echo xcopy ..\common\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd "%VS110COMNTOOLS%\..\..\Xml\Schemas" /R /Y
if defined VS110COMNTOOLS xcopy ..\common\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd "%VS110COMNTOOLS%\..\..\Xml\Schemas" /R /Y
echo xcopy ..\common\Oracle.ManagedDataAccess.Catalog.xml "%VS110COMNTOOLS%\..\..\Xml\Schemas" /R /Y
if defined VS110COMNTOOLS xcopy ..\common\Oracle.ManagedDataAccess.Catalog.xml "%VS110COMNTOOLS%\..\..\Xml\Schemas" /R /Y


REM Add a registry entry for enabling event logs
echo.
echo reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\Oracle Data Provider for .NET, Managed Driver" /v EventMessageFile /t REG_EXPAND_SZ /d %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll /f
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\Oracle Data Provider for .NET, Managed Driver" /v EventMessageFile /t REG_EXPAND_SZ /d %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll /f


REM Create a registry entry to add managed assembly in the Add Reference Dialog box in VS.NET
echo.
echo reg add "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /ve /t REG_SZ /d %~dp0..\common /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /ve /t REG_SZ /d %~dp0..\common /f
