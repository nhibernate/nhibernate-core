@echo off
pushd %~dp0

set NANT="%~dp0Tools\nant\bin\NAnt.exe" -t:net-4.0
set BUILDTOOL="%~dp0Tools/BuildTool/BuildTool.csproj"
set AVAILABLE_CONFIGURATIONS=%~dp0available-test-configurations
set CURRENT_CONFIGURATION=%~dp0current-test-configuration
set NUNIT="%~dp0Tools\NUnit.ConsoleRunner.3.7.0\tools\nunit3-console.exe"

:main-menu
echo ========================= NHIBERNATE BUILD MENU ==========================
echo --- TESTING ---
echo A. (Step 1) Set up a new test configuration for a particular database.
echo B. (Step 2) Activate a test configuration.
echo C. (Step 3) Run tests using active configuration with 32bits runner (Needs built in Visual Studio).
echo D.       Or run tests using active configuration with 64bits runner (Needs built in Visual Studio).
echo.
echo --- BUILD ---
echo E. Build NHibernate (Debug)
echo F. Build NHibernate (Release)
echo G. Build Release Package (Also runs tests and creates documentation)
echo.
echo --- Code generation ---
echo H. Generate async code (Generates files in Async sub-folders)
echo.
echo --- TeamCity (CI) build options
echo I. TeamCity build menu
echo.
echo --- Exit ---
echo X. Make the beautiful build menu go away.
echo.

dotnet run %BUILDTOOL% prompt ABCDEFGHIX -c Release --no-build
if errorlevel 9 goto end
if errorlevel 8 goto teamcity-menu
if errorlevel 7 goto build-async
if errorlevel 6 goto build-release-package
if errorlevel 5 goto build-release
if errorlevel 4 goto build-debug
if errorlevel 3 goto test-run-64
if errorlevel 2 goto test-run-32
if errorlevel 1 goto test-activate
if errorlevel 0 goto test-setup-menu

:test-setup-menu
echo A. Add a test configuration for SQL Server.
echo B. Add a test configuration for Firebird.
echo C. Add a test configuration for SQLite.
echo D. Add a test configuration for PostgreSQL.
echo E. Add a test configuration for Oracle.
echo F. Add a test configuration for Oracle with managed driver.
echo G. Add a test configuration for SQL Server Compact.
echo H. Add a test configuration for MySql.
echo I. Add a test configuration for SAP HANA.
echo.
echo X.  Exit to main menu.
echo.

dotnet run %BUILDTOOL% prompt ABCDEFGHIX -c Release --no-build
if errorlevel 9 goto main-menu
if errorlevel 8 goto test-setup-hana
if errorlevel 7 goto test-setup-mysql
if errorlevel 6 goto test-setup-sqlserverce
if errorlevel 5 goto test-setup-oracle-managed
if errorlevel 4 goto test-setup-oracle
if errorlevel 3 goto test-setup-postgresql
if errorlevel 2 goto test-setup-sqlite
if errorlevel 1 goto test-setup-firebird
if errorlevel 0 goto test-setup-sqlserver

:test-setup-sqlserver
set CONFIG_NAME=MSSQL
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-sqlserverce
set CONFIG_NAME=SqlServerCe
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-firebird
set CONFIG_NAME=FireBird
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-sqlite
set CONFIG_NAME=SQLite
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-postgresql
set CONFIG_NAME=PostgreSQL
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-mysql
set CONFIG_NAME=MySql
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-oracle
set CONFIG_NAME=Oracle
set TEST_PLATFORM=x86
set LIB_FILES=lib\teamcity\oracle\x86\*.dll
set LIB_FILES2=
goto test-setup-generic

:test-setup-oracle-managed
set CONFIG_NAME=Oracle-Managed
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-hana
set CONFIG_NAME=HANA
set TEST_PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-generic
set CFGNAME=
set /p CFGNAME=Enter a name for your test configuration or press enter to use default name: 
if /I "%CFGNAME%"=="" set CFGNAME=%CONFIG_NAME%-%TEST_PLATFORM%
mkdir "%AVAILABLE_CONFIGURATIONS%\%CFGNAME%"
if /I "%LIB_FILES%"=="" goto test-setup-generic-skip-copy
copy %LIB_FILES% "%AVAILABLE_CONFIGURATIONS%\%CFGNAME%"
if /I "%LIB_FILES2%"=="" goto test-setup-generic-skip-copy
copy %LIB_FILES2% "%AVAILABLE_CONFIGURATIONS%\%CFGNAME%"
:test-setup-generic-skip-copy
copy src\NHibernate.Config.Templates\%CONFIG_NAME%.cfg.xml "%AVAILABLE_CONFIGURATIONS%\%CFGNAME%\hibernate.cfg.xml"
echo Done setting up files.  Starting notepad to edit connection string in file:
echo %AVAILABLE_CONFIGURATIONS%\%CFGNAME%\hibernate.cfg.xml
start notepad "%AVAILABLE_CONFIGURATIONS%\%CFGNAME%\hibernate.cfg.xml"
echo When you're done, don't forget to activate the configuration through the menu.
goto main-menu


:test-activate
dotnet run %BUILDTOOL% pick-folder "%AVAILABLE_CONFIGURATIONS%" folder.tmp "Which test configuration should be activated?" -c Release --no-build
set /p FOLDER=<folder.tmp
del folder.tmp
mkdir "%CURRENT_CONFIGURATION%" 2> nul
del /q "%CURRENT_CONFIGURATION%\*"
copy "%FOLDER%\*" "%CURRENT_CONFIGURATION%"
echo Configuration activated.
goto main-menu

:test-run-32
SET NUNITPLATFORM=--x86
goto test-run

:test-run-64
SET NUNITPLATFORM=
goto test-run

:test-run
start "nunit3-console" cmd /K %NUNIT% %NUNITPLATFORM% --agents=1 --process=separate NHibernate.nunit
goto main-menu

rem :build-test
rem %NANT% test
rem goto main-menu

:build-async
%NANT% generate-async
goto main-menu

:build-debug
%NANT% clean build
echo.
echo Assuming the build succeeded, your results will be in the build folder.
echo.
goto main-menu

:build-release
%NANT% -D:project.config=release clean release
echo.
echo Assuming the build succeeded, your results will be in the build folder.
echo.
goto main-menu

:build-release-package
%NANT% -D:project.config=release clean package nugetpushbat
echo.
echo Assuming the build succeeded, your results will be in the build folder,
echo including NuGet packages and tools to push them.
echo.
goto main-menu

:teamcity-menu
echo.
echo --- TeamCity (CI) build options
echo A. NHibernate Trunk (default SQL Server)
echo B. NHibernate Trunk - Firebird (32-bit)
echo C. NHibernate Trunk - Firebird (64-bit)
echo D. NHibernate Trunk - SQLite (32-bit)
echo E. NHibernate Trunk - SQLite (64-bit)
echo F. NHibernate Trunk - PostgreSQL
echo G. NHibernate Trunk - Oracle (32-bit)
echo H. NHibernate Trunk - Oracle Managed (32-bit)
echo I. NHibernate Trunk - Oracle Managed (64-bit)
echo J. NHibernate Trunk - SQL Server Compact (32-bit)
echo K. NHibernate Trunk - SQL Server Compact (64-bit)
echo L. NHibernate Trunk - SQL Server ODBC (32-bit)
echo.
echo X.  Exit to main menu.
echo.

dotnet run %BUILDTOOL% prompt ABCDEFGHIJKLX -c Release --no-build
if errorlevel 12 goto main-menu
if errorlevel 11 goto teamcity-sqlServerOdbc
if errorlevel 10 goto teamcity-sqlServerCe64
if errorlevel 9 goto teamcity-sqlServerCe32
if errorlevel 8 goto teamcity-oraclemanaged-64
if errorlevel 7 goto teamcity-oraclemanaged-32
if errorlevel 6 goto teamcity-oracle32
if errorlevel 5 goto teamcity-postgresql
if errorlevel 4 goto teamcity-sqlite64
if errorlevel 3 goto teamcity-sqlite32
if errorlevel 2 goto teamcity-firebird64
if errorlevel 1 goto teamcity-firebird32
if errorlevel 0 goto teamcity-trunk

:teamcity-trunk
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-firebird32
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=firebird32
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-firebird64
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=firebird64
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-sqlite32
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlite32
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-sqlite64
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlite64
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-postgresql
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=postgresql
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-oracle32
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=oracle32
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-oraclemanaged-32
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=oracle-managed32
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-oraclemanaged-64
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=oracle-managed64
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-sqlServerOdbc
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlServerOdbc
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-sqlServerCe32
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlServerCe32
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:teamcity-sqlServerCe64
move "%CURRENT_CONFIGURATION%" "%CURRENT_CONFIGURATION%-backup" 2> nul
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlServerCe64
move "%CURRENT_CONFIGURATION%-backup" "%CURRENT_CONFIGURATION%" 2> nul
goto main-menu

:end
popd
