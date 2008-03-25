using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class LRUMapFixture
	{
		[Test]
		public void PutWithSizeLimit()
		{
			int size = 10;
			IDictionary cache = new LRUMap(size);

			for (int i = 0; i < size; i++)
			{
				cache.Add("key:" + i, "data:" + i);
			}

			for (int i = 0; i < size; i++)
			{
				string data = (string)cache["key:" + i];
				Assert.AreEqual("data:" + i, data, "Data is wrong.");
			}
		}

		[Test]
		public void PutWithNoSizeLimit()
		{
			int size = 10;
			IDictionary cache = new LRUMap();

			for (int i = 0; i < size; i++)
			{
				cache.Add("key:" + i, "data:" + i);
			}

			for (int i = 0; i < size; i++)
			{
				string data = (string)cache["key:" + i];
				Assert.AreEqual("data:" + i, data, "Data is wrong.");
			}
		}

		[Test]
		public void PutAndRemove()
		{
			int size = 10;
			IDictionary cache = new LRUMap(size);

			cache.Add("key:" + 1, "data:" + 1);
			cache.Remove("key:" + 1);
			Assert.IsNull(cache["key:" + 1]);
		}

		[Test]
		public void RemoveEmpty()
		{
			int size = 10;
			IDictionary cache = new LRUMap(size);

			cache.Remove("key:" + 1);
		}

		[Test]
		public void GetEntrySet()
		{
			int size = 10;
			IDictionary cache = new LRUMap(size);

			for (int i = 0; i < size; i++)
			{
				cache.Add("key:" + i, "data:" + i);
			}
			Assert.AreEqual(size, cache.Keys.Count, "Set contains the wrong number of items.");

			// check minimal correctness
			foreach (DictionaryEntry entry in cache)
			{
				Assert.AreNotEqual(-1, entry.Value.ToString().IndexOf("data:"));
			}
		}    
	}
}
