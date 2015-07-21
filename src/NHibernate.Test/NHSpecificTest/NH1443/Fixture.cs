using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1443
{
	[TestFixture]
	public class Fixture
	{
		private static void Bug(Configuration cfg)
		{
			var su = new SchemaExport(cfg);
			var sb = new StringBuilder(500);
			su.Execute(x => sb.AppendLine(x), false, false);
			string script = sb.ToString();


			if (Dialect.Dialect.GetDialect(cfg.Properties).SupportsIfExistsBeforeTableName)
				Assert.That(script, Is.StringMatching("drop table if exists nhibernate.dbo.Aclass"));
			else
				Assert.That(script, Is.StringMatching("drop table nhibernate.dbo.Aclass"));

			Assert.That(script, Is.StringMatching("create table nhibernate.dbo.Aclass"));
			
		}

		[Test]
		public void WithDefaultValuesInConfiguration()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1443.AclassWithNothing.hbm.xml", GetType().Assembly);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "dbo");
			Bug(cfg);
		}

		[Test]
		public void WithDefaultValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1443.AclassWithDefault.hbm.xml", GetType().Assembly);
			Bug(cfg);
		}

		[Test]
		public void WithSpecificValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1443.AclassWithSpecific.hbm.xml", GetType().Assembly);
			Bug(cfg);
		}

		[Test]
		public void WithDefaultValuesInConfigurationPriorityToMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1443.AclassWithDefault.hbm.xml", GetType().Assembly);
			cfg.SetProperty(Environment.DefaultCatalog, "somethingDifferent");
			cfg.SetProperty(Environment.DefaultSchema, "somethingDifferent");
			Bug(cfg);
		}
	}

	public class Aclass
	{
		public int Id { get; set; }
	}
}