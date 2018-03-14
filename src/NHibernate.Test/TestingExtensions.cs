using NHibernate.Driver;

namespace NHibernate.Test
{
	public static class TestingExtensions
	{
		public static bool IsOdbcDriver(this IDriver driver)
		{
			if (driver is OdbcDriver) return true;
			return false;
		}

		public static bool IsOdbcDriver(this System.Type driverClass)
		{
			if (typeof(OdbcDriver).IsAssignableFrom(driverClass)) return true;
			return false;
		}

		public static bool IsOleDbDriver(this IDriver driver)
		{
			if (driver is OleDbDriver) return true;
			return false;
		}

		public static bool IsOleDbDriver(this System.Type driverClass)
		{
			if (typeof(OleDbDriver).IsAssignableFrom(driverClass)) return true;
			return false;
		}

		/// <summary>
		/// Matches both SQL Server 2000 and 2008 drivers
		/// </summary>
		public static bool IsSqlClientDriver(this IDriver driver)
		{
			if (driver is SqlClientDriver) return true;
			return false;
		}

		/// <summary>
		/// Matches both SQL Server 2000 and 2008 drivers
		/// </summary>
		public static bool IsSqlClientDriver(this System.Type driverClass)
		{
			if (typeof(SqlClientDriver).IsAssignableFrom(driverClass)) return true;
			return false;
		}

		public static bool IsSql2008ClientDriver(this IDriver driver)
		{
			if (driver is Sql2008ClientDriver) return true;
			return false;
		}

		public static bool IsMySqlDataDriver(this System.Type driverClass)
		{
			if (typeof(MySqlDataDriver).IsAssignableFrom(driverClass)) return true;
			return false;
		}


		public static bool IsFirebirdClientDriver(this IDriver driver)
		{
			if (driver is FirebirdClientDriver) return true;
			return false;
		}

		/// <summary>
		/// If driver is Firebird, clear the pool.
		/// Firebird will pool each connection created during the test and will marked as used any table
		/// referenced by queries. It will at best delays those tables drop until connections are actually
		/// closed, or immediately fail dropping them.
		/// This results in other tests failing when they try to create tables with same name.
		/// By clearing the connection pool the tables will get dropped. This is done by the following code.
		/// Moved from NH1908 test case, contributed by Amro El-Fakharany.
		/// </summary>
		public static void ClearPoolForFirebirdClientDriver(this IDriver driver)
		{
			switch (driver)
			{
				case FirebirdClientDriver fbDriver:
					fbDriver.ClearPool(null);
					break;
			}
		}

		public static bool IsOracleDataClientDriver(this IDriver driver)
		{
			if (driver is OracleDataClientDriver) return true;
			return false;
		}

		public static bool IsOracleDataClientDriver(this System.Type driverClass)
		{
			if (typeof(OracleDataClientDriver).IsAssignableFrom(driverClass)) return true;
			return false;
		}

		public static bool IsOracleLiteDataClientDriver(this IDriver driver)
		{
			if (driver is OracleLiteDataClientDriver) return true;
			return false;
		}

		public static bool IsOracleManagedDataClientDriver(this IDriver driver)
		{
			if (driver is OracleManagedDataClientDriver) return true;
			return false;
		}
	}
}
