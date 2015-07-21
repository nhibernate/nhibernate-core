using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Test for the IdentityMap.
	/// </summary>
	[TestFixture]
	public class IdentitySetFixture
	{
		protected MutableHashCode item1 = null;
		protected MutableHashCode item2 = null;
		protected IDictionary expectedMap = null;

		protected NoHashCode noHashCode1 = null;
		protected NoHashCode noHashCode2 = null;

		protected object value1 = null;
		protected object value2 = null;

		[SetUp]
		public void SetUp()
		{
			item1 = new MutableHashCode(1);
			item2 = new MutableHashCode(2);

			value1 = new object();
			value2 = new object();

			noHashCode1 = new NoHashCode();
			noHashCode2 = new NoHashCode();

			expectedMap = new Hashtable();
			expectedMap.Add(item1, value1);
			expectedMap.Add(item2, value2);
		}

		protected virtual IdentitySet GetIdentitySet()
		{
			return new IdentitySet();
		}

		/// <summary>
		/// Verify that the object being added as the Key does not have it's GetHashCode
		/// method called.
		/// </summary>
		[Test]
		public void AddNoHashCode()
		{
			ISet<object> set = GetIdentitySet();
			set.Add(noHashCode1);

			Assert.AreEqual(1, set.Count, "The item was added succesfully");
		}


		/// <summary>
		/// An IdentityMap can not use a ValueType as the Key because of the boxing/unboxing
		/// that occurs with them.  This verifies that an Exception is thrown if a ValueType 
		/// is used as the key.
		/// </summary>
		[Test]
		public void AddValueTypeException()
		{
			ISet<object> set = GetIdentitySet();
			int intKey = 3;
			Assert.Throws<ArgumentException>(() => set.Add(intKey));
		}

		[Test]
		public void Count()
		{
			ISet<object> set = GetIdentitySet();
			set.Add(new object());
			set.Add(new object());

			Assert.AreEqual(2, set.Count, "Expect 2 items in the IdentitySet");
		}

		/// <summary>
		/// Test that two different references to the same object passed to the Contains method
		/// both return true.
		/// </summary>
		[Test]
		public void ContainsSameObjectByRef()
		{
			ISet<object> set = GetIdentitySet();

			MutableHashCode item1Copy = item1;

			set.Add(item1);

			Assert.AreSame(item1, item1Copy);
			Assert.IsTrue(set.Contains(item1Copy), "We should be able to get the same object out of the IdentitySet with " +
												   "two different references to the same object.");
		}

		/// <summary>
		/// Test that even though the HashCode and Equals of the same reference have been changed
		/// that the Contains still recognizes it by the Identity of the object - not the values.
		/// </summary>
		[Test]
		public void ContainsSameObjectWithDiffEquals()
		{
			ISet<object> map = GetIdentitySet();

			map.Add(item1);

			item1.HashCodeField = 5;

			Assert.IsTrue(map.Contains(item1),
						  "Even though item1's HashCode field change the IdentityMap.Contains() should still return true");
		}

		/// <summary>
		/// Test to make sure that two objects that are equal by the Equals definition of the class MutableHashCode
		/// do not get translated to the same key because they are different objects.
		/// </summary>
		[Test]
		public void ContainsDiffObjectWithEquals()
		{
			ISet<object> map = GetIdentitySet();
			item1.HashCodeField = 4;
			item2.HashCodeField = 4;

			map.Add(item1);

			Assert.AreEqual(item1, item2, "They should be equal.");
			Assert.IsFalse(map.Contains(item2), "Even though item1.Equals(item2) IdentitySet should not find by item2");
		}


		/// <summary>
		/// Add the same MutableHashCode class twice and ensure there is only
		/// one item in the IdentitySet.
		/// </summary>
		[Test]
		public void SetItemChangedHashCodeTwice()
		{
			ISet<object> actualMap = GetIdentitySet();

			actualMap.Add(item1);

			// change the Property that GetHashCode method uses
			item1.HashCodeField = 2;
			actualMap.Add(item1);
			Assert.AreEqual(1, actualMap.Count, "Should only be 1 item in the IdentitySet");
		}

		/// <summary>
		/// Adds two different objects that are Equal() to each other to verify that
		/// it does not use the objects Equal() but instead the IdentitySet.
		/// </summary>
		[Test]
		public void SetItemsEqualHashCodeDiffIdentity()
		{
			ISet<object> actualSet = GetIdentitySet();
			IDictionary normalMap = new Hashtable();

			item1.HashCodeField = 3;
			item2.HashCodeField = 3;

			Assert.AreEqual(item1, item2, "The two objects are equal");
			Assert.IsTrue(item1 != item2, "The two items are different objects in memory");

			normalMap[item1] = value1;
			normalMap[item2] = value2;

			Assert.AreEqual(1, normalMap.Count, "The Hashtable should have 1 element");

			actualSet.Add(item1);
			actualSet.Add(item2);

			Assert.AreEqual(2, actualSet.Count, "The IdentityMap should have 2 elements");
		}

	}
}