using System;

using NHibernate.SqlCommand;

using NUnit.Framework;
using NHibernate.SqlTypes;

namespace NHibernate.Test.SqlCommandTest
{
	/// <summary>
	/// Summary description for SqlStringFixture.
	/// </summary>
	[TestFixture]
	public class SqlStringFixture
	{
		[Test]
		public void Append() 
		{
			SqlString sql = new SqlString( new string[] { "select", " from table" } );

			SqlString postAppendSql = sql.Append(" where A=B" );

			Assert.IsFalse( sql==postAppendSql, "should be a new object" );
			Assert.AreEqual( 3, postAppendSql.SqlParts.Count );

			sql = postAppendSql;

			postAppendSql = sql.Append( new SqlString(" and C=D") );

			Assert.AreEqual(4, postAppendSql.SqlParts.Count );

			Assert.AreEqual( "select from table where A=B and C=D", postAppendSql.ToString() );

		}

		
		[Test]
		public void CompactWithNoParams() 
		{
			SqlString sql = new SqlString( new string[] { "", "select", " from table" } );
			SqlString compacted = sql.Compact();

			Assert.AreEqual( 1, compacted.SqlParts.Count );
			Assert.AreEqual( "select from table", compacted.ToString() );
		}

		[Test]
		public void CompactWithParams() 
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			Parameter param = new Parameter( SqlTypeFactory.Int32 );

			builder.Add("select from table ");
			builder.Add("where ");
			builder.Add("id = ");
			builder.Add(param);
			builder.Add(" and ");
			builder.Add("'a'='a'");

			SqlString sql = builder.ToSqlString();
			sql = sql.Compact();

			Assert.AreEqual( 3, sql.SqlParts.Count );
			Assert.AreEqual( "select from table where id = ? and 'a'='a'", sql.ToString() );

		}

		[Test]
		public void CompactWithNoString() 
		{
			Parameter p1 = Parameter.Placeholder;
			Parameter p2 = Parameter.Placeholder;
			
			SqlString sql = new SqlString( new object[] {p1, p2} );
			SqlString compacted = sql.Compact();

			Assert.AreEqual( 2, compacted.SqlParts.Count );
			Assert.AreEqual( "??", compacted.ToString() );

		}

		[Test]
		public void ContainsUntypedParameterWithoutParam() 
		{
			SqlString sql = new SqlString( new string[] {"select", " from table"} );
			Assert.IsFalse( sql.ContainsUntypedParameter );
		}

		[Test]
		public void ContainsUntypedParameterWithParam() 
		{
			Parameter p1 = new Parameter( SqlTypeFactory.Int32 );
			
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", p1} );
			Assert.IsFalse( sql.ContainsUntypedParameter );
		}

		[Test]
		public void ContainsUntypedParameterWithUntypedParam() 
		{
			
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", Parameter.Placeholder} );
			Assert.IsTrue( sql.ContainsUntypedParameter );
		}

		[Test]
		public void ContainsUntypedParameterWithMixedUntypedParam() 
		{
			Parameter p1 = new Parameter( SqlTypeFactory.Int32 );
			
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", Parameter.Placeholder, " and b = " , p1} );
			Assert.IsTrue( sql.ContainsUntypedParameter );
		}

		[Test]
		public void Count() 
		{
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", Parameter.Placeholder, " and b = " , Parameter.Placeholder } );
			Assert.AreEqual( 5, sql.Count, "Count with no nesting failed." );

			sql = sql.Append( new SqlString( new object[] {" more parts ", " another part "} ) );
			Assert.AreEqual( 7, sql.Count, "Added a SqlString to a SqlString" );

			SqlString nestedSql = new SqlString( new object[] { "nested 1", "nested 2" } );

			sql = sql.Append( new SqlString( new object[] { nestedSql, " not nested 1", " not nested 2" } ) );

			Assert.AreEqual( 11, sql.Count, "Added 2 more levels of nesting" );
		}

		[Test]
		public void EndsWith() 
		{
			SqlString sql = new SqlString( new string[] {"select", " from table" } );
			Assert.IsTrue( sql.EndsWith("ble") );
			Assert.IsFalse( sql.EndsWith("'") );
		}

		[Test]
		public void EndsWithEmptyString() 
		{
			SqlString sql = new SqlString( new string[] { "", "select", " from table", "" } );
			Assert.IsTrue( sql.EndsWith("ble") );
			Assert.IsFalse( sql.EndsWith("'") );
		}

		[Test]
		public void EndsWithParameter() 
		{
			SqlString sql = new SqlString( new object[] { "", "select", " from table where id = ", Parameter.Placeholder } );
			Assert.IsFalse( sql.EndsWith("'") );
			Assert.IsFalse( sql.EndsWith("") );
		}

		[Test]
		public void IsEmptyWithEmptyStrings()
		{
			SqlString sql = new SqlString( new object[] { "", "" } );
			Assert.IsTrue( sql.IsEmpty, "only empty string put in.");
		}

		[Test]
		public void IsEmptyWithString()
		{
			SqlString sql = new SqlString( "not empty" );
			Assert.IsFalse( sql.IsEmpty, "valid string in there" );
		}

		[Test]
		public void IsEmptyWithParam()
		{
			SqlString sql = new SqlString( new object[] { "", Parameter.Placeholder } );
			Assert.IsFalse( sql.IsEmpty, "had a parameter - should not be empty" );
		}

		[Test]
		public void Replace() 
		{
			SqlString sql = new SqlString( new object[] {"select ", "from table ", "where a = ", Parameter.Placeholder, " and c = ", Parameter.Placeholder } );
			SqlString replacedSql = sql.Replace( "table", "replacedTable" );
			Assert.AreEqual( sql.ToString().Replace("table", "replacedTable"), replacedSql.ToString());

			replacedSql = sql.Replace( "not found", "not in here" );
			Assert.AreEqual( sql.ToString().Replace( "not found", "not in here" ), replacedSql.ToString(), "replace no found string" );

			replacedSql = sql.Replace( "le", "LE" );
			Assert.AreEqual( sql.ToString().Replace( "le", "LE" ), replacedSql.ToString(), "multi-match replace" );
			Assert.AreEqual( 2, replacedSql.GetParameterTypes().Length, "multi-match replace - parameters" );
		}
		
		[Test]
		public void StartsWith() 
		{
			SqlString sql = new SqlString( new string[] {"select", " from table" } );
			Assert.IsTrue( sql.StartsWith("s") );
			Assert.IsFalse( sql.StartsWith(",") );
		}

		[Test]
		public void StartsWithEmptyString() 
		{
			SqlString sql = new SqlString( new string[] { "", "select", " from table" } );
			Assert.IsTrue( sql.StartsWith("s") );
			Assert.IsFalse( sql.StartsWith(",") );
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

			Assert.AreEqual( "select from table where p = ?", sql.ToString() );


		}

		[Test]
		public void TrimAllString() 
		{
			SqlString sql = new SqlString( new string[] {"   extra space", " in the middle", " at the end     " } );
			sql = sql.Trim();

			Assert.AreEqual( "extra space in the middle at the end", sql.ToString() );
		}

		[Test]
		public void TrimBeginParamEndString() 
		{
			Parameter p1 = Parameter.Placeholder;
			
			SqlString sql = new SqlString( new object[] {p1, "   extra space   "} );
			sql = sql.Trim();

			Assert.AreEqual( "?   extra space", sql.ToString() );
		}

		[Test]
		public void TrimBeginStringEndParam() 
		{
			Parameter p1 = Parameter.Placeholder;
			
			SqlString sql = new SqlString( new object[] { "   extra space   ", p1 } );
			sql = sql.Trim();

			Assert.AreEqual( "extra space   ?", sql.ToString() );
		}

		[Test]
		public void TrimAllParam() 
		{
			Parameter p1 = Parameter.Placeholder;
			Parameter p2 = Parameter.Placeholder;
			
			SqlString sql = new SqlString( new object[] { p1, p2 } );
			sql = sql.Trim();

			Assert.AreEqual( "??", sql.ToString() );
		}


	}
}
