@echo off
pushd %~dp0

set NANT=Tools\nant\bin\NAnt.exe -t:net-3.5

echo --- SETUP ---
echo A.  Set up for Visual Studio (creates AssemblyInfo.cs files).
echo.
echo --- TESTING ---
echo B.  Learn how to set up database and connection string for testing.
echo C.  How to increase the window scroll/size so you can see more test output.
echo D.  Build and run all tests.
echo.
echo --- BUILD ---
echo E.  Build NHibernate (Debug)
echo F.  Build NHibernate (Release)
echo G.  Build Release Package (Also runs tests and creates documentation)
echo.
echo --- GRAMMAR ---
echo H.  Grammar operations (related to Hql.g and HqlSqlWalker.g)
echo.
echo --- TeamCity (CI) build options
echo I.  TeamCity build menu
echo.

if exist %SYSTEMROOT%\System32\choice.exe ( goto prompt-choice )
goto prompt-set

:prompt-choice
choice /C:abcdefghi

if errorlevel 255 goto end
if errorlevel 9 goto teamcity-menu
if errorlevel 8 goto grammar
if errorlevel 7 goto build-release-package
if errorlevel 6 goto build-release
if errorlevel 5 goto build-debug
if errorlevel 4 goto build-test
if errorlevel 3 goto help-larger-window
if errorlevel 2 goto help-test-setup
if errorlevel 1 goto build-visual-studio
if errorlevel 0 goto end

:prompt-set
set /p OPT=[A, B, C, D, E, F, G, H, I]? 

if /I "%OPT%"=="A" goto build-visual-studio
if /I "%OPT%"=="B" goto help-test-setup
if /I "%OPT%"=="C" goto help-larger-window
if /I "%OPT%"=="D" goto build-test
if /I "%OPT%"=="E" goto build-debug
if /I "%OPT%"=="F" goto build-release
if /I "%OPT%"=="G" goto build-release-package
if /I "%OPT%"=="H" goto grammar
if /I "%OPT%"=="I" goto teamcity-menu
goto prompt-set

:help-test-setup
echo.
echo 1.  Install SQL Server 2008 (or use the database included with VS).
echo 2.  Edit connection settings in build-common\nhibernate-properties.xml
echo.
echo 3.  If you want to run NUnit tests in Visual Studio directly,
echo     edit src\NHibernate.Test\App.config and change this property:
echo         connection.connection_string
echo     Note that you will need a third party tool to run tests in VS.
echo.
echo     You will also need to create a database called "nhibernate"
echo     if you just run the tests directly from VS.
echo.
goto end

:help-larger-window
echo.
echo 1.  Right click on the title bar of this window.
echo 2.  Select "Properties".
echo 3.  Select the "Layout" tab.
echo 4.  Set the following options.
echo         Screen Buffer Size
echo             Width: 160
echo             Height: 9999
echo         Window Size
echo             Width: 160
echo             Height: 50
echo.
goto end

:build-visual-studio
%NANT% visual-studio
goto end

:build-debug
%NANT% clean build
echo.
echo Assuming the build succeeded, your results will be in the build folder.
echo.
goto end

:build-release
%NANT% -D:project.config=release clean release
echo.
echo Assuming the build succeeded, your results will be in the build folder.
echo.
goto end

:build-release-package
%NANT% -D:project.config=release clean package
echo.
echo Assuming the build succeeded, your results will be in the build folder.
echo.
goto end

:build-test
%NANT% test
goto end

:grammar
echo.
echo --- GRAMMAR ---
echo A.  Regenerate all grammars.
echo         Hql.g           to  HqlLexer.cs
echo         Hql.g           to  HqlParser.cs
echo         HqlSqlWalker.g  to  HqlSqlWalker.cs
echo         SqlGenerator.g  to  SqlGenerator.cs
echo B.  Regenerate all grammars, with Hql.g in debug mode.
echo C.  Regenerate all grammars, with HqlSqlWalker.g in debug mode.
echo D.  Regenerate all grammars, with SqlGenerator.g in debug mode.
echo E.  Quick instructions on using debug mode.
echo.

if exist %SYSTEMROOT%\System32\choice.exe ( goto grammar-prompt-choice )
goto grammar-prompt-set

:grammar-prompt-choice
choice /C:abcde

if errorlevel 255 goto end
if errorlevel 5 goto antlr-debug
if errorlevel 4 goto antlr-sqlgenerator-debug
if errorlevel 3 goto antlr-hqlsqlwalker-debug
if errorlevel 2 goto antlr-hql-debug
if errorlevel 1 goto antlr-all
if errorlevel 0 goto end

:grammar-prompt-set
set /p OPT=[A, B, C, D, E]?

if /I "%OPT%"=="A" goto antlr-all
if /I "%OPT%"=="B" goto antlr-hql-debug
if /I "%OPT%"=="C" goto antlr-hqlsqlwalker-debug
if /I "%OPT%"=="D" goto antlr-sqlgenerator-debug
if /I "%OPT%"=="E" goto antlr-debug
goto grammar-prompt-set

:antlr-all
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto end

:antlr-hql-debug
echo *** Regenerating from Hql.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlDebug.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto end

:antlr-hqlsqlwalker-debug
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalkerDebug.bat
echo *** Regenerating from SqlGenerator.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGenerator.bat
goto end

:antlr-sqlgenerator-debug
echo *** Regenerating from Hql.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
echo *** Regenerating from HqlSqlWalker.g
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
echo *** Regenerating from SqlGenerator.g (Debug Enabled)
call src\NHibernate\Hql\Ast\ANTLR\AntlrSqlGeneratorDebug.bat
goto end

:antlr-debug
echo To use the debug grammar:
echo   1.  Create a unit test that runs the hql parser on the input you're interested in.
echo       The one you want to debug must be the first grammar parsed.
echo   2.  Run the unit test.  It will appear to stall.
echo   3.  Download and run AntlrWorks (java -jar AntlrWorks.jar).
echo   4.  Open the grammar you intend to debug in AntlrWorks.
echo   5.  Choose "Debug Remote" and accept the default port.
echo   6.  You should now be connected and able to step through your grammar.
goto end

:teamcity-menu
echo.
echo --- TeamCity (CI) build options
echo A.  NHibernate Trunk (default SQL Server)
echo B.  NHibernate Trunk - Firebird (32-bit)
echo C.  NHibernate Trunk - Firebird (64-bit)
echo D.  NHibernate Trunk - SQLite (32-bit)
echo E.  NHibernate Trunk - SQLite (64-bit)
echo F.  NHibernate Trunk - PostgreSQL
echo G.  NHibernate Trunk - Oracle (32-bit)
echo.

if exist %SYSTEMROOT%\System32\choice.exe ( goto teamcity-menu-prompt-choice )
goto teamcity-menu-prompt-set

:teamcity-menu-prompt-choice
choice /C:abcdefg

if errorlevel 255 goto end
if errorlevel 7 goto teamcity-oracle32
if errorlevel 6 goto teamcity-postgresql
if errorlevel 5 goto teamcity-sqlite64
if errorlevel 4 goto teamcity-sqlite32
if errorlevel 3 goto teamcity-firebird64
if errorlevel 2 goto teamcity-firebird32
if errorlevel 1 goto teamcity-trunk
if errorlevel 0 goto end

:teamcity-menu-prompt-set
set /p OPT=[A, B, C, D, E, F, G]? 

if /I "%OPT%"=="A" goto teamcity-trunk
if /I "%OPT%"=="B" goto teamcity-firebird32
if /I "%OPT%"=="C" goto teamcity-firebird64
if /I "%OPT%"=="D" goto teamcity-sqlite32
if /I "%OPT%"=="E" goto teamcity-sqlite64
if /I "%OPT%"=="F" goto teamcity-postgresql
if /I "%OPT%"=="G" goto teamcity-oracle32
goto teamcity-menu-prompt-set

:teamcity-trunk
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1
goto end

:teamcity-firebird32
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=firebird32
goto end

:teamcity-firebird64
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=firebird64
goto end

:teamcity-sqlite32
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlite32
goto end

:teamcity-sqlite64
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=sqlite64
goto end

:teamcity-postgresql
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=postgresql
goto end

:teamcity-oracle32
%NANT% /f:teamcity.build -D:skip.manual=true -D:CCNetLabel=-1 -D:config.teamcity=oracle32
goto end

:end
popd
pause
