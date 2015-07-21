using log4net.Config;
using log4net.Core;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1587
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Bug()
		{
			XmlConfigurator.Configure();
			var cfg = new Configuration();
			if (TestConfigurationHelper.hibernateConfigFile != null)
				cfg.Configure(TestConfigurationHelper.hibernateConfigFile);
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1587.Mappings.hbm.xml", GetType().Assembly);
			cfg.Configure();

			bool useOptimizer= false;
			using (var ls = new LogSpy("NHibernate.Tuple.Entity.PocoEntityTuplizer"))
			{
				cfg.BuildSessionFactory();
				foreach (LoggingEvent loggingEvent in ls.Appender.GetEvents())
				{
					if (((string)(loggingEvent.MessageObject)).StartsWith("Create Instantiator using optimizer"))
					{
						useOptimizer = true;
						break;
					}
				}
			}
			Assert.That(useOptimizer);
		}
	}
}