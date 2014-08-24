using System;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaUpdate
{
	[TestFixture]
	public class MigrationFixture
	{

		private void MigrateSchema(string resource1, string resource2)
		{
			Configuration v1cfg = TestConfigurationHelper.GetDefaultConfiguration();
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