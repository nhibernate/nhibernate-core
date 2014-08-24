﻿using System;
using System.Collections;
using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2297
{
	[TestFixture]
	public class Fixture // Purposefully doesn't inherit from BugTestCase
	{
		[TestCase(".MappingsNames.hbm.xml",
			ExpectedException = typeof (InvalidOperationException),
			ExpectedMessage =
				"ICompositeUserType NHibernate.Test.NHSpecificTest.NH2297.InvalidNamesCustomCompositeUserType returned a null value for 'PropertyNames'."
			)]
		[TestCase(".MappingsTypes.hbm.xml",
			ExpectedException = typeof (InvalidOperationException),
			ExpectedMessage =
				"ICompositeUserType NHibernate.Test.NHSpecificTest.NH2297.InvalidTypesCustomCompositeUserType returned a null value for 'PropertyTypes'."
			)]
		public void InvalidCustomCompositeUserTypeThrowsMeaningfulException(string mappingFile)
		{
			var cfg = new Configuration();

			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			const string MappingsAssembly = "NHibernate.Test";

			Assembly assembly = Assembly.Load(MappingsAssembly);

			string ns = GetType().Namespace;
			string bugNumber = ns.Substring(ns.LastIndexOf('.') + 1);

			cfg.AddResource(MappingsAssembly + "." + "NHSpecificTest." + bugNumber + mappingFile, assembly);

			// build session factory creates the invalid custom type mapper, and throws the exception
			cfg.BuildSessionFactory();
		}
	}
}
