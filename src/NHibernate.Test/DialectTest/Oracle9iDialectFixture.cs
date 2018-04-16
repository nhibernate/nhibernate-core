using System.Data;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Oracl9iDialectFixture
	{
		[Test]
		public void ScaleTypes()
		{
			const int @default = 7;
			const int min = 0;
			const int intermediate = 5;
			const int max = 9;
			var dialect = new Oracle9iDialect();

			Assert.That(dialect.GetTypeName(SqlTypeFactory.DateTime), Is.EqualTo($"timestamp({@default})").IgnoreCase, "Default datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(min)), Is.EqualTo($"timestamp({min})").IgnoreCase, "Min datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(intermediate)), Is.EqualTo($"timestamp({intermediate})").IgnoreCase, "Intermediate datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max)), Is.EqualTo($"timestamp({max})").IgnoreCase, "Max datetime");
			Assert.That(dialect.GetLongestTypeName(DbType.DateTime), Is.EqualTo($"timestamp({max})").IgnoreCase, "Longest datetime");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetDateTime(max + 1)), Is.EqualTo($"timestamp({@default})").IgnoreCase, "Over max datetime");

			Assert.That(dialect.GetTypeName(SqlTypeFactory.Time), Is.EqualTo($"timestamp({@default})").IgnoreCase, "Default time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(min)), Is.EqualTo($"timestamp({min})").IgnoreCase, "Min time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(intermediate)), Is.EqualTo($"timestamp({intermediate})").IgnoreCase, "Intermediate time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max)), Is.EqualTo($"timestamp({max})").IgnoreCase, "Max time");
			Assert.That(dialect.GetLongestTypeName(DbType.Time), Is.EqualTo($"timestamp({max})").IgnoreCase, "Longest time");
			Assert.That(dialect.GetTypeName(SqlTypeFactory.GetTime(max + 1)), Is.EqualTo($"timestamp({@default})").IgnoreCase, "Over max time");
		}
	}
}
