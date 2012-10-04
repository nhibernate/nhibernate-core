using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class PagingTests : LinqTestCase
	{
		[Test]
		public void Customers1to5()
		{
			var q = (from c in db.Customers select c.CustomerId).Take(5);
			var query = q.ToList();

			Assert.AreEqual(5, query.Count);
		}

		[Test]
		public void Customers11to20()
		{
			var query = (from c in db.Customers
						 orderby c.CustomerId
						 select c.CustomerId).Skip(10).Take(10).ToList();
			Assert.AreEqual(query[0], "BSBEV");
			Assert.AreEqual(10, query.Count);
		}

		[Test]
		public void Customers11to20And21to30ShouldNoCacheQuery()
		{
			var query = (from c in db.Customers
							orderby c.CustomerId
							select c.CustomerId).Skip(10).Take(10).ToList();
			Assert.AreEqual(query[0], "BSBEV");
			Assert.AreEqual(10, query.Count);

			query = (from c in db.Customers
						orderby c.CustomerId
						select c.CustomerId).Skip(20).Take(10).ToList();
			Assert.AreNotEqual(query[0], "BSBEV");
			Assert.AreEqual(10, query.Count);

			query = (from c in db.Customers
						orderby c.CustomerId
						select c.CustomerId).Skip(10).Take(20).ToList();
			Assert.AreEqual(query[0], "BSBEV");
			Assert.AreEqual(20, query.Count);
		}

		[Test]
		[Ignore("Multiple Takes (or Skips) not handled correctly")]
		public void CustomersChainedTake()
		{
			var q = (from c in db.Customers
					 orderby c.CustomerId
					 select c.CustomerId).Take(5).Take(6);
			
			var query = q.ToList();

			Assert.AreEqual(5, query.Count);
			Assert.AreEqual("ALFKI", query[0]);
			Assert.AreEqual("BLAUS", query[4]);
		}

		[Test]
		[Ignore("Multiple Takes (or Skips) not handled correctly")]
		public void CustomersChainedSkip()
		{
			var q = (from c in db.Customers select c.CustomerId).Skip(10).Skip(5);
			var query = q.ToList();
			Assert.AreEqual(query[0], "CONSH");
			Assert.AreEqual(76, query.Count);
		}

		[Test]
		[Ignore("Count with Skip or Take is incorrect (Skip / Take done on the query not the HQL, so get applied at the wrong point")]
		public void CountAfterTakeShouldReportTheCorrectNumber()
		{
			var users = db.Customers.Skip(3).Take(10);
			Assert.AreEqual(10, users.Count());
		}

		[Test]
		public void OrderedPagedProductsWithOuterProjection()
		{
			//NH-3108
			var inMemoryIds = db.Products.ToList()
				.OrderBy(p => p.ProductId)
				.Skip(10).Take(20)
				.Select(p => p.ProductId)
				.ToList();

			var ids = db.Products 
				.OrderBy(p => p.ProductId) 
				.Skip(10).Take(20) 
				.Select(p => p.ProductId) 
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void OrderedPagedProductsWithInnerProjection()
		{
			//NH-3108 (not failing)
			var inMemoryIds = db.Products.ToList() 
				.OrderBy(p => p.ProductId) 
				.Select(p => p.ProductId)
				.Skip(10).Take(20)
				.ToList();

			var ids = db.Products 
				.OrderBy(p => p.ProductId) 
				.Select(p => p.ProductId)
				.Skip(10).Take(20)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void DescendingOrderedPagedProductsWithOuterProjection()
		{
			//NH-3108
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(p => p.ProductId)
				.Skip(10).Take(20)
				.Select(p => p.ProductId)
				.ToList();

			var ids = db.Products
				.OrderByDescending(p => p.ProductId) 
				.Skip(10).Take(20) 
				.Select(p => p.ProductId) 
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void DescendingOrderedPagedProductsWithInnerProjection()
		{
			//NH-3108 (not failing)
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(p => p.ProductId) 
				.Select(p => p.ProductId)
				.Skip(10).Take(20)
				.ToList();

			var ids = db.Products
				.OrderByDescending(p => p.ProductId) 
				.Select(p => p.ProductId)
				.Skip(10).Take(20)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void PagedProductsWithOuterWhereClause()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.ToList();

			var ids = db.Products
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void PagedProductsWithOuterWhereClauseResort()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.OrderBy(x => x.Name)
				.ToList();

			var ids = db.Products
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.OrderBy(x => x.Name)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void PagedProductsWithInnerAndOuterWhereClauses()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.Where(x => x.UnitsInStock < 100)
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.OrderBy(x => x.Name)
				.ToList();

			var ids = db.Products
				.Where(x => x.UnitsInStock < 100)
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.OrderBy(x => x.Name)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void PagedProductsWithOuterWhereClauseEquivalent()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.ToList();

			var subquery = db.Products
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20);

			var ids = db.Products
				.OrderByDescending(x => x.ProductId)
				.Where(x => subquery.Contains(x))
				.Where(x => x.UnitsInStock > 0);

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}

		[Test]
		public void PagedProductsWithOuterWhereClauseAndProjection()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.Select(x => x.ProductId)
				.ToList();

			var ids = db.Products
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.Select(x => x.ProductId)
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}
		
		[Test]
		public void PagedProductsWithOuterWhereClauseAndComplexProjection()
		{
			//NH-2588
			var inMemoryIds = db.Products.ToList()
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.Select(x => new { x.ProductId })
				.ToList();

			var ids = db.Products
				.OrderByDescending(x => x.ProductId)
				.Skip(10).Take(20)
				.Where(x => x.UnitsInStock > 0)
				.Select(x => new { x.ProductId })
				.ToList();

			Assert.That(ids, Is.EqualTo(inMemoryIds));
		}
	}
}