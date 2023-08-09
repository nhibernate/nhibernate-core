using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NHibernate.Cfg;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class SyncOnlyCacheFixture : CacheFixture
	{
		protected override void Configure(Configuration cfg)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.CacheReadWriteLockFactory, "sync");
		}

		[Test]
		public void AsyncOperationsThrow()
		{
			var cache = new HashtableCacheProvider().BuildCache("region", new Dictionary<string, string>());
			var strategy = CreateCache(cache);
			CacheKey key = CreateCacheKey("key");
			var stamp = Timestamper.Next();
			Assert.ThrowsAsync<InvalidOperationException>(
				() =>
					strategy.PutAsync(key, "value", stamp, 0, null, false, default(CancellationToken)));
			Assert.ThrowsAsync<InvalidOperationException>(() => strategy.GetAsync(key, stamp, default(CancellationToken)));
		}
	}
}
