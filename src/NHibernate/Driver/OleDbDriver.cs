using System;
using System.Data;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the OleDb DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the OleDb DataProvider.
	/// </remarks>
	public class OleDbDriver : DriverBase
	{
		public OleDbDriver()
		{
		}
		
		public override IDbConnection CreateConnection()
		{
			return new System.Data.OleDb.OleDbConnection();
		}

		public override IDbCommand CreateCommand() 
		{
			return new System.Data.OleDb.OleDbCommand();
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

	}
}
