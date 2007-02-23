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
	/// Summary description for NullExpressionFixture.
	/// </summary>
	[TestFixture]
	public class NullExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void NullSqlStringTest()
		{
			ISession session = factory.OpenSession();

			ICriterion expression = Expression.Expression.IsNull("Address");

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

			string expectedSql = "sql_alias.address is null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}