@ECHO OFF
REM
REM unconfigure.bat
REM
REM This .bat file unconfigures ODP.NET, Managed Driver
REM

REM determine if the configuration is on a 32-bit or 64-bit OS
set ODAC_CFG_PREFIX=Wow6432Node\
if (%PROCESSOR_ARCHITECTURE%) == (x86) if (%PROCESSOR_ARCHITEW6432%) == () set ODAC_CFG_PREFIX=


REM Unconfigure machine.config for ODP.NET, Managed Driver's configuration file section handler and client factory
echo.
echo OraProvCfg /action:unconfig /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll"
OraProvCfg /action:unconfig /product:odpm /frameworkversion:v4.0.30319 /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll"


REM Unconfigure machine.config for ODP.NET, Managed Driver's Performance Counter
echo.
echo OraProvCfg /action:unregister /product:odpm /component:perfcounter /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" 
OraProvCfg /action:unregister /product:odpm /component:perfcounter /providerpath:"%~dp0..\common\Oracle.ManagedDataAccess.dll" 


REM Remove the ODP.NET, Managed Driver assemblies from the GAC
echo.
echo OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccess, Version=4.121.1.0"   
OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccess, Version=4.121.1.0"     
echo.
echo OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccessDTC, processorArchitecture=x86, Version=4.121.1.0" 
OraProvCfg /action:ungac /providerpath:"Oracle.ManagedDataAccessDTC, processorArchitecture=x86, Version=4.121.1.0" 
 

REM Disable intelli-sense for ODP.NET, Managed Provider configuration section
if EXIST "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd" (
  echo.
  echo del "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd" 
  del "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd"
)
if EXIST "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml" (
  echo.
  echo del "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml" 
  del "%VS100COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml"
)

if EXIST "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd" (
  echo.
  echo del "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd" 
  del "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Client.Configuration.Section.xsd"
)
if EXIST "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml" (
  echo.
  echo del "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml" 
  del "%VS110COMNTOOLS%..\..\Xml\Schemas\Oracle.ManagedDataAccess.Catalog.xml"
)


REM Remove the registry entry for enabling event logs
echo.
echo reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\Application\Oracle Data Provider for .NET, Managed Driver" /f
reg delete "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\Application\Oracle Data Provider for .NET, Managed Driver" /f


REM Delete the registry entry to remove managed assembly in the Add Reference Dialog box in VS.NET
echo.
echo reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /f
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\%ODAC_CFG_PREFIX%Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\odp.net.managed" /f
