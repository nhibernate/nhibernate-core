using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Returns an unmodifiable view of the specified IDictionary. 
	/// This method allows modules to provide users with "read-only" access to internal dictionary. 
	/// Query operations on the returned dictionary "read through" to the specified dictionary, 
	/// and attempts to modify the returned dictionary, 
	/// whether direct or via its collection views, result in an <see cref="NotSupportedException"/>.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	[Serializable]
	public class UnmodifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> dictionary;

		/// <summary>
		/// Initializes a new instance of the UnmodifiableDictionary class that contains elements wrapped
		/// from the specified IDictionary. 
		/// </summary>
		/// <param name="dictionary">The <see cref="IDictionary{TK,TV}"/>  whose elements are wrapped.</param>
		public UnmodifiableDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}

		#region IDictionary<TKey,TValue> Members

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		public void Add(TKey key, TValue value)
		{
			throw new NotSupportedException();
		}

		public bool Remove(TKey key)
		{
			throw new NotSupportedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get { return dictionary[key]; }
			set { throw new NotSupportedException(); }
		}

		public ICollection<TKey> Keys
		{
			get { return dictionary.Keys; }
		}

		public ICollection<TValue> Values
		{
			get { return dictionary.Values; }
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			dictionary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>) this).GetEnumerator();
		}

		#endregion
	}
}