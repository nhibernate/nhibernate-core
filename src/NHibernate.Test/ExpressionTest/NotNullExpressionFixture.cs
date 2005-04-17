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
	/// Summary description for NotNullExpressionFixture.
	/// </summary>
	[TestFixture]
	public class NotNullExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void NotNullSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion notNullExpression = NExpression.Expression.IsNotNull("Address");

			SqlString sqlString = notNullExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.address IS NOT NULL";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}

	}
}
