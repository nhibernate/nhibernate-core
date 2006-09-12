using System;

using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;

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

			string expectedSql = "sql_alias.count_ between ? and ?";
			CompareSqlStrings( sqlString, expectedSql, 2 );

			session.Close();
		}
	}
}