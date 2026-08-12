using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Firebird3DialectFixture
	{
		private readonly Firebird3Dialect _dialect = new Firebird3Dialect();

		[Test]
		public void GetLimitStringWithLimitOnly()
		{
			var str = _dialect.GetLimitString(new SqlString("select * from fish"), null, new SqlString("10"));
			Assert.That(str.ToString(), Is.EqualTo("select * from fish fetch first 10 rows only"));
		}

		[Test]
		public void GetLimitStringWithOffsetOnly()
		{
			var str = _dialect.GetLimitString(new SqlString("select * from fish order by name"), new SqlString("5"), null);
			Assert.That(str.ToString(), Is.EqualTo("select * from fish order by name offset 5 rows"));
		}

		[Test]
		public void GetLimitStringWithOffsetAndLimit()
		{
			var str = _dialect.GetLimitString(new SqlString("select * from fish order by name"), new SqlString("5"), new SqlString("15"));
			Assert.That(str.ToString(), Is.EqualTo("select * from fish order by name offset 5 rows fetch first 15 rows only"));
		}

		[Test]
		public void GetLimitStringWithParameters()
		{
			var str = _dialect.GetLimitString(
				new SqlString("select * from fish order by name"),
				5,
				15,
				Parameter.Placeholder,
				Parameter.Placeholder);

			Assert.That(str.ToString(), Is.EqualTo("select * from fish order by name offset ? rows fetch first ? rows only"));
		}

		[Test]
		public void LimitAndOffsetAreNotAdjusted()
		{
			// The standard offset/fetch clause takes a zero based offset and a row count.
			Assert.That(_dialect.OffsetStartsAtOne, Is.False, "OffsetStartsAtOne");
			Assert.That(_dialect.UseMaxForLimit, Is.False, "UseMaxForLimit");
			Assert.That(_dialect.SupportsVariableLimit, Is.True, "SupportsVariableLimit");
			Assert.That(_dialect.SupportsLimitOffset, Is.True, "SupportsLimitOffset");
		}
	}
}
