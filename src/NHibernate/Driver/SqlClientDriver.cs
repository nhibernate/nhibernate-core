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

		/// <summary>
		/// The SqlClient driver does NOT support more than 1 open IDataReader
		/// with only 1 IDbConnection.
		/// </summary>
		/// <value><c>false</c> - it is not supported.</value>
		/// <remarks>
		/// Ms Sql 2000 (and 7) throws an Exception when multiple DataReaders are 
		/// attempted to be Opened.  When Yukon comes out a new Driver will be 
		/// created for Yukon because it is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false;	}
		}


		
	}
}
