using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;

namespace NHibernate.Test
{
	/// <summary>
	/// Helps with Asserts with two Objects.
	/// </summary>
	[TestFixture]
	public class ObjectAssert
	{
		public ObjectAssert()
		{
		}

		internal static void AreEqual(Iesi.Collections.ISet expected, Iesi.Collections.ISet actual) 
		{
			Assert.AreEqual( expected.Count, actual.Count, "two sets are diff size" );
			foreach( object obj in expected ) 
			{
				Assert.IsTrue( actual.Contains( obj ), obj.ToString() + " is not contained in actual" );
			}
		}

		/// <summary>
		/// Compares one dimensional arrays for equality.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <remarks>The objects contained in the arrays must implement Equals correctly.</remarks>
		internal static void AreEqual(IList expected, IList actual) 
		{

			AreEqual(expected, actual, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="indexMatters">A boolean indicating if the List are compared at Index or by Contains.</param>
		internal static void AreEqual(IList expected, IList actual, bool indexMatters) 
		{
			Assert.AreEqual(expected.Count, actual.Count);
			for(int i = 0; i < expected.Count; i++) 
			{
				if(indexMatters) 
				{
					Assert.IsTrue(expected[i].Equals(actual[i]), "The item at index " + i + " was not equal" );
				}
				else 
				{
					Assert.IsTrue(actual.Contains(expected[i]), "The item " + expected[i].ToString() + " could not be found in the actual List.");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="compareValues">Set it to false when you only care about the keys, specifically with Sets.</param>
		internal static void AreEqual(IDictionary expected, IDictionary actual, bool compareValues) 
		{
			Assert.AreEqual(expected.Count, actual.Count);

			foreach(DictionaryEntry de in expected) 
			{
				Assert.IsTrue(actual.Contains(de.Key));
				if(compareValues)
					Assert.AreEqual(expected[de.Key], actual[de.Key], "The item identified by the key " + de.Key.ToString());
			}
		}

		[Test]
		public void TestIDictionaryEqual() 
		{
			IDictionary expected = new Hashtable(2);
			IDictionary actualWithEqualValues = new Hashtable(2);

			expected["ZERO"] = "zero";
			expected["ONE"] = "one";

			actualWithEqualValues["ZERO"] = "zero";
			actualWithEqualValues["ONE"] = "one";

			ObjectAssert.AreEqual(expected, actualWithEqualValues, true);

		}

		public static void AreEqual(DateTime expected, DateTime actual, bool useMilliseconds) 
		{
			Assert.AreEqual(expected.Year, actual.Year, "Year");
			Assert.AreEqual(expected.Month, actual.Month, "Month");
			Assert.AreEqual(expected.Day, actual.Day, "Day");
			Assert.AreEqual(expected.Hour, actual.Hour, "Hour");
			Assert.AreEqual(expected.Minute, actual.Minute, "Minute");
			Assert.AreEqual(expected.Second, actual.Second, "Second");
			if(useMilliseconds)
				Assert.AreEqual(expected.Millisecond, actual.Millisecond, "Millisecond");

		}

		
	}
}
