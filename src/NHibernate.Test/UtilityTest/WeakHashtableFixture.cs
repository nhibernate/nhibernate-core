using System;
using System.Collections;
using System.Runtime.CompilerServices;

using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class WeakHashtableFixture
	{
		private static WeakHashtable Create()
		{
			return new WeakHashtable();
		}

		// NoInlining to keep temporary variables' lifetime from being extended.
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static WeakHashtable CreateWithTwoObjects()
		{
			var table = Create();

			table[new object()] = new object();
			table[new object()] = new object();

			return table;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static WeakRefWrapper CreateWeakRefWrapper()
		{
			object obj = new object();
			return new WeakRefWrapper(obj);
		}

		[Test]
		public void Basic()
		{
			// Keep references to the key and the value
			object key = new object();
			object value = new object();

			WeakHashtable table = Create();

			table[key] = value;

			Assert.AreSame(value, table[key]);
		}

		[Test]
		public void WeakReferenceGetsFreedButHashCodeRemainsConstant()
		{
			WeakRefWrapper wr = CreateWeakRefWrapper();
			int hashCode = wr.GetHashCode();

			GC.Collect();

			Assert.IsFalse(wr.IsAlive);
			Assert.IsNull(wr.Target);
			Assert.AreEqual(hashCode, wr.GetHashCode());
		}

		[Test]
		public void Scavenging()
		{
			WeakHashtable table = CreateWithTwoObjects();

			GC.Collect();
			table.Scavenge();

			Assert.AreEqual(0, table.Count);
		}

		[Test]
		public void IterationAfterGC()
		{
			WeakHashtable table = CreateWithTwoObjects();

			GC.Collect();

			Assert.AreEqual(2, table.Count, "should not have been scavenged yet");
			Assert.IsFalse(table.GetEnumerator().MoveNext(), "should not have live elements");
		}

		[Test]
		public void Iteration()
		{
			object key = new object();
			object value = new object();

			WeakHashtable table = Create();
			table[key] = value;

			foreach (DictionaryEntry de in table)
			{
				Assert.AreSame(key, de.Key);
				Assert.AreSame(value, de.Value);
			}
		}

		[Test]
		public void RetrieveNonExistentItem()
		{
			WeakHashtable table = Create();
			object obj = table[new object()];
			Assert.IsNull(obj);
		}

		[Test]
		public void WeakRefWrapperEquals()
		{
			object obj = new object();
			Assert.AreEqual(new WeakRefWrapper(obj), new WeakRefWrapper(obj));
			Assert.IsFalse(new WeakRefWrapper(obj).Equals(null));
			Assert.IsFalse(new WeakRefWrapper(obj).Equals(10));
		}

		[Test]
		public void IsSerializable()
		{
			WeakHashtable weakHashtable = new WeakHashtable();
			weakHashtable.Add("key", new object());
			NHAssert.IsSerializable(weakHashtable);
		}
	}
}