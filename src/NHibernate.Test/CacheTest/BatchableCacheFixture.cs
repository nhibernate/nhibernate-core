﻿using System;
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
	[TestFixture]
	public class BatchableCacheFixture : TestCase
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
			configuration.SetProperty(Environment.GenerateStatistics, "true");
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
		public void MultipleGetReadOnlyCollectionTest()
		{
			var persister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
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
				AssertMultipleCacheCollectionCalls(ids, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
			}
		}

		[Test]
		public void GetManyReadWriteTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadWrite).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			int[] getIds;
			int[] loadIds;

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadWrite>().ToList();
				loadIds = getIds = items.OrderBy(o => o.Id).Select(o => o.Id).ToArray();
				tx.Commit();
			}

			// Batch size 3
			var parentTestCases = new List<Tuple<int[], int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMany will be called three times. First time in type
				// DefaultLoadEventListener, the second time in BatchingEntityLoader and third in ReadWriteCache.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2}, // Triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {1, 2, 3}, // Triggered by Load method of BatchingEntityLoader type
						new[] {0, 1, 2} // Triggered by PutMany method of ReadWriteCache type
					},
					new[] {0, 1, 2},
					null
				),
				// When there are not enough uninitialized entities after the demanded one to fill the batch,
				// the nearest before the demanded entity are added.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
						new[] {4, 5, 3},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3},
						new[] {4, 3, 2},
						new[] {5, 4, 3},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					1,
					new[]
					{
						new[] {1, 2, 3}, // 2 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 4, 5},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 2, 1},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2}, // 1 gets assembled inside LoadFromSecondLevelCache
						new[] {2, 3, 4},
						new[] {0, 2, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0},
						new[] {0, 2, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				// Tests by loading different ids
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4}, // Triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {3, 4, 5}, // Triggered by Load method of BatchingEntityLoader type
						new[] {0, 4, 5}, // Triggered by PutMany method of ReadWriteCache type
					},
					new[] {0, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 4).ToArray(),
					4,
					new[]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
						new[] {3, 4, 5}
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 1).ToArray(),
					1,
					new[]
					{
						new[] {1, 5, 4}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {5, 3, 2},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				)
			};

			foreach (var tuple in parentTestCases)
			{
				AssertMultipleCacheCalls<ReadWrite>(tuple.Item1, getIds, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
			}
		}

		[Test]
		public void GetManyReadWriteItemTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadWriteItem).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			int[] getIds;
			int[] loadIds;

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadWriteItem>().Take(6).ToList();
				loadIds = getIds = items.OrderBy(o => o.Id).Select(o => o.Id).ToArray();
				tx.Commit();
			}
			// Batch size 4
			var parentTestCases = new List<Tuple<int[], int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMany will be called three times. First time in type
				// DefaultLoadEventListener, the second time in BatchingEntityLoader and third in ReadWriteCache.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2, 3}, // Triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {1, 2, 3, 4}, // Triggered by Load method of BatchingEntityLoader type
						new[] {0, 1, 2, 3} // Triggered by PutMany method of ReadWriteCache type
					},
					new[] {0, 1, 2, 3},
					null
				),
				// When there are not enough uninitialized entities after the demanded one to fill the batch,
				// the nearest before the demanded entity are added.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3, 2},
						new[] {5, 3, 2, 1},
						new[] {4, 5, 3, 2}
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
						new[] {5, 4, 3, 2}
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					1,
					new[]
					{
						new[] {1, 2, 3, 4}, // 2 and 4 get assembled inside LoadFromSecondLevelCache
						new[] {3, 5, 0},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3, 2}, // 4 and 2 get assembled inside LoadFromSecondLevelCache
						new[] {3, 1, 0},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2, 3}, // 1 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 4, 5},
						new[] {0, 2, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3, 2}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0},
						new[] {0, 2, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				// Tests by loading different ids
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4, 3}, // Triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {5, 4, 3, 2}, // Triggered by Load method of BatchingEntityLoader type
						new[] {0, 5, 4, 3}, // Triggered by PutMany method of ReadWriteCache type
					},
					new[] {0, 5, 4, 3},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 5).ToArray(),
					5,
					new[]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
						new[] {2, 3, 4, 5}
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 1).ToArray(),
					1,
					new[]
					{
						new[] {1, 5, 4, 3}, // 4 get assembled inside LoadFromSecondLevelCache
						new[] {5, 3, 2, 0},
						new[] {1, 3, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
			};

			foreach (var tuple in parentTestCases)
			{
				AssertMultipleCacheCalls<ReadWriteItem>(tuple.Item1, getIds, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
			}
		}

		[Test]
		public void MultipleGetReadOnlyTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnly).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			int[] getIds;
			int[] loadIds;

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnly>().ToList();
				loadIds = getIds = items.OrderBy(o => o.Id).Select(o => o.Id).ToArray();
				tx.Commit();
			}
			// Batch size 3
			var parentTestCases = new List<Tuple<int[], int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
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
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3},
						new[] {4, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					1,
					new[]
					{
						new[] {1, 2, 3}, // 2 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 4, 5}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {3, 2, 1}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2}, // 1 gets assembled inside LoadFromSecondLevelCache
						new[] {2, 3, 4}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				// Tests by loading different ids
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4}, // triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {3, 4, 5}, // triggered by Load method of BatchingEntityLoader type
					},
					new[] {0, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 4).ToArray(),
					4,
					new[]
					{
						new[] {4, 5, 3},
						new[] {5, 3, 2},
					},
					new[] {3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 1).ToArray(),
					1,
					new[]
					{
						new[] {1, 5, 4}, // 4 gets assembled inside LoadFromSecondLevelCache
						new[] {5, 3, 2}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				)
			};

			foreach (var tuple in parentTestCases)
			{
				AssertMultipleCacheCalls<ReadOnly>(tuple.Item1, getIds, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
			}
		}

		[Test]
		public void MultipleGetReadOnlyItemTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			int[] getIds;
			int[] loadIds;

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnlyItem>().Take(6).ToList();
				loadIds = getIds = items.OrderBy(o => o.Id).Select(o => o.Id).ToArray();
				tx.Commit();
			}
			// Batch size 4
			var parentTestCases = new List<Tuple<int[], int, int[][], int[], Func<int, bool>>>
			{
				// When the cache is empty, GetMultiple will be called two times. One time in type
				// DefaultLoadEventListener and the other time in BatchingEntityLoader.
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
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
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3, 2},
						new[] {5, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					1,
					new[]
					{
						new[] {1, 2, 3, 4}, // 2 and 4 get assembled inside LoadFromSecondLevelCache
						new[] {3, 5, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					5,
					new[]
					{
						new[] {5, 4, 3, 2}, // 4 and 2 get assembled inside LoadFromSecondLevelCache
						new[] {3, 1, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					0,
					new[]
					{
						new[] {0, 1, 2, 3}, // 1 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 4, 5}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds,
					4,
					new[]
					{
						new[] {4, 5, 3, 2}, // 5 and 3 get assembled inside LoadFromSecondLevelCache
						new[] {2, 1, 0}
					},
					new[] {0, 2, 4},
					(i) => i % 2 != 0
				),
				// Tests by loading different ids
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4, 3}, // triggered by LoadFromSecondLevelCache method of DefaultLoadEventListener type
						new[] {5, 4, 3, 2}, // triggered by Load method of BatchingEntityLoader type
					},
					new[] {0, 5, 4, 3},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 5).ToArray(),
					5,
					new[]
					{
						new[] {5, 4, 3, 2},
						new[] {4, 3, 2, 1},
					},
					new[] {2, 3, 4, 5},
					null
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 0).ToArray(),
					0,
					new[]
					{
						new[] {0, 5, 4, 3} // 0 get assembled and no further processing is done
					},
					null,
					(i) => i % 2 == 0 // Cache all even indexes before loading
				),
				new Tuple<int[], int, int[][], int[], Func<int, bool>>(
					loadIds.Where((v, i) => i != 1).ToArray(),
					1,
					new[]
					{
						new[] {1, 5, 4, 3}, // 4 get assembled inside LoadFromSecondLevelCache
						new[] {5, 3, 2, 0}
					},
					new[] {1, 3, 5},
					(i) => i % 2 == 0
				),
			};

			foreach (var tuple in parentTestCases)
			{
				AssertMultipleCacheCalls<ReadOnlyItem>(tuple.Item1, getIds, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
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
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "Cache put");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "Cache put many");
			// Lock get
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "Cache get many");

			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4, 5}
				},
				cache.PutMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4, 5}
				},
				cache.LockMultipleCalls
			);
			AssertEquivalent(
				ids,
				new[]
				{
					new[] {0, 1, 2, 3, 4, 5}
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
		public void UpdateTimestampsCacheTest()
		{
			var timestamp = Sfi.UpdateTimestampsCache;
			var field = typeof(UpdateTimestampsCache).GetField(
				"_updateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(field, Is.Not.Null, "Unable to find _updateTimestamps field");
			var cache = (BatchableCache) field.GetValue(timestamp);
			Assert.That(cache, Is.Not.Null, "Cache field");

			cache.Clear();
			cache.ClearStatistics();

			const string query = "from ReadOnly e where e.Name = :name";
			const string name = "Name1";
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s
					.CreateQuery(query)
					.SetString("name", name)
					.SetCacheable(true)
					.UniqueResult();
				t.Commit();
			}

			// Run a second time, to test the query cache
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var result =
					s
						.CreateQuery(query)
						.SetString("name", name)
						.SetCacheable(true)
						.UniqueResult();

				Assert.That(result, Is.Not.Null);
				t.Commit();
			}

			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "GetMany");
			Assert.That(cache.GetCalls, Has.Count.EqualTo(0), "Get");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(0), "PutMany");
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "Put");

			// Update entities to put some update ts
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var readwrite1 = s.Query<ReadWrite>().First();
				readwrite1.Name = "NewName";
				t.Commit();
			}
			// PreInvalidate + Invalidate => 2 calls
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(2), "PutMany after update");
			Assert.That(cache.PutCalls, Has.Count.EqualTo(0), "Put after update");
		}

		[Test]
		public void QueryCacheTest()
		{
			// QueryCache batching is used by QueryBatch.
			if (!Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");

			var cache = GetDefaultQueryCache();
			var timestamp = Sfi.UpdateTimestampsCache;
			var tsField = typeof(UpdateTimestampsCache).GetField(
				"_updateTimestamps",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(tsField, Is.Not.Null, "Unable to find _updateTimestamps field");
			var tsCache = (BatchableCache) tsField.GetValue(timestamp);
			Assert.That(tsCache, Is.Not.Null, "_updateTimestamps is null");

			cache.Clear();
			cache.ClearStatistics();
			tsCache.Clear();
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
					queries.Execute();
					t.Commit();
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
					var readwrite1 = s.Query<ReadWrite>().Single(e => e.Name == name3);
					readwrite1.Name = "NewName";
					t.Commit();
				}

				Assert.That(tsCache.GetMultipleCalls, Has.Count.EqualTo(1), "tsCache GetMany after update");
				Assert.That(tsCache.GetCalls, Has.Count.EqualTo(0), "tsCache Get  after update");
				// Pre-invalidate + invalidate => 2 calls
				Assert.That(tsCache.PutMultipleCalls, Has.Count.EqualTo(2), "tsCache PutMany  after update");
				Assert.That(tsCache.PutCalls, Has.Count.EqualTo(0), "tsCache Put  after update");

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

		[TestCase(true)]
		[TestCase(false)]
		public void QueryEntityBatchCacheTest(bool clearEntityCacheAfterQuery)
		{
			var persister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			var cache = (BatchableCache) persister.Cache.Cache;
			var queryCache = GetDefaultQueryCache();

			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			cache.ClearStatistics();
			queryCache.ClearStatistics();

			List<ReadOnlyItem> items;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				items = s.Query<ReadOnlyItem>()
				         .WithOptions(o => o.SetCacheable(true))
				         .ToList();

				tx.Commit();
			}

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(1), "Unexpected query cache PutCalls");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache PutMultipleCalls");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(0), "Unexpected entity cache GetMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(36), "Unexpected items count");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(1), "Unexpected cache miss count");

			cache.ClearStatistics();
			queryCache.ClearStatistics();

			if (clearEntityCacheAfterQuery)
			{
				cache.Clear();
			}

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				items = s.Query<ReadOnlyItem>()
				         .WithOptions(o => o.SetCacheable(true))
				         .ToList();

				tx.Commit();
			}

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(0), "Unexpected query cache PutCalls");
			// Ideally the PutMultipleCalls count should be 1 when clearing the cache after the first query, in order to achieve this
			// the CacheBatcher would need to be on the session and executed once the query is processed
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 9 : 0), "Unexpected entity cache PutMultipleCalls");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache GetMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(36));
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}

		[TestCase(true, false)]
		[TestCase(false, false)]
		[TestCase(true, true)]
		[TestCase(false, true)]
		public void QueryFetchCollectionBatchCacheTest(bool clearEntityCacheAfterQuery, bool future)
		{
			if (future && !Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");
			}

			var persister = Sfi.GetEntityPersister(typeof(ReadOnly).FullName);
			var itemPersister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			var collectionPersister = Sfi.GetCollectionPersister($"{typeof(ReadOnly).FullName}.Items");
			var cache = (BatchableCache) persister.Cache.Cache;
			var itemCache = (BatchableCache) itemPersister.Cache.Cache;
			var collectionCache = (BatchableCache) collectionPersister.Cache.Cache;
			var queryCache = GetDefaultQueryCache();

			int middleId;

			using (var s = OpenSession())
			{
				var ids = s.Query<ReadOnly>().Select(o => o.Id).OrderBy(o => o).ToList();
				middleId = ids[2];
			}

			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			queryCache.ClearStatistics();
			cache.ClearStatistics();
			cache.Clear();
			itemCache.ClearStatistics();
			itemCache.Clear();
			collectionCache.ClearStatistics();
			collectionCache.Clear();

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

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.GetMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache GetMultipleCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache PutCalls");
			Assert.That(queryCache.PutMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache PutMultipleCalls");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache PutMultipleCalls");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(0), "Unexpected entity cache GetMultipleCalls");
			Assert.That(collectionCache.PutMultipleCalls, Has.Count.EqualTo(1), "Unexpected collection cache PutMultipleCalls");
			Assert.That(collectionCache.GetMultipleCalls, Has.Count.EqualTo(0), "Unexpected collection cache GetMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(future ? 3 : 6), "Unexpected items count");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache miss count");

			cache.ClearStatistics();
			itemCache.ClearStatistics();
			collectionCache.ClearStatistics();
			queryCache.ClearStatistics();

			if (clearEntityCacheAfterQuery)
			{
				cache.Clear();
				collectionCache.Clear();
				itemCache.Clear();
			}

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

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.GetMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache GetCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(0), "Unexpected query cache PutCalls");
			Assert.That(queryCache.PutMultipleCalls, Has.Count.EqualTo(0), "Unexpected query cache PutMultipleCalls");
			Assert.That(collectionCache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected collection cache GetMultipleCalls");
			Assert.That(collectionCache.GetMultipleCalls[0], Has.Length.EqualTo(6), "Unexpected collection cache GetMultipleCalls length");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache GetMultipleCalls");
			Assert.That(cache.GetMultipleCalls[0], Has.Length.EqualTo(6), "Unexpected entity cache GetMultipleCalls length");
			Assert.That(itemCache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity item cache GetMultipleCalls");
			Assert.That(itemCache.GetMultipleCalls[0], Has.Length.EqualTo(36), "Unexpected entity item cache GetMultipleCalls length");
			// Ideally the PutMultipleCalls count should be 1 when clearing the cache after the first query, in order to achieve this
			// the CacheBatcher would need to be on the session and executed once the batch fetch queries are processed
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 2 : 0), "Unexpected entity cache PutMultipleCalls");
			Assert.That(collectionCache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 2 : 0), "Unexpected collection cache PutMultipleCalls");
			Assert.That(itemCache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 9 : 0), "Unexpected entity item cache PutMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(future ? 3 : 6));
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache hit count");
		}

		[TestCase(true, false)]
		[TestCase(false, false)]
		[TestCase(true, true)]
		[TestCase(false, true)]
		public void QueryFetchEntityBatchCacheTest(bool clearEntityCacheAfterQuery, bool future)
		{
			if (future && !Sfi.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				Assert.Ignore($"{Sfi.ConnectionProvider.Driver} does not support multiple queries");
			}

			var persister = Sfi.GetEntityPersister(typeof(ReadOnlyItem).FullName);
			var parentPersister = Sfi.GetEntityPersister(typeof(ReadOnly).FullName);
			var cache = (BatchableCache) persister.Cache.Cache;
			var parentCache = (BatchableCache) parentPersister.Cache.Cache;
			var queryCache = GetDefaultQueryCache();

			int middleId;

			using (var s = OpenSession())
			{
				var ids = s.Query<ReadOnlyItem>().Select(o => o.Id).OrderBy(o => o).ToList();
				middleId = ids[17];
			}

			Sfi.Statistics.Clear();
			Sfi.EvictQueries();
			queryCache.ClearStatistics();
			cache.ClearStatistics();
			cache.Clear();
			parentCache.ClearStatistics();
			parentCache.Clear();

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

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.GetMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache GetMultipleCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache PutCalls");
			Assert.That(queryCache.PutMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache PutMultipleCalls");
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache PutMultipleCalls");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(0), "Unexpected entity cache GetMultipleCalls");
			Assert.That(parentCache.PutMultipleCalls, Has.Count.EqualTo(1), "Unexpected parent cache PutMultipleCalls");
			Assert.That(parentCache.GetMultipleCalls, Has.Count.EqualTo(0), "Unexpected parent cache GetMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(future ? 18 : 36), "Unexpected items count");
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache miss count");

			cache.ClearStatistics();
			parentCache.ClearStatistics();
			queryCache.ClearStatistics();

			if (clearEntityCacheAfterQuery)
			{
				cache.Clear();
				parentCache.Clear();
			}

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

			Assert.That(queryCache.GetCalls, Has.Count.EqualTo(future ? 0 : 1), "Unexpected query cache GetCalls");
			Assert.That(queryCache.GetMultipleCalls, Has.Count.EqualTo(future ? 1 : 0), "Unexpected query cache GetCalls");
			Assert.That(queryCache.PutCalls, Has.Count.EqualTo(0), "Unexpected query cache PutCalls");
			Assert.That(queryCache.PutMultipleCalls, Has.Count.EqualTo(0), "Unexpected query cache PutMultipleCalls");
			Assert.That(parentCache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected parent cache GetMultipleCalls");
			Assert.That(parentCache.GetMultipleCalls[0], Has.Length.EqualTo(6), "Unexpected parent cache GetMultipleCalls length");
			Assert.That(cache.GetMultipleCalls, Has.Count.EqualTo(1), "Unexpected entity cache GetMultipleCalls");
			Assert.That(cache.GetMultipleCalls[0], Has.Length.EqualTo(36), "Unexpected entity cache GetMultipleCalls length");
			// Ideally the PutMultipleCalls count should be 1 when clearing the cache after the first query, in order to achieve this
			// the CacheBatcher would need to be on the session and executed once the batch fetch queries are processed
			Assert.That(cache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 9 : 0), "Unexpected entity cache PutMultipleCalls");
			Assert.That(parentCache.PutMultipleCalls, Has.Count.EqualTo(clearEntityCacheAfterQuery ? 2 : 0), "Unexpected parent cache PutMultipleCalls");
			Assert.That(items, Has.Count.EqualTo(future ? 18 : 36));
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Unexpected cache miss count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(future ? 2 : 1), "Unexpected cache hit count");
		}

		private void AssertMultipleCacheCalls<TEntity>(IEnumerable<int> loadIds,  IReadOnlyList<int> getIds, int idIndex, 
														int[][] fetchedIdIndexes, int[] putIdIndexes, Func<int, bool> cacheBeforeLoadFn = null)
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
					foreach (var id in getIds.Where((o, i) => cacheBeforeLoadFn(i)))
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
				foreach (var id in loadIds)
				{
					s.Load<TEntity>(id);
				}
				var item = s.Get<TEntity>(getIds[idIndex]);
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
						Is.EquivalentTo(putIdIndexes.Select(o => getIds[o])));
				}

				for (int i = 0; i < fetchedIdIndexes.GetLength(0); i++)
				{
					Assert.That(
						cache.GetMultipleCalls[i].OfType<CacheKey>().Select(o => (int) o.Key),
						Is.EquivalentTo(fetchedIdIndexes[i].Select(o => getIds[o])));
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

		private BatchableCache GetDefaultQueryCache()
		{
			var queryCache = Sfi.GetQueryCache(null);
			var field = typeof(StandardQueryCache).GetField(
				"_cache",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.That(field, Is.Not.Null, "Unable to find _cache field");
			var cache = (BatchableCache) field.GetValue(queryCache);
			Assert.That(cache, Is.Not.Null, "_cache is null");

			return cache;
		}
	}
}
