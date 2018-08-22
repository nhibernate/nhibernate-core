using System;
using System.Collections.Generic;

namespace NHibernate.Util
{
	public static class DictionaryHelper
	{
		/// <summary>
		/// Gets element from dictionary, or default TKey value if element is not present
		/// </summary>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key)
		{
			TValue value;
			dic.TryGetValue(key, out value);
			return value;
		}

		/// <summary>
		/// Gets element from dictionary, or <param name="defaultValue"></param> value if element is not present
		/// </summary>
		/// <param name="dic">source dictionary</param>
		/// <param name="key">key to get element</param>
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue)
		{
			TValue value;
			return dic.TryGetValue(key, out value) ? value : defaultValue;
		}

		/// <summary>
		/// Gets element from dictionary, ads a value and returns it if key is not present
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(
			this IDictionary<TKey, TValue> dict,
			TKey key,
			Func<TKey, TValue> valueFactory)
		{

			TValue value;
			if (!dict.TryGetValue(key, out value))
			{
				value = valueFactory(key);
				dict.Add(key, value);
			}
			return value;
		}

		/// <summary>
		/// Gets element from dictionary, ads a value and returns it if key is not present
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary,
			TKey key,
			Func<TValue> valueFactory)
		{
			TValue value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = valueFactory();
				dictionary.Add(key, value);
			}
			return value;
		}

		/// <summary>
		/// Gets element from dictionary, ads a default type value and returns it if key is not present
		/// </summary>
		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			return dictionary.GetOrAdd(key, () => new TValue());
		}
	}
}
