using System;
using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;
using NExpression = NHibernate.Expression;

namespace NHibernate.Test.ExpressionTest
{
	[TestFixture]
	public class SQLExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void StraightSqlTest()
		{
			using (ISession session = factory.OpenSession())
			{
				ICriterion sqlExpression = Expression.Expression.Sql("{alias}.address is not null");

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

				string expectedSql = "sql_alias.address is not null";

				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void NoParamsSqlStringTest()
		{
			using (ISession session = factory.OpenSession())
			{
				ICriterion sqlExpression = Expression.Expression.Sql(new SqlString("{alias}.address is not null"));

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

				string expectedSql = "sql_alias.address is not null";

				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void WithParameterTest()
		{
			using (ISession session = factory.OpenSession())
			{
				SqlStringBuilder builder = new SqlStringBuilder();

				string expectedSql = "sql_alias.address = ?";

				builder.Add("{alias}.address = ");
				builder.AddParameter();

				ICriterion sqlExpression = Expression.Expression.Sql(builder.ToSqlString(), "some address", NHibernateUtil.String);

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

				CompareSqlStrings(sqlString, expectedSql, 1);
			}
		}
	}
}