using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Schema;

namespace NHibernate.Dialect.Schema
{
	/// <summary>
	/// Common implementation of schema reader.
	/// </summary>
	/// <remarks>
	/// This implementation of <see cref="ISchemaReader"/> is based on the new <see cref="DbConnection"/> of
	/// .NET 2.0.
	/// </remarks>
	/// <seealso cref="DbConnection.GetSchema()"/>
	public class SchemaReader : ISchemaReader
	{
		private readonly DbConnection connection;

		public SchemaReader(DbConnection connection)
		{
			this.connection = connection;
		}

		public virtual DataTable GetTables(string catalog, string schemaPattern, string tableNamePattern, string[] types)
		{
			string[] restrictions = new string[4] { catalog, schemaPattern, tableNamePattern, null };
			return connection.GetSchema("Tables", restrictions);
		}

		public virtual DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern, string columnNamePattern)
		{
			string[] restrictions = new string[4] { catalog, schemaPattern, tableNamePattern, columnNamePattern };
			return connection.GetSchema("Columns", restrictions);
		}

		public DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			string[] restrictions = new string[4] { catalog, schemaPattern, tableName, null };
			return connection.GetSchema("Indexes", restrictions);
		}

		public DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			string[] restrictions = new string[5] { catalog, schemaPattern, tableName, indexName, null };
			return connection.GetSchema("IndexColumns", restrictions);
		}

		public virtual DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			string[] restrictions = new string[4] { catalog, schema, table, null };
			return connection.GetSchema("ForeignKeys", restrictions);
		}
	}
}