using System.Collections.Generic;
using NHibernate.SqlCommand;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Anything that can be loaded by outer join - namely persisters for classes or collections.
	/// </summary>
	public interface IJoinable
	{
		// Should this interface extend PropertyMapping?

		/// <summary>
		/// An identifying name; a class name or collection role name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The columns that identify the item.
		/// </summary>
		string[] KeyColumnNames { get; }

		/// <summary>
		/// The columns to join on.
		/// </summary>
		string[] JoinColumnNames { get; }

		/// <summary>
		/// Is this instance actually a ICollectionPersister?
		/// </summary>
		bool IsCollection { get; }

		/// <summary>
		/// The table to join to.
		/// </summary>
		string TableName { get; }

		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias, string currentEntitySuffix,
							  string currentCollectionSuffix, bool includeCollectionColumns);

		/// <summary>
		/// Get the where clause part of any joins (optional operation)
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses);

		/// <summary>
		/// Get the from clause part of any joins (optional operation)
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses);

		/// <summary>
		/// Get the where clause filter, given a query alias and considering enabled session filters
		/// </summary>
		string FilterFragment(string alias, IDictionary<string, IFilter> enabledFilters);

		string OneToManyFilterFragment(string alias);

		/// <summary>
		/// Very, very, very ugly...
		/// </summary>
		/// <value>Does this persister "consume" entity column aliases in the result
		/// set?</value>
		bool ConsumesEntityAlias();

		/// <summary>
		/// Very, very, very ugly...
		/// </summary>
		/// <value>Does this persister "consume" collection column aliases in the result
		/// set?</value>
		bool ConsumesCollectionAlias();
	}
}