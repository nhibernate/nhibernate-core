using System;
using System.Data;

using NHibernate.Driver;

namespace NHibernate.Connection
{
	/// <summary>
	/// Summary description for DriverConnectionProvider.
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

	}
}
