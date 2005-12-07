using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NExpression = NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;
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
			
			NExpression.ICriterion andExpression = NExpression.Expression.Eq("Address", "12 Adress");

			SqlString sqlString = andExpression.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );

			string expectedSql = "simple_alias.address = :simple_alias.address";
			Parameter[] expectedParams = new Parameter[1];
			
			// even though a String parameter is a Size based Parameter it will not
			// be a ParameterLength unless in the mapping file it is defined as
			// type="String(200)" -> in the mapping file it is now defined as 
			// type="String" length="200"
			Parameter firstAndParam = new Parameter( "address", "simple_alias", new SqlTypes.StringSqlType() );
			expectedParams[0] = firstAndParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();
		}
	
		[Test]
		public void TestQuoting() 
		{
			ISession session = factory.OpenSession();
			DateTime now = DateTime.Now;

			NExpression.ICriterion andExpression = NExpression.Expression.Eq( "Date", now );

			SqlString sqlString = andExpression.ToSqlString( factoryImpl, typeof(SimpleComponent), "simp_comp", BaseExpressionFixture.EmptyAliasClasses  );
			string quotedColumn = dialect.QuoteForColumnName( "d[at]e_" );
			string expectedSql = "simp_comp." + quotedColumn + " = :simp_comp." + quotedColumn;
			
			CompareSqlStrings( sqlString, expectedSql );
			
			session.Close();
		}

		[Test]
		public void SimpleDateExpression() 
		{
			ISession session = factory.OpenSession();
			
			NExpression.ICriterion andExpression = NExpression.Expression.Ge( "Date", DateTime.Now );

			SqlString sqlString = andExpression.ToSqlString( factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses  );

			string expectedSql = "simple_alias.date_ >= :simple_alias.date_";
			Parameter[] expectedParams = new Parameter[1];
			
			Parameter firstAndParam = new Parameter( "date_", "simple_alias", new SqlTypes.DateTimeSqlType() );
			expectedParams[0] = firstAndParam;

			CompareSqlStrings(sqlString, expectedSql, expectedParams);

			session.Close();
		}

		[Test]
		[ExpectedException(typeof(QueryException))]
		public void MisspelledPropertyWithNormalizedEntityPersister() 
		{
			NExpression.ICriterion expression = NExpression.Expression.Eq( "MisspelledProperty", DateTime.Now );
			expression.ToSqlString( factoryImpl, typeof(Multi), "multi", BaseExpressionFixture.EmptyAliasClasses  );
		}

	}
}
