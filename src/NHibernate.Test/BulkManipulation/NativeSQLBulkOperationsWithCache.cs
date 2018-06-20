using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NSubstitute;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.BulkManipulation
{
	[TestFixture]
	public class NativeSQLBulkOperationsWithCache : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] { "BulkManipulation.Vehicle.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.UseQueryCache, "true");
			cfg.SetProperty(Environment.UseSecondLevelCache, "true");
			cfg.SetProperty(Environment.CacheProvider, typeof(SubstituteCacheProvider).AssemblyQualifiedName);
		}

		[Test]
		public void SimpleNativeSQLInsert_DoesNotEvictEntireCacheWhenQuerySpacesAreAdded()
		{
			List<string> clearCalls = new List<string>();
			(Sfi.Settings.CacheProvider as SubstituteCacheProvider).OnClear(x =>
			{
				clearCalls.Add(x);
			});
			using (var s = OpenSession())
			{
				string ssql = "UPDATE Vehicle SET Vin='123' WHERE Vin='123c'";

				using (var t = s.BeginTransaction())
				{

					s.CreateSQLQuery(ssql).ExecuteUpdate();
					t.Commit();

					Assert.AreEqual(1, clearCalls.Count);
				}

				clearCalls.Clear();

				using (var t = s.BeginTransaction())
				{
					s.CreateSQLQuery(ssql).AddSynchronizedQuerySpace("Unknown").ExecuteUpdate();
					t.Commit();

					Assert.AreEqual(0, clearCalls.Count);
				}
			}
		}
	}

	public class SubstituteCacheProvider : ICacheProvider
	{
		private readonly ConcurrentDictionary<string, Lazy<ICache>> _caches = new ConcurrentDictionary<string, Lazy<ICache>>();
		private Action<string> _onClear;

		public ICache BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return _caches.GetOrAdd(regionName, x => new Lazy<ICache>(() =>
			 {
				 var cache = Substitute.For<ICache>();
				 cache.RegionName.Returns(regionName);
				 cache.When(c => c.Clear()).Do(c => _onClear?.Invoke(regionName));
				 return cache;
			 })).Value;
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

		public ICache GetCache(string region)
		{
			Lazy<ICache> cache;
			_caches.TryGetValue(region, out cache);
			return cache?.Value;
		}

		public IEnumerable<ICache> GetAllCaches()
		{
			return _caches.Values.Select(x => x.Value);
		}

		public void OnClear(Action<string> callback)
		{
			_onClear = callback;
		}
	}
}
