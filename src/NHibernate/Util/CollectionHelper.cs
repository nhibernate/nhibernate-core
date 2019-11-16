using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iesi.Collections.Generic;

namespace NHibernate.Util
{
	public static class CollectionHelper
	{
		[Serializable]
		private class EmptyEnumerator : IDictionaryEnumerator
		{
			public object Key
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Key"); }
			}

			public object Value
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Value"); }
			}

			public DictionaryEntry Entry
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Entry"); }
			}

			public void Reset()
			{
			}

			public object Current
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Current"); }
			}

			public bool MoveNext()
			{
				return false;
			}
		}

		[Serializable]
		private class EmtpyAsyncEnumerator<T> : IAsyncEnumerator<T>
		{
			public T Current => throw new InvalidOperationException("EmtpyAsyncEnumerator.get_Current");

			public ValueTask DisposeAsync()
			{
				return default;
			}

			public ValueTask<bool> MoveNextAsync()
			{
				return new ValueTask<bool>(false);
			}
		}

		private class EmptyEnumerableClass : IEnumerable
		{
			public IEnumerator GetEnumerator()
			{
				return new EmptyEnumerator();
			}
		}

		private class EmptyAsyncEnumerableClass : IAsyncEnumerable<object>
		{
			public IAsyncEnumerator<object> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			{
				return new EmtpyAsyncEnumerator<object>();
			}
		}

		/// <summary>
		/// A read-only dictionary that is always empty and permits lookup by <see langword="null" /> key.
		/// </summary>
		[Serializable]
		private class EmptyMapClass : IDictionary
		{
			private static readonly EmptyEnumerator emptyEnumerator = new EmptyEnumerator();

			public bool Contains(object key)
			{
				return false;
			}

			public void Add(object key, object value)
			{
				throw new NotSupportedException("EmptyMap.Add");
			}

			public void Clear()
			{
				throw new NotSupportedException("EmptyMap.Clear");
			}

			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return emptyEnumerator;
			}

			public void Remove(object key)
			{
				throw new NotSupportedException("EmptyMap.Remove");
			}

			public object this[object key]
			{
				get { return null; }
				set { throw new NotSupportedException("EmptyMap.set_Item"); }
			}

			public ICollection Keys
			{
				get { return this; }
			}

			public ICollection Values
			{
				get { return this; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool IsFixedSize
			{
				get { return true; }
			}

			public void CopyTo(Array array, int index)
			{
			}

			public int Count
			{
				get { return 0; }
			}

			public object SyncRoot
			{
				get { return this; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public IEnumerator GetEnumerator()
			{
				return emptyEnumerator;
			}
		}

		// To be removed in v6.0
		[Serializable]
		private class EmptyListClass : IList
		{
			public int Add(object value)
			{
				throw new NotImplementedException();
			}

			public bool Contains(object value)
			{
				return false;
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public int IndexOf(object value)
			{
				return -1;
			}

			public void Insert(int index, object value)
			{
				throw new NotImplementedException();
			}

			public void Remove(object value)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public object this[int index]
			{
				get { throw new IndexOutOfRangeException(); }
				set { throw new IndexOutOfRangeException(); }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public bool IsFixedSize
			{
				get { return true; }
			}

			public void CopyTo(Array array, int index)
			{
			}

			public int Count
			{
				get { return 0; }
			}

			public object SyncRoot
			{
				get { return this; }
			}

			public bool IsSynchronized
			{
				get { return false; }
			}

			public IEnumerator GetEnumerator()
			{
				return new EmptyEnumerator();
			}
		}

		public static readonly IEnumerable EmptyEnumerable = new EmptyEnumerableClass();
		public static readonly IAsyncEnumerable<object> EmptyAsyncEnumerable = new EmptyAsyncEnumerableClass();
		public static readonly IDictionary EmptyMap = new EmptyMapClass();

		public static IDictionary<TKey, TValue> EmptyDictionary<TKey, TValue>()
		{
			return EmptyMapClass<TKey, TValue>.Instance;
		}

		internal static ISet<T> EmptySet<T>() => EmptyReadOnlySet<T>.Instance;

		public static readonly ICollection EmptyCollection = EmptyMap;
		// Since v5
		[Obsolete("It has no more usages in NHibernate and will be removed in a future version.")]
		public static readonly IList EmptyList = new EmptyListClass();

		// Obsolete since v5
		/// <summary>
		/// Determines if two collections have equals elements, with the same ordering.
		/// </summary>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <returns><c>true</c> if collection are equals, <c>false</c> otherwise.</returns>
		[Obsolete("It has no more usages in NHibernate and will be removed in a future version.")]
		public static bool CollectionEquals(ICollection c1, ICollection c2)
		{
			if (c1 == c2)
			{
				return true;
			}

			if (c1 == null || c2 == null)
			{
				return false;
			}

			if (c1.Count != c2.Count)
			{
				return false;
			}

			var e2 = c2.GetEnumerator();
			try
			{
				foreach (var item1 in c1)
				{
					e2.MoveNext();
					if (!Equals(item1, e2.Current))
					{
						return false;
					}
				}
			}
			finally
			{
				// Most IEnumerator will have a disposable concrete implementation, must check it.
				(e2 as IDisposable)?.Dispose();
			}

			return true;
		}

		// Since v5
		[Obsolete("It has no more usages in NHibernate and will be removed in a future version.")]
		public static bool DictionaryEquals(IDictionary a, IDictionary b)
		{
			if (Equals(a, b))
			{
				return true;
			}

			if (a == null || b == null)
			{
				return false;
			}

			if (a.Count != b.Count)
			{
				return false;
			}

			foreach (object key in a.Keys)
			{
				if (!Equals(a[key], b[key]))
				{
					return false;
				}
			}

			return true;
		}

		// Obsolete since v5
		/// <summary>
		/// Computes a hash code for <paramref name="coll"/>.
		/// </summary>
		/// <remarks>The hash code is computed as the sum of hash codes of individual elements
		/// plus a length of the collection, so that the value is independent of the
		/// collection iteration order.
		/// </remarks>
		[Obsolete("It has no more usages in NHibernate and will be removed in a future version.")]
		public static int GetHashCode(IEnumerable coll)
		{
			var result = 0;

			foreach (var obj in coll)
			{
				unchecked
				{
					if (obj != null)
						result += obj.GetHashCode();
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// Creates a <see cref="Hashtable" /> that uses case-insensitive string comparison
		/// associated with invariant culture.
		/// </summary>
		/// <remarks>
		/// This is different from the method in <see cref="System.Collections.Specialized.CollectionsUtil" />
		/// in that the latter uses the current culture and is thus vulnerable to the "Turkish I" problem.
		/// </remarks>
		public static IDictionary<string, T> CreateCaseInsensitiveHashtable<T>()
		{
			return new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Creates a <see cref="Hashtable" /> that uses case-insensitive string comparison
		/// associated with invariant culture.
		/// </summary>
		/// <remarks>
		/// This is different from the method in <see cref="System.Collections.Specialized.CollectionsUtil" />
		/// in that the latter uses the current culture and is thus vulnerable to the "Turkish I" problem.
		/// </remarks>
		public static IDictionary<string, T> CreateCaseInsensitiveHashtable<T>(IDictionary<string, T> dictionary)
		{
			return new Dictionary<string, T>(dictionary, StringComparer.OrdinalIgnoreCase);
		}

		// ~~~~~~~~~~~~~~~~~~~~~~ Generics ~~~~~~~~~~~~~~~~~~~~~~
		[Serializable]
		private class EmptyEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			#region IEnumerator<KeyValuePair<TKey,TValue>> Members

			KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
			{
				get
				{
					throw new InvalidOperationException(
						string.Format("EmptyEnumerator<{0},{1}>.KeyValuePair", typeof(TKey), typeof(TValue)));
				}
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			public bool MoveNext()
			{
				return false;
			}

			public void Reset()
			{
			}

			public object Current
			{
				get
				{
					throw new InvalidOperationException(
						string.Format("EmptyEnumerator<{0},{1}>.Current", typeof(TKey), typeof(TValue)));
				}
			}

			#endregion
		}

		[Serializable]
		public class EmptyEnumerableClass<T> : IEnumerable<T>
		{
			#region IEnumerable<T> Members

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return new EmptyEnumerator<T>();
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return ((IEnumerable<T>)this).GetEnumerator();
			}

			#endregion
		}

		[Serializable]
		internal class EmptyAsyncEnumerableClass<T> : IAsyncEnumerable<T>
		{
			public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
			{
				return new EmtpyAsyncEnumerator<T>();
			}
		}

		[Serializable]
		private class EmptyEnumerator<T> : IEnumerator<T>
		{
			#region IEnumerator<T> Members

			T IEnumerator<T>.Current
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Current"); }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
			}

			#endregion

			#region IEnumerator Members

			public bool MoveNext()
			{
				return false;
			}

			public void Reset()
			{
			}

			public object Current
			{
				get { throw new InvalidOperationException("EmptyEnumerator.get_Current"); }
			}

			#endregion
		}

		[Serializable]
		private class EmptyReadOnlySet<T>
		{
			public static readonly ISet<T> Instance = new ReadOnlySet<T>(new HashSet<T>());
		}

		/// <summary>
		/// A read-only dictionary that is always empty and permits lookup by <see langword="null" /> key.
		/// </summary>
		[Serializable]
		public class EmptyMapClass<TKey, TValue> : IDictionary<TKey, TValue>
		{
#pragma warning disable 618 // Constructor is obsolete, to be switched to non-obsolete but private.
			internal static readonly IDictionary<TKey, TValue> Instance = new EmptyMapClass<TKey, TValue>();
#pragma warning restore 618

			private static readonly EmptyEnumerator<TKey, TValue> emptyEnumerator = new EmptyEnumerator<TKey, TValue>();

			// Since v5.1. To be switched to private.
			[Obsolete("Please use CollectionHelper.EmptyDictionary<TKey, TValue>() instead.")]
			public EmptyMapClass()
			{
			}

			#region IDictionary<TKey,TValue> Members

			public bool ContainsKey(TKey key)
			{
				return false;
			}

			public void Add(TKey key, TValue value)
			{
				throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.Add", typeof(TKey), typeof(TValue)));
			}

			public bool Remove(TKey key)
			{
				throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.Remove", typeof(TKey), typeof(TValue)));
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				value = default(TValue);
				return false;
			}

			public TValue this[TKey key]
			{
				get { return default(TValue); }
				set { throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.set_Item", typeof(TKey), typeof(TValue))); }
			}

			public ICollection<TKey> Keys
			{
				get { return Array.Empty<TKey>(); }
			}

			public ICollection<TValue> Values
			{
				get { return Array.Empty<TValue>(); }
			}

			#endregion

			#region ICollection<KeyValuePair<TKey,TValue>> Members

			public void Add(KeyValuePair<TKey, TValue> item)
			{
				throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.Add", typeof(TKey), typeof(TValue)));
			}

			public void Clear()
			{
				throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.Clear", typeof(TKey), typeof(TValue)));
			}

			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				return false;
			}

			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
			}

			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				throw new NotSupportedException(string.Format("EmptyMapClass<{0},{1}>.Remove", typeof(TKey), typeof(TValue)));
			}

			public int Count
			{
				get { return 0; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			#endregion

			#region IEnumerable<KeyValuePair<TKey,TValue>> Members

			IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
			{
				return emptyEnumerator;
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
			}

			#endregion
		}

		/// <summary>
		/// Computes a hash code for <paramref name="coll"/>.
		/// </summary>
		/// <remarks>The hash code is computed as the sum of hash codes of individual elements
		/// plus a length of the collection, so that the value is independent of the
		/// collection iteration order.
		/// </remarks>
		public static int GetHashCode<T>(IEnumerable<T> coll)
		{
			var result = 0;

			foreach (var obj in coll)
			{
				unchecked
				{
					if (!ReferenceEquals(obj, null))
						result += obj.GetHashCode();
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// Computes a hash code for <paramref name="coll"/>.
		/// </summary>
		/// <remarks>The hash code is computed as the sum of hash codes of individual elements
		/// plus a length of the collection, so that the value is independent of the
		/// collection iteration order.
		/// </remarks>
		public static int GetHashCode<T>(IEnumerable<T> coll, IEqualityComparer<T> comparer)
		{
			var result = 0;

			foreach (var obj in coll)
			{
				unchecked
				{
					if (!ReferenceEquals(obj, null))
						result += comparer.GetHashCode(obj);
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// Determines if two sets have equal elements. Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="s1">The first set.</param>
		/// <param name="s2">The second set.</param>
		/// <returns><c>true</c> if sets are equals, <c>false</c> otherwise.</returns>
		public static bool SetEquals<T>(ISet<T> s1, ISet<T> s2)
			=> FastCheckEquality(s1, s2) ?? s1.SetEquals(s2);

		// Obsolete since v5
		/// <summary>
		/// Determines if two collections have equals elements, with the same ordering.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <returns><c>true</c> if collections are equals, <c>false</c> otherwise.</returns>
		[Obsolete("Please use SequenceEquals instead.")]
		public static bool CollectionEquals<T>(ICollection<T> c1, ICollection<T> c2)
			=> SequenceEquals(c1, c2);

		/// <summary>
		/// Determines if two collections have equals elements, with the same ordering. Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <returns><c>true</c> if collections are equals, <c>false</c> otherwise.</returns>
		public static bool SequenceEquals<T>(IEnumerable<T> c1, IEnumerable<T> c2)
			=> SequenceEquals(c1, c2, null);

		/// <summary>
		/// Determines if two collections have equals elements, with the same ordering. Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <param name="comparer">The element comparer.</param>
		/// <returns><c>true</c> if collections are equals, <c>false</c> otherwise.</returns>
		public static bool SequenceEquals<T>(IEnumerable<T> c1, IEnumerable<T> c2, IEqualityComparer<T> comparer)
			=> FastCheckEquality(c1, c2) ?? c1.SequenceEqual(c2, comparer);

		/// <summary>
		/// Determines if two collections have the same elements with the same duplication count, whatever their ordering.
		/// Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <returns><c>true</c> if collections are equals, <c>false</c> otherwise.</returns>
		public static bool BagEquals<T>(IEnumerable<T> c1, IEnumerable<T> c2)
			=> BagEquals(c1, c2, null);

		/// <summary>
		/// Determines if two collections have the same elements with the same duplication count, whatever their ordering.
		/// Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="c1">The first collection.</param>
		/// <param name="c2">The second collection.</param>
		/// <param name="comparer">The element comparer.</param>
		/// <returns><c>true</c> if collections are equals, <c>false</c> otherwise.</returns>
		public static bool BagEquals<T>(IEnumerable<T> c1, IEnumerable<T> c2, IEqualityComparer<T> comparer)
		{
			var result = FastCheckEquality(c1, c2);
			if (result.HasValue)
				return result.Value;
			var l2 = c2.ToLookup(e => e, comparer);
			// Lookups return an empty sequence if a key is missing, no need to test if it contains it.
			return c1.ToLookup(e => e, comparer).All(g => g.Count() == l2[g.Key].Count());
		}

		/// <summary>
		/// Determines if two maps have the same key-values. Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="K">The type of the keys.</typeparam>
		/// <typeparam name="V">The type of the values.</typeparam>
		/// <param name="m1">The first map.</param>
		/// <param name="m2">The second map.</param>
		/// <returns><c>true</c> if maps are equals, <c>false</c> otherwise.</returns>
		public static bool DictionaryEquals<K, V>(IDictionary<K, V> m1, IDictionary<K, V> m2)
			=> DictionaryEquals(m1, m2, null);

		/// <summary>
		/// Determines if two maps have the same key-values. Supports <c>null</c> arguments.
		/// </summary>
		/// <typeparam name="K">The type of the keys.</typeparam>
		/// <typeparam name="V">The type of the values.</typeparam>
		/// <param name="m1">The first map.</param>
		/// <param name="m2">The second map.</param>
		/// <param name="comparer">The value comparer.</param>
		/// <returns><c>true</c> if maps are equals, <c>false</c> otherwise.</returns>
		public static bool DictionaryEquals<K, V>(IDictionary<K, V> m1, IDictionary<K, V> m2, IEqualityComparer<V> comparer)
			=> FastCheckEquality(m1, m2) ??
				(comparer == null ? DictionaryEquals(m1, m2, EqualityComparer<V>.Default) :
					m1.All(kv => m2.TryGetValue(kv.Key, out var value) && comparer.Equals(kv.Value, value)));

		private static bool? FastCheckEquality<T>(IEnumerable<T> c1, IEnumerable<T> c2)
		{
			if (c1 == c2)
			{
				return true;
			}

			if (c1 == null || c2 == null)
			{
				return false;
			}

			if (c1.Count() != c2.Count())
			{
				return false;
			}

			// Requires elements comparison.
			return null;
		}
	}
}
