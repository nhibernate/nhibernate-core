using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class OrderByTests : LinqTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.ShowSql, "true");
		}

		[Test]
		public void GroupByThenOrderBy()
		{
			var query = from c in db.Customers
						group c by c.Address.Country into g
						orderby g.Key
						select new { Country = g.Key, Count = g.Count() };

			var ids = query.ToList();
			Assert.NotNull(ids);
			AssertOrderedBy.Ascending(ids, arg => arg.Country);
		}

		[Test]
		public void AscendingOrderByClause()
		{
			var query = from c in db.Customers
						orderby c.CustomerId
						select c.CustomerId;

			var ids = query.ToList();

			if (ids.Count > 1)
			{
				Assert.Greater(ids[1], ids[0]);
			}
		}

		[Test]
		public void DescendingOrderByClause()
		{
			var query = from c in db.Customers
						orderby c.CustomerId descending
						select c.CustomerId;

			var ids = query.ToList();

			if (ids.Count > 1)
			{
				Assert.Greater(ids[0], ids[1]);
			}
		}

		[Test]
		public void OrderByCalculatedAggregatedSubselectProperty()
		{
			//NH-2781
			var result = db.Orders
				.Select(o => new
								 {
									 o.OrderId,
									 TotalQuantity = o.OrderLines.Sum(c => c.Quantity)
								 })
				.OrderBy(s => s.TotalQuantity)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(830));

			AssertOrderedBy.Ascending(result, s => s.TotalQuantity);
		}

		[Test]
		public void AggregateAscendingOrderByClause()
		{
			var query = from c in db.Customers
						orderby c.Orders.Count
						select c;

			var customers = query.ToList();

			// Verify ordering for first 10 customers - to avoid loading all orders.
			AssertOrderedBy.Ascending(customers.Take(10).ToList(), customer => customer.Orders.Count);
		}

		[Test]
		public void AggregateDescendingOrderByClause()
		{
			var query = from c in db.Customers
						orderby c.Orders.Count descending
						select c;

			var customers = query.ToList();

			// Verify ordering for first 10 customers - to avoid loading all orders.
			AssertOrderedBy.Descending(customers.Take(10).ToList(), customer => customer.Orders.Count);
		}

		[Test]
		public void ComplexAscendingOrderByClause()
		{
			var query = from c in db.Customers
						where c.Address.Country == "Belgium"
						orderby c.Address.Country, c.Address.City
						select c.Address.City;

			var ids = query.ToList();

			if (ids.Count > 1)
			{
				Assert.Greater(ids[1], ids[0]);
			}
		}

		[Test]
		public void ComplexDescendingOrderByClause()
		{
			var query = from c in db.Customers
						where c.Address.Country == "Belgium"
						orderby c.Address.Country descending, c.Address.City descending
						select c.Address.City;

			var ids = query.ToList();

			if (ids.Count > 1)
			{
				Assert.Greater(ids[0], ids[1]);
			}
		}

		[Test]
		public void ComplexAscendingDescendingOrderByClause()
		{
			var query = from c in db.Customers
						where c.Address.Country == "Belgium"
						orderby c.Address.Country ascending, c.Address.City descending
						select c.Address.City;

			var ids = query.ToList();

			if (ids.Count > 1)
			{
				Assert.Greater(ids[0], ids[1]);
			}
		}

		[Test]
		public void OrderByDoesNotFilterResultsOnJoin()
		{
			// Check preconditions.
			var allAnimalsWithNullFather = from a in db.Animals where a.Father == null select a;
			Assert.Greater(allAnimalsWithNullFather.Count(), 0);
			// Check join result.
			var allAnimals = db.Animals;
			var orderedAnimals = from a in db.Animals orderby a.Father.SerialNumber select a;
			// ReSharper disable RemoveToList.2
			// We to ToList() first or it skips the generation of the joins.
			Assert.AreEqual(allAnimals.ToList().Count(), orderedAnimals.ToList().Count());
			// ReSharper restore RemoveToList.2
		}

		[Test]
		public void OrderByWithSelfReferencedSubquery1()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore("On Oracle this generates a correlated subquery two levels deep which isn't supported until Oracle 10g.");

			//NH-3044
			var result = (from order in db.Orders
						  where order == db.Orders.OrderByDescending(x => x.OrderDate).First(x => x.Customer == order.Customer)
						  orderby order.Customer.CustomerId
						  select order).ToList();

			AssertOrderedBy.Ascending(result.Take(5).ToList(), x => x.Customer.CustomerId);
		}

		[Test]
		public void OrderByWithSelfReferencedSubquery2()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore("On Oracle this generates a correlated subquery two levels deep which isn't supported until Oracle 10g.");

			//NH-3044
			var result = (from order in db.Orders
						  where order == db.Orders.OrderByDescending(x => x.OrderDate).First(x => x.Customer == order.Customer)
						  orderby order.ShippingDate descending
						  select order).ToList();

			// Different databases may sort null either first or last.
			// We only bother about the non-null values here.
			result = result.Where(x => x.ShippingDate != null).ToList();

			AssertOrderedBy.Descending(result.Take(5).ToList(), x => x.ShippingDate);
		}

		[Test(Description = "NH-3217")]
		public void OrderByNullCompareAndSkipAndTake()
		{
			db.Orders.OrderBy(o => o.Shipper == null ? 0 : o.Shipper.ShipperId).Skip(3).Take(4).ToList();
		}

		[Test(Description = "NH-3445"), KnownBug("NH-3445")]
		public void OrderByWithSelectDistinctAndTake()
		{
			db.Orders.Select(o => o.ShippedTo).Distinct().OrderBy(o => o).Take(1000).ToList();
		}
	}
}