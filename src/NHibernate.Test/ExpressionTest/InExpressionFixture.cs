using System;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.DomainModel;
using NHibernate.SqlCommand;
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

			ICriterion inExpression = Expression.In("Count", new int[] { 3, 4, 5 });

			CreateObjects(typeof(Simple), session);
			SqlString sqlString = inExpression.ToSqlString(criteria, criteriaQuery);

			string expectedSql = "sql_alias.count_ in (?, ?, ?)";

			CompareSqlStrings(sqlString, expectedSql, 3);

			session.Close();
		}

		[Test]
		public void InEmptyList()
		{
			ISession session = factory.OpenSession();
			InExpression expression = new InExpression("Count", Array.Empty<object>());
			CreateObjects(typeof(Simple), session);
			SqlString sql = expression.ToSqlString(criteria, criteriaQuery);
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
				var sql = inExpression.ToSqlString(criteria, criteriaQuery);

				// Allow some dialectal differences in function name and parameter style.
				Assert.That(sql.ToString(),
					Does.StartWith("substring(sql_alias.Name").Or.StartsWith("substr(sql_alias.Name"));
				Assert.That(sql.ToString(), Does.EndWith(") in (?, ?)"));

				// Ensure no parameters are duplicated.
				var parameters = criteriaQuery.CollectedParameters.ToList();
				Assert.That(parameters.Count, Is.EqualTo(4));
				Assert.That(parameters[0].Value, Is.EqualTo(1));
				Assert.That(parameters[1].Value, Is.EqualTo(1));
				Assert.That(parameters[2].Value, Is.EqualTo("A"));
				Assert.That(parameters[3].Value, Is.EqualTo("B"));
			}
		}
	}
}
