using System;

using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	[TestFixture]
	public class BetweenExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void BetweenSqlStringTest()
		{
			ISession session = factory.OpenSession();

			CreateObjects( typeof( Simple ), session );
			ICriterion betweenExpression = Expression.Expression.Between( "Count", 5, 10 );
			SqlString sqlString = betweenExpression.ToSqlString( criteria, criteriaQuery );

			string expectedSql = "sql_alias.count_ between :sql_alias.count__lo and :sql_alias.count__hi";
			Parameter[ ] expectedParams = new Parameter[2];

			Parameter firstBetweenParam = new Parameter( "count__lo", "sql_alias", new Int32SqlType() );
			expectedParams[ 0 ] = firstBetweenParam;

			Parameter secondBetweenParam = new Parameter( "count__hi", "sql_alias", new Int32SqlType() );
			expectedParams[ 1 ] = secondBetweenParam;

			CompareSqlStrings( sqlString, expectedSql, expectedParams );

			session.Close();
		}
	}
}