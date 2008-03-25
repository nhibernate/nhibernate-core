namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : ReflectionBasedDriver
	{
		public OracleClientDriver() : base(
			"System.Data.OracleClient, version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
			"System.Data.OracleClient.OracleConnection",
			"System.Data.OracleClient.OracleCommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return ":"; }
		}
	}
}