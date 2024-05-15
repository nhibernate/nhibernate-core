using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3530
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private CultureInfo initialCulture;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			initialCulture = CurrentCulture;
		}

		[OneTimeTearDown]
		public void FixtureTearDown()
		{
			CurrentCulture = initialCulture;
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		protected override void CreateSchema()
		{
			var sb = new StringBuilder();
			var intType = Dialect.GetTypeName(SqlTypeFactory.Int32);
			var stringType = Dialect.GetTypeName(SqlTypeFactory.GetAnsiString(255));

			var catalog = GetQuotedDefaultCatalog();
			var schema = GetQuotedDefaultSchema();
			var table = GetQualifiedName(catalog, schema, "LocaleEntity");

			sb.Append($"{Dialect.CreateTableString} {table} (");
			sb.Append($"Id ");

			if (Dialect.HasDataTypeInIdentityColumn)
			{
				sb.Append($"{intType}");
			}
			sb.Append($" {Dialect.GetIdentityColumnString(DbType.Int32)}, ");

			// Generate columns
			sb.Append($"IntegerValue {stringType}, ");
			sb.Append($"DateTimeValue {stringType}, ");
			sb.Append($"DoubleValue {stringType}, ");
			sb.Append($"DecimalValue {stringType}");

			// Add the primary key contraint for the identity column
			if (Dialect.GenerateTablePrimaryKeyConstraintForIdentityColumn)
			{
				sb.Append($", {Dialect.PrimaryKeyString} ( Id )");
			}
			sb.Append(")");

			using (var cn = Sfi.ConnectionProvider.GetConnection())
			{
				try
				{
					using (var cmd = cn.CreateCommand())
					{
						cmd.CommandText = sb.ToString();
						cmd.ExecuteNonQuery();
					}
				}
				finally
				{
					Sfi.ConnectionProvider.CloseConnection(cn);
				}
			}
		}

		private string GetQuotedDefaultCatalog()
		{
			var t = cfg.GetType();
			var getQuotedDefaultCatalog = t.GetMethod("GetQuotedDefaultCatalog", BindingFlags.Instance | BindingFlags.NonPublic);

			return (string)getQuotedDefaultCatalog.Invoke(cfg, [Dialect]);
		}

		private string GetQuotedDefaultSchema()
		{
			var t = cfg.GetType();
			var getQuotedDefaultSchema = t.GetMethod("GetQuotedDefaultSchema", BindingFlags.Instance | BindingFlags.NonPublic);

			return (string) getQuotedDefaultSchema.Invoke(cfg, [Dialect]);
		}

		private string GetQualifiedName(string catalog, string schema, string name)
		{
			return Dialect.Qualify(catalog, schema, name);
		}

		[TestCaseSource(nameof(GetTestCases))]
		public void TestLocales(CultureInfo from, CultureInfo to)
		{
			DateTime leapDay = new DateTime(2024, 2, 29, new GregorianCalendar(GregorianCalendarTypes.USEnglish));
			double doubleValue = 12.3f;
			int intValue = 4;
			object id;

			CurrentCulture = from;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new LocaleEntity()
				{
					DateTimeValue = leapDay,
					DoubleValue = doubleValue,
					IntegerValue = intValue,
				};

				id = session.Save(entity);
				tx.Commit();
			}

			CurrentCulture = to;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = session.Get<LocaleEntity>(id);

				Assert.AreEqual(leapDay, entity.DateTimeValue);
				Assert.AreEqual(intValue, entity.IntegerValue);
				Assert.True(doubleValue - entity.DoubleValue < double.Epsilon);
			}
		}

		private CultureInfo CurrentCulture
		{
			get
			{
				return CultureInfo.CurrentCulture;
			}
			set
			{
				CultureInfo.CurrentCulture = value;
			}
		}

		public static object[][] GetTestCases()
		{
			return [
				[new CultureInfo("en-US"), new CultureInfo("de-DE")],
				[new CultureInfo("en-US"), new CultureInfo("ar-SA", false)],
				[new CultureInfo("en-US"), new CultureInfo("th-TH", false)],
			];
		}
	}
}
