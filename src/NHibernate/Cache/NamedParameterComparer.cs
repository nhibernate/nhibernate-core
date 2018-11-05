using System;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Cache
{
	internal class NamedParameterComparer : IEqualityComparer<KeyValuePair<string, TypedValue>>
	{
		public static readonly NamedParameterComparer Instance = new NamedParameterComparer();

		public bool Equals(KeyValuePair<string, TypedValue> x, KeyValuePair<string, TypedValue> y)
		{
			return StringComparer.Ordinal.Equals(x.Key, y.Key) &&
			       (ReferenceEquals(x.Value, y.Value) || x.Value != null && y.Value != null && x.Value.Equals(y.Value));
		}

		public int GetHashCode(KeyValuePair<string, TypedValue> obj)
		{
			unchecked
			{
				return 397 * StringComparer.Ordinal.GetHashCode(obj.Key) ^
				       (obj.Value != null ? obj.Value.GetHashCode() : 0);
			}
		}
	}
}
