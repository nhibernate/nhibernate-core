using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Dialect.Schema;
using NHibernate.Exceptions;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{
	public class DatabaseMetadata
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DatabaseMetadata));

		private readonly IDictionary<string, TableMetadata> tables = new Dictionary<string, TableMetadata>();
		private readonly ISet<string> sequences = new HashedSet<string>();
		private readonly bool extras;
		private readonly ISchemaReader meta;
		private readonly ISQLExceptionConverter sqlExceptionConverter;
		private static readonly string[] Types = {"TABLE", "VIEW"};

		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect)
			: this(connection, dialect, true)
		{
		}


		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect, bool extras)
		{
			meta = dialect.GetSchemaReader(connection);
			this.extras = extras;
			InitSequences(connection, dialect);
			sqlExceptionConverter = dialect.BuildSQLExceptionConverter();
		}

		public TableMetadata GetTableMetadata(string name, string schema, string catalog, bool isQuoted)
		{
			string identifier = Identifier(catalog, schema, name);
			TableMetadata table;
			tables.TryGetValue(identifier, out table);
			if (table != null)
				return table; // EARLY exit

			try
			{
				DataRowCollection rows = meta.GetTables(catalog, schema, name, Types).Rows;
				foreach (DataRow tableRow in rows)
				{
					string tableName = (string) tableRow["TABLE_NAME"];
					if (name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
					{
						table = new TableMetadata(tableRow, meta, extras);
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

		private static string Identifier(string catalog, string schema, string name)
		{
			return Table.Qualify(catalog, schema, name);
		}

		private void InitSequences(DbConnection connection, Dialect.Dialect dialect)
		{
			if (dialect.SupportsSequences)
			{
				string sql = dialect.QuerySequencesString;
				if (sql != null)
				{
					using (IDbCommand statement = connection.CreateCommand())
					{
						statement.CommandText = sql;
						using (IDataReader rs = statement.ExecuteReader())
						{
							while (rs.Read())
								sequences.Add(((string) rs[1]).ToLower().Trim());
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
			return "DatabaseMetadata" + StringHelper.CollectionToString((ICollection)tables.Keys) + " " +
						 StringHelper.CollectionToString((ICollection)sequences);
		}
	}
}