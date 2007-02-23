using System;
using System.Collections;
using System.Threading;
using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class CacheFixture
	{
		[Test]
		public void TestSimpleCache()
		{
			DoTestCache(new HashtableCacheProvider());
		}

		private CacheKey CreateCacheKey(string text)
		{
			return new CacheKey(text, NHibernateUtil.String, "Foo", null);
		}

		public void DoTestCache(ICacheProvider cacheProvider)
		{
			ICache cache = cacheProvider.BuildCache(typeof(String).FullName, new Hashtable());

			long longBefore = Timestamper.Next();

			Thread.Sleep(15);

			long before = Timestamper.Next();

			Thread.Sleep(15);

			ICacheConcurrencyStrategy ccs = new ReadWriteCache();
			ccs.Cache = cache;

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

		private void DoTestMinValueTimestampOnStrategy(ICache cache, ICacheConcurrencyStrategy strategy)
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
			ICache cache = new HashtableCacheProvider().BuildCache("region", new Hashtable());
			ICacheConcurrencyStrategy strategy = new ReadWriteCache();
			strategy.Cache = cache;

			DoTestMinValueTimestampOnStrategy(cache, new ReadWriteCache());
			DoTestMinValueTimestampOnStrategy(cache, new NonstrictReadWriteCache());
			DoTestMinValueTimestampOnStrategy(cache, new ReadOnlyCache());
		}
	}
}