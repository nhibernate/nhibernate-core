using NHibernate.Criterion;
using NHibernate.DomainModel;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for NotNullExpressionFixture.
	/// </summary>
	[TestFixture]
	public class NotNullExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void NotNullSqlStringTest()
		{
			ISession session = factory.OpenSession();

			ICriterion notNullExpression = Expression.IsNotNull("Address");

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = notNullExpression.ToSqlString(criteria, criteriaQuery);

			string expectedSql = "sql_alias.address is not null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}
