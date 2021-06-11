using System.Data;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class PostgreSQLDialectFixture
	{
		[Test]
		public void WhenSqlTypeIsDateTimeOffset_PostgreSQLDialect_ReturnsTimestampTz()
		{
			var dialect = new PostgreSQLDialect();

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTimeOffSet), Is.EqualTo("timestamptz").IgnoreCase, "Default datetimeoffset");
		}
	}
}
