using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	[TestFixture]
	public class MsSql2012DialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			var d = new MsSql2012Dialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT fish.id FROM fish ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id FROM fish fish_ ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_ ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
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
				"SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish WHERE scales = ? ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name) OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void GetLimitStringWithInnerOrder()
		{
			var d = new MsSql2012Dialect();

			var str = d.GetLimitString(new SqlString("SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name ORDER BY CURRENT_TIMESTAMP OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void OnlyOffsetLimit()
		{
			var d = new MsSql2012Dialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), null, new SqlString("10"));
			Assert.That(str.ToString(), Is.EqualTo("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name OFFSET 0 ROWS FETCH FIRST 10 ROWS ONLY"));
		}

		[Test]
		public void GetLimitStringWithSqlComments()
		{
			var d = new MsSql2012Dialect();
			var limitSqlQuery = d.GetLimitString(new SqlString(" /* criteria query */ SELECT p from lcdtm"), null, new SqlString("2"));
			Assert.That(limitSqlQuery, Is.Not.Null);
			Assert.That(limitSqlQuery.ToString(), Is.EqualTo(" /* criteria query */ SELECT p from lcdtm ORDER BY CURRENT_TIMESTAMP OFFSET 0 ROWS FETCH FIRST 2 ROWS ONLY"));
		}

		[Test]
		public void GetLimitStringWithSqlCommonTableExpression()
		{
			const string SQL = @"
				WITH DirectReports (ManagerID, EmployeeID, Title, DeptID, Level)
				(   -- Anchor member definition
					SELECT  ManagerID, EmployeeID, Title, Deptid, 0 AS Level
					FROM    MyEmployees
					WHERE   ManagerID IS NULL
					
					UNION ALL
					
					-- Recursive member definition
					SELECT  e.ManagerID, e.EmployeeID, e.Title, e.Deptid, Level + 1
					FROM    MyEmployees AS e
					INNER JOIN DirectReports AS ON e.ManagerID = d.EmployeeID
				)
				-- Statement that executes the CTE
				SELECT  ManagerID, EmployeeID, Title, Level
				FROM    DirectReports";

			const string EXPECTED_SQL = @"
				WITH DirectReports (ManagerID, EmployeeID, Title, DeptID, Level)
				(   -- Anchor member definition
					SELECT  ManagerID, EmployeeID, Title, Deptid, 0 AS Level
					FROM    MyEmployees
					WHERE   ManagerID IS NULL
					
					UNION ALL
					
					-- Recursive member definition
					SELECT  e.ManagerID, e.EmployeeID, e.Title, e.Deptid, Level + 1
					FROM    MyEmployees AS e
					INNER JOIN DirectReports AS ON e.ManagerID = d.EmployeeID
				)
				-- Statement that executes the CTE
				SELECT  ManagerID, EmployeeID, Title, Level
				FROM    DirectReports ORDER BY CURRENT_TIMESTAMP OFFSET 0 ROWS FETCH FIRST 2 ROWS ONLY";

			var d = new MsSql2012Dialect();
			var limitSqlQuery = d.GetLimitString(new SqlString(SQL), null, new SqlString("2"));
			Assert.That(limitSqlQuery, Is.Not.Null);
			Assert.That(limitSqlQuery.ToString(), Is.EqualTo(EXPECTED_SQL));
		}

		[Test]
		public void DontReturnLimitStringForStoredProcedureCall()
		{
			VerifyLimitStringForStoredProcedureCalls("EXEC sp_stored_procedures");
			VerifyLimitStringForStoredProcedureCalls(@"
				DECLARE @id int
				SELECT  @id = id FROM persons WHERE name LIKE ?
				EXEC    get_person_summary @id");
			VerifyLimitStringForStoredProcedureCalls(@"
				DECLARE @id int
				SELECT DISTINCT TOP 1 @id = id FROM persons WHERE name LIKE ?
				EXEC    get_person_summary @id");
			VerifyLimitStringForStoredProcedureCalls(@"
				DECLARE @id int
				SELECT DISTINCT TOP (?) PERCENT WITH TIES @id = id FROM persons WHERE name LIKE ?
				EXEC    get_person_summary @id");
		}

		private static void VerifyLimitStringForStoredProcedureCalls(string sql)
		{
			var d = new MsSql2012Dialect();
			var limitSql = d.GetLimitString(new SqlString(sql), null, new SqlString("2"));
			Assert.That(limitSql, Is.Null, "Limit only: {0}", sql);

			limitSql = d.GetLimitString(new SqlString(sql), new SqlString("10"), null);
			Assert.That(limitSql, Is.Null, "Offset only: {0}", sql);

			limitSql = d.GetLimitString(new SqlString(sql), new SqlString("10"), new SqlString("2"));
			Assert.That(limitSql, Is.Null, "Limit and Offset: {0}", sql);
		}
	}
}