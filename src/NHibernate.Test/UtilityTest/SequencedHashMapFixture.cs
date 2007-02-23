using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for SequencedHashMapFixture.
	/// </summary>
	[TestFixture]
	public class SequencedHashMapFixture
	{
		private SequencedHashMap _shm;
		private SequencedHashMap _emptyShm;
		private IList _expectedKeys;
		private IList _expectedValues;

		[SetUp]
		public void SetUp()
		{
			_shm = new SequencedHashMap();
			_emptyShm = new SequencedHashMap();

			_expectedKeys = new ArrayList();
			_expectedKeys.Add("test1");
			_expectedKeys.Add("test2");
			_expectedKeys.Add("test3");

			_expectedValues = new ArrayList();
			_expectedValues.Add(1);
			_expectedValues.Add("2");
			_expectedValues.Add(true);

			for (int i = 0; i < _expectedKeys.Count; i++)
			{
				_shm[_expectedKeys[i]] = _expectedValues[i];
			}
		}

		[Test]
		public void Add()
		{
			object newKey = "test4";
			object newValue = "test4's value";

			_expectedKeys.Add(newKey);
			_expectedValues.Add(newValue);

			_shm.Add(newKey, newValue);


			int i = 0;
			foreach (DictionaryEntry de in _shm)
			{
				Assert.AreEqual(_expectedKeys[i], de.Key);
				Assert.AreEqual(_expectedValues[i], de.Value);
				i++;
			}

			Assert.AreEqual(4, i, "Did not loop through 4 items");
			Assert.AreEqual(4, _shm.Count);
		}

		[Test]
		public void Clear()
		{
			_shm.Clear();

			Assert.AreEqual(0, _shm.Count);

			foreach (DictionaryEntry de in _shm)
			{
				Assert.Fail("Should not be any entries but found Key = " + de.Key.ToString() + " and Value = " + de.Value.ToString());
			}
		}

		[Test]
		public void Contains()
		{
			Assert.IsTrue(_shm.Contains("test1"));
			Assert.IsFalse(_shm.Contains("test10"));
		}

		[Test]
		public void GetEnumerator()
		{
			int i = 0;

			foreach (DictionaryEntry de in _shm)
			{
				Assert.AreEqual(_expectedKeys[i], de.Key);
				Assert.AreEqual(_expectedValues[i], de.Value);
				i++;
			}

			Assert.AreEqual(3, i);
		}

		[Test]
		public void GetEnumeratorEmpty()
		{
			bool noEntries = true;

			for (int i = 0; i < _emptyShm.Count; i++)
			{
				noEntries = false;
			}

			Assert.IsTrue(noEntries, "should not have any entries in the enumerator");
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetEnumeratorModifyExceptionFromAdd()
		{
			foreach (DictionaryEntry de in _shm)
			{
				_shm["newkey"] = de.Value;
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetEnumeratorModifyExceptionFromRemove()
		{
			foreach (DictionaryEntry de in _shm)
			{
				_shm.Remove(de.Key);
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetEnumeratorModifyExceptionFromUpdate()
		{
			foreach (DictionaryEntry de in _shm)
			{
				_shm[de.Key] = new object();
			}
		}

		[Test]
		public void Remove()
		{
			// remove an item that exists
			_shm.Remove("test1");
			Assert.AreEqual(2, _shm.Count);

			// try to remove an item that does not exist
			_shm.Remove("test10");
			Assert.AreEqual(2, _shm.Count);
		}

		[Test]
		public void Item()
		{
			Assert.AreEqual(1, _shm["test1"]);
			Assert.AreEqual("2", _shm["test2"]);
			Assert.AreEqual(true, _shm["test3"]);
		}

		[Test]
		public void Count()
		{
			Assert.AreEqual(3, _shm.Count);
			_shm.Add("new key", "new value");
			Assert.AreEqual(4, _shm.Count);
		}

		[Test]
		public void ContainsKey()
		{
			Assert.IsTrue(_shm.ContainsKey("test1"));
			Assert.IsFalse(_shm.ContainsKey("test10"));
		}

		[Test]
		public void ContainsValue()
		{
			Assert.IsTrue(_shm.ContainsValue("2"));
			Assert.IsTrue(_shm.ContainsValue(true));
			Assert.IsFalse(_shm.ContainsValue("not in there"));
		}

		[Test]
		public void CopyTo()
		{
			DictionaryEntry[] destArray = new DictionaryEntry[3];
			_shm.CopyTo(destArray, 0);

			Assert.AreEqual(3, destArray.Length);

			for (int i = 0; i < destArray.Length; i++)
			{
				Assert.AreEqual(_expectedKeys[i], destArray[i].Key);
				Assert.AreEqual(_expectedValues[i], destArray[i].Value);
			}
		}

		[Test]
		public void Keys()
		{
			int i = 0;
			foreach (object obj in _shm.Keys)
			{
				i++;
				Assert.IsTrue(_expectedKeys.Contains(obj));
			}

			Assert.AreEqual(3, i);

			SequencedHashMap empty = new SequencedHashMap();
			foreach (object obj in empty.Keys)
			{
				Assert.Fail("should not be a key: " + obj);
			}
		}

		[Test]
		public void Values()
		{
			int i = 0;
			foreach (object obj in _shm.Values)
			{
				i++;
				Assert.IsTrue(_expectedValues.Contains(obj));
			}

			Assert.AreEqual(3, i);

			SequencedHashMap empty = new SequencedHashMap();
			foreach (object obj in empty.Values)
			{
				Assert.Fail("should not be a value:" + obj);
			}
		}

		[Test]
		public void ValuesEmpty()
		{
			bool noValues = true;

			for (int i = 0; i < _emptyShm.Values.Count; i++)
			{
				noValues = false;
			}

			Assert.IsTrue(noValues, "should have no values.");
		}

		[Test]
		public void FirstKey()
		{
			Assert.IsNotNull(_shm.FirstKey);
			Assert.AreEqual("test1", _shm.FirstKey);
			Assert.IsNull(_emptyShm.FirstKey);
		}

		[Test]
		public void FirstValue()
		{
			Assert.IsNotNull(_shm.FirstValue);
			Assert.AreEqual(1, _shm.FirstValue);
			Assert.IsNull(_emptyShm.FirstValue);
		}

		[Test]
		public void LastKey()
		{
			Assert.IsNotNull(_shm.LastKey);
			Assert.AreEqual("test3", _shm.LastKey);
			Assert.IsNull(_emptyShm.LastKey);
		}

		[Test]
		public void LastValue()
		{
			Assert.IsNotNull(_shm.LastValue);
			Assert.AreEqual(true, _shm.LastValue);
			Assert.IsNull(_emptyShm.LastValue);
		}


		//[Test]
		// User should uncomment if they want to see the Performance Comparison
		public void Performance()
		{
			// set the hashtable and SequencedHashMap to be the 
			IDictionary hashtable;
			IDictionary sequenced;
			IDictionary list;

			int numOfRuns = 1;

			int numOfEntries = Int16.MaxValue;

			long hashStart;
			long[] hashPopulateTicks = new long[numOfRuns];
			long[] hashItemTicks = new long[numOfRuns];

			long seqStart;
			long[] seqPopulateTicks = new long[numOfRuns];
			long[] seqItemTicks = new long[numOfRuns];

			long listStart;
			long[] listPopulateTicks = new long[numOfRuns];
			long[] listItemTicks = new long[numOfRuns];

			for (int runIndex = 0; runIndex < numOfRuns; runIndex++)
			{
				object key;
				object value;
				hashtable = new Hashtable();
				sequenced = new SequencedHashMap();
				list = new ListDictionary();

				hashStart = DateTime.Now.Ticks;

				for (int i = 0; i < numOfEntries; i++)
				{
					hashtable.Add("test" + i, new object());
				}

				hashPopulateTicks[runIndex] = DateTime.Now.Ticks - hashStart;

				hashStart = DateTime.Now.Ticks;
				for (int i = 0; i < numOfEntries; i++)
				{
					key = "test" + i;
					value = hashtable[key];
				}

				hashItemTicks[runIndex] = DateTime.Now.Ticks - hashStart;

				hashtable.Clear();

				seqStart = DateTime.Now.Ticks;

				for (int i = 0; i < numOfEntries; i++)
				{
					sequenced.Add("test" + i, new object());
				}

				seqPopulateTicks[runIndex] = DateTime.Now.Ticks - seqStart;

				seqStart = DateTime.Now.Ticks;
				for (int i = 0; i < numOfEntries; i++)
				{
					key = "test" + i;
					value = sequenced[key];
				}

				seqItemTicks[runIndex] = DateTime.Now.Ticks - seqStart;

				sequenced.Clear();

				listStart = DateTime.Now.Ticks;

				for (int i = 0; i < numOfEntries; i++)
				{
					list.Add("test" + i, new object());
				}

				listPopulateTicks[runIndex] = DateTime.Now.Ticks - listStart;

				listStart = DateTime.Now.Ticks;
				for (int i = 0; i < numOfEntries; i++)
				{
					key = "test" + i;
					value = list[key];
				}

				listItemTicks[runIndex] = DateTime.Now.Ticks - listStart;


				list.Clear();
			}

			for (int runIndex = 0; runIndex < numOfRuns; runIndex++)
			{
				decimal seqPopulateOverhead = ((decimal) seqPopulateTicks[runIndex] / (decimal) hashPopulateTicks[runIndex]);
				decimal seqItemOverhead = ((decimal) seqItemTicks[runIndex] / (decimal) hashItemTicks[runIndex]);

				string errMessage = "SequenceHashMap vs Hashtable:";
				errMessage += "\n POPULATE:";
				errMessage += "\n\t seqPopulateTicks[" + runIndex + "] took " + seqPopulateTicks[runIndex] + " ticks.";
				errMessage += "\n\t hashPopulateTicks[" + runIndex + "] took " + hashPopulateTicks[runIndex] + " ticks.";
				errMessage += "\n\t for an overhead of " + seqPopulateOverhead.ToString();
				errMessage += "\n ITEM:";
				errMessage += "\n\t seqItemTicks[" + runIndex + "] took " + seqItemTicks[runIndex] + " ticks.";
				errMessage += "\n\t hashItemTicks[" + runIndex + "] took " + hashItemTicks[runIndex] + " ticks.";
				errMessage += "\n\t for an overhead of " + seqItemOverhead.ToString();

				Console.Out.WriteLine(errMessage);

				decimal listPopulateOverhead = ((decimal) listPopulateTicks[runIndex] / (decimal) seqPopulateTicks[runIndex]);
				decimal listItemOverhead = ((decimal) listItemTicks[runIndex] / (decimal) seqItemTicks[runIndex]);

				errMessage = "ListDictionary vs SequenceHashMap:";
				errMessage += "\n POPULATE:";
				errMessage += "\n\t listPopulateTicks[" + runIndex + "] took " + listPopulateTicks[runIndex] + " ticks.";
				errMessage += "\n\t seqPopulateTicks[" + runIndex + "] took " + seqPopulateTicks[runIndex] + " ticks.";
				errMessage += "\n\t for an overhead of " + listPopulateOverhead.ToString();
				errMessage += "\n ITEM:";
				errMessage += "\n\t listItemTicks[" + runIndex + "] took " + listItemTicks[runIndex] + " ticks.";
				errMessage += "\n\t seqItemTicks[" + runIndex + "] took " + seqItemTicks[runIndex] + " ticks.";
				errMessage += "\n\t for an overhead of " + listItemOverhead.ToString();

				Console.Out.WriteLine(errMessage);
			}
		}

		[Test]
		public void Serialize()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter f = new BinaryFormatter();
			f.Serialize(stream, _shm);
			stream.Position = 0;

			SequencedHashMap shm = (SequencedHashMap) f.Deserialize(stream);
			stream.Close();

			Assert.AreEqual(3, shm.Count);
			int index = 0;
			foreach (DictionaryEntry de in shm)
			{
				Assert.AreEqual(_expectedKeys[index], de.Key);
				Assert.AreEqual(_expectedValues[index], de.Value);
				index++;
			}

			Assert.AreEqual(3, index);
		}
	}
}