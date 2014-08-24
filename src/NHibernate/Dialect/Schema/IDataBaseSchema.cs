using System.Collections.Generic;
using System.Data;

namespace NHibernate.Dialect.Schema
{
	/// <summary>
	/// This class is specific of NHibernate and supply DatabaseMetaData of Java.
	/// In the .NET Framework, there is no direct equivalent.
	/// </summary>
	/// <remarks>
	/// Implementation is provide by a dialect.
	/// </remarks>
	public interface IDataBaseSchema
	{
		/// <summary>
		/// In the Java language, this field indicates that the database treats mixed-case, 
		/// quoted SQL identifiers as case-insensitive and stores them in mixed case.
		/// </summary>
		bool StoresMixedCaseQuotedIdentifiers { get; }

		/// <summary>
		/// In the Java language, this field indicates that the database treats mixed-case, 
		/// quoted SQL identifiers as case-insensitive and stores them in upper case.
		/// </summary>
		bool StoresUpperCaseQuotedIdentifiers { get; }

		/// <summary>
		/// In the Java language, this field indicates that the database treats mixed-case, 
		/// unquoted SQL identifiers as case-insensitive and stores them in upper case.
		/// </summary>
		bool StoresUpperCaseIdentifiers { get; }

		/// <summary>
		/// In the Java language, this field indicates that the database treats mixed-case, 
		/// quoted SQL identifiers as case-insensitive and stores them in lower case. 
		/// </summary>
		bool StoresLowerCaseQuotedIdentifiers { get; }

		/// <summary>
		/// In the Java language, this field indicates that the database treats mixed-case, 
		/// unquoted SQL identifiers as case-insensitive and stores them in lower case, 
		/// </summary>
		bool StoresLowerCaseIdentifiers { get; }

		/// <summary>
		/// Gets a description of the tables available for the catalog
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schemaPattern">Schema pattern, retrieves those without the schema</param>
		/// <param name="tableNamePattern">A table name pattern</param>
		/// <param name="types">a list of table types to include</param>
		/// <returns>Each row</returns>
		DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types);

		/// <summary>
		/// The name of the column that represent the TABLE_NAME in the <see cref="DataTable"/>
		/// returned by <see cref="GetTables"/>.
		/// </summary>
		string ColumnNameForTableName { get;}

		/// <summary>
		/// Get the Table MetaData.
		/// </summary>
		/// <param name="rs">The <see cref="DataRow"/> resultSet of <see cref="GetTables"/>.</param>
		/// <param name="extras">Include FKs and indexes</param>
		/// <returns></returns>
		ITableMetadata GetTableMetadata(DataRow rs, bool extras);

		/// <summary>
		/// Gets a description of the table columns available
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schemaPattern">Schema pattern, retrieves those without the schema</param>
		/// <param name="tableNamePattern">A table name pattern</param>
		/// <param name="columnNamePattern">a columng name patterm</param>
		/// <returns>A description of the table columns available</returns>
		DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern);

		/// <summary>
		/// Get a description of the given table's indices and statistics.
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schemaPattern">Schema pattern, retrieves those without the schema</param>
		/// <param name="tableName">A table name pattern</param>
		/// <returns>A description of the table's indices available</returns>
		/// <remarks>The result is relative to the schema collections "Indexes".</remarks>
		DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName);

		/// <summary>
		/// Get a description of the given table's indices and statistics.
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schemaPattern">Schema pattern, retrieves those without the schema</param>
		/// <param name="tableName">A table name pattern</param>
		/// <param name="indexName">The name of the index</param>
		/// <returns>A description of the table's indices available</returns>
		/// <remarks>The result is relative to the schema collections "IndexColumns".</remarks>
		DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName);
		
		/*
		/// <summary>
		/// Gets a description of the primary keys available
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schema">Schema name, retrieves those without the schema</param>
		/// <param name="table">A table name</param>
		/// <returns>A description of the primary keys available</returns>
		//DataTable GetPrimaryKeys(string catalog, string schema, string table);
		*/

		/// <summary>
		/// Gets a description of the foreign keys available
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schema">Schema name, retrieves those without the schema</param>
		/// <param name="table">A table name</param>
		/// <returns>A description of the foreign keys available</returns>
		DataTable GetForeignKeys(string catalog, string schema, string table);

		/// <summary>
		/// Get all reserved words
		/// </summary>
		/// <returns>A set of reserved words</returns>
		ISet<string> GetReservedWords();
	}
}
