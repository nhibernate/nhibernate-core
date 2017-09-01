using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using NHibernate.Dialect.Schema;
using NHibernate.Exceptions;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{
	public partial class DatabaseMetadata : IDatabaseMetadata
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (DatabaseMetadata));

		private readonly IDictionary<string, ITableMetadata> tables = new Dictionary<string, ITableMetadata>();
		private readonly ISet<string> sequences = new HashSet<string>();
		private readonly bool extras;
		private readonly Dialect.Dialect dialect;
		private readonly IDataBaseSchema meta;
		private readonly ISQLExceptionConverter sqlExceptionConverter;
		private static readonly string[] Types = {"TABLE", "VIEW"};

		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect)
			: this(connection, dialect, true)
		{
		}


		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect, bool extras)
		{
			meta = dialect.GetDataBaseSchema(connection);
			this.dialect = dialect;
			this.extras = extras;
			InitSequences(connection, dialect);
			sqlExceptionConverter = dialect.BuildSQLExceptionConverter();
		}

		public ITableMetadata GetTableMetadata(string name, string schema, string catalog, bool isQuoted)
		{
			string identifier = Identifier(catalog, schema, name);
			ITableMetadata table;
			tables.TryGetValue(identifier, out table);
			if (table != null)
				return table; // EARLY exit

			try
			{
				DataTable metaInfo;
				if ((isQuoted && meta.StoresMixedCaseQuotedIdentifiers))
				{
					metaInfo = meta.GetTables(catalog, schema, name, Types);
				}
				else
				{
					if ((isQuoted && meta.StoresUpperCaseQuotedIdentifiers) || (!isQuoted && meta.StoresUpperCaseIdentifiers))
					{
						metaInfo =
							meta.GetTables(StringHelper.ToUpperCase(catalog), StringHelper.ToUpperCase(schema),
										   StringHelper.ToUpperCase(name), Types);
					}
					else
					{
						if ((isQuoted && meta.StoresLowerCaseQuotedIdentifiers) || (!isQuoted && meta.StoresLowerCaseIdentifiers))
						{
							metaInfo =
								meta.GetTables(StringHelper.ToLowerCase(catalog), StringHelper.ToLowerCase(schema),
											   StringHelper.ToLowerCase(name), Types);
						}
						else
						{
							metaInfo = meta.GetTables(catalog, schema, name, Types);
						}
					}

				}
				DataRowCollection rows = metaInfo.Rows;

				foreach (DataRow tableRow in rows)
				{
					string tableName = Convert.ToString(tableRow[meta.ColumnNameForTableName]);
					if (name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
					{
						table = meta.GetTableMetadata(tableRow, extras);
						tables[identifier] = table;
						return table;
					}
				}

				log.Info("table not found: " + name);
				return null;
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(sqlExceptionConverter, sqle, "could not get table metadata: " + name);
			}
		}

		private string Identifier(string catalog, string schema, string name)
		{
			return dialect.Qualify(catalog, schema, name);
		}

		private void InitSequences(DbConnection connection, Dialect.Dialect dialect)
		{
			if (dialect.SupportsSequences)
			{
				string sql = dialect.QuerySequencesString;
				if (sql != null)
				{
					using (var statement = connection.CreateCommand())
					{
						statement.CommandText = sql;
						using (var rs = statement.ExecuteReader())
						{
							while (rs.Read())
								sequences.Add(((string) rs[0]).ToLower().Trim());
						}
					}
				}
			}
		}

		public bool IsSequence(object key)
		{
			string sKey = key as string;
			if (sKey != null)
			{
				string[] strings = sKey.Split('.');
				return sequences.Contains(strings[strings.Length - 1].ToLower());
			}
			return false;
		}

		public bool IsTable(object key)
		{
			string sKey = key as string;
			if (sKey != null)
			{
				Table tbl = new Table(sKey);
				if (GetTableMetadata(tbl.Name, tbl.Schema, tbl.Catalog, tbl.IsQuoted) != null)
				{
					return true;
				}
				else
				{
					string[] strings = sKey.Split('.');
					if (strings.Length == 3)
					{
						tbl = new Table(strings[2]);
						tbl.Catalog = strings[0];
						tbl.Schema = strings[1];
						return GetTableMetadata(tbl.Name, tbl.Schema, tbl.Catalog, tbl.IsQuoted) != null;
					}
					else if (strings.Length == 2)
					{
						tbl = new Table(strings[1]);
						tbl.Schema = strings[0];
						return GetTableMetadata(tbl.Name, tbl.Schema, tbl.Catalog, tbl.IsQuoted) != null;
					}
				}
			}
			return false;
		}

		public override String ToString()
		{
			return string.Format("DatabaseMetadata{0} {1}", StringHelper.CollectionToString(tables.Keys), StringHelper.CollectionToString(sequences));
		}
	}
}
