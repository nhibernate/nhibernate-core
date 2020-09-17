using System;
using System.Linq;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// A collection role that may be queried or loaded by outer join.
	/// </summary>
	public interface IQueryableCollection : IPropertyMapping, IJoinable, ICollectionPersister
	{
		/// <summary> 
		/// Get the index formulas if this is an indexed collection 
		/// (optional operation)
		/// </summary>
		string[] IndexFormulas { get; }

		/// <summary>
		/// Get the persister of the element class, if this is a
		/// collection of entities (optional operation).  Note that
		/// for a one-to-many association, the returned persister
		/// must be <c>OuterJoinLoadable</c>.
		/// </summary>
		IEntityPersister ElementPersister { get; }

		/// <summary>
		/// Should we load this collection role by outer joining?
		/// </summary>
		FetchMode FetchMode { get; }

		/// <summary>
		/// Get the names of the collection index columns if this is an indexed collection (optional operation)
		/// </summary>
		string[] IndexColumnNames { get; }

		/// <summary>
		/// Get the names of the collection element columns (or the primary key columns in the case of a one-to-many association)
		/// </summary>
		string[] ElementColumnNames { get; }

		/// <summary>
		/// Does this collection role have a where clause filter?
		/// </summary>
		bool HasWhere { get; }

		/// <summary>
		/// Generate a list of collection index and element columns
		/// </summary>
		// Since v5.4
		[Obsolete("Use GetSelectFragment extension method instead.")]
		string SelectFragment(string alias, string columnSuffix);

		/// <summary> 
		/// Get the names of the collection index columns if
		/// this is an indexed collection (optional operation),
		/// aliased by the given table alias
		/// </summary>
		string[] GetIndexColumnNames(string alias);

		/// <summary>
		/// Get the names of the collection element columns (or the primary
		/// key columns in the case of a one-to-many association),
		/// aliased by the given table alias
		/// </summary>
		string[] GetElementColumnNames(string alias);

		/// <summary>
		/// Get the extra where clause filter SQL
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		string GetSQLWhereString(string alias);

		/// <summary>
		/// Get the order by SQL
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		string GetSQLOrderByString(string alias);

		/// <summary>
		/// Get the order-by to be applied at the target table of a many to many
		/// </summary>
		/// <param name="alias">The alias for the many-to-many target table</param>
		/// <returns>Appropriate order-by fragment or empty string.</returns>
		string GetManyToManyOrderByString(string alias);

		// Obsolete since v5.2
		/// <summary>
		/// Generate the table alias to use for the collection's key columns
		/// </summary>
		/// <param name="alias">The alias for the target table</param>
		/// <returns>Appropriate table alias.</returns>
		[Obsolete("Use directly the alias parameter value instead")]
		string GenerateTableAliasForKeyColumns(string alias);
	}

	public static class QueryableCollectionExtensions
	{
		/// <summary>
		/// Gets the select fragment containing collection element, index and indentifier columns.
		/// </summary>
		/// <param name="queryable">The <see cref="IQueryableCollection"/> instance.</param>
		/// <param name="alias">The table alias.</param>
		/// <param name="columnSuffix">The column suffix.</param>
		/// <returns>The element, index and indentifier select fragment.</returns>
		// 6.0 TODO: Move into IQueryableCollection
		public static SelectFragment GetSelectFragment(this IQueryableCollection queryable, string alias, string columnSuffix)
		{
			if (queryable is AbstractCollectionPersister collectionPersister)
			{
				return collectionPersister.GetSelectFragment(alias, columnSuffix);
			}

#pragma warning disable 618
			var renderedText = queryable.SelectFragment(alias, columnSuffix);
#pragma warning restore 618
			var identifierAlias = queryable.GetIdentifierColumnAlias(null);
			var indexAliases = queryable.GetIndexColumnAliases(null);
			var columnAliases = queryable.GetKeyColumnAliases(null)
			                             .Union(queryable.GetElementColumnAliases(null));
			if (indexAliases != null)
			{
				columnAliases = columnAliases.Union(indexAliases);
			}

			if (identifierAlias != null)
			{
				columnAliases = columnAliases.Union(new[] {identifierAlias});
			}

			return new SelectFragment(queryable.Factory.Dialect, renderedText, columnAliases.ToList())
				.SetSuffix(columnSuffix);
		}
	}
}
