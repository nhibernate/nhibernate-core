@echo off
set NANT=%~dp0\Tools\nant\bin\NAnt.exe -t:net-3.5

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
choice /C abcdefg

if errorlevel 255 goto end
if errorlevel 7 goto build-release-package
if errorlevel 6 goto build-release
if errorlevel 5 goto build-debug
if errorlevel 4 goto build-test
if errorlevel 3 goto help-larger-window
if errorlevel 2 goto help-test-setup
if errorlevel 1 goto build-visual-studio
if errorlevel 0 goto end

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

:end
pause
