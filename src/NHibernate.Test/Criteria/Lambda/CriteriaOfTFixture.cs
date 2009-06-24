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
		public void Equality()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq("Name", "test name"));

			ICriteria<Person> actual =
				CreateTestCriteria<Person>()
					.Add(p => p.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}