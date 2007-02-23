using System;
using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;
using NExpression = NHibernate.Expression;

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

			ICriterion notNullExpression = Expression.Expression.IsNotNull("Address");

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = notNullExpression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

			string expectedSql = "sql_alias.address is not null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}