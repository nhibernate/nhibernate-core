using System;
using NHibernate.AdoNet.Util;

namespace NHibernate.Id
{
	public struct PersistentIdGeneratorParmsNames
	{
		static PersistentIdGeneratorParmsNames()
		{
			SqlStatementLogger = new SqlStatementLogger(false, false);
		}
		
		/// <summary> The configuration parameter holding the schema name</summary>
		public readonly static string Schema = "schema";

		/// <summary> 
		/// The configuration parameter holding the table name for the
		/// generated id
		/// </summary>
		public readonly static string Table = "target_table";

		/// <summary> 
		/// The configuration parameter holding the table names for all
		/// tables for which the id must be unique
		/// </summary>
		public readonly static string Tables = "identity_tables";

		/// <summary> 
		/// The configuration parameter holding the primary key column
		/// name of the generated id
		/// </summary>
		public readonly static string PK = "target_column";

		/// <summary> The configuration parameter holding the catalog name</summary>
		public readonly static string Catalog = "catalog";

		public readonly static SqlStatementLogger SqlStatementLogger;
	}

	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that requires creation of database objects
	/// All <see cref="IPersistentIdentifierGenerator"/>s that also implement 
	/// An <see cref="IConfigurable" />  have access to a special mapping parameter: schema
	/// </summary>
	public interface IPersistentIdentifierGenerator : IIdentifierGenerator
	{
		/// <summary>
		/// The SQL required to create the underlying database objects
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// An array of <see cref="String"/> objects that contain the sql to create the 
		/// necessary database objects.
		/// </returns>
		string[] SqlCreateStrings(Dialect.Dialect dialect);

		/// <summary>
		/// The SQL required to remove the underlying database objects
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// A <see cref="String"/> that will drop the database objects.
		/// </returns>
		string[] SqlDropString(Dialect.Dialect dialect);

		/// <summary>
		/// Return a key unique to the underlying database objects.
		/// </summary>
		/// <returns>
		/// A key unique to the underlying database objects.
		/// </returns>
		/// <remarks>
		/// Prevents us from trying to create/remove them multiple times
		/// </remarks>
		string GeneratorKey();
	}
}