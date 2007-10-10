using System;
using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Util;
using NUnit.Framework;
using NExpression = NHibernate.Expression;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Test the SimpleExpression class.
	/// </summary>
	/// <remarks>
	/// There are no need for the subclasses EqExpression, GeExpression,
	/// LeExpression,  LikeExpression, or LtExpression to have their own 
	/// TestFixtures because all they do is override one property.
	/// </remarks>
	[TestFixture]
	public class SimpleExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void SimpleSqlStringTest()
		{
			ISession session = factory.OpenSession();

			CreateObjects(typeof(Simple), session);

			ICriterion andExpression = Expression.Expression.Eq("Address", "12 Adress");

			SqlString sqlString = andExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

			string expectedSql = "sql_alias.address = ?";
			CompareSqlStrings(sqlString, expectedSql, 1);

			session.Close();
		}

		[Test]
		public void TestQuoting()
		{
			using (ISession session = factory.OpenSession())
			{
				DateTime now = DateTime.Now;

				CreateObjects(typeof(SimpleComponent), session);

				ICriterion andExpression = Expression.Expression.Eq("Date", now);

				SqlString sqlString = andExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
				string quotedColumn = dialect.QuoteForColumnName("d[at]e_");
				string expectedSql = "sql_alias." + quotedColumn + " = ?";

				CompareSqlStrings(sqlString, expectedSql);
			}
		}

		[Test]
		public void SimpleDateExpression()
		{
			using (ISession session = factory.OpenSession())
			{
				CreateObjects(typeof(Simple), session);
				ICriterion andExpression = Expression.Expression.Ge("Date", DateTime.Now);

				SqlString sqlString = andExpression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());

				string expectedSql = "sql_alias.date_ >= ?";
				CompareSqlStrings(sqlString, expectedSql, 1);
			}
		}

		[Test]
		[ExpectedException(typeof(QueryException))]
		public void MisspelledPropertyWithNormalizedEntityPersister()
		{
			using (ISession session = factory.OpenSession())
			{
				CreateObjects(typeof(Multi), session);

				ICriterion expression = Expression.Expression.Eq("MisspelledProperty", DateTime.Now);
				expression.ToSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			}
		}
	}
}