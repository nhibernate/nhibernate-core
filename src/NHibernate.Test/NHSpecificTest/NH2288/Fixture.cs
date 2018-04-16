using System.Collections;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2288
{
	[TestFixture]
	public class Fixture
	{
		private static void CheckDialect(Configuration configuration)
		{
			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			if (!(dialect is MsSql2000Dialect))
				Assert.Ignore("Specific test for MsSQL dialects");
		}

		private static void AssertThatCheckOnTableExistenceIsCorrect(Configuration configuration)
		{
			var su = new SchemaExport(configuration);
			var sb = new StringBuilder(500);
			su.Execute(x => sb.AppendLine(x), false, false);
			string script = sb.ToString();
			Assert.That(script, Does.Contain("if exists (select 1 from nhibernate.sys.objects where object_id = OBJECT_ID(N'nhibernate.dbo.[Aclasses_Id_FK]') and parent_object_id = OBJECT_ID(N'nhibernate.dbo.Aclass'))"));
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
