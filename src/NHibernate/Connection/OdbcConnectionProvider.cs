using System;
using System.Data;
using System.Data.Odbc;

namespace NHibernate.Connection
{
	/// <summary>
	/// A connection provider for connection to databases through 
	/// and ODBC driver.
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the ODBC DataProvider.
	/// </remarks>
	public class OdbcConnectionProvider : ConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OdbcConnectionProvider));

		public override IDbConnection GetConnection() 
		{
			log.Debug("Obtaining OdbcConnection");
			try 
			{
				IDbConnection conn = new OdbcConnection(this.ConnectionString);
				conn.Open();
				return conn;
			} 
			catch (Exception e) 
			{
				throw new ADOException("Could not create OdbcConnection", e);
			}
		}

		public override bool IsStatementCache 
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInSql 
		{
			get {return false;}
		}

		public override bool UseNamedPrefixInParameter 
		{
			get {return false;}
		}

		public override string NamedPrefix 	
		{
			get {return "";}
		}

		public override IDbCommand CreateCommand() 
		{
			return new OdbcCommand();
		}

	}
}
