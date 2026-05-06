using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3609
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var order = new Order
			{
				UniqueId = "0ab92479-8a17-4dbc-9bef-ce4344940cec",
				CreatedDate = new DateTime(2024, 09, 24)
			};
			session.Save(order);
			session.Save(new LineItem { Order = order, ItemName = "Bananas", Amount = 5 });
			session.Save(new CleanLineItem { Order = order, ItemName = "Bananas", Amount = 5 });

			order = new Order
			{
				UniqueId = "4ca17d84-97aa-489f-8701-302a3879a388",
				CreatedDate = new DateTime(2021, 09, 19)
			};
			session.Save(order);
			session.Save(new LineItem { Order = order, ItemName = "Apples", Amount = 10 });
			session.Save(new CleanLineItem { Order = order, ItemName = "Apples", Amount = 10 });

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from CleanLineItem").ExecuteUpdate();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void QueryWithAny()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			// This form of query is how we first discovered the issue.  This is a simplified reproduction of the
			// sort of Linq that we were using in our app.  It seems to occur when we force an EXISTS( ... ) subquery.
			var validOrders = session.Query<Order>().Where(x => x.CreatedDate > new DateTime(2024, 9, 10));
			var orderCount = session.Query<LineItem>().Count(x => validOrders.Any(y => y == x.Order));

			Assert.That(orderCount, Is.EqualTo(1));
			transaction.Commit();
		}

		[Test]
		public void QueryWithAnyOnCleanLines()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			// This form of query is how we first discovered the issue.  This is a simplified reproduction of the
			// sort of Linq that we were using in our app.  It seems to occur when we force an EXISTS( ... ) subquery.
			var validOrders = session.Query<Order>().Where(x => x.CreatedDate > new DateTime(2024, 9, 10));
			var orderCount = session.Query<CleanLineItem>().Count(x => validOrders.Any(y => y == x.Order));

			Assert.That(orderCount, Is.EqualTo(1));
			transaction.Commit();
		}

		[Test]
		public void QueryWithContains()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var validOrders = session.Query<Order>().Where(x => x.CreatedDate > new DateTime(2024, 9, 10));
			var orderCount = session.Query<LineItem>().Count(x => validOrders.Contains(x.Order));

			Assert.That(orderCount, Is.EqualTo(1));
			transaction.Commit();
		}

		[Test]
		public void SimpleQueryForDataWhichWasInsertedViaAdoShouldProvideExpectedResults()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			// This style of equivalent query does not exhibit the problem.  This test passes no matter which NH version.
			var lineItem = session.Query<LineItem>().FirstOrDefault(x => x.Order.CreatedDate > new DateTime(2024, 9, 10));
			Assert.That(lineItem, Is.Not.Null);
			transaction.Commit();
		}
	}
}
