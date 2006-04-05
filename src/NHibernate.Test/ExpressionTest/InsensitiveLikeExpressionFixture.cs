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
	/// Summary description for InsensitiveLikeExpressionFixture.
	/// </summary>
	[TestFixture]
	public class InsensitiveLikeExpressionFixture : BaseExpressionFixture
	{
		[Test]
		public void InsentitiveLikeSqlStringTest() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion expression = NExpression.Expression.InsensitiveLike("Address", "12 Adress");

			CreateObjects( typeof( Simple ), session );
			SqlString sqlString = expression.ToSqlString( criteria, criteriaQuery );
			
			string expectedSql = "lower(sql_alias.address) like :sql_alias.address";
			if ((factory as ISessionFactoryImplementor).Dialect is Dialect.PostgreSQLDialect)
			{
				expectedSql = "sql_alias.address ilike :sql_alias.address";
			}
			Parameter[] expectedParams = new Parameter[1];

			Parameter firstParam = new Parameter( "address", "sql_alias", new SqlTypes.StringSqlType() );
			expectedParams[0] = firstParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();
		}


	}
}
