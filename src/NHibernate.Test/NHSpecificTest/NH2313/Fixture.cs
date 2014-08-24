using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2313
{
	public class Fixture
	{
		[Test]
		public void WhenLoadWorngMappingThenMessageShouldContaingWrongClassName()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2313.Mappings.hbm.xml", GetType().Assembly);
			cfg.Executing(c=> c.BuildSessionFactory()).Throws<MappingException>().And.ValueOf.Message.Should().Contain("TheOther");
		}
	}
}