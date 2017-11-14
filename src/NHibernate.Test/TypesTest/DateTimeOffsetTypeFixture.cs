using System;
using System.Data;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.SqlTypes;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class DateTimeOffsetTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "DateTimeOffset";
		protected virtual DateTimeOffsetType Type => NHibernateUtil.DateTimeOffset;
		protected virtual bool RevisionCheck => true;

		protected const int DateId = 1;
		protected const int AdditionalDateId = 2;

		protected override bool AppliesTo(Dialect.Dialect dialect) =>
			TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet);

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory) =>
			// Cannot handle DbType.DateTimeOffset via .Net ODBC.
			!(factory.ConnectionProvider.Driver is OdbcDriver);

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
				var d = new DateTimeOffsetClass
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
				s.CreateQuery("delete from DateTimeOffsetClass").ExecuteUpdate();
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
			var current = DateTimeOffset.Parse("2004-01-01");
			var next = Type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTimeOffset>(), "next should be DateTimeOffset");
			Assert.That(next, Is.GreaterThan(current), "next should be greater than current");
		}

		[Test]
		public void Seed()
		{
			Assert.That(Type.Seed(null), Is.TypeOf<DateTimeOffset>(), "seed should be DateTime");
		}

		[Test]
		public void DeepCopyNotNull()
		{
			var value1 = DateTimeOffset.Now;
			var value2 = Type.DeepCopy(value1, null);

			Assert.That(value2, Is.EqualTo(value1), "Copies should be the same.");
		}

		[Test]
		public void Equality()
		{
			var testDate = GetTestDate();
			var sameDate = GetSameDate(testDate);
			Assert.That(Type.IsEqual(testDate, sameDate), Is.True);
		}

		[Test]
		public void Inequality()
		{
			var testDate = GetTestDate();
			var diffDate = GetDifferentDate(testDate);
			Assert.That(Type.IsEqual(testDate, diffDate), Is.False);
		}

		[Test]
		public void ReadWrite()
		{
			var entity = new DateTimeOffsetClass
			{
				Id = AdditionalDateId,
				Value = GetTestDate()
			};

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
				t.Commit();
			}
			var afterNow = Now.AddTicks(DateAccuracyInTicks);

			if (RevisionCheck)
			{
				Assert.That(entity.Revision, Is.GreaterThan(beforeNow).And.LessThan(afterNow), "Revision not correctly seeded.");
				Assert.That(entity.NullableValue, Is.Null, "NullableValue unexpectedly seeded.");
			}

			// Retrieve, compare then update
			DateTimeOffsetClass retrieved;
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					retrieved = s.Get<DateTimeOffsetClass>(AdditionalDateId);

					Assert.That(retrieved, Is.Not.Null, "Entity not saved or cannot be retrieved by its key.");
					Assert.That(retrieved.Value, Is.EqualTo(entity.Value), "Unexpected value.");
					if (RevisionCheck)
						Assert.That(retrieved.Revision, Is.EqualTo(entity.Revision), "Revision should be the same.");
					Assert.That(retrieved.NullableValue, Is.EqualTo(entity.NullableValue), "NullableValue should be the same.");
					t.Commit();
				}
				beforeNow = Now.AddTicks(-DateAccuracyInTicks);
				using (var t = s.BeginTransaction())
				{
					retrieved.NullableValue = GetTestDate();
					retrieved.Value = GetTestDate().AddMonths(-1);
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
			}

			// Retrieve and compare again
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var retrievedAgain = s.Get<DateTimeOffsetClass>(AdditionalDateId);

				Assert.That(retrievedAgain, Is.Not.Null, "Entity deleted or cannot be retrieved again by its key.");
				Assert.That(
					retrievedAgain.Value,
					Is.EqualTo(retrieved.Value),
					"Unexpected value at second compare.");
				if (RevisionCheck)
					Assert.That(retrievedAgain.Revision, Is.EqualTo(retrieved.Revision), "Revision should be the same again.");
				Assert.That(
					retrievedAgain.NullableValue,
					Is.EqualTo(retrieved.NullableValue.Value),
					"Unexpected NullableValue at second compare.");
				t.Commit();
			}
		}

		[Test]
		public void PropertiesHasExpectedType()
		{
			var classMetaData = Sfi.GetClassMetadata(typeof(DateTimeOffsetClass));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeOffsetClass.Revision)),
				Is.EqualTo(Type));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeOffsetClass.Value)),
				Is.EqualTo(Type));
			Assert.That(
				classMetaData.GetPropertyType(nameof(DateTimeOffsetClass.NullableValue)),
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
				var d = new DateTimeOffsetClass
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
				var d = s.Get<DateTimeOffsetClass>(DateId);
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
						"from DateTimeOffsetClass d where d.Value = :value and " +
						"d.NullableValue = :nullableValue and " +
						"d.Revision = :revision and " +
						":other1 = :other2")
					.SetDateTimeOffset("value", Now)
					.SetDateTimeOffset("nullableValue", Now)
					.SetDateTimeOffset("revision", Now)
					.SetDateTimeOffset("other1", Now)
					.SetDateTimeOffset("other2", Now);
				driver.ClearStats();
				q.List<DateTimeOffsetClass>();
				t.Commit();
			}

			AssertSqlType(driver, 5, false);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var q = s
					.CreateQuery(
						"from DateTimeOffsetClass d where d.Value = :value and " +
						"d.NullableValue = :nullableValue and " +
						"d.Revision = :revision and " +
						":other1 = :other2")
					.SetParameter("value", Now, Type)
					.SetParameter("nullableValue", Now, Type)
					.SetParameter("revision", Now, Type)
					.SetParameter("other1", Now, Type)
					.SetParameter("other2", Now, Type);
				driver.ClearStats();
				q.List<DateTimeOffsetClass>();
				t.Commit();
			}

			AssertSqlType(driver, 5, true);
		}

		private void AssertSqlType(ClientDriverWithParamsStats driver, int expectedCount, bool exactType)
		{
			var typeSqlTypes = Type.SqlTypes(Sfi);
			if (typeSqlTypes.Any(t => t is DateTimeOffsetSqlType))
			{
				var expectedType = exactType ? typeSqlTypes.First(t => t is DateTimeOffsetSqlType) : SqlTypeFactory.DateTimeOffSet;
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
			else
			{
				Assert.Ignore("Test does not involve tested types");
			}
		}

		protected virtual long DateAccuracyInTicks => Dialect.TimestampResolutionInTicks;

		protected virtual DateTimeOffset Now => DateTimeOffset.Now;

		protected virtual DateTimeOffset GetTestDate()
		{
			return DateTimeOffsetType.Round(Now, DateAccuracyInTicks)
				// Take another date than now for checking the value do not get overridden by seeding.
				.AddDays(1);
		}

		/// <summary>
		/// Return a date time still considered equal but as different as possible.
		/// </summary>
		/// <param name="original">The originale date time.</param>
		/// <returns>An equal date time.</returns>
		protected virtual DateTimeOffset GetSameDate(DateTimeOffset original)
		{
			return new DateTimeOffset(original.Ticks, original.Offset);
		}

		/// <summary>
		/// Return a different date time but as few different as possible.
		/// </summary>
		/// <param name="original">The originale date time.</param>
		/// <returns>An inequal date time.</returns>
		protected virtual DateTimeOffset GetDifferentDate(DateTimeOffset original)
		{
			return original.AddTicks(DateAccuracyInTicks);
		}
	}

	[TestFixture]
	public class DateTimeOffsetTypeWithScaleFixture : DateTimeOffsetTypeFixture
	{
		protected override string TypeName => "DateTimeOffsetWithScale";
		protected override DateTimeOffsetType Type => (DateTimeOffsetType)TypeFactory.GetDateTimeOffsetType(3);
		protected override long DateAccuracyInTicks => Math.Max(TimeSpan.TicksPerMillisecond, base.DateAccuracyInTicks);
		// The timestamp rounding in seeding does not account scale.
		protected override bool RevisionCheck => false;

		[Test]
		public void LowerDigitsAreIgnored()
		{
			if (!Dialect.SupportsDateTimeScale)
				Assert.Ignore("Lower digits cannot be ignored when dialect does not support scale");

			var baseDate = new DateTimeOffset(2017, 10, 01, 17, 55, 24, 548, TimeSpan.FromHours(2));
			var entity = new DateTimeOffsetClass
			{
				Id = AdditionalDateId,
				Value = baseDate.AddTicks(TimeSpan.TicksPerMillisecond / 3)
			};
			Assert.That(entity.Value, Is.Not.EqualTo(baseDate));

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(entity);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var retrieved = s.Load<DateTimeOffsetClass>(AdditionalDateId);
				Assert.That(retrieved.Value, Is.EqualTo(baseDate));
				t.Commit();
			}
		}
	}
}
