using System;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2892
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty("hbm2ddl.keywords", "auto-quote");
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			{
				var order = new Order();

				session.Save(order);
				session.Flush();

				var orderLine = new OrderLine
				{
					Orders = order
				};

				session.Save(orderLine);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete($"from {nameof(OrderLine)}");
				session.Delete($"from {nameof(Order)}");
				session.Flush();
			}
		}

		[Test]
		public void SelectOrderLineFromOrder()
		{
			using (ISession session = OpenSession())
			{
				var order = session.Query<Order>().FirstOrDefault();

				Assert.That(order, Is.Not.Null);
				Assert.DoesNotThrow(() => order.Elements.FirstOrDefault());
				Assert.That(order.OrderLines, Is.Not.Null);
				Assert.NotZero(order.OrderLines.Count);
				Assert.NotZero(order.OrderLines.First().Id);
			}
		}
	}
}
