using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using log4net;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

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
		private static readonly ILog log = LogManager.GetLogger( typeof( TableGenerator ) );

		/// <summary>
		/// The name of the column parameter.
		/// </summary>
		public const string Column = "column";
		
		/// <summary>
		/// The name of the table parameter.
		/// </summary>
		public const string Table = "table";
		
		/// <summary>
		/// The name of the schema parameter.
		/// </summary>
		public const string Schema = "schema";

		private string tableName;
		private string columnName;
		private string query;

		private SqlString updateSql;

		#region IConfigurable Members

		/// <summary>
		/// Configures the TableGenerator by reading the value of <c>table</c>, 
		/// <c>column</c>, and <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public virtual void Configure( IType type, IDictionary parms, Dialect.Dialect dialect )
		{
			this.tableName = PropertiesHelper.GetString( Table, parms, "hibernate_unique_key" );
			this.columnName = PropertiesHelper.GetString( Column, parms, "next_hi" );
			string schemaName = ( string ) parms[ Schema ];
			if( schemaName != null && tableName.IndexOf( StringHelper.Dot ) < 0 )
			{
				tableName = schemaName + "." + tableName;
			}

			query = "select " + columnName + " from " + tableName;
			if( dialect.SupportsForUpdate )
			{
				query += " for update";
			}

			// build the sql string for the Update since it uses parameters
			Parameter setParam = new Parameter( columnName, new Int32SqlType() );
			Parameter whereParam = new Parameter( columnName, new Int32SqlType() );

			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add( "update " + tableName + " set " )
				.Add( columnName )
				.Add( " = " )
				.Add( setParam )
				.Add( " where " )
				.Add( columnName )
				.Add( " = " )
				.Add( whereParam );

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
		[MethodImpl( MethodImplOptions.Synchronized )]
		public virtual object Generate( ISessionImplementor session, object obj )
		{
			// This has to be done using a different connection to the containing
			// transaction becase the new hi value must remain valid even if the
			// containing transaction rolls back.
			//
			// We make an exception for SQLite and use the session's connection,
			// since SQLite only allows one connection to the database.

			bool isSQLite = session.Factory.Dialect is Dialect.SQLiteDialect;
			IDbConnection conn;
			if( isSQLite )
			{
				conn = session.Connection;
			}
			else
			{
				conn = session.Factory.OpenConnection();
			}

			int result;
			int rows;
			try
			{
				IDbTransaction trans = null;
				if( !isSQLite )
				{
					trans = conn.BeginTransaction();
				}

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
						rs = qps.ExecuteReader();
						if( !rs.Read() )
						{
							string err = "could not read a hi value - you need to populate the table: " + tableName;
							log.Error( err );
							throw new IdentifierGenerationException( err );
						}
						result = Convert.ToInt32( rs[ 0 ] );
					} 
						// TODO: change to SqlException
					catch( Exception e )
					{
						log.Error( "could not read a hi value", e );
						throw;
					}
					finally
					{
						if ( rs != null ) rs.Close();
						qps.Dispose();
					}

					IDbCommand ups = session.Factory.ConnectionProvider.Driver.GenerateCommand( session.Factory.Dialect, updateSql );
					ups.Connection = conn;
					ups.Transaction = trans;

					try
					{
						( ( IDbDataParameter ) ups.Parameters[ 0 ] ).Value = result + 1;
						( ( IDbDataParameter ) ups.Parameters[ 1 ] ).Value = result;

						rows = ups.ExecuteNonQuery();
					} 
						// TODO: change to SqlException
					catch( Exception e )
					{
						log.Error( "could not update hi value in: " + tableName, e );
						throw;
					}
					finally
					{
						ups.Dispose();
					}

				}
				while( rows == 0 );

				if( !isSQLite )
				{
					trans.Commit();
				}

				return result;
			}
				// TODO: Shouldn't we have a Catch with a rollback here?
			finally
			{
				if( !isSQLite )
				{
					session.Factory.CloseConnection( conn );
				}
			}
		}

		#endregion

		#region IPersistentIdentifierGenerator Members
		
		/// <summary>
		/// The SQL required to create the database objects for a TableGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// An array of <see cref="String"/> objects that contain the Dialect specific sql to 
		/// create the necessary database objects and to create the first value as <c>1</c> 
		/// for the TableGenerator.
		/// </returns>
		public string[ ] SqlCreateStrings( Dialect.Dialect dialect )
		{
			// changed the first value to be "1" by default since an uninitialized Int32 is 0 - leaving
			// it at 0 would cause problems with an unsaved-value="0" which is what most people are 
			// defaulting <id>'s with Int32 types at.
			return new string[ ]
				{
					"create table " + tableName + " ( " + columnName + " " + dialect.GetTypeName( SqlTypeFactory.GetInt32() ) + " )",
					"insert into " + tableName + " values ( 1 )"
				};
		}

		/// <summary>
		/// The SQL required to remove the underlying database objects for a TableGenerator.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with creating the sql.</param>
		/// <returns>
		/// A <see cref="String"/> that will drop the database objects for the TableGenerator.
		/// </returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return "drop table " + tableName + dialect.CascadeConstraintsString;
		}

		/// <summary>
		/// Return a key unique to the underlying database objects for a TableGenerator.
		/// </summary>
		/// <returns>
		/// The configured table name.
		/// </returns>
		public object GeneratorKey()
		{
			return tableName;
		}
		
		#endregion

	}
}