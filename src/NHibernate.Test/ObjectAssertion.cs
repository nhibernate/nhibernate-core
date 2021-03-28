using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

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

		internal static void AreEqual<T>(ISet<T> expected, ISet<T> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count, "two sets are diff size");
			foreach (T obj in expected)
			{
				Assert.IsTrue(actual.Contains(obj), obj.ToString() + " is not contained in actual");
			}
		}

		/// <summary>
		/// Compares one dimensional arrays for equality.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <remarks>The objects contained in the arrays must implement Equals correctly.</remarks>
		internal static void AreEqual<T>(IList<T> expected, IList<T> actual)
		{
			AreEqual(expected, actual, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="indexMatters">A boolean indicating if the List are compared at Index or by Contains.</param>
		internal static void AreEqual<T>(IList<T> expected, IList<T> actual, bool indexMatters)
		{
			Assert.AreEqual(expected.Count, actual.Count, "list lengths differ");
			for (int i = 0; i < expected.Count; i++)
			{
				if (indexMatters)
				{
					Assert.IsTrue(expected[i].Equals(actual[i]), "The item at index " + i + " was not equal");
				}
				else
				{
					Assert.IsTrue(actual.Contains(expected[i]),
					              "The item " + expected[i].ToString() + " could not be found in the actual List.");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		/// <param name="compareValues">Set it to false when you only care about the keys, specifically with Sets.</param>
		internal static void AreEqual<TKey, TItem>(IDictionary<TKey,TItem> expected, IDictionary<TKey,TItem> actual, bool compareValues)
		{
			Assert.AreEqual(expected.Count, actual.Count);

			foreach (KeyValuePair<TKey, TItem> de in expected)
			{
				Assert.IsTrue(actual.ContainsKey(de.Key));
				if (compareValues)
					Assert.AreEqual(expected[de.Key], actual[de.Key], "The item identified by the key " + de.Key.ToString());
			}
		}

		[Test]
		public void TestIDictionaryEqual()
		{
			IDictionary<string, string> expected = new Dictionary<string, string>();
			IDictionary<string, string> actualWithEqualValues = new Dictionary<string, string>();

			expected["ZERO"] = "zero";
			expected["ONE"] = "one";

			actualWithEqualValues["ZERO"] = "zero";
			actualWithEqualValues["ONE"] = "one";

			AreEqual(expected, actualWithEqualValues, true);
		}

		public static void AreEqual(DateTime expected, DateTime actual, bool useMilliseconds)
		{
			Assert.AreEqual(expected.Year, actual.Year, "Year");
			Assert.AreEqual(expected.Month, actual.Month, "Month");
			Assert.AreEqual(expected.Day, actual.Day, "Day");
			Assert.AreEqual(expected.Hour, actual.Hour, "Hour");
			Assert.AreEqual(expected.Minute, actual.Minute, "Minute");
			Assert.AreEqual(expected.Second, actual.Second, "Second");
			if (useMilliseconds)
				Assert.AreEqual(expected.Millisecond, actual.Millisecond, "Millisecond");
		}
	}
}
