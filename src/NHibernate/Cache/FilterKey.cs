using System;
using System.Collections;
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
		private string filterName;
		private IDictionary filterParameters = new Hashtable();

		public FilterKey(string name, IDictionary @params, IDictionary types)
		{
			filterName = name;
			foreach (DictionaryEntry me in @params)
			{
				IType type = (IType) types[me.Key];
				filterParameters[me.Key] = new TypedValue(type, me.Value);
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
			if (!(other is FilterKey))
				return false;
			FilterKey that = (FilterKey) other;
			if (!that.filterName.Equals(filterName))
				return false;
			if (!that.filterParameters.Equals(filterParameters))
				return false;
			return true;
		}

		public override string ToString()
		{
			return "FilterKey[" + filterName + filterParameters + ']';
		}

		public static ISet CreateFilterKeys(IDictionary enabledFilters)
		{
			if (enabledFilters.Count == 0)
				return null;
			Set result = new HashedSet();
			foreach (FilterImpl filter in enabledFilters)
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
