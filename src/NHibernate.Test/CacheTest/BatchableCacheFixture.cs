using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Test.CacheTest.Caches;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class BatchableCacheFixture : TestCase
	{
		protected override IList Mappings => new[]
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

		protected override bool CheckDatabaseWasCleaned()
		{
			base.CheckDatabaseWasCleaned();
			return true; // We are unable to delete read-only items.
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
				s.Delete("from ReadWrite");
				tx.Commit();
			}
		}

		[Test]
		public void MultipleGetReadOnlyCollectionTest()
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();
			
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnly>().ToList();
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				tx.Commit();
			}

			// Batch size 5
			var testCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultInitializeCollectionEventListener and the other time in BatchingCollectionInitializer.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
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
					new int[][]
					{
						new[] {4, 5, 3, 2, 1},
						new[] {5, 3, 2, 1, 0},
					},
					new[] {1, 2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3, 2, 1},
						new[] {4, 3, 2, 1, 0},
					},
					new[] {1, 2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2, 3, 4} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new int[][]
					{
						new[] {1, 2, 3, 4, 5}, // 2 and 4 get assembled inside InitializeCollectionFromCache
						new[] {3, 5, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3, 2, 1}, // 4 and 2 get assembled inside InitializeCollectionFromCache
						new[] {3, 1, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2, 3, 4}, // 1 and 3 get assembled inside InitializeCollectionFromCache
						new[] {2, 4, 5}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new int[][]
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
				AssertMultipleCacheCollectionCalls(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
			}
		}

		[Test]
		public void MultipleGetReadOnlyTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnly).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnly>().ToList();
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				tx.Commit();
			}
			// Batch size 3
			var parentTestCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
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
					new int[][]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3},
						new[] {4, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new int[][]
					{
						new[] {1, 2, 3}, // 2 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 4, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 2, 1}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2}, // 1 gets assembled inside LoadFromSecondLevelCache
						new[] {2, 3, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new int[][]
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
				AssertMultipleCacheCalls<ReadOnly>(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
			}
		}

		[Test]
		public void MultipleGetReadOnlyItemTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnlyItem>().Take(6).ToList();
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				tx.Commit();
			}
			// Batch size 4
			var parentTestCases = new List<Tuple<int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
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
					new int[][]
					{
						new[] {4, 5, 3, 2},
						new[] {5, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					1,
					new int[][]
					{
						new[] {1, 2, 3, 4}, // 2 and 4 get assembled inside LoadFromSecondLevelCache
						new[] {3, 5, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					5,
					new int[][]
					{
						new[] {5, 4, 3, 2}, // 4 and 2 get assembled inside LoadFromSecondLevelCache
						new[] {3, 1, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					0,
					new int[][]
					{
						new[] {0, 1, 2, 3}, // 1 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 4, 5}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int, int[][], int[], Func<int, bool>>(
					4,
					new int[][]
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
				AssertMultipleCacheCalls<ReadOnlyItem>(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
			}
		}

		[Test]
		public void MultiplePutReadWriteTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadWrite).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			cache.Clear();
			cache.ClearStatistics();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadWrite>().ToList();
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));
				tx.Commit();
			}
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(2));

			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.PutMultipleCalls
			);
			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.LockMultipleCalls
			);
			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2},
					new[] {3, 4, 5}
				},
				cache.UnlockMultipleCalls
			);
		}

		[Test]
		public void MultiplePutReadWriteItemTest()
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadWrite).FullName}.Items");
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var cache = (BatchableCache) persister.Cache.Cache;
			var ids = new List<int>();

			cache.Clear();
			cache.ClearStatistics();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadWrite>().ToList();
				ids.AddRange(items.OrderBy(o => o.Id).Select(o => o.Id));

				// Initialize the first item collection
				NHibernateUtil.Initialize(items.First(o => o.Id == ids[0]).Items);
				tx.Commit();
			}
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0));
			// Called in: DefaultInitializeCollectionEventListener, BatchingCollectionInitializer and ReadWriteCache
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(3));

			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.PutMultipleCalls
			);
			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.LockMultipleCalls
			);
			AssertEquivalent(
				ids,
				new int[][]
				{
					new[] {0, 1, 2, 3, 4}
				},
				cache.UnlockMultipleCalls
			);
		}

		[Test]
		public void UpdateTimestampsCacheTest()
		{
			var timestamp = Sfi.UpdateTimestampsCache;
			var field = typeof(UpdateTimestampsCache).GetField(
				"_batchUpdateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(field, Is.Not.Null);
			var cache = (BatchableCache) field.GetValue(timestamp);
			Assert.That(cache, Is.Not.Null);

			using (var s = OpenSession())
			{
				const string query = "from ReadOnly e where e.Name = :name";
				const string name = "Name1";
				s
					.CreateQuery(query)
					.SetString("name", name)
					.SetCacheable(true)
					.UniqueResult();

				// Run a second time, just to test the query cache
				var result = s
				             .CreateQuery(query)
				             .SetString("name", name)
				             .SetCacheable(true)
				             .UniqueResult();

				Assert.That(result, Is.Not.Null);
				Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1));
				Assert.That(cache.GetCalls, Has.Count.EqualTo(0));
			}
		}

		private void AssertMultipleCacheCalls<TEntity>(List<int> ids, int idIndex, int[][] fetchedIdIndexes, int[] putIdIndexes, Func<int, bool> cacheBeforeLoadFn = null)
			where TEntity : CacheEntity
		{
			var persister = Sfi.GetEntityPersister(typeof(TEntity).FullName);
			var cache = (BatchableCache) persister.Cache.Cache;
			cache.Clear();

			if (cacheBeforeLoadFn != null)
			{
				using (var s = Sfi.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					foreach (var id in ids.Where((o, i) => cacheBeforeLoadFn(i)))
					{
						s.Get<TEntity>(id);
					}
					tx.Commit();
				}
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				cache.ClearStatistics();

				foreach (var id in ids)
				{
					s.Load<TEntity>(id);
				}
				var item = s.Get<TEntity>(ids[idIndex]);
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

				tx.Commit();
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

		private void AssertMultipleCacheCollectionCalls(List<int> ids, int idIndex, int[][] fetchedIdIndexes, int[] putIdIndexes, Func<int, bool> cacheBeforeLoadFn = null)
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			var cache = (BatchableCache) persister.Cache.Cache;
			cache.Clear();

			if (cacheBeforeLoadFn != null)
			{
				using (var s = Sfi.OpenSession())
				using (var tx = s.BeginTransaction())
				{
					foreach (var id in ids.Where((o, i) => cacheBeforeLoadFn(i)))
					{
						var item = s.Get<ReadOnly>(id);
						NHibernateUtil.Initialize(item.Items);
					}
					tx.Commit();
				}
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				cache.ClearStatistics();

				foreach (var id in ids)
				{
					s.Get<ReadOnly>(id);
				}
				var item = s.Get<ReadOnly>(ids[idIndex]);
				Assert.That(item, Is.Not.Null);
				NHibernateUtil.Initialize(item.Items);
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

				tx.Commit();
			}
		}

	}
}
