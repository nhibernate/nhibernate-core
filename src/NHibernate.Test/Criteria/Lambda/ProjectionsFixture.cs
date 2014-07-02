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
					QueryOver.Of<Child>(() => _subqueryChildAlias)
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
						.Add(Projections.Avg("personAlias.Age"), "Age")
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
						.Add(Projections.Avg(() => personAlias.Age).WithAlias(() => personAlias.Age))
						.Add(Projections.Count<Person>(p => p.Age))
						.Add(Projections.Count(() => personAlias.Age))
						.Add(Projections.CountDistinct<Person>(p => p.Age))
						.Add(Projections.CountDistinct(() => personAlias.Age))
						.Add(Projections.Group<Person>(p => p.Age))
						.Add(Projections.Group(() => personAlias.Age))
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

		[Test]
		public void InlineProjectionList()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.Alias(Projections.Avg("Age"), "personAgeProjectionAlias"))
						.Add(Projections.Avg("Age"))
						.Add(Projections.Avg("personAlias.Age"), "Age")
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
				.SelectList(list => list
						.SelectAvg(p => p.Age).WithAlias(() => personAgeProjectionAlias)
						.Select(Projections.Avg("Age")) // allows private properties
						.SelectAvg(() => personAlias.Age).WithAlias(() => personAlias.Age)
						.SelectCount(p => p.Age)
						.SelectCount(() => personAlias.Age)
						.SelectCountDistinct(p => p.Age)
						.SelectCountDistinct(() => personAlias.Age)
						.SelectGroup(p => p.Age)
						.SelectGroup(() => personAlias.Age)
						.SelectMax(p => p.Age)
						.SelectMax(() => personAlias.Age)
						.SelectMin(p => p.Age)
						.SelectMin(() => personAlias.Age)
						.Select(p => p.Age)
						.Select(() => personAlias.Age)
						.SelectSubQuery(DetachedQueryOverAge)
						.SelectSum(p => p.Age)
						.SelectSum(() => personAlias.Age));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SelectSingleFunction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetProjection(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDate")));

			var actual =
				CreateTestQueryOver<Person>()
					.Select(p => p.BirthDate.Year);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SelectSingleFunctionOfDateTimeOffset()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetProjection(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")));

			var actual =
				CreateTestQueryOver<Person>()
					.Select(p => p.BirthDateAsDateTimeOffset.Year);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SelectMultipleFunction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDate")))
						.Add(Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("personAlias.BirthDate"))));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.SelectList(list => list
						.Select(p => p.BirthDate.Year)
						.Select(() => personAlias.BirthDate.Month));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SelectMultipleFunctionOfDateTimeOffset()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetProjection(Projections.ProjectionList()
						.Add(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("BirthDateAsDateTimeOffset")))
						.Add(Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("personAlias.BirthDateAsDateTimeOffset"))));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.SelectList(list => list
						.Select(p => p.BirthDateAsDateTimeOffset.Year)
						.Select(() => personAlias.BirthDateAsDateTimeOffset.Month));

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}