using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

		protected override string[] Mappings => new[] { "BulkManipulation.Vehicle.hbm.xml" };

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
			((SubstituteCacheProvider) Sfi.Settings.CacheProvider).OnClear(x =>
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
		private readonly ConcurrentDictionary<string, Lazy<CacheBase>> _caches = new ConcurrentDictionary<string, Lazy<CacheBase>>();
		private Action<string> _onClear;

		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return _caches.GetOrAdd(regionName, x => new Lazy<CacheBase>(() =>
			 {
				 var cache = Substitute.For<CacheBase>();
				 cache.RegionName.Returns(regionName);
				 cache.When(c => c.Clear()).Do(c => _onClear?.Invoke(regionName));
				 cache.When(c => c.ClearAsync(Arg.Any<CancellationToken>())).Do(c => _onClear?.Invoke(regionName));
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

		public CacheBase GetCache(string region)
		{
			_caches.TryGetValue(region, out var cache);
			return cache?.Value;
		}

		public IEnumerable<CacheBase> GetAllCaches()
		{
			return _caches.Values.Select(x => x.Value);
		}

		public void OnClear(Action<string> callback)
		{
			_onClear = callback;
		}
	}
}
