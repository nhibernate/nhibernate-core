using System;
using System.Data;
using System.Collections;
using System.Runtime.CompilerServices;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;


namespace NHibernate.Id 
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that uses a database table to store the last
	/// generated value.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It is not intended that applications use this strategy directly. However,
	/// it may be used to build other (efficient) strategies. The return type is
	/// <c>System.Int32</c>
	/// </para>
	/// <para>
	/// The hi value MUST be fetched in a seperate transaction to the <c>ISession</c>
	/// transaction so the generator must be able to obtain a new connection and commit it.
	/// Hence this implementation may not be used when the user is supplying connections.
	/// </para>
	/// <para>
	/// Mapping parameters supported are: <c>table</c>, <c>column</c>
	/// </para>
	/// </remarks>
	public class TableGenerator : IPersistentIdentifierGenerator, IConfigurable 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TableGenerator));

		public const string Column = "column";
		public const string Table = "table";
		public const string Schema = "schema";

		private string tableName;
		private string columnName;
		private string query;
		
		private SqlString updateSql;

		public virtual void Configure(IType type, IDictionary parms, Dialect.Dialect dialect) 
		{
			this.tableName = PropertiesHelper.GetString(Table, parms, "hibernate_unique_key");
			this.columnName = PropertiesHelper.GetString(Column, parms, "next_hi");
			string schemaName = (string) parms[Schema];
			if(schemaName!=null && tableName.IndexOf(StringHelper.Dot)<0)
				tableName = schemaName + "." + tableName;

			query = "select " + columnName + " from " + tableName;
			if ( dialect.SupportsForUpdate ) query += " for update";
			
			// build the sql string for the Update since it uses parameters
			Parameter setParam = new Parameter();
			setParam.Name = columnName;
			setParam.DbType = DbType.Int32;
		
			Parameter whereParam = new Parameter();
			whereParam.Name = columnName;
			whereParam.DbType = DbType.Int32;

			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add("update " + tableName + " set ")
				.Add(columnName)
				.Add(" = ")
				.Add(setParam)
				.Add(" where ")
				.Add(columnName)
				.Add(" = ")
				.Add(whereParam);

			updateSql = builder.ToSqlString();
				
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual object Generate(ISessionImplementor session, object obj) 
		{
			//this has to be done using a different connection to the containing
			//transaction becase the new hi value must remain valid even if the
			//contataining transaction rolls back
			IDbConnection conn = session.Factory.OpenConnection();
			int result;
			int rows;
			try {
				IDbTransaction trans = conn.BeginTransaction();

				do {
					//the loop ensure atomicitiy of the 
					//select + uspdate even for no transaction
					//or read committed isolation level (needed for .net?)

					IDbCommand qps = conn.CreateCommand();
					qps.CommandText = query;
					qps.CommandType = CommandType.Text;
					qps.Transaction = trans;
					try 
					{
						IDataReader rs = qps.ExecuteReader();
						if ( !rs.Read() ) 
						{
							string err = "could not read a hi value - you need to populate the table: " + tableName;
							log.Error(err);
							throw new IdentifierGenerationException(err);
						}

						result = rs.GetInt32(0);
						rs.Close();
					} 
					catch (Exception e) 
					{
						log.Error("could not read a hi value", e);
						throw e;
					} 
					finally 
					{
					}


					IDbCommand ups = updateSql.BuildCommand(session.Factory.ConnectionProvider.Driver);
					ups.Connection = conn;
					ups.Transaction = trans;

					try 
					{
						((IDbDataParameter)ups.Parameters[0]).Value = result + 1;
						((IDbDataParameter)ups.Parameters[1]).Value = result;

						rows = ups.ExecuteNonQuery();
					} 
					catch (Exception e) 
					{
						log.Error("could not update hi value in: " + tableName, e);
						throw e;
					} 
					finally 
					{
					}

				} while (rows==0);

				trans.Commit();

				return result;

			} 
			finally 
			{
				session.Factory.CloseConnection(conn);
			}
		}

		public string[] SqlCreateStrings(Dialect.Dialect dialect) 
		{
			return new string[] {
						"create table " + tableName + " ( " + columnName + " " + dialect.SqlTypeToString(SqlTypeFactory.GetInt32()) + " )",
						"insert into " + tableName + " values ( 0 )"
								};
		}

		public string SqlDropString(Dialect.Dialect dialect) 
		{
			return "drop table " + tableName + dialect.CascadeConstraintsString;
		}

		public object GeneratorKey() 
		{
			return tableName;
		}

	}
}
