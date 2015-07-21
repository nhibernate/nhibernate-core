using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary> 
	/// A persister that may have an identity assigned by execution of a SQL <tt>INSERT</tt>. 
	/// </summary>
	public interface IPostInsertIdentityPersister
	{
		/* NH cosideration:
		 * this interface was de-wired from IEntityPersister because we want use it for ICollectionPersister too.
		 * More exactly we want use it for id-bag.
		 */
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
		SqlString GetSelectByUniqueKeyString(string propertyName);

		#region NH specific
		/// <summary>
		/// Get the identifier type
		/// </summary>
		IType IdentifierType { get; }

		string GetInfoString();
		#endregion
	}
}