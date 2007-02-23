using System;
using System.Collections;

using NUnit.Framework;

namespace Iesi.Collections.Test
{
	/// <summary>
	/// Summary description for SetFixture.
	/// </summary>
	public abstract class SetFixture
	{
		private IList _aInitValues;
		private IList _bInitValues;
		protected ISet _set;

		public static object one = "one";
		public static object two = "two";
		public static object three = "three";

		[SetUp]
		public virtual void SetUp()
		{
			_aInitValues = new ArrayList();
			_aInitValues.Add("zero");
			_aInitValues.Add("one");
			_aInitValues.Add("two");
			_aInitValues.Add("three");

			_bInitValues = new ArrayList();
			_bInitValues.Add("two");
			_bInitValues.Add("three");
			_bInitValues.Add("four");

			_set = CreateInstance();
			_set.Add(one);
			_set.Add(two);
			_set.Add(three);
		}

		#region System.IClonable Member Tests

		[Test]
		public void Clone()
		{
			ISet clonedSet = (ISet) _set.Clone();

			Assert.AreEqual(ExpectedType, clonedSet.GetType(), "cloned set should be the same type");
			Assert.AreEqual(_set.Count, clonedSet.Count, "set and cloned version should be same");

			clonedSet.Add("not in original");
			Assert.IsFalse(_set.Count == clonedSet.Count, "adding to clone should not add to original.");

			foreach (object obj in _set)
			{
				Assert.IsTrue(clonedSet.Contains(obj), "cloned set should have same objects as original set.");
			}
		}

		#endregion

		#region System.Collections.ICollection Member Tests

		[Test]
		public void CopyTo()
		{
			object[] dest = new object[3];
			_set.CopyTo(dest, 0);

			int count = 0;

			foreach (object obj in dest)
			{
				Assert.IsTrue(_set.Contains(obj), "set should contain the object in the array");
				count++;
			}

			Assert.AreEqual(3, count, "should have 3 items in array");
		}

		[Test]
		public void Count()
		{
			Assert.AreEqual(3, _set.Count, "should be 3 items");
			Assert.AreEqual(0, CreateInstance().Count, "new set should have nothing in it.");
		}

		#endregion

		#region Iesi.Collections.ISet Constructor Tests

		[Test]
		public void CtorWithDefaults()
		{
			ArrayList init = new ArrayList(3);
			init.Add("one");
			init.Add("two");
			init.Add("three");

			ISet theSet = CreateInstance(init);

			Assert.AreEqual(3, init.Count, "3 items in set");

			int index = 0;
			foreach (object obj in init)
			{
				Assert.IsTrue(theSet.Contains(obj), "set should contain obj at index = " + index.ToString());
				index++;
			}
		}

		#endregion

		#region Iesi.Collections.ISet Member Tests

		[Test]
		public void Add()
		{
			Assert.IsTrue(_set.Add("four"), "should have added 'four'");
			Assert.AreEqual(4, _set.Count, "should have added 'four'");

			Assert.IsFalse(_set.Add(two), "'two' was already there");
			Assert.AreEqual(4, _set.Count, "object already in set");
		}

		[Test]
		public void AddAll()
		{
			ArrayList addAll = new ArrayList(3);
			addAll.Add("four");
			addAll.Add("five");
			addAll.Add("four");

			Assert.IsTrue(_set.AddAll(addAll), "should have modified set");
			Assert.AreEqual(5, _set.Count, "should have added one 'four' and 'five'");

			Assert.IsFalse(_set.AddAll(addAll), "all elements already in set");
		}

		[Test]
		public void Clear()
		{
			_set.Clear();
			Assert.AreEqual(0, _set.Count, "should have no items in ISet.");
		}

		[Test]
		public void Contains()
		{
			Assert.IsTrue(_set.Contains(one), "does contain one");
			Assert.IsFalse(_set.Contains("four"), "does not contain 'four'");
		}

		[Test]
		public void ContainsAll()
		{
			ArrayList all = new ArrayList(2);
			all.Add("one");
			all.Add("two");

			Assert.IsTrue(_set.ContainsAll(all), "should contain 'one' and 'two'");

			all.Add("not in there");
			Assert.IsFalse(_set.ContainsAll(all), "should not contain the just added 'not in there'");
		}

		[Test]
		public void ExclusiveOr()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = Set.ExclusiveOr(a, b);

			Assert.AreEqual(3, ab.Count, "contains 3 elements - 'zero', 'one', and 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet aNull = Set.ExclusiveOr(a, null);
			Assert.AreEqual(_aInitValues.Count, aNull.Count, "count still same");
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "all A elements kept");

			ISet bNull = Set.ExclusiveOr(null, b);
			Assert.AreEqual(_bInitValues.Count, bNull.Count, "count still same");
			Assert.IsTrue(bNull.ContainsAll(_bInitValues), "all B elements kept");

			ISet bothNull = Set.ExclusiveOr(null, null);
			Assert.AreEqual(null, bothNull, "two null sets return null set");
		}

		[Test]
		public void Intersect()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = Set.Intersect(a, b);

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'two', and 'three'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet aNull = Set.Intersect(a, null);
			Assert.AreEqual(0, aNull.Count, "no elements intersected with null set");

			ISet bNull = Set.Intersect(null, b);
			Assert.AreEqual(0, bNull.Count, "no elements intersected with null set");

			ISet bothNull = Set.Intersect(null, null);
			Assert.AreEqual(null, bothNull, "null sets intersect as null set");
		}

		[Test]
		public void IsEmpty()
		{
			Assert.IsFalse(_set.IsEmpty, "set should have initial values");

			Assert.IsTrue(CreateInstance().IsEmpty, "new set is empty");
		}

		[Test]
		public void Minus()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = Set.Minus(a, b);

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'zero', and 'one'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet aNull = Set.Minus(a, null);
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "should have removed no elements");

			ISet bNull = Set.Minus(null, b);
			Assert.AreEqual(null, bNull, "null set remained null");

			ISet bothNull = Set.Minus(null, null);
			Assert.AreEqual(null, bothNull, "both sets are null");
		}

		[Test]
		public void Remove()
		{
			Assert.IsTrue(_set.Remove(one), "should have removed 'one'");
			Assert.IsFalse(_set.Contains(one), "one should have been removed");
			Assert.AreEqual(2, _set.Count, "should be 2 items after one removed.");

			Assert.IsFalse(_set.Remove(one), "was already removed.");
		}

		[Test]
		public void RemoveAll()
		{
			ArrayList all = new ArrayList(2);
			all.Add(one);
			all.Add("not in there");

			Assert.IsTrue(_set.RemoveAll(all), "should have removed an element");
			Assert.AreEqual(2, _set.Count, "should be down to 2 elements.");
			Assert.IsFalse(_set.RemoveAll(all), "all of the elements already removed so set not modified.");
		}

		[Test]
		public void RetainAll()
		{
			ArrayList retain = new ArrayList(2);
			retain.Add(one);
			retain.Add("not in there");

			Assert.IsTrue(_set.RetainAll(retain), "set was modified");
			Assert.AreEqual(1, _set.Count, "only 1 element retained");

			Assert.IsFalse(_set.RetainAll(retain), "set was not modified");
		}

		[Test]
		public void Union()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = Set.Union(a, b);

			Assert.AreEqual(5, ab.Count, "contains 5 elements - 'zero' through 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet aNull = Set.Union(a, null);
			Assert.AreEqual(_aInitValues.Count, aNull.Count, "count not changed");
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "still contains all initial values");

			ISet bNull = Set.Union(null, b);
			Assert.AreEqual(_bInitValues.Count, bNull.Count, "count not changed");
			Assert.IsTrue(bNull.ContainsAll(_bInitValues), "still contains all initial values");

			ISet bothNull = Set.Union(null, null);
			Assert.AreEqual(null, bothNull, "two nulls intersect as null");
		}

		#endregion

		#region Iesi.Collection.ISet Operator Tests

		[Test]
		public void ExclusiveOrOperator()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = (Set) a ^ (Set) b;

			Assert.AreEqual(3, ab.Count, "contains 3 elements - 'zero', 'one', and 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");
		}

		[Test]
		public void IntersectOperator()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = (Set) a & (Set) b;

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'two', and 'three'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");
		}

		[Test]
		public void MinusOperator()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = (Set) a - (Set) b;

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'zero', and 'one'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");
		}

		[Test]
		public void UnionOperator()
		{
			ISet a = CreateInstance(_aInitValues);
			ISet b = CreateInstance(_bInitValues);

			ISet ab = (Set) a | (Set) b;

			Assert.AreEqual(5, ab.Count, "contains 5 elements - 'zero' through 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");
		}

		#endregion

		protected abstract ISet CreateInstance();

		protected abstract ISet CreateInstance(ICollection init);

		protected abstract Type ExpectedType { get; }
	}
}