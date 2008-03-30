using NHibernate.Dialect;
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

			SqlString str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.id, query.__hibernate_sort_expr_1__ FROM (SELECT fish.id, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.id, query.__hibernate_sort_expr_1__ FROM (SELECT DISTINCT fish_.id, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish fish_) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 ixx9_ FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.ixx9_, query.__hibernate_sort_expr_1__ FROM (SELECT DISTINCT fish_.id as ixx9_, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish fish_) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), 5, 15);
			Assert.AreEqual(
				"SELECT TOP 15 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.*, query.__hibernate_sort_expr_1__ FROM (SELECT *, name as __hibernate_sort_expr_1__ FROM fish) query ) page WHERE page.row > 5 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), 7, 28);
			Assert.AreEqual(
				"SELECT TOP 28 id, name FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__ DESC) as row, query.id, query.name, query.__hibernate_sort_expr_1__ FROM (SELECT fish.id, fish.name, name as __hibernate_sort_expr_1__ FROM fish) query ) page WHERE page.row > 7 ORDER BY __hibernate_sort_expr_1__ DESC",
				str.ToString());

			str =
				d.GetLimitString(
					new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), 10, 20);
			Assert.AreEqual(
				"SELECT TOP 20 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__ DESC) as row, query.*, query.__hibernate_sort_expr_1__ FROM (SELECT *, name as __hibernate_sort_expr_1__ FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t) query ) page WHERE page.row > 10 ORDER BY __hibernate_sort_expr_1__ DESC",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 *, some_count FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.*, query.some_count, query.__hibernate_sort_expr_1__ FROM (SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 * FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.*, query.__hibernate_sort_expr_1__ FROM (SELECT *, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish WHERE scales = ?) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 Type, Name FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.Type, query.Name, query.__hibernate_sort_expr_1__ FROM (SELECT f.Type, COUNT(DISTINCT f.Name) AS Name, COUNT(DISTINCT f.Name) as __hibernate_sort_expr_1__ FROM Fish f GROUP BY f.Type) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
				str.ToString());
		}

		[Test]
		public void NH1187()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();
			SqlString result =
				d.GetLimitString(new SqlString("select concat(a.Description,', ', a.Description) as desc from Animal a"), 0, 10);
			Assert.AreEqual("SELECT TOP 10 desc FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.desc, query.__hibernate_sort_expr_1__ FROM (select concat(a.Description,', ', a.Description) as desc, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ from Animal a) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__", result.ToString());

			// The test use the function "cast" because cast need the keyWork "as" too
			SqlString str =
				d.GetLimitString(new SqlString("SELECT fish.id, cast('astring, with,comma' as string) as bar FROM fish"), 0, 10);
			Assert.AreEqual(
				"SELECT TOP 10 id, bar FROM (SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__) as row, query.id, query.bar, query.__hibernate_sort_expr_1__ FROM (SELECT fish.id, cast('astring, with,comma' as string) as bar, CURRENT_TIMESTAMP as __hibernate_sort_expr_1__ FROM fish) query ) page WHERE page.row > 0 ORDER BY __hibernate_sort_expr_1__",
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
	}
}
