namespace NHibernate.Cfg
{
	/// <summary>
	/// A set of rules for determining the physical column and table names given the information in the mapping
	/// document. May be used to implement project-scoped naming standards for database objects.
	/// </summary>
	public interface INamingStrategy
	{
		/// <summary>
		/// Return a table name for an entity class
		/// </summary>
		/// <param name="className">the fully-qualified class name</param>
		/// <returns>a table name</returns>
		string ClassToTableName(string className);

		/// <summary>
		/// Return a column name for a property path expression 
		/// </summary>
		/// <param name="propertyName">a property path</param>
		/// <returns>a column name</returns>
		string PropertyToColumnName(string propertyName);

		/// <summary>
		/// Alter the table name given in the mapping document
		/// </summary>
		/// <param name="tableName">a table name</param>
		/// <returns>a table name</returns>
		string TableName(string tableName);

		/// <summary>
		/// Alter the column name given in the mapping document
		/// </summary>
		/// <param name="columnName">a column name</param>
		/// <returns>a column name</returns>
		string ColumnName(string columnName);

		/// <summary>
		/// Return a table name for a collection
		/// </summary>
		/// <param name="className">the fully-qualified name of the owning entity class</param>
		/// <param name="propertyName">a property path</param>
		/// <returns>a table name</returns>
		string PropertyToTableName(string className, string propertyName);

		/// <summary> 
		/// Return the logical column name used to refer to a column in the metadata
		/// (like index, unique constraints etc)
		/// A full bijection is required between logicalNames and physical ones
		/// logicalName have to be case insersitively unique for a given table 
		/// </summary>
		/// <param name="columnName">given column name if any </param>
		/// <param name="propertyName">property name of this column </param>
		string LogicalColumnName(string columnName, string propertyName);
	}
}