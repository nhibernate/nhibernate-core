using System;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.SchemaUpdate
{
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

			Tool.hbm2ddl.SchemaUpdate v1schemaUpdate = new Tool.hbm2ddl.SchemaUpdate(v1cfg);
			v1schemaUpdate.Execute(true, true);

			Assert.AreEqual(0, v1schemaUpdate.Exceptions.Count);

			Configuration v2cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource2))
				v2cfg.AddInputStream(stream);
			

			Tool.hbm2ddl.SchemaUpdate v2schemaUpdate = new Tool.hbm2ddl.SchemaUpdate(v2cfg);
			v2schemaUpdate.Execute(true, true);
			Assert.AreEqual(0, v2schemaUpdate.Exceptions.Count);
		}
	}
}