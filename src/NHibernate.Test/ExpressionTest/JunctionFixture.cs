using NHibernate.DomainModel;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
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
		private Conjunction _conjunction;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			_conjunction = Expression.Conjunction();
			_conjunction.Add(Expression.IsNull("Address"))
				.Add(Expression.Between("Count", 5, 10));
		}

		[Test]
		public void SqlString()
		{
			SqlString sqlString;
			using (ISession session = factory.OpenSession())
			{
				CreateObjects(typeof(Simple), session);
				sqlString = _conjunction.ToSqlString(criteria, criteriaQuery);
			}

			string expectedSql = "(sql_alias.address is null and sql_alias.count_ between ? and ?)";

			CompareSqlStrings(sqlString, expectedSql, 2);
		}

		[Test]
		public void GetTypedValues()
		{
			TypedValue[] typedValues;
			using (ISession session = factory.OpenSession())
			{
				CreateObjects(typeof(Simple), session);
				typedValues = _conjunction.GetTypedValues(criteria, criteriaQuery);
			}

			TypedValue[] expectedTV = new TypedValue[2];
			expectedTV[0] = new TypedValue(NHibernateUtil.Int32, 5);
			expectedTV[1] = new TypedValue(NHibernateUtil.Int32, 10);

			Assert.AreEqual(2, typedValues.Length);

			for (int i = 0; i < typedValues.Length; i++)
			{
				Assert.AreEqual(expectedTV[i].Type, typedValues[i].Type);
				Assert.AreEqual(expectedTV[i].Value, typedValues[i].Value);
			}
		}

		[Test]
		public void ToStringTest()
		{
			Assert.AreEqual("(Address is null and Count between 5 and 10)",
			                _conjunction.ToString());
		}
	}
}