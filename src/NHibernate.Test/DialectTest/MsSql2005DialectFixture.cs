using System;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
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

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) Contact1_19_0_, Rating2_19_0_ FROM (select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_, ROW_NUMBER() OVER(ORDER BY c.Rating DESC, c.Last_Name, c.First_Name) as __hibernate_sort_row from dbo.Contact c where COALESCE(c.Rating, 0) > 0) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) id FROM (SELECT fish.id, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) id FROM (SELECT DISTINCT fish_.id, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish fish_) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) ixx9_ FROM (SELECT DISTINCT fish_.id as ixx9_, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish fish_) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY name) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) id, name FROM (SELECT fish.id, fish.name, ROW_NUMBER() OVER(ORDER BY fish.name DESC) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY name DESC) as __hibernate_sort_row FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) *, some_count FROM (SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish WHERE scales = ?) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) Type, Name FROM (SELECT f.Type, COUNT(DISTINCT f.Name) AS Name, ROW_NUMBER() OVER(ORDER BY COUNT(DISTINCT f.Name)) as __hibernate_sort_row FROM Fish f GROUP BY f.Type) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());
		}

		[Test]
		[Description("should use only TOP clause if there is no offset")]
		public void OnlyOffsetLimit()
		{
			var d = new MsSql2005Dialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), null, new SqlString("10"));
			Assert.That(str.ToString(), Is.EqualTo("select distinct TOP (10)  c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"));
		}

		[Test]
		public void NH1187()
		{
			MsSql2005Dialect d = new MsSql2005Dialect();
			SqlString result = d.GetLimitString(new SqlString("select concat(a.Description,', ', a.Description) as desc from Animal a"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual("SELECT TOP (222) desc FROM (select concat(a.Description,', ', a.Description) as desc, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row from Animal a) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row", result.ToString());

			// The test use the function "cast" because cast need the keyWork "as" too
			SqlString str = d.GetLimitString(new SqlString("SELECT fish.id, cast('astring, with,comma' as string) as bar FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT TOP (222) id, bar FROM (SELECT fish.id, cast('astring, with,comma' as string) as bar, ROW_NUMBER() OVER(ORDER BY CURRENT_TIMESTAMP) as __hibernate_sort_row FROM fish) as query WHERE query.__hibernate_sort_row > 111 ORDER BY query.__hibernate_sort_row",
				str.ToString());
		}

		[Test]
		public void NH2809()
		{
			var d = new MsSql2005Dialect();

			string t = d.GetTypeName(new BinarySqlType());
			Assert.That(t, Is.EqualTo("VARBINARY(MAX)"));

			t = d.GetTypeName(new BinarySqlType(), SqlClientDriver.MaxSizeForLengthLimitedBinary - 1, 0, 0);
			Assert.That(t, Is.EqualTo(String.Format("VARBINARY({0})", SqlClientDriver.MaxSizeForLengthLimitedBinary - 1)));

			t = d.GetTypeName(new BinarySqlType(), SqlClientDriver.MaxSizeForLengthLimitedBinary + 1, 0, 0);
			Assert.That(t, Is.EqualTo("VARBINARY(MAX)"));
		}

		[Test]
		public void QuotedAndParenthesisStringTokenizerTests_WithComma_InQuotes()
		{
			MsSql2005Dialect.QuotedAndParenthesisStringTokenizer tokenizier =
				new MsSql2005Dialect.QuotedAndParenthesisStringTokenizer(
					new SqlString("select concat(a.Description,', ', a.Description) from Animal a"));
			string[] expected = new string[]
				{
					"select",
					"concat(a.Description,', ', a.Description)",
					"from",
					"Animal",
					"a"
				};
			int current = 0;
			foreach (SqlString token in tokenizier)
			{
				Assert.AreEqual(expected[current], token.ToString());
				current += 1;
			}
			Assert.AreEqual(current, expected.Length);
		}

		[Test]
		public void QuotedAndParenthesisStringTokenizerTests_WithFunctionCallContainingComma()
		{
			MsSql2005Dialect.QuotedAndParenthesisStringTokenizer tokenizier =
				new MsSql2005Dialect.QuotedAndParenthesisStringTokenizer(
					new SqlString("SELECT fish.id, cast('astring, with,comma' as string) as bar, f FROM fish"));
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
			IList<SqlString> tokens = tokenizier.GetTokens();
			foreach (SqlString token in tokens)
			{
				Assert.AreEqual(expected[current], token.ToString());
				current += 1;
			}
			Assert.AreEqual(current, expected.Length);
		}

		[Test]
		public void QuotedStringTokenizerTests()
		{
			MsSql2005Dialect.QuotedAndParenthesisStringTokenizer tokenizier =
				new MsSql2005Dialect.QuotedAndParenthesisStringTokenizer(
					new SqlString("SELECT fish.\"id column\", fish.'fish name' as 'bar\\' column', f FROM fish"));
			string[] expected = new string[]
				{
					"SELECT",
					"fish.\"id column\"",
					",",
					"fish.'fish name'",
					"as",
					"'bar\\' column'",
					",",
					"f",
					"FROM",
					"fish"
				};
			int current = 0;
			IList<SqlString> tokens = tokenizier.GetTokens();
			foreach (SqlString token in tokens)
			{
				Assert.AreEqual(expected[current], token.ToString());
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
			Assert.Throws<NotSupportedException>(() => d.GetLimitString(new SqlString(" /* criteria query */ SELECT p from lcdtm"), null, new SqlString("2")));
		}
	}
}