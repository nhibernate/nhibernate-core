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

		protected virtual IDictionary GetIdentityMap() 
		{
			return IdentityMap.Instantiate( 10 );
		}

		/// <summary>
		/// Verify that the object being added as the Key does not have it's GetHashCode
		/// method called.
		/// </summary>
		[Test]
		public void AddNoHashCode()
		{
			IDictionary map = GetIdentityMap();
			map.Add(noHashCode1, value1);

			Assert.AreEqual(1, map.Count, "The item was added succesfully");
		}

		/// <summary>
		/// Verify that ConcurrentEntities returns an ICollection that contains the same
		/// Keys/Values as originally added into the IdentityMap.
		/// </summary>
		[Test]
		public void ConcurrentEntries() 
		{
			IDictionary map = GetIdentityMap();
			
			map.Add(noHashCode1, value1);
			map.Add(noHashCode2, value2);

			// call ConcurrentEntries and verify it doesn't use the HashCode to build the 
			// new list.
			ICollection concurrent = IdentityMap.ConcurrentEntries(map);

			Assert.AreEqual(2, concurrent.Count, "There are two elements in concurrent Map");
			foreach(DictionaryEntry de in concurrent) 
			{
				NoHashCode noCode = (NoHashCode)de.Key;
				object noCodeValue = de.Value;

				Assert.IsTrue(map.Contains(noCode), "The Key in the concurrent map should have been in the original map's Keys");
				Assert.IsTrue(noCodeValue==map[noCode], "The Value identified by the Key in concurrent map should be the same as the IdentityMap");
			}

		}

		/// <summary>
		/// Tests that it is safe to modify the IdentityMap while iterating through the
		/// ConcurrentEntities.
		/// </summary>
		[Test]
		public void ConcurrentEntitiesModification()
		{
			NoHashCode noHashCode3 = new NoHashCode();
			object value3 = new object();

			NoHashCode noHashCode4 = new NoHashCode();
			object value4 = new object();

			IDictionary map = GetIdentityMap();
			map.Add(noHashCode1, value1);
			map.Add(noHashCode2, value2);

			ICollection concurrent = IdentityMap.ConcurrentEntries(map);

			for( int i = 0; i < concurrent.Count; )
			{
				if(i==0) map.Add(noHashCode3, value3);
				if(i==1) map.Add(noHashCode4, value4);

				i++;
				Assert.AreEqual(2, concurrent.Count, "Should still be 2 items in the concurrent ICollection");
				Assert.AreEqual(2 + i, map.Count, "Should be " + (2 + i) + " items in the IdentityMap"); 
			}

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
			IDictionary map = GetIdentityMap();
			int intKey = 3;
			object objectValue = new object();
			map.Add(intKey, objectValue); 

		}

		[Test]
		public void Count() 
		{
			IDictionary map = GetIdentityMap();
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
			IDictionary map = GetIdentityMap();

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
			IDictionary map = GetIdentityMap();

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
			IDictionary map = GetIdentityMap();
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
			IDictionary actualMap = GetIdentityMap();
			
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
			IDictionary actualMap = GetIdentityMap();
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

			IDictionary map = GetIdentityMap();
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
		/// <remarks>
		/// This is actually a problem with NUnit settings.  To resolve this go to Tools-Options and make
		/// sure that Reload before each test run is NOT checked.
		/// </remarks>
		//[Test]
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
		[Serializable]
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

		/// <summary>
		/// The IdentityMap should not ever call the GetHashCode() because that
		/// will have side effects on Collections/Entities.
		/// </summary>
		[Serializable]
		protected class NoHashCode 
		{
			public override int GetHashCode()
			{
				throw new NotImplementedException("This method should not get called during test");
			}

			public override bool Equals(object obj) 
			{
				return base.Equals(obj);
			}
		}
		
	}

	
	
}
