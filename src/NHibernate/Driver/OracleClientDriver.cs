namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary></summary>
		public OracleClientDriver()
		{
			connectionType = System.Type.GetType( "System.Data.OracleClient.OracleConnection, System.Data.OracleClient, version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );
			commandType = System.Type.GetType( "System.Data.OracleClient.OracleCommand, System.Data.OracleClient, version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" );
		}

		/// <summary></summary>
		public override System.Type CommandType
		{
			get { return commandType; }
		}

		/// <summary></summary>
		public override System.Type ConnectionType
		{
			get { return connectionType; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}
	}
}