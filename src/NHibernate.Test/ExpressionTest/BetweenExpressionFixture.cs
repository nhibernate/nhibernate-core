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
	/// Summary description for BetweenExpressionFixture.
	/// </summary>
	[TestFixture]
	public class BetweenExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void BetweenSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.Expression betweenExpression = NExpression.Expression.Between("Count", 5, 10);
			SqlString sqlString = betweenExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias");


			string expectedSql = "simple_alias.count_ between :simple_alias.count__lo and :simple_alias.count__hi";
			Parameter[] expectedParams = new Parameter[2];

			Parameter firstBetweenParam = new Parameter( "count__lo", "simple_alias", new SqlTypes.Int32SqlType() );
			expectedParams[0] = firstBetweenParam;
			
			Parameter secondBetweenParam = new Parameter( "count__hi", "simple_alias", new SqlTypes.Int32SqlType() );
			expectedParams[1] = secondBetweenParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();

		}
	}
}
