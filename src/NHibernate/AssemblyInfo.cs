using System;
using System.Reflection;
using System.Security;

[assembly: CLSCompliantAttribute(true)]
[assembly: AssemblyTitleAttribute("NHibernate")]
[assembly: AssemblyDescriptionAttribute("An object persistence library for relational databases.")]
[assembly: AssemblyCompanyAttribute("NHibernate.info")]
[assembly: AssemblyProductAttribute("NHibernate")]
[assembly: AssemblyCopyrightAttribute("Licensed under LGPL.")]
[assembly: AssemblyDelaySignAttribute(false)]
#if FEATURE_SECURITY_PERMISSIONS
[assembly: AllowPartiallyTrustedCallersAttribute()]
[assembly: SecurityRulesAttribute(SecurityRuleSet.Level1)]
#endif

