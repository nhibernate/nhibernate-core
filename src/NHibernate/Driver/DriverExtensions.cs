using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	public static class DriverExtensions
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(DriverExtensions));
		// No threading protection on this member: we do not care, at worst it will cause some duplicated warning.
		private static bool _hasWarnedForMissingBeginTransaction;

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

			// Use reflection for supporting custom drivers.
			var beginMethod = driver.GetType().GetMethod(
				nameof(DriverBase.BeginTransaction),
				new[] { typeof(IsolationLevel), typeof(DbConnection) });
			if (beginMethod != null && beginMethod.ReturnType == typeof(DbTransaction) && !beginMethod.IsStatic)
			{
				return (DbTransaction) beginMethod.Invoke(driver, new object[] { isolationLevel, connection });
			}

			if (!_hasWarnedForMissingBeginTransaction)
			{
				// Ideally it should be a dictionary per driver class, but keep it simple. Drivers not deriving from
				// DriverBase are already not common, so applications which would use two distinct such drivers are
				// even less common.
				_hasWarnedForMissingBeginTransaction = true;
				Log.Warn(
					"Driver {0} is obsolete. It should implement a 'public DbTransaction BeginTransaction(" +
					"IsolationLevel, DbConnection) method.",
					driver);
			}

			if (isolationLevel == IsolationLevel.Unspecified)
			{
				return connection.BeginTransaction();
			}
			return connection.BeginTransaction(isolationLevel);
		}
	}
}
