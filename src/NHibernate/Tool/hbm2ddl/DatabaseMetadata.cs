namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using Exceptions;
	using Iesi.Collections;
	using log4net;
	using Mapping;
	using Util;

	public class DatabaseMetadata
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (DatabaseMetadata));

		private readonly IDictionary tables = new Hashtable();
		private readonly ISet sequences = new HashedSet();
		private readonly bool extras;
		private readonly ISQLExceptionConverter sqlExceptionConverter;

		private readonly ISchemaReader schemaReader;

		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect)
			: this(connection, dialect, true)
		{
		}


		public DatabaseMetadata(DbConnection connection, Dialect.Dialect dialect, bool extras)
		{
			schemaReader = new InformationSchemaReader(connection);
			this.extras = extras;
			InitSequences(connection, dialect);
			sqlExceptionConverter = dialect.BuildSQLExceptionConverter();
		}

		private static readonly String[] Types = {"TABLE", "VIEW"};

		private static readonly object tableIndexStatistic = null;

		public static object TableIndexStatistic
		{
			get { return tableIndexStatistic; }
		}


		public TableMetadata GetTableMetadata(string name, string schema)
		{
			Object identifier = Identifier(schema, name);
			TableMetadata table = (TableMetadata) tables[identifier];
			if (table != null)
			{
				return table;
			}
			else
			{
				try
				{
					DataRowCollection rows = schemaReader.GetTables(schema, name, Types).Rows;
					foreach (DataRow tableRow in rows)
					{
						string tableName = (string) tableRow["TABLE_NAME"];
						if (name.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
						{
							table = new TableMetadata(tableRow, schemaReader, extras);
							this.tables.Add(identifier, table);
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
		}

		private static string Identifier(String schema, String name)
		{
			return Table.Qualify(null, schema, name);
		}


		private void InitSequences(DbConnection connection, Dialect.Dialect dialect)
		{
			if (dialect.SupportsSequences)
			{
				String sql = dialect.QuerySequencesString;
				if (sql != null)
				{
					IDbCommand statement = null;
					IDataReader rs = null;
					try
					{
						statement = connection.CreateCommand();
						statement.CommandText = sql;
						rs = statement.ExecuteReader();

						while (rs.Read())
						{
							sequences.Add(((String) rs[1]).ToLower());
						}
					}
					finally
					{
						if (rs != null) rs.Close();
						if (statement != null) statement.Dispose();
					}
				}
			}
		}


		public bool IsTable(Object key)
		{
			if (key is String)
			{
				Table tbl = new Table((String) key);
				if (GetTableMetadata(tbl.Name, tbl.Schema) != null)
				{
					return true;
				}
				else
				{
					String[] strings = StringHelper.Split(".", (String) key);
					if (strings.Length == 3)
					{
						tbl = new Table(strings[2]);
						tbl.Schema = (strings[1]);
						return GetTableMetadata(tbl.Name, tbl.Schema) != null;
					}
					else if (strings.Length == 2)
					{
						tbl = new Table(strings[1]);
						tbl.Schema = (strings[0]);
						return GetTableMetadata(tbl.Name, tbl.Schema) != null;
					}
				}
			}
			return false;
		}

		public override String ToString()
		{
			return "DatabaseMetadata" + StringHelper.CollectionToString(tables.Keys) + " " +
			       StringHelper.CollectionToString(sequences);
		}


		public bool IsSequence(Object key)
		{
			if (key is String)
			{
				String[] strings = StringHelper.Split(".", (String) key);
				return sequences.Contains(strings[strings.Length - 1].ToLower());
			}
			return false;
		}
	}
}