@echo off

for /f "usebackq tokens=*" %%i in (`%userprofile%\.nuget\pacakges\vswhere2\2.1.4\tools\vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

if exist "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" (
  "%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe" %*
) else ( 
  exit /b -1
)
