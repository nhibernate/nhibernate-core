using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
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
	[TestFixture]
	public abstract class AbstractDateTimeTypeFixture : TypeFixtureBase
	{
		protected abstract AbstractDateTimeType Type { get; }
		protected virtual bool RevisionCheck => true;

		protected const int DateId = 1;
		protected const int AdditionalDateId = 2;

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
					Id = DateId,
					Value = Now
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
		public void Next()
		{
			var current = DateTime.Parse("2004-01-01");
			var next = Type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTime>(), "next should be DateTime");
			Assert.That(next, Is.GreaterThan(current), "next should be greater than current");
		}

		[Test]
		public void Seed()
		{
			Assert.That(Type.Seed(null), Is.TypeOf<DateTime>(), "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			var value1 = DateTime.Now;
			var value2 = Type.DeepCopy(value1, null);

			Assert.That(value2, Is.EqualTo(value1), "Copies should be the same.");

			// Bit moot with structs...
			value2 = ((DateTime) value2).AddHours(2);
			Assert.That(value2, Is.Not.EqualTo(value1), "value2 was changed, value1 should not have changed also.");
		}

		[Test]
		[TestCase(DateTimeKind.Unspecified)]
		[TestCase(DateTimeKind.Local)]
		[TestCase(DateTimeKind.Utc)]
		public void Equality(DateTimeKind kind)
		{
			var testDate = GetTestDate(kind);
			var sameDate = GetSameDate(testDate);
			Assert.That(Type.IsEqual(testDate, sameDate), Is.True);
		}

		[Test]
		[TestCase(DateTimeKind.Unspecified)]
		[TestCase(DateTimeKind.Local)]
		[TestCase(DateTimeKind.Utc)]
		public void Inequality(DateTimeKind kind)
		{
			var testDate = GetTestDate(kind);
			var diffDate = GetDifferentDate(testDate);
			Assert.That(Type.IsEqual(testDate, diffDate), Is.False);
		}

		[Test]
		[TestCase(DateTimeKind.Unspecified)]
		[TestCase(DateTimeKind.Local)]
		[TestCase(DateTimeKind.Utc)]
		public void ReadWrite(DateTimeKind kind)
		{
			var entity = new DateTimeClass
			{
				Id = AdditionalDateId,
				Value = GetTestDate(kind)
			};

			var typeKind = GetTypeKind();
			// Now must be acquired before transaction because some db freezes current_timestamp at transaction start,
			// like PostgreSQL. https://www.postgresql.org/docs/7.2/static/functions-datetime.html#AEN6700
			// This then wrecks tests with DbTimestampType if the always out of tran Now is called for fetching
			// beforeNow only after transaction start.
			// And account db accuracy
			var beforeNow = Now.AddTicks(-DateAccuracyInTicks);
			// Save
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(entity);
				if (kind != typeKind && typeKind != DateTimeKind.Unspecified)
				{
					Assert.That(() => t.Commit(), Throws.TypeOf<PropertyValueException>());
					return;
				}
				t.Commit();
			}
			var afterNow = Now.AddTicks(DateAccuracyInTicks);

			if (RevisionCheck)
			{
				Assert.That(entity.Revision, Is.GreaterThan(beforeNow).And.LessThan(afterNow), "Revision not correctly seeded.");
				if (typeKind != DateTimeKind.Unspecified)
					Assert.That(entity.Revision.Kind, Is.EqualTo(typeKind), "Revision kind not correctly seeded.");
				Assert.That(entity.NullableValue, Is.Null, "NullableValue unexpectedly seeded.");
			}

			// Retrieve, compare then update
			DateTimeClass retrieved;
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					retrieved = s.Get<DateTimeClass>(AdditionalDateId);

					Assert.That(retrieved, Is.Not.Null, "Entity not saved or cannot be retrieved by its key.");
					Assert.That(retrieved.Value, Is.EqualTo(GetExpectedValue(entity.Value)), "Unexpected value.");
					if (RevisionCheck)
						Assert.That(retrieved.Revision, Is.EqualTo(entity.Revision), "Revision should be the same.");
					Assert.That(retrieved.NullableValue, Is.EqualTo(entity.NullableValue), "NullableValue should be the same.");
					if (typeKind != DateTimeKind.Unspecified)
					{
						Assert.That(retrieved.Value.Kind, Is.EqualTo(typeKind), "Value kind not correctly retrieved.");
						if (RevisionCheck)
							Assert.That(retrieved.Revision.Kind, Is.EqualTo(typeKind), "Revision kind not correctly retrieved.");
					}
					t.Commit();
				}
				beforeNow = Now.AddTicks(-DateAccuracyInTicks);
				using (var t = s.BeginTransaction())
				{
					retrieved.NullableValue = GetTestDate(kind);
					retrieved.Value = GetTestDate(kind).AddMonths(-1);
					t.Commit();
				}
				afterNow = Now.AddTicks(DateAccuracyInTicks);
			}

			if (RevisionCheck)
			{
				Assert.That(
					retrieved.Revision,
					Is.GreaterThan(beforeNow).And.LessThan(afterNow).And.GreaterThanOrEqualTo(entity.Revision),
					"Revision not correctly incremented.");
				if (typeKind != DateTimeKind.Unspecified)
					Assert.That(retrieved.Revision.Kind, Is.EqualTo(typeKind), "Revision kind incorrectly changed.");
			}

			// Retrieve and compare again
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var retrievedAgain = s.Get<DateTimeClass>(AdditionalDateId);

				Assert.That(retrievedAgain, Is.Not.Null, "Entity deleted or cannot be retrieved again by its key.");
				Assert.That(
					retrievedAgain.Value,
					Is.EqualTo(GetExpectedValue(retrieved.Value)),
					"Unexpected value at second compare.");
				if (RevisionCheck)
					Assert.That(retrievedAgain.Revision, Is.EqualTo(retrieved.Revision), "Revision should be the same again.");
				Assert.That(
					retrievedAgain.NullableValue,
					Is.EqualTo(GetExpectedValue(retrieved.NullableValue.Value)),
					"Unexpected NullableValue at second compare.");
				if (typeKind != DateTimeKind.Unspecified)
				{
					Assert.That(retrievedAgain.Value.Kind, Is.EqualTo(typeKind), "Value kind not correctly retrieved again.");
					if (RevisionCheck)
						Assert.That(retrievedAgain.Revision.Kind, Is.EqualTo(typeKind), "Revision kind not correctly retrieved again.");
					Assert.That(
						retrievedAgain.NullableValue.Value.Kind,
						Is.EqualTo(typeKind),
						"NullableValue kind not correctly retrieved again.");
				}
				t.Commit();
			}
		}

		[Test]
		public void PropertiesHasExpectedType()
		{
			var classMetaData = Sfi.GetClassMetadata(typeof(DateTimeClass));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeClass.Revision)),
				Is.EqualTo(Type));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeClass.Value)),
				Is.EqualTo(Type));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeClass.NullableValue)),
				Is.EqualTo(Type));
		}

		[Test]
		public void DbHasExpectedType()
		{
			var validator = new SchemaValidator(cfg);
			validator.Validate();
		}

		[Test]
		public virtual void SaveUseExpectedSqlType()
		{
			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var d = new DateTimeClass
				{
					Id = 2,
					Value = Now,
					NullableValue = Now
				};
				driver.ClearStats();
				s.Save(d);
				t.Commit();
			}

			// 2 properties + revision
			AssertSqlType(driver, 3, true);
		}

		[Test]
		public virtual void UpdateUseExpectedSqlType()
		{
			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var d = s.Get<DateTimeClass>(DateId);
				d.Value = Now;
				d.NullableValue = Now;
				driver.ClearStats();
				t.Commit();
			}

			// 2 properties + revision x 2 (check + update)
			AssertSqlType(driver, 4, true);
		}

		[Test]
		public virtual void QueryUseExpectedSqlType()
		{
			if (!TestDialect.SupportsNonDataBoundCondition)
				Assert.Ignore("Dialect does not support the test query");

			var driver = (ClientDriverWithParamsStats) Sfi.ConnectionProvider.Driver;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var q = s
					.CreateQuery(
						"from DateTimeClass d where d.Value = :value and " +
						"d.NullableValue = :nullableValue and " +
						"d.Revision = :revision and " +
						":other1 = :other2")
					.SetDateTime("value", Now)
					.SetDateTime("nullableValue", Now)
					.SetDateTime("revision", Now)
					.SetDateTime("other1", Now)
					.SetDateTime("other2", Now);
				driver.ClearStats();
				q.List<DateTimeClass>();
				t.Commit();
			}

			AssertSqlType(driver, 5, false);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var q = s
					.CreateQuery(
						"from DateTimeClass d where d.Value = :value and " +
						"d.NullableValue = :nullableValue and " +
						"d.Revision = :revision and " +
						":other1 = :other2")
					.SetParameter("value", Now, Type)
					.SetParameter("nullableValue", Now, Type)
					.SetParameter("revision", Now, Type)
					.SetParameter("other1", Now, Type)
					.SetParameter("other2", Now, Type);
				driver.ClearStats();
				q.List<DateTimeClass>();
				t.Commit();
			}

			AssertSqlType(driver, 5, true);
		}

		/// <summary>
		/// Tests if the type FromStringValue implementation behaves as expected.
		/// </summary>
		/// <param name="timestampValue"></param>
		[Test]
		[TestCase("2011-01-27T15:50:59.6220000+02:00")]
		[TestCase("2011-01-27T14:50:59.6220000+01:00")]
		[TestCase("2011-01-27T13:50:59.6220000Z")]
		public void FromStringValue_ParseValidValues(string timestampValue)
		{
			var timestamp = DateTime.Parse(timestampValue);

			Assert.That(
				timestamp.Kind,
				Is.EqualTo(DateTimeKind.Local),
				"Kind is NOT Local. dotnet framework parses datetime values with kind set to Local and " +
				"time correct to local timezone.");

			var typeKind = GetTypeKind();
			if (typeKind == DateTimeKind.Utc)
				timestamp = timestamp.ToUniversalTime();

			var value = (DateTime) Type.FromStringValue(timestampValue);

			Assert.That(value, Is.EqualTo(timestamp), timestampValue);

			if (typeKind != DateTimeKind.Unspecified)
				Assert.AreEqual(GetTypeKind(), value.Kind, "Unexpected FromStringValue kind");
		}

		/// <summary>
		/// Test the framework IsEqual behavior. If the test fails then the <see cref="AbstractDateTimeType"/>
		/// and <see cref="DateTimeNoMsType"/> implemention could not work propertly at run-time.
		/// </summary>
		[Test, Category("Expected framework behavior")]
		public void ExpectedIsEqualDotnetFrameworkBehavior()
		{
			const string assertMessage = "Values should be equal dotnet framework ignores Kind value.";
			var utc = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Utc);
			var local = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Local);
			var unspecified = new DateTime(1976, 11, 30, 10, 0, 0, 300, DateTimeKind.Unspecified);
			Assert.That(utc, Is.EqualTo(local), assertMessage);
			Assert.That(utc, Is.EqualTo(unspecified), assertMessage);
			Assert.That(unspecified, Is.EqualTo(local), assertMessage);
		}

		private void AssertSqlType(ClientDriverWithParamsStats driver, int expectedCount, bool exactType)
		{
			var typeSqlTypes = Type.SqlTypes(Sfi);
			if (typeSqlTypes.Any(t => t is DateTime2SqlType))
			{
				var expectedType = exactType ? typeSqlTypes.First(t => t is DateTime2SqlType) : SqlTypeFactory.DateTime2;
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime),
					Is.EqualTo(0),
					"Found unexpected SqlTypeFactory.DateTime usages.");
				Assert.That(
					driver.GetCount(expectedType),
					Is.EqualTo(expectedCount),
					"Unexpected SqlTypeFactory.DateTime2 usage count.");
				Assert.That(driver.GetCount(DbType.DateTime), Is.EqualTo(0), "Found unexpected DbType.DateTime usages.");
				Assert.That(
					driver.GetCount(expectedType),
					Is.EqualTo(expectedCount),
					"Unexpected DbType.DateTime2 usage count.");
			}
			else if (typeSqlTypes.Any(t => t is DateTimeSqlType))
			{
				var expectedType = exactType ? typeSqlTypes.First(t => t is DateTimeSqlType) : SqlTypeFactory.DateTime;
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime2),
					Is.EqualTo(0),
					"Found unexpected SqlTypeFactory.DateTime2 usages.");
				Assert.That(
					driver.GetCount(expectedType),
					Is.EqualTo(expectedCount),
					"Unexpected SqlTypeFactory.DateTime usage count.");
				Assert.That(driver.GetCount(DbType.DateTime2), Is.EqualTo(0), "Found unexpected DbType.DateTime2 usages.");
				Assert.That(driver.GetCount(expectedType), Is.EqualTo(expectedCount), "Unexpected DbType.DateTime usage count.");
			}
			else if (typeSqlTypes.Any(t => Equals(t, SqlTypeFactory.Date)))
			{
				Assert.That(
					driver.GetCount(SqlTypeFactory.DateTime),
					Is.EqualTo(0),
					"Found unexpected SqlTypeFactory.DateTime usages.");
				Assert.That(
					driver.GetCount(SqlTypeFactory.Date),
					Is.EqualTo(expectedCount),
					"Unexpected SqlTypeFactory.Date usage count.");
				Assert.That(driver.GetCount(DbType.DateTime), Is.EqualTo(0), "Found unexpected DbType.DateTime usages.");
				Assert.That(driver.GetCount(DbType.Date), Is.EqualTo(expectedCount), "Unexpected DbType.Date usage count.");
			}
			else
			{
				Assert.Ignore("Test does not involve tested types");
			}
		}

		protected virtual long DateAccuracyInTicks => Dialect.TimestampResolutionInTicks;

		protected virtual DateTime Now => GetTypeKind() == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

		protected virtual DateTime GetTestDate(DateTimeKind kind)
		{
			return AbstractDateTimeType.Round(
					kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.SpecifyKind(DateTime.Now, kind),
					DateAccuracyInTicks)
				// Take another date than now for checking the value do not get overridden by seeding.
				.AddDays(1);
		}

		private DateTime GetExpectedValue(DateTime value)
		{
			var expectedValue = value;
			var typeKind = GetTypeKind();
			if (typeKind != DateTimeKind.Unspecified && typeKind != value.Kind && value.Kind != DateTimeKind.Unspecified)
			{
				expectedValue = typeKind == DateTimeKind.Local ? expectedValue.ToLocalTime() : expectedValue.ToUniversalTime();
			}
			return expectedValue;
		}

		/// <summary>
		/// Return a date time still considered equal but as different as possible.
		/// </summary>
		/// <param name="original">The originale date time.</param>
		/// <returns>An equal date time.</returns>
		protected virtual DateTime GetSameDate(DateTime original)
		{
			if (GetTypeKind() != DateTimeKind.Unspecified)
				return new DateTime(original.Ticks, original.Kind);

			switch (original.Kind)
			{
				case DateTimeKind.Local:
					return DateTime.SpecifyKind(original, DateTimeKind.Unspecified);
				case DateTimeKind.Unspecified:
					return DateTime.SpecifyKind(original, DateTimeKind.Utc);
				default:
					return DateTime.SpecifyKind(original, DateTimeKind.Local);
			}
		}

		/// <summary>
		/// Return a different date time but as few different as possible.
		/// </summary>
		/// <param name="original">The originale date time.</param>
		/// <returns>An inequal date time.</returns>
		protected virtual DateTime GetDifferentDate(DateTime original)
		{
			return original.AddTicks(DateAccuracyInTicks);
		}

		private static readonly PropertyInfo _kindProperty =
			typeof(AbstractDateTimeType).GetProperty("Kind", BindingFlags.Instance | BindingFlags.NonPublic);

		protected DateTimeKind GetTypeKind()
		{
			return (DateTimeKind) _kindProperty.GetValue(Type);
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
			// Firebird will pool each connection created during the test and will marked as used any table
			// referenced by queries. It will at best delays those tables drop until connections are actually
			// closed, or immediately fail dropping them.
			// This results in other tests failing when they try to create tables with same name.
			// By clearing the connection pool the tables will get dropped. This is done by the following code.
			// Moved from NH1908 test case, contributed by Amro El-Fakharany.
			_driverImplementation.ClearPoolForFirebirdClientDriver();
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

		void IDriver.ExpandQueryParameters(DbCommand cmd, SqlString sqlString, SqlType[] parameterTypes)
		{
			_driverImplementation.ExpandQueryParameters(cmd, sqlString, parameterTypes);
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
