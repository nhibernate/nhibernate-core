using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Oracle8iDialectFixture
	{
		[Test]
		public void GetLimitStringWithTableStartingWithSelectKeyword()
		{
			var dialect = new Oracle8iDialect();
			var sqlString=new SqlString(@"select selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" as column3_2_ from ""SelectLimit"" selectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" as column3_2_ from ""SelectLimit"" selectlimi0_ ) where rownum <=1");
			var limited=dialect.GetLimitString(sqlString, null, new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithTableNotStartingWithSelectKeyword()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"select eselectlimi0.""Id"" as column1_2_,eselectlimi0.""FirstName"" as column2_2_,eselectlimi0.""LastName"" as column3_2_ from ""SelectLimit"" eselectlimi0");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select eselectlimi0.""Id"" as column1_2_,eselectlimi0.""FirstName"" as column2_2_,eselectlimi0.""LastName"" as column3_2_ from ""SelectLimit"" eselectlimi0 ) where rownum <=1");
			
			var limited = dialect.GetLimitString(sqlString, null, new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}
	}
}