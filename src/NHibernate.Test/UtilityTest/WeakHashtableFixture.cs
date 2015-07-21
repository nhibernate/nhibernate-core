using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class WeakHashtableFixture
	{
		protected WeakHashtable Create()
		{
			return new WeakHashtable();
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
			object obj = new object();
			WeakRefWrapper wr = new WeakRefWrapper(obj);
			int hashCode = wr.GetHashCode();
			obj = null;

			GC.Collect();

			Assert.IsFalse(wr.IsAlive);
			Assert.IsNull(wr.Target);
			Assert.AreEqual(hashCode, wr.GetHashCode());
		}

		[Test]
		public void Scavenging()
		{
			WeakHashtable table = Create();

			table[new object()] = new object();
			table[new object()] = new object();

			GC.Collect();
			table.Scavenge();

			Assert.AreEqual(0, table.Count);
		}

		[Test]
		public void IterationAfterGC()
		{
			WeakHashtable table = Create();

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