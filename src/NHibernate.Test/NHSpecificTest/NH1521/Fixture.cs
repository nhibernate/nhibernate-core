using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1521
{
	[TestFixture]
	public class Fixture
	{
		private static void CheckDialect(Configuration configuration)
		{
			if (!configuration.Properties[Environment.Dialect].Contains("MsSql"))
				Assert.Ignore("Specific test for MsSQL dialects");
		}

		private static void AssertThatCheckOnTableExistenceIsCorrect(Configuration configuration)
		{
			var su = new SchemaExport(configuration);
			var sb = new StringBuilder(500);
			su.Execute(x => sb.AppendLine(x), false, false);
			string script = sb.ToString();
			Assert.That(script, Is.StringContaining("if exists (select * from dbo.sysobjects where id = object_id(N'nhibernate.dbo.Aclass') and OBJECTPROPERTY(id, N'IsUserTable') = 1)"));
		}

		[Test]
		public void TestForClassWithDefaultSchema()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1521.AclassWithNothing.hbm.xml", GetType().Assembly);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "dbo");
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithDefaultValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1521.AclassWithDefault.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithSpecificValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1521.AclassWithSpecific.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithDefaultValuesInConfigurationPriorityToMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1521.AclassWithDefault.hbm.xml", GetType().Assembly);
			cfg.SetProperty(Environment.DefaultCatalog, "somethingDifferent");
			cfg.SetProperty(Environment.DefaultSchema, "somethingDifferent");
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}
	}

	public class Aclass
	{
		public int Id { get; set; }
	}
}