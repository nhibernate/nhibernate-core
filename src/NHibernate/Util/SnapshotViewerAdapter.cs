using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util;

internal sealed class SnapshotViewAdapter<TKey, TValue> : ISnapshotView<TKey, TValue>
{
	private readonly IReadOnlyCollection<KeyValuePair<TKey, TValue>> _keyValuePairs;

	internal SnapshotViewAdapter(IReadOnlyCollection<KeyValuePair<TKey, TValue>> keyValuePairs)
	{
		_keyValuePairs = keyValuePairs;
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _keyValuePairs.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public int Count => _keyValuePairs.Count;
	public void Dispose() { }
}

internal sealed class DictionarySnapshotViewAdapter<TKey, TValue> : ISnapshotView<TKey, TValue>
{
	private readonly IDictionary _dictionary;

	internal DictionarySnapshotViewAdapter(IDictionary dictionary)
	{
		_dictionary = dictionary;
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		var enumerator = _dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			yield return new KeyValuePair<TKey, TValue>((TKey) enumerator.Key, (TValue) enumerator.Value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public int Count => _dictionary.Count;
	public void Dispose() { }
}
