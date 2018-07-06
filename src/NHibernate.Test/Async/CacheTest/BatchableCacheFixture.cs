﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Multi;
using NHibernate.Test.CacheTest.Caches;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class BatchableCacheFixtureAsync : TestCase
	{
		protected override string[] Mappings => new[]
		{
			"CacheTest.ReadOnly.hbm.xml",
			"CacheTest.ReadWrite.hbm.xml"
		};

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string CacheConcurrencyStrategy => null;

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.CacheProvider, typeof(BatchableCacheProvider).AssemblyQualifiedName);
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
			// Must manually evict "readonly" entities since their caches are readonly
			Sfi.Evict(typeof(ReadOnly));
			Sfi.Evict(typeof(ReadOnlyItem));
			Sfi.EvictQueries();
		}

		[Test]
		public async Task MultipleGetReadOnlyCollectionTestAsync()
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var ids = new List<int>();
			
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = await (s.Query<ReadOnly>().ToListAsync());
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				await (tx.CommitAsync());
			}

			// Batch size 5
			var testCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultInitializeCollectionEventListener and the other time in BatchingCollectionInitializer.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3, 4}, // triggered by InitializeCollectionFromCache method of DefaultInitializeCollectionEventListener type
						new[] {1, 2, 3, 4, 5}, // triggered by Initialize method of BatchingCollectionInitializer type
					},
					new[] {0, 1, 2, 3, 4},
					null
				),
				// When there are not enough uninitialized collections after the demanded one to fill the batch,
				// the nearest before the demanded collection are added.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3, 2, 1},
						new[] {5, 3, 2, 1, 0},
					},
					new[] {1, 2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3, 2, 1},
						new[] {4, 3, 2, 1, 0},
					},
					new[] {1, 2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3, 4} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new[]
					{
						new[] {1, 2, 3, 4, 5}, // 2 and 4 get assembled inside InitializeCollectionFromCache
						new[] {3, 5, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3, 2, 1}, // 4 and 2 get assembled inside InitializeCollectionFromCache
						new[] {3, 1, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3, 4}, // 1 and 3 get assembled inside InitializeCollectionFromCache
						new[] {2, 4, 5}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3, 2, 1}, // 5, 3 and 1 get assembled inside InitializeCollectionFromCache
						new[] {2, 0}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				)
			};

			foreach (var tuple in testCases)
			{
				await (AssertMultipleCacheCollectionCallsAsync(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
			}
		}

		[Test]
		public async Task MultipleGetReadOnlyTestAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnly).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var ids = new List<int>();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = await (s.Query<ReadOnly>().ToListAsync());
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				await (tx.CommitAsync());
			}
			// Batch size 3
			var parentTestCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2}, // triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {1, 2, 3}, // triggered by Load method of BatchingEntityLoader type
					},
					new[] {0, 1, 2},
					null
				),
				// When there are not enough uninitialized entities after the demanded one to fill the batch,
				// the nearest before the demanded entity are added.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3},
						new[] {4, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new[]
					{
						new[] {1, 2, 3}, // 2 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 4, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 2, 1}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2}, // 1 gets assembled inside LoadFromSecondLevelCache
						new[] {2, 3, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				)
			};

			foreach (var tuple in parentTestCases)
			{
				await (AssertMultipleCacheCallsAsync<ReadOnly>(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
			}
		}

		[Test]
		public async Task MultipleGetReadOnlyItemTestAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var ids = new List<int>();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = await (s.Query<ReadOnlyItem>().Take(6).ToListAsync());
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				await (tx.CommitAsync());
			}
			// Batch size 4
			var parentTestCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3}, // triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {1, 2, 3, 4}, // triggered by Load method of BatchingEntityLoader type
					},
					new[] {0, 1, 2, 3},
					null
				),
				// When there are not enough uninitialized entities after the demanded one to fill the batch,
				// the nearest before the demanded entity are added.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3, 2},
						new[] {5, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new[]
					{
						new[] {1, 2, 3, 4}, // 2 and 4 get assembled inside LoadFromSecondLevelCache
						new[] {3, 5, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new[]
					{
						new[] {5, 4, 3, 2}, // 4 and 2 get assembled inside LoadFromSecondLevelCache
						new[] {3, 1, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new[]
					{
						new[] {0, 1, 2, 3}, // 1 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 4, 5}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new[]
					{
						new[] {4, 5, 3, 2}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				)
			};

			foreach (var tuple in parentTestCases)
			{
				await (AssertMultipleCacheCallsAsync<ReadOnlyItem>(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
			}
		}

		[Test]
		public async Task MultiplePutReadWriteTestAsync()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadWrite).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			await (cache.ClearAsync(CancellationToken.None));
			cache.ClearStatistics();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = await (s.Query<ReadWrite>().ToListAsync());
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				await (tx.CommitAsync());
			}
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(2));

			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.PutMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.LockMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.UnlockMultipleCalls
			);
		}

		[Test]
		public async Task MultiplePutReadWriteItemTestAsync()
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadWrite).FullName}.Items");
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			await (cache.ClearAsync(CancellationToken.None));
			cache.ClearStatistics();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = await (s.Query<ReadWrite>().ToListAsync());
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));

				// Initialize the first item collection
				await (NHibernateUtil.InitializeAsync(items.First(o => o.Id == ids[0]).Items));
				await (tx.CommitAsync());
			}
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
			// Called in: DefaultInitializeCollectionEventListener, BatchingCollectionInitializer and ReadWriteCache
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(3));

			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.PutMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.LockMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.UnlockMultipleCalls
			);
		}

		[Test]
		public async Task UpdateTimestampsCacheTestAsync()
		{
			var timestamp = Sfi.UpdateTimestampsCache;
			var fieldReadonly = typeof(UpdateTimestampsCache).GetField(
				"_batchReadOnlyUpdateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(fieldReadonly, Is.Not.Null, "Unable to find _batchReadOnlyUpdateTimestamps field");
			Assert.That(fieldReadonly.GetValue(timestamp), Is.Not.Null, "_batchReadOnlyUpdateTimestamps is null");
			var field = typeof(UpdateTimestampsCache).GetField(
				"_batchUpdateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(field, Is.Not.Null, "Unable to find _batchUpdateTimestamps field");
			var cache = (BatchableCache) field.GetValue(timestamp);
			Assert.That(cache, Is.Not.Null, "_batchUpdateTimestamps is null");

			await (cache.ClearAsync(CancellationToken.None));
			cache.ClearStatistics();

			const string query = "from ReadOnly e where e.Name = :name";
			const string name = "Name1";
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s
					.CreateQuery(query)
					.SetString("name", name)
					.SetCacheable(true)
					.UniqueResultAsync());
				await (t.CommitAsync());
			}

			// Run a second time, to test the query cache
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var result =
					await (s
						.CreateQuery(query)
						.SetString("name", name)
						.SetCacheable(true)
						.UniqueResultAsync());

				Assert.That(result, Is.Not.Null);
				await (t.CommitAsync());
			}

			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "GetMany");
			Assert.That(cache.GetCalls, Has.Count.EqualTo(0), "Get");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(0), "PutMany");
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "Put");

			// Update entities to put some update ts
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var readwrite1 = await (s.Query<ReadWrite>().FirstAsync());
				readwrite1.Name = "NewName";
				await (t.CommitAsync());
			}
			// PreInvalidate + Invalidate => 2 calls
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(2), "PutMany after update");
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "Put after update");
		}

		[Test]
		public async Task QueryCacheTestAsync()
		{
			// QueryCache batching is used by QueryBatch.
			if (!Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");

			var queryCache = Sfi.GetQueryCache(null);
			var readonlyField = typeof(StandardQueryCache).GetField(
				"_batchableReadOnlyCache",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(readonlyField, Is.Not.Null, "Unable to find _batchableReadOnlyCache field");
			Assert.That(readonlyField.GetValue(queryCache), Is.Not.Null, "_batchableReadOnlyCache is null");
			var field = typeof(StandardQueryCache).GetField(
				"_batchableCache",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(field, Is.Not.Null, "Unable to find _batchableCache field");
			var cache = (BatchableCache) field.GetValue(queryCache);
			Assert.That(cache, Is.Not.Null, "_batchableCache is null");

			var timestamp = Sfi.UpdateTimestampsCache;
			var tsField = typeof(UpdateTimestampsCache).GetField(
				"_batchUpdateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(tsField, Is.Not.Null, "Unable to find _batchUpdateTimestamps field");
			var tsCache = (BatchableCache) tsField.GetValue(timestamp);
			Assert.That(tsCache, Is.Not.Null, "_batchUpdateTimestamps is null");

			await (cache.ClearAsync(CancellationToken.None));
			cache.ClearStatistics();
			await (tsCache.ClearAsync(CancellationToken.None));
			tsCache.ClearStatistics();

			using (var s = OpenSession())
			{
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
					await (queries.ExecuteAsync(CancellationToken.None));
					await (t.CommitAsync());
				}

				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "cache GetMany first execution");
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0), "cache Get first execution");
				Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "cache PutMany first execution");
				Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "cache Put first execution");

				Assert.That(tsCache.GetMultipleCalls, Has.Count.EqualTo(0), "tsCache GetMany first execution");
				Assert.That(tsCache.GetCalls, Has.Count.EqualTo(0), "tsCache Get first execution");

				// Run a second time, to test the query cache
				using (var t = s.BeginTransaction())
				{
					await (queries.ExecuteAsync(CancellationToken.None));
					await (t.CommitAsync());
				}

				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(0, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name1), "q1");
				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(1, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name2), "q2");
				Assert.That(
					await (queries.GetResultAsync<ReadWrite>(2, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name3), "q3");
				Assert.That(
					await (queries.GetResultAsync<ReadWrite>(3, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name4), "q4");
				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(4, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name5), "q5");

				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(2), "cache GetMany secondExecution");
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0), "cache Get secondExecution");
				Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "cache PutMany secondExecution");
				Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "cache Put secondExecution");

				Assert.That(tsCache.GetMultipleCalls, Has.Count.EqualTo(1), "tsCache GetMany secondExecution");
				Assert.That(tsCache.GetCalls, Has.Count.EqualTo(0), "tsCache Get secondExecution");
				Assert.That(tsCache.PutMultipleCalls, Has.Count.EqualTo(0), "tsCache PutMany secondExecution");
				Assert.That(tsCache.PutCalls, Has.Count.EqualTo(0), "tsCache Put secondExecution");

				// Update an entity to invalidate them
				using (var t = s.BeginTransaction())
				{
					var readwrite1 = await (s.Query<ReadWrite>().SingleAsync(e => e.Name == name3));
					readwrite1.Name = "NewName";
					await (t.CommitAsync());
				}

				Assert.That(tsCache.GetMultipleCalls, Has.Count.EqualTo(1), "tsCache GetMany after update");
				Assert.That(tsCache.GetCalls, Has.Count.EqualTo(0), "tsCache Get  after update");
				// Pre-invalidate + invalidate => 2 calls
				Assert.That(tsCache.PutMultipleCalls, Has.Count.EqualTo(2), "tsCache PutMany  after update");
				Assert.That(tsCache.PutCalls, Has.Count.EqualTo(0), "tsCache Put  after update");

				// Run a third time, to re-test the query cache
				using (var t = s.BeginTransaction())
				{
					await (queries.ExecuteAsync(CancellationToken.None));
					await (t.CommitAsync());
				}

				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(0, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name1), "q1 after update");
				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(1, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name2), "q2 after update");
				Assert.That(
					await (queries.GetResultAsync<ReadWrite>(2, CancellationToken.None)),
					Has.Count.EqualTo(0), "q3 after update");
				Assert.That(
					await (queries.GetResultAsync<ReadWrite>(3, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadWrite.Name)).EqualTo(name4), "q4 after update");
				Assert.That(
					await (queries.GetResultAsync<ReadOnly>(4, CancellationToken.None)),
					Has.Count.EqualTo(1).And.One.Property(nameof(ReadOnly.Name)).EqualTo(name5), "q5 after update");

				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(3), "cache GetMany thirdExecution");
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0), "cache Get thirdExecution");
				// ReadWrite queries should have been re-put, so count should have been incremented
				Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(2), "cache PutMany thirdExecution");
				Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "cache Put thirdExecution");

				// Readonly entities should have been still cached, so their queries timestamp should have been
				// rechecked and the get count incremented
				Assert.That(tsCache.GetMultipleCalls, Has.Count.EqualTo(2), "tsCache GetMany thirdExecution");
				Assert.That(tsCache.GetCalls, Has.Count.EqualTo(0), "tsCache Get thirdExecution");
				Assert.That(tsCache.PutMultipleCalls, Has.Count.EqualTo(2), "tsCache PutMany thirdExecution");
				Assert.That(tsCache.PutCalls, Has.Count.EqualTo(0), "tsCache Put thirdExecution");
			}
		}

		private async Task AssertMultipleCacheCallsAsync<TEntity>(List<int> ids, int idIndex, int[][] fetchedIdIndexes, int[] putIdIndexes, Func<int, bool> cacheBeforeLoadFn = null, CancellationToken cancellationToken = default(CancellationToken))
			where TEntity : CacheEntity
		{
			var persister = Sfi.GetEntityPersister(typeof(TEntity).FullName);
			var cache = (BatchableCache) persister.Cache.Cache;
			await (cache.ClearAsync(cancellationToken));

			if (cacheBeforeLoadFn != null)
			{
				using (var s = Sfi.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					foreach (var id in ids.Where((o, i) => cacheBeforeLoadFn(i)))
					{
						await (s.GetAsync<TEntity>(id, cancellationToken));
					}
					await (tx.CommitAsync(cancellationToken));
				}
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				cache.ClearStatistics();

				foreach (var id in ids)
				{
					await (s.LoadAsync<TEntity>(id, cancellationToken));
				}
				var item = await (s.GetAsync<TEntity>(ids[idIndex], cancellationToken));
				Assert.That(item, Is.Not.Null);
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0));
				Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(fetchedIdIndexes.GetLength(0)));
				if (putIdIndexes == null)
				{
					Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(0));
				}
				else
				{
					Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1));
					Assert.That(
						cache.PutMultipleCalls[0].OfType<CacheKey>().Select(o => (int) o.Key),
						Is.EquivalentTo(putIdIndexes.Select(o => ids[o])));
				}

				for (int i = 0; i < fetchedIdIndexes.GetLength(0); i++)
				{
					Assert.That(
						cache.GetMultipleCalls[i].OfType<CacheKey>().Select(o => (int) o.Key),
						Is.EquivalentTo(fetchedIdIndexes[i].Select(o => ids[o])));
				}

				await (tx.CommitAsync(cancellationToken));
			}
		}

		private void AssertEquivalent(List<int> ids, int[][] expectedIdIndexes, List<object[]> result)
		{
			Assert.That(result, Has.Count.EqualTo(expectedIdIndexes.GetLength(0)));
			for (int i = 0; i < expectedIdIndexes.GetLength(0); i++)
			{
				Assert.That(
					result[i].OfType<CacheKey>().Select(o => (int) o.Key),
					Is.EquivalentTo(expectedIdIndexes[i].Select(o => ids[o])));
			}
		}

		private async Task AssertMultipleCacheCollectionCallsAsync(List<int> ids, int idIndex, int[][] fetchedIdIndexes, int[] putIdIndexes, Func<int, bool> cacheBeforeLoadFn = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			var cache = (BatchableCache) persister.Cache.Cache;
			await (cache.ClearAsync(cancellationToken));

			if (cacheBeforeLoadFn != null)
			{
				using (var s = Sfi.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					foreach (var id in ids.Where((o, i) => cacheBeforeLoadFn(i)))
					{
						var item = await (s.GetAsync<ReadOnly>(id, cancellationToken));
						await (NHibernateUtil.InitializeAsync(item.Items, cancellationToken));
					}
					await (tx.CommitAsync(cancellationToken));
				}
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				cache.ClearStatistics();

				foreach (var id in ids)
				{
					await (s.GetAsync<ReadOnly>(id, cancellationToken));
				}
				var item = await (s.GetAsync<ReadOnly>(ids[idIndex], cancellationToken));
				Assert.That(item, Is.Not.Null);
				await (NHibernateUtil.InitializeAsync(item.Items, cancellationToken));
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0));
				Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(fetchedIdIndexes.GetLength(0)));
				if (putIdIndexes == null)
				{
					Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(0));
				}
				else
				{
					Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1));
					Assert.That(
						cache.PutMultipleCalls[0].OfType<CacheKey>().Select(o => (int) o.Key),
						Is.EquivalentTo(putIdIndexes.Select(o => ids[o])));
				}

				for (int i = 0; i < fetchedIdIndexes.GetLength(0); i++)
				{
					Assert.That(
						cache.GetMultipleCalls[i].OfType<CacheKey>().Select(o => (int) o.Key),
						Is.EquivalentTo(fetchedIdIndexes[i].Select(o => ids[o])));
				}

				await (tx.CommitAsync(cancellationToken));
			}
		}

	}
}
