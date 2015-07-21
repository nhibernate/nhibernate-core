using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	public class NullableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey : class
	{
		private TValue _nullValue;
		private bool _gotNullValue;
		private readonly Dictionary<TKey, TValue> _dict;

		public NullableDictionary()
		{
			_dict = new Dictionary<TKey, TValue>();
		}

		public NullableDictionary(IEqualityComparer<TKey> comparer)
		{
			_dict = new Dictionary<TKey, TValue>(comparer);
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				return _gotNullValue;
			}
			else
			{
				return _dict.ContainsKey(key);
			}
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				_nullValue = value;
			}
			else
			{
				_dict[key] = value;
			}
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				if (_gotNullValue)
				{
					_nullValue = default(TValue);
					_gotNullValue = false;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return _dict.Remove(key);
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				if (_gotNullValue)
				{
					value = _nullValue;
					return true;
				}
				else
				{
					value = default(TValue);
					return false;
				}
			}
			else
			{
				return _dict.TryGetValue(key, out value);
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					return _nullValue;
				}
				else
				{
					TValue ret;

					_dict.TryGetValue(key, out ret);

					return ret;
				}
			}
			set
			{
				if (key == null)
				{
					_nullValue = value;
					_gotNullValue = true;
				}
				else
				{
					_dict[key] = value;
				}
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (_gotNullValue)
				{
					List<TKey> keys = new List<TKey>(_dict.Keys);
					keys.Add(null);
					return keys;
				}
				else
				{
					return _dict.Keys;
				}
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				if (_gotNullValue)
				{
					List<TValue> values = new List<TValue>(_dict.Values);
					values.Add(_nullValue);
					return values;
				}
				else
				{
					return _dict.Values;
				}
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (KeyValuePair<TKey, TValue> kvp in _dict)
			{
				yield return kvp;
			}

			if (_gotNullValue)
			{
				yield return new KeyValuePair<TKey, TValue>(null, _nullValue);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key == null)
			{
				_nullValue = item.Value;
				_gotNullValue = true;
			}
			else
			{
				_dict.Add(item.Key, item.Value);
			}
		}

		public void Clear()
		{
			_dict.Clear();
			_nullValue = default(TValue);
			_gotNullValue = false;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue val;

			if (TryGetValue(item.Key, out val))
			{
				return Equals(item.Value, val);
			}
			else
			{
				return false;
			}
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new System.NotImplementedException();
		}

		public int Count
		{
			get
			{
				if (_gotNullValue)
				{
					return _dict.Count + 1;
				}
				else
				{
					return _dict.Count;
				}
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
	}
}