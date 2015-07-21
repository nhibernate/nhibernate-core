using System.Reflection;
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
			const string resource = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.1_Version.hbm.xml";
			var cfg = BuildConfiguration(resource);

			new SchemaExport(cfg).Execute(true, true, false);

			var validator = new Tool.hbm2ddl.SchemaValidator((cfg));
			validator.Validate();
		}

		[Test, SetCulture("tr-TR"), SetUICulture("tr-TR")]
		public void ShouldVerifySameTableTurkish()
		{
			//NH-3063

			// Turkish have unusual casing rules for the letter 'i'. This test verifies that
			// code paths executed by the SchemaValidator correctly handles case insensitive
			// comparisons for this.

			// Just make sure that we have an int property in the mapped class. This is
			// the 'i' we rely on for the test.
			var v = new Version();
			Assert.That(v.Id, Is.TypeOf<int>());


			const string resource = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.1_Version.hbm.xml";
			var cfg = BuildConfiguration(resource);

			new SchemaExport(cfg).Execute(true, true, false);

			var validator = new Tool.hbm2ddl.SchemaValidator(cfg);
			validator.Validate();
		}

		[Test]
		public void ShouldNotVerifyModifiedTable()
		{
			const string resource1 = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.1_Version.hbm.xml";
			var cfgV1 = BuildConfiguration(resource1);

			const string resource2 = "NHibernate.Test.Tools.hbm2ddl.SchemaValidator.2_Version.hbm.xml";
			var cfgV2 = BuildConfiguration(resource2);

			new SchemaExport(cfgV1).Execute(true, true, false);

			var validatorV2 = new Tool.hbm2ddl.SchemaValidator(cfgV2);
			try
			{
				validatorV2.Validate();
			}
			catch (HibernateException e)
			{
				Assert.That(e.Message, Is.StringStarting("Missing column: Name"));
			}
		}

		private static Configuration BuildConfiguration(string resource)
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
				cfg.AddInputStream(stream);
			return cfg;
		}
	}
}