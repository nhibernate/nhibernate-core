using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Summary description for SqlStringFixture.
	/// </summary>
	[TestFixture]
	public class SqlStringFixture
	{

		//[Test]
		//public void StringPerf()
		//{
		//
		//    // Just a quick piece of code to measure performance of some SqlString operations. Commented out,
		//    // since we don't want this to run on every build.
		//
		//    var str = new SqlString(new object[] { "  select x from xs where ", Parameter.Placeholder, " and ", Parameter.Placeholder, " and ", Parameter.Placeholder, " and ", Parameter.Placeholder, " and ", Parameter.Placeholder, " and ", Parameter.Placeholder, "   " });

		//    double[] allSub = new double[5];
		//    for (int a = 0; a < 5; ++a)
		//    {
		//        Stopwatch sw = Stopwatch.StartNew();
		//        for (int i = 0; i < 10000; ++i)
		//            str.Substring(6, 20);
		//        sw.Stop();
		//        Console.WriteLine(" Substring 10000 iterations (ms): " + sw.Elapsed.TotalMilliseconds);
		//        allSub[a] = sw.Elapsed.TotalMilliseconds;
		//    }
		//    Console.WriteLine("Substring average per 10000 iters (ms): " + allSub.Average());


		//    double[] allTrim = new double[5];
		//    for (int a = 0; a < 5; ++a)
		//    {
		//        Stopwatch sw = Stopwatch.StartNew();
		//        for (int i = 0; i < 10000; ++i)
		//            str.Trim();
		//        sw.Stop();
		//        Console.WriteLine(" Trim 10000 iterations (ms): " + sw.Elapsed.TotalMilliseconds);
		//        allTrim[a] = sw.Elapsed.TotalMilliseconds;
		//    }

		//    Console.WriteLine("Trim average per 10000 iters (ms): " + allTrim.Average());
		//}


		[Test]
		public void Append()
		{
			SqlString sql = new SqlString(new string[] { "select", " from table" });

			SqlString postAppendSql = sql.Append(" where A=B");

			Assert.IsFalse(sql == postAppendSql, "should be a new object");
			Assert.AreEqual(1, postAppendSql.Count);

			sql = postAppendSql;

			postAppendSql = sql.Append(new SqlString(" and C=D"));

			Assert.AreEqual(1, postAppendSql.Count);

			Assert.AreEqual("select from table where A=B and C=D", postAppendSql.ToString());
		}

		[Test]
		public void Count()
		{
			SqlString sql =
				new SqlString(
					new object[] { "select", " from table where a = ", Parameter.Placeholder, " and b = ", Parameter.Placeholder });
			Assert.AreEqual(4, sql.Count, "Count with no nesting failed.");

			sql = sql.Append(new SqlString(new object[] { " more parts ", " another part " }));
			Assert.AreEqual(5, sql.Count, "Added a SqlString to a SqlString");
		}

		[Test]
		public void Split()
		{
			SqlString sql = new SqlString(new string[] { "select", " alfa, beta, gamma", " from table" });
			var parts1 = sql.Split(",").Select(s => s.ToString()).ToArray();
			var expectedParts1 = new[] { "select alfa", " beta", " gamma from table" };
			Assert.That(parts1, Is.EqualTo(expectedParts1));

			SqlString sql2 = sql.Substring(6);
			var parts2 = sql2.Split(",").Select(s => s.ToString()).ToArray();
			var expectedParts2 = new[] { " alfa", " beta", " gamma from table" };
			Assert.That(parts2, Is.EqualTo(expectedParts2));
		}

		[Test]
		public void EndsWith()
		{
			SqlString sql = new SqlString(new string[] { "select", " from table" });
			Assert.IsTrue(sql.EndsWith("ble"));
			Assert.IsFalse(sql.EndsWith("'"));
		}

		[Test]
		public void EndsWithEmptyString()
		{
			SqlString sql = new SqlString(new string[] { "", "select", " from table", "" });
			Assert.IsTrue(sql.EndsWith("ble"));
			Assert.IsFalse(sql.EndsWith("'"));
		}

		[Test]
		public void EndsWithParameter()
		{
			SqlString sql = new SqlString(new object[] { "", "select", " from table where id = ", Parameter.Placeholder });
			Assert.IsFalse(sql.EndsWith("'"));
			Assert.IsFalse(sql.EndsWith(""));
		}

		[Test]
		public void Replace()
		{
			SqlString sql =
				new SqlString(
					new object[] { "select ", "from table ", "where a = ", Parameter.Placeholder, " and c = ", Parameter.Placeholder });
			SqlString replacedSql = sql.Replace("table", "replacedTable");
			Assert.AreEqual(sql.ToString().Replace("table", "replacedTable"), replacedSql.ToString());

			replacedSql = sql.Replace("not found", "not in here");
			Assert.AreEqual(sql.ToString().Replace("not found", "not in here"), replacedSql.ToString(), "replace no found string");

			replacedSql = sql.Replace("le", "LE");
			Assert.AreEqual(sql.ToString().Replace("le", "LE"), replacedSql.ToString(), "multi-match replace");
		}

		[Test]
		public void StartsWith()
		{
			SqlString sql = new SqlString(new string[] { "select", " from table" });
			Assert.IsTrue(sql.StartsWithCaseInsensitive("s"));
			Assert.IsFalse(sql.StartsWithCaseInsensitive(","));
		}

		[Test]
		public void StartsWithWhenContainsParameters()
		{
			SqlString sql = new SqlString(" and ", "(", new SqlString("blah = ", Parameter.Placeholder), ")");
			Assert.IsTrue(sql.StartsWithCaseInsensitive(" and"));
			Assert.IsFalse(sql.StartsWithCaseInsensitive("blah"));
		}

		[Test]
		public void StartsWithEmptyString()
		{
			SqlString sql = new SqlString(new string[] { "", "select", " from table" });
			Assert.IsTrue(sql.StartsWithCaseInsensitive("s"));
			Assert.IsFalse(sql.StartsWithCaseInsensitive(","));
		}

		[Test]
		public void Substring()
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			Parameter p = Parameter.Placeholder;

			builder.Add(" select from table");
			builder.Add(" where p = ");
			builder.Add(p);

			SqlString sql = builder.ToSqlString();

			sql = sql.Substring(1);

			Assert.AreEqual("select from table where p = ?", sql.ToString());
		}

		[Test]
		public void SubstringComplex()
		{
			SqlString str =
				new SqlString(new object[] { "select ", Parameter.Placeholder, " from table where x = ", Parameter.Placeholder });

			SqlString substr7 = str.Substring(7);
			Assert.AreEqual(
				new SqlString(new object[] { Parameter.Placeholder, " from table where x = ", Parameter.Placeholder }), substr7);

			SqlString substr10 = str.Substring(10);
			Assert.AreEqual(new SqlString(new object[] { "rom table where x = ", Parameter.Placeholder }), substr10);

			Assert.AreEqual(SqlString.Empty, str.Substring(200));
		}

		[Test]
		public void Substring2Complex()
		{
			SqlString str =
				new SqlString(new object[] { "select ", Parameter.Placeholder, " from table where x = ", Parameter.Placeholder });

			SqlString substr7_10 = str.Substring(7, 10);
			Assert.AreEqual(new SqlString(new object[] { Parameter.Placeholder, " from tab" }), substr7_10);

			SqlString substr10_200 = str.Substring(10, 200);
			Assert.AreEqual(new SqlString(new object[] { "rom table where x = ", Parameter.Placeholder }), substr10_200);

			SqlString substr200_10 = str.Substring(200, 10);
			Assert.AreEqual(SqlString.Empty, substr200_10);
		}

		[Test]
		public void IndexOf()
		{
			SqlString str =
				new SqlString(new object[] { "select ", Parameter.Placeholder, " from table where x = ", Parameter.Placeholder });

			Assert.AreEqual(0, str.IndexOfCaseInsensitive("select"));
			Assert.AreEqual(1, str.IndexOfCaseInsensitive("el"));

			Assert.AreEqual(7 + 1 + 6, str.IndexOfCaseInsensitive("table"));
		}

		[Test]
		public void IndexOfNonCompacted()
		{
			SqlString str = new SqlString(new object[] { "select ", " from" });
			Assert.AreEqual(6, str.IndexOfCaseInsensitive("  "));
		}

		[Test]
		public void TrimAllString()
		{
			SqlString sql = new SqlString(new string[] { "   extra space", " in the middle", " at the end     " });
			sql = sql.Trim();

			Assert.AreEqual("extra space in the middle at the end", sql.ToString());
		}

		[Test]
		public void TrimBeginParamEndString()
		{
			Parameter p1 = Parameter.Placeholder;

			SqlString sql = new SqlString(new object[] { p1, "   extra space   " });
			sql = sql.Trim();

			Assert.AreEqual("?   extra space", sql.ToString());
		}

		[Test]
		public void TrimBeginStringEndParam()
		{
			Parameter p1 = Parameter.Placeholder;

			SqlString sql = new SqlString(new object[] { "   extra space   ", p1 });
			sql = sql.Trim();

			Assert.AreEqual("extra space   ?", sql.ToString());
		}

		[Test]
		public void TrimAllParam()
		{
			Parameter p1 = Parameter.Placeholder;
			Parameter p2 = Parameter.Placeholder;

			SqlString sql = new SqlString(new object[] { p1, p2 });
			sql = sql.Trim();

			Assert.AreEqual("??", sql.ToString());
		}

		[Test]
		public void SubstringStartingWithLast()
		{
			SqlString sql = new SqlString(new object[] { "select x from y where z = ", Parameter.Placeholder, " order by t" });

			Assert.AreEqual("order by t", sql.SubstringStartingWithLast("order by").ToString());
		}

		[Test]
		public void NoSubstringStartingWithLast()
		{
			SqlString sql = new SqlString(new object[] { "select x from y where z = ", Parameter.Placeholder, " order by t" });

			Assert.AreEqual("", sql.SubstringStartingWithLast("zzz").ToString());
		}

		[Test]
		public void SubstringStartingWithLastAndParameters()
		{
			SqlString sql =
				new SqlString(
					new object[] { "select x from y where z = ", Parameter.Placeholder, " order by ", Parameter.Placeholder });

			Assert.AreEqual(new SqlString(new object[] { "order by ", Parameter.Placeholder }),
							sql.SubstringStartingWithLast("order by"));
		}

		[Test]
		public void SubstringStartingWithLastMultiplePossibilities()
		{
			SqlString sql = new SqlString(new string[] { " order by x", " order by z" });

			Assert.AreEqual("order by z", sql.SubstringStartingWithLast("order by").ToString());
		}

		[Test]
		public void Insert()
		{
			SqlString sql = new SqlString(new object[] { "begin ", Parameter.Placeholder, " end" });

			Assert.AreEqual("beginning ? end", sql.Insert(5, "ning").ToString());
			Assert.AreEqual("begin middle? end", sql.Insert(6, "middle").ToString());
			Assert.AreEqual("begin ?middle end", sql.Insert(7, "middle").ToString());
			Assert.AreEqual("beg|in ? end", sql.Insert(3, "|").ToString());
			Assert.AreEqual("begin ? ending", sql.Insert(11, "ing").ToString());
			Assert.AreEqual("begin ? enXd", sql.Insert(10, "X").ToString());
		}

		[Test]
		public void Parse()
		{
			SqlString sql =
				SqlString.Parse("select col from table where col = ? or col1 = 'a?b' and col2 = ? and col3 = 'x' and col4 = ?");
			SqlString expectedSql = new SqlString(
				new object[]
					{
						"select col from table where col = ",
						Parameter.Placeholder,
						" or col1 = 'a?b' and col2 = ",
						Parameter.Placeholder,
						" and col3 = 'x' and col4 = ",
						Parameter.Placeholder
					}
				);

			Assert.AreEqual(expectedSql, sql);

			Assert.AreEqual(new SqlString("simple"), SqlString.Parse("simple"));

			Assert.AreEqual(SqlString.Empty, SqlString.Parse(""));
		}


		[Test]
		public void GetSubselectStringSimple()
		{
			SqlString sql = SqlString.Parse("select col from table where col = test order by col");
			Assert.AreEqual(" from table where col = test ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringParameterInOrderBy()
		{
			SqlString sql = SqlString.Parse("select col from table where col = test order by ? asc");
			Assert.AreEqual(" from table where col = test ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringSimpleEndsWithParameter()
		{
			SqlString sql = SqlString.Parse("select col from table where col = ? order by col");
			Assert.AreEqual(" from table where col = ? ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringSimpleParameterInMiddle()
		{
			SqlString sql = SqlString.Parse("select col from table where col = ? and foo = bar order by col");
			Assert.AreEqual(" from table where col = ? and foo = bar ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithFormulaProperty()
		{
			SqlString sql =
				SqlString.Parse("select (select foo from bar where foo=col order by foo) from table where col = ? order by col");
			Assert.AreEqual(" from table where col = ? ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithSubselectInWhere()
		{
			SqlString sql =
				SqlString.Parse(
					"select (select foo from bar where foo=col order by foo) from table where col = (select yadda from blah where yadda=x order by yadda) order by col");
			Assert.AreEqual(" from table where col = (select yadda from blah where yadda=x order by yadda) ",
							sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithFormulaPropertyWithSubselect()
		{
			SqlString sql =
				SqlString.Parse(
					"select (select (select blah from yadda where blah=foo order by blah) from bar where foo=col order by foo) from table where col = ? order by col");
			Assert.AreEqual(" from table where col = ? ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithParenthesisOnlyInWhere()
		{
			SqlString sql = SqlString.Parse("select col from table where (col = test) order by col");
			Assert.AreEqual(" from table where (col = test) ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithTwoFormulas()
		{
			SqlString sql =
				SqlString.Parse(
					"select (select foo from bar where foo=col order by foo), (select foo from bar where foo=col order by foo) from table where col = ? order by col");
			Assert.AreEqual(" from table where col = ? ", sql.GetSubselectString().ToString());
		}

		[Test]
		public void GetSubselectStringWithOrderByInSubselect()
		{
			SqlString sql = SqlString.Parse("select col from table where (col = test) and id in (select id from foo order by bar)");
			Assert.AreEqual(" from table where (col = test) and id in (select id from foo order by bar)", sql.GetSubselectString().ToString());
		}

		[Test]
		public void ParameterPropertyShouldReturnNewInstances()
		{
			SqlString parameterString1 = new SqlString(Parameter.Placeholder);
			Parameter[] parameters1 = parameterString1.OfType<Parameter>().ToArray();

			SqlString parameterString2 = new SqlString(Parameter.Placeholder);
			Parameter[] parameters2 = parameterString2.OfType<Parameter>().ToArray();

			Assert.AreEqual(parameterString1, parameterString2);
			Assert.AreNotSame(parameterString1, parameterString2);

			parameters1[0].ParameterPosition = 231;
			Assert.IsNull(parameters2[0].ParameterPosition);

			// more simple version of the test
			Assert.That(Parameter.Placeholder, Is.Not.SameAs(Parameter.Placeholder));
		}


		[Test]
		public void HashcodeEqualForEqualStringsWithDifferentHistory()
		{
			// Verify that sql strings that are generated in different ways, but _now_ have
			// equal content, also have equal hashcodes.

			SqlString sql = new SqlString(new string[] { "select", " from table" });
			sql = sql.Substring(6);

			SqlString sql2 = new SqlString(new string[] { " from table" });

			Assert.That(sql, Is.EqualTo(sql2));
			Assert.That(sql.GetHashCode(), Is.EqualTo(sql2.GetHashCode()));
		}
	}
}
