using System;
using System.Collections;

using NHibernate.Util;

using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Test for the IdentityMap.
	/// </summary>
	[TestFixture]
	public class IdentityMapFixture
	{
		protected MutableHashCode item1 = null;
		protected object value1 = null;
		protected MutableHashCode item2 = null;
		protected object value2 = null;
		protected IDictionary expectedMap = null;

		[SetUp]
		public void SetUp() 
		{
			item1 = new MutableHashCode(1);
			value1 = new object();
			item2 = new MutableHashCode(2);
			value2 = new object();
			
			expectedMap = new Hashtable();
			expectedMap.Add(item1, value1);
			expectedMap.Add(item2, value2);

		}


		/// <summary>
		/// An IdentityMap can not use a ValueType as the Key because of the boxing/unboxing
		/// that occurs with them.  This verifies that an Exception is thrown if a ValueType 
		/// is used as the key.
		/// </summary>
		[Test]
		[ExpectedException(typeof(System.ArgumentException))]
		public void AddValueTypeException() 
		{
			IDictionary map = IdentityMap.Instantiate();
			int intKey = 3;
			object objectValue = new object();
			map.Add(intKey, objectValue); 

		}

		[Test]
		public void Count() 
		{
			IDictionary map = IdentityMap.Instantiate();
			map.Add(new object(), new object());
			map.Add(new object(), new object());

			Assert.AreEqual(2, map.Count, "Expect 2 items in the IdentityMap");

		}

		/// <summary>
		/// Test that two different references to the same object passed to the Contains method
		/// both return true.
		/// </summary>
		[Test]
		public void ContainsSameObjectByRef() 
		{
			IDictionary map = IdentityMap.Instantiate();

			MutableHashCode item1Copy = item1;
			
			map.Add(item1, new object());

			Assert.AreSame(item1, item1Copy);
			Assert.IsTrue(map.Contains(item1Copy), "We should be able to get the same object out of the IdentityMap with " +
				"two different references to the same object."); 
		}

		/// <summary>
		/// Test that even though the HashCode and Equals of the same reference have been changed
		/// that the Contains still recognizes it by the Identity of the object - not the values.
		/// </summary>
		[Test]
		public void ContainsSameObjectWithDiffEquals()
		{
			IDictionary map = IdentityMap.Instantiate();

			map.Add(item1, new object());

			item1.HashCodeField = 5;

			Assert.IsTrue(map.Contains(item1), "Even though item1's HashCode field change the IdentityMap.Contains() should still return true");

		}

		/// <summary>
		/// Test to make sure that two objects that are equal by the Equals definition of the class MutableHashCode
		/// do not get translated to the same key because they are different objects.
		/// </summary>
		[Test]
		public void ContainsDiffObjectWithEquals() 
		{
			IDictionary map = IdentityMap.Instantiate();
			item1.HashCodeField = 4;
			item2.HashCodeField = 4;

			map.Add(item1, new object());

			Assert.AreEqual(item1, item2, "They should be equal.");
			Assert.IsFalse(map.Contains(item2), "Even though item1.Equals(item2) IdentityMap should not find by item2");

		}


		/// <summary>
		/// Add the same MutableHashCode class twice and ensure there is only
		/// one item in the IdentityMap.
		/// </summary>
		[Test]
		public void SetItemChangedHashCodeTwice() 
		{
			IDictionary actualMap = IdentityMap.Instantiate();

			actualMap[item1] =  value1;

			// change the Property that GetHashCode method uses
			item1.HashCodeField = 2;
			actualMap[item1] = value1;
			Assert.AreEqual(1, actualMap.Count, "Should only be 1 item in the IdentityMap");
		}

		/// <summary>
		/// Adds two different objects that are Equal() to each other to verify that
		/// it does not use the objects Equal() but instead the IdentityMap.
		/// </summary>
		[Test]
		public void SetItemsEqualHashCodeDiffIdentity() 
		{
			IDictionary actualMap = IdentityMap.Instantiate();
			IDictionary normalMap = new Hashtable();

			item1.HashCodeField = 3;
			item2.HashCodeField = 3;

			Assert.AreEqual(item1, item2, "The two objects are equal");
			Assert.IsTrue(item1!=item2, "The two items are different objects in memory");
			
			normalMap[item1] = value1;
			normalMap[item2] = value2;

			Assert.AreEqual(1, normalMap.Count, "The Hashtable should have 1 element");
			
			actualMap[item1] = value1;
			actualMap[item2] = value2;

			Assert.AreEqual(2, actualMap.Count, "The IdentityMap should have 2 elements");

		}

		/// <summary>
		/// Verify the Keys returns the object passed as the key, not the
		/// IdentityKey that the object was converted to.
		/// </summary>
		[Test]
		public void Keys() 
		{

			IDictionary map = IdentityMap.Instantiate();
			map.Add(item1, value1);
			map.Add(item2, value2);

			Assert.AreEqual(expectedMap.Keys.Count, map.Keys.Count, "Same number of Keys");
			foreach(MutableHashCode key in map.Keys) 
			{
				Assert.IsTrue(expectedMap.Contains(key), "Expected to find " + key.HashCodeField);
			}
		}

		/// <summary>
		/// Whenever I run the test in the NUnit Gui two times it throws an error because 
		/// it can't find the method System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(object).
		/// I have isolated it to not be a problem with IdentityMap.IdentityKey and need to figure out if
		/// I have misconfigured NUnit on my machine or if NUnit is falling back to the .NET 1.0 Framework.
		/// The only reason I think it might be falling back is because that method was added in the .NET 1.1 
		/// Framework.
		/// </summary>
		[Test]
		[Ignore("I just wanted to verify that an Exception would occur without the NHibernate related code.  " + 
			 "The Exception only occurs the 2nd Time the Test is run in the same NUnit session.")]
		public void MethodMissingException() 
		{
			System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(new object());
		}

		/// <summary>
		/// Provides an object whose HashCode is based on a Mutable field.  Not a good
		/// practice but perfect for testing IdentityMap because it simulates an object
		/// being loaded with its default constructor (no params) and then having the 
		/// fields initialized.  If the class overrides GetHashCode() then it will be inconsistent
		/// between the construction and field population by NHibernate.
		/// </summary>
		protected class MutableHashCode 
		{
			private int hashCodeField;

			public MutableHashCode() 
			{
			}

			public MutableHashCode(int hashCodeField) 
			{
				this.hashCodeField = hashCodeField;
			}

			public int HashCodeField 
			{
				get { return hashCodeField;}
				set { hashCodeField = value;}
			}

			public override int GetHashCode()
			{
				return hashCodeField.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				// I am not putting all of the proper comparisons in here
				// because this is just simple test code.
				return hashCodeField.Equals(((MutableHashCode)obj).HashCodeField);
			}

		}
		
	}

	
	
}
