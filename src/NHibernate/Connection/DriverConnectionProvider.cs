using System;
using System.Collections;
using System.Data;

using NHibernate.Driver;

namespace NHibernate.Connection
{
	/// <summary>
	/// A ConnectionProvider that uses an IDriver to create connections.
	/// </summary>
	/// <remarks>
	/// This IConnectionProvider implements a rudimentary connection pool.
	/// </remarks>
	public class DriverConnectionProvider : ConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DriverConnectionProvider));
		private static object poolLock = new object();
		private readonly ArrayList pool = new ArrayList();
		
		private int checkedOut = 0;

		public DriverConnectionProvider()
		{
		}

		
		public override IDbConnection GetConnection() 
		{
			if(log.IsDebugEnabled) log.Debug("total checked-out connections: " + checkedOut);

			lock(poolLock) 
			{
				if( pool.Count > 0 ) 
				{
					int last = pool.Count - 1;
					if(log.IsDebugEnabled) 
					{
						log.Debug("using pooled connection, pool size: " + last);
						checkedOut++;
					}

					IDbConnection conn = (IDbConnection)pool[last];
					pool.RemoveAt(last);
					return conn;
				}
			}

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

			for(int i = 0; i < pool.Count; i++) 
			{
				try 
				{
					((IDbConnection)pool[i]).Close();
				}
				catch(Exception e) 
				{
					log.Warn("problem closing pooled connection", e);
				}
			}

			pool.Clear();
		}

		public override void CloseConnection(IDbConnection conn)
		{
			if(log.IsDebugEnabled) checkedOut--;

			lock(poolLock) 
			{
				int currentSize = pool.Count;
				if( currentSize < PoolSize ) 
				{
					if(log.IsDebugEnabled) log.Debug("returning connection to pool, pool size: " + (currentSize + 1) );
					pool.Add(conn);
					return;
				}
			}

			base.CloseConnection(conn);
		}



	}
}
