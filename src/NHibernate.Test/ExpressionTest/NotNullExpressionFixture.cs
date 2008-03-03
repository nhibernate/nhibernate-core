using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
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
			SqlString sqlString = notNullExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

			string expectedSql = "sql_alias.address is not null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}