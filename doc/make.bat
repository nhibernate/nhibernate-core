@ECHO OFF

pushd %~dp0

REM Command file for Sphinx documentation

set SPHINXBUILD=python -m sphinx
set SOURCEDIR=source
set BUILDDIR=build
set SPHINXPROJ=NHibernate

if "%1" == "" goto help

%SPHINXBUILD% -M %1 %SOURCEDIR% %BUILDDIR% %SPHINXOPTS%
goto end

:help
%SPHINXBUILD% -M help %SOURCEDIR% %BUILDDIR% %SPHINXOPTS%

:end
popd
