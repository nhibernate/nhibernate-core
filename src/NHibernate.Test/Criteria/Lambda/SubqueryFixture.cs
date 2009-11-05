using System;
using System.Collections;

using NUnit.Framework;

using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class SubqueryFixture : LambdaFixtureBase
	{

		private Person _subqueryPersonAlias = null;

		private DetachedCriteria DetachedCriteriaPerson
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverPerson);
			}
		}

		private DetachedCriteria DetachedCriteriaName
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverName);
			}
		}

		private QueryOver<Person> DetachedQueryOverPerson
		{
			get
			{
				return
					new QueryOver<Person>(() => _subqueryPersonAlias)
						.Where(() => _subqueryPersonAlias.Name == "subquery name");
			}
		}

		private QueryOver<Person> DetachedQueryOverName
		{
			get
			{
				return
					new QueryOver<Person>(() => _subqueryPersonAlias)
						.Where(() => _subqueryPersonAlias.Name == "subquery name")
						.Select(p => p.Name);
			}
		}

		[Test]
		public void Property()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereProperty(p => p.Name).Eq(DetachedQueryOverName);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyAsSyntax()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.Where(p => p.Name == DetachedQueryOverName.As<string>());

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}