using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Util
{
	/// <summary></summary>
	public sealed class FilterHelper
	{
		private readonly string[] filterNames;
		private readonly string[] filterConditions;

		public FilterHelper(IDictionary<string, string> filters, Dialect.Dialect dialect, SQLFunctionRegistry sqlFunctionRegistry)
		{
			int filterCount = filters.Count;
			filterNames = new string[filterCount];
			filterConditions = new string[filterCount];
			filterCount = 0;
			foreach (var entry in filters)
			{
				filterNames[filterCount] = entry.Key;
				filterConditions[filterCount] =
					Template.RenderWhereStringTemplate(entry.Value, FilterImpl.MARKER, dialect, sqlFunctionRegistry)?
						.Replace(":", ":" + entry.Key + ".");
				filterCount++;
			}
		}

		public bool IsAffectedBy(IDictionary<string, IFilter> enabledFilters)
		{
			for (int i = 0, max = filterNames.Length; i < max; i++)
			{
				if (enabledFilters.ContainsKey(filterNames[i]))
				{
					return true;
				}
			}
			return false;
		}

		public string Render(String alias, IDictionary<string, IFilter> enabledFilters)
		{
			StringBuilder buffer = new StringBuilder();
			Render(buffer, alias, enabledFilters);
			return buffer.ToString();
		}

		public void Render(StringBuilder buffer, string alias, IDictionary<string, IFilter> enabledFilters)
		{
			Render(buffer, alias, CollectionHelper.EmptyDictionary<string, string>(), enabledFilters);
		}

		public void Render(StringBuilder buffer, string defaultAlias, IDictionary<string, string> propMap, IDictionary<string, IFilter> enabledFilters)
		{
			for (int i = 0; i < filterNames.Length; i++)
			{
				if (enabledFilters.ContainsKey(filterNames[i]))
				{
					string condition = filterConditions[i];
					if (!string.IsNullOrEmpty(condition))
					{
						buffer.Append(" and ");
						AddFilterString(buffer, defaultAlias, propMap, condition);
					}
				}
			}
		}

		private static void AddFilterString(StringBuilder buffer, string defaultAlias, IDictionary<string, string> propMap, string condition)
		{
			int i;
			int upTo = 0;
			while ((i = condition.IndexOf(FilterImpl.MARKER, upTo, StringComparison.Ordinal)) >= 0 && upTo < condition.Length)
			{
				buffer.Append(condition, upTo, i - upTo);
				int startOfProperty = i + FilterImpl.MARKER.Length + 1;

				upTo = condition.IndexOf(' ', startOfProperty);
				upTo = upTo >= 0 ? upTo : condition.Length;
				string property = condition.Substring(startOfProperty, upTo - startOfProperty);

				if (!propMap.TryGetValue(property, out var fullColumn))
					fullColumn = string.IsNullOrEmpty(defaultAlias)
						? property
						: defaultAlias + "." + property;

				buffer.Append(fullColumn);
			}
			buffer.Append(condition.Substring(upTo));
		}

		/// <summary>
		/// Get only filters enabled for many-to-one association.
		/// </summary>
		/// <param name="enabledFilters">All enabled filters</param>
		/// <returns>A new <see cref="IDictionary{TKey,TValue}"/> for filters enabled for many to one.</returns>
		public static IDictionary<string, IFilter> GetEnabledForManyToOne(IDictionary<string, IFilter> enabledFilters)
		{
			var enabledFiltersForManyToOne = new Dictionary<string, IFilter>();
			foreach (var enabledFilter in enabledFilters)
			{
				if (enabledFilter.Value.FilterDefinition.UseInManyToOne)
					enabledFiltersForManyToOne.Add(enabledFilter.Key, enabledFilter.Value);
			}
			return enabledFiltersForManyToOne;
		}

		internal static SqlString ExpandDynamicFilterParameters(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, ISessionImplementor session)
		{
			var enabledFilters = session.EnabledFilters;
			if (enabledFilters.Count == 0 || !ParserHelper.HasHqlVariable(sqlString))
			{
				return sqlString;
			}

			Dialect.Dialect dialect = session.Factory.Dialect;
			string symbols = ParserHelper.HqlSeparators + dialect.OpenQuote + dialect.CloseQuote;

			var result = new SqlStringBuilder();
			foreach (var sqlPart in sqlString)
			{
				var parameter = sqlPart as Parameter;
				if (parameter != null)
				{
					result.Add(parameter);
					continue;
				}

				var sqlFragment = sqlPart.ToString();
				var tokens = new StringTokenizer(sqlFragment, symbols, true);

				foreach (string token in tokens)
				{
					if (ParserHelper.IsHqlVariable(token))
					{
						string filterParameterName = token.Substring(1);
						string[] parts = StringHelper.ParseFilterParameterName(filterParameterName);
						string filterName = parts[0];
						string parameterName = parts[1];
						var filter = (FilterImpl) enabledFilters[filterName];

						int? collectionSpan = filter.GetParameterSpan(parameterName);
						IType type = filter.FilterDefinition.GetParameterType(parameterName);
						int parameterColumnSpan = type.GetColumnSpan(session.Factory);

						// Add query chunk
						string typeBindFragment = string.Join(", ", Enumerable.Repeat("?", parameterColumnSpan));
						string bindFragment;
						if (collectionSpan.HasValue && !type.ReturnedClass.IsArray)
						{
							bindFragment = string.Join(", ", Enumerable.Repeat(typeBindFragment, collectionSpan.Value));
						}
						else
						{
							bindFragment = typeBindFragment;
						}

						// dynamic-filter parameter tracking
						var filterParameterFragment = SqlString.Parse(bindFragment);
						var dynamicFilterParameterSpecification = new DynamicFilterParameterSpecification(filterName, parameterName, type, collectionSpan);
						var parameters = filterParameterFragment.GetParameters().ToArray();
						var sqlParameterPos = 0;
						var paramTrackers = dynamicFilterParameterSpecification.GetIdsForBackTrack(session.Factory);
						foreach (var paramTracker in paramTrackers)
						{
							parameters[sqlParameterPos++].BackTrack = paramTracker;
						}

						parameterSpecs.Add(dynamicFilterParameterSpecification);
						result.Add(filterParameterFragment);
					}
					else
					{
						result.Add(token);
					}
				}
			}
			return result.ToSqlString();
		}
	}
}
