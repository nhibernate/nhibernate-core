using System;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NExpression = NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Test the LogicalExpression class.
	/// </summary>
	/// <remarks>
	/// There are no need for the subclasses AndExpresssion and OrExpression to have their own 
	/// TestFixtures because all they do is override one property.
	/// </remarks>
	[TestFixture]
	public class LogicalExpressionFixture : BaseExpressionFixture
	{
		
		[Test]
		public void LogicalSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion orExpression = NExpression.Expression.Or(NExpression.Expression.IsNull("Address"),NExpression.Expression.Between("Count", 5, 10) );

			
			SqlString sqlString = orExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "(simple_alias.address IS NULL or simple_alias.count_ between :simple_alias.count__lo and :simple_alias.count__hi)";
			
			CompareSqlStrings(sqlString, expectedSql, 2);

			session.Close();
		}
	}
}
