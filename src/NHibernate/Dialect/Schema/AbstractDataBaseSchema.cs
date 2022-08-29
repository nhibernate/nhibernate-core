using System;
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
		private readonly Dialect _dialect;

		protected AbstractDataBaseSchema(DbConnection connection) : this(connection, null) { }

		protected AbstractDataBaseSchema(DbConnection connection, Dialect dialect)
		{
			Connection = connection;
			_dialect = dialect;
		}

		protected DbConnection Connection { get; }

		public virtual bool IncludeDataTypesInReservedWords => true;

		/// <summary>
		/// Should <see cref="Dialect.Qualify"/> be used for searching tables instead of using separately
		/// the table, schema and catalog names? If <see langword="true" />, dialect must be provided
		/// with <see cref="AbstractDataBaseSchema(DbConnection, Dialect)"/>.
		/// </summary>
		public virtual bool UseDialectQualifyInsteadOfTableName => false;

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
			if (UseDialectQualifyInsteadOfTableName)
			{
				var actualTablePattern = GetActualTableName(catalog, schemaPattern, tableNamePattern);
				var tables = Connection.GetSchema("Tables", new[] { null, null, actualTablePattern });

				// Caller may check the table name of yielded results, we need to patch them
				foreach (DataRow tableRow in tables.Rows)
				{
					var tableName = Convert.ToString(tableRow[ColumnNameForTableName]);
					if (tableName.Equals(actualTablePattern, StringComparison.InvariantCultureIgnoreCase))
					{
						tableRow[ColumnNameForTableName] = tableNamePattern;
						// Columns are looked-up according to the row table name, and schema and catalog data.
						// We need to patch schema and catalog for being able to reconstruct the adequate table name.
						if (!string.IsNullOrEmpty(catalog))
						{
							tableRow["TABLE_CATALOG"] = catalog;
						}
						if (!string.IsNullOrEmpty(schemaPattern))
						{
							tableRow["TABLE_SCHEMA"] = schemaPattern;
						}
					}
				}

				return tables;
			}

			var restrictions = new[] { catalog, schemaPattern, tableNamePattern };
			return Connection.GetSchema("Tables", restrictions);
		}

		public virtual string ColumnNameForTableName
		{
			get { return "TABLE_NAME"; }
		}

		public abstract ITableMetadata GetTableMetadata(DataRow rs, bool extras);

		public virtual DataTable GetColumns(string catalog, string schemaPattern, string tableNamePattern,
											string columnNamePattern)
		{
			if (UseDialectQualifyInsteadOfTableName)
			{
				var actualTablePattern = GetActualTableName(catalog, schemaPattern, tableNamePattern);
				return Connection.GetSchema("Columns", new[] { null, null, actualTablePattern, columnNamePattern });
			}

			var restrictions = new[] { catalog, schemaPattern, tableNamePattern, columnNamePattern };
			return Connection.GetSchema("Columns", restrictions);
		}

		public virtual DataTable GetIndexInfo(string catalog, string schemaPattern, string tableName)
		{
			if (UseDialectQualifyInsteadOfTableName)
			{
				var actualTableName = GetActualTableName(catalog, schemaPattern, tableName);
				return Connection.GetSchema("Indexes", new[] { null, null, actualTableName, null });
			}

			var restrictions = new[] { catalog, schemaPattern, tableName, null };
			return Connection.GetSchema("Indexes", restrictions);
		}

		public virtual DataTable GetIndexColumns(string catalog, string schemaPattern, string tableName, string indexName)
		{
			if (UseDialectQualifyInsteadOfTableName)
			{
				var actualTableName = GetActualTableName(catalog, schemaPattern, tableName);
				return Connection.GetSchema("IndexColumns", new[] { null, null, actualTableName, indexName, null });
			}

			var restrictions = new[] { catalog, schemaPattern, tableName, indexName, null };
			return Connection.GetSchema("IndexColumns", restrictions);
		}

		public virtual DataTable GetForeignKeys(string catalog, string schema, string table)
		{
			if (UseDialectQualifyInsteadOfTableName)
			{
				var actualTableName = GetActualTableName(catalog, schema, table);
				return Connection.GetSchema(ForeignKeysSchemaName, new[] { null, null, actualTableName, null });
			}

			var restrictions = new[] { catalog, schema, table, null };
			return Connection.GetSchema(ForeignKeysSchemaName, restrictions);
		}

		public virtual ISet<string> GetReservedWords()
		{
			var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			DataTable dtReservedWords = Connection.GetSchema(DbMetaDataCollectionNames.ReservedWords);
			foreach (DataRow row in dtReservedWords.Rows)
			{
				result.Add(row["ReservedWord"].ToString());
			}

			if (IncludeDataTypesInReservedWords)
			{
				DataTable dtTypes = Connection.GetSchema(DbMetaDataCollectionNames.DataTypes);
				foreach (DataRow row in dtTypes.Rows)
				{
					result.Add(row["TypeName"].ToString());
				}
			}

			return result;
		}

		protected virtual string ForeignKeysSchemaName
		{
			get { return "ForeignKeys"; }
		}

		private string GetActualTableName(string catalog, string schemaPattern, string tableNamePattern)
		{
			if (_dialect == null)
				throw new InvalidOperationException($"{this}: cannot qualify table name without the dialect");

			// _dialect is supposed to concatenate catalog and schema with the table name as an
			// unqualified name instead of actually qualifying it.
			return _dialect.Qualify(catalog, schemaPattern, tableNamePattern);
		}

		#endregion
	}
}
