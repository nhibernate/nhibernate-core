using System;
using System.Data;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Odbc DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the Odbc DataProvider.
	/// </remarks>
	public class OdbcDriver : DriverBase
	{
		public OdbcDriver()
		{
		}
		
		public override IDbConnection CreateConnection()
		{
			return new System.Data.Odbc.OdbcConnection();
		}

		public override IDbCommand CreateCommand() 
		{
			return new System.Data.Odbc.OdbcCommand();
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
			get {return String.Empty;}
		}
	}
}
