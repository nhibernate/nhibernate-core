@echo off

for /f "usebackq tokens=1* delims=: " %%i in (`%~dp0\vswhere.1.0.58\tools\vswhere.exe -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" %*
)
