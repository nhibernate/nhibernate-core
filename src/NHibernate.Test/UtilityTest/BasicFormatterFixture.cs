using NHibernate.AdoNet.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class BasicFormatterFixture
	{
		[Test]
		public void StringWithNestedDelimiters()
		{
			string formattedSql = null;
			IFormatter formatter = new BasicFormatter();
			string sql = @"INSERT INTO Table (Name, id) VALUES (@p0, @p1); @p0 = 'a'(b', @p1 = 1";
			Assert.DoesNotThrow(() => formattedSql = formatter.Format(sql));
			Assert.That(formattedSql, Is.StringContaining("'a'(b'"));

			sql = @"UPDATE Table SET Column = @p0;@p0 = '(')'";
			Assert.DoesNotThrow(() => formattedSql = formatter.Format(sql));
			Assert.That(formattedSql, Is.StringContaining("'(')'"));
		}
	}
}
