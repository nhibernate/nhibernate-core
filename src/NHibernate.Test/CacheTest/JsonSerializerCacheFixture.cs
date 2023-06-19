using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Caches.CoreDistributedCache;
using NHibernate.Caches.CoreDistributedCache.Memory;
using NHibernate.Caches.Util.JsonSerializer;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Multi;
using NHibernate.Transform;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class JsonSerializerCacheFixture : TestCase
	{
		protected override string[] Mappings => new[]
		{
			"CacheTest.ReadOnly.hbm.xml",
			"CacheTest.ReadWrite.hbm.xml"
		};

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.CacheProvider, typeof(CoreDistributedCacheProvider).AssemblyQualifiedName);
			CoreDistributedCacheProvider.CacheFactory = new MemoryFactory();
			var serializer = new JsonCacheSerializer();
			serializer.RegisterType(typeof(Tuple<string, object>), "tso");
			CoreDistributedCacheProvider.DefaultSerializer = serializer;
		}

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var totalItems = 6;
				for (var i = 1; i <= totalItems; i++)
				{
					var parent = new ReadOnly
					{
						Name = $"Name{i}"
					};
					for (var j = 1; j <= totalItems; j++)
					{
						var child = new ReadOnlyItem
						{
							Parent = parent
						};
						parent.Items.Add(child);
					}
					s.Save(parent);
				}
				for (var i = 1; i <= totalItems; i++)
				{
					var parent = new ReadWrite
					{
						Name = $"Name{i}"
					};
					for (var j = 1; j <= totalItems; j++)
					{
						var child = new ReadWriteItem
						{
							Parent = parent
						};
						parent.Items.Add(child);
					}
					s.Save(parent);
				}
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from ReadOnlyItem").ExecuteUpdate();
				s.CreateQuery("delete from ReadWriteItem").ExecuteUpdate();
				s.CreateQuery("delete from ReadOnly").ExecuteUpdate();
				s.CreateQuery("delete from ReadWrite").ExecuteUpdate();
				tx.Commit();
			}
			// Must rebuild the session factory, CoreDistribted cache being not clearable.
			RebuildSessionFactory();
		}

		[Test]
		public void CacheableScalarSqlQueryWithTransformer()
		{
			void AssertQuery(bool fromCache)
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				using (EnableStatisticsScope())
				{
					var l = s.CreateSQLQuery("select ro.Name as RegionCode from ReadOnly ro")
							.AddScalar("regionCode", NHibernateUtil.String)
							.SetResultTransformer(Transformers.AliasToBean<ResultDto>())
							.SetCacheable(true)
							.List();
					t.Commit();

					Assert.That(l.Count, Is.EqualTo(6));
					var msg = "Results are expected from " + (fromCache ? "cache" : "DB");
					Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(fromCache ? 0 : 1), msg);
					Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(fromCache ? 1 : 0), msg);
				}
			}

			AssertQuery(false);
			AssertQuery(true);
		}

		[Test]
		public void CacheableScalarSqlMultiQueryWithTransformer()
		{
			void AssertQuery(bool fromCache)
			{
				using (var s = OpenSession())
				using (var t = s.BeginTransaction())
				using (EnableStatisticsScope())
				{
					var q1 = s.CreateSQLQuery("select rw.Name as RegionCode from ReadWrite rw")
							.AddScalar("regionCode", NHibernateUtil.String)
							.SetResultTransformer(Transformers.AliasToBean<ResultDto>())
							.SetCacheable(true);
					var q2 = s.CreateSQLQuery("select rw.Id as OrgId from ReadWrite rw")
							.AddScalar("orgId", NHibernateUtil.Int64)
							.SetResultTransformer(Transformers.AliasToBean<ResultDto>())
							.SetCacheable(true);

					var batch = s.CreateQueryBatch();
					batch.Add<ResultDto>(q1);
					batch.Add<ResultDto>(q2);
					batch.Execute();

					var l1 = batch.GetResult<ResultDto>(0);
					var l2 = batch.GetResult<ResultDto>(1);

					t.Commit();

					Assert.That(l1.Count, Is.EqualTo(6), "Unexpected results count for the first query.");
					Assert.That(l2.Count, Is.EqualTo(6), "Unexpected results count for the second query.");
					var msg = "Results are expected from " + (fromCache ? "cache" : "DB");
					Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(fromCache ? 0 : 2), msg);
					Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(fromCache ? 2 : 0), msg);
				}
			}

			AssertQuery(false);
			AssertQuery(true);
		}

		class ResultDto
		{
			public long OrgId { get; set; }
			public string RegionCode { get; set; }
		}

		[Test]
		public void QueryCacheTest()
		{
			// QueryCache batching is used by QueryBatch.
			if (!Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");

			Sfi.Statistics.Clear();

			using var s = OpenSession();

			const string query = "from ReadOnly e where e.Name = :name";
			const string name1 = "Name1";
			const string name2 = "Name2";
			const string name3 = "Name3";
			const string name4 = "Name4";
			const string name5 = "Name5";
			var q1 =
				s
					.CreateQuery(query)
					.SetString("name", name1)
					.SetCacheable(true);
			var q2 =
				s
					.CreateQuery(query)
					.SetString("name", name2)
					.SetCacheable(true);
			var q3 =
				s
					.Query<ReadWrite>()
					.Where(r => r.Name == name3)
					.WithOptions(o => o.SetCacheable(true));
			var q4 =
				s
					.QueryOver<ReadWrite>()
					.Where(r => r.Name == name4)
					.Cacheable();
			var q5 =
				s
					.CreateSQLQuery("select * " + query)
					.AddEntity(typeof(ReadOnly))
					.SetString("name", name5)
					.SetCacheable(true);

			var queries =
				s
					.CreateQueryBatch()
					.Add<ReadOnly>(q1)
					.Add<ReadOnly>(q2)
					.Add(q3)
					.Add(q4)
					.Add<ReadOnly>(q5);

			using (var t = s.BeginTransaction())
			{
				queries.Execute();
				t.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "queries first execution count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(5), "cache misses first execution");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "cache hits first execution");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(5), "cache puts first execution");

			// Run a second time, to test the query cache
			using (var t = s.BeginTransaction())
			{
				queries.Execute();
				t.Commit();
			}

			Assert.That(
				queries.GetResult<ReadOnly>(0),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name1), "q1");
			Assert.That(
				queries.GetResult<ReadOnly>(1),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name2), "q2");
			Assert.That(
				queries.GetResult<ReadWrite>(2),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name3), "q3");
			Assert.That(
				queries.GetResult<ReadWrite>(3),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name4), "q4");
			Assert.That(
				queries.GetResult<ReadOnly>(4),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name5), "q5");

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "queries second execution count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(5), "cache misses second execution");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(5), "cache hits second execution");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(5), "cache puts second execution");

			// Update an entity to invalidate them
			using (var t = s.BeginTransaction())
			{
				var readwrite1 = s.Query<ReadWrite>().Single(e => e.Name == name3);
				readwrite1.Name = "NewName";
				t.Commit();
			}

			// Run a third time, to re-test the query cache
			using (var t = s.BeginTransaction())
			{
				queries.Execute();
				t.Commit();
			}

			Assert.That(
				queries.GetResult<ReadOnly>(0),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name1), "q1 after update");
			Assert.That(
				queries.GetResult<ReadOnly>(1),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name2), "q2 after update");
			Assert.That(
				queries.GetResult<ReadWrite>(2),
				Has.Count.EqualTo(0), "q3 after update");
			Assert.That(
				queries.GetResult<ReadWrite>(3),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name4), "q4 after update");
			Assert.That(
				queries.GetResult<ReadOnly>(4),
				Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name5), "q5 after update");

			// The two ReadWrite queries should have been re-executed, so count should have been incremented accordingly.
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(3), "queries third execution count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(7), "cache misses third execution");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(8), "cache hits third execution");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(7), "cache puts third execution");
		}

		[Test]
		public void QueryEntityBatchCacheTest()
		{
			Sfi.Statistics.Clear();

			List<ReadOnlyItem> items;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				items = s.Query<ReadOnlyItem>()
						 .WithOptions(o => o.SetCacheable(true))
						 .ToList();

				tx.Commit();
			}

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "query first execution count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(1), "cache misses first execution");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "cache hits first execution");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "cache puts first execution");

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				items = s.Query<ReadOnlyItem>()
						 .WithOptions(o => o.SetCacheable(true))
						 .ToList();

				tx.Commit();
			}

			Assert.That(items, Has.Count.EqualTo(36));

			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "query second execution count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "cache misses second execution");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "cache hits second execution");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "cache puts second execution");
		}

		[TestCase(false)]
		[TestCase(true)]
		public void QueryFetchCollectionBatchCacheTest(bool future)
		{
			if (future && !Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");
			}

			int middleId;

			using (var s = OpenSession())
			{
				var ids = s.Query<ReadOnly>().Select(o => o.Id).OrderBy(o => o).ToList();
				middleId = ids[2];
			}

			Sfi.Statistics.Clear();

			List<ReadOnly> items;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				if (future)
				{
					s.Query<ReadOnly>()
					 .WithOptions(o => o.SetCacheable(true))
					 .FetchMany(o => o.Items)
					 .Where(o => o.Id > middleId)
					 .ToFuture();

					items = s.Query<ReadOnly>()
							 .WithOptions(o => o.SetCacheable(true))
							 .FetchMany(o => o.Items)
							 .Where(o => o.Id <= middleId)
							 .ToFuture()
							 .ToList();
				}
				else
				{
					items = s.Query<ReadOnly>()
							 .WithOptions(o => o.SetCacheable(true))
							 .FetchMany(o => o.Items)
							 .ToList();
				}

				tx.Commit();
			}

			Assert.That(items, Has.Count.EqualTo(future ? 3 : 6), "Unexpected items count");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache miss count");

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				if (future)
				{
					s.Query<ReadOnly>()
					 .WithOptions(o => o.SetCacheable(true))
					 .FetchMany(o => o.Items)
					 .Where(o => o.Id > middleId)
					 .ToFuture();

					items = s.Query<ReadOnly>()
							 .WithOptions(o => o.SetCacheable(true))
							 .FetchMany(o => o.Items)
							 .Where(o => o.Id <= middleId)
							 .ToFuture()
							 .ToList();
				}
				else
				{
					items = s.Query<ReadOnly>()
							 .WithOptions(o => o.SetCacheable(true))
							 .FetchMany(o => o.Items)
							 .ToList();
				}

				tx.Commit();
			}

			Assert.That(items, Has.Count.EqualTo(future ? 3 : 6));
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache hit count");
		}

		[TestCase(false)]
		[TestCase(true)]
		public void QueryFetchEntityBatchCacheTest(bool future)
		{
			if (future && !Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");
			}

			int middleId;

			using (var s = OpenSession())
			{
				var ids = s.Query<ReadOnlyItem>().Select(o => o.Id).OrderBy(o => o).ToList();
				middleId = ids[17];
			}

			Sfi.Statistics.Clear();

			List<ReadOnlyItem> items;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				if (future)
				{
					s.Query<ReadOnlyItem>()
					 .WithOptions(o => o.SetCacheable(true))
					 .Fetch(o => o.Parent)
					 .Where(o => o.Id > middleId)
					 .ToFuture();

					items = s.Query<ReadOnlyItem>()
							 .WithOptions(o => o.SetCacheable(true))
							 .Fetch(o => o.Parent)
							 .Where(o => o.Id <= middleId)
							 .ToFuture()
							 .ToList();
				}
				else
				{
					items = s.Query<ReadOnlyItem>()
							 .WithOptions(o => o.SetCacheable(true))
							 .Fetch(o => o.Parent)
							 .ToList();
				}

				tx.Commit();
			}

			Assert.That(items, Has.Count.EqualTo(future ? 18 : 36), "Unexpected items count");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache miss count");

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				if (future)
				{
					s.Query<ReadOnlyItem>()
					 .WithOptions(o => o.SetCacheable(true))
					 .Fetch(o => o.Parent)
					 .Where(o => o.Id > middleId)
					 .ToFuture();

					items = s.Query<ReadOnlyItem>()
							 .WithOptions(o => o.SetCacheable(true))
							 .Fetch(o => o.Parent)
							 .Where(o => o.Id <= middleId)
							 .ToFuture()
							 .ToList();
				}
				else
				{
					items = s.Query<ReadOnlyItem>()
							 .WithOptions(o => o.SetCacheable(true))
							 .Fetch(o => o.Parent)
							 .ToList();
				}

				tx.Commit();
			}

			Assert.That(items, Has.Count.EqualTo(future ? 18 : 36));
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache hit count");
		}
	}
}
