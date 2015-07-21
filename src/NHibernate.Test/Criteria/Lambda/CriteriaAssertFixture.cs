
using System;

using NUnit.Framework;

using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class CriteriaAssertFixture : LambdaFixtureBase
	{

		private void AssertCriteriaAreNotEqual(ICriteria expected, ICriteria actual)
		{
			try
			{
				AssertCriteriaAreEqual(expected, actual);
			}
			catch
			{
				return;
			}
			Assert.Fail("No exception thrown");
		}

		private void AssertCriteriaAreNotEqual(DetachedCriteria expected, DetachedCriteria actual)
		{
			try
			{
				AssertCriteriaAreEqual(expected, actual);
			}
			catch
			{
				return;
			}
			Assert.Fail("No exception thrown");
		}

		[Test]
		public void DifferentTypes()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>();

			DetachedCriteria actual =
				DetachedCriteria.For<Child>();

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentAliases()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>("personAlias1");

			DetachedCriteria actual =
				DetachedCriteria.For<Person>("personAlias2");

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentOperators()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.Add(Expression.Eq("Property", "Value"));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.Add(Expression.Gt("Property", "Value"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentPaths()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.Add(Expression.Eq("a.b.Property1", "Value"));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.Add(Expression.Eq("a.b.Property2", "Value"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentValues()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.Add(Expression.Eq("Property", "Value1"));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.Add(Expression.Eq("Property", "Value2"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentNestedCriterion()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.Add(Expression.Not(Expression.Eq("Property", "Value")));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.Add(Expression.Not(Expression.Gt("Property", "Value")));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentOrder()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.AddOrder(Order.Asc("name"));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.AddOrder(Order.Desc("name"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentSubCriteria()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.CreateCriteria("Child")
						.Add(Expression.Eq("Name", "test"));

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.CreateCriteria("Child")
						.Add(Expression.Gt("Name", "test"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentJoinType()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.CreateCriteria("Child", JoinType.InnerJoin);

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.CreateCriteria("Child", JoinType.LeftOuterJoin);

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentFetchMode()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.SetFetchMode("Father", FetchMode.Eager);

			DetachedCriteria actual =
				DetachedCriteria.For<Person>()
					.SetFetchMode("Father", FetchMode.Lazy);

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentLockMode()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.SetLockMode("fatherAlias", LockMode.Upgrade);

			ICriteria actual =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.SetLockMode("fatherAlias", LockMode.Force);

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentProjections()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetProjection(Projections.Avg("Age"));

			ICriteria actual =
				CreateTestCriteria(typeof(Person))
					.SetProjection(Projections.Max("Age"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentPropertyName()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.Add(Expression.GtProperty("Age1", "fatherAlias.Age"));

			ICriteria actual =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.Add(Expression.GtProperty("Age2", "fatherAlias.Age"));

			AssertCriteriaAreNotEqual(expected, actual);
		}

		[Test]
		public void DifferentSubquery()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyIn("Name", DetachedCriteria.For<Person>().Add(Expression.Eq("Name", "subquery test"))));

			ICriteria actual =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyIn("Name", DetachedCriteria.For<Person>().Add(Expression.Eq("Name", "subqueryx test"))));

			AssertCriteriaAreNotEqual(expected, actual);
		}

	}

}
