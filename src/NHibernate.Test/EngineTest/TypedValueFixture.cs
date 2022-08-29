using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.EngineTest
{
	[TestFixture]
	public class TypedValueFixture
	{
		[Test]
		public void EqualsCollection()
		{
			ArrayList value1 = new ArrayList();
			value1.Add(10);
			value1.Add(20);

			ArrayList value2 = (ArrayList) value1.Clone();

			TypedValue t1 = new TypedValue(NHibernateUtil.Int32, value1);
			TypedValue t2 = new TypedValue(NHibernateUtil.Int32, value2);

			Assert.IsTrue(t1.Equals(t2));
		}

		[Test]
		public void EqualsCollectionWithHashSet()
		{
			var value1 = new[] { 10, 20 };

			var value2 = new HashSet<int> { 10, 20 };

			var t1 = new TypedValue(NHibernateUtil.Int32, value1, true);
			var t2 = new TypedValue(NHibernateUtil.Int32, value2, true);

			Assert.That(t1.Equals(t2), Is.True, "t1.Equals(t2)");
			Assert.That(t2.Equals(t1), Is.True, "t2.Equals(t1)");
		}

		[Test]
		public void ToStringWithNullValue()
		{
			Assert.AreEqual("null", new TypedValue(NHibernateUtil.Int32, null).ToString());
		}

		[Test]
		public void WhenTheTypeIsAnArray_ChoseTheDefaultComparer()
		{
			byte[] value = new byte[] { 1, 2, 3 };

			var tv = new TypedValue(NHibernateUtil.BinaryBlob, value);

			Assert.That(tv.Comparer, Is.TypeOf<TypedValue.DefaultComparer>());
		}

		[Test]
		public void PaddedShouldNotEqualShort()
		{
			var v1 = new TypedValue(NHibernateUtil.Int32, ToLazyEnumerable(3, 2, 1, 0, 0, 0), true);
			var v2 = new TypedValue(NHibernateUtil.Int32, ToLazyEnumerable(3, 2, 1, 0), true);

			Warn.If(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()), "Equal hash codes");
			Assert.That(v1, Is.Not.EqualTo(v2));
		}

		[Test]
		public void ShortShouldNotEqualLonger()
		{
			var v1 = new TypedValue(NHibernateUtil.Int32, ToLazyEnumerable(1, 2, 3), true);
			var v2 = new TypedValue(NHibernateUtil.Int32, ToLazyEnumerable(1, 2, 3, 1, -1), true);

			Warn.If(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()), "Equal hash codes");
			Assert.That(v1, Is.Not.EqualTo(v2));
		}

		static IEnumerable<T> ToLazyEnumerable<T>(params T[] array)
		{
			// We want to get an IEnumerable which is lazy (eg does not implement ICollection interface)

			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var item in array) yield return item;
		}
	}
}
