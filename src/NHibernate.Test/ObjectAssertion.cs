using System;
using System.Collections;

using NUnit.Framework;
using NHibernate.DomainModel;

namespace NHibernate.Test
{
	/// <summary>
	/// Helps with Assertions with two Objects.
	/// </summary>
	[TestFixture]
	public class ObjectAssertion
	{
		public ObjectAssertion()
		{
		}

		internal static void AssertPropertiesEqual(JoinedSubclassBase expected, JoinedSubclassBase actual) 
		{
			Assertion.AssertEquals(expected.Id, actual.Id);
			Assertion.AssertEquals(expected.TestDateTime, actual.TestDateTime);
			Assertion.AssertEquals(expected.TestLong, actual.TestLong);
			Assertion.AssertEquals(expected.TestString, actual.TestString);
		}

		internal static void AssertPropertiesEqual(JoinedSubclassOne expected, JoinedSubclassOne actual) 
		{
			ObjectAssertion.AssertPropertiesEqual((JoinedSubclassBase)expected, (JoinedSubclassBase)actual);
			Assertion.AssertEquals(expected.OneTestLong, actual.OneTestLong);
		}

		/// <summary>
		/// Compares one dimensional arrays for equality.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <remarks>The objects contained in the arrays must implement Equals correctly.</remarks>
		internal static void AssertEquals(IList expected, IList actual) 
		{

			Assertion.AssertEquals(expected.Count, actual.Count);

			for(int i = 0; i < expected.Count; i++) 
			{
				Assertion.AssertEquals("The item at index " + i + " was not equal", expected[i], actual[i]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="compareValues">Set it to false when you only care about the keys, specifically with Sets.</param>
		internal static void AssertEquals(IDictionary expected, IDictionary actual, bool compareValues) 
		{
			Assertion.AssertEquals(expected.Count, actual.Count);

			foreach(DictionaryEntry de in expected) 
			{
				Assertion.Assert(actual.Contains(de.Key));
				if(compareValues)
					Assertion.AssertEquals("The item identified by the key " + de.Key.ToString(), expected[de.Key], actual[de.Key]);	
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

			ObjectAssertion.AssertEquals(expected, actualWithEqualValues, true);

		}

		public static void AssertEquals(DateTime expected, DateTime actual, bool useMilliseconds) 
		{
			Assertion.AssertEquals("Year", expected.Year, actual.Year);
			Assertion.AssertEquals("Month", expected.Month, actual.Month);
			Assertion.AssertEquals("Day", expected.Day, actual.Day);
			Assertion.AssertEquals("Hour", expected.Hour, actual.Hour);
			Assertion.AssertEquals("Minute", expected.Minute, actual.Minute);
			Assertion.AssertEquals("Second", expected.Second, actual.Second);
			if(useMilliseconds)
				Assertion.AssertEquals("Millisecond", expected.Millisecond, actual.Millisecond);

		}

		
	}
}
