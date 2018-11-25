using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	public static class DriverExtensions
	{
		internal static void AdjustParameterForValue(this IDriver driver, DbParameter parameter, SqlType sqlType, object value)
		{
			var adjustingDriver = driver as IParameterAdjuster;
			adjustingDriver?.AdjustParameterForValue(parameter, sqlType, value);
		}

		// 6.0 TODO: merge into IDriver
		/// <summary>
		/// Begin an ADO <see cref="DbTransaction" />.
		/// </summary>
		/// <param name="driver">The driver.</param>
		/// <param name="isolationLevel">The isolation level requested for the transaction.</param>
		/// <param name="connection">The connection on which to start the transaction.</param>
		/// <returns>The started <see cref="DbTransaction" />.</returns>
		public static DbTransaction BeginTransaction(this IDriver driver, IsolationLevel isolationLevel, DbConnection connection)
		{
			if (driver is DriverBase driverBase)
			{
				return driverBase.BeginTransaction(isolationLevel, connection);
			}

			// So long for custom drivers not deriving from DriverBase, they will have to wait for 6.0 if they
			// need the feature.
			if (isolationLevel == IsolationLevel.Unspecified)
			{
				return connection.BeginTransaction();
			}
			return connection.BeginTransaction(isolationLevel);
		}
	}
}
