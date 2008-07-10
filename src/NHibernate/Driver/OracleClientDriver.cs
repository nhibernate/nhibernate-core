using System.Data;
using System.Data.OracleClient;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle DataProvider.
	/// </summary>
	public class OracleClientDriver : DriverBase
	{
		public override IDbConnection CreateConnection()
		{
			return new OracleConnection();
		}

		public override IDbCommand CreateCommand()
		{
			return new OracleCommand();
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