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
	}
}
