using System;
using System.Collections;
using System.Linq.Dynamic.Core;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Driver;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	[TestFixture, Obsolete]
	public class ObsoleteDriverFixture : TestCase
	{
		protected override IList Mappings => new [] {"Simple.hbm.xml"};

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			switch (dialect)
			{
				case FirebirdDialect _:
				case MsSql2000Dialect _:
				case MsSqlCeDialect _:
				case MySQLDialect _:
				case PostgreSQLDialect _:
				case SQLiteDialect _:
					System.Type driverType = ReflectHelper.ClassForName(cfg.GetProperty(Cfg.Environment.ConnectionDriver));
					return !(driverType.IsOdbcDriver() || driverType.IsOleDbDriver());
				default:
					return false;
			}
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			System.Type driverType = ReflectHelper.ClassForName(cfg.GetProperty(Cfg.Environment.ConnectionDriver));
			if (driverType.IsOdbcDriver() || driverType.IsOleDbDriver())
			{
				// ODBC and OLE DB drivers are not obsoleted, do not switch it.
				return;
			}

			var dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);
			System.Type driver;
			switch (dialect)
			{
				case FirebirdDialect _:
					driver = typeof(FirebirdClientDriver);
					break;
				case MsSql2008Dialect _:
					driver = typeof(Sql2008ClientDriver);
					break;
				case MsSql2000Dialect _:
					driver = typeof(SqlClientDriver);
					break;
				case MsSqlCeDialect _:
					driver = typeof(SqlServerCeDriver);
					break;
				case MySQLDialect _:
					driver = typeof(MySqlDataDriver);
					break;
				case PostgreSQLDialect _:
					driver = typeof(NpgsqlDriver);
					break;
				case SQLiteDialect _:
					driver = typeof(SQLite20Driver);
					break;
				default:
					return;
			}
			configuration.SetProperty(Cfg.Environment.ConnectionDriver, driver.FullName);
		}

		[Test]
		public void GenerateObsoleteWarning()
		{
			using (var spy = new LogSpy(typeof(ConnectionProvider)))
			{
				using (BuildSessionFactory())
				{
					Assert.That(spy.GetWholeLog(), Does.Contain("Install the NHibernate.Driver."));
				}
			}
		}

		[Test]
		public void CanUseObsoleteDriver()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var count = s.Query<Simple>().Count();
				Assert.That(count, Is.Zero);
				t.Commit();
			}
		}
	}
}
