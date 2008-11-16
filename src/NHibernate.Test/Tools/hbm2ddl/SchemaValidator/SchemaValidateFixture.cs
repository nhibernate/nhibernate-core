using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaValidator
{
	[TestFixture]
	public class SchemaValidateFixture
	{
		[Test]
		public void ShouldVerifySameTable()
		{
			string resource1 = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.1_Version.hbm.xml";
			Configuration v1cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource1))
			new NHibernate.Tool.hbm2ddl.SchemaExport(v1cfg).Execute(true,true,false,true);

			var v1schemaValidator = new NHibernate.Tool.hbm2ddl.SchemaValidator((v1cfg));
			v1schemaValidator.Validate();

		}

		[Test]
		[ExpectedException(typeof(HibernateException), ExpectedMessage = "Missing column: Name in nhibernate.dbo.Version")]
		public void ShouldNotVerifyModifiedTable()
		{
			string resource1 = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.1_Version.hbm.xml";
			string resource2 = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.2_Version.hbm.xml";
			Configuration v1cfg = TestConfigurationHelper.GetDefaultConfiguration();
			Configuration v2cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource1))
				v1cfg.AddInputStream(stream);
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource2))
				v2cfg.AddInputStream(stream);
			new NHibernate.Tool.hbm2ddl.SchemaExport(v1cfg).Execute(true, true, false, true);
			var v2schemaValidator = new NHibernate.Tool.hbm2ddl.SchemaValidator((v2cfg));
			v2schemaValidator.Validate();
		}
	}
}
