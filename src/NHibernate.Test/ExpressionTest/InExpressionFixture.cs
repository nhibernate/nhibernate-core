using System;
using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Summary description for InExpressionFixture.
	/// </summary>
	[TestFixture]
	public class InExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void InSqlStringTest()
		{
			ISession session = factory.OpenSession();

			ICriterion inExpression = Expression.In("Count", new int[] {3, 4, 5});

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = inExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

			string expectedSql = "sql_alias.count_ in (?, ?, ?)";

			CompareSqlStrings(sqlString, expectedSql, 3);

			session.Close();
		}

		[Test]
		public void InEmptyList()
		{
			ISession session = factory.OpenSession();
			InExpression expression = new InExpression("Count", new object[0]);
			CreateObjects(typeof(Simple), session);
			SqlString sql = expression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			Assert.AreEqual("1=0", sql.ToString());
			session.Close();
		}

		[Test]
		public void InSqlFunctionTest()
		{
			using (var session = factory.OpenSession())
			{
				CreateObjects(typeof(Simple), session);
				var inExpression = Restrictions.In(
					Projections.SqlFunction(
						"substring",
						NHibernateUtil.String,
						Projections.Property("Name"),
						Projections.Constant(1),
						Projections.Constant(1)),
					new object[] { "A", "B" });
				var sql = inExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
				Assert.That(sql.ToString(), Is.EqualTo("substring(sql_alias.Name, ?, ?) in (?, ?)"));
				Assert.That(criteriaQuery.CollectedParameters.Count, Is.EqualTo(4));
			}
		}
	}
}