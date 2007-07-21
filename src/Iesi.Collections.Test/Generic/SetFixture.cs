using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Iesi.Collections.Generic.Test
{
	/// <summary>
	/// Summary description for SetFixture.
	/// </summary>
	public abstract class GenericSetFixture
	{
		private IList<string> _aInitValues;
		private IList<string> _bInitValues;
		protected ISet<string> _set;

		public static string one = "one";
		public static string two = "two";
		public static string three = "three";

		[SetUp]
		public virtual void SetUp()
		{
			_aInitValues = new List<string>();
			_aInitValues.Add("zero");
			_aInitValues.Add("one");
			_aInitValues.Add("two");
			_aInitValues.Add("three");

			_bInitValues = new List<string>();
			_bInitValues.Add("two");
			_bInitValues.Add("three");
			_bInitValues.Add("four");

			_set = CreateInstance(new string[] {one, two, three});
		}

		#region System.IClonable Member Tests

		[Test]
		public void Clone()
		{
			ISet<string> clonedSet = (ISet<string>) _set.Clone();

			Assert.AreEqual(ExpectedType, clonedSet.GetType(), "cloned set should be the same type");
			Assert.AreEqual(_set.Count, clonedSet.Count, "set and cloned version should be same");

			try
			{
				clonedSet.Add("not in original");
				Assert.IsFalse(_set.Count == clonedSet.Count, "adding to clone should not add to original.");
				if (clonedSet.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!clonedSet.IsReadOnly)
					throw;
			}

			foreach (string str in _set)
			{
				Assert.IsTrue(clonedSet.Contains(str), "cloned set should have same objects as original set.");
			}
		}

		#endregion

		#region System.Collections.ICollection Member Tests

		[Test]
		public void CopyTo()
		{
			string[] dest = new string[3];
			_set.CopyTo(dest, 0);

			int count = 0;

			foreach (string obj in dest)
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

		#region Iesi.Collections.ISet<string> Constructor Tests

		[Test]
		public void CtorWithDefaults()
		{
			List<string> init = new List<string>(3);
			init.Add("one");
			init.Add("two");
			init.Add("three");

			ISet<string> theSet = CreateInstance(init);

			Assert.AreEqual(3, init.Count, "3 items in set");

			int index = 0;
			foreach (string obj in init)
			{
				Assert.IsTrue(theSet.Contains(obj), "set should contain obj at index = " + index.ToString());
				index++;
			}
		}

		#endregion

		#region Iesi.Collections.ISet<string> Member Tests

		[Test]
		public void Add()
		{
			try
			{
				Assert.IsTrue(_set.Add("four"), "should have added 'four'");
				Assert.AreEqual(4, _set.Count, "should have added 'four'");

				Assert.IsFalse(_set.Add(two), "'two' was already there");
				Assert.AreEqual(4, _set.Count, "object already in set");
				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
		}

		[Test]
		public void AddAll()
		{
			List<string> addAll = new List<string>(3);
			addAll.Add("four");
			addAll.Add("five");
			addAll.Add("four");

			try
			{
				Assert.IsTrue(_set.AddAll(addAll), "should have modified set");
				Assert.AreEqual(5, _set.Count, "should have added one 'four' and 'five'");

				Assert.IsFalse(_set.AddAll(addAll), "all elements already in set");
				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
		}

		[Test]
		public void Clear()
		{
			try
			{
				_set.Clear();
				Assert.AreEqual(0, _set.Count, "should have no items in ISet.");

				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
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
			List<string> all = new List<string>(2);
			all.Add("one");
			all.Add("two");

			Assert.IsTrue(_set.ContainsAll(all), "should contain 'one' and 'two'");

			all.Add("not in there");
			Assert.IsFalse(_set.ContainsAll(all), "should not contain the just added 'not in there'");
		}

		[Test]
		public void ExclusiveOr()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = Set<string>.ExclusiveOr(a, b);

			Assert.AreEqual(3, ab.Count, "contains 3 elements - 'zero', 'one', and 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet<string> aNull = Set<string>.ExclusiveOr(a, null);
			Assert.AreEqual(_aInitValues.Count, aNull.Count, "count still same");
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "all A elements kept");

			ISet<string> bNull = Set<string>.ExclusiveOr(null, b);
			Assert.AreEqual(_bInitValues.Count, bNull.Count, "count still same");
			Assert.IsTrue(bNull.ContainsAll(_bInitValues), "all B elements kept");

			ISet<string> bothNull = Set<string>.ExclusiveOr(null, null);
			Assert.AreEqual(null, bothNull, "two null sets return null set");
		}

		[Test]
		public void Intersect()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = Set<string>.Intersect(a, b);

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'two', and 'three'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet<string> aNull = Set<string>.Intersect(a, null);
			Assert.AreEqual(0, aNull.Count, "no elements intersected with null set");

			ISet<string> bNull = Set<string>.Intersect(null, b);
			Assert.AreEqual(0, bNull.Count, "no elements intersected with null set");

			ISet<string> bothNull = Set<string>.Intersect(null, null);
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
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = Set<string>.Minus(a, b);

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'zero', and 'one'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet<string> aNull = Set<string>.Minus(a, null);
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "should have removed no elements");

			ISet<string> bNull = Set<string>.Minus(null, b);
			Assert.AreEqual(null, bNull, "null set remained null");

			ISet<string> bothNull = Set<string>.Minus(null, null);
			Assert.AreEqual(null, bothNull, "both sets are null");
		}

		[Test]
		public void Remove()
		{
			try
			{
				Assert.IsTrue(_set.Remove(one), "should have removed 'one'");
				Assert.IsFalse(_set.Contains(one), "one should have been removed");
				Assert.AreEqual(2, _set.Count, "should be 2 items after one removed.");

				Assert.IsFalse(_set.Remove(one), "was already removed.");
				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
		}

		[Test]
		public void RemoveAll()
		{
			List<string> all = new List<string>(2);
			all.Add(one);
			all.Add("not in there");

			try
			{
				Assert.IsTrue(_set.RemoveAll(all), "should have removed an element");
				Assert.AreEqual(2, _set.Count, "should be down to 2 elements.");
				Assert.IsFalse(_set.RemoveAll(all), "all of the elements already removed so set not modified.");
				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
		}

		[Test]
		public void RetainAll()
		{
			List<string> retain = new List<string>(2);
			retain.Add(one);
			retain.Add("not in there");

			try
			{
				Assert.IsTrue(_set.RetainAll(retain), "set was modified");
				Assert.AreEqual(1, _set.Count, "only 1 element retained");

				Assert.IsFalse(_set.RetainAll(retain), "set was not modified");
				if (_set.IsReadOnly)
					Assert.Fail("Read-only set can be modified");
			}
			catch (NotSupportedException)
			{
				if (!_set.IsReadOnly)
					throw;
			}
		}

		[Test]
		public void Union()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = Set<string>.Union(a, b);

			Assert.AreEqual(5, ab.Count, "contains 5 elements - 'zero' through 'four'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");
			Assert.IsTrue(ab.Contains("four"), "should contain 'four'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");

			ISet<string> aNull = Set<string>.Union(a, null);
			Assert.AreEqual(_aInitValues.Count, aNull.Count, "count not changed");
			Assert.IsTrue(aNull.ContainsAll(_aInitValues), "still contains all initial values");

			ISet<string> bNull = Set<string>.Union(null, b);
			Assert.AreEqual(_bInitValues.Count, bNull.Count, "count not changed");
			Assert.IsTrue(bNull.ContainsAll(_bInitValues), "still contains all initial values");

			ISet<string> bothNull = Set<string>.Union(null, null);
			Assert.AreEqual(null, bothNull, "two nulls intersect as null");
		}

		#endregion

		#region Iesi.Collection.ISet<string> Operator Tests

		[Test]
		public void ExclusiveOrOperator()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = (Set<string>) a ^ (Set<string>) b;

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
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = (Set<string>) a & (Set<string>) b;

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'two', and 'three'");
			Assert.IsTrue(ab.Contains("two"), "should contain 'two'");
			Assert.IsTrue(ab.Contains("three"), "should contain 'three'");
		}

		[Test]
		public void MinusOperator()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = (Set<string>) a - (Set<string>) b;

			Assert.AreEqual(2, ab.Count, "contains 2 elements - 'zero', and 'one'");
			Assert.IsTrue(ab.Contains("zero"), "should contain 'zero'");
			Assert.IsTrue(ab.Contains("one"), "should contain 'one'");

			Assert.IsTrue(a.ContainsAll(_aInitValues), "should not have modified a");
			Assert.IsTrue(b.ContainsAll(_bInitValues), "should not have modified b");
		}

		[Test]
		public void UnionOperator()
		{
			ISet<string> a = CreateInstance(_aInitValues);
			ISet<string> b = CreateInstance(_bInitValues);

			ISet<string> ab = (Set<string>) a | (Set<string>) b;

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

		protected abstract ISet<string> CreateInstance();

		protected abstract ISet<string> CreateInstance(ICollection<string> init);

		protected abstract Type ExpectedType { get; }
	}
}
