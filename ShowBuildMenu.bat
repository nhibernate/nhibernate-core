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

if exist %SYSTEMROOT%\System32\choice.exe ( goto prompt-choice )
goto prompt-set

:prompt-choice
choice /C:abcdefgh

if errorlevel 255 goto end
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
set /p OPT=[A, B, C, D, E, F, G, H]? 

if /I "%OPT%"=="A" goto build-visual-studio
if /I "%OPT%"=="B" goto help-test-setup
if /I "%OPT%"=="C" goto help-larger-window
if /I "%OPT%"=="D" goto build-test
if /I "%OPT%"=="E" goto build-debug
if /I "%OPT%"=="F" goto build-release
if /I "%OPT%"=="G" goto build-release-package
if /I "%OPT%"=="H" goto grammar
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
%NANT% -D:project.config=release clean build
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
echo A.  Regenerate HqlLexer.cs and HqlParser.cs from Hql.g.
echo B.  Regenerate HqlSqlWalker.cs from HqlSqlWalker.g.
echo C.  Regenerate Hql.g in debug mode.
echo D.  Regenerate HqlSqlWalker.g in debug mode.
echo E.  Quick instructions on using debug mode.
echo.

if exist %SYSTEMROOT%\System32\choice.exe ( goto grammar-prompt-choice )
goto grammar-prompt-set

:grammar-prompt-choice
choice /C:abcde

if errorlevel 255 goto end
if errorlevel 5 goto antlr-debug
if errorlevel 4 goto antlr-hqlsqlwalker-debug
if errorlevel 3 goto antlr-hql-debug
if errorlevel 2 goto antlr-hqlsqlwalker
if errorlevel 1 goto antlr-hql
if errorlevel 0 goto end

:grammar-prompt-set
set /p OPT=[A, B, C, D, E]? 

if /I "%OPT%"=="A" goto antlr-hql
if /I "%OPT%"=="B" goto antlr-hqlsqlwalker
if /I "%OPT%"=="C" goto antlr-hql-debug
if /I "%OPT%"=="D" goto antlr-hqlsqlwalker-debug
if /I "%OPT%"=="E" goto antlr-debug
goto grammar-prompt-set

:antlr-hql
call src\NHibernate\Hql\Ast\ANTLR\AntlrHql.bat
goto end

:antlr-hqlsqlwalker
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalker.bat
goto end

:antlr-hql-debug
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlDebug.bat
goto end

:antlr-hqlsqlwalker-debug
call src\NHibernate\Hql\Ast\ANTLR\AntlrHqlSqlWalkerDebug.bat
goto end

:antlr-debug
echo To use the debug grammar:
echo   1.  Create a unit test that runs the hql parser on the input you're interested in.
echo   2.  Run the unit test.  It will appear to stall.
echo   3.  Download and run AntlrWorks.
echo   4.  Choose "Debug Remote" and allow the default ports.
echo   5.  You should now be connected and able to step through your grammar.
goto end

:end
popd
pause
