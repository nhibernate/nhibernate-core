using System;
using NHibernate.Util;
using NExpression = NHibernate.Expression;
using NHibernate.SqlCommand;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Test the PropertyExpression class.
	/// </summary>
	/// <remarks>
	/// There are no need for the subclasses EqPropertyExpression,
	/// LePropertyExpression, or LtPropertyExpression to have their own 
	/// TestFixtures because all they do is override one property.
	/// </remarks>
	[TestFixture]
	public class PropertyExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void SqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion expression = NExpression.Expression.EqProperty("Address", "Name");

			CreateObjects( typeof( Simple ), session );
			SqlString sqlString = expression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

			string expectedSql = "sql_alias.address = sql_alias.Name";

			CompareSqlStrings(sqlString, expectedSql);

			session.Close();
		}
	}
}
