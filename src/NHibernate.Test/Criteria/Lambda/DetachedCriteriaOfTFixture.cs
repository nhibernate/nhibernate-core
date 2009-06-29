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
	public class DetachedCriteriaOfTFixture : LambdaFixtureBase
	{

		[Test]
		public void SimpleCriterion_NoAlias()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.Add(Restrictions.Eq("Name", "test name"));

			DetachedCriteria<Person> actual =
				DetachedCriteria.QueryOver<Person>()
					.Where(p => p.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Where_BehavesTheSameAs_And()
		{
			DetachedCriteria<Person> expected =
				DetachedCriteria.QueryOver<Person>()
					.And(p => p.Name == "test name");

			DetachedCriteria<Person> actual =
				DetachedCriteria.QueryOver<Person>()
					.Where(p => p.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}