using NHibernate.Persister.Entity;

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
		string[] IndexFormulas { get;}

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
	}
}