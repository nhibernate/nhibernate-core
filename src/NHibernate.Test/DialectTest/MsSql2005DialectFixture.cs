using System;
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

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 Contact1_19_0_, Rating2_19_0_ FROM (select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_, ROW_NUMBER() OVER(ORDER BY c.Rating DESC, c.Last_Name, c.First_Name) as __hibernate_sort_row from dbo.Contact c where COALESCE(c.Rating, 0) > 0) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT fish.id, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 id FROM (SELECT DISTINCT fish_.id, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish fish_) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 ixx9_ FROM (SELECT DISTINCT fish_.id as ixx9_, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish fish_) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), 5, 15);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 15 * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY name) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 5 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), 7, 28);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 28 id, name FROM (SELECT fish.id, fish.name, ROW_NUMBER() OVER(ORDER BY fish.name DESC) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 7 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str =
				d.GetLimitString(
					new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), 10, 20);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 20 * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY name DESC) as __hibernate_sort_row FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t) as query WHERE query.__hibernate_sort_row > 10 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 *, some_count FROM (SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish WHERE scales = ?) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 Type, Name FROM (SELECT f.Type, COUNT(DISTINCT f.Name) AS Name, ROW_NUMBER() OVER(ORDER BY COUNT(DISTINCT f.Name)) as __hibernate_sort_row FROM Fish f GROUP BY f.Type) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
				str.ToString());
		}

		[Test]
		[Description("should use only TOP clause if there is no offset")]
		public void OnlyOffsetLimit()
		{
			var d = new MsSql2005Dialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), 0, 10);
			System.Console.WriteLine(str);
			Assert.That(str.ToString(), Is.EqualTo("select distinct top 10 c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"));
		}

		[Test]
		public void NH1187()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();
			SqlString result =
				d.GetLimitString(new SqlString("select concat(a.Description,', ', a.Description) as desc from Animal a"), 1, 10);
			System.Console.WriteLine(result);
			Assert.AreEqual("SELECT TOP 10 desc FROM (select concat(a.Description,', ', a.Description) as desc, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row from Animal a) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row", result.ToString());

			// The test use the function "cast" because cast need the keyWork "as" too
			SqlString str =
				d.GetLimitString(new SqlString("SELECT fish.id, cast('astring, with,comma' as string) as bar FROM fish"), 1, 10);
			System.Console.WriteLine(str);
			Assert.AreEqual(
				"SELECT TOP 10 id, bar FROM (SELECT fish.id, cast('astring, with,comma' as string) as bar, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 1 ORDER BY query.__hibernate_sort_row",
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

			[Test]
			public void GetLimitStringWithSqlComments()
			{
				var d = new MsSql2005Dialect();
				Assert.Throws<NotSupportedException>(()=> d.GetLimitString(new SqlString(" /* criteria query */ SELECT p from lcdtm"), 0, 2));
			}
	}
}
