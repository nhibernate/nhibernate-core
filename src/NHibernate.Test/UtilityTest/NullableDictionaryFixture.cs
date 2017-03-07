using System.Collections.Generic;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Tests for NullableDictionary.
	/// </summary>
	[TestFixture]
	public class NullableDictionaryFixture
	{
		private NullableDictionary<string, string> nullableDictionary;

		private readonly string _itemKey = "key";
		private readonly string _itemValue = "value";

		private readonly string _nullItemKey = null;
		private readonly string _nullItemValue = "null value";

		[SetUp]
		public void SetUp()
		{
			nullableDictionary = new NullableDictionary<string, string>();
		}

		[Test]
		public void AddKeyValue()
		{
			//non-null key
			nullableDictionary.Add(_itemKey, _itemValue);

			Assert.AreEqual(1, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_itemKey], _itemValue);

			//null key
			nullableDictionary.Add(_nullItemKey, _nullItemValue);

			Assert.AreEqual(2, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_nullItemKey], _nullItemValue);
		}

		[Test]
		public void AddUsingIndexer()
		{
			//non-null key
			nullableDictionary[_itemKey] = _itemValue;

			Assert.AreEqual(1, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_itemKey], _itemValue);

			//null key
			nullableDictionary[_nullItemKey] = _nullItemValue;

			Assert.AreEqual(2, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_nullItemKey], _nullItemValue);
		}

		[Test]
		public void AddKeyValuePair()
		{
			//non-null key
			nullableDictionary.Add(new KeyValuePair<string, string>(_itemKey, _itemValue));

			Assert.AreEqual(1, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_itemKey], _itemValue);

			//null key
			nullableDictionary.Add(new KeyValuePair<string, string>(_nullItemKey, _nullItemValue));

			Assert.AreEqual(2, nullableDictionary.Count);
			Assert.AreEqual(nullableDictionary[_nullItemKey], _nullItemValue);
		}
	}
}