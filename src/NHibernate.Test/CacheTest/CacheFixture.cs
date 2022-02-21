using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class CacheFixture: TestCase
	{
		[Test]
		public void TestSimpleCache()
		{
			DoTestCache(new HashtableCacheProvider());
		}

		protected CacheKey CreateCacheKey(string text)
		{
			return new CacheKey(text, NHibernateUtil.String, "Foo", null, null);
		}

		public void DoTestCache(ICacheProvider cacheProvider)
		{
			var cache = (CacheBase) cacheProvider.BuildCache(typeof(String).FullName, new Dictionary<string, string>());

			long longBefore = Timestamper.Next();

			Thread.Sleep(15);

			long before = Timestamper.Next();

			Thread.Sleep(15);

			ICacheConcurrencyStrategy ccs = CreateCache(cache);

			// cache something
			CacheKey fooKey = CreateCacheKey("foo");

			Assert.IsTrue(ccs.Put(fooKey, "foo", before, null, null, false));

			Thread.Sleep(15);

			long after = Timestamper.Next();

			Assert.IsNull(ccs.Get(fooKey, longBefore));
			Assert.AreEqual("foo", ccs.Get(fooKey, after));
			Assert.IsFalse(ccs.Put(fooKey, "foo", before, null, null, false));

			// update it;

			ISoftLock fooLock = ccs.Lock(fooKey, null);

			Assert.IsNull(ccs.Get(fooKey, after));
			Assert.IsNull(ccs.Get(fooKey, longBefore));
			Assert.IsFalse(ccs.Put(fooKey, "foo", before, null, null, false));

			Thread.Sleep(15);

			long whileLocked = Timestamper.Next();

			Assert.IsFalse(ccs.Put(fooKey, "foo", whileLocked, null, null, false));

			Thread.Sleep(15);

			ccs.Release(fooKey, fooLock);

			Assert.IsNull(ccs.Get(fooKey, after));
			Assert.IsNull(ccs.Get(fooKey, longBefore));
			Assert.IsFalse(ccs.Put(fooKey, "bar", whileLocked, null, null, false));
			Assert.IsFalse(ccs.Put(fooKey, "bar", after, null, null, false));

			Thread.Sleep(15);

			long longAfter = Timestamper.Next();

			Assert.IsTrue(ccs.Put(fooKey, "baz", longAfter, null, null, false));
			Assert.IsNull(ccs.Get(fooKey, after));
			Assert.IsNull(ccs.Get(fooKey, whileLocked));

			Thread.Sleep(15);

			long longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get(fooKey, longLongAfter));

			// update it again, with multiple locks

			ISoftLock fooLock1 = ccs.Lock(fooKey, null);
			ISoftLock fooLock2 = ccs.Lock(fooKey, null);

			Assert.IsNull(ccs.Get(fooKey, longLongAfter));

			Thread.Sleep(15);

			whileLocked = Timestamper.Next();

			Assert.IsFalse(ccs.Put(fooKey, "foo", whileLocked, null, null, false));

			Thread.Sleep(15);

			ccs.Release(fooKey, fooLock2);

			Thread.Sleep(15);

			long betweenReleases = Timestamper.Next();

			Assert.IsFalse(ccs.Put(fooKey, "bar", betweenReleases, null, null, false));
			Assert.IsNull(ccs.Get(fooKey, betweenReleases));

			Thread.Sleep(15);

			ccs.Release(fooKey, fooLock1);

			Assert.IsFalse(ccs.Put(fooKey, "bar", whileLocked, null, null, false));

			Thread.Sleep(15);

			longAfter = Timestamper.Next();

			Assert.IsTrue(ccs.Put(fooKey, "baz", longAfter, null, null, false));
			Assert.IsNull(ccs.Get(fooKey, whileLocked));

			Thread.Sleep(15);

			longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get(fooKey, longLongAfter));
		}

		private void DoTestMinValueTimestampOnStrategy(CacheBase cache, ICacheConcurrencyStrategy strategy)
		{
			CacheKey key = CreateCacheKey("key");
			strategy.Cache = cache;
			strategy.Put(key, "value", long.MinValue, 0, null, false);

			Assert.IsNull(strategy.Get(key, long.MinValue), "{0} strategy fails the test", strategy.GetType());
			Assert.IsNull(strategy.Get(key, long.MaxValue), "{0} strategy fails the test", strategy.GetType());
		}

		[Test]
		public void MinValueTimestamp()
		{
			var cache = new HashtableCacheProvider().BuildCache("region", new Dictionary<string, string>());

			DoTestMinValueTimestampOnStrategy(cache, CreateCache(cache));
			DoTestMinValueTimestampOnStrategy(cache, CreateCache(cache, CacheFactory.NonstrictReadWrite));
			DoTestMinValueTimestampOnStrategy(cache, CreateCache(cache, CacheFactory.ReadOnly));
		}

		protected virtual ICacheConcurrencyStrategy CreateCache(CacheBase cache, string strategy = CacheFactory.ReadWrite)
		{
			return CacheFactory.CreateCache(strategy, cache, Sfi.Settings);
		}

		protected override string[] Mappings => Array.Empty<string>();
	}
}
