using NHibernate.Persister.Entity;

namespace NHibernate.Id
{
	/// <summary> 
	/// A persister that may have an identity assigned by execution of a SQL <tt>INSERT</tt>. 
	/// </summary>
	public interface IPostInsertIdentityPersister : IEntityPersister
	{
		/// <summary> 
		/// Get the database-specific SQL command to retrieve the last
		/// generated IDENTITY value.
		/// </summary>
		string IdentitySelectString { get;}

		/// <summary> The names of the primary key columns in the root table. </summary>
		/// <returns> The primary key column names. </returns>
		string[] RootTableKeyColumnNames { get;}

		/// <summary> 
		/// Get a SQL select string that performs a select based on a unique
		/// key determined by the given property name). 
		/// </summary>
		/// <param name="propertyName">
		/// The name of the property which maps to the
		/// column(s) to use in the select statement restriction.
		/// </param>
		/// <returns> The SQL select string </returns>
		string GetSelectByUniqueKeyString(string propertyName);
	}
}