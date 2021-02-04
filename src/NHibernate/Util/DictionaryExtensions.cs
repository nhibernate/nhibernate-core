#if NETSTANDARD2_0 || NETFX
using System.Collections.Generic;

namespace NHibernate.Util
{
	internal static class DictionaryExtensions
	{
		public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (dictionary.ContainsKey(key))
			{
				return false;
			}

			dictionary.Add(key, value);
			return true;
		}
	}
}
#endif
