using System.Collections;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2288
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
			script.Should().Contain(
				"if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[Aclasses_Id_FK]') AND parent_object_id = OBJECT_ID('dbo.Aclass'))");
		}

		[Test]
		public void TestForClassWithDefaultSchema()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "dbo");
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2288.AclassWithNothing.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithDefaultValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "somethingDifferent");
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2288.AclassWithDefault.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithSpecificValuesInMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "somethingDifferent");
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2288.AclassWithSpecific.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}

		[Test]
		public void WithDefaultValuesInConfigurationPriorityToMapping()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			CheckDialect(cfg);
			cfg.SetProperty(Environment.DefaultCatalog, "nhibernate");
			cfg.SetProperty(Environment.DefaultSchema, "somethingDifferent");
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2288.AclassWithDefault.hbm.xml", GetType().Assembly);
			AssertThatCheckOnTableExistenceIsCorrect(cfg);
		}
	}

	public class Aclass
	{
		public int Id { get; set; }
	}

	public class Bclass
	{
		public int Id { get; set; }
		public ICollection Aclasses { get; set; }
	}
}