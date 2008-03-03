using System;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for InsensitiveLikeExpressionFixture.
	/// </summary>
	[TestFixture]
	public class InsensitiveLikeExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void InsentitiveLikeSqlStringTest()
		{
			ISession session = factory.OpenSession();

			ICriterion expression = Expression.InsensitiveLike("Address", "12 Adress");

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

			string expectedSql = "lower(sql_alias.address) like ?";
			if ((factory as ISessionFactoryImplementor).Dialect is PostgreSQLDialect)
			{
				expectedSql = "sql_alias.address ilike ?";
			}

			CompareSqlStrings(sqlString, expectedSql, 1);

			session.Close();
		}
	}
}