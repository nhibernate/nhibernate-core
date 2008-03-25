
						How To Build NHibernate Documentation

================================================================================
Introduction
================================================================================

This directory contains the DocBook source files used to generate the NHibernate
Documentation.

To build the documentation, you have to install some tools; after that, running

    nant doc

in the project directory. The result will be in /build/NHibernate-<version>/doc.

================================================================================
Prerequisites
================================================================================
You need a Java Runtime Environment (JRE) to run Saxon which is used to build
the documentation. You don't need to download and install Saxon, its jars are
present in the repository.

You need HTML Help Workshop to build the CHM help file, and Visual Studio
Help Integration Kit (VSHIK) to build the HtmlHelp2 help file.

================================================================================
DocBook Quick Start
================================================================================
Read this article on Code Project (by Jim Crafton)
	"Documention with DocBook on Win32"
	http://www.codeproject.com/winhelp/docbook_howto.asp


================================================================================
DocBook Manual
================================================================================
Can be downloaded at http://docbook.org/tdg/en/tdg-en-html-2.0.10.zip
Can be viewed (online) at
http://www.oasis-open.org/docbook/documentation/reference/html/docbook.html


================================================================================
DocBook XSL Guide
================================================================================
Can be viewed at http://www.sagehill.net/docbookxsl/index.html


================================================================================
URL for htmlhelp parameters
================================================================================
http://docbook.sourceforge.net/release/xsl/snapshot/doc/html/
