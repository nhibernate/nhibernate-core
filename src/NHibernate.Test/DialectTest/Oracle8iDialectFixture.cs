using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class Oracle8iDialectFixture
	{
		#region Limit only
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
			var sqlString = new SqlString(@"select eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" as column3_2_ from ""ESelectLimit"" eselectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" as column3_2_ from ""ESelectLimit"" eselectlimi0_ ) where rownum <=1");
			
			var limited = dialect.GetLimitString(sqlString, null, new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithTableStartingWithSelectKeywordAndDifferentCasing()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"sElEct selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" As column3_2_ fRom ""SelectLimit"" selectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( sElEct selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" As column3_2_ fRom ""SelectLimit"" selectlimi0_ ) where rownum <=1");
			var limited = dialect.GetLimitString(sqlString, null, new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithTableNotStartingWithSelectKeywordAndDifferentCasing()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"sElEct eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" As column3_2_ fRom ""ESelectLimit"" eselectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( sElEct eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" As column3_2_ fRom ""ESelectLimit"" eselectlimi0_ ) where rownum <=1");

			var limited = dialect.GetLimitString(sqlString, null, new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}
		#endregion
		#region Offset And List
		[Test]
		public void GetLimitStringWithOffsetAndLimitAndTableStartingWithSelectKeyword()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"select selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" as column3_2_ from ""SelectLimit"" selectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select row_.*, rownum rownum_ from ( select selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" as column3_2_ from ""SelectLimit"" selectlimi0_ ) row_ where rownum <=1) where rownum_ >1");
			var limited = dialect.GetLimitString(sqlString, new SqlString("1"), new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithOffsetAndLimitAndTableNotStartingWithSelectKeyword()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"select eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" as column3_2_ from ""ESelectLimit"" eselectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select row_.*, rownum rownum_ from ( select eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" as column3_2_ from ""ESelectLimit"" eselectlimi0_ ) row_ where rownum <=1) where rownum_ >1");

			var limited = dialect.GetLimitString(sqlString, new SqlString("1"), new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithOffsetAndLimitAndTableStartingWithSelectKeywordAndDifferentCasing()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"sElEct selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" As column3_2_ fRom ""SelectLimit"" selectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select row_.*, rownum rownum_ from ( sElEct selectlimi0_.""Id"" as column1_2_,selectlimi0_.""FirstName"" as column2_2_,selectlimi0_.""LastName"" As column3_2_ fRom ""SelectLimit"" selectlimi0_ ) row_ where rownum <=1) where rownum_ >1");
			var limited = dialect.GetLimitString(sqlString, new SqlString("1"), new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}

		[Test]
		public void GetLimitStringWithOffsetAndLimitAndTableNotStartingWithSelectKeywordAndDifferentCasing()
		{
			var dialect = new Oracle8iDialect();
			var sqlString = new SqlString(@"sElEct eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" As column3_2_ fRom ""ESelectLimit"" eselectlimi0_");
			var expected = new SqlString(@"select column1_2_,column2_2_,column3_2_ from ( select row_.*, rownum rownum_ from ( sElEct eselectlimi0_.""Id"" as column1_2_,eselectlimi0_.""FirstName"" as column2_2_,eselectlimi0_.""LastName"" As column3_2_ fRom ""ESelectLimit"" eselectlimi0_ ) row_ where rownum <=1) where rownum_ >1");

			var limited = dialect.GetLimitString(sqlString, new SqlString("1"), new SqlString("1"));
			Assert.AreEqual(limited, expected);
		}
		#endregion
	}
}