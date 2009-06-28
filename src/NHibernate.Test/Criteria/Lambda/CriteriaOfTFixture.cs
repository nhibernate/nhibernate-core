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
					.Add(Restrictions.Eq("Name", "test name"));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Name == "test name");

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
		public void SubCriteria_Join_ToOne()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Father")
						.Add(Expression.Eq("Name", "test name"));

			ICriteria<Person> actual =
				CreateTestQueryOver<Person>()
					.Join(p => p.Father) // sub-criteria
						.Where(f => f.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_Join_ToMany()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Children")
						.Add(Expression.Eq("Nickname", "test name"));

			ICriteria<Child> actual =
				CreateTestQueryOver<Person>()
					.Join<Child>(p => p.Children) // sub-criteria
						.Where(c => c.Nickname == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}