namespace NHibernate.Test.SchemaUpdate
{
	using System;
	using System.IO;
	using System.Reflection;
	using Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;

	[TestFixture]
	public class MigrationFixture
	{
		[Test]
		public void SimpleColumnAddition()
		{
			String resource2 = "NHibernate.Test.SchemaUpdate.2_Version.hbm.xml";
			String resource1 = "NHibernate.Test.SchemaUpdate.1_Version.hbm.xml";

			Configuration v1cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource1))
				v1cfg.AddInputStream(stream);
			new SchemaExport(v1cfg).Execute(false, true, true, false);

			SchemaUpdate v1schemaUpdate = new SchemaUpdate(v1cfg);
			v1schemaUpdate.Execute(true, true);

			Assert.AreEqual(0, v1schemaUpdate.Exceptions.Count);

			Configuration v2cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource2))
				v2cfg.AddInputStream(stream);
			

			SchemaUpdate v2schemaUpdate = new SchemaUpdate(v2cfg);
			v2schemaUpdate.Execute(true, true);
			Assert.AreEqual(0, v2schemaUpdate.Exceptions.Count);

		}
	}
}