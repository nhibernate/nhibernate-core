using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	using System.Collections.Generic;

	[TestFixture]
	public class MsSql2005DialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 Contact1_19_0_, Rating2_19_0_ FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__ DESC, __hibernate_sort_expr_1__, __hibernate_sort_expr_2__) as row, query.Contact1_19_0_, query.Rating2_19_0_, query.__hibernate_sort_expr_0__, query.__hibernate_sort_expr_1__, query.__hibernate_sort_expr_2__ FROM (select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_, c.Rating as __hibernate_sort_expr_0__, c.Last_Name as __hibernate_sort_expr_1__, c.First_Name as __hibernate_sort_expr_2__ from dbo.Contact c where COALESCE(c.Rating, 0) > 0) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__ DESC, __hibernate_sort_expr_1__, __hibernate_sort_expr_2__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.id, query.__hibernate_sort_expr_0__ FROM (SELECT fish.id, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.id, query.__hibernate_sort_expr_0__ FROM (SELECT DISTINCT fish_.id, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish fish_) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 ixx9_ FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.ixx9_, query.__hibernate_sort_expr_0__ FROM (SELECT DISTINCT fish_.id as ixx9_, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish fish_) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), 5, 15);
			Assert.AreEqual(
				"SELECT TOP 15 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.*, query.__hibernate_sort_expr_0__ FROM (SELECT *, name as __hibernate_sort_expr_0__ FROM fish) query ) page WHERE page.row > 5 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), 7, 28);
			Assert.AreEqual(
				"SELECT TOP 28 id, name FROM (SELECT ROW_NUMBER() OVER(ORDER BY name DESC) as row, query.id, query.name FROM (SELECT fish.id, fish.name FROM fish) query ) page WHERE page.row > 7 ORDER BY name DESC",
				str.ToString());

			str =
				d.GetLimitString(
					new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), 10, 20);
			Assert.AreEqual(
				"SELECT TOP 20 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__ DESC) as row, query.*, query.__hibernate_sort_expr_0__ FROM (SELECT *, name as __hibernate_sort_expr_0__ FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t) query ) page WHERE page.row > 10 ORDER BY __hibernate_sort_expr_0__ DESC",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 *, some_count FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.*, query.some_count, query.__hibernate_sort_expr_0__ FROM (SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.*, query.__hibernate_sort_expr_0__ FROM (SELECT *, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish WHERE scales = ?) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 Type, Name FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.Type, query.Name, query.__hibernate_sort_expr_0__ FROM (SELECT f.Type, COUNT(DISTINCT f.Name) AS Name, COUNT(DISTINCT f.Name) as __hibernate_sort_expr_0__ FROM Fish f GROUP BY f.Type) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());
		}

		[Test]
		public void NH1187()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();
			SqlString result =
				d.GetLimitString(new SqlString("select concat(a.Description,', ', a.Description) as desc from Animal a"), 0, 10);
			Assert.AreEqual("SELECT TOP 10 desc FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.desc, query.__hibernate_sort_expr_0__ FROM (select concat(a.Description,', ', a.Description) as desc, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ from Animal a) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__", result.ToString());

			// The test use the function "cast" because cast need the keyWork "as" too
			SqlString str =
				d.GetLimitString(new SqlString("SELECT fish.id, cast('astring, with,comma' as string) as bar FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id, bar FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_0__) as row, query.id, query.bar, query.__hibernate_sort_expr_0__ FROM (SELECT fish.id, cast('astring, with,comma' as string) as bar, CURRENT_TIMESTAMP as __hibernate_sort_expr_0__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_0__",
				str.ToString());
	}
		[Test]
		public void QuotedAndParenthesisStringTokenizerTests_WithComma_InQuotes()
		{
			MsSql2005Dialect.QuotedAndParenthesisStringTokenizer tokenizier =
				new MsSql2005Dialect.QuotedAndParenthesisStringTokenizer(
					"select concat(a.Description,', ', a.Description) from Animal a");
			string[] expected = new string[]
				{
					"select",
					"concat(a.Description,', ', a.Description)",
					"from",
					"Animal",
					"a"
				};
			int current = 0;
			foreach (string token in tokenizier)
			{
				Assert.AreEqual(expected[current], token);
				current += 1;
			}
			Assert.AreEqual(current, expected.Length);
		}

		[Test]
		public void QuotedAndParenthesisStringTokenizerTests_WithFunctionCallContainingComma()
		{
			MsSql2005Dialect.QuotedAndParenthesisStringTokenizer tokenizier =
				new MsSql2005Dialect.QuotedAndParenthesisStringTokenizer(
					"SELECT fish.id, cast('astring, with,comma' as string) as bar, f FROM fish");
			string[] expected = new string[]
				{
					"SELECT",
					"fish.id",
					",",
					"cast('astring, with,comma' as string)",
					"as",
					"bar",
					",",
					"f",
					"FROM",
					"fish"
				};
			int current = 0;
			IList<string> tokens = tokenizier.GetTokens();
			foreach (string token in tokens)
			{
				Assert.AreEqual(expected[current], token);
				current += 1;
			}
			Assert.AreEqual(current, expected.Length);
		}

	    [Test]
	    public void GetIfExistsDropConstraintTest_without_schema()
	    {
	        MsSql2005Dialect dialect = new MsSql2005Dialect();
            Table foo = new Table("Foo");
	        string expected = "if exists (select 1 from sys.objects" +
                              " where object_id = OBJECT_ID(N'[Bar]')" +
                              " AND parent_object_id = OBJECT_ID('Foo'))";
	        string ifExistsDropConstraint = dialect.GetIfExistsDropConstraint(foo, "Bar");
            System.Console.WriteLine(ifExistsDropConstraint);
            Assert.AreEqual(expected, ifExistsDropConstraint);
	    }

	    [Test]
	    public void GetIfExistsDropConstraintTest_For_Schema_other_than_dbo()
	    {
	        MsSql2005Dialect dialect = new MsSql2005Dialect();
            Table foo = new Table("Foo");
	        foo.Schema = "Other";
	        string expected = "if exists (select 1 from sys.objects" +
                              " where object_id = OBJECT_ID(N'Other.[Bar]')" +
                              " AND parent_object_id = OBJECT_ID('Other.Foo'))";
	        string ifExistsDropConstraint = dialect.GetIfExistsDropConstraint(foo, "Bar");
            System.Console.WriteLine(ifExistsDropConstraint);
            Assert.AreEqual(expected, ifExistsDropConstraint);
	    }
	}
}
