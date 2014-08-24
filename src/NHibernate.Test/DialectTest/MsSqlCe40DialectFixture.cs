using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	public class MsSqlCe40DialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			var dialect = new MsSqlCe40Dialect();
			var str = dialect.GetLimitString(new SqlString("SELECT id FROM user ORDER BY name"), new SqlString("13"), new SqlString("17"));
			Assert.AreEqual("SELECT id FROM user ORDER BY name OFFSET 13 ROWS FETCH NEXT 17 ROWS ONLY", str.ToString());
		}

		[Test]
		public void GetLimitStringWithDummyOrder()
		{
			var dialect = new MsSqlCe40Dialect();
            var str = dialect.GetLimitString(new SqlString("SELECT id FROM user"), new SqlString("13"), new SqlString("17"));
			Assert.AreEqual("SELECT id FROM user ORDER BY GETDATE() OFFSET 13 ROWS FETCH NEXT 17 ROWS ONLY", str.ToString());
		}
	}
}
