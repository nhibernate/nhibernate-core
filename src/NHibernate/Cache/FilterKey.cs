using System;
using System.Collections.Generic;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache
{
	[Serializable]
	public class FilterKey
	{
		private readonly string filterName;
		private readonly Dictionary<string, TypedValue> filterParameters = new Dictionary<string, TypedValue>();

		public FilterKey(string name, IDictionary<string, object> @params, IDictionary<string, IType> types)
		{
			filterName = name;
			foreach (KeyValuePair<string, object> me in @params)
			{
				IType type = types[me.Key];
				filterParameters[me.Key] = new TypedValue(type, me.Value);
			}
		}

		public override int GetHashCode()
		{
			int result = 13;
			result = 37 * result + filterName.GetHashCode();
			result = 37 * result + CollectionHelper.GetHashCode<KeyValuePair<string, TypedValue>>(filterParameters);
			return result;
		}

		public override bool Equals(object other)
		{
			if (!(other is FilterKey))
				return false;
			FilterKey that = (FilterKey) other;
			if (!that.filterName.Equals(filterName))
				return false;
			if (!CollectionHelper.DictionaryEquals(that.filterParameters, filterParameters))
				return false;
			return true;
		}

		public override string ToString()
		{
			return "FilterKey[" + filterName + filterParameters + ']';
		}

		public static ISet CreateFilterKeys(IDictionary<string, IFilter> enabledFilters)
		{
			if (enabledFilters.Count == 0)
				return null;
			Set result = new HashedSet();
			foreach (FilterImpl filter in enabledFilters.Values)
			{
				FilterKey key = new FilterKey(
					filter.Name,
					filter.Parameters,
					filter.FilterDefinition.ParameterTypes
					);
				result.Add(key);
			}
			return result;
		}
	}
}