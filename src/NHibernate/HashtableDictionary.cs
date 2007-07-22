using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate
{
	/// <summary>
	/// An implementation of <see cref="IDictionary<K,V>" /> that retains the behavior of
	/// <see cref="Hashtable" /> when accessing the <see cref="this[K]" /> property.
	/// </summary>
	[Serializable]
	public class HashtableDictionary<K, V> : IDictionary<K, V> where V : class
	{
		private readonly IDictionary<K, V> source;

		public HashtableDictionary()
		{
			source = new Dictionary<K, V>();
		}

		public HashtableDictionary(int capacity)
		{
			source = new Dictionary<K, V>(capacity);
		}

		public HashtableDictionary(IEqualityComparer<K> comparer)
		{
			source = new Dictionary<K, V>(comparer);
		}

		public HashtableDictionary(int capacity, IEqualityComparer<K> comparer)
		{
			source = new Dictionary<K, V>(capacity, comparer);
		}

		public HashtableDictionary(IDictionary<K, V> dictionary)
		{
			source = new Dictionary<K, V>(dictionary);
		}

		public HashtableDictionary(IDictionary<K, V> dictionary, IEqualityComparer<K> comparer)
		{
			source = new Dictionary<K, V>(dictionary, comparer);
		}

		public V this[K key]
		{
			// Here's the difference: Hashtable would just return null if it didn't contain that key. While
			// Dictionary<K,V> throws an exception.
			get { return source.ContainsKey(key) ? source[key] : null; }
			set { source[key] = value; }
		}

		public int Count
		{
			get { return source.Count; }
		}

		public bool IsReadOnly
		{
			get { return source.IsReadOnly; }
		}

		public ICollection<K> Keys
		{
			get { return source.Keys; }
		}

		public ICollection<V> Values
		{
			get { return source.Values; }
		}

		public void Add(K key, V value)
		{
			source.Add(key, value);
		}

		public void Add(KeyValuePair<K, V> item)
		{
			source.Add(item);
		}

		public void Clear()
		{
			source.Clear();
		}

		public bool ContainsKey(K key)
		{
			return source.ContainsKey(key);
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			return source.Contains(item);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			source.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return source.GetEnumerator();
		}

		public bool Remove(K key)
		{
			return source.Remove(key);
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			return source.Remove(item);
		}

		public bool TryGetValue(K key, out V value)
		{
			return source.TryGetValue(key, out value);
		}
	}
}