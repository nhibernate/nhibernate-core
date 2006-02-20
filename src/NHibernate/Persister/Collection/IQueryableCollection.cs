using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
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
		/// Generate a list of collection index and element columns
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		SqlString SelectFragment( string alias );

		/// <summary>
		/// Get the names of the collection index columns if this is an indexed collection (optional operation)
		/// </summary>
		string[] IndexColumnNames { get; }

		/// <summary>
		/// Get the names of the collection element columns (or the primary key columns in the case of a one-to-many association)
		/// </summary>
		string[] ElementColumnNames { get; }

		/// <summary>
		/// Get the extra where clause filter SQL
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		string GetSQLWhereString( string alias );

		/// <summary>
		/// Get the order by SQL
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		string GetSQLOrderByString( string alias );

		/// <summary>
		/// Does this collection role have a where clause filter?
		/// </summary>
		bool HasWhere { get; }

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
	}
}
