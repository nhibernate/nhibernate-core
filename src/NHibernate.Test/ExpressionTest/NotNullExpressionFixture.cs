using System;

using NHibernate.Util;
using NExpression = NHibernate.Expression;
using NHibernate.SqlCommand;

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
			SqlString sqlString = notNullExpression.ToSqlString(criteria, criteriaQuery, CollectionHelper.EmptyMap);

			string expectedSql = "sql_alias.address is not null";
			CompareSqlStrings(sqlString, expectedSql, 0);

			session.Close();
		}

	}
}
