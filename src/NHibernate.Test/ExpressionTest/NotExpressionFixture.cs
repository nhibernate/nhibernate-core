using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for NotExpressionFixture.
	/// </summary>
	[TestFixture]
	public class NotExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void NotSqlStringTest()
		{
			ISession session = factory.OpenSession();

			ICriterion notExpression = Expression.Not(Expression.Eq("Address", "12 Adress"));

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = notExpression.ToSqlString(criteria, criteriaQuery);

			string expectedSql = "not (sql_alias.address = ?)";
			CompareSqlStrings(sqlString, expectedSql, 1);
			session.Close();
		}
	}
}
