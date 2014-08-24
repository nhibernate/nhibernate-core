@echo off
pushd %~dp0

set NANT="%~dp0Tools\nant\bin\NAnt.exe" -t:net-4.0
set BUILDTOOL="%~dp0Tools\BuildTool\bin\Release\BuildTool.exe"
set AVAILABLE_CONFIGURATIONS=%~dp0available-test-configurations
set CURRENT_CONFIGURATION=%~dp0current-test-configuration
set NUNIT="%~dp0Tools\nunit\nunit-x86.exe"

:main-menu
echo ========================= NHIBERNATE BUILD MENU ==========================
echo --- SETUP ---
echo A. Set up for Visual Studio (creates AssemblyInfo.cs files).
echo.
echo --- TESTING ---
echo B. (Step 1) Set up a new test configuration for a particular database.
echo C. (Step 2) Activate a test configuration.
echo D. (Step 3) Run tests using active configuration.
echo.
echo --- BUILD ---
echo E. Build NHibernate (Debug)
echo F. Build NHibernate (Release)
echo G. Build Release Package (Also runs tests and creates documentation)
echo.
echo --- GRAMMAR ---
echo H. Grammar operations (related to Hql.g and HqlSqlWalker.g)
echo.
echo --- TeamCity (CI) build options
echo I. TeamCity build menu
echo.
echo --- Exit ---
echo X. Make the beautiful build menu go away.
echo.

%BUILDTOOL% prompt ABCDEFGHIX
if errorlevel 9 goto end
if errorlevel 8 goto teamcity-menu
if errorlevel 7 goto grammar-menu
if errorlevel 6 goto build-release-package
if errorlevel 5 goto build-release
if errorlevel 4 goto build-debug
if errorlevel 3 goto test-run
if errorlevel 2 goto test-activate
if errorlevel 1 goto test-setup-menu
if errorlevel 0 goto build-visual-studio

:test-setup-menu
echo A. Add a test configuration for SQL Server.
echo B. Add a test configuration for Firebird (x86).
echo C. Add a test configuration for Firebird (x64). [not recommended]
echo D. Add a test configuration for SQLite (x86).
echo E. Add a test configuration for SQLite (x64). [not recommended]
echo F. Add a test configuration for PostgreSQL.
echo G. Add a test configuration for Oracle.
echo H. Add a test configuration for SQL Server Compact (x86).
echo I. Add a test configuration for SQL Server Compact (x64).
echo.
echo X.  Exit to main menu.
echo.

%BUILDTOOL% prompt ABCDEFGHIX
if errorlevel 9 goto main-menu
if errorlevel 8 goto test-setup-sqlservercex64
if errorlevel 7 goto test-setup-sqlservercex86
if errorlevel 6 goto test-setup-oracle
if errorlevel 5 goto test-setup-postgresql
if errorlevel 4 goto test-setup-sqlitex64
if errorlevel 3 goto test-setup-sqlitex86
if errorlevel 2 goto test-setup-firebirdx64
if errorlevel 1 goto test-setup-firebirdx86
if errorlevel 0 goto test-setup-sqlserver

:test-setup-sqlserver
set CONFIG_NAME=MSSQL
set PLATFORM=AnyCPU
set LIB_FILES=
set LIB_FILES2=
goto test-setup-generic

:test-setup-sqlservercex86
set CONFIG_NAME=SqlServerCe32
set PLATFORM=AnyCPU
set LIB_FILES=lib\teamcity\SqlServerCe\*.dll
set LIB_FILES2=lib\teamcity\SqlServerCe\X86\*.dll
goto test-setup-generic

:test-setup-sqlservercex64
set CONFIG_NAME=SqlServerCe64
set PLATFORM=AnyCPU
set LIB_FILES=lib\teamcity\sqlServerCe\*.dll
set LIB_FILES2=lib\teamcity\sqlServerCe\AMD64\*.dll
goto test-setup-generic

:test-setup-firebirdx86
set CONFIG_NAME=FireBird
set PLATFORM=x86
set LIB_FILES=lib\teamcity\firebird\*.dll
set LIB_FILES2=lib\teamcity\firebird\x86\*
goto test-setup-generic

:test-setup-firebirdx64
set CONFIG_NAME=FireBird
set PLATFORM=x64
set LIB_FILES=lib\teamcity\firebird\*.dll
set LIB_FILES2=lib\teamcity\firebird\x64\*
goto test-setup-generic

:test-setup-sqlitex86
set CONFIG_NAME=SQLite
set PLATFORM=x86
set LIB_FILES=lib\teamcity\sqlite\x86\*
set LIB_FILES2=
goto test-setup-generic

:test-setup-sqlitex64
set CONFIG_NAME=SQLite
set PLATFORM=x64
set LIB_FILES=lib\teamcity\sqlite\x64\*
set LIB_FILES2=
goto test-setup-generic

:test-setup-postgresql
set CONFIG_NAME=PostgreSQL
set PLATFORM=AnyCPU
set LIB_FILES=lib\teamcity\postgresql\*.dll
set LIB_FILES2=
goto test-setup-generic

:test-setup-oracle
set CONFIG_NAME=Oracle
set PLATFORM=x86
set LIB_FILES=lib\teamcity\oracle\x86\*.dll
set LIB_FILES2=
goto test-setup-generic

:test-setup-generic
set /p CFGNAME=Enter a name for your test configuration or press enter to use default name: 
if /I "%CFGNAME%"=="" set CFGNAME=%CONFIG_NAME%-%PLATFORM%
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
%BUILDTOOL% pick-folder "%AVAILABLE_CONFIGURATIONS%" folder.tmp "Which test configuration should be activated?"
set /p FOLDER=<folder.tmp
del folder.tmp
mkdir "%CURRENT_CONFIGURATION%" 2> nul
del /q "%CURRENT_CONFIGURATION%\*"
copy "%FOLDER%\*" "%CURRENT_CONFIGURATION%"
echo Configuration activated.
goto main-menu

:test-run
start "" %NUNIT% NHibernate.nunit
goto main-menu

rem :build-test
rem %NANT% test
rem goto main-menu

:build-visual-studio
%NANT% visual-studio
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

:grammar-menu
echo.
echo --- GRAMMAR ---
echo A. Regenerate all grammars.
echo        Hql.g           to  HqlLexer.cs
echo        Hql.g           to  HqlParser.cs
echo        HqlSqlWalker.g  to  HqlSqlWalker.cs
echo        SqlGenerator.g  to  SqlGenerator.cs
echo B. Regenerate all grammars, with Hql.g in debug mode.
echo C. Regenerate all grammars, with HqlSqlWalker.g in debug mode.
echo D. Regenerate all grammars, with SqlGenerator.g in debug mode.
echo E. Quick instructions on using debug mode.
echo.
echo X. Exit to main menu.
echo.

%BUILDTOOL% prompt ABCDEX
if errorlevel 5 goto main-menu
if errorlevel 4 goto antlr-debug
if errorlevel 3 goto antlr-sqlgenerator-debug
if errorlevel 2 goto antlr-hqlsqlwalker-debug
if errorlevel 1 goto antlr-hql-debug
if errorlevel 0 goto antlr-all

:antlr-all
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto main-menu

:antlr-hql-debug
echo *** Regenerating from Hql.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlDebug.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto main-menu

:antlr-hqlsqlwalker-debug
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalkerDebug.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto main-menu

:antlr-sqlgenerator-debug
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGeneratorDebug.bat
goto main-menu

:antlr-debug
echo To use the debug grammar:
echo   1. Create a unit test that runs the hql parser on the input you're interested in.
echo       The one you want to debug must be the first grammar parsed.
echo   2. Run the unit test.  It will appear to stall.
echo   3. Download and run AntlrWorks (java -jar AntlrWorks.jar).
echo   4. Open the grammar you intend to debug in AntlrWorks.
echo   5. Choose "Debug Remote" and accept the default port.
echo   6. You should now be connected and able to step through your grammar.
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

%BUILDTOOL% prompt ABCDEFGHIJKLX
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
