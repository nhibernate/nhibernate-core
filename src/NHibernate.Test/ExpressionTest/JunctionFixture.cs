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
		NExpression.Conjunction _conjunction;
		
		[SetUp]
		public override void SetUp() 
		{
			base.SetUp();
			_conjunction = NExpression.Expression.Conjunction();
			_conjunction.Add(NExpression.Expression.IsNull("Address"))
				.Add(NExpression.Expression.Between("Count", 5, 10));
		}

		[Test]
		public void SqlString()
		{
			SqlString sqlString = _conjunction.ToSqlString(factoryImpl, typeof(Simple), "simple_alias", BaseExpressionFixture.EmptyAliasClasses );
			
			string expectedSql = "(simple_alias.address is null and simple_alias.count_ between :simple_alias.count__lo and :simple_alias.count__hi)";
			
			CompareSqlStrings(sqlString, expectedSql, 2);
	
		}

		[Test]
		public void GetTypedValues() 
		{
			TypedValue[] typedValues = _conjunction.GetTypedValues( factoryImpl, typeof(Simple), BaseExpressionFixture.EmptyAliasClasses );

			TypedValue[] expectedTV = new TypedValue[2];
			expectedTV[0] = new TypedValue(NHibernateUtil.Int32, 5);
			expectedTV[1] = new TypedValue(NHibernateUtil.Int32, 10);

			Assert.AreEqual(2, typedValues.Length);

			for(int i=0; i<typedValues.Length; i++) 
			{
				Assert.AreEqual(expectedTV[i].Type, typedValues[i].Type);
				Assert.AreEqual(expectedTV[i].Value, typedValues[i].Value);
			}
		}

		[Test]
		public void ToStringTest()
		{
			Assert.AreEqual( "(Address is null and Count between 5 and 10)",
				_conjunction.ToString() );
		}
		
	}
}
