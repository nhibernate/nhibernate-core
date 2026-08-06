using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest;

/// <summary>
/// Tests for the generic <see cref="IdentityMap{TKey,TValue}"/>, mirroring
/// <see cref="IdentityMapFixture"/>.
/// </summary>
[TestFixture]
public class IdentityMapGenericFixture
{
	protected MutableHashCode item1;
	protected MutableHashCode item2;

	protected NoHashCode noHashCode1;
	protected NoHashCode noHashCode2;

	protected object value1;
	protected object value2;

	[SetUp]
	public void SetUp()
	{
		item1 = new MutableHashCode(1);
		item2 = new MutableHashCode(2);

		value1 = new object();
		value2 = new object();

		noHashCode1 = new NoHashCode();
		noHashCode2 = new NoHashCode();
	}

	protected virtual IDictionary<object, object> GetIdentityMap() => IdentityMapUtils.Instantiate<object, object>(10);

	/// <summary>
	/// Verify that the object being added as the Key does not have its GetHashCode
	/// method called.
	/// </summary>
	[Test]
	public void AddNoHashCode()
	{
		var map = GetIdentityMap();
		map.Add(noHashCode1, value1);

		Assert.AreEqual(1, map.Count, "The item was added succesfully");
	}

	/// <summary>
	/// An IdentityMap can not use a ValueType as the Key because of the boxing/unboxing
	/// that occurs with them. This verifies that an Exception is thrown if a ValueType
	/// is used as the key.
	/// </summary>
	[Test]
	public void AddValueTypeException()
	{
		var map = GetIdentityMap();
		object intKey = 3;
		object objectValue = new object();
		Assert.Throws<ArgumentException>(() => map.Add(intKey, objectValue));
	}

	[Test]
	public void Count()
	{
		var map = GetIdentityMap();
		map.Add(new object(), new object());
		map.Add(new object(), new object());

		Assert.AreEqual(2, map.Count, "Expect 2 items in the IdentityMap");
	}

	/// <summary>
	/// Test that two different references to the same object passed to ContainsKey
	/// both return true.
	/// </summary>
	[Test]
	public void ContainsSameObjectByRef()
	{
		var map = GetIdentityMap();

		var item1Copy = item1;

		map.Add(item1, new object());

		Assert.AreSame(item1, item1Copy);
		Assert.IsTrue(map.ContainsKey(item1Copy), "We should be able to get the same object out of the IdentityMap with " +
		                                          "two different references to the same object.");
	}

	/// <summary>
	/// Test that even though the HashCode and Equals of the same reference have been changed
	/// that ContainsKey still recognizes it by the Identity of the object - not the values.
	/// </summary>
	[Test]
	public void ContainsSameObjectWithDiffEquals()
	{
		var map = GetIdentityMap();

		map.Add(item1, new object());

		item1.HashCodeField = 5;

		Assert.IsTrue(map.ContainsKey(item1),
		              "Even though item1's HashCode field change the IdentityMap.ContainsKey() should still return true");
	}

	/// <summary>
	/// Test to make sure that two objects that are equal by the Equals definition of the class MutableHashCode
	/// do not get translated to the same key because they are different objects.
	/// </summary>
	[Test]
	public void ContainsDiffObjectWithEquals()
	{
		var map = GetIdentityMap();
		item1.HashCodeField = 4;
		item2.HashCodeField = 4;

		map.Add(item1, new object());

		Assert.AreEqual(item1, item2, "They should be equal.");
		Assert.IsFalse(map.ContainsKey(item2), "Even though item1.Equals(item2) IdentityMap should not find by item2");
	}

	/// <summary>
	/// Add the same MutableHashCode class twice and ensure there is only
	/// one item in the IdentityMap.
	/// </summary>
	[Test]
	public void SetItemChangedHashCodeTwice()
	{
		var actualMap = GetIdentityMap();

		actualMap[item1] = value1;

		// change the Property that GetHashCode method uses
		item1.HashCodeField = 2;
		actualMap[item1] = value1;
		Assert.AreEqual(1, actualMap.Count, "Should only be 1 item in the IdentityMap");
	}

	/// <summary>
	/// Adds two different objects that are Equal() to each other to verify that
	/// it does not use the objects Equal() but instead the IdentityMap's reference identity.
	/// </summary>
	[Test]
	public void SetItemsEqualHashCodeDiffIdentity()
	{
		var actualMap = GetIdentityMap();

		item1.HashCodeField = 3;
		item2.HashCodeField = 3;

		Assert.AreEqual(item1, item2, "The two objects are equal");
		Assert.IsTrue(item1 != item2, "The two items are different objects in memory");

		actualMap[item1] = value1;
		actualMap[item2] = value2;

		Assert.AreEqual(2, actualMap.Count, "The IdentityMap should have 2 elements");
	}

	/// <summary>
	/// Verify the Keys returns the object passed as the key, not some transformed identity key.
	/// </summary>
	[Test]
	public void Keys()
	{
		var map = GetIdentityMap();
		map.Add(item1, value1);
		map.Add(item2, value2);

		Assert.AreEqual(2, map.Keys.Count, "Same number of Keys");
		CollectionAssert.Contains(map.Keys, item1);
		CollectionAssert.Contains(map.Keys, item2);
	}

	/// <summary>
	/// Verify that GetEntries returns a snapshot that contains the same
	/// Keys/Values as originally added into the IdentityMap.
	/// </summary>
	[Test]
	public void GetEntries()
	{
		var map = GetIdentityMap();

		map.Add(noHashCode1, value1);
		map.Add(noHashCode2, value2);

		var snapshot = IdentityMapUtils.GetSnapshot(map);

		Assert.AreEqual(2, snapshot.Count, "There are two elements in the snapshot");
		foreach (var de in snapshot)
		{
			Assert.IsTrue(map.ContainsKey(de.Key), "The Key in the snapshot should have been in the original map's Keys");
			Assert.IsTrue(de.Value == map[de.Key],
			              "The Value identified by the Key in the snapshot should be the same as the IdentityMap");
		}
	}

	/// <summary>
	/// Tests that it is safe to modify the IdentityMap while iterating through GetEntries,
	/// and that the snapshot's contents stay fixed even as the underlying map keeps changing
	/// (i.e. it does not observe writes that occur after it was created, whether those writes
	/// happen before or after the snapshot is frozen).
	/// </summary>
	[Test]
	public void GetEntriesModification()
	{
		var noHashCode3 = new NoHashCode();
		var value3 = new object();

		var noHashCode4 = new NoHashCode();
		var value4 = new object();

		var map = GetIdentityMap();
		map.Add(noHashCode1, value1);
		map.Add(noHashCode2, value2);

		var snapshot = IdentityMapUtils.GetSnapshot(map);

		for (var i = 0; i < 2; i++)
		{
			if (i == 0) map.Add(noHashCode3, value3);
			if (i == 1) map.Add(noHashCode4, value4);

			Assert.AreEqual(2, snapshot.Count, "Snapshot should still have 2 items even after the map is modified");
			Assert.AreEqual(2 + i + 1, map.Count, "Should be " + (2 + i + 1) + " items in the IdentityMap");
		}

		CollectionAssert.AreEquivalent(
			new object[] { noHashCode1, noHashCode2 },
			snapshot.KeyList().ToList(),
			"Snapshot keys should be exactly the ones present when it was created");
	}
}
