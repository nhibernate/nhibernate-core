using System;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	/// <summary>
	/// Summary description for MsSqlDialectFixture.
	/// </summary>
	[TestFixture]
	public class MsSqlDialectFixture : DialectFixture
	{
		[SetUp]
		public override void SetUp()
		{
			// Generic Dialect inherits all of the Quoting functions from
			// Dialect (which is abstract)
			d = new MsSql2000Dialect();
			tableWithNothingToBeQuoted = new string[] {"plainname", "[plainname]"};
			tableAlreadyQuoted = new string[] {"[Quote[d[Na]]$`]", "[Quote[d[Na]]$`]", "Quote[d[Na]$`"};
			tableThatNeedsToBeQuoted = new string[] {"Quote[d[Na]$`", "[Quote[d[Na]]$`]", "Quote[d[Na]$`"};
		}

		[Test]
		public void GetLimitString()
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add("select id, col1, col2 from someTable");
			SqlString limitedSql = d.GetLimitString(builder.ToSqlString(), 0, 100);
			Assert.AreEqual("select top 100 id, col1, col2 from someTable", limitedSql.ToString(),
			                "Bad limit SQL");

			// test when building with SqlParts
			builder = new SqlStringBuilder();
			builder.Add("select").Add(" id, col1, col2 ").Add("from someTable");
			limitedSql = d.GetLimitString(builder.ToSqlString(), 0, 100);
			Assert.AreEqual("select top 100 id, col1, col2 from someTable", limitedSql.ToString(),
			                "Bad limit SQL");

			// now add a subselect to see if only one top gets added
			builder.Add(" where id in (select id from othertable)");
			limitedSql = d.GetLimitString(builder.ToSqlString(), 0, 100);
			Assert.AreEqual("select top 100 id, col1, col2 from someTable where id in (select id from othertable)",
			                limitedSql.ToString(),
			                "Bad limit SQL");

			// now the distinct case
			// first with simple string
			builder = new SqlStringBuilder();
			builder.Add("select distinct id, col1, col2 from someTable");
			limitedSql = d.GetLimitString(builder.ToSqlString(), 0, 100);
			Assert.AreEqual("select distinct top 100 id, col1, col2 from someTable", limitedSql.ToString(),
			                "Bad limit SQL");

			// now with parts
			builder = new SqlStringBuilder();
			builder.Add("select").Add(" distinct").Add(" id, col1, col2 from someTable");
			limitedSql = d.GetLimitString(builder.ToSqlString(), 0, 100);
			Assert.AreEqual("select distinct top 100 id, col1, col2 from someTable", limitedSql.ToString(),
			                "Bad limit SQL");
		}

		[Test]
		public void TimestampRounding()
		{
			DateTime input = new DateTime(2000, 1, 1, 10, 11, 12, 13);
			DateTime expected = new DateTime(2000, 1, 1, 10, 11, 12, 10);

			Assert.AreEqual(expected, TimestampType.Round(input, d.TimestampResolutionInTicks));
		}
	}
}