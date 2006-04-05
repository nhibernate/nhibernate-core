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

			CreateObjects( typeof( Simple ), session );
			SqlString sqlString = notNullExpression.ToSqlString( criteria, criteriaQuery );

			string expectedSql = "sql_alias.address is not null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}

	}
}
