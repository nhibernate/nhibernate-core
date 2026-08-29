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
	}
}
