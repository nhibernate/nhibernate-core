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
	/// Test the Junction class.
	/// </summary>
	/// <remarks>
	/// There are no need for the subclasses Conjunction and Disjunction to have their own 
	/// TestFixtures because all they do is override one property.
	/// </remarks>
	[TestFixture]
	public class JunctionFixture : BaseExpressionFixture
	{
		
		[Test]
		public void JunctionSqlStringTest()
		{
			
			ISession session = factory.OpenSession();
			
			NExpression.Conjunction conjunction = NExpression.Expression.Conjunction();
			conjunction.Add(NExpression.Expression.IsNull("Address"))
				.Add(NExpression.Expression.Between("Count", 5, 10));

			SqlString sqlString = conjunction.ToSqlString(factoryImpl, typeof(Simple), "simple_alias");

			string expectedSql = "(simple_alias.address IS NULL and simple_alias.count_ between :simple_alias.count__lo and :simple_alias.count__hi)";
			
			CompareSqlStrings(sqlString, expectedSql, 2);
	
			session.Close();
		}

		
	}
}
