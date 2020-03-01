using System.Collections.Generic;
using NHibernate.Impl;
using NHibernate.Util;

namespace NHibernate
{
	// 6.0 TODO: merge into IQuery.
	public static class QueryExtensions
	{
		/// <summary>
		/// Bind the keys values of the given dictionary to named parameters of the query,
		/// matching keys with parameter names and mapping value types to
		/// NHibernate types using heuristics.
		/// </summary>
		/// <param name="query">The query on which to set the parameters.</param>
		/// <param name="map">A dictionary of parameters values by names.</param>
		public static IQuery SetParameters(this IQuery query, IDictionary<string, object> map)
		{
			var absQuery = ReflectHelper.CastOrThrow<AbstractQueryImpl>(query, "SetParameters(IDictionary<string, object>)");
			return absQuery.SetParameters(map);
		}

		/// <summary>
		/// Bind the property values of the given object to named parameters of the query,
		/// matching property names with parameter names and mapping property types to
		/// NHibernate types using heuristics.
		/// </summary>
		/// <param name="query">The query on which to set the parameters.</param>
		/// <param name="map">An object whose properties are to be used as parameters.</param>
		public static IQuery SetParameters(this IQuery query, object map)
		{
			if (query is AbstractQueryImpl absQuery)
			{
				return absQuery.SetParameters(map);
			}

#pragma warning disable 618
			return query.SetProperties(map);
#pragma warning restore 618
		}
	}
}
