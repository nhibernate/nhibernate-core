using System;
using System.Data;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class SqlClientDriver : DriverBase
	{
		public SqlClientDriver()
		{
		}
		
		public override IDbConnection CreateConnection()
		{
			return new System.Data.SqlClient.SqlConnection();
		}

		public override IDbCommand CreateCommand() 
		{
			return new System.Data.SqlClient.SqlCommand();
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

		
	}
}
