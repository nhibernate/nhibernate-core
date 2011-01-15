rem I wanted to put this in the nant build file, but I had very annoying problems with 64-bit java running from the 32-bit nant process.
@echo off
pushd %~dp0
java.exe -cp ..\..\..\..\..\Tools\Antlr\antlr-3.2.jar org.antlr.Tool -debug -o Generated Hql.g
popd