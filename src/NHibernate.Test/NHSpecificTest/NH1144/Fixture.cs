using log4net.Core;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1144
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Configuration configuration;

		protected override void Configure(Configuration configuration)
		{
			this.configuration = configuration;
			this.configuration.Properties[Environment.BatchSize] = "10";
		}

		[Test]
		public void CanSaveInSingleBatch()
		{
			if (!typeof(OracleDataClientDriver).IsAssignableFrom(ReflectHelper.ClassForName(cfg.GetProperty(Environment.ConnectionDriver))))
			{
				Assert.Ignore("Only applicable for Oracle Data Client driver");
			}

			MainClass[] mc = new MainClass[]
			                 	{
			                 		new MainClass("d0"), new MainClass("d0"), new MainClass("d1"), new MainClass("d1"),
			                 		new MainClass("d1")
			                 	};

			bool executedBatch = false;

			using (LogSpy spy = new LogSpy(typeof (AbstractBatcher)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						foreach (MainClass mainClass in mc)
						{
							s.Save(mainClass);
						}

						tx.Commit();
						foreach (LoggingEvent loggingEvent in spy.Appender.GetEvents())
						{
							if ("Executing batch".Equals(loggingEvent.RenderedMessage))
							{
								executedBatch = true;
								break;
							}
						}
					}
				}
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from MainClass");
					tx.Commit();
				}
			}

			Assert.IsTrue(executedBatch);
		}
	}
}
