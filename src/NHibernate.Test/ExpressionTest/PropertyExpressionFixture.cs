using System;

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
			
			NExpression.Expression andExpression = NExpression.Expression.EqProperty("Address", "Name");

			SqlString sqlString = andExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias");

			string expectedSql = "simple_alias.address = simple_alias.Name";

			CompareSqlStrings(sqlString, expectedSql);

			session.Close();
		}
	}
}
