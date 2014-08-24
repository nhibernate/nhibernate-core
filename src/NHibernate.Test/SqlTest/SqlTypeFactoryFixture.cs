using System.Data;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class SqlTypeFactoryFixture
	{
		[Test]
		[Description("Should cache constructed types")]
		public void GetSqlTypeWithPrecisionScale()
		{
			var st = SqlTypeFactory.GetSqlType(DbType.Decimal, 10, 2);
			Assert.That(st, Is.SameAs(SqlTypeFactory.GetSqlType(DbType.Decimal, 10, 2)));
			Assert.That(st, Is.Not.SameAs(SqlTypeFactory.GetSqlType(DbType.Decimal, 10, 1)));
			Assert.That(st, Is.Not.SameAs(SqlTypeFactory.GetSqlType(DbType.Double, 10, 2)));
		}
	}
}