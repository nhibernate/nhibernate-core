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
			
			NExpression.Expression andExpression = NExpression.Expression.Eq("Address", "12 Adress");

			SqlString sqlString = andExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias");

			string expectedSql = "simple_alias.address = :simple_alias.address";
			Parameter[] expectedParams = new Parameter[1];
			
			ParameterLength firstAndParam = new ParameterLength();
			firstAndParam.DbType = DbType.String;
			firstAndParam.TableAlias = "simple_alias";
			firstAndParam.Length = 200;
			firstAndParam.Name = "address";

			expectedParams[0] = firstAndParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();
		}


	}
}
