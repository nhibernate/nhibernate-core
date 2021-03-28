using System;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaUpdate
{
	[TestFixture]
	public class MigrationFixture
	{
		private Configuration _configurationToDrop;
		private FirebirdClientDriver _fireBirdDriver;
		
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var driverClass = ReflectHelper.ClassForName(cfg.GetProperty(Environment.ConnectionDriver));
			// Odbc is not supported by schema update: System.Data.Odbc.OdbcConnection.GetSchema("ForeignKeys") fails with an ArgumentException: ForeignKeys is undefined.
			// It seems it would require its own DataBaseSchema, but this is bound to the dialect, not the driver.
			if (typeof(OdbcDriver).IsAssignableFrom(driverClass))
				Assert.Ignore("Test is not compatible with ODBC");

			if (typeof(FirebirdClientDriver).IsAssignableFrom(driverClass))
				_fireBirdDriver = new FirebirdClientDriver();
		}

		[TearDown]
		public void TearDown()
		{
			if (_configurationToDrop != null)
				DropSchema(_configurationToDrop);
			_configurationToDrop = null;
		}

		private void MigrateSchema(string resource1, string resource2)
		{
			var v1cfg = GetConfigurationForMapping(resource1);
			DropSchema(v1cfg);
			_configurationToDrop = v1cfg;

			Tool.hbm2ddl.SchemaUpdate v1schemaUpdate = new Tool.hbm2ddl.SchemaUpdate(v1cfg);
			v1schemaUpdate.Execute(true, true);

			foreach (Exception e in v1schemaUpdate.Exceptions)
				Console.WriteLine(e);

			Assert.AreEqual(0, v1schemaUpdate.Exceptions.Count);

			var v2cfg = GetConfigurationForMapping(resource2);

			Tool.hbm2ddl.SchemaUpdate v2schemaUpdate = new Tool.hbm2ddl.SchemaUpdate(v2cfg);
			v2schemaUpdate.Execute(true, true);

			foreach (Exception e in v2schemaUpdate.Exceptions)
				Console.WriteLine(e);

			Assert.AreEqual(0, v2schemaUpdate.Exceptions.Count);
		}

		[Test]
		public void SimpleColumnAddition()
		{
			String resource2 = "NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.2_Version.hbm.xml";
			String resource1 = "NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.1_Version.hbm.xml";

			MigrateSchema(resource1, resource2);
		}

		[Test]
		public void SimpleColumnReplace()
		{
			String resource2 = "NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.2_Person.hbm.xml";
			String resource1 = "NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.1_Person.hbm.xml";

			MigrateSchema(resource1, resource2);
		}

		[Test]
		public void AutoUpdateFailuresAreThrown()
		{
			var cfg1 = GetConfigurationForMapping("NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.1_Person.hbm.xml");
			var sf1 = cfg1.BuildSessionFactory();
			var dialect = Dialect.Dialect.GetDialect(cfg1.Properties);
			if (!dialect.SupportsUnique)
				Assert.Ignore("This test requires a dialect supporting unique constraints");

			_configurationToDrop = cfg1;
			CreateSchema(cfg1);

			using (var s = sf1.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person());
				s.Save(new Person());
				t.Commit();
			}

			// This schema switches to not-nullable the person properties, which should fail due to an existing person with null properties.
			var cfg2 = GetConfigurationForMapping("NHibernate.Test.Tools.hbm2ddl.SchemaUpdate.3_Person.hbm.xml");
			cfg2.Properties[Environment.Hbm2ddlAuto] = SchemaAutoAction.Update.ToString();
			cfg2.Properties[Environment.Hbm2ddlThrowOnUpdate] = "true";
			Assert.That(() => cfg2.BuildSessionFactory(), Throws.InstanceOf<AggregateHibernateException>());
		}

		private Configuration GetConfigurationForMapping(string resourcePath)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
				cfg.AddInputStream(stream);
			return cfg;
		}

		private void CreateSchema(Configuration cfg)
		{
			// Firebird will pool each connection created during the test and will marked as used any table
			// referenced by queries. It will at best delays those tables drop until connections are actually
			// closed, or immediately fail dropping them.
			// This results in other tests failing when they try to create tables with the same name.
			// By clearing the connection pool the tables will get dropped.
			_fireBirdDriver?.ClearPool(null);

			new SchemaExport(cfg).Create(false, true);
		}

		private void DropSchema(Configuration cfg)
		{
			// Firebird will pool each connection created during the test and will marked as used any table
			// referenced by queries. It will at best delays those tables drop until connections are actually
			// closed, or immediately fail dropping them.
			// This results in other tests failing when they try to create tables with the same name.
			// By clearing the connection pool the tables will get dropped.
			_fireBirdDriver?.ClearPool(null);

			new SchemaExport(cfg).Drop(false, true);
		}
	}
}
