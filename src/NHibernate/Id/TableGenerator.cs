using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using log4net;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that uses a database table to store the last
	/// generated value.
	/// </summary>
	/// <remarks>
	/// <p>
	/// It is not intended that applications use this strategy directly. However,
	/// it may be used to build other (efficient) strategies. The return type is
	/// <c>System.Int32</c>
	/// </p>
	/// <p>
	/// The hi value MUST be fetched in a seperate transaction to the <c>ISession</c>
	/// transaction so the generator must be able to obtain a new connection and commit it.
	/// Hence this implementation may not be used when the user is supplying connections.
	/// </p>
	/// <p>
	/// The mapping parameters <c>table</c> and <c>column</c> are required.
	/// </p>
	/// </remarks>
	public class TableGenerator : IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TableGenerator));
		/// <summary>
		/// An additional where clause that is added to 
		/// the queries against the table.
		/// </summary>
		public const string Where = "where";

		/// <summary>
		/// The name of the column parameter.
		/// </summary>
		public const string ColumnParamName = "column";

		/// <summary>
		/// The name of the table parameter.
		/// </summary>
		public const string TableParamName = "table";

		/// <summary>Default column name </summary>
		public const string DefaultColumnName = "next_hi";

		/// <summary>Default table name </summary>
		public const string DefaultTableName = "hibernate_unique_key";

		private string tableName;
		private string columnName;
		private string whereClause;
		private string query;

		protected SqlType columnSqlType;
		protected PrimitiveType columnType;

		private SqlString updateSql;
		private SqlType[] parameterTypes;

		#region IConfigurable Members

		/// <summary>
		/// Configures the TableGenerator by reading the value of <c>table</c>, 
		/// <c>column</c>, and <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect"/> to help with Configuration.</param>
		public virtual void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{

			tableName = PropertiesHelper.GetString(TableParamName, parms, DefaultTableName);
			columnName = PropertiesHelper.GetString(ColumnParamName, parms, DefaultColumnName);
			whereClause = PropertiesHelper.GetString(Where, parms, "");
			string schemaName = PropertiesHelper.GetString(PersistentIdGeneratorParmsNames.Schema, parms, null);
			string catalogName = PropertiesHelper.GetString(PersistentIdGeneratorParmsNames.Catalog, parms, null);

			if (tableName.IndexOf('.') < 0)
			{
				tableName = Table.Qualify(catalogName, schemaName, tableName);
			}

			query = "select " + columnName + " from " + dialect.AppendLockHint(LockMode.Upgrade, tableName)
			        + dialect.ForUpdateString;

			columnType = type as PrimitiveType;
			if (columnType == null)
			{
				log.Error("Column type for TableGenerator is not a value type");
				throw new ArgumentException("type is not a ValueTypeType", "type");
			}

			// build the sql string for the Update since it uses parameters
			if (type is Int16Type)
			{
				columnSqlType = SqlTypeFactory.Int16;
			}
			else if (type is Int64Type)
			{
				columnSqlType = SqlTypeFactory.Int64;
			}
			else
			{
				columnSqlType = SqlTypeFactory.Int32;
			}

			parameterTypes = new SqlType[2] {columnSqlType, columnSqlType};

			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add("update " + tableName + " set ")
				.Add(columnName)
				.Add(" = ")
				.Add(Parameter.Placeholder)
				.Add(" where ")
				.Add(columnName)
				.Add(" = ")
				.Add(Parameter.Placeholder);
			if (string.IsNullOrEmpty(whereClause) == false)
			{
				builder.Add(" and ")
					.Add(whereClause);
			}

			updateSql = builder.ToSqlString();
		}

		#endregion

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate a <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/> 
		/// for the identifier by selecting and updating a value in a table.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/>.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual object Generate(ISessionImplementor session, object obj)
		{
			// This has to be done using a different connection to the containing
			// transaction becase the new hi value must remain valid even if the
			// containing transaction rolls back.
			//
			// We make an exception for SQLite and use the session's connection,
			// since SQLite only allows one connection to the database.

			bool isSQLite = session.Factory.Dialect is SQLiteDialect;
			IDbConnection conn;
			if (isSQLite)
			{
				conn = session.Connection;
			}
			else
			{
				conn = session.Factory.OpenConnection();
			}

			try
			{
				IDbTransaction trans = null;
				if (!isSQLite)
				{
					trans = conn.BeginTransaction();
				}

				long result;
				int rows;
				do
				{
					//the loop ensure atomicitiy of the 
					//select + uspdate even for no transaction
					//or read committed isolation level (needed for .net?)

					IDbCommand qps = conn.CreateCommand();
					IDataReader rs = null;
					qps.CommandText = query;
					qps.CommandType = CommandType.Text;
					qps.Transaction = trans;
					try
					{
						log.Debug(string.Format("Reading high value:{0}", qps.CommandText));
						rs = qps.ExecuteReader();
						if (!rs.Read())
						{
							string err = "could not read a hi value - you need to populate the table: " + tableName;
							log.Error(err);
							throw new IdentifierGenerationException(err);
						}
						result = Convert.ToInt64(columnType.Get(rs, 0));
					} 
						// TODO: change to SqlException
					catch (Exception e)
					{
						log.Error("could not read a hi value", e);
						throw;
					}
					finally
					{
						if (rs != null) rs.Close();
						qps.Dispose();
					}

					IDbCommand ups =
						session.Factory.ConnectionProvider.Driver.GenerateCommand(CommandType.Text, updateSql, parameterTypes);
					ups.Connection = conn;
					ups.Transaction = trans;

					try
					{
						columnType.Set(ups, result + 1, 0);
						columnType.Set(ups, result, 1);

						log.Debug(string.Format("Updating high value:{0}", ups.CommandText));
						rows = ups.ExecuteNonQuery();
					} 
						// TODO: change to SqlException
					catch (Exception e)
					{
						log.Error("could not update hi value in: " + tableName, e);
						throw;
					}
					finally
					{
						ups.Dispose();
					}
				} while (rows == 0);

				if (!isSQLite)
				{
					trans.Commit();
				}

				return result;
			}
				// TODO: Shouldn't we have a Catch with a rollback here?
			finally
			{
				if (!isSQLite)
				{
					session.Factory.CloseConnection(conn);
				}
			}
		}

		#endregion

		#region IPersistentIdentifierGenerator Members

		/// <summary>
		/// The SQL required to create the database objects for a TableGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// An array of <see cref="String"/> objects that contain the Dialect specific sql to 
		/// create the necessary database objects and to create the first value as <c>1</c> 
		/// for the TableGenerator.
		/// </returns>
		public string[] SqlCreateStrings(Dialect.Dialect dialect)
		{
			// changed the first value to be "1" by default since an uninitialized Int32 is 0 - leaving
			// it at 0 would cause problems with an unsaved-value="0" which is what most people are 
			// defaulting <id>'s with Int32 types at.
			return new string[]
				{
					"create table " + tableName + " ( " + columnName + " " + dialect.GetTypeName(columnSqlType) + " )",
					"insert into " + tableName + " values ( 1 )"
				};
		}

		/// <summary>
		/// The SQL required to remove the underlying database objects for a TableGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// A <see cref="String"/> that will drop the database objects for the TableGenerator.
		/// </returns>
		public string SqlDropString(Dialect.Dialect dialect)
		{
			return dialect.GetDropTableString(tableName);
		}

		/// <summary>
		/// Return a key unique to the underlying database objects for a TableGenerator.
		/// </summary>
		/// <returns>
		/// The configured table name.
		/// </returns>
		public string GeneratorKey()
		{
			return tableName;
		}

		#endregion
	}
}