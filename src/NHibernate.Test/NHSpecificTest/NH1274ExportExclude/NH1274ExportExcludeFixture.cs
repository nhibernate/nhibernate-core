using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1274ExportExclude
{
	[TestFixture]
	public class NH1274ExportExcludeFixture
	{
		[Test]
		public void SchemaExport_Drop_CreatesDropScript()
		{
			Configuration configuration = GetConfiguration();
			SchemaExport export = new SchemaExport(configuration);
			TextWriter tw = new StringWriter();
			export.Drop(tw, false);
			string s = tw.ToString();

			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);

			if (dialect.SupportsIfExistsBeforeTableName)
			{
				Assert.IsTrue(s.Contains("drop table if exists Home_Drop"));
				Assert.IsTrue(s.Contains("drop table if exists Home_All"));
			}
			else
			{
				Assert.IsTrue(s.Contains("drop table Home_Drop"));
				Assert.IsTrue(s.Contains("drop table Home_All"));
			}
		}

		[Test]
		public void SchemaExport_Export_CreatesExportScript()
		{
			Configuration configuration = GetConfiguration();
			SchemaExport export = new SchemaExport(configuration);
			TextWriter tw = new StringWriter();
			export.Create(tw, false);
			string s = tw.ToString();

			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			if (dialect.SupportsIfExistsBeforeTableName)
			{
				Assert.IsTrue(s.Contains("drop table if exists Home_Drop"));
				Assert.IsTrue(s.Contains("drop table if exists Home_All"));
			}
			else
			{
				Assert.IsTrue(s.Contains("drop table Home_Drop"));
				Assert.IsTrue(s.Contains("drop table Home_All"));
			}

			Assert.IsTrue(s.Contains("create table Home_All"));
			Assert.IsTrue(s.Contains("create table Home_Export"));
		}

		[Test]
		public void SchemaExport_Update_CreatesUpdateScript()
		{
			Configuration configuration = GetConfiguration();
			SchemaUpdate update = new SchemaUpdate(configuration);
			TextWriter tw = new StringWriter();
			update.Execute(tw.WriteLine, false);

			string s = tw.ToString();
			Assert.IsTrue(s.Contains("create table Home_Update"));
			Assert.IsTrue(s.Contains("create table Home_All"));
		}

		[Test]
		public void SchemaExport_Validate_CausesValidateException()
		{
			Configuration configuration = GetConfiguration();
			SchemaValidator validator = new SchemaValidator(configuration);
			try
			{
				validator.Validate();
			}
			catch (HibernateException he)
			{
				Assert.IsTrue(he.Message.Contains("Home_Validate"));
				return;
			}
			throw new Exception("Should not get to this exception");
		}

		private Configuration GetConfiguration()
		{
			Configuration cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			Assembly assembly = Assembly.Load(MappingsAssembly);

			foreach (string file in Mappings)
			{
				cfg.AddResource(MappingsAssembly + "." + file, assembly);
			}
			return cfg;
		}


		protected static string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		public virtual string BugNumber
		{
			get
			{
				string ns = GetType().Namespace;
				return ns.Substring(ns.LastIndexOf('.') + 1);
			}
		}

		protected IList Mappings
		{
			get
			{
				return new string[]
				{
					"NHSpecificTest." + BugNumber + ".Mappings.hbm.xml"
				};
			}
		}
	}
}