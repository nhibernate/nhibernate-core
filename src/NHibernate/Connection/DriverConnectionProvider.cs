using System;
using System.Data;
using log4net;

namespace NHibernate.Connection
{
	/// <summary>
	/// A ConnectionProvider that uses an IDriver to create connections.
	/// </summary>
	public class DriverConnectionProvider : ConnectionProvider
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( DriverConnectionProvider ) );

		/// <summary></summary>
		public DriverConnectionProvider()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IDbConnection GetConnection()
		{
			log.Debug( "Obtaining IDbConnection from Driver" );
			try
			{
				IDbConnection conn = Driver.CreateConnection();
				conn.ConnectionString = ConnectionString;
				conn.Open();
				return conn;
			}
			catch( Exception e )
			{
				throw new ADOException( "Could not create connection from Driver", e );
			}
		}

		/// <summary></summary>
		public override bool IsStatementCache
		{
			get { return false; }
		}

		/// <summary></summary>
		public override void Close()
		{
			log.Info( "cleaning up connection pool" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		public override void CloseConnection( IDbConnection conn )
		{
			base.CloseConnection( conn );
			//TODO: make sure I want to do this - pretty sure I do because of Oracle problems.
			conn.Dispose();
		}


	}
}