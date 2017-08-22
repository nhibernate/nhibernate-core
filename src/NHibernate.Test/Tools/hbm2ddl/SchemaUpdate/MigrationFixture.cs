using System;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaUpdate
{
	[TestFixture]
	public class MigrationFixture
	{
		private void MigrateSchema(string resource1, string resource2)
		{
			Configuration v1cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var driverClass = ReflectHelper.ClassForName(v1cfg.GetProperty(Environment.ConnectionDriver));
			// Odbc is not supported by schema update: System.Data.Odbc.OdbcConnection.GetSchema("ForeignKeys") fails with an ArgumentException: ForeignKeys is undefined.
			// It seems it would require its own DataBaseSchema, but this is bound to the dialect, not the driver.
			if (typeof(OdbcDriver).IsAssignableFrom(driverClass))
				Assert.Ignore("Test is not compatible with ODBC");

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource1))
				v1cfg.AddInputStream(stream);
			new SchemaExport(v1cfg).Execute(false, true, true);

			Tool.hbm2ddl.SchemaUpdate v1schemaUpdate = new Tool.hbm2ddl.SchemaUpdate(v1cfg);
			v1schemaUpdate.Execute(true, true);

			foreach (Exception e in v1schemaUpdate.Exceptions)
				Console.WriteLine(e);

			Assert.AreEqual(0, v1schemaUpdate.Exceptions.Count);

			Configuration v2cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource2))
				v2cfg.AddInputStream(stream);

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
	}
}