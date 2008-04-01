using System.Data;

namespace NHibernate.Dialect.Schema
{
	/// <summary>
	/// The reader of the schema to enable SchemaUpdate.
	/// </summary>
	/// <remarks>
	/// The schema reader implementation is provide by a dialect.
	/// </remarks>
	public interface ISchemaReader
	{
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
		/// <remarks>The result is relative to the schema sollections "Indexes".</remarks>
		DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName);

		/// <summary>
		/// Get a description of the given table's indices and statistics.
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schemaPattern">Schema pattern, retrieves those without the schema</param>
		/// <param name="tableName">A table name pattern</param>
		/// <param name="indexName">The name of the index</param>
		/// <returns>A description of the table's indices available</returns>
		/// <remarks>The result is relative to the schema sollections "IndexColumns".</remarks>
		DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName);

		/// <summary>
		/// Gets a description of the primary keys available
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schema">Schema name, retrieves those without the schema</param>
		/// <param name="table">A table name</param>
		/// <returns>A description of the primary keys available</returns>
		//DataTable GetPrimaryKeys(string catalog, string schema, string table);


		/// <summary>
		/// Gets a description of the foreign keys available
		/// </summary>
		/// <param name="catalog">A catalog, retrieves those without a catalog</param>
		/// <param name="schema">Schema name, retrieves those without the schema</param>
		/// <param name="table">A table name</param>
		/// <returns>A description of the foreign keys available</returns>
		DataTable GetForeignKeys(string catalog, string schema, string table);
	}
}