using System;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

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

			ICriterion expression = Expression.IsNull("Address");

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, criteriaQuery);

			string expectedSql = "sql_alias.address is null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}
	}
}