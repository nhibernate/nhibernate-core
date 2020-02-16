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

			Assert.That(s, Does.Match("create ((column|row) )?table Home_All"));
			Assert.That(s, Does.Match("create ((column|row) )?table Home_Export"));
		}

		[Test]
		public void SchemaExport_Update_CreatesUpdateScript()
		{
			Configuration configuration = GetConfiguration();
			SchemaUpdate update = new SchemaUpdate(configuration);
			TextWriter tw = new StringWriter();
			update.Execute(tw.WriteLine, false);

			string s = tw.ToString();
			Assert.That(s, Does.Match("create ((column|row) )?table Home_Update"));
			Assert.That(s, Does.Match("create ((column|row) )?table Home_All"));
		}

		[Test]
		public void SchemaExport_Validate_CausesValidateException()
		{
			Configuration configuration = GetConfiguration();
			SchemaValidator validator = new SchemaValidator(configuration);

			Assert.That(
				() => validator.Validate(),
				Throws.TypeOf<SchemaValidationException>()
				      .And.Message.EqualTo("Schema validation failed: see list of validation errors")
				      .And.Property("ValidationErrors").Contains("Missing table: Home_Validate"));
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
