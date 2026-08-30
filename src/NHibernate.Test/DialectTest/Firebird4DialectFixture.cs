using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Firebird4DialectFixture
	{
		private readonly Firebird4Dialect _dialect = new Firebird4Dialect();

		[Test]
		public void CurrentTimestampIsNotTimeZoneAware()
		{
			Assert.That(_dialect.CurrentTimestampSQLFunctionName, Is.EqualTo("localtimestamp"));
			Assert.That(_dialect.CurrentTimestampSelectString, Is.EqualTo("select LOCALTIMESTAMP from RDB$DATABASE"));
		}

		[Test]
		public void GetTypeNameDecimalWithPrecisionGreaterThan18ReturnsThatPrecision()
		{
			var result = _dialect.GetTypeName(NHibernateUtil.Decimal.SqlType, 0, 29, 2);

			Assert.That(result, Is.EqualTo("DECIMAL(29, 2)"));
		}

		[Test]
		public void MaxAliasLengthIsRaisedToTheFirebird4Limit()
		{
			Assert.That(_dialect.MaxAliasLength, Is.EqualTo(63));
		}
	}
}
