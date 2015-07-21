using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1255
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CanLoadMappingWithNotNullIgnore()
		{
			var cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);
			Assert.DoesNotThrow(
				() => cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1255.Mappings.hbm.xml", typeof (Customer).Assembly));
		}
	}
}
