using System.Linq;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class EagerLoadTests : LinqTestCase
	{
		[Test]
		public void CanSelectAndFetch()
		{
			//NH-3075
			var result = db.Orders
			  .Select(o => o.Customer)
			  .Fetch(c => c.Orders)
			  .ToList();

			session.Close();

			Assert.IsNotEmpty(result);
			Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].Orders));
		}

		[Test]
		public void CanSelectAndFetchMany()
		{
			var result = db.OrderLines
							.Select(o => o.Product)
							.FetchMany(o => o.OrderLines)
							.ToList();

			session.Close();

			Assert.IsNotEmpty(result);
			Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].OrderLines));
		}

		[Test]
		public void CanSelectManyAndFetch()
		{
			var result = db.Orders
							.SelectMany(o => o.OrderLines)
							.Fetch(o => o.Product)
							.ToList();

			session.Close();

			Assert.IsNotEmpty(result);
			Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].Product));
		}

		[Test]
		public void CanSelectManyAndFetchMany()
		{
			var result = db.Employees
							.SelectMany(o => o.Orders)
							.FetchMany(o => o.OrderLines)
							.ToList();

			session.Close();

			Assert.IsNotEmpty(result);
			Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].OrderLines));
		}

		[Test]
		public void CanSelectAndFetchHql()
		{
			//NH-3075
			var result = this.session.CreateQuery("select c from Order o left join o.Customer c left join fetch c.Orders").List<Customer>();

			session.Close();

			Assert.IsNotEmpty(result);
			Assert.IsTrue(NHibernateUtil.IsInitialized(result[0].Orders));
		}

		[Test]
		public void RelationshipsAreLazyLoadedByDefault()
		{
			var x = db.Customers.ToList();

			session.Close();

			Assert.AreEqual(91, x.Count);
			Assert.IsFalse(NHibernateUtil.IsInitialized(x[0].Orders));
		}

		[Test]
		public void RelationshipsCanBeEagerLoaded()
		{
			var x = db.Customers.Fetch(c => c.Orders).ToList();

			session.Close();

			Assert.AreEqual(91, x.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
			Assert.IsFalse(NHibernateUtil.IsInitialized(x[0].Orders.First().OrderLines));
		}

		[Test]
		public void MultipleRelationshipsCanBeEagerLoaded()
		{
			var x = db.Employees.Fetch(e => e.Subordinates).Fetch(e => e.Orders).ToList();

			session.Close();

			Assert.AreEqual(9, x.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
			Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Subordinates));
		}

		[Test]
		public void NestedRelationshipsCanBeEagerLoaded()
		{
			var x = db.Customers.FetchMany(c => c.Orders).ThenFetchMany(o => o.OrderLines).ToList();

			session.Close();

			Assert.AreEqual(91, x.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
			Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders.First().OrderLines));
		}

		[Test]
		public void WhenFetchSuperclassCollectionThenNotThrows()
		{
			// NH-2277
			Assert.That(() => session.Query<Lizard>().Fetch(x => x.Children).ToList(), Throws.Nothing);
			session.Close();
		}

		[Test]
		public void FetchWithWhere()
		{
			// NH-2381 NH-2362
			(from p in session.Query<Product>().Fetch(a => a.Supplier)
			 where p.ProductId == 1
			 select p).ToList();
		}

		[Test]
		public void FetchManyWithWhere()
		{
					// NH-2381 NH-2362
			(from s
				in session.Query<Supplier>().FetchMany(a => a.Products)
			 where s.SupplierId == 1
			 select s).ToList();
		}

		[Test]
		public void FetchAndThenFetchWithWhere()
		{
			// NH-2362
			(from p
				in session.Query<User>().Fetch(a => a.Role).ThenFetch(a => a.Entity)
			 where p.Id == 1
			 select p).ToList();
		}

		[Test]
		public void FetchAndThenFetchManyWithWhere()
		{
			// NH-2362
			(from p
				in session.Query<Employee>().Fetch(a => a.Superior).ThenFetchMany(a => a.Orders)
			 where p.EmployeeId == 1
			 select p).ToList();
		}

		[Test]
		public void FetchManyAndThenFetchWithWhere()
		{
			// NH-2362
			(from s
				in session.Query<Supplier>().FetchMany(a => a.Products).ThenFetch(a => a.Category)
			 where s.SupplierId == 1
			 select s).ToList();
		}

		[Test]
		public void FetchManyAndThenFetchManyWithWhere()
		{
			// NH-2362
			(from s
				in session.Query<Supplier>().FetchMany(a => a.Products).ThenFetchMany(a => a.OrderLines)
			 where s.SupplierId == 1
			 select s).ToList();
		}

		[Test]
		public void WhereBeforeFetchAndOrderBy()
		{
			//NH-2915
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.Fetch(x => x.Customer)
				.OrderBy(x => x.OrderId)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].Customer));
		}
		
		[Test]
		public void WhereBeforeFetchManyAndOrderBy()
		{
			//NH-2915
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.OrderBy(x => x.OrderId)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
		}
		
		[Test]
		public void WhereBeforeFetchManyThenFetchAndOrderBy()
		{
			//NH-2915
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.ThenFetch(x => x.Product)
				.OrderBy(x => x.OrderId)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines.First().Product));
		}

		[Test]
		public void WhereBeforeFetchAndSelect()
		{
			//NH-3056
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.Fetch(x => x.Customer)
				.Select(x => x)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].Customer));
		}
		
		[Test]
		public void WhereBeforeFetchManyAndSelect()
		{
			//NH-3056
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.Select(x => x)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
		}
		
		[Test]
		public void WhereBeforeFetchManyThenFetchAndSelect()
		{
			//NH-3056
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.ThenFetch(x => x.Product)
				.Select(x => x)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines.First().Product));
		}

		[Test]
		public void WhereBeforeFetchAndWhere()
		{
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.Fetch(x => x.Customer)
				.Where(x => true)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].Customer));
		}
		
		[Test]
		public void WhereBeforeFetchManyAndWhere()
		{
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.Where(x => true)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
		}
		
		[Test]
		public void WhereBeforeFetchManyThenFetchAndWhere()
		{
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var orders = db.Orders
				.Where(x => x.OrderId != firstOrderId)
				.FetchMany(x => x.OrderLines)
				.ThenFetch(x => x.Product)
				.Where(x => true)
				.ToList();

			Assert.AreEqual(829, orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines));
			Assert.IsTrue(NHibernateUtil.IsInitialized(orders[0].OrderLines.First().Product));
		}

		[Test]
		public void WhereAfterFetchAndSingleOrDefault()
		{
			//NH-3186
			var firstOrderId = db.Orders.OrderBy(x => x.OrderId)
				.Select(x => x.OrderId)
				.First();

			var order = db.Orders
				.Fetch(x => x.Shipper)
				.SingleOrDefault(x => x.OrderId == firstOrderId);

			Assert.IsTrue(NHibernateUtil.IsInitialized(order.Shipper));
		}

		[Test]
		public void WhereReuseJoins()
		{
			OrderLine orderLine;
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.Customer.ContactName == "Maria Anders")
				              .Fetch(o => o.Order).ThenFetch(o => o.Customer)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.True);
			}

			session.Clear();
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.Customer.ContactName == "Maria Anders")
				              .Fetch(o => o.Order)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.False);
			}

			session.Clear();
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.OrderLines.Any(l => l.Product.Name == "Tofu"))
				              .Fetch(o => o.Order).ThenFetch(o => o.Customer)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				sql = sql.Substring(0, sql.IndexOf("where"));
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.True);
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .Where(o => o.Superior.Superior.Superior.FirstName != null)
				  .Fetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, ","), Is.EqualTo(31), "Only the first level should be fetched.");
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(3));
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .Where(o => o.Superior.FirstName != null)
				  .Fetch(o => o.Superior).ThenFetch(o => o.Superior).ThenFetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public void OrderByReuseJoins()
		{
			OrderLine orderLine;
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.OrderId == 10248)
				              .OrderBy(o => o.Order.Customer.ContactName)
				              .Fetch(o => o.Order).ThenFetch(o => o.Customer)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.True);
			}

			session.Clear();
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.OrderId == 10248)
				              .OrderBy(o => o.Order.Customer.ContactName)
				              .Fetch(o => o.Order)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.False);
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .OrderBy(o => o.Superior.Superior.Superior.FirstName)
				  .Fetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, ","), Is.EqualTo(31), "Only the first level should be fetched.");
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(3));
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .OrderBy(o => o.Superior.FirstName)
				  .Fetch(o => o.Superior).ThenFetch(o => o.Superior).ThenFetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(3));
			}
		}

		[Test]
		public void WhereAndOrderByReuseJoins()
		{
			OrderLine orderLine;
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.Customer.ContactName == "Maria Anders")
				              .OrderBy(o => o.Order.Customer.ContactName)
				              .Fetch(o => o.Order).ThenFetch(o => o.Customer)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.True);
			}

			session.Clear();
			using (var logSpy = new SqlLogSpy())
			{
				orderLine = db.OrderLines
				              .Where(o => o.Order.Customer.ContactName == "Maria Anders")
				              .OrderBy(o => o.Order.Customer.ContactName)
				              .Fetch(o => o.Order)
				              .ToList()
				              .First();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(2));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(orderLine.Order.Customer), Is.False);
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .Where(o => o.Superior.Superior.Superior.FirstName != null)
				  .OrderBy(o => o.Superior.Superior.Superior.FirstName)
				  .Fetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, ","), Is.EqualTo(31), "Only the first level should be fetched.");
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(3));
			}

			using (var logSpy = new SqlLogSpy())
			{
				db.Employees
				  .Where(o => o.Superior.FirstName != null)
				  .OrderBy(o => o.Superior.FirstName)
				  .Fetch(o => o.Superior).ThenFetch(o => o.Superior).ThenFetch(o => o.Superior)
				  .ToList()
				  .FirstOrDefault();

				var sql = logSpy.GetWholeLog();
				Assert.That(GetTotalOccurrences(sql, "join"), Is.EqualTo(3));
				Assert.That(GetTotalOccurrences(sql, "inner join"), Is.EqualTo(1));
				Assert.That(GetTotalOccurrences(sql, "left outer join"), Is.EqualTo(2));
			}
		}

		[Test]
		public void FetchBeforeSelect()
		{
			var result = db.Orders
			               .Where(o => o.OrderId == 10248)
			               .Fetch(x => x.Customer)
			               .Select(x => new {x.Customer.ContactName})
			               .ToList();

			Assert.True(result.Any());
		}
	}
}
