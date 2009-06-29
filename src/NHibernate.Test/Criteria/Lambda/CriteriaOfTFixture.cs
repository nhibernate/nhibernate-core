using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class CriteriaOfTFixture : LambdaFixtureBase
	{

		[Test]
		public void SimpleCriterion_NoAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq("Name", "test name"))
					.Add(Restrictions.Not(Restrictions.Eq("Name", "not test name")))
					.Add(Restrictions.Gt("Age", 10))
					.Add(Restrictions.Ge("Age", 11))
					.Add(Restrictions.Lt("Age", 50))
					.Add(Restrictions.Le("Age", 49));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Name == "test name")
					.And(p => p.Name != "not test name")
					.And(p => p.Age > 10)
					.And(p => p.Age >= 11)
					.And(p => p.Age < 50)
					.And(p => p.Age <= 49);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyCriterion_NoAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.EqProperty("Age", "Height"))
					.Add(Restrictions.NotEqProperty("Age", "Height"))
					.Add(Restrictions.GtProperty("Age", "Height"))
					.Add(Restrictions.GeProperty("Age", "Height"))
					.Add(Restrictions.LtProperty("Age", "Height"))
					.Add(Restrictions.LeProperty("Age", "Height"));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Age == p.Height)
					.And(p => p.Age != p.Height)
					.And(p => p.Age > p.Height)
					.And(p => p.Age >= p.Height)
					.And(p => p.Age < p.Height)
					.And(p => p.Age <= p.Height);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void MultipleCriterionExpression()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.And(
						Restrictions.Eq("Name", "test name"),
						Restrictions.Or(
							Restrictions.Gt("Age", 21),
							Restrictions.Eq("HasCar", true))));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test name" && (p.Age > 21 || p.HasCar));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Where_BehavesTheSameAs_And()
		{
			Person personAlias = null;
			Impl.CriteriaImpl<Person> expected = (Impl.CriteriaImpl<Person>)
				CreateTestQueryOver<Person>(() => personAlias)
					.And(() => personAlias.Name == "test name")
					.And(p => p.Name == "test name");

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name")
					.Where(p => p.Name == "test name");

			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual);
		}

		[Test]
		public void SimpleCriterion_AliasReferenceSyntax()
		{
			ICriteria expected = 
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Eq("personAlias.Name", "test name"));

			Person personAlias = null;
			ICriteria<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinWalk_ToOne()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Father")
						.Add(Expression.Eq("Name", "test name"));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.JoinWalk(p => p.Father) // sub-criteria
						.Where(f => f.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinWalk_ToMany()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Children")
						.Add(Expression.Eq("Nickname", "test name"));

			ICriteria<Child> actual =
				CreateTestQueryOver<Person>()
					.JoinWalk<Child>(p => p.Children) // sub-criteria
						.Where(c => c.Nickname == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Alias_Join()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.CreateAlias("Children", "childAlias");

			Person fatherAlias = null;
			Child childAlias = null;
			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.Join(p => p.Father, () => fatherAlias)
					.Join(p => p.Children, () => childAlias);

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}