using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.JCollections;


namespace NHibernate.UnitTesting
{
	[TestFixture]
	public class HashSetUnitTesting {
		HashSet fruits1;
		HashSet fruits2;

		[SetUp]
		public void Init() {
			fruits1 = new HashSet();
			fruits1.Add("apple");
			fruits1.Add("orange");
			fruits1.Add("orange");
			fruits1.Add("apple");
			fruits1.Add("banana");

			fruits2 = new HashSet();
			fruits2.Add("banana");
			fruits2.Add("naranjilla");
		}

		[Test]
		public void Simple() {
			Assertion.AssertEquals("3 elements: apple, orange, banana", 3, fruits1.Count);
			foreach (string s in fruits1.ToArray(typeof(string))) {
				if (s != "apple" && s != "orange" && s != "banana")
					Assertion.Fail("Something different to apple and orange in the hashSet");
			}
		}

		[Test]
		public void Intersection() {
			fruits1.RetainAll(fruits2);
			Assertion.AssertEquals("1 element: banana", 1, fruits1.Count);
			IEnumerator enumerator = fruits1.GetEnumerator();
			enumerator.MoveNext();
			Assertion.AssertEquals("The intersection is a banana", "banana", enumerator.Current);
			Assertion.AssertEquals("Should be only one item", false, enumerator.MoveNext());
		}

		[Test]
		public void Union() {
			fruits1.AddAll(fruits2);
			Assertion.AssertEquals("Union has 4 elements", 4, fruits1.Count);
			foreach (string s in fruits1.ToArray(typeof(string))) {
				if (s != "apple" && s != "orange" && s != "banana" && s != "naranjilla")
					Assertion.Fail("Something different to apple and orange in the hashSet");
			}
		}

		[Test]
		public void Comparison() {
			HashSet myFruits = new HashSet(new String[]{"apple", "banana", "orange"});
			Assertion.AssertEquals("Order shouldn't matter in comparison", true, fruits1.ContainsAll(myFruits));
			Assertion.AssertEquals("Order shouldn't matter in comparison", true, myFruits.ContainsAll(fruits1));
			Assertion.AssertEquals("Order shouldn't matter in comparison", false, fruits1.ContainsAll(fruits2));
		}
			
		[Test]
		public void AssymetricDifference() {
			fruits1.RemoveAll(fruits2);
			Assertion.AssertEquals("2 elements: orange, apple", 2, fruits1.Count);
			foreach (string s in fruits1.ToArray(typeof(string))) {
				if (s != "apple" && s != "orange")
					Assertion.Fail("Something different to apple and orange in the hashSet");
			}
		}

		[Test]
		public void NullsInSet() {
			HashSet nullHere = new HashSet(fruits1);
			nullHere.Add(null);
			Assertion.AssertEquals("null is part of the set", fruits1.Count + 1, nullHere.Count);
			nullHere.Add(null);
			Assertion.AssertEquals("null is added just once", fruits1.Count + 1, nullHere.Count);
			nullHere.RemoveAll(fruits1);
			Assertion.AssertEquals("only null left", 1, nullHere.Count);
			foreach (object o in nullHere)
				Assertion.AssertNull("only null left", o);
			nullHere.Remove(null);
			Assertion.AssertEquals("nothing left", 0, nullHere.Count);
			nullHere.Remove(null);
			Assertion.AssertEquals("nothing left", 0, nullHere.Count);
		}

		[Test]
		public void EmptySets() {
			// a set with one number
			HashSet emptySet = new HashSet(new int[]{101});
			// an empty set
			emptySet.Remove(102 - 1);
			Assertion.AssertEquals("nothing left", 0, emptySet.Count);
			// a set with three numbers
			HashSet someNumbers = new HashSet(new int[]{10, 20, 30, 20});
			Assertion.AssertEquals("3 numbers", 3, someNumbers.Count);
			someNumbers.Remove(null);
			Assertion.AssertEquals("3 numbers", 3, someNumbers.Count);
			someNumbers.RemoveAll(emptySet);
			Assertion.AssertEquals("3 numbers", 3, someNumbers.Count);
			someNumbers.AddAll(emptySet);
			Assertion.AssertEquals("3 numbers", 3, someNumbers.Count);
			emptySet.Remove(100);
			Assertion.AssertEquals("0 numbers", 0, emptySet.Count);
			emptySet.RemoveAll(someNumbers);
			Assertion.AssertEquals("0 numbers", 0, emptySet.Count);
		}

		[Test]
		public void CopyingToArrays() {
			string[] stringArray = new string[fruits1.Count];
			fruits1.CopyTo(stringArray, 0);
			for (int i = 0; i < stringArray.Length; i++)
				if (stringArray[i] != "apple" && stringArray[i] != "orange" && stringArray[i] != "banana")
					Assertion.Fail("Something different to apple and orange in the copied array");

			string[] stringArray2 = (string[])fruits1.ToArray(typeof(string));
			for (int i = 0; i < stringArray2.Length; i++)
				if (stringArray2[i] != "apple" && stringArray2[i] != "orange" && stringArray2[i] != "banana")
					Assertion.Fail("Something different to apple and orange in the copied array");

			object[] objectArray = fruits1.ToArray();
			for (int i = 0; i < objectArray.Length; i++) {
				string s = (string)objectArray[i];
				if (s != "apple" && s != "orange" && s != "banana")
					Assertion.Fail("Something different to apple and orange in the copied array");
			}
		}

	}
}
