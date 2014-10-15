using System;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2779
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Order order = new Order() { OrderId = "Order-1", InternalOrderId = 1 };
				session.Save(order);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			using (LogSpy logSpy = new LogSpy()) //  <-- Logging must be set DEBUG to reproduce bug
			{
				Order order = session.Get<Order>("Order-1");
				Assert.IsNotNull(order);

				// Cleanup
				session.Delete(order);
				tx.Commit();
			}
		}

		public class LogSpy : IDisposable
		{
			private readonly DebugAppender appender;
			private readonly Logger loggerImpl;

			public LogSpy()
			{
				appender = new DebugAppender
				{
					Layout = new PatternLayout("%message"),
					Threshold = Level.All
				};
				loggerImpl = (Logger)LogManager.GetLogger("NHibernate").Logger;
				loggerImpl.AddAppender(appender);
				loggerImpl.Level = Level.All;
			}

			public void Dispose()
			{
				loggerImpl.RemoveAppender(appender);
			}
		}
	}
}
