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
using NHibernate.SqlTypes;

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
			
			CreateObjects( typeof( Simple ), session );
			
			NExpression.ICriterion andExpression = NExpression.Expression.Eq("Address", "12 Adress");

			SqlString sqlString = andExpression.ToSqlString( criteria, criteriaQuery );

			string expectedSql = "sql_alias.address = ?";
			CompareSqlStrings(sqlString, expectedSql, 1);

			session.Close();
		}
	
		[Test]
		public void TestQuoting() 
		{
			using( ISession session = factory.OpenSession() )
			{
				DateTime now = DateTime.Now;

				CreateObjects( typeof( SimpleComponent ), session );

				NExpression.ICriterion andExpression = NExpression.Expression.Eq( "Date", now );

				SqlString sqlString = andExpression.ToSqlString( criteria, criteriaQuery );
				string quotedColumn = dialect.QuoteForColumnName( "d[at]e_" );
				string expectedSql = "sql_alias." + quotedColumn + " = ?";
			
				CompareSqlStrings( sqlString, expectedSql );
			}
		}

		[Test]
		public void SimpleDateExpression() 
		{
			using( ISession session = factory.OpenSession() )
			{
				CreateObjects( typeof( Simple ), session );
				NExpression.ICriterion andExpression = NExpression.Expression.Ge( "Date", DateTime.Now );

				SqlString sqlString = andExpression.ToSqlString( criteria, criteriaQuery );

				string expectedSql = "sql_alias.date_ >= ?";
				CompareSqlStrings(sqlString, expectedSql, 1);
			}
		}

		[Test]
		[ExpectedException(typeof(QueryException))]
		public void MisspelledPropertyWithNormalizedEntityPersister() 
		{
			using( ISession session = factory.OpenSession() )
			{
				CreateObjects( typeof( Multi ), session );

				NExpression.ICriterion expression = NExpression.Expression.Eq( "MisspelledProperty", DateTime.Now );
				expression.ToSqlString( criteria, criteriaQuery );
			}
		}

	}
}
