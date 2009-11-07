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

		private Child _subqueryChildAlias = null;

		private DetachedCriteria DetachedCriteriaChild
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverChild);
			}
		}

		private DetachedCriteria DetachedCriteriaName
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverName);
			}
		}

		private DetachedCriteria DetachedCriteriaAge
		{
			get
			{
				return ToDetachedCriteria(DetachedQueryOverAge);
			}
		}

		private QueryOver<Child> DetachedQueryOverChild
		{
			get
			{
				return
					new QueryOver<Child>(() => _subqueryChildAlias)
						.Where(() => _subqueryChildAlias.Nickname == "subquery name");
			}
		}

		private QueryOver<Child> DetachedQueryOverName
		{
			get
			{
				return
					new QueryOver<Child>(() => _subqueryChildAlias)
						.Where(() => _subqueryChildAlias.Nickname == "subquery name")
						.Select(p => p.Nickname);
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
		public void Property()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGe("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGt("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyIn("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyLe("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLt("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyNe("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyNotIn("Name", DetachedCriteriaName));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereProperty(p => p.Name).Eq(DetachedQueryOverName)
					.WithSubquery.WhereProperty(p => p.Age).Ge(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).Gt(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Name).In(DetachedQueryOverName)
					.WithSubquery.WhereProperty(p => p.Age).Le(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).Lt(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Name).Ne(DetachedQueryOverName)
					.WithSubquery.WhereProperty(p => p.Name).NotIn(DetachedQueryOverName);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyAsSyntax()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyNe("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGe("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGt("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLe("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLt("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.Where(p => p.Name == DetachedQueryOverName.As<string>())
					.WithSubquery.Where(p => p.Name != DetachedQueryOverName.As<string>())
					.WithSubquery.Where(p => p.Age >= DetachedQueryOverAge.As<int>())
					.WithSubquery.Where(p => p.Age > DetachedQueryOverAge.As<int>())
					.WithSubquery.Where(p => p.Age <= DetachedQueryOverAge.As<int>())
					.WithSubquery.Where(p => p.Age < DetachedQueryOverAge.As<int>());

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Value()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.Eq("Name", DetachedCriteriaName))
					.Add(Subqueries.Ge("Age", DetachedCriteriaAge))
					.Add(Subqueries.Gt("Age", DetachedCriteriaAge))
					.Add(Subqueries.In("Name", DetachedCriteriaName))
					.Add(Subqueries.Le("Age", DetachedCriteriaAge))
					.Add(Subqueries.Lt("Age", DetachedCriteriaAge))
					.Add(Subqueries.Ne("Name", DetachedCriteriaName))
					.Add(Subqueries.NotIn("Name", DetachedCriteriaName));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereValue("Name").Eq(DetachedQueryOverName)
					.WithSubquery.WhereValue("Age").Ge(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").Gt(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Name").In(DetachedQueryOverName)
					.WithSubquery.WhereValue("Age").Le(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").Lt(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Name").Ne(DetachedQueryOverName)
					.WithSubquery.WhereValue("Name").NotIn(DetachedQueryOverName);

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}