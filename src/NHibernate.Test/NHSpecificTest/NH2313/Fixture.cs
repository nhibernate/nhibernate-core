using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2313
{
	public class Fixture
	{
		[Test]
		public void WhenLoadWorngMappingThenMessageShouldContaingWrongClassName()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2313.Mappings.hbm.xml", GetType().Assembly);
			Assert.That(() => cfg.BuildSessionFactory(), Throws.TypeOf<MappingException>()
															   .And.Message.ContainsSubstring("TheOther"));
		}
	}
}