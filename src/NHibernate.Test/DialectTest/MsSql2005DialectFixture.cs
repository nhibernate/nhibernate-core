using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class MsSql2005DialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();

			SqlString str = d.GetLimitString(new SqlString("SELECT * FROM fish"), 0, 10);
			Assert.AreEqual("WITH query AS (SELECT TOP 10 ROW_NUMBER() OVER (ORDER BY CURRENT_TIMESTAMP) as __hibernate_row_nr__,  * FROM fish) SELECT * FROM query WHERE __hibernate_row_nr__ > 0 ORDER BY __hibernate_row_nr__", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT * FROM fish"), 0, 10);
			Assert.AreEqual("WITH query AS (SELECT DISTINCT TOP 10 ROW_NUMBER() OVER (ORDER BY CURRENT_TIMESTAMP) as __hibernate_row_nr__,  * FROM fish) SELECT * FROM query WHERE __hibernate_row_nr__ > 0 ORDER BY __hibernate_row_nr__", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), 5, 15);
			Assert.AreEqual("WITH query AS (SELECT TOP 15 ROW_NUMBER() OVER (ORDER BY name) as __hibernate_row_nr__,  * FROM fish ORDER BY name) SELECT * FROM query WHERE __hibernate_row_nr__ > 5 ORDER BY __hibernate_row_nr__", str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name DESC"), 7, 28);
			Assert.AreEqual("WITH query AS (SELECT TOP 28 ROW_NUMBER() OVER (ORDER BY name DESC) as __hibernate_row_nr__,  * FROM fish ORDER BY name DESC) SELECT * FROM query WHERE __hibernate_row_nr__ > 7 ORDER BY __hibernate_row_nr__", str.ToString());
		}
	}
}
