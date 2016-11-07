using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2303
{
	public class Fixture
	{
		[Test]
		public void IndependentSubclassElementCanExtendSubclass()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			Assert.That(() => cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2303.Mappings.hbm.xml", GetType().Assembly), Throws.Nothing);
			cfg.BuildSessionFactory();
			Assert.That(() => cfg.BuildSessionFactory(), Throws.Nothing);
		}
	}
}