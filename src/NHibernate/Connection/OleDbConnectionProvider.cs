using System;
using System.Data;
using System.Collections;
using System.Data.OleDb;

namespace NHibernate.Connection 
{
	/// <summary>
	/// A connection provider for connection to databases through an OleDb driver.
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the OleDb DataProvider.
	/// </remarks>
	public class OleDbConnectionProvider : ConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OleDbConnectionProvider));


		public override IDbConnection GetConnection() 
		{
			log.Debug("Obtaining OleDbConnection");
			try 
			{
				IDbConnection conn = new OleDbConnection(this.ConnectionString);
				conn.Open();
				return conn;
			} 
			catch (Exception e) 
			{
				throw new ADOException("Could not create OleDb connection", e);
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
			return new OleDbCommand();
		}

	}
}
