using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2266
{
	public class Fixture
	{
		[Test]
		public void WhenBuildSessionFactoryThenThrows()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2266.Mappings.hbm.xml", GetType().Assembly);
			Assert.That(() => cfg.BuildSessionFactory(), Throws.TypeOf<NotSupportedException>()
															   .And.Message.ContainsSubstring("does not have mapped subclasses")
															   .And.Message.ContainsSubstring(typeof (TemporaryToken).FullName));
		}
	}
}