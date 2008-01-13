using NHibernate.DomainModel;
using NHibernate.Expressions;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;

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
				ICriterion sqlExpression = Expression.Sql("{alias}.address is not null");

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

				string expectedSql = "sql_alias.address is not null";

				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void NoParamsSqlStringTest()
		{
			using (ISession session = factory.OpenSession())
			{
				ICriterion sqlExpression = Expressions.Expression.Sql(new SqlString("{alias}.address is not null"));

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

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

				ICriterion sqlExpression = Expressions.Expression.Sql(builder.ToSqlString(), "some address", NHibernateUtil.String);

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

				CompareSqlStrings(sqlString, expectedSql, 1);
			}
		}
	}
}