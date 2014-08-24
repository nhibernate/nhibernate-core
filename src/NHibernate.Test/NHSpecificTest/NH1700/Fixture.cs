using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1700
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ShouldNotThrowDuplicateMapping()
		{
			var cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);

			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1700.Mappings.hbm.xml", GetType().Assembly);
			new SchemaExport(cfg).Create(false, true);

			new SchemaExport(cfg).Drop(false, true);
		}
	}
}