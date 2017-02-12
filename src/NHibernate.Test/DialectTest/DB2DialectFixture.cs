using System;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class DB2DialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			DB2Dialect dialect = new DB2Dialect();
			SqlString sql = new SqlString(
				new object[]
					{
						"select a, b, c ",
						"from d",
						" where X = ",
						Parameter.Placeholder,
						" and Z = ",
						Parameter.Placeholder,
						" order by a, x"
					});

			SqlString limited = dialect.GetLimitString(sql, new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"select * from (select rownumber() over(order by a, x) as rownum, a, b, c from d where X = ? and Z = ? order by a, x) as tempresult where rownum between 111+1 and 222",
				limited.ToString());
			Assert.AreEqual(2, limited.GetParameterCount());
		}

		[Test]
		public void GetLimitString_NoOffsetSpecified_UsesFetchFirstOnly()
		{
			// arrange
			DB2Dialect dialect = new DB2Dialect();
			SqlString sql = new SqlString(
				new object[]
					{
						"select a, b, c ",
						"from d",
						" where X = ",
						Parameter.Placeholder,
						" and Z = ",
						Parameter.Placeholder,
						" order by a, x"
					});

			// act
			SqlString limited = dialect.GetLimitString(sql, null, new SqlString("222"));

			// assert
			Assert.AreEqual(
				"select a, b, c from d where X = ? and Z = ? order by a, x fetch first 222 rows only",
				limited.ToString());
			Assert.AreEqual(2, limited.GetParameterCount());
		}
	}
}