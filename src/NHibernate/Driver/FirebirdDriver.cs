using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// Summary description for FirebirdDriver.
	/// </summary>
	public class FirebirdDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		public FirebirdDriver()
		{
			connectionType = System.Type.GetType("FirebirdSql.Data.Firebird.FbConnection, FirebirdSql.Data.Firebird");
			commandType = System.Type.GetType("FirebirdSql.Data.Firebird.FbCommand, FirebirdSql.Data.Firebird");
			
			if( connectionType==null || commandType==null )
			{
				throw new HibernateException( 
					"The IDbCommand and IDbConnection implementation in the Assembly FirebirdSql.Data could not be found.  " +					
					"Please ensure that the Assemblies needed to communicate with Firebird " +
					"are in the Global Assembly Cache or in a location that NHibernate " +
					"can use System.Type.GetType(string) to load the types from."
					);
			}
		}

		public override System.Type CommandType
		{
			get	{ return commandType; }
		}

		public override System.Type ConnectionType
		{
			get	{ return connectionType; }
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
