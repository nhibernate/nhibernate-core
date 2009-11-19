using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Criterion;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class RestrictionsFixture : LambdaFixtureBase
	{

		[Test]
		public void ArbitraryCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Lt("Age", 65))
					.Add(Restrictions.Ge("personAlias.Age", 18));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(Restrictions.Where<Person>(p => p.Age < 65))
					.And(Restrictions.Where(() => personAlias.Age >= 18));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SqlFunctions()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Between("Age", 18, 65))
					.Add(Restrictions.Between("personAlias.Age", 18, 65))
					.Add(Restrictions.InsensitiveLike("Name", "test"))
					.Add(Restrictions.InsensitiveLike("Name", "tEsT", MatchMode.Anywhere))
					.Add(Restrictions.IsEmpty("Children"))
					.Add(Restrictions.IsNotEmpty("Children"))
					.Add(Restrictions.IsNotNull("Name"))
					.Add(Restrictions.IsNull("Name"))
					.Add(Restrictions.Like("Name", "%test%"))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere))
					.Add(Restrictions.Like("Name", "test", MatchMode.Anywhere, '?'));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(Restrictions.WhereProperty<Person>(p => p.Age).IsBetween(18).And(65))
					.And(Restrictions.WhereProperty(() => personAlias.Age).IsBetween(18).And(65))
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsInsensitiveLike("test"))
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsInsensitiveLike("tEsT", MatchMode.Anywhere))
					.And(Restrictions.WhereProperty<Person>(p => p.Children).IsEmpty)
					.And(Restrictions.WhereProperty<Person>(p => p.Children).IsNotEmpty)
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsNotNull)
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsNull)
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsLike("%test%"))
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsLike("test", MatchMode.Anywhere))
					.And(Restrictions.WhereProperty<Person>(p => p.Name).IsLike("test", MatchMode.Anywhere, '?'));

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}