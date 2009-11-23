using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Criterion;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class ProjectionsFixture : LambdaFixtureBase
	{

		private Child _subqueryChildAlias = null;

		private DetachedCriteria DetachedCriteriaAge
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverAge);
			}
		}

		private QueryOver<Child> DetachedQueryOverAge
		{
			get
			{
				return
					new QueryOver<Child>(() => _subqueryChildAlias)
						.Where(() => _subqueryChildAlias.Nickname == "subquery name")
						.Select(p => p.Age);
			}
		}

		[Test]
		public void ArbitraryProjections()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.Alias(Projections.Avg("Age"), "personAgeProjectionAlias"))
						.Add(Projections.Avg("Age"), "personAgeProjectionAlias")
						.Add(Projections.Avg("personAlias.Age"))
						.Add(Projections.Count("Age"))
						.Add(Projections.Count("personAlias.Age"))
						.Add(Projections.CountDistinct("Age"))
						.Add(Projections.CountDistinct("personAlias.Age"))
						.Add(Projections.GroupProperty("Age"))
						.Add(Projections.GroupProperty("personAlias.Age"))
						.Add(Projections.Max("Age"))
						.Add(Projections.Max("personAlias.Age"))
						.Add(Projections.Min("Age"))
						.Add(Projections.Min("personAlias.Age"))
						.Add(Projections.Property("Age"))
						.Add(Projections.Property("personAlias.Age"))
						.Add(Projections.SubQuery(DetachedCriteriaAge))
						.Add(Projections.Sum("Age"))
						.Add(Projections.Sum("personAlias.Age")));

			Person personAlias = null;
			Person personAgeProjectionAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Select(Projections.ProjectionList()
						.Add(Projections.Avg<Person>(p => p.Age).WithAlias(() => personAgeProjectionAlias))
						.Add(Projections.Avg<Person>(p => p.Age), () => personAgeProjectionAlias)
						.Add(Projections.Avg(() => personAlias.Age))
						.Add(Projections.Count<Person>(p => p.Age))
						.Add(Projections.Count(() => personAlias.Age))
						.Add(Projections.CountDistinct<Person>(p => p.Age))
						.Add(Projections.CountDistinct(() => personAlias.Age))
						.Add(Projections.GroupProperty<Person>(p => p.Age))
						.Add(Projections.GroupProperty(() => personAlias.Age))
						.Add(Projections.Max<Person>(p => p.Age))
						.Add(Projections.Max(() => personAlias.Age))
						.Add(Projections.Min<Person>(p => p.Age))
						.Add(Projections.Min(() => personAlias.Age))
						.Add(Projections.Property<Person>(p => p.Age))
						.Add(Projections.Property(() => personAlias.Age))
						.Add(Projections.SubQuery(DetachedQueryOverAge))
						.Add(Projections.Sum<Person>(p => p.Age))
						.Add(Projections.Sum(() => personAlias.Age)));

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}