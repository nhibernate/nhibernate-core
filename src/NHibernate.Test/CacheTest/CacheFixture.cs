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

			Assert.IsTrue( ccs.Put("foo", "foo", before) );

			System.Threading.Thread.Sleep(15);

			long after = Timestamper.Next();

			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.AreEqual( "foo", ccs.Get("foo", after) );
			Assert.IsFalse( ccs.Put("foo", "foo", before) );

			// update it;

			ccs.Lock("foo");

			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.IsFalse( ccs.Put("foo", "foo", before) );

			System.Threading.Thread.Sleep(15);

			long whileLocked = Timestamper.Next();

			Assert.IsFalse( ccs.Put("foo", "foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", null);

			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", longBefore) );
			Assert.IsFalse( ccs.Put("foo", "bar", whileLocked) );
			Assert.IsFalse( ccs.Put("foo", "bar", after) );

			System.Threading.Thread.Sleep(15);

			long longAfter = Timestamper.Next();

			Assert.IsTrue( ccs.Put("foo", "baz", longAfter) );
			Assert.IsNull( ccs.Get("foo", after) );
			Assert.IsNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			long longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get("foo", longLongAfter) );

			// update it again, with multiple locks

			ccs.Lock("foo");
			ccs.Lock("foo");

			Assert.IsNull( ccs.Get("foo", longLongAfter) );

			System.Threading.Thread.Sleep(15);

			whileLocked = Timestamper.Next();

			Assert.IsFalse( ccs.Put("foo", "foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", null);

			System.Threading.Thread.Sleep(15);

			long betweenReleases = Timestamper.Next();

			Assert.IsFalse( ccs.Put("foo", "bar", betweenReleases) );
			Assert.IsNull( ccs.Get("foo", betweenReleases) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo", null);

			Assert.IsFalse( ccs.Put("foo", "bar", whileLocked) );

			System.Threading.Thread.Sleep(15);

			longAfter = Timestamper.Next();

			Assert.IsTrue( ccs.Put("foo", "baz", longAfter) );
			Assert.IsNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			longLongAfter = Timestamper.Next();

			Assert.AreEqual("baz", ccs.Get("foo", longLongAfter) );

		}
	}
}
