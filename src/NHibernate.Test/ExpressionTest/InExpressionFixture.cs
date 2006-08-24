using System;

using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

using NUnit.Framework;

using NExpression = NHibernate.Expression;

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

			ICriterion inExpression = Expression.Expression.In( "Count", new int[ ] {3, 4, 5} );

			CreateObjects( typeof( Simple ), session );
			SqlString sqlString = inExpression.ToSqlString( criteria, criteriaQuery );

			string expectedSql = "sql_alias.count_ in (:sql_alias.count__0, :sql_alias.count__1, :sql_alias.count__2)";
			Parameter[ ] expectedParams = new Parameter[3];

			for( int i = 0; i < expectedParams.Length; i++ )
			{
				Parameter param = new Parameter( "count_" + "_" + i, "sql_alias", SqlTypeFactory.Int32 );
				expectedParams[ i ] = param;
			}

			CompareSqlStrings( sqlString, expectedSql, expectedParams );

			session.Close();
		}

		[Test]
		public void InEmptyList()
		{
			ISession session = factory.OpenSession();
			InExpression expression = new InExpression( "Count", new object[0] );
			CreateObjects( typeof( Simple ), session );
			SqlString sql = expression.ToSqlString( criteria, criteriaQuery );
			Assert.AreEqual( "1=0", sql.ToString() );
			session.Close();
		}
	}
}