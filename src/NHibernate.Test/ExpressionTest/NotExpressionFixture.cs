using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
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
			SqlString sqlString = notExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

			string expectedSql = "not (sql_alias.address = ?)";
			CompareSqlStrings(sqlString, expectedSql, 1);
			session.Close();
		}
	}
}