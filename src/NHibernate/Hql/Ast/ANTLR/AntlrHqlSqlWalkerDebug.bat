@echo off
rem I wanted to put this in the nant build file, but I had very annoying problems with 64-bit java running from the 32-bit nant process.
pushd %~dp0
..\..\..\..\..\Tools\Antlr\Antlr3.exe -debug -o Generated HqlSqlWalker.g
popd