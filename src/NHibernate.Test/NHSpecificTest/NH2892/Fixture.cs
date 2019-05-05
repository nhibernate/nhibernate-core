using System;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;
using static NUnit.Framework.Assert;

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
				session.CreateQuery($"delete from {nameof(OrderLine)}").ExecuteUpdate();
				session.CreateSQLQuery($"delete from \"{nameof(Order)}\"").ExecuteUpdate();
			}
		}

		[Test]
		public void SelectOrderLineFromOrder()
		{
			using (ISession session = OpenSession())
			{
				var order = session.Query<Order>().FirstOrDefault();

				That(order, Is.Not.Null);

				That(order.OrderLines, Is.Not.Null);
				NotZero(order.OrderLines.Count);
				NotZero(order.OrderLines.First().Id);
				AreEqual(order.Id, order.OrderLines.First().Id);
			}
		}
	}
}
