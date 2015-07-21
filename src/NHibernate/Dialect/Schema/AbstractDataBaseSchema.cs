using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NHibernate.Dialect.Schema
{
	/// <summary>
	/// Common implementation of schema reader.
	/// </summary>
	/// <remarks>
	/// This implementation of <see cref="IDataBaseSchema"/> is based on the new <see cref="DbConnection"/> of
	/// .NET 2.0.
	/// </remarks>
	/// <seealso cref="DbConnection.GetSchema()"/>
	public abstract class AbstractDataBaseSchema : IDataBaseSchema
	{
		private readonly DbConnection connection;

		protected AbstractDataBaseSchema(DbConnection connection)
		{
			this.connection = connection;
		}

		protected DbConnection Connection
		{
			get { return connection; }
		}

		#region IDataBaseSchema Members

		public virtual bool StoresMixedCaseQuotedIdentifiers
		{
			get { return true; }
		}

		public virtual bool StoresUpperCaseQuotedIdentifiers
		{
			get { return false; }
		}

		public virtual bool StoresUpperCaseIdentifiers
		{
			get { return false; }
		}

		public virtual bool StoresLowerCaseQuotedIdentifiers
		{
			get { return false; }
		}

		public virtual bool StoresLowerCaseIdentifiers
		{
			get { return false; }
		}

		public virtual DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			var restrictions = new[] { catalog, schemaPattern, tableNamePattern };
			return connection.GetSchema("Tables", restrictions);
		}

		public virtual string ColumnNameForTableName
		{
			get { return "TABLE_NAME"; }
		}

		public abstract ITableMetadata GetTableMetadata(DataRow rs, bool extras);

		public virtual DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern,
		                                    string columnNamePattern)
		{
			var restrictions = new[] {catalog, schemaPattern, tableNamePattern, columnNamePattern};
			return connection.GetSchema("Columns", restrictions);
		}

		public virtual DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			var restrictions = new[] {catalog, schemaPattern, tableName, null};
			return connection.GetSchema("Indexes", restrictions);
		}

		public virtual DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			var restrictions = new[] {catalog, schemaPattern, tableName, indexName, null};
			return connection.GetSchema("IndexColumns", restrictions);
		}

		public virtual DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			var restrictions = new[] {catalog, schema, table, null};
			return connection.GetSchema(ForeignKeysSchemaName, restrictions);
		}

		public virtual ISet<string> GetReservedWords()
		{
			var result = new HashSet<string>();
			DataTable dtReservedWords = connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
			foreach (DataRow row in dtReservedWords.Rows)
			{
				result.Add(row["ReservedWord"].ToString());
			}
			return result;
		}

		protected virtual string ForeignKeysSchemaName
		{
			get { return "ForeignKeys"; }
		}

		#endregion
	}
}