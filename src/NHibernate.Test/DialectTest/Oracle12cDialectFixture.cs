using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class oracle12cDialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			var d = new Oracle12cDialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT fish.id FROM fish OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id FROM fish fish_ OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_ OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish ORDER BY name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT fish.id, fish.name FROM fish ORDER BY name DESC OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish WHERE scales = ? OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name) OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void GetLimitStringWithInnerOrder()
		{
			var d = new Oracle12cDialect();

			var str = d.GetLimitString(new SqlString("SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void OnlyOffsetLimit()
		{
			var d = new Oracle12cDialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), null, new SqlString("10"));
			Assert.That(str.ToString(), Is.EqualTo("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name FETCH FIRST 10 ROWS ONLY"));
		}

		[Test]
		public void GetLimitStringWithSqlComments()
		{
			var d = new Oracle12cDialect();
			var limitSqlQuery = d.GetLimitString(new SqlString(" /* criteria query */ SELECT p from lcdtm"), null, new SqlString("2"));
			Assert.That(limitSqlQuery, Is.Not.Null);
			Assert.That(limitSqlQuery.ToString(), Is.EqualTo(" /* criteria query */ SELECT p from lcdtm FETCH FIRST 2 ROWS ONLY"));
		}
	}
}