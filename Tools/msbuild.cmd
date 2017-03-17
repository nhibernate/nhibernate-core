@echo off
set MSBuildSDKsPath="C:\Program Files\dotnet\sdk\1.0.1\Sdks"

set pre=Microsoft.VisualStudio.Product.
set ids=%pre%Community %pre%Professional %pre%Enterprise %pre%BuildTools

for /f "usebackq tokens=1* delims=: " %%i in (`%~dp0\vswhere.1.0.58\tools\vswhere.exe -latest -products %ids% -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" %*
) else ( 
  exit /b -1
)
