using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest;

/// <summary>
/// Runs every <see cref="IdentityMapGenericFixture"/> reference-identity test against the
/// sequenced backing store, and adds the ordering guarantees that only
/// <see cref="IdentityMapUtils.InstantiateSequenced"/> makes.
/// </summary>
[TestFixture]
public class IdentityMapGenericSequencedFixture : IdentityMapGenericFixture
{
	protected override IDictionary<object, object> GetIdentityMap() => IdentityMapUtils.InstantiateSequenced<object, object>(10);

	[Test]
	public void IteratesInInsertionOrder()
	{
		var map = GetIdentityMap();
		var keys = Enumerable.Range(0, 30).Select(_ => new object()).ToArray();
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		Assert.That(map.Select(kv => kv.Key), Is.EqualTo(keys));
		Assert.That(map.Keys, Is.EqualTo(keys));
	}

	/// <summary>
	/// Matches the legacy <see cref="IdentityMap.InstantiateSequenced"/> behaviour, where
	/// overwriting a value moved the key to the end of the iteration order.
	/// </summary>
	[Test]
	public void OverwritingAValueMovesTheKeyToTheEnd()
	{
		var map = GetIdentityMap();
		var keys = Enumerable.Range(0, 4).Select(_ => new object()).ToArray();
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		map[keys[0]] = "rewritten";

		Assert.That(map.Keys, Is.EqualTo(new[] { keys[1], keys[2], keys[3], keys[0] }));
		Assert.That(map.Count, Is.EqualTo(4));
	}

	[Test]
	public void SnapshotPreservesOrder()
	{
		var map = GetIdentityMap();
		var keys = Enumerable.Range(0, 10).Select(_ => new object()).ToArray();
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = IdentityMapUtils.GetSnapshot(map);

		map.Add(new object(), "added");
		map[keys[0]] = "rewritten";

		Assert.That(snapshot.Count, Is.EqualTo(keys.Length));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
	}
}
