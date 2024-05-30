using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3530;

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
		using var session = OpenSession();
		using var transaction = session.BeginTransaction();

		session.CreateQuery("delete from System.Object").ExecuteUpdate();

		transaction.Commit();
	}

	protected override void CreateSchema()
	{
		CreateTable("Integer");
		CreateTable("DateTime");
		CreateTable("Double");
		CreateTable("Decimal");
		CreateTable("Float");

		base.CreateSchema();
	}

	/// <summary>
	/// This function creates the schema for our custom entities.
	/// If the SchemaExporter provided a mechanism to override the database
	/// type, this method would not be required.
	/// </summary>
	/// <param name="name"></param>
	private void CreateTable(string name)
	{
		var sb = new StringBuilder();
		var guidType = Dialect.GetTypeName(SqlTypeFactory.Guid);
		var stringType = Dialect.GetTypeName(SqlTypeFactory.GetAnsiString(255));

		var catalog = GetQuotedDefaultCatalog();
		var schema = GetQuotedDefaultSchema();
		var table = GetQualifiedName(catalog, schema, $"{name}Entity");

		sb.Append($"{Dialect.CreateTableString} {table} (");

		// Generate columns
		sb.Append($"Id {guidType}, ");
		sb.Append($"DataValue {stringType}");

		// Add the primary key contraint for the identity column
		sb.Append($", {Dialect.PrimaryKeyString} ( Id )");
		sb.Append(')');

		using var cn = Sfi.ConnectionProvider.GetConnection();
		try
		{
			using var cmd = cn.CreateCommand();

			cmd.CommandText = sb.ToString();
			cmd.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			Assert.Warn($"Creating the schema failed, assuming it already exists. {ex}");
		}
		finally
		{
			Sfi.ConnectionProvider.CloseConnection(cn);
		}
	}

	private string GetQuotedDefaultCatalog()
	{
		var t = cfg.GetType();
		var getQuotedDefaultCatalog = t.GetMethod("GetQuotedDefaultCatalog", BindingFlags.Instance | BindingFlags.NonPublic);

		return (string) getQuotedDefaultCatalog.Invoke(cfg, [Dialect]);
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

	private void PerformTest<T, U>(CultureInfo from, CultureInfo to, T expectedValue, Action<T, T> assert)
		where T : struct
		where U : DataEntity<T>, new()
	{
		object id;

		CurrentCulture = from;
		using (var session = OpenSession())
		using (var tx = session.BeginTransaction())
		{
			var entity = new U()
			{
				DataValue = expectedValue
			};

			id = session.Save(entity);
			tx.Commit();
		}

		CurrentCulture = to;
		using (var session = OpenSession())
		using (var tx = session.BeginTransaction())
		{
			var entity = session.Get<U>(id);

			assert(expectedValue, entity.DataValue);
		}
	}

	[Test, TestCaseSource(nameof(GetTestCases))]
	public void TestNHDateTime(CultureInfo from, CultureInfo to)
	{
		var leapDay = new DateTime(2024, 2, 29, new GregorianCalendar(GregorianCalendarTypes.USEnglish));

		PerformTest<DateTime, NHDateTimeEntity>(from, to, leapDay, (expected, actual) => Assert.AreEqual(expected, actual));
	}

	[Test, TestCaseSource(nameof(GetTestCases))]
	public void TestDateTime(CultureInfo from, CultureInfo to)
	{
		var leapDay = new DateTime(2024, 2, 29, new GregorianCalendar(GregorianCalendarTypes.USEnglish));

		PerformTest<DateTime, DateTimeEntity>(from, to, leapDay, (expected, actual) => Assert.AreEqual(expected, actual));
	}

	[Test, TestCaseSource(nameof(GetTestCases))]
	public void TestDecimal(CultureInfo from, CultureInfo to)
	{
		decimal decimalValue = 12.3m;

		PerformTest<decimal, DecimalEntity>(from, to, decimalValue, (expected, actual) => Assert.AreEqual(expected, actual));
	}

	[Test, TestCaseSource(nameof(GetTestCases))]
	public void TestDouble(CultureInfo from, CultureInfo to)
	{
		double doubleValue = 12.3d;

		PerformTest<double, DoubleEntity>(from, to, doubleValue, 
			(expected, actual) => Assert.True(Math.Abs(expected - actual) < double.Epsilon, $"Expected {expected} but was {actual}\n")
		);
	}

	[Test, TestCaseSource(nameof(GetTestCases))]

	public void TestInteger(CultureInfo from, CultureInfo to)
	{
		int integerValue = 123;

		PerformTest<int, IntegerEntity>(from, to, integerValue, (expected, actual) => Assert.AreEqual(expected, actual));
	}

	[Test, TestCaseSource(nameof(GetTestCases))]
	public void TestFloat(CultureInfo from, CultureInfo to)
	{
		float floatValue = 12.3f;

		PerformTest<float, FloatEntity>(from, to, floatValue,
			(expected, actual) => Assert.True(Math.Abs(expected - actual) < float.Epsilon, $"Expected {expected} but was {actual}\n")
		);
	}

	private CultureInfo CurrentCulture
	{
		get => CultureInfo.CurrentCulture;
		set => CultureInfo.CurrentCulture = value;
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
