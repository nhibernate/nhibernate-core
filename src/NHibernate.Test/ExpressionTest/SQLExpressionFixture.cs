using System;

using NHibernate.SqlCommand;
using NExpression = NHibernate.Expression;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest 
{
	[TestFixture]
	public class SQLExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void StraightSqlTest() 
		{
			using( ISession session = factory.OpenSession() )
			{
				NExpression.ICriterion sqlExpression = NExpression.Expression.Sql("{alias}.address is not null");
				
				CreateObjects( typeof( Simple ), session );
				SqlString sqlString = sqlExpression.ToSqlString( criteria, criteriaQuery );

				string expectedSql = "sql_alias.address is not null";
			
				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void NoParamsSqlStringTest() 
		{
			using( ISession session = factory.OpenSession() )
			{
				NExpression.ICriterion sqlExpression = NExpression.Expression.Sql( new SqlString( "{alias}.address is not null") );

				CreateObjects( typeof( Simple ), session );
				SqlString sqlString = sqlExpression.ToSqlString( criteria, criteriaQuery );

				string expectedSql = "sql_alias.address is not null";
			
				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void WithParameterTest() 
		{
			using( ISession session = factory.OpenSession() )
			{
				SqlStringBuilder builder = new SqlStringBuilder();
			
				string expectedSql = "sql_alias.address = ?";
				Parameter[] expectedParams = new Parameter[1];
			
				Parameter firstAndParam = Parameter.Placeholder;
				expectedParams[0] = firstAndParam;

				builder.Add( "{alias}.address = " );
				builder.Add( firstAndParam );

				NExpression.ICriterion sqlExpression = NExpression.Expression.Sql(builder.ToSqlString(), "some address", NHibernateUtil.String );

				CreateObjects( typeof( Simple ), session );
				SqlString sqlString = sqlExpression.ToSqlString( criteria, criteriaQuery );

				CompareSqlStrings(sqlString, expectedSql, expectedParams);
			}
		}
	}
}
