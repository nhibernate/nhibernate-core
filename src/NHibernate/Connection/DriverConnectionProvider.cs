using System;
using System.Collections;
using System.Data;

using NHibernate.Driver;

namespace NHibernate.Connection
{
	/// <summary>
	/// A ConnectionProvider that uses an IDriver to create connections.
	/// </summary>
	public class DriverConnectionProvider : ConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DriverConnectionProvider));
		
		public DriverConnectionProvider()
		{
		}

		
		public override IDbConnection GetConnection() 
		{
			log.Debug("Obtaining IDbConnection from Driver");
			try 
			{
				IDbConnection conn = Driver.CreateConnection();
				conn.ConnectionString = ConnectionString; 
				conn.Open();
				return conn;
			} 
			catch (Exception e) 
			{
				throw new ADOException("Could not create connection from Driver", e);
			}
		}

		public override bool IsStatementCache 
		{
			get { return false; }
		}

		public override void Close()
		{
			log.Info("cleaning up connection pool");
		}

		public override void CloseConnection(IDbConnection conn)
		{
			base.CloseConnection(conn);
			//TODO: make sure I want to do this - pretty sure I do because of Oracle problems.
			conn.Dispose();
		}



	}
}
