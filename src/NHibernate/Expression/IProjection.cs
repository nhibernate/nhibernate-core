using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	public interface IProjection
	{
		/// <summary>
		/// Render the SQL Fragment.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="position"></param>
		/// <param name="cirteriaQuery"></param>
		/// <returns></returns>
		SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery cirteriaQuery);

		/// <summary>
		/// Render the SQL Fragment to be used in the Group By Clause.
		/// </summary>
		/// <param name="criteria"></param>
		/// <param name="criteriaQuery"></param>
		/// <returns></returns>
		SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery);

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
		/// Get the SQL select clause column aliases for a particular user-visible alias
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		string[] GetColumnAliases(int loc);

		/// <summary>
		/// Get the SQL select clause column aliases for a particular user-visible alias
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="loc"></param>
		/// <returns></returns>
		string[] GetColumnAliases(string alias, int loc);

		/// <summary>
		/// Get the user-visible aliases for this projection (ie. the ones that will be passed to the ResultTransformer)
		/// </summary>
		string[] Aliases { get; }

		/// <summary>
		/// Does this projection specify grouping attributes?
		/// </summary>
		bool IsGrouped { get; }
	}
}