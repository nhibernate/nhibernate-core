using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
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
				ICriterion sqlExpression = Restrictions.Sql("{alias}.address is not null");

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery);

				string expectedSql = "sql_alias.address is not null";

				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void NoParamsSqlStringTest()
		{
			using (ISession session = factory.OpenSession())
			{
				ICriterion sqlExpression = Restrictions.Sql(new SqlString("{alias}.address is not null"));

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery);

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

				ICriterion sqlExpression = Restrictions.Sql(builder.ToSqlString(), "some address", NHibernateUtil.String);

				CreateObjects(typeof(Simple), session);
				SqlString sqlString = sqlExpression.ToSqlString(criteria, criteriaQuery);

				CompareSqlStrings(sqlString, expectedSql, 1);
			}
		}
	}
}
