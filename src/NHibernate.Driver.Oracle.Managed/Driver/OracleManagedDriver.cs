using System.Data.Common;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.ManagedDataAccess DataProvider
	/// </summary>
	public class OracleManagedDriver : OracleDataClientDriverBase
	{
		/// <summary>
		/// Initializes a new instance of <see cref="OracleManagedDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.ManagedDataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleManagedDriver()
			: base("Oracle.ManagedDataAccess")
		{
		}

		public override bool HasDelayedDistributedTransactionCompletion => true;

		public override DbConnection CreateConnection()
		{
			return new Oracle.ManagedDataAccess.Client.OracleConnection();
		}

		public override DbCommand CreateCommand()
		{
			return new Oracle.ManagedDataAccess.Client.OracleCommand();
		}
	}
}
