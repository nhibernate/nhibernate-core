using System;
using System.Data;
using System.Collections;
using System.Data.OleDb;

namespace NHibernate.Connection {
	
	/// <summary>
	/// A connection provider for connection to sql server databases
	/// </summary>
	public class OleDbConnectionProvider : IConnectionProvider{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OleDbConnectionProvider));
		private string connString = null;
		
		public void CloseConnection(IDbConnection conn) {
			log.Debug("Closing OleDb connection");
			try {
				conn.Close();
			} catch(Exception e) {
				throw new ADOException("Could not close OleDb connection", e);
			}
		}
		public void Configure(IDictionary settings) {
			log.Info("Configuring SqlServerConnectionProvider");
			connString = Cfg.Environment.Properties[ Cfg.Environment.ConnectionString ] as string;
			if (connString==null) throw new HibernateException("Could not find connection string setting");
		}

		public IDbConnection GetConnection() {
			log.Debug("Obtaining SqlConnection");
			try {
				IDbConnection conn = new OleDbConnection(connString);
				conn.Open();
				return conn;
			} catch (Exception e) {
				throw new ADOException("Could not create OleDb connection", e);
			}
		}

		public bool IsStatementCache {
			get { return false; }
		}

	}
}
