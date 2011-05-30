using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2736
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				{
					SalesOrder order = new SalesOrder() { Number = 1 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					SalesOrder order = new SalesOrder() { Number = 2 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					SalesOrder order = new SalesOrder() { Number = 3 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				{
					SalesOrder order = new SalesOrder() { Number = 4 };
					order.Items.Add(new Item { SalesOrder = order, Quantity = 1 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 2 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 3 });
					order.Items.Add(new Item { SalesOrder = order, Quantity = 4 });
					session.Persist(order);
				}
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Item").ExecuteUpdate();
				session.CreateQuery("delete from SalesOrder").ExecuteUpdate();
				transaction.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void TestHqlParametersWithTake()
		{
			using (ISession session = Sfi.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var query = session.CreateQuery("select o.Id, i.Id from SalesOrder o left join o.Items i with i.Quantity = :pQuantity take :pTake");
				query.SetParameter("pQuantity", 1);
				query.SetParameter("pTake", 2);
				var result = query.List();
				Assert.That(result.Count, Is.EqualTo(2));
			}
		}
	}
}
