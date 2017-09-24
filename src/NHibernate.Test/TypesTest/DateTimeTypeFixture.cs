using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// TestFixtures for the <see cref="DateTimeType"/>.
	/// </summary>
	[TestFixture]
	public class DateTimeTypeFixture
	{
		[Test]
		public void Next()
		{
			var type = NHibernateUtil.DateTime;
			object current = DateTime.Parse("2004-01-01");
			object next = type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTime>(), "next should be DateTime");
			Assert.That(next, Is.GreaterThan(current), "next should be greater than current");
		}

		[Test]
		public void Seed()
		{
			var type = NHibernateUtil.DateTime;
			Assert.That(type.Seed(null), Is.TypeOf<DateTime>(), "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			NullableType type = NHibernateUtil.DateTime;

			object value1 = DateTime.Now;
			object value2 = type.DeepCopy(value1, null);

			Assert.That(value2, Is.EqualTo(value1), "Copies should be the same.");

			value2 = ((DateTime) value2).AddHours(2);
			Assert.That(value2, Is.Not.EqualTo(value1), "value2 was changed, value1 should not have changed also.");
		}

		[Test]
		public void EqualityShouldIgnoreKindAndMillisecond()
		{
			var type = NHibernateUtil.DateTime;
			var localTime = DateTime.Now;
			var unspecifiedKid = new DateTime(
				localTime.Year,
				localTime.Month,
				localTime.Day,
				localTime.Hour,
				localTime.Minute,
				localTime.Second,
				0,
				DateTimeKind.Unspecified);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
			Assert.That(type.IsEqual(localTime, unspecifiedKid), Is.True);
		}
	}

	[TestFixture]
	public class DateTimeSqlTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "DateTime";
		private const int _dateId = 1;

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			var driverClass = ReflectHelper.ClassForName(configuration.GetProperty(Cfg.Environment.ConnectionDriver));
			ClientDriverWithParamsStats.DriverClass = driverClass;

			configuration.SetProperty(
				Cfg.Environment.ConnectionDriver,
				typeof(ClientDriverWithParamsStats).AssemblyQualifiedName);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var d = new DateTimeClass
				{
					Id = _dateId,
					LocalDateTimeValue = DateTime.Now.AddDays(-1),
					UtcDateTimeValue = DateTime.UtcNow.AddDays(-1),
					NormalDateTimeValue = DateTime.Now.AddDays(-1)
				};
				s.Save(d);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from DateTimeClass").ExecuteUpdate();
				t.Commit();
			}
		}

		protected override void DropSchema()
		{
			(Sfi.ConnectionProvider.Driver as ClientDriverWithParamsStats)?.CleanUp();
			base.DropSchema();
		}

		[Test]
		public void DbHasExpectedType()
		{
			var validator = new SchemaValidator(cfg);
			validator.Validate();
		}

		[Test]
		public void SaveUseExpectedSqlType()
		{
			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var d = new DateTimeClass
				{
					Id = 2,
					LocalDateTimeValue = DateTime.Now,
					UtcDateTimeValue = DateTime.UtcNow,
					NormalDateTimeValue = DateTime.Now
				};
				driver.ClearStats();
				s.Save(d);
				t.Commit();
			}

			AssertSqlType(driver, 3);
		}

		[Test]
		public void UpdateUseExpectedSqlType()
		{
			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var d = s.Get<DateTimeClass>(_dateId);
				d.LocalDateTimeValue = DateTime.Now;
				d.UtcDateTimeValue = DateTime.UtcNow;
				d.NormalDateTimeValue = DateTime.Now;
				driver.ClearStats();
				t.Commit();
			}

			AssertSqlType(driver, 3);
		}

		[Test]
		public void QueryUseExpectedSqlType()
		{
			if (!TestDialect.SupportsNonDataBoundCondition)
				Assert.Ignore("Dialect does not support the test query");

			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var q = s
					.CreateQuery(
						"from DateTimeClass d where d.LocalDateTimeValue = :local and " +
						"d.UtcDateTimeValue = :utc and d.NormalDateTimeValue = :normal and " +
						":other1 = :other2")
					.SetDateTime("local", DateTime.Now)
					.SetDateTime("utc", DateTime.UtcNow)
					.SetDateTime("normal", DateTime.Now)
					.SetDateTime("other1", DateTime.Now)
					.SetDateTime("other2", DateTime.Now);
				driver.ClearStats();
				q.List<DateTimeClass>();
				t.Commit();
			}

			AssertSqlType(driver, 5);
		}

		private void AssertSqlType(ClientDriverWithParamsStats driver, int expectedCount)
		{
			if (NHibernateUtil.DateTime.SqlTypes(Sfi).Any(t => Equals(t, SqlTypeFactory.DateTime2)))
			{
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime),
					Is.EqualTo(0),
					"Found unexpected SqlTypeFactory.DateTime usages.");
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime2),
					Is.EqualTo(expectedCount),
					"Unexpected SqlTypeFactory.DateTime2 usage count.");
				Assert.That(driver.GetCount(DbType.DateTime), Is.EqualTo(0), "Found unexpected DbType.DateTime usages.");
				Assert.That(
					driver.GetCount(DbType.DateTime2),
					Is.EqualTo(expectedCount),
					"Unexpected DbType.DateTime2 usage count.");
			}
			else
			{
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime2),
					Is.EqualTo(0),
					"Found unexpected SqlTypeFactory.DateTime2 usages.");
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime),
					Is.EqualTo(expectedCount),
					"Unexpected SqlTypeFactory.DateTime usage count.");
				Assert.That(driver.GetCount(DbType.DateTime2), Is.EqualTo(0), "Found unexpected DbType.DateTime2 usages.");
				Assert.That(driver.GetCount(DbType.DateTime), Is.EqualTo(expectedCount), "Unexpected DbType.DateTime usage count.");
			}
		}
	}

	public class ClientDriverWithParamsStats : IDriver
	{
		private readonly Dictionary<SqlType, int> _usedSqlTypes = new Dictionary<SqlType, int>();
		private readonly Dictionary<DbType, int> _usedDbTypes = new Dictionary<DbType, int>();
		internal static System.Type DriverClass { get; set; }

		private readonly IDriver _driverImplementation;

		public ClientDriverWithParamsStats()
		{
			_driverImplementation = (IDriver) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(DriverClass);
		}

		private static void Inc<T>(T type, IDictionary<T, int> dic)
		{
			dic[type] = GetCount(type, dic) + 1;
		}

		private static int GetCount<T>(T type, IDictionary<T, int> dic) =>
			dic.TryGetValue(type, out var count) ? count : 0;

		public void ClearStats()
		{
			_usedSqlTypes.Clear();
			_usedDbTypes.Clear();
		}

		public int GetCount(SqlType type) =>
			GetCount(type, _usedSqlTypes);

		public int GetCount(DbType type) =>
			GetCount(type, _usedDbTypes);

		DbCommand IDriver.GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			var cmd = _driverImplementation.GenerateCommand(type, sqlString, parameterTypes);

			foreach (var sqlType in parameterTypes)
			{
				Inc(sqlType, _usedSqlTypes);
			}
			foreach (DbParameter param in cmd.Parameters)
			{
				Inc(param.DbType, _usedDbTypes);
			}

			return cmd;
		}

		DbParameter IDriver.GenerateParameter(DbCommand command, string name, SqlType sqlType)
		{
			var param = _driverImplementation.GenerateParameter(command, name, sqlType);
			Inc(sqlType, _usedSqlTypes);
			Inc(param.DbType, _usedDbTypes);
			return param;
		}

		#region Firebird mess

		public void CleanUp()
		{
			if (_driverImplementation is FirebirdClientDriver fbDriver)
			{
				// Firebird will pool each connection created during the test and will marked as used any table
				// referenced by queries. It will at best delays those tables drop until connections are actually
				// closed, or immediately fail dropping them.
				// This results in other tests failing when they try to create tables with same name.
				// By clearing the connection pool the tables will get dropped. This is done by the following code.
				// Moved from NH1908 test case, contributed by Amro El-Fakharany.
				fbDriver.ClearPool(null);
			}
		}

		#endregion

		#region Pure forwarding

		void IDriver.Configure(IDictionary<string, string> settings)
		{
			_driverImplementation.Configure(settings);
		}

		DbConnection IDriver.CreateConnection()
		{
			return _driverImplementation.CreateConnection();
		}

		bool IDriver.SupportsMultipleOpenReaders => _driverImplementation.SupportsMultipleOpenReaders;

		void IDriver.PrepareCommand(DbCommand command)
		{
			_driverImplementation.PrepareCommand(command);
		}

		void IDriver.RemoveUnusedCommandParameters(DbCommand cmd, SqlString sqlString)
		{
			_driverImplementation.RemoveUnusedCommandParameters(cmd, sqlString);
		}

		void IDriver.ExpandQueryParameters(DbCommand cmd, SqlString sqlString)
		{
			_driverImplementation.ExpandQueryParameters(cmd, sqlString);
		}

		IResultSetsCommand IDriver.GetResultSetsCommand(ISessionImplementor session)
		{
			return _driverImplementation.GetResultSetsCommand(session);
		}

		bool IDriver.SupportsMultipleQueries => _driverImplementation.SupportsMultipleQueries;

		void IDriver.AdjustCommand(DbCommand command)
		{
			_driverImplementation.AdjustCommand(command);
		}

		bool IDriver.RequiresTimeSpanForTime => _driverImplementation.RequiresTimeSpanForTime;

		bool IDriver.SupportsSystemTransactions => _driverImplementation.SupportsSystemTransactions;

		bool IDriver.SupportsNullEnlistment => _driverImplementation.SupportsNullEnlistment;

		bool IDriver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled =>
			_driverImplementation.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;

		bool IDriver.HasDelayedDistributedTransactionCompletion =>
			_driverImplementation.HasDelayedDistributedTransactionCompletion;

		DateTime IDriver.MinDate => _driverImplementation.MinDate;

		#endregion
	}
}
