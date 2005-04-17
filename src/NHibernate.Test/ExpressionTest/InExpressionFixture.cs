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
	/// Summary description for InExpressionFixture.
	/// </summary>
	[TestFixture]
	public class InExpressionFixture : BaseExpressionFixture
	{
		
		[Test]
		public void InSqlStringTest() 
		{
			
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion inExpression = NExpression.Expression.In("Count", new int[]{3,4,5});

			SqlString sqlString = inExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.count_ in (:simple_alias.count__0, :simple_alias.count__1, :simple_alias.count__2)";
			Parameter[] expectedParams = new Parameter[3];

			for(int i = 0 ; i < expectedParams.Length; i++) 
			{
				Parameter param = new Parameter( "count_" + "_" + i, "simple_alias", new SqlTypes.Int32SqlType() );
				expectedParams[i] = param;
			}

			CompareSqlStrings(sqlString, expectedSql, expectedParams);
			
			session.Close();
		}


	}
}
