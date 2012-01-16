set NANT=%~dp0Tools\nant\bin\NAnt.exe -t:net-3.5

%NANT% -D:project.config=release clean package
