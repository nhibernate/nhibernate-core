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

			SqlString sqlString = inExpression.ToSqlString( factoryImpl, typeof( Simple ), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.count_ in (:simple_alias.count__0, :simple_alias.count__1, :simple_alias.count__2)";
			Parameter[ ] expectedParams = new Parameter[3];

			for( int i = 0; i < expectedParams.Length; i++ )
			{
				Parameter param = new Parameter( "count_" + "_" + i, "simple_alias", new Int32SqlType() );
				expectedParams[ i ] = param;
			}

			CompareSqlStrings( sqlString, expectedSql, expectedParams );

			session.Close();
		}

		[Test]
		public void InEmptyList()
		{
			InExpression expression = new InExpression( "Count", new object[0] );
			SqlString sql = expression.ToSqlString( factoryImpl, typeof( Simple ), "simple_alias", EmptyAliasClasses );
			Assert.AreEqual( "1=0", sql.ToString() );
		}
	}
}