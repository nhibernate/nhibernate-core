using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2303
{
	public class Fixture
	{
		[Test]
		public void IndependentSubclassElementCanExtendSubclass()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.Executing(c => c.AddResource("NHibernate.Test.NHSpecificTest.NH2303.Mappings.hbm.xml", GetType().Assembly)).
				NotThrows();
			cfg.BuildSessionFactory();
			cfg.Executing(c => c.BuildSessionFactory()).NotThrows();
		}
	}
}