using NHibernate.Driver;

namespace NHibernate.Test
{
	public static class TestingExtensions
	{
		public static bool IsOdbcDriver(this IDriver driver)
		{
			return (driver is OdbcDriver);
		}

		public static bool IsOdbcDriver(this System.Type driverClass)
		{
			return typeof(OdbcDriver).IsAssignableFrom(driverClass);
		}

		public static bool IsOleDbDriver(this IDriver driver)
		{
			return (driver is OleDbDriver);
		}

		public static bool IsOleDbDriver(this System.Type driverClass)
		{
			return typeof(OleDbDriver).IsAssignableFrom(driverClass);
		}

		public static bool IsSqlClientDriver(this IDriver driver)
		{
			return (driver is SqlClientDriver);
		}

		public static bool IsSqlClientDriver(this System.Type driverClass)
		{
			return typeof(SqlClientDriver).IsAssignableFrom(driverClass);
		}

		public static bool IsSql2008ClientDriver(this IDriver driver)
		{
			return (driver is Sql2008ClientDriver);
		}

		public static bool IsMySqlDataDriver(this System.Type driverClass)
		{
			return typeof(MySqlDataDriver).IsAssignableFrom(driverClass);
		}


		public static bool IsFirebirdClientDriver(this IDriver driver)
		{
			return (driver is FirebirdClientDriver);
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
			if (driver is FirebirdClientDriver fbDriver)
			{
				fbDriver.ClearPool(null);
			}
		}

		public static bool IsOracleDataClientDriver(this IDriver driver)
		{
			return (driver is OracleDataClientDriver);
		}

		public static bool IsOracleDataClientDriver(this System.Type driverClass)
		{
			return typeof(OracleDataClientDriver).IsAssignableFrom(driverClass);
		}

		public static bool IsOracleLiteDataClientDriver(this IDriver driver)
		{
			return (driver is OracleLiteDataClientDriver);
		}

		public static bool IsOracleManagedDataClientDriver(this IDriver driver)
		{
			return (driver is OracleManagedDataClientDriver);
		}
	}
}
