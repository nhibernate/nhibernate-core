using System;

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
		[Test]
		public void Append() 
		{
			SqlString sql = new SqlString( new string[] { "select", " from table" } );

			SqlString postAppendSql = sql.Append(" where A=B" );

			Assert.IsFalse( sql==postAppendSql, "should be a new object" );
			Assert.AreEqual( 3, postAppendSql.SqlParts.Length );

			sql = postAppendSql;

			postAppendSql = sql.Append( new SqlString(" and C=D") );

			Assert.AreEqual(4, postAppendSql.SqlParts.Length );

			Assert.AreEqual( "select from table where A=B and C=D", postAppendSql.ToString() );

		}

		
		[Test]
		public void CompactWithNoParams() 
		{
			SqlString sql = new SqlString( new string[] { "", "select", " from table" } );
			SqlString compacted = sql.Compact();

			Assert.AreEqual( 1, compacted.SqlParts.Length );
			Assert.AreEqual( "select from table", compacted.ToString() );
		}

		[Test]
		public void CompactWithParams() 
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			Parameter param = new Parameter();
			param.Name = "id";
			param.SqlType = new SqlTypes.Int32SqlType();

			builder.Add("select from table ");
			builder.Add("where ");
			builder.Add("id = ");
			builder.Add(param);
			builder.Add(" and ");
			builder.Add("'a'='a'");

			SqlString sql = builder.ToSqlString();
			sql = sql.Compact();

			Assert.AreEqual( 3, sql.SqlParts.Length );
			Assert.AreEqual( "select from table where id = :id and 'a'='a'", sql.ToString() );

		}

		[Test]
		public void CompactWithNoString() 
		{
			Parameter p1 = new Parameter();
			Parameter p2 = new Parameter();
			p1.Name = "p1";
			p2.Name = "p2";
			
			SqlString sql = new SqlString( new object[] {p1, p2} );
			SqlString compacted = sql.Compact();

			Assert.AreEqual( 2, compacted.SqlParts.Length );
			Assert.AreEqual( ":p1:p2", compacted.ToString() );

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
			Parameter p1 = new Parameter();
			p1.SqlType = new SqlTypes.Int32SqlType();
			p1.Name = "p1";
		
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", p1} );
			Assert.IsFalse( sql.ContainsUntypedParameter );
		}

		[Test]
		public void ContainsUntypedParameterWithUntypedParam() 
		{
			
			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", new Parameter()} );
			Assert.IsTrue( sql.ContainsUntypedParameter );
		}

		[Test]
		public void ContainsUntypedParameterWithMixedUntypedParam() 
		{
			Parameter p1 = new Parameter();
			p1.SqlType = new SqlTypes.Int32SqlType();
			p1.Name = "p1";

			SqlString sql = new SqlString( new object[] {"select", " from table where a = ", new Parameter(), " and b = " , p1} );
			Assert.IsTrue( sql.ContainsUntypedParameter );
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
			SqlString sql = new SqlString( new object[] { "", "select", " from table where id = ", new Parameter() } );
			Assert.IsFalse( sql.EndsWith("'") );
			Assert.IsFalse( sql.EndsWith("") );
		}

		[Test]
		public void ParameterIndexesNoParams() 
		{
			SqlString sql = new SqlString( new object[] {"select ", "from table ", "where 'a'='a'" } );
			
			Assert.AreEqual( 0, sql.ParameterIndexes.Length );
		}

		[Test]
		public void ParameterIndexOneParam() 
		{
			SqlString sql = new SqlString( new object[] {"select ", "from table ", "where a = ", new Parameter() } );
			
			Assert.AreEqual( 1, sql.ParameterIndexes.Length );
			Assert.AreEqual( 3, sql.ParameterIndexes[0] );
		}


		[Test]
		public void ParameterIndexManyParam() 
		{
			SqlString sql = new SqlString( new object[] {"select ", "from table ", "where a = ", new Parameter(), " and c = ", new Parameter() } );
			
			Assert.AreEqual( 2, sql.ParameterIndexes.Length );
			Assert.AreEqual( 3, sql.ParameterIndexes[0] );
			Assert.AreEqual( 5, sql.ParameterIndexes[1] );
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
			Parameter p = new Parameter();
			p.Name = "p1";

			builder.Add(" select from table");
			builder.Add(" where p = ");
			builder.Add(p);

			SqlString sql = builder.ToSqlString();

			sql = sql.Substring(1);

			Assert.AreEqual( "select from table where p = :p1", sql.ToString() );


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
			Parameter p1 = new Parameter();
			p1.Name = "p1";

			SqlString sql = new SqlString( new object[] {p1, "   extra space   "} );
			sql = sql.Trim();

			Assert.AreEqual( ":p1   extra space", sql.ToString() );
		}

		[Test]
		public void TrimBeginStringEndParam() 
		{
			Parameter p1 = new Parameter();
			p1.Name = "p1";

			SqlString sql = new SqlString( new object[] { "   extra space   ", p1 } );
			sql = sql.Trim();

			Assert.AreEqual( "extra space   :p1", sql.ToString() );
		}

		[Test]
		public void TrimAllParam() 
		{
			Parameter p1 = new Parameter();
			p1.Name = "p1";
			Parameter p2 = new Parameter();
			p2.Name = "p2";
			
			SqlString sql = new SqlString( new object[] { p1, p2 } );
			sql = sql.Trim();

			Assert.AreEqual( ":p1:p2", sql.ToString() );
		}


	}
}
