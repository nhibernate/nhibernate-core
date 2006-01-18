using System;
using System.Reflection;
using System.Runtime.InteropServices;

/*
 * This file contains the parts of AssemblyInfo that never change from build 
 * to build.  The file AssemblyInfoVersion contains the parts that will be different
 * depending on the framework version and version of NH that is built
 */ 
[assembly: CLSCompliantAttribute(true)]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyDescriptionAttribute("An object persistence library for relational databases.")]
[assembly: AssemblyCompanyAttribute("NHibernate.org")]
[assembly: AssemblyProductAttribute("NHibernate")]
[assembly: AssemblyCopyrightAttribute("Licensed under LGPL.")]

#if STRONG
/*
 * This key location is valid when the build is done from NAnt.  If building from
 * VS.NET make sure to change this.
 */ 
[assembly: AssemblyKeyFileAttribute("..\\NHibernate.snk")]
#endif

