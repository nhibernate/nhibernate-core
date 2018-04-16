namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.ManagedDataAccess DataProvider
	/// </summary>
	public class OracleManagedDataClientDriver : OracleDataClientDriverBase
	{
		/// <summary>
		/// Initializes a new instance of <see cref="OracleManagedDataClientDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Oracle.ManagedDataAccess</c> assembly can not be loaded.
		/// </exception>
		public OracleManagedDataClientDriver()
			: base("Oracle.ManagedDataAccess")
		{
		}

		public override bool HasDelayedDistributedTransactionCompletion => true;
	}
}
