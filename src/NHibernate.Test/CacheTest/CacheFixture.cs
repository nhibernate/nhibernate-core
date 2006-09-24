using System;
using System.Collections;

using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest {

	[TestFixture]
	public class CacheFixture 
	{
		[Test]
		public void TestSimpleCache() 
		{
			DoTestCache( new HashtableCacheProvider() );
		}
	
		public void DoTestCache(ICacheProvider cacheProvider) 
		{
			ICache cache = cacheProvider.BuildCache( typeof(String).FullName, new Hashtable() );

			long longBefore = Timestamper.Next();

			System.Threading.Thread.Sleep(15);

			long before = Timestamper.Next();

			System.Threading.Thread.Sleep(15);

			ICacheConcurrencyStrategy ccs = new ReadWriteCache();
			ccs.Cache = cache;

			// cache something

			Assert.IsTrue( ccs.Put("foo", "foo", before, null, null, false) );

			System.Threading.Thread.Sleep(15);

			long after = Timestamper.Next();

			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.AreEqual( "foo", ccs.Get("foo", after) );
			Assert.IsFalse( ccs.Put("foo", "foo", before, null, null, false) );

			// update it;

			ISoftLock fooLock = ccs.Lock("foo", null);

			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.IsFalse(ccs.Put("foo", "foo", before, null, null, false));

			System.Threading.Thread.Sleep(15);

			long whileLocked = Timestamper.Next();

			Assert.IsFalse(ccs.Put("foo", "foo", whileLocked, null, null, false));

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", fooLock);

			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.IsFalse(ccs.Put("foo", "bar", whileLocked, null, null, false));
			Assert.IsFalse(ccs.Put("foo", "bar", after, null, null, false));

			System.Threading.Thread.Sleep(15);

			long longAfter = Timestamper.Next();

			Assert.IsTrue(ccs.Put("foo", "baz", longAfter, null, null, false));
			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			long longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get("foo", longLongAfter) );

			// update it again, with multiple locks

			ISoftLock fooLock1 = ccs.Lock("foo", null);
			ISoftLock fooLock2 = ccs.Lock("foo", null);

			Assert.IsNull( ccs.Get("foo", longLongAfter) );

			System.Threading.Thread.Sleep(15);

			whileLocked = Timestamper.Next();

			Assert.IsFalse(ccs.Put("foo", "foo", whileLocked, null, null, false));

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", fooLock2);

			System.Threading.Thread.Sleep(15);

			long betweenReleases = Timestamper.Next();

			Assert.IsFalse(ccs.Put("foo", "bar", betweenReleases, null, null, false));
			Assert.IsNull( ccs.Get("foo", betweenReleases) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", fooLock1);

			Assert.IsFalse(ccs.Put("foo", "bar", whileLocked, null, null, false));

			System.Threading.Thread.Sleep(15);

			longAfter = Timestamper.Next();

			Assert.IsTrue(ccs.Put("foo", "baz", longAfter, null, null, false));
			Assert.IsNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get("foo", longLongAfter) );

		}

		private void DoTestMinValueTimestampOnStrategy( ICache cache, ICacheConcurrencyStrategy strategy )
		{
			strategy.Cache = cache;
			strategy.Put("key", "value", long.MinValue, 0, null, false);

			Assert.IsNull( strategy.Get( "key", long.MinValue ), "{0} strategy fails the test", strategy.GetType() );
			Assert.IsNull( strategy.Get( "key", long.MaxValue ), "{0} strategy fails the test", strategy.GetType() );
		}

		[Test]
		public void MinValueTimestamp()
		{
			ICache cache = new HashtableCacheProvider().BuildCache( "region", new Hashtable() );
			ICacheConcurrencyStrategy strategy = new ReadWriteCache();
			strategy.Cache = cache;

			DoTestMinValueTimestampOnStrategy( cache, new ReadWriteCache() );
			DoTestMinValueTimestampOnStrategy( cache, new NonstrictReadWriteCache() );
			DoTestMinValueTimestampOnStrategy( cache, new ReadOnlyCache() );
		}
	}
}
