using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1427
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void TestMappingWithJoinElementContainingXmlComments()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			Assert.DoesNotThrow(() => cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1427.Mappings.hbm.xml", GetType().Assembly));
		}
	}
}
