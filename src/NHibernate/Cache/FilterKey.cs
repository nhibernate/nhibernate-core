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

		public FilterKey(string name, IEnumerable<KeyValuePair<string, object>> @params, IDictionary<string, IType> types, EntityMode entityMode)
		{
			filterName = name;
			foreach (KeyValuePair<string, object> me in @params)
			{
				IType type = types[me.Key];
				filterParameters[me.Key] = new TypedValue(type, me.Value, entityMode);
			}
		}

		public override int GetHashCode()
		{
			int result = 13;
			result = 37 * result + filterName.GetHashCode();
			result = 37 * result + CollectionHelper.GetHashCode(filterParameters);
			return result;
		}

		public override bool Equals(object other)
		{
			var that = other as FilterKey;
			if (that == null)
			{
				return false;
			}
			if (!that.filterName.Equals(filterName))
				return false;
			if (!CollectionHelper.DictionaryEquals<string, TypedValue>(that.filterParameters, filterParameters))
				return false;
			return true;
		}

		public override string ToString()
		{
			return string.Format("FilterKey[{0}{1}]", filterName, CollectionPrinter.ToString(filterParameters));
		}

		public static ISet CreateFilterKeys(IDictionary<string, IFilter> enabledFilters, EntityMode entityMode)
		{
			if (enabledFilters.Count == 0)
				return null;
			Set result = new HashedSet();
			foreach (FilterImpl filter in enabledFilters.Values)
			{
				FilterKey key = new FilterKey(filter.Name, filter.Parameters, filter.FilterDefinition.ParameterTypes, entityMode);
				result.Add(key);
			}
			return result;
		}
	}
}