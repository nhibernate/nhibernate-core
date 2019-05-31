using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class BuildCacheFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "CacheTest.EntitiesInSameRegion.hbm.xml" };

		// Disable the TestCase cache overrides.
		protected override string CacheConcurrencyStrategy => null;

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.CacheProvider, typeof(LockedCacheProvider).AssemblyQualifiedName);
		}

		[Theory]
		public void CommonRegionHasOneUniqueCacheAndExpectedConcurrency(bool withPrefix)
		{
			const string prefix = "Prefix";
			const string region = "Common";
			var fullRegion = (withPrefix ? prefix + "." : "") + region;
			ISessionFactoryImplementor sfi = null;
			if (withPrefix)
				cfg.SetProperty(Environment.CacheRegionPrefix, prefix);
			try
			{
				sfi = withPrefix ? BuildSessionFactory() : Sfi;
				var commonRegionCache = sfi.GetSecondLevelCacheRegion(fullRegion);
				var entityAName = typeof(EntityA).FullName;
				var entityAConcurrencyCache = sfi.GetEntityPersister(entityAName).GetCache(null);
				var entityACache = entityAConcurrencyCache.Cache;
				var entityBName = typeof(EntityB).FullName;
				var entityBConcurrencyCache = sfi.GetEntityPersister(entityBName).GetCache(null);
				var entityBCache = entityBConcurrencyCache.Cache;
				var relatedAConcurrencyCache =
					sfi.GetCollectionPersister(StringHelper.Qualify(entityAName, nameof(EntityA.Related))).GetCache(null);
				var relatedACache = relatedAConcurrencyCache.Cache;
				var relatedBConcurrencyCache =
					sfi.GetCollectionPersister(StringHelper.Qualify(entityBName, nameof(EntityB.Related))).GetCache(null);
				var relatedBCache = relatedBConcurrencyCache.Cache;
				var queryCache = sfi.GetQueryCache(region).Cache;
				Assert.Multiple(
					() =>
					{
						Assert.That(commonRegionCache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for common region");
						Assert.That(entityACache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for EntityA");
						Assert.That(entityBCache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for EntityB");
						Assert.That(relatedACache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for RelatedA");
						Assert.That(relatedBCache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for RelatedB");
						Assert.That(queryCache.RegionName, Is.EqualTo(fullRegion), "Unexpected region name for query cache");
					});
				Assert.Multiple(
					() =>
					{
						Assert.That(entityAConcurrencyCache, Is.InstanceOf<ReadWriteCache>(), "Unexpected concurrency for EntityA");
						Assert.That(relatedAConcurrencyCache, Is.InstanceOf<NonstrictReadWriteCache>(), "Unexpected concurrency for RelatedA");
						Assert.That(entityBConcurrencyCache, Is.InstanceOf<ReadOnlyCache>(), "Unexpected concurrency for EntityB");
						Assert.That(relatedBConcurrencyCache, Is.InstanceOf<ReadWriteCache>(), "Unexpected concurrency for RelatedB");
						Assert.That(entityACache, Is.SameAs(commonRegionCache), "Unexpected cache for EntityA");
						Assert.That(entityBCache, Is.SameAs(commonRegionCache), "Unexpected cache for EntityB");
						Assert.That(relatedACache, Is.SameAs(commonRegionCache), "Unexpected cache for RelatedA");
						Assert.That(relatedBCache, Is.SameAs(commonRegionCache), "Unexpected cache for RelatedB");
						Assert.That(queryCache, Is.SameAs(commonRegionCache), "Unexpected cache for query cache");
					});
			}
			finally
			{
				if (withPrefix)
				{
					cfg.Properties.Remove(Environment.CacheRegionPrefix);
					sfi?.Dispose();
				}
			}
		}

		[Test]
		public void RetrievedQueryCacheMatchesGloballyStoredOne()
		{
			var region = "RetrievedQueryCacheMatchesGloballyStoredOne";
			LockedCache.Semaphore = new SemaphoreSlim(0);
			LockedCache.CreationCount = 0;
			try
			{
				var failures = new ConcurrentBag<Exception>();
				var thread1 = new Thread(
					() =>
					{
						try
						{
							Sfi.GetQueryCache(region);
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					});
				var thread2 = new Thread(
					() =>
					{
						try
						{
							Sfi.GetQueryCache(region);
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					});
				thread1.Start();
				thread2.Start();

				// Give some time to threads for reaching the wait, having all of them ready to do most of their job concurrently.
				Thread.Sleep(100);
				// Let only one finish its job, it should be the one being stored in query cache dictionary.
				LockedCache.Semaphore.Release(1);
				// Give some time to released thread to finish its job.
				Thread.Sleep(100);
				// Release other thread
				LockedCache.Semaphore.Release(10);

				thread1.Join();
				thread2.Join();
				Assert.That(failures, Is.Empty, $"{failures.Count} thread(s) failed.");
			}
			finally
			{
				LockedCache.Semaphore.Dispose();
				LockedCache.Semaphore = null;
			}

			var queryCache = Sfi.GetQueryCache(region).Cache;
			var globalCache = Sfi.GetSecondLevelCacheRegion(region);
			Assert.That(globalCache, Is.SameAs(queryCache));
			Assert.That(LockedCache.CreationCount, Is.EqualTo(1));
		}
	}

	public class LockedCache : HashtableCache
	{
		public static SemaphoreSlim Semaphore { get; set; }

		private static int _creationCount;

		public static int CreationCount
		{
			get => _creationCount;
			set => _creationCount = value;
		}

		public LockedCache(string regionName) : base(regionName)
		{
			Semaphore?.Wait();
			Interlocked.Increment(ref _creationCount);
		}
	}

	public class LockedCacheProvider : ICacheProvider
	{
		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new LockedCache(regionName);
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start(IDictionary<string, string> properties)
		{
		}

		public void Stop()
		{
		}
	}
}
