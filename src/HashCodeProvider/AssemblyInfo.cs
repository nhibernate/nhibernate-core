using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("HashCodeProvider")]
[assembly: AssemblyDescription("Utility to call Object.GetHashCode nonvirtually.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("http://www.msjogren.net/dotnet/eng/samples/misc.asp")]
[assembly: AssemblyProduct("HashCodeProvider")]
[assembly: AssemblyCopyright(
"Copyright (c) 2002 Mattias Sjogren" + 
"\n\nThis software is provided 'as-is', without any express or implied warranty. In no event will the authors be held liable for any damages arising from the use of this software." + 
"\n\nPermission is granted to anyone to use this software for any purpose, including commercial applications, subject to the following restrictions:" + 
"\n\n1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required." + 
"\n\n2. No substantial portion of the source code may be redistributed without the express written permission of the copyright holders, where \"substantial\" is defined as enough code to be recognizably from this library.")]


//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.0.0")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: CLSCompliantAttribute(true)]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFileAttribute("..\\..\\..\\NHibernate.snk")]
[assembly: AllowPartiallyTrustedCallersAttribute()]
