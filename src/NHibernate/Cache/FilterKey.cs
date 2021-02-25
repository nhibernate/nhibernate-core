using System;
using System.Collections.Generic;
using System.Linq;
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

		// Sets and dictionaries are populated last during deserialization, causing them to be potentially empty
		// during the deserialization callback. This causes them to be unreliable when used in hashcode or equals
		// computations. These computations occur during the deserialization callback for example when another
		// serialized set or dictionary contain an instance of this class. (QueryKey was having such a dictionary
		// or set.)
		// So better serialize them as other structures, so long for Equals implementation which actually needs a
		// dictionary.
		private readonly KeyValuePair<string, TypedValue>[] _filterParameters;

		// Since v5.2
		[Obsolete("Use overload taking a FilterImpl")]
		public FilterKey(string name, IEnumerable<KeyValuePair<string, object>> @params, IDictionary<string, IType> types)
		{
			_filterName = name;
			_filterParameters = @params.Select(
				p => new KeyValuePair<string, TypedValue>(
					p.Key,
					new TypedValue(types[p.Key], p.Value))).ToArray();
		}

		public FilterKey(FilterImpl filter)
		{
			var types = filter.FilterDefinition.ParameterTypes;
			_filterName = filter.Name;
			_filterParameters = filter.Parameters.Select(
				p => new KeyValuePair<string, TypedValue>(
					p.Key,
					new TypedValue(
						types[p.Key],
						p.Value,
						filter.GetParameterSpan(p.Key) != null))).ToArray();
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
			if (that == null || !that._filterName.Equals(_filterName))
				return false;

			// BagEquals is less efficient than a DictionaryEquals, but serializing dictionaries causes issues on
			// deserialization if GetHashCode or Equals are called in its deserialization callback. And building
			// dictionaries on the fly will in most cases be worst than BagEquals, unless re-coding its short-circuits.
			return CollectionHelper.BagEquals(
				_filterParameters,
				that._filterParameters,
				NamedParameterComparer.Instance);
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
