using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Engine;
using NHibernate.Loader.Criteria;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	public interface IProjection
	{
		/// <summary>
		/// Render the SQL Fragment.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="position">The position.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <param name="enabledFilters">The enabled filters.</param>
		/// <returns></returns>
		SqlString ToSqlString(ICriteria criteria, int position,
			ICriteriaQuery criteriaQuery, 
			IDictionary<string, IFilter> enabledFilters);

		/// <summary>
		/// Render the SQL Fragment to be used in the Group By Clause.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <param name="enabledFilters">The enabled filters.</param>
		/// <returns></returns>
		SqlString ToGroupSqlString(ICriteria criteria, 
			ICriteriaQuery criteriaQuery,
			IDictionary<string, IFilter> enabledFilters);

		/// <summary>
		/// Return types for a particular user-visible alias
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="criteriaQuery"></param>
		/// <returns></returns>
		IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="criteria"></param>
		/// <param name="criteriaQuery"></param>
		/// <returns></returns>
		IType[] GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery);

		/// <summary>
		/// Get the user-visible aliases for this projection (ie. the ones that will be passed to the ResultTransformer)
		/// </summary>
		string[] Aliases { get; }

		/// <summary>
		/// Does this projection specify grouping attributes?
		/// </summary>
		bool IsGrouped { get; }

		/// <summary>
		/// Does this projection specify aggregate attributes?
		/// </summary>
		bool IsAggregate { get; }

		/// <summary>
		/// Gets the typed values for parameters in this projection
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <returns></returns>
		TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery);

		/// <summary>
		/// Get the SQL column aliases used by this projection for the columns it writes for inclusion into the
		/// <code>SELECT</code> clause <see cref="IProjection.ToSqlString" />.  NHibernate always uses column aliases 
		/// to extract data from the <see cref="System.Data.IDataReader" />, so it is important that these be implemented 
		/// correctly in order for NHibernate to be able to extract these values correctly.
		/// </summary>
		/// <param name="position">Just as in <see cref="IProjection.ToSqlString" />, represents the number of columns rendered prior to this projection.</param>
		/// <param name="criteria">The local criteria to which this project is attached (for resolution).</param>
		/// <param name="criteriaQuery">The overall criteria query instance.</param>
		/// <returns>The columns aliases.</returns>
		string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery);

		/// <summary>
		/// Get the SQL column aliases used by this projection for the columns it writes for inclusion into the
		/// <code>SELECT</code> clause (<see cref="IProjection.ToSqlString" />) for a particular criteria-level alias.
		/// </summary>
		/// <param name="alias">The criteria-level alias.</param>
		/// <param name="position">Just as in <see cref="IProjection.ToSqlString" />, represents the number of columns rendered prior to this projection.</param>
		/// <param name="criteria">The local criteria to which this project is attached (for resolution).</param>
		/// <param name="criteriaQuery">The overall criteria query instance.</param>
		/// <returns>The columns aliases.</returns>
		string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery);
	}
}
