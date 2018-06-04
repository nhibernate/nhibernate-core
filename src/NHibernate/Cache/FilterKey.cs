using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache
{
	[Serializable]
	public class FilterKey
	{
		private readonly string _filterName;
		private readonly Dictionary<string, TypedValue> _filterParameters = new Dictionary<string, TypedValue>();

		// Since v5.2
		[Obsolete("Use overload taking a FilterImpl")]
		public FilterKey(string name, IEnumerable<KeyValuePair<string, object>> @params, IDictionary<string, IType> types)
		{
			_filterName = name;
			foreach (KeyValuePair<string, object> me in @params)
			{
				IType type = types[me.Key];
				_filterParameters[me.Key] = new TypedValue(type, me.Value);
			}
		}

		public FilterKey(FilterImpl filter)
		{
			var types = filter.FilterDefinition.ParameterTypes;
			_filterName = filter.Name;
			foreach (var me in filter.Parameters)
			{
				var type = types[me.Key];
				_filterParameters[me.Key] = new TypedValue(type, me.Value, filter.GetParameterSpan(me.Key) != null);
			}
		}

		public override int GetHashCode()
		{
			var result = 13;
			result = 37 * result + _filterName.GetHashCode();
			result = 37 * result + CollectionHelper.GetHashCode(_filterParameters, NamedParameterComparer.Instance);
			return result;
		}

		public override bool Equals(object other)
		{
			var that = other as FilterKey;
			if (that == null)
			{
				return false;
			}
			if (!that._filterName.Equals(_filterName))
				return false;
			if (!CollectionHelper.DictionaryEquals<string, TypedValue>(that._filterParameters, _filterParameters))
				return false;
			return true;
		}

		public override string ToString()
		{
			return string.Format("FilterKey[{0}{1}]", _filterName, CollectionPrinter.ToString(_filterParameters));
		}

		public static ISet<FilterKey> CreateFilterKeys(IDictionary<string, IFilter> enabledFilters)
		{
			if (enabledFilters.Count == 0)
				return null;

			var result = new HashSet<FilterKey>();
			foreach (FilterImpl filter in enabledFilters.Values)
			{
				var key = new FilterKey(filter);
				result.Add(key);
			}
			return result;
		}
	}
}
