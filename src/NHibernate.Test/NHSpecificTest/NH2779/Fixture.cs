using log4net;
using log4net.Core;
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
				Order order = new Order { OrderId = "Order-1", InternalOrderId = 1 };
				session.Save(order);
				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			using (new LogSpy(LogManager.GetLogger(typeof(Fixture).Assembly, "NHibernate"), Level.All)) //  <-- Logging must be set DEBUG to reproduce bug
			{
				Order order = session.Get<Order>("Order-1");
				Assert.IsNotNull(order);

				// Cleanup
				session.Delete(order);
				tx.Commit();
			}
		}
	}
}
