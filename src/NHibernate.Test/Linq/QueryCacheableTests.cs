using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryCacheableTests : LinqTestCase
	{
		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.UseQueryCache, "true");
			cfg.SetProperty(Environment.GenerateStatistics, "true");
			base.Configure(cfg);
		}

		[Test]
		public void QueryIsCacheable()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			var x = (from c in db.Customers
					 select c)
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			var x2 = (from c in db.Customers
					  select c)
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void QueryIsCacheable2()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			var x = (from c in db.Customers
					 select c)
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			var x2 = (from c in db.Customers
					  select c).ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "Unexpected cache hit count");
		}

		[Test]
		public void QueryIsCacheable3()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			var x = (from c in db.Customers.WithOptions(o => o.SetCacheable(true))
					 select c).ToList();

			var x2 = (from c in db.Customers
					  select c).ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "Unexpected cache hit count");
		}

		[Test]
		public void QueryIsCacheableWithRegion()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			Sfi.EvictQueries("test");
			Sfi.EvictQueries("other");

			var x = (from c in db.Customers
					 select c)
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("test"))
				.ToList();

			var x2 = (from c in db.Customers
					  select c)
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("test"))
				.ToList();

			var x3 = (from c in db.Customers
					  select c)
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("other"))
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void CacheableBeforeOtherClauses()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			db.Customers
				.WithOptions(o => o.SetCacheable(true))
				.Where(c => c.ContactName != c.CompanyName).Take(1).ToList();
			db.Customers.Where(c => c.ContactName != c.CompanyName).Take(1).ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "Unexpected cache hit count");
		}

		[Test]
		public void CacheableRegionBeforeOtherClauses()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			Sfi.EvictQueries("test");
			Sfi.EvictQueries("other");

			db.Customers
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("test"))
				.Where(c => c.ContactName != c.CompanyName).Take(1)
				.ToList();
			db.Customers
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("test"))
				.Where(c => c.ContactName != c.CompanyName).Take(1)
				.ToList();
			db.Customers
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("other"))
				.Where(c => c.ContactName != c.CompanyName).Take(1)
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void CacheableRegionSwitched()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			Sfi.EvictQueries("test");

			db.Customers
				.Where(c => c.ContactName != c.CompanyName).Take(1)
				.WithOptions(o => o.SetCacheable(true).SetCacheRegion("test"))
				.ToList();

			db.Customers
				.Where(c => c.ContactName != c.CompanyName).Take(1)
				.WithOptions(o => o.SetCacheRegion("test").SetCacheable(true))
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void GroupByQueryIsCacheable()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			var c = db
				.Customers
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			c = db
				.Customers
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.ToList();

			c = db
				.Customers
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void GroupByQueryIsCacheable2()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			var c = db
				.Customers
				.WithOptions(o => o.SetCacheable(true))
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.ToList();

			c = db
				.Customers
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.ToList();

			c = db
				.Customers
				.WithOptions(o => o.SetCacheable(true))
				.GroupBy(x => x.Address.Country)
				.Select(x => x.Key)
				.ToList();

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[Test]
		public void CanBeCombinedWithFetch()
		{
			//NH-2587
			//NH-3982 (GH-1372)

			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			db.Customers
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			db.Orders
				.WithOptions(o => o.SetCacheable(true))
				.ToList();

			db.Customers
			   .WithOptions(o => o.SetCacheable(true))
				.Fetch(x => x.Orders)
				.ToList();

			db.Orders
				.WithOptions(o => o.SetCacheable(true))
				.Fetch(x => x.OrderLines)
				.ToList();

			var customer = db.Customers
				.WithOptions(o => o.SetCacheable(true))
				.Fetch(x => x.Address)
				.Where(x => x.CustomerId == "VINET")
				.SingleOrDefault();

			customer = db.Customers
				.WithOptions(o => o.SetCacheable(true))
				.Fetch(x => x.Address)
				.Where(x => x.CustomerId == "VINET")
				.SingleOrDefault();

			Assert.That(NHibernateUtil.IsInitialized(customer.Address), Is.True, "Expected the fetched Address to be initialized");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(5), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(5), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}
	}
}
