using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

using log4net;

using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns a <c>Int64</c>, constructed by
	/// counting from the maximum primary key value at startup. Not safe for use in a
	/// cluster!
	/// </summary>
	/// <remarks>
	/// <para>
	/// java author Gavin King, .NET port Mark Holden
	/// </para>
	/// <para>
	/// Mapping parameters supported, but not usually needed: table, column.
	/// </para>
	/// </remarks>
	public class IncrementGenerator : IPersistentIdentifierGenerator, IConfigurable
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( IncrementGenerator ) );

		/// <summary></summary>
		public const string Column = "target_column";

		/// <summary></summary>
		public const string Table = "target_table";

		/// <summary></summary>
		public const string Schema = "schema";

		private string tableName;
		private long next;
		private string sql;
		private System.Type returnClass;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="d"></param>
		public void Configure( IType type, IDictionary parms, Dialect.Dialect d )
		{
			tableName = parms[ Table ] as string;
			string columnName = ( ( Column ) parms[ Column ] ).Name;

			string schemaName = parms[ Schema ] as string;
			if( schemaName != null && tableName.IndexOf( StringHelper.Dot ) < 0 )
			{
				tableName = schemaName + "." + tableName;
			}
			returnClass = type.ReturnedClass;

			sql = "select max(" + columnName + ") from " + tableName;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.Synchronized )]
		public object Generate( ISessionImplementor session, object obj )
		{
			if( sql != null )
			{
				getNext( session );
			}
			return IdentifierGeneratorFactory.CreateNumber( next++, returnClass );
		}

		private void getNext( ISessionImplementor session )
		{
			log.Debug( "fetching initial value: " + sql );

			IDbConnection conn = session.Factory.OpenConnection();
			IDbCommand qps = conn.CreateCommand();
			qps.CommandText = sql;
			qps.CommandType = CommandType.Text;
			try
			{
				IDataReader rs = qps.ExecuteReader();
				if( rs.Read() )
				{
					if( !rs.IsDBNull( 0 ) )
					{
						next = Convert.ToInt64( rs.GetValue( 0 ) ) + 1;
					}
					else
					{
						next = 1L;
					}
				}
				else
				{
					next = 1L;
				}
				sql = null;
				log.Debug( "first free id: " + next );
			}
			catch( Exception e )
			{
				log.Error( "could not get increment value", e );
				throw;
			}
			finally
			{
				session.Factory.CloseConnection( conn );
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string[] SqlCreateStrings( Dialect.Dialect dialect )
		{
			return new string[0];
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string SqlDropString( Dialect.Dialect dialect )
		{
			return null;
		}

		/// <summary></summary>
		public object GeneratorKey()
		{
			return tableName;
		}
	}
}