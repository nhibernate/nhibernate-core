#if !NETSTANDARD2_0 || DRIVER_PACKAGE
using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.ManagedDataAccess DataProvider
	/// </summary>
	[Obsolete("Use NHibernate.Driver.Oracle.Managed NuGet package and OracleManagedDriver."
	          + "  There are also Loquacious configuration points: .Connection.ByOracleManagedDriver() and .DataBaseIntegration(x => x.OracleManagedDriver()).")]
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
#endif
