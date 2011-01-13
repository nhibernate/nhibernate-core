@echo off
pushd %~dp0\..\..\src\NHibernate\Hql\Ast\ANTLR
java.exe -cp ..\..\..\..\..\Tools\Antlr\antlr-3.2.jar org.antlr.Tool -o Generated Hql.g
popd