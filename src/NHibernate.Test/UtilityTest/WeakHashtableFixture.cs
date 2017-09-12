using System;

using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class WeakHashtableFixture
	{
		protected WeakHashtable<object, object> Create()
		{
			return new WeakHashtable<object, object>();
		}

		[Test]
		public void Basic()
		{
			// Keep references to the key and the value
			object key = new object();
			object value = new object();

			var table = Create();

			table[key] = value;

			Assert.AreSame(value, table[key]);
		}

		[Test]
		public void WeakReferenceGetsFreedButHashCodeRemainsConstant()
		{
			var obj = new object();
			var wr = WeakRefWrapper<object>.Wrap(obj);
			int hashCode = wr.GetHashCode();
			obj = null;

			GC.Collect();

			Assert.IsFalse(wr.TryGetTarget(out var target));
			Assert.IsNull(target);
			Assert.AreEqual(hashCode, wr.GetHashCode());
		}

		[Test]
		public void Scavenging()
		{
			var table = Create();

			table[new object()] = new object();
			table[new object()] = new object();

			GC.Collect();
			table.Scavenge();

			Assert.AreEqual(0, table.Count);
		}

		[Test]
		public void IterationAfterGC()
		{
			var table = Create();

			table[new object()] = new object();
			table[new object()] = new object();

			GC.Collect();

			Assert.AreEqual(2, table.Count, "should not have been scavenged yet");
			Assert.IsFalse(table.GetEnumerator().MoveNext(), "should not have live elements");
		}

		[Test]
		public void Iteration()
		{
			object key = new object();
			object value = new object();

			var table = Create();
			table[key] = value;

			foreach (var de in table)
			{
				Assert.AreSame(key, de.Key);
				Assert.AreSame(value, de.Value);
			}
		}

		[Test]
		public void RetrieveNonExistentItem()
		{
			var table = Create();
			table.TryGetValue(new object(), out var obj);
			Assert.IsNull(obj);
		}

		[Test]
		public void WeakRefWrapperEquals()
		{
			object obj = new object();
			Assert.AreEqual(WeakRefWrapper<object>.Wrap(obj), WeakRefWrapper<object>.Wrap(obj));
			Assert.IsFalse(WeakRefWrapper<object>.Wrap(obj).Equals(null));
			Assert.IsFalse(WeakRefWrapper<object>.Wrap(obj).Equals(10));
		}

		[Test]
		public void IsSerializable()
		{
			var weakHashtable = Create();
			weakHashtable.Add("key", new object());
			NHAssert.IsSerializable(weakHashtable);
		}
	}
}
