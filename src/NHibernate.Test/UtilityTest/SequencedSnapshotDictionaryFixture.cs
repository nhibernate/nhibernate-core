using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest;

/// <summary>
/// Tests for the insertion-ordered <see cref="SequencedSnapshotDictionary{TKey,TValue}"/>
/// that backs <see cref="IdentityMap{TKey,TValue}.InstantiateSequenced"/>.
/// </summary>
[TestFixture]
public class SequencedSnapshotDictionaryFixture
{
	private ISerializationStrategy _initialStrategy;

	[SetUp]
	public void SetUp()
	{
		_initialStrategy = SerializationConfiguration.Strategy;
	}

	[TearDown]
	public void TearDown()
	{
		SerializationConfiguration.Strategy = _initialStrategy;
	}

	[Test]
	public void AddsIterateInInsertionOrder()
	{
		var map = NewMap();
		var keys = Keys(50);

		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		Assert.That(KeyOrder(map), Is.EqualTo(keys));
		Assert.That(map.Keys, Is.EqualTo(keys));
		Assert.That(map.Values, Is.EqualTo(keys));
	}

	/// <summary>
	/// The <see cref="SequencedHashMap"/> this type replaces removes and re-inserts an entry
	/// whenever its value is overwritten through the indexer, so the key moves to the end.
	/// That behaviour is deliberately preserved.
	/// </summary>
	[Test]
	public void OverwritingExistingKeyMovesItToTheEnd()
	{
		var map = NewMap();
		var keys = Keys(4);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		map[keys[1]] = "rewritten";

		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[0], keys[2], keys[3], keys[1] }));
		Assert.That(map[keys[1]], Is.EqualTo("rewritten"));
		Assert.That(map.Count, Is.EqualTo(4), "an overwrite must not change the live count");
	}

	[Test]
	public void OverwritingTheLastKeyLeavesItLast()
	{
		var map = NewMap();
		var keys = Keys(3);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		map[keys[2]] = "rewritten";

		Assert.That(KeyOrder(map), Is.EqualTo(keys));
	}

	[Test]
	public void RepeatedOverwritesKeepMovingTheKeyToTheEnd()
	{
		var map = NewMap();
		var keys = Keys(3);
		foreach (var key in keys)
		{
			map.Add(key, 0);
		}

		map[keys[0]] = 1;
		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[1], keys[2], keys[0] }));

		map[keys[1]] = 1;
		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[2], keys[0], keys[1] }));

		map[keys[2]] = 1;
		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[0], keys[1], keys[2] }));
	}

	[Test]
	public void AddThrowsOnDuplicateKeyWithoutDisturbingOrder()
	{
		var map = NewMap();
		var keys = Keys(3);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		Assert.Throws<ArgumentException>(() => map.Add(keys[0], "again"));

		Assert.That(KeyOrder(map), Is.EqualTo(keys));
		Assert.That(map[keys[0]], Is.EqualTo("initial"));
		Assert.That(map.Count, Is.EqualTo(3));
	}

	[Test]
	public void RemovalPreservesTheOrderOfTheRemainingEntries()
	{
		var map = NewMap();
		var keys = Keys(6);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		Assert.That(map.Remove(keys[0]), Is.True);
		Assert.That(map.Remove(keys[3]), Is.True);
		Assert.That(map.Remove(keys[5]), Is.True);

		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[1], keys[2], keys[4] }));
	}

	[Test]
	public void ReAddingARemovedKeyPutsItAtTheEnd()
	{
		var map = NewMap();
		var keys = Keys(3);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		map.Remove(keys[0]);
		map.Add(keys[0], keys[0]);

		Assert.That(KeyOrder(map), Is.EqualTo(new[] { keys[1], keys[2], keys[0] }));
	}

	[Test]
	public void OrderSurvivesBucketGrowth()
	{
		var map = NewMap(1);
		var keys = Keys(500);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		Assert.That(KeyOrder(map), Is.EqualTo(keys));
		foreach (var key in keys)
		{
			Assert.That(map[key], Is.SameAs(key));
		}
	}

	[Test]
	public void OrderSurvivesHeavyOverwriteChurn()
	{
		var map = NewMap();
		var keys = Keys(40);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		var slotsAfterFill = map.EntrySlotCount;

		var expected = new List<object>(keys);
		for (var round = 0; round < 20; round++)
		{
			foreach (var key in keys)
			{
				map[key] = round;
				expected.Remove(key);
				expected.Add(key);
			}
		}

		Assert.That(KeyOrder(map), Is.EqualTo(expected));
		Assert.That(map.Count, Is.EqualTo(keys.Length));
		Assert.That(map.EntrySlotCount, Is.EqualTo(slotsAfterFill));
	}

	[Test]
	public void KeysAndValuesEnumerateInOrder()
	{
		var map = NewMap();
		var keys = Keys(5);
		for (var i = 0; i < keys.Length; i++)
		{
			map.Add(keys[i], i);
		}

		map[keys[0]] = 99; // moves keys[0] to the end

		Assert.That(map.Keys, Is.EqualTo(new[] { keys[1], keys[2], keys[3], keys[4], keys[0] }));
		Assert.That(map.Values, Is.EqualTo(new object[] { 1, 2, 3, 4, 99 }));
	}

	[Test]
	public void CopyToWritesEntriesInOrder()
	{
		var map = NewMap();
		var keys = Keys(4);
		for (var i = 0; i < keys.Length; i++)
		{
			map.Add(keys[i], i);
		}

		map.Remove(keys[1]);
		map[keys[0]] = 42; // moves keys[0] to the end

		var target = new KeyValuePair<object, object>[3];
		map.CopyTo(target, 0);

		Assert.That(target.Select(kv => kv.Key), Is.EqualTo(new[] { keys[2], keys[3], keys[0] }));
		Assert.That(target.Select(kv => kv.Value), Is.EqualTo(new object[] { 2, 3, 42 }));
	}

	[Test]
	public void AddRemoveChurnRecyclesSlots()
	{
		var map = NewMap();

		for (var i = 0; i < 5000; i++)
		{
			var key = new object();
			map.Add(key, i);
			map.Remove(key);
		}

		Assert.That(map.Count, Is.EqualTo(0));
		Assert.That(map.Count, Is.EqualTo(0));
		Assert.That(map.EntrySlotCount, Is.LessThanOrEqualTo(1),
					"add/remove churn must reuse the freed slot instead of consuming new ones");
	}

	[Test]
	public void RemovingKeepsSnapshotsIsolated()
	{
		var map = NewMap();
		var keys = Keys(10);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		using var snapshot = map.GetSnapshot();

		for (var i = 0; i < 500; i++)
		{
			map[keys[i % keys.Length]] = i;
		}

		Assert.That(snapshot.Count, Is.EqualTo(keys.Length));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys), "the snapshot must keep its original order");
		foreach (var key in keys)
		{
			Assert.That(snapshot.ValueFor(key), Is.EqualTo("initial"));
		}
	}

	[Test]
	public void SnapshotHonoursComparer()
	{
		var map = NewMap();
		var a = new MutableHashCode(7);
		var b = new MutableHashCode(7); // equal by value, different reference
		var value = new object();

		map[a] = value;
		using var snapshot = map.GetSnapshot();

		Assert.That(map.ContainsKey(b), Is.False, "the live map must use identity, not value equality");
		Assert.That(snapshot.HasKey(b), Is.False, "the snapshot must use identity, not value equality");
		Assert.That(snapshot.HasKey(a), Is.True);
		Assert.That(snapshot.ValueFor(a), Is.SameAs(value));
	}

	[Test]
	public void SnapshotEnumeratesInTheSameOrderAsTheMap()
	{
		var map = NewMap();
		var keys = Keys(30);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		map[keys[0]] = "moved";

		using var snapshot = map.GetSnapshot();

		Assert.That(snapshot.KeyList(), Is.EqualTo(KeyOrder(map)));
	}

	[Test]
	public void SnapshotIsUnaffectedByLaterReordering()
	{
		var map = NewMap();
		var keys = Keys(5);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		using var snapshot = map.GetSnapshot();

		map[keys[0]] = "moved";
		map[keys[1]] = "moved";
		map.Remove(keys[4]);
		map.Add(new object(), "added");

		Assert.That(snapshot.KeyList(), Is.EqualTo(keys), "the snapshot keeps its frozen order");
		Assert.That(snapshot.ValueList(), Is.EqualTo(Enumerable.Repeat("initial", 5)));
	}

	[Test]
	public void SnapshotIsIsolatedFromClear()
	{
		var map = NewMap();
		var keys = Keys(10);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();

		map.Clear();

		Assert.That(map.Count, Is.EqualTo(0));
		Assert.That(snapshot.Count, Is.EqualTo(10));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
	}

	[Test]
	public void SnapshotIsIsolatedFromAppend()
	{
		var map = NewMap(4);
		var keys = Keys(4);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();

		var added = Keys(4);
		foreach (var key in added)
		{
			map.Add(key, key);
		}

		Assert.That(snapshot.Count, Is.EqualTo(4));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
		foreach (var key in added)
		{
			Assert.That(snapshot.HasKey(key), Is.False,
						"entries appended after the freeze must not be reachable through the snapshot's chains");
		}
	}

	[Test]
	public void RemovingAnEntryAppendedAfterTheSnapshotLeavesTheSnapshotUntouched()
	{
		var map = NewMap(4);
		var keys = Keys(4);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();

		var extra = new object();
		map.Add(extra, extra);
		map.Remove(extra);

		Assert.That(snapshot.Count, Is.EqualTo(4));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
		Assert.That(map.Count, Is.EqualTo(4));
	}

	[Test]
	public void ChainedAppendsNeverLeakIntoAnEarlierSnapshot()
	{
		var map = NewMap();
		var initialKeys = Keys(3);
		foreach (var key in initialKeys)
		{
			map.Add(key, key);
		}

		using var snapshotA = map.GetSnapshot();
		var beforeAppends = map.CopyOnWriteCount;

		var d = new object();
		map.Add(d, d);

		using var snapshotB = map.GetSnapshot();

		var e = new object();
		map.Add(e, e);
		var f = new object();
		map.Add(f, f);

		Assert.That(snapshotA.KeyList(), Is.EqualTo(initialKeys));
		Assert.That(snapshotB.KeyList(), Is.EqualTo(new[] { initialKeys[0], initialKeys[1], initialKeys[2], d }));
		Assert.That(map.Keys, Is.EqualTo(new[] { initialKeys[0], initialKeys[1], initialKeys[2], d, e, f }));
		Assert.That(map.CopyOnWriteCount, Is.EqualTo(beforeAppends));
	}

	[Test]
	public void RehashAndChainChurnUnderAnOutstandingSnapshotCopyNothingAndAreInvisible()
	{
		// Start small so growing to 400 entries forces several rehashes.
		var map = NewMap(1);
		var original = Keys(20);
		foreach (var key in original)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();
		var copiesAfterSnapshot = map.CopyOnWriteCount;

		// Force repeated rehashes (and growth) while the snapshot is outstanding.
		var added = Keys(380);
		foreach (var key in added)
		{
			map.Add(key, key);
		}

		// Growth and rehashing touch only the hash structure, which no snapshot reads
		Assert.That(map.CopyOnWriteCount, Is.EqualTo(copiesAfterSnapshot));

		// Now remove the entries that were added, which unlinks them from their bucket chains.
		// These slots are all at or beyond the snapshot, so this must stay copy-free too.
		foreach (var key in added)
		{
			map.Remove(key);
		}

		Assert.That(map.CopyOnWriteCount, Is.EqualTo(copiesAfterSnapshot));

		Assert.That(snapshot.Count, Is.EqualTo(original.Length));
		Assert.That(snapshot.KeyList(), Is.EqualTo(original));
		Assert.That(snapshot.ValueList(), Is.EqualTo(original));

		Assert.That(map.Count, Is.EqualTo(original.Length));
		Assert.That(KeyOrder(map), Is.EqualTo(original));
		foreach (var key in original)
		{
			Assert.That(map[key], Is.SameAs(key));
		}

		foreach (var key in added)
		{
			Assert.That(map.ContainsKey(key), Is.False);
		}
	}

	[Test]
	public void RemovalsWithinSnapshotCopyAndStayIsolated()
	{
		var map = NewMap(1);
		var keys = Keys(20);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();
		var before = map.CopyOnWriteCount;

		foreach (var key in keys)
		{
			map.Remove(key);
		}

		// +1 copy for slots
		// +1 copy for values
		// +1 copy for orderNext
		Assert.That(map.CopyOnWriteCount, Is.EqualTo(before + 3),
					"removing entries the snapshot can see must pay the copy-on-write cost");
		Assert.That(map.Count, Is.EqualTo(0));

		Assert.That(snapshot.Count, Is.EqualTo(keys.Length));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
		Assert.That(snapshot.ValueList(), Is.EqualTo(keys));
	}

	[Test]
	public void UpdatesWithinSnapshotCopyAndStayIsolated()
	{
		var map = NewMap(1);
		var keys = Keys(20);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();
		var before = map.CopyOnWriteCount;

		foreach (var key in keys)
		{
			map[key] = new object();
		}

		// +1 copy for values
		// +1 copy for orderNext
		Assert.That(map.CopyOnWriteCount, Is.EqualTo(before + 2),
					"updating entries the snapshot can see must pay the copy-on-write cost");
		Assert.That(map.Count, Is.EqualTo(keys.Length));

		Assert.That(snapshot.Count, Is.EqualTo(keys.Length));
		Assert.That(snapshot.KeyList(), Is.EqualTo(keys));
		Assert.That(snapshot.ValueList(), Is.EqualTo(keys));
	}

	[Test]
	public void OverlappingSnapshotsAreEachIsolated()
	{
		var map = NewMap(4, new FewBucketsComparer());
		var initialKeys = Keys(3);
		foreach (var key in initialKeys)
		{
			map.Add(key, "v1");
		}

		using var snapshotA = map.GetSnapshot();

		map[initialKeys[0]] = "v2";
		var keyAddedAfterSnapshotA = new object();
		map.Add(keyAddedAfterSnapshotA, "v2");

		using var snapshotB = map.GetSnapshot();

		map[initialKeys[1]] = "v3";
		map.Remove(initialKeys[2]);

		Assert.That(map[initialKeys[1]], Is.EqualTo("v3"));

		Assert.That(snapshotA.Count, Is.EqualTo(3));
		Assert.That(snapshotA.KeyList(), Is.EqualTo(initialKeys));
		Assert.That(snapshotA.ValueList(), Is.EqualTo(new object[] { "v1", "v1", "v1" }));

		Assert.That(snapshotB.Count, Is.EqualTo(4));
		Assert.That(snapshotB.KeyList(), Is.EqualTo(new[] { initialKeys[1], initialKeys[2], initialKeys[0], keyAddedAfterSnapshotA }));
		Assert.That(snapshotB.ValueFor(initialKeys[0]), Is.EqualTo("v2"));
		Assert.That(snapshotB.ValueFor(initialKeys[1]), Is.EqualTo("v1"));
	}

	[Test]
	public void AppendsAfterSnapshotCopyNothing()
	{
		var map = NewMap();
		foreach (var key in Keys(8))
		{
			map.Add(key, key);
		}

		using var snapshot = map.GetSnapshot();

		var before = map.CopyOnWriteCount;
		foreach (var key in Keys(32))
		{
			map.Add(key, key);
		}

		Assert.That(map.CopyOnWriteCount - before, Is.EqualTo(0),
					"appends while a snapshot is outstanding must not copy any array");
		Assert.That(snapshot.Count, Is.EqualTo(8));
	}

	[Test]
	public void DisposingSnapshotAvoidsCopyOnWriteOnSubsequentWrites()
	{
		var map = NewMap();
		var keys = Keys(8);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		using (var snapshot = map.GetSnapshot())
		{
			Assert.That(map.OutstandingSnapshots, Is.EqualTo(1));
			Assert.That(snapshot.Count, Is.EqualTo(8));
		}

		Assert.That(map.OutstandingSnapshots, Is.EqualTo(0));

		var before = map.CopyOnWriteCount;
		foreach (var key in keys)
		{
			map[key] = "rewritten";
		}
		map.Add(new object(), "added");
		map.Remove(keys[0]);

		Assert.That(map.CopyOnWriteCount, Is.EqualTo(before),
					"once every snapshot is released, writes must go back to being fully in place");
	}

	[Test]
	public void LeavingSnapshotUndisposedCausesSubsequentWritesToCopy()
	{
		var map = NewMap();
		var keys = Keys(8);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		var snapshot = map.GetSnapshot(); // deliberately not disposed

		var before = map.CopyOnWriteCount;
		map[keys[0]] = "rewritten";

		Assert.That(map.CopyOnWriteCount, Is.GreaterThan(before));
		Assert.That(map.OutstandingSnapshots, Is.EqualTo(1));
		Assert.That(snapshot.ValueFor(keys[0]), Is.EqualTo("initial"));
	}

	[Test]
	public void OverlappingSnapshotsOnlyStopCopyingOnceAllAreDisposed()
	{
		var map = NewMap();
		var keys = Keys(8);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		var first = map.GetSnapshot();
		var second = map.GetSnapshot();
		Assert.That(map.OutstandingSnapshots, Is.EqualTo(2));

		first.Dispose();
		Assert.That(map.OutstandingSnapshots, Is.EqualTo(1));

		var before = map.CopyOnWriteCount;
		map[keys[0]] = "rewritten";
		Assert.That(map.CopyOnWriteCount, Is.GreaterThan(before),
					"one snapshot is still outstanding, so writes must still copy");

		second.Dispose();
		Assert.That(map.OutstandingSnapshots, Is.EqualTo(0));

		before = map.CopyOnWriteCount;
		map[keys[1]] = "rewritten";
		map.Add(new object(), "added");
		Assert.That(map.CopyOnWriteCount, Is.EqualTo(before));
	}

	[Test]
	public void ReadsAfterDisposeThrowObjectDisposedException()
	{
		var map = NewMap();
		var key = new object();
		map.Add(key, "value");

		var snapshot = map.GetSnapshot();
		snapshot.Dispose();

		Assert.Throws<ObjectDisposedException>(() => { var _ = snapshot.Count; });
		Assert.Throws<ObjectDisposedException>(() => { var _ = snapshot.ValueFor(key); });
		Assert.Throws<ObjectDisposedException>(() => snapshot.TryGetValueByIdentity(key, out _));
		Assert.Throws<ObjectDisposedException>(() => snapshot.HasKey(key));
		Assert.Throws<ObjectDisposedException>(() => snapshot.GetEnumerator());
	}

	[Test]
	public void DisposeIsIdempotent()
	{
		var map = NewMap();
		map.Add(new object(), "value");

		var snapshot = map.GetSnapshot();
		snapshot.Dispose();
		snapshot.Dispose();
		snapshot.Dispose();

		Assert.That(map.OutstandingSnapshots, Is.EqualTo(0),
					"repeated disposal must not drive the refcount negative");
	}

	[Test]
	public void DisposingSnapshotMidEnumerationThrowsOnNextMoveNext()
	{
		var map = NewMap();
		foreach (var key in Keys(5))
		{
			map.Add(key, key);
		}

		var snapshot = map.GetSnapshot();
		using var enumerator = snapshot.GetEnumerator();

		Assert.That(enumerator.MoveNext(), Is.True);
		snapshot.Dispose();

		Assert.Throws<ObjectDisposedException>(() => enumerator.MoveNext());
	}

	[Test]
	public void EnumerationThrowsWhenMapIsModified()
	{
		var map = NewMap();
		foreach (var key in Keys(5))
		{
			map.Add(key, key);
		}

		Assert.Throws<InvalidOperationException>(() =>
		{
			foreach (var _ in map)
			{
				map.Add(new object(), "added");
			}
		});
	}

	[Test]
	public void EnumerationThrowsWhenAValueIsOverwritten()
	{
		var map = NewMap();
		var keys = Keys(5);
		foreach (var key in keys)
		{
			map.Add(key, "initial");
		}

		Assert.Throws<InvalidOperationException>(() =>
		{
			foreach (var _ in map)
			{
				map[keys[0]] = "rewritten";
			}
		});
	}

	[Test]
	public void KeysEnumerationThrowsWhenMapIsModified()
	{
		var map = NewMap();
		foreach (var key in Keys(5))
		{
			map.Add(key, key);
		}

		Assert.Throws<InvalidOperationException>(() =>
		{
			foreach (var _ in map.Keys)
			{
				map.Add(new object(), "added");
			}
		});
	}

	[Test]
	public void ValuesEnumerationThrowsWhenMapIsModified()
	{
		var map = NewMap();
		foreach (var key in Keys(5))
		{
			map.Add(key, key);
		}

		Assert.Throws<InvalidOperationException>(() =>
		{
			foreach (var _ in map.Values)
			{
				map.Add(new object(), "added");
			}
		});
	}

	[Test]
	public void NegativeCapacityThrows()
	{
		Assert.Throws<ArgumentOutOfRangeException>(
			() => new SequencedSnapshotDictionary<object, object>(-1, ReferenceComparer<object>.Instance));
	}

	[Test]
	public void ZeroCapacityStillAcceptsEntries()
	{
		var map = NewMap(0);
		var keys = Keys(20);
		foreach (var key in keys)
		{
			map.Add(key, key);
		}

		Assert.That(map.Count, Is.EqualTo(20));
		Assert.That(KeyOrder(map), Is.EqualTo(keys));
	}

	[Test]
	public void IndexerGetThrowsForMissingKey()
	{
		var map = NewMap();
		Assert.Throws<KeyNotFoundException>(() => { var _ = map[new object()]; });
	}

	[Test]
	public void RemovingAbsentKeyIsANoOp()
	{
		var map = NewMap();
		var key = new object();
		map.Add(key, "value");

		Assert.That(map.Remove(new object()), Is.False);
		Assert.That(map.Count, Is.EqualTo(1));
		Assert.That(map[key], Is.EqualTo("value"));
	}

	[Test]
	public void ContainsAndRemoveByPairMatchOnValueToo()
	{
		var map = NewMap();
		var key = new object();
		map.Add(key, "value");

		Assert.That(map.Contains(new KeyValuePair<object, object>(key, "value")), Is.True);
		Assert.That(map.Contains(new KeyValuePair<object, object>(key, "other")), Is.False);

		Assert.That(map.Remove(new KeyValuePair<object, object>(key, "other")), Is.False);
		Assert.That(map.Count, Is.EqualTo(1));

		Assert.That(map.Remove(new KeyValuePair<object, object>(key, "value")), Is.True);
		Assert.That(map.Count, Is.EqualTo(0));
	}

	[Test]
	public void ClearedMapIsStillUsable()
	{
		var map = NewMap();
		foreach (var key in Keys(10))
		{
			map.Add(key, key);
		}

		map.Clear();
		Assert.That(map.Count, Is.EqualTo(0));
		Assert.That(map, Is.Empty);

		var after = Keys(5);
		foreach (var key in after)
		{
			map.Add(key, key);
		}

		Assert.That(KeyOrder(map), Is.EqualTo(after));
	}

	[Test]
	public void KeysAndValuesAreCachedAcrossAccesses()
	{
		var map = NewMap();
		var keys = map.Keys;
		var values = map.Values;

		Assert.That(map.Keys, Is.SameAs(keys));
		Assert.That(map.Values, Is.SameAs(values));
	}

	[Test]
	public void CopyToThrowsForNullArray()
	{
		var map = NewMap();
		Assert.Throws<ArgumentNullException>(() => map.CopyTo(null, 0));
		Assert.Throws<ArgumentNullException>(() => map.Keys.CopyTo(null, 0));
		Assert.Throws<ArgumentNullException>(() => map.Values.CopyTo(null, 0));
	}

	[Test]
	public void CopyToThrowsForNegativeIndex()
	{
		var map = NewMap();
		Assert.Throws<ArgumentOutOfRangeException>(() => map.CopyTo(new KeyValuePair<object, object>[1], -1));
		Assert.Throws<ArgumentOutOfRangeException>(() => map.Keys.CopyTo(new object[1], -1));
		Assert.Throws<ArgumentOutOfRangeException>(() => map.Values.CopyTo(new object[1], -1));
	}

	[Test]
	public void CopyToThrowsForInsufficientSpace()
	{
		var map = NewMap();
		foreach (var key in Keys(3))
		{
			map.Add(key, key);
		}

		Assert.Throws<ArgumentException>(() => map.CopyTo(new KeyValuePair<object, object>[2], 0));
		Assert.Throws<ArgumentException>(() => map.CopyTo(new KeyValuePair<object, object>[3], 1));
		Assert.Throws<ArgumentException>(() => map.Keys.CopyTo(new object[2], 0));
		Assert.Throws<ArgumentException>(() => map.Values.CopyTo(new object[2], 0));
	}

	[Test]
	public void KeysAndValuesCollectionsAreReadOnly()
	{
		var map = NewMap();
		Assert.That(map.Keys.IsReadOnly, Is.True);
		Assert.That(map.Values.IsReadOnly, Is.True);
		Assert.Throws<NotSupportedException>(() => map.Keys.Add(new object()));
		Assert.Throws<NotSupportedException>(() => map.Keys.Clear());
		Assert.Throws<NotSupportedException>(() => map.Keys.Remove(new object()));
		Assert.Throws<NotSupportedException>(() => map.Values.Add(new object()));
		Assert.Throws<NotSupportedException>(() => map.Values.Clear());
		Assert.Throws<NotSupportedException>(() => map.Values.Remove(new object()));
	}

	[Test]
	public void KeysAndValuesContainsWork()
	{
		var map = NewMap();
		var key = new object();
		map.Add(key, "value");

		Assert.That(map.Keys.Contains(key), Is.True);
		Assert.That(map.Keys.Contains(new object()), Is.False);
		Assert.That(map.Values.Contains("value"), Is.True);
		Assert.That(map.Values.Contains("other"), Is.False);

		map.Remove(key);
		Assert.That(map.Keys.Contains(key), Is.False);
		Assert.That(map.Values.Contains("value"), Is.False);
	}

	[Test]
	public void RemovedEntryDoesNotRetainKeyOrValueReferences()
	{
		var map = NewMap();
		var weak = PopulateAndRemove(map);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.That(weak.IsAlive, Is.False);
	}

	private static WeakReference PopulateAndRemove(SequencedSnapshotDictionary<object, object> map)
	{
		var key = new object();
		var value = new object();
		map.Add(key, value);
		var weak = new WeakReference(value);
		map.Remove(key);
		return weak;
	}

	[Test]
	public void OverwrittenValueIsNotRetained()
	{
		var map = NewMap();
		var key = new object();
		var weak = PopulateAndOverwrite(map, key);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.That(weak.IsAlive, Is.False);
	}

	private static WeakReference PopulateAndOverwrite(SequencedSnapshotDictionary<object, object> map, object key)
	{
		var original = new object();
		map.Add(key, original);
		var weak = new WeakReference(original);
		map[key] = new object();
		return weak;
	}

	[Test]
	public void ClearedEntriesDoNotRetainKeyOrValueReferences()
	{
		var map = NewMap();
		var weak = PopulateAndClear(map);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.That(weak.IsAlive, Is.False);
	}

	private static WeakReference PopulateAndClear(SequencedSnapshotDictionary<object, object> map)
	{
		var key = new object();
		var value = new object();
		map.Add(key, value);
		var weak = new WeakReference(value);
		map.Clear();
		return weak;
	}

	[Test]
	public void SerializationRoundTripPreservesOrderContentsAndIdentitySemantics()
	{
		TestConfigurationHelper.UseTestSerialization();

		var map = NewMap();
		var a = new MutableHashCode(1);
		var b = new MutableHashCode(1);
		var c = new MutableHashCode(2);
		map.Add(a, "valueA");
		map.Add(b, "valueB");
		map.Add(c, "valueC");
		map[a] = "valueA"; // move a to the end, so order is b, c, a
		map.Remove(c);

		using var unusedSnapshot = map.GetSnapshot();

		var bytes = SerializationHelper.Serialize(map);
		var restored = (SequencedSnapshotDictionary<object, object>) SerializationHelper.Deserialize(bytes);
		((IDeserializationCallback) restored).OnDeserialization(null);

		Assert.That(restored.Count, Is.EqualTo(2));
		Assert.That(restored.Values, Is.EqualTo(new object[] { "valueB", "valueA" }),
					"iteration order, including the move-to-end, must survive the round trip");
		Assert.That(restored.OutstandingSnapshots, Is.EqualTo(0),
					"a freshly deserialized instance must start with no outstanding snapshots");

		// A round trip necessarily produces new key instances, so the live `a`/`b` references
		// can never be reference-equal to anything inside `restored`. What must hold is that
		// identity semantics still apply to the deserialized keys, and that each is findable by
		// reference after the post-deserialization rehash (proving the rehash used each key's
		// own bucket, not its pre-serialization one).
		var restoredKeys = restored.Keys.ToArray();
		Assert.That(restoredKeys, Has.Length.EqualTo(2));
		Assert.That(restored.ContainsKey(new MutableHashCode(1)), Is.False,
					"the restored map must still use identity, not value equality");
		foreach (var key in restoredKeys)
		{
			Assert.That(restored.ContainsKey(key), Is.True);
		}

		// Writes right after deserialization must not pay any copy-on-write cost, since
		// OnDeserialization reset the snapshot bookkeeping to a clean state.
		var copiesBeforeWrite = restored.CopyOnWriteCount;
		restored[restoredKeys[0]] = "rewritten";
		Assert.That(restored.CopyOnWriteCount, Is.EqualTo(copiesBeforeWrite),
					"writes right after deserialization must not pay any copy-on-write cost");

		// still writable and enumerable after the rehash
		var extra = new object();
		restored.Add(extra, "valueD");
		Assert.That(restored.Count, Is.EqualTo(3));
		Assert.That(restored.Keys, Is.EqualTo(new[] { restoredKeys[1], restoredKeys[0], extra }));
	}

	[Test]
	public void RandomizedOrderAndSnapshotIsolationStress() => RunRandomizedStress(seed: 20260806, comparer: null);

	[Test]
	public void RandomizedOrderAndSnapshotIsolationStressWithForcedBucketCollisions() => RunRandomizedStress(seed: 20260806, comparer: new FewBucketsComparer());

	[Test, Explicit]
	public void RandomizedOrderAndSnapshotIsolationStress_100kIter() => RunRandomizedStress(seed: 20260806, comparer: null, iterCount: 100_000);

	[Test, Explicit]
	public void RandomizedOrderAndSnapshotIsolationStress_100kStep() => RunRandomizedStress(seed: 20260806, comparer: null, stepCount: 100_000);

	/// <summary>
	/// Randomised model-based check. The model is an ordered list of key/value pairs mutated
	/// with exactly the semantics this dictionary promises: append on add, move-to-end on
	/// overwrite, remove in place.
	/// Every snapshot taken along the way must keep exactly the contents and order it had when
	/// it was created, regardless of what happens to the live map afterwards.
	/// </summary>
	private static void RunRandomizedStress(int seed, IEqualityComparer<object> comparer, int iterCount = 100, int stepCount = 500)
	{
		var rnd = new Random(seed);

		for (var iter = 0; iter < iterCount; iter++)
		{
			var map = new SequencedSnapshotDictionary<object, object>(
				1 << rnd.Next(0, 5),
				comparer ?? ReferenceComparer<object>.Instance);

			var model = new List<KeyValuePair<object, object>>();
			var pool = Keys(30);

			// Snapshots still held, each with the model as it was when the snapshot was taken.
			var outstanding = new List<(ISnapshotView<object, object> View,
				List<KeyValuePair<object, object>> Expected)>();

			for (var step = 0; step < stepCount; step++)
			{
				switch (rnd.Next(0, 10))
				{
					case 0:
					case 1:
					case 2:
					case 3:
						{
							// Add or overwrite.
							var key = pool[rnd.Next(pool.Length)];
							var value = rnd.Next(1000);
							var at = model.FindIndex(kv => ReferenceEquals(kv.Key, key));
							if (at >= 0)
							{
								model.RemoveAt(at); // overwrite moves the key to the end
							}
							model.Add(new KeyValuePair<object, object>(key, value));
							map[key] = value;
							break;
						}
					case 4:
					case 5:
						{
							// Remove, sometimes an absent key.
							var key = pool[rnd.Next(pool.Length)];
							var at = model.FindIndex(kv => ReferenceEquals(kv.Key, key));
							var expected = at >= 0;
							if (expected)
							{
								model.RemoveAt(at);
							}

							Assert.That(map.Remove(key), Is.EqualTo(expected));
							break;
						}
					case 6:
						{
							if (rnd.Next(0, 20) == 0)
							{
								model.Clear();
								map.Clear();
							}
							break;
						}
					case 7:
						{
							outstanding.Add((map.GetSnapshot(), [.. model]));
							break;
						}
					case 8:
						{
							if (outstanding.Count > 0)
							{
								var idx = rnd.Next(outstanding.Count);
								outstanding[idx].View.Dispose();
								outstanding.RemoveAt(idx);
							}
							break;
						}
					default:
						{
							// Read-back check against the model.
							var key = pool[rnd.Next(pool.Length)];
							var at = model.FindIndex(kv => ReferenceEquals(kv.Key, key));
							if (at >= 0)
							{
								Assert.That(map.TryGetValue(key, out var actual), Is.True);
								Assert.That(actual, Is.EqualTo(model[at].Value));
							}
							else
							{
								Assert.That(map.ContainsKey(key), Is.False);
							}
							break;
						}
				}

				Assert.That(map.Count, Is.EqualTo(model.Count));

				// Every outstanding snapshot must still match the model as of its creation,
				// including ordering.
				foreach (var (view, expected) in outstanding)
				{
					Assert.That(view.Count, Is.EqualTo(expected.Count));
					Assert.That(view.KeyList(), Is.EqualTo(expected.Select(kv => kv.Key)));
					Assert.That(view.ValueList(), Is.EqualTo(expected.Select(kv => kv.Value)));
				}
			}

			// The live map must match the model in both contents and order.
			Assert.That(map.Keys, Is.EqualTo(model.Select(kv => kv.Key)));
			Assert.That(map.Values, Is.EqualTo(model.Select(kv => kv.Value)));

			// Slot usage must stay bounded by the key pool rather than by how many operations
			// were performed (overwrites consume nothing and removals recycle).
			Assert.That(map.EntrySlotCount, Is.LessThanOrEqualTo(pool.Length),
						"slot usage must track the live high-water mark, not the write volume");

			Assert.That(map.OutstandingSnapshots, Is.EqualTo(outstanding.Count));
			foreach (var (view, _) in outstanding)
			{
				view.Dispose();
			}

			Assert.That(map.OutstandingSnapshots, Is.EqualTo(0));
		}
	}

	private static SequencedSnapshotDictionary<object, object> NewMap(
		int capacity = 16,
		IEqualityComparer<object> comparer = null) =>
		new(capacity, comparer ?? ReferenceComparer<object>.Instance);

	private static object[] Keys(int count) => [.. Enumerable.Range(0, count).Select(_ => new object())];

	private static List<object> KeyOrder(IEnumerable<KeyValuePair<object, object>> map) =>
		[.. map.Select(kv => kv.Key)];

	private sealed class FewBucketsComparer : IEqualityComparer<object>
	{
		private readonly int _bucketCount;
		public FewBucketsComparer(int bucketCount = 2) => _bucketCount = bucketCount;

		public new bool Equals(object x, object y) => ReferenceEquals(x, y);

		public int GetHashCode(object obj) =>
			System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj) % _bucketCount;
	}
}
