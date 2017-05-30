using System;
using System.Collections;
using System.Threading;
using NHibernate.Stat;
using NUnit.Framework;
using NHibernate.Transform;

namespace NHibernate.Test.SecondLevelCacheTests
{
	[TestFixture]
	public class ScalarQueryFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		public void FillDb(int startId)
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				for (int i = startId; i < startId + 10; i++)
				{
					s.Save(new AnotherItem { Id = i, Name = (i / 2).ToString() });
				}
				tx.Commit();
			}
		}

		public void CleanUp()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from AnotherItem");
				tx.Commit();
			}
		}

		[Test]
		public void ShouldHitCacheUsingNamedQueryWithProjection()
		{
			FillDb(1);
			Sfi.Statistics.Clear();

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.GetNamedQuery("Stat").List();
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));

			Sfi.Statistics.Clear();

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.GetNamedQuery("Stat").List();
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));

			Sfi.Statistics.LogSummary();
			CleanUp();
		}

		[Test]
		public void ShouldHitCacheUsingQueryWithProjection()
		{
			FillDb(1);
			Sfi.Statistics.Clear();

			int resultCount;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				resultCount =
					s.CreateQuery("select ai.Name, count(*) from AnotherItem ai group by ai.Name").SetCacheable(true).SetCacheRegion(
						"Statistics").List().Count;
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));

			Sfi.Statistics.Clear();

			int secondResultCount;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				secondResultCount = s.CreateQuery("select ai.Name, count(*) from AnotherItem ai group by ai.Name")
					.SetCacheable(true).SetCacheRegion("Statistics").List().Count;
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
			Assert.That(secondResultCount, Is.EqualTo(resultCount));

			Sfi.Statistics.LogSummary();
			CleanUp();
		}

		[Test]
		public void QueryCacheInvalidation()
		{
			Sfi.EvictQueries();
			Sfi.Statistics.Clear();

			const string queryString = "from Item i where i.Name='widget'";

			object savedId = CreateItem(queryString);

			QueryStatistics qs = Sfi.Statistics.GetQueryStatistics(queryString);
			EntityStatistics es = Sfi.Statistics.GetEntityStatistics(typeof(Item).FullName);

			Thread.Sleep(200);

			IList result;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				result = s.CreateQuery(queryString).SetCacheable(true).List();
				Assert.That(result.Count, Is.EqualTo(1));
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(0));

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				result = s.CreateQuery(queryString).SetCacheable(true).List();
				Assert.That(result.Count, Is.EqualTo(1));
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(1));
			Assert.That(es.FetchCount, Is.EqualTo(0));

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				result = s.CreateQuery(queryString).SetCacheable(true).List();
				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(NHibernateUtil.IsInitialized(result[0]));
				var i = (Item)result[0];
				i.Name = "Widget";
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(2));
			Assert.That(qs.CacheMissCount, Is.EqualTo(2));
			Assert.That(es.FetchCount, Is.EqualTo(0));

			Thread.Sleep(200);

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();

				var i = s.Get<Item>(savedId);
				Assert.That(i.Name, Is.EqualTo("Widget"));

				s.Delete(i);
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(2));
			Assert.That(qs.CacheMissCount, Is.EqualTo(3));
			Assert.That(qs.CachePutCount, Is.EqualTo(3));
			Assert.That(qs.ExecutionCount, Is.EqualTo(3));
			Assert.That(es.FetchCount, Is.EqualTo(0)); //check that it was being cached
		}

		private object CreateItem(string queryString)
		{
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();
				var i = new Item { Name = "widget" };
				savedId = s.Save(i);
				tx.Commit();
			}
			return savedId;
		}

		[Test]
		public void SimpleProjections()
		{
			var transformer = new CustomTransformer();
			Sfi.EvictQueries();
			Sfi.Statistics.Clear();

			const string queryString = "select i.Name, i.Description from AnotherItem i where i.Name='widget'";

			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();
				var i = new AnotherItem { Name = "widget" };
				savedId = s.Save(i);
				tx.Commit();
			}

			QueryStatistics qs = Sfi.Statistics.GetQueryStatistics(queryString);
			EntityStatistics es = Sfi.Statistics.GetEntityStatistics(typeof(AnotherItem).FullName);

			Thread.Sleep(200);

			IList result;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(0));

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(1));

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).SetResultTransformer(transformer).List();
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(2), "hit count should go up since the cache contains the result before the possible application of a resulttransformer");

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).SetResultTransformer(transformer).List();
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(3), "hit count should go up since we are using the same resulttransformer");
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				result = s.CreateQuery(queryString).SetCacheable(true).List();
				Assert.That(result.Count, Is.EqualTo(1));
				var i = s.Get<AnotherItem>(savedId);
				i.Name = "Widget";
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(4));
			Assert.That(qs.CacheMissCount, Is.EqualTo(2));

			Thread.Sleep(200);

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery(queryString).SetCacheable(true).List();

				var i = s.Get<AnotherItem>(savedId);
				Assert.That(i.Name, Is.EqualTo("Widget"));

				s.Delete(i);
				tx.Commit();
			}

			Assert.That(qs.CacheHitCount, Is.EqualTo(4));
			Assert.That(qs.CacheMissCount, Is.EqualTo(3));
			Assert.That(qs.CachePutCount, Is.EqualTo(3));
			Assert.That(qs.ExecutionCount, Is.EqualTo(3));
			Assert.That(es.FetchCount, Is.EqualTo(0)); //check that it was being cached
		}

		public class CustomTransformer: IResultTransformer
		{
			public object TransformTuple(object[] tuple, string[] aliases)
			{
				return new AnotherItem {Name = tuple[0].ToString(), Description = tuple[1].ToString()};
			}

			public IList TransformList(IList collection)
			{
				return collection;
			}
		}
	}
}