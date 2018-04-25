@ECHO OFF

pushd %~dp0

REM Command file for Sphinx documentation

set PATH=%PATH%;..\Tools\python;..\Tools\python\Scripts
set SPHINXBUILD=python -m sphinx
set SOURCEDIR=reference
set BUILDDIR=..\build
set SPHINXPROJ=NHibernate

if "%1" == "" goto help

%SPHINXBUILD% -M %1 %SOURCEDIR% %BUILDDIR% %SPHINXOPTS%
goto end

:help
%SPHINXBUILD% -M help %SOURCEDIR% %BUILDDIR% %SPHINXOPTS%

:end
popd
