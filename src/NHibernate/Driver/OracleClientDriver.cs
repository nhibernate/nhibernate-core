using System;
using System.Data;
using System.Reflection;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider	
	/// <see cref="System.Data.OracleClient.OracleConnection"/>.
	/// </summary>
	public class OracleClientDriver: DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		public OracleClientDriver()
		{
			connectionType = System.Type.GetType("System.Data.OracleClient.OracleConnection, System.Data.OracleClient, version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			commandType = System.Type.GetType("System.Data.OracleClient.OracleCommand, System.Data.OracleClient, version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
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
			get {return ":";}
		}
	}
}

