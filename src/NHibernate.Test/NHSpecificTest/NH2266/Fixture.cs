using System;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2266
{
	public class Fixture
	{
		[Test]
		public void WhenBuildSessionFactoryThenThrows()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2266.Mappings.hbm.xml", GetType().Assembly);
			cfg.Executing(c => c.BuildSessionFactory()).Throws<NotSupportedException>()
				.And.ValueOf.Message.Should().Contain("does not have mapped subclasses").And.Contain(typeof(TemporaryToken).FullName);
		}
	}
}