using System;
using NUnit.Framework;

namespace NHibernate.Cache {

	[TestFixture]
	public class ICacheTest {
		
		[Test]
		public void TestSimpleCache() {
			DoTestCache( new SimpleCache() );
		}
	
		public void DoTestCache(ICache cache) {
			long longBefore = Timestamper.Next();

			System.Threading.Thread.Sleep(15);

			long before = Timestamper.Next();

			System.Threading.Thread.Sleep(15);

			ICacheConcurrencyStrategy ccs = new ReadWriteCache(cache);

			// cache something

			Assertion.Assert( ccs.Put("foo", "foo", before) );

			System.Threading.Thread.Sleep(15);

			long after = Timestamper.Next();

			Assertion.AssertNull( ccs.Get("foo", longBefore) );
			Assertion.AssertEquals( "foo", ccs.Get("foo", after) );
			Assertion.Assert( !ccs.Put("foo", "foo", before) );

			// update it;

			ccs.Lock("foo");

			Assertion.AssertNull( ccs.Get("foo", after) );
			Assertion.AssertNull( ccs.Get("foo", longBefore) );
			Assertion.Assert( !ccs.Put("foo", "foo", before) );

			System.Threading.Thread.Sleep(15);

			long whileLocked = Timestamper.Next();

			Assertion.Assert( !ccs.Put("foo", "foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo");

			Assertion.AssertNull( ccs.Get("foo", after) );
			Assertion.AssertNull( ccs.Get("foo", longBefore) );
			Assertion.Assert( !ccs.Put("foo", "bar", whileLocked) );
			Assertion.Assert( !ccs.Put("foo", "bar", after) );

			System.Threading.Thread.Sleep(15);

			long longAfter = Timestamper.Next();

			Assertion.Assert( ccs.Put("foo", "baz", longAfter) );
			Assertion.AssertNull( ccs.Get("foo", after) );
			Assertion.AssertNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			long longLongAfter = Timestamper.Next();

			Assertion.AssertEquals("baz", ccs.Get("foo", longLongAfter) );

			// update it again, with multiple locks

			ccs.Lock("foo");
			ccs.Lock("foo");

			Assertion.AssertNull( ccs.Get("foo", longLongAfter) );

			System.Threading.Thread.Sleep(15);

			whileLocked = Timestamper.Next();

			Assertion.Assert( !ccs.Put("foo", "foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo");

			System.Threading.Thread.Sleep(15);

			long betweenReleases = Timestamper.Next();

			Assertion.Assert( !ccs.Put("foo", "bar", betweenReleases) );
			Assertion.AssertNull( ccs.Get("foo", betweenReleases) );

			System.Threading.Thread.Sleep(15);

			ccs.Release("foo");

			Assertion.Assert( !ccs.Put("foo", "bar", whileLocked) );

			System.Threading.Thread.Sleep(15);

			longAfter = Timestamper.Next();

			Assertion.Assert( ccs.Put("foo", "baz", longAfter) );
			Assertion.AssertNull( ccs.Get("foo", whileLocked) );

			System.Threading.Thread.Sleep(15);

			longLongAfter = Timestamper.Next();

			Assertion.AssertEquals("baz", ccs.Get("foo", longLongAfter) );

		}
	}
}
