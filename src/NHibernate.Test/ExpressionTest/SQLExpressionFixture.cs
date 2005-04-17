using System;

using NHibernate.SqlCommand;
using NExpression = NHibernate.Expression;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest 
{
	/// <summary>
	/// Summary description for SQLExpressionFixture.
	/// </summary>
	[TestFixture]
	public class SQLExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void StraightSqlTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion sqlExpression = NExpression.Expression.Sql("$alias.address is not null");

			SqlString sqlString = sqlExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.address is not null";
			
			CompareSqlStrings(sqlString, expectedSql);

			session.Close();
		}

		[Test]
		public void NoParamsSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion sqlExpression = NExpression.Expression.Sql( new SqlString( "$alias.address is not null") );

			SqlString sqlString = sqlExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.address is not null";
			
			CompareSqlStrings(sqlString, expectedSql);

			session.Close();
		}

		[Test]
		public void WithParameterTest() 
		{
			ISession session = factory.OpenSession();
			SqlStringBuilder builder = new SqlStringBuilder();
			
			string expectedSql = "simple_alias.address = :address";
			Parameter[] expectedParams = new Parameter[1];
			
			Parameter firstAndParam = new Parameter( "address", new SqlTypes.StringSqlType() );
			expectedParams[0] = firstAndParam;

			builder.Add( "$alias.address = " );
			builder.Add( firstAndParam );

			NExpression.ICriterion sqlExpression = NExpression.Expression.Sql(builder.ToSqlString(), "some address", NHibernateUtil.String );

			SqlString sqlString = sqlExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();
		}
	}
}
