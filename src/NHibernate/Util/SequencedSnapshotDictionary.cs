using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NHibernate.Util;

/// <summary>
/// A read-only view of a dictionary. Dispose it as soon as possible.
/// </summary>
internal interface ISnapshotView<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDisposable;

/// <summary>
/// A dictionary with whose entries are sequenced based on the order in which they were
/// added or modified and implements snapshotting with copy-on-write.
/// </summary>
/// <remarks>
/// This class is not thread safe.
/// The core dictionary implementation is based on <see cref="System.Collections.Generic.Dictionary{TKey,TValue}" />
/// </remarks>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
[Serializable]
internal sealed class SequencedSnapshotDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IDeserializationCallback
	where TKey : notnull
{
	/// <summary>
	/// The hot half of an entry: everything a lookup probe needs, and nothing else. Kept deliberately
	/// narrow.
	/// </summary>
	[Serializable]
	private struct Slot
	{
		public TKey Key;

		// Index of the next entry in the same bucket chain, -1 for the end of the chain. Doubles as
		// the free-list link while the slot is unused.
		public int HashNext;
	}

	private readonly IEqualityComparer<TKey> _comparer;

	// Physical slot order is meaningless. Iteration order is the _orderPrev/_orderNext list.
	private Slot[] _slots;
	private TValue[] _values;
	private int[] _orderNext;
	private int[] _orderPrev;

	// Ends of the iteration-order list, -1 when empty.
	private int _orderHead;
	private int _orderTail;

	// Head of the free-slot list (-1 when empty) and its length.
	private int _freeListHead;
	private int _freeListCount;

	// Per-bucket chain head stored as (entry index + 1), so the default 0 means "empty" and a
	// freshly allocated array needs no initialization pass. Must be rebuilt on deserialization
	// as hash codes are not stable.
	[NonSerialized]
	private int[] _buckets;

	// Epoch each array was last (re)created at. An array whose epoch is <= _latestSnapshotEpoch is
	// potentially shared with an outstanding snapshot and must be copied before it can be written.
	// Only the three arrays a snapshot actually reads need one.
	[NonSerialized]
	private int _slotsEpoch;
	[NonSerialized]
	private int _valuesEpoch;
	[NonSerialized]
	private int _orderNextEpoch;

	// Only increments whenever a snapshot is taken
	[NonSerialized]
	private int _epoch;

	// Arrays with epoch <= this must be copied before mutation. -1 when there are no active snapshots.
	// Only the latest snapshot is tracked as the snapshot constraint of the latest snapshot includes
	// the constraints of any snapshot before it. A change that would affect an earlier snapshot will
	// always affect a newer snapshot (so any change that affects any snapshot will always affect the
	// latest snapshot).
	[NonSerialized]
	private int _latestSnapshotEpoch;

	// EntrySlotCount as of the latest GetSnapshot(). A write is observable (and requires cloning) if
	// its epoch <= latestSnapshotEpoch AND index < _latestSnapshotEntrySlotCount.
	[NonSerialized]
	private int _latestSnapshotEntrySlotCount;

	// _orderTail as of the latest GetSnapshot(). Tracked for an optimization in _orderNext updates where
	// if index == _latestSnapshotTail then the update to _orderNext is not meaningfully observable by
	// any outstanding snapshot.
	[NonSerialized]
	private int _latestSnapshotOrderTail;

	[NonSerialized]
	private int _version; // bumped on every mutation to fail enumerate-and-update scenarios

	[NonSerialized]
	private int _outstandingSnapshots;

	// Number of array copies performed to preserve isolation for an outstanding snapshot. Exposed for
	// tests to verify that disposing snapshots promptly avoids this cost; not incremented by
	// growth/rehash, which are unrelated to snapshotting.
	[NonSerialized]
	internal int CopyOnWriteCount;

	[NonSerialized]
	private KeyCollection _keys;
	[NonSerialized]
	private ValueCollection _valuesView;

	public SequencedSnapshotDictionary(int capacity = 16, IEqualityComparer<TKey> comparer = null)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(capacity));
		}

		_comparer = comparer ?? EqualityComparer<TKey>.Default;
		InitializeEmpty(BucketCountFor(capacity), CapacityFor(capacity));
	}

	private static int BucketCountFor(int liveCount)
	{
		var bucketCount = 1;
		while (bucketCount < liveCount)
		{
			bucketCount <<= 1;
		}

		return bucketCount;
	}

	private static int CapacityFor(int neededCount)
	{
		if (neededCount <= 0)
		{
			return 0;
		}

		var capacity = 4;
		while (capacity < neededCount)
		{
			capacity <<= 1;
		}

		return capacity;
	}

	private void InitializeEmpty(int bucketCount, int capacity)
	{
		_buckets = new int[bucketCount];
		if (capacity == 0)
		{
			_slots = [];
			_values = [];
			_orderNext = [];
			_orderPrev = [];
		}
		else
		{
			_slots = new Slot[capacity];
			_values = new TValue[capacity];
			_orderNext = new int[capacity];
			_orderPrev = new int[capacity];
		}

		EntrySlotCount = 0;
		Count = 0;
		_orderHead = -1;
		_orderTail = -1;
		_freeListHead = -1;
		_freeListCount = 0;

		_epoch = 0;
		_slotsEpoch = 0;
		_valuesEpoch = 0;
		_orderNextEpoch = 0;
		_latestSnapshotEpoch = -1;
		_latestSnapshotEntrySlotCount = 0;
		_latestSnapshotOrderTail = -1;
		_version = 0;
		_outstandingSnapshots = 0;
		CopyOnWriteCount = 0;
		_keys = null;
		_valuesView = null;
	}

	public int Count { get; private set; }

	// Number of SnapshotView instances handed out that have not yet been
	// disposed. Should return to zero once every taken snapshot has been released.
	// Exposed for tests.
	internal int OutstandingSnapshots => _outstandingSnapshots;

	// How many slots have ever been handed out. Free-list reuse is what keeps this
	// pinned to the high-water mark of live entries instead of growing with write volume.
	// This number only ever goes up in Insert up or gets reset to 0 in Clear.
	// Exposed for tests.
	internal int EntrySlotCount { get; private set; }

	/// <summary>
	/// O(1). Returns a read-only, insertion-ordered view frozen at this exact moment. No copying
	/// happens here.
	/// </summary>
	/// <remarks>
	/// Dispose the returned view as soon possible. Once every outstanding snapshot has been disposed,
	/// the dictionary reverts to fully in-place as if no snapshot had ever been taken. 
	/// </remarks>
	public ISnapshotView<TKey, TValue> GetSnapshot()
	{
		_latestSnapshotEpoch = _epoch;
		_latestSnapshotEntrySlotCount = EntrySlotCount;
		_latestSnapshotOrderTail = _orderTail;

		_epoch++;
		_outstandingSnapshots++;
		return new SnapshotView(this, _slots, _values, _orderNext, _orderHead, Count);
	}

	// Called by SnapshotView.Dispose(). Once the last outstanding snapshot is released, no view
	// can observe any array any more, so the floor is reset and writes go back to being fully in
	// place.
	private void ReleaseSnapshot()
	{
		if (_outstandingSnapshots > 0 && --_outstandingSnapshots == 0)
		{
			_latestSnapshotEpoch = -1;
			_latestSnapshotEntrySlotCount = EntrySlotCount;
			_latestSnapshotOrderTail = _orderTail;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			var index = FindEntry(key, out _);
			if (index < 0)
			{
				throw new KeyNotFoundException();
			}

			return _values[index];
		}
		set => SetCore(key, value, throwIfExists: false);
	}

	private KeyCollection KeysInternal => _keys ??= new KeyCollection(this);

	private ValueCollection ValuesInternal => _valuesView ??= new ValueCollection(this);

	public ICollection<TKey> Keys => KeysInternal;

	public ICollection<TValue> Values => _valuesView ??= new ValueCollection(this);

	public bool IsReadOnly => false;

	public void Add(TKey key, TValue value)
	{
		SetCore(key, value, throwIfExists: true);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public bool ContainsKey(TKey key) => FindEntry(key, out _) >= 0;

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		var index = FindEntry(item.Key, out _);
		return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], item.Value);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		var index = FindEntry(key, out _);
		if (index < 0)
		{
			value = default!;
			return false;
		}

		value = _values[index];
		return true;
	}

	public void Clear()
	{
		if (Count == 0 && EntrySlotCount == 0)
		{
			return;
		}

		// No snapshot ever reads the bucket array, so it is always safe to zero in place.
		Array.Clear(_buckets, 0, _buckets.Length);

		if (_slotsEpoch <= _latestSnapshotEpoch || _valuesEpoch <= _latestSnapshotEpoch || _orderNextEpoch <= _latestSnapshotEpoch)
		{
			// At least one array is still visible to an outstanding snapshot, so it has to be
			// abandoned rather than zeroed. Replace the whole set together, since slot indices are
			// only meaningful if every array is the same length.
			_slots = [];
			_values = [];
			_orderNext = [];
			_orderPrev = [];
			_slotsEpoch = _epoch;
			_valuesEpoch = _epoch;
			_orderNextEpoch = _epoch;
			CopyOnWriteCount++;
		}
		else
		{
			// Don't keep the cleared entries' keys/values alive.
			Array.Clear(_slots, 0, EntrySlotCount);
			Array.Clear(_values, 0, EntrySlotCount);
		}

		EntrySlotCount = 0;
		Count = 0;
		_orderHead = -1;
		_orderTail = -1;
		_freeListHead = -1;
		_freeListCount = 0;
		_version++;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array is null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		}

		if (array.Length - arrayIndex < Count)
		{
			throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(array));
		}

		var destinationIndex = arrayIndex;
		for (var sourceIndex = _orderHead; sourceIndex >= 0; sourceIndex = _orderNext[sourceIndex])
		{
			array[destinationIndex++] = new KeyValuePair<TKey, TValue>(_slots[sourceIndex].Key, _values[sourceIndex]);
		}
	}

	public bool Remove(TKey key)
	{
		var index = FindEntry(key, out var bucket);
		if (index < 0)
		{
			return false;
		}

		RemoveAt(index, bucket);
		return true;
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		var index = FindEntry(item.Key, out var bucket);
		if (index < 0 || !EqualityComparer<TValue>.Default.Equals(_values[index], item.Value))
		{
			return false;
		}

		RemoveAt(index, bucket);
		return true;
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		var version = _version;
		var i = _orderHead;
		while (i >= 0)
		{
			yield return new KeyValuePair<TKey, TValue>(_slots[i].Key, _values[i]);

			if (_version != version)
			{
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
			}

			i = _orderNext[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	// Returns the index of the entry for `key`, or -1, and always reports the key's bucket so callers
	// never have to hash it twice. Touches only _buckets and _slots.
	private int FindEntry(TKey key, out int bucket)
	{
		bucket = BucketIndex(_comparer, key, _buckets.Length);
		var slots = _slots;
		for (var i = _buckets[bucket] - 1; i >= 0; i = slots[i].HashNext)
		{
			if (_comparer.Equals(slots[i].Key, key))
			{
				return i;
			}
		}

		return -1;
	}

	private void SetCore(TKey key, TValue value, bool throwIfExists)
	{
		var existing = FindEntry(key, out var bucket);
		if (existing < 0)
		{
			Insert(key, value, bucket);
			return;
		}

		if (throwIfExists)
		{
			throw new ArgumentException($"An item with the key '{key}' has already been added.");
		}

		// Move-to-end on modify.
		// The value is written in place and only the order links move.
		// The key and hash chain are untouched, so _slots does not need copying here.
		EnsureValuesWritable(existing);
		_values[existing] = value;

		if (existing != _orderTail)
		{
			EnsureOrderNextWritable(_orderPrev[existing]);
			EnsureOrderNextWritable(existing);
			EnsureOrderNextWritable(_orderTail);

			OrderUnlink(existing);
			OrderAppend(existing);
		}

		_version++;
	}

	private void Insert(TKey key, TValue value, int bucket)
	{
		int index;
		if (_freeListCount > 0)
		{
			// Reusing a free slot never needs a copy. Either we have already copied since the last
			// snapshot (in which case the arrays are private anyway), or no visible slot has been
			// freed since the snapshot which means it is in neither the order list nor any bucket
			// chain the snapshot can walk.
			index = _freeListHead;
			_freeListHead = _slots[index].HashNext;
			_freeListCount--;
		}
		else
		{
			if (EntrySlotCount == _slots.Length)
			{
				Grow();
			}

			index = EntrySlotCount;
			EntrySlotCount++;
		}

		_slots[index].Key = key;
		_slots[index].HashNext = _buckets[bucket] - 1;
		_values[index] = value;
		_buckets[bucket] = index + 1;
		Count++;
		_version++;

		// This check is purely defensive. It never actually triggers a cloning of _orderNext as
		// that would mean _orderTail is behind a snapshot's tail which can only happen during a
		// remove or a move-to-end update and both of those operations clone _orderNext
		EnsureOrderNextWritable(_orderTail);
		OrderAppend(index);

		if (Count > _buckets.Length)
		{
			Rehash();
		}
	}

	private void RemoveAt(int index, int bucket)
	{
		var chainPrev = -1;
		for (var i = _buckets[bucket] - 1; i >= 0; i = _slots[i].HashNext)
		{
			if (i == index)
			{
				break;
			}

			chainPrev = i;
		}

		var orderPrev = _orderPrev[index];

		EnsureSlotsWritable(index);
		EnsureValuesWritable(index);
		EnsureOrderNextWritable(orderPrev);
		EnsureOrderNextWritable(index);

		if (chainPrev < 0)
		{
			_buckets[bucket] = _slots[index].HashNext + 1;
		}
		else
		{
			_slots[chainPrev].HashNext = _slots[index].HashNext;
		}

		OrderUnlink(index);

		_slots[index].Key = default!;
		_values[index] = default!;
		_orderNext[index] = -1;
		_orderPrev[index] = -1;

		_slots[index].HashNext = _freeListHead;
		_freeListHead = index;
		_freeListCount++;

		Count--;
		_version++;
	}

	private void OrderUnlink(int index)
	{
		var prev = _orderPrev[index];
		var next = _orderNext[index];

		if (prev >= 0)
		{
			_orderNext[prev] = next;
		}
		else
		{
			_orderHead = next;
		}

		if (next >= 0)
		{
			_orderPrev[next] = prev;
		}
		else
		{
			_orderTail = prev;
		}
	}

	private void OrderAppend(int index)
	{
		_orderPrev[index] = _orderTail;
		_orderNext[index] = -1;

		if (_orderTail >= 0)
		{
			// Appending to _orderTail is always safe because snapshots stop reading at the count
			// they froze.
			_orderNext[_orderTail] = index;
		}
		else
		{
			_orderHead = index;
		}

		_orderTail = index;
	}

	// A snapshot can observe a slot's key, hash link and value only if the slot existed when it was
	// taken. Everything at or beyond _latestSnapshotEntrySlotCount is invisible to it.
	private void EnsureSlotsWritable(int index)
	{
		if (index < _latestSnapshotEntrySlotCount && _slotsEpoch <= _latestSnapshotEpoch)
		{
			var newSlots = new Slot[_slots.Length];
			Array.Copy(_slots, newSlots, _slots.Length);
			_slots = newSlots;

			_slotsEpoch = _epoch;
			CopyOnWriteCount++;
		}
	}

	private void EnsureValuesWritable(int index)
	{
		if (index < _latestSnapshotEntrySlotCount && _valuesEpoch <= _latestSnapshotEpoch)
		{
			var newValues = new TValue[_values.Length];
			Array.Copy(_values, newValues, _values.Length);
			_values = newValues;

			_valuesEpoch = _epoch;
			CopyOnWriteCount++;
		}
	}

	private void EnsureOrderNextWritable(int index)
	{
		// A snapshot stops after emitting the entry count it froze, so the frozen tail's forward link
		// is never read.
		if (index >= 0 && index != _latestSnapshotOrderTail &&
			index < _latestSnapshotEntrySlotCount && _orderNextEpoch <= _latestSnapshotEpoch)
		{
			var newOrderNext = new int[_orderNext.Length];
			Array.Copy(_orderNext, newOrderNext, _orderNext.Length);
			_orderNext = newOrderNext;

			_orderNextEpoch = _epoch;
			CopyOnWriteCount++;
		}
	}

	// No guards needed for _buckets, Slot.HashNext and _orderPrev. A SnapshotView is enumerate-only
	// and forward-only. It reads the key, the value, and the forward order link up to a maximum of
	// its frozen count.

	private void Grow()
	{
		var capacity = _slots.Length == 0 ? 4 : _slots.Length * 2;

		var slots = new Slot[capacity];
		Array.Copy(_slots, slots, EntrySlotCount);
		_slots = slots;

		var values = new TValue[capacity];
		Array.Copy(_values, values, EntrySlotCount);
		_values = values;

		var orderNext = new int[capacity];
		Array.Copy(_orderNext, orderNext, EntrySlotCount);
		_orderNext = orderNext;

		var orderPrev = new int[capacity];
		Array.Copy(_orderPrev, orderPrev, EntrySlotCount);
		_orderPrev = orderPrev;

		_slotsEpoch = _epoch;
		_valuesEpoch = _epoch;
		_orderNextEpoch = _epoch;
	}

	// Re-partitions every entry across a larger bucket array. This rewrites only hash links and the
	// bucket array itself, neither of which a snapshot reads, so it needs no copy at all.
	private void Rehash()
	{
		var bucketCount = _buckets.Length * 2;
		var buckets = new int[bucketCount];

		for (var i = _orderHead; i >= 0; i = _orderNext[i])
		{
			var bucket = BucketIndex(_comparer, _slots[i].Key, bucketCount);
			_slots[i].HashNext = buckets[bucket] - 1;
			buckets[bucket] = i + 1;
		}

		_buckets = buckets;
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
		var versionBefore = _version;

		var ordered = new List<KeyValuePair<TKey, TValue>>(Count);
		for (var i = _orderHead; i >= 0; i = _orderNext[i])
		{
			ordered.Add(new KeyValuePair<TKey, TValue>(_slots[i].Key, _values[i]));
		}

		InitializeEmpty(BucketCountFor(ordered.Count), CapacityFor(ordered.Count));

		foreach (var entry in ordered)
		{
			Insert(entry.Key, entry.Value, BucketIndex(_comparer, entry.Key, _buckets.Length));
		}

		// Undo version increments caused by Insert above
		_version = versionBefore;
	}

	private sealed class KeyCollection : ICollection<TKey>, ICollection
	{
		private readonly SequencedSnapshotDictionary<TKey, TValue> _owner;

		public KeyCollection(SequencedSnapshotDictionary<TKey, TValue> owner)
		{
			_owner = owner;
		}

		public int Count => _owner.Count;
		bool ICollection<TKey>.IsReadOnly => true;

		bool ICollection<TKey>.Contains(TKey item)
		{
			return _owner.ContainsKey(item);
		}

		public void CopyTo(TKey[] array, int arrayIndex)
		{
			if (array is null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}

			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(array));
			}

			var i = arrayIndex;
			for (var e = _owner._orderHead; e >= 0; e = _owner._orderNext[e])
			{
				array[i++] = _owner._slots[e].Key;
			}
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			var version = _owner._version;
			var i = _owner._orderHead;
			while (i >= 0)
			{
				if (_owner._version != version)
				{
					throw new InvalidOperationException("Collection was modified during enumeration.");
				}

				var next = _owner._orderNext[i];
				yield return _owner._slots[i].Key;

				if (_owner._version != version)
				{
					throw new InvalidOperationException("Collection was modified during enumeration.");
				}

				i = next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException("Keys collection is read-only.");
		void ICollection<TKey>.Clear() => throw new NotSupportedException("Keys collection is read-only.");
		bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException("Keys collection is read-only.");

		void ICollection.CopyTo(Array array, int index)
		{
			_owner.KeyCopyTo(array, index);
		}

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => _owner;
	}

	private sealed class ValueCollection : ICollection<TValue>, ICollection
	{
		private readonly SequencedSnapshotDictionary<TKey, TValue> _owner;

		public ValueCollection(SequencedSnapshotDictionary<TKey, TValue> owner)
		{
			_owner = owner;
		}

		public int Count => _owner.Count;
		bool ICollection<TValue>.IsReadOnly => true;

		bool ICollection<TValue>.Contains(TValue item)
		{
			var valueComparer = EqualityComparer<TValue>.Default;

			for (var i = _owner._orderHead; i >= 0; i = _owner._orderNext[i])
			{
				if (valueComparer.Equals(_owner._values[i], item))
				{
					return true;
				}
			}

			return false;
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			if (array is null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}

			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.", nameof(array));
			}

			var i = arrayIndex;
			for (var e = _owner._orderHead; e >= 0; e = _owner._orderNext[e])
			{
				array[i++] = _owner._values[e];
			}
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			var version = _owner._version;
			var i = _owner._orderHead;
			while (i >= 0)
			{
				if (_owner._version != version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}

				var next = _owner._orderNext[i];
				yield return _owner._values[i];

				if (_owner._version != version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}

				i = next;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException("Values collection is read-only.");
		void ICollection<TValue>.Clear() => throw new NotSupportedException("Values collection is read-only.");
		bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException("Values collection is read-only.");

		void ICollection.CopyTo(Array array, int index)
		{
			_owner.ValueCopyTo(array, index);
		}

		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => _owner;
	}

	/// <summary>
	/// A read-only, insertion-ordered view of a <see cref="SequencedSnapshotDictionary{TKey,TValue}"/>
	/// frozen at the moment <see cref="SequencedSnapshotDictionary{TKey,TValue}.GetSnapshot"/> was
	/// called.
	/// </summary>
	private sealed class SnapshotView : ISnapshotView<TKey, TValue>
	{
		private readonly SequencedSnapshotDictionary<TKey, TValue> _owner;
		private readonly Slot[] _slots;
		private readonly TValue[] _values;
		private readonly int[] _orderNext;
		private readonly int _head;
		private readonly int _count;
		private bool _disposed;

		internal SnapshotView(
			SequencedSnapshotDictionary<TKey, TValue> owner,
			Slot[] slots,
			TValue[] values,
			int[] orderNext,
			int head,
			int count)
		{
			_owner = owner;
			_slots = slots;
			_values = values;
			_orderNext = orderNext;
			_head = head;
			_count = count;
		}

		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return _count;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			ThrowIfDisposed();
			return EnumerateEntries();
		}

		private IEnumerator<KeyValuePair<TKey, TValue>> EnumerateEntries()
		{
			// Walk exactly the number of entries that were live at the freeze and no further. That
			// bound is what makes appends after the snapshot free: the last entry emitted is the
			// frozen tail, whose forward link is the only one an append rewrites, and it is never read.
			var i = _head;
			for (var emitted = 0; emitted < _count && i >= 0; emitted++)
			{
				ThrowIfDisposed();
				yield return new KeyValuePair<TKey, TValue>(_slots[i].Key, _values[i]);
				i = _orderNext[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(nameof(SnapshotView));
			}
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;
			_owner.ReleaseSnapshot();
		}
	}

	private static int BucketIndex(IEqualityComparer<TKey> comparer, TKey key, int bucketCount)
	{
		// MurmurHash3 avalanche to spread more evenly.
		var h = unchecked((uint) comparer.GetHashCode(key));
		unchecked
		{
			h ^= h >> 16;
			h *= 0x85ebca6bu;
			h ^= h >> 13;
			h *= 0xc2b2ae35u;
			h ^= h >> 16;
		}
		return (int) (h & (uint) (bucketCount - 1));
	}

	// --- IDictionary explicit members ---
	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool IDictionary.IsFixedSize => false;

	bool IDictionary.IsReadOnly => false;

	ICollection IDictionary.Keys => KeysInternal;

	ICollection IDictionary.Values => ValuesInternal;

	object IDictionary.this[object key]
	{
		get
		{
			if (IsCompatibleKey(key))
			{
				if (TryGetValue((TKey) key, out var value))
				{
					return value;
				}
			}

			return null;
		}
		set
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (default(TKey) != null && value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			try
			{
				var tempKey = (TKey) key;
				try
				{
					this[tempKey] = (TValue) value!;
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(nameof(value));
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(nameof(key));
			}
		}
	}

	private static bool IsCompatibleKey(object key)
	{
		if (key == null)
		{
			throw new ArgumentNullException(nameof(key));
		}
		return key is TKey;
	}

	void IDictionary.Add(object key, object value)
	{
		if (key == null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		if (default(TKey) != null && value == null)
		{
			throw new ArgumentNullException(nameof(value));
		}

		try
		{
			var tempKey = (TKey) key;

			try
			{
				Add(tempKey, (TValue) value!);
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(nameof(value));
			}
		}
		catch (InvalidCastException)
		{
			throw new ArgumentException(nameof(key));
		}
	}

	bool IDictionary.Contains(object key)
	{
		if (IsCompatibleKey(key))
		{
			return ContainsKey((TKey) key);
		}

		return false;
	}

	IDictionaryEnumerator IDictionary.GetEnumerator() => new DictionaryEnumerator(this);

	private sealed class DictionaryEnumerator : IDictionaryEnumerator
	{
		private readonly SequencedSnapshotDictionary<TKey, TValue> _owner;
		private readonly int _version;
		private int _current;

		public DictionaryEnumerator(SequencedSnapshotDictionary<TKey, TValue> owner)
		{
			_owner = owner;
			_version = owner._version;
			_current = owner._orderHead;
		}

		public bool MoveNext()
		{
			if (_current < 0)
			{
				return false;
			}

			Entry = new DictionaryEntry(_owner._slots[_current].Key, _owner._values[_current]);

			if (_version != _owner._version)
			{
				throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
			}

			_current = _owner._orderNext[_current];

			return true;
		}

		public void Reset()
		{
			if (_version != _owner._version)
			{
				throw new InvalidOperationException("Dictionary has changed. Cannot reset.");
			}

			_current = _owner._orderHead;
		}

		public object Current => Entry;

		public object Key => Entry.Key;

		public object Value => Entry.Value;

		public DictionaryEntry Entry { get; private set; }
	}

	void IDictionary.Remove(object key)
	{
		if (IsCompatibleKey(key))
		{
			Remove((TKey) key);
		}
	}

	void ICollection.CopyTo(Array array, int index)
	{
		KeyCopyTo(array, index);
	}

	private void KeyCopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (array.Rank != 1)
		{
			throw new ArgumentException("Multi dimension array not supported", nameof(array));
		}

		if (array.GetLowerBound(0) != 0)
		{
			throw new ArgumentException("Non-zero lower bound not supported", nameof(array));
		}

		if ((uint) index > (uint) array.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		if (array.Length - index < Count)
		{
			throw new ArgumentException("The array is too small to copy the elements.", nameof(array));
		}

		if (array is TKey[] keys)
		{
			var destinationIndex = index;
			for (var sourceIndex = _orderHead; sourceIndex >= 0; sourceIndex = _orderNext[sourceIndex])
			{
				keys[destinationIndex++] = _slots[sourceIndex].Key;
			}

			return;
		}

		if (array is not object[] objects)
		{
			throw new ArgumentException("Invalid array type", nameof(array));
		}

		try
		{
			var destinationIndex = index;
			for (var sourceIndex = _orderHead; sourceIndex >= 0; sourceIndex = _orderNext[sourceIndex])
			{
				objects[destinationIndex++] = _slots[sourceIndex].Key;
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Invalid array type", nameof(array));
		}
	}

	private void ValueCopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (array.Rank != 1)
		{
			throw new ArgumentException("Multi dimension array not supported", nameof(array));
		}

		if (array.GetLowerBound(0) != 0)
		{
			throw new ArgumentException("Non-zero lower bound not supported", nameof(array));
		}

		if ((uint) index > (uint) array.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		if (array.Length - index < Count)
		{
			throw new ArgumentException("The array is too small to copy the elements.", nameof(array));
		}

		if (array is TValue[] value)
		{
			var destinationIndex = index;
			for (var sourceIndex = _orderHead; sourceIndex >= 0; sourceIndex = _orderNext[sourceIndex])
			{
				value[destinationIndex++] = _values[sourceIndex];
			}

			return;
		}

		if (array is not object[] objects)
		{
			throw new ArgumentException("Invalid array type", nameof(array));
		}

		try
		{
			var destinationIndex = index;
			for (var sourceIndex = _orderHead; sourceIndex >= 0; sourceIndex = _orderNext[sourceIndex])
			{
				objects[destinationIndex++] = _values[sourceIndex];
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Invalid array type", nameof(array));
		}
	}
}
