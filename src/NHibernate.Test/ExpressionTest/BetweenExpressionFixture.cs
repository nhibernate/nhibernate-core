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

			Parameter firstBetweenParam = new Parameter();
			firstBetweenParam.SqlType = new SqlTypes.Int32SqlType(); 
			firstBetweenParam.TableAlias = "simple_alias";
			firstBetweenParam.Name = "count__lo";
			expectedParams[0] = firstBetweenParam;
			
			Parameter secondBetweenParam = new Parameter();
			secondBetweenParam.SqlType = new SqlTypes.Int32SqlType();
			secondBetweenParam.TableAlias = "simple_alias";
			secondBetweenParam.Name = "count__hi";
			expectedParams[1] = secondBetweenParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();

		}
	}
}
