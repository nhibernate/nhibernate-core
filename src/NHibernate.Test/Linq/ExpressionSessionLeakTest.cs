using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq {
	public class ExpressionSessionLeakTest : LinqTestCase {
		[Test]
		public void SessionGetsCollected() {

			var leakTest = session.SessionFactory.OpenSession();

			// Comment this line out to make the test pass
			var query = leakTest.Query<Customer>().FirstOrDefault(t => t.CustomerId != "");
			leakTest.Dispose();

			GCAssert.IsGarbageCollected(ref leakTest);
		}
	}

	public static class GCAssert {
		public static void IsGarbageCollected<TObject>(ref TObject @object)
			where TObject : class {
			Action<TObject> emptyAction = o => { };
			IsGarbageCollected(ref @object, emptyAction);
		}

		public static void IsGarbageCollected<TObject>(
			ref TObject @object,
			Action<TObject> useObject)
			where TObject : class {
			if (typeof (TObject) == typeof (string)) {
				// Strings are copied by value, and don't leak anyhow.
				return;
			}

			useObject(@object);
			WeakReference reference = new WeakReference(@object, true);
			@object = null;

			// The object should have gone out of scope about now, 
			// so the garbage collector can clean it up.
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.WaitForFullGCComplete();
			GC.Collect();

			Assert.Null(reference.Target);
		}
	}
}
