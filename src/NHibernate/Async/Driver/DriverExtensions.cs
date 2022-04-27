using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace NHibernate.Driver
{
	partial class DriverExtensions
	{
#if NETSTANDARD2_1_OR_GREATER
		// 6.0 TODO: merge into IDriver
		/// <summary>
		/// Begin an ADO <see cref="DbTransaction" />.
		/// </summary>
		/// <param name="driver">The driver.</param>
		/// <param name="isolationLevel">The isolation level requested for the transaction.</param>
		/// <param name="connection">The connection on which to start the transaction.</param>
		/// <returns>The started <see cref="DbTransaction" />.</returns>
		public static ValueTask<DbTransaction> BeginTransactionAsync(this IDriver driver, IsolationLevel isolationLevel, DbConnection connection)
		{
			if (driver is DriverBase driverBase)
			{
				return driverBase.BeginTransactionAsync(isolationLevel, connection);
			}

			// So long for custom drivers not deriving from DriverBase, they will have to wait for 6.0 if they
			// need the feature.
			if (isolationLevel == IsolationLevel.Unspecified)
			{
				return connection.BeginTransactionAsync();
			}
			return connection.BeginTransactionAsync(isolationLevel);
		}
#endif
	}
}
