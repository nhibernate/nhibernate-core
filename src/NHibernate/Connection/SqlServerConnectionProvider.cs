using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace NHibernate.Connection 
{
	/// <summary>
	/// A connection provider for connection to SqlServer databases using the 
	/// <see cref="System.Data.SqlClient.SqlConnection"/>.
	/// </summary>
	public class SqlServerConnectionProvider : ConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SqlServerConnectionProvider));

		public override IDbConnection GetConnection() 
		{
			log.Debug("Obtaining SqlConnection");
			try 
			{
				IDbConnection conn = new SqlConnection(this.ConnectionString);
				conn.Open();
				return conn;
			} 
			catch (Exception e) 
			{
				throw new ADOException("Could not create SqlServer connection", e);
			}
		}

		public override bool IsStatementCache 
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInSql 
		{
			get {return true;}
		}

		public override bool UseNamedPrefixInParameter 
		{
			get {return true;}
		}

		public override string NamedPrefix 	
		{
			get {return "@";}
		}

		public override IDbCommand CreateCommand() 
		{
			return new System.Data.SqlClient.SqlCommand();
		}

	}
}
