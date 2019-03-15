﻿using System.Linq;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
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

		[Test]
		public void FetchIsCachable()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();

			Order order;

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				order = s.Query<Order>()
				         .WithOptions(o => o.SetCacheable(true))
				         .Fetch(x => x.Customer)
				         .FetchMany(x => x.OrderLines)
				         .ThenFetch(x => x.Product)
				         .ThenFetchMany(x => x.OrderLines)
				         .Where(x => x.OrderId == 10248)
				         .ToList()
				         .First();

				t.Commit();
			}

			AssertFetchedOrder(order);

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(1), "Unexpected cache miss count");

			Sfi.Statistics.Clear();

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				order = s.Query<Order>()
				         .WithOptions(o => o.SetCacheable(true))
				         .Fetch(x => x.Customer)
				         .FetchMany(x => x.OrderLines)
				         .ThenFetch(x => x.Product)
				         .ThenFetchMany(x => x.OrderLines)
				         .Where(x => x.OrderId == 10248)
				         .ToList()
				         .First();
				t.Commit();
			}

			AssertFetchedOrder(order);

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");

		}

		[Test]
		public void FutureFetchIsCachable()
		{
			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			var multiQueries = Sfi.ConnectionProvider.Driver.SupportsMultipleQueries;

			Order order;

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Query<Order>()
				 .WithOptions(o => o.SetCacheable(true))
				 .Fetch(x => x.Customer)
				 .Where(x => x.OrderId == 10248)
				 .ToFuture();

				order = s.Query<Order>()
				         .WithOptions(o => o.SetCacheable(true))
				         .FetchMany(x => x.OrderLines)
				         .ThenFetch(x => x.Product)
				         .ThenFetchMany(x => x.OrderLines)
				         .Where(x => x.OrderId == 10248)
				         .ToFuture()
				         .ToList()
				         .First();

				t.Commit();
			}

			AssertFetchedOrder(order);

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(multiQueries ? 1 : 2), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(2), "Unexpected cache miss count");

			Sfi.Statistics.Clear();

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Query<Order>()
				 .WithOptions(o => o.SetCacheable(true))
				 .Fetch(x => x.Customer)
				 .Where(x => x.OrderId == 10248)
				 .ToFuture();

				order = s.Query<Order>()
				         .WithOptions(o => o.SetCacheable(true))
				         .FetchMany(x => x.OrderLines)
				         .ThenFetch(x => x.Product)
				         .ThenFetchMany(x => x.OrderLines)
				         .Where(x => x.OrderId == 10248)
				         .ToFuture()
				         .ToList()
				         .First();

				t.Commit();
			}

			AssertFetchedOrder(order);

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2), "Unexpected cache hit count");
		}

		private static void AssertFetchedOrder(Order order)
		{
			Assert.That(NHibernateUtil.IsInitialized(order.Customer), Is.True, "Expected the fetched Customer to be initialized");
			Assert.That(NHibernateUtil.IsInitialized(order.OrderLines), Is.True, "Expected the fetched  OrderLines to be initialized");
			Assert.That(order.OrderLines, Has.Count.EqualTo(3), "Expected the fetched OrderLines to have 3 items");
			var orderLine = order.OrderLines.First();
			Assert.That(NHibernateUtil.IsInitialized(orderLine.Product), Is.True, "Expected the fetched Product to be initialized");
			Assert.That(NHibernateUtil.IsInitialized(orderLine.Product.OrderLines), Is.True, "Expected the fetched OrderLines to be initialized");
		}
	}
}
