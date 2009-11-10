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
		public void PropertyAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Subqueries.PropertyEq("personAlias.Name", DetachedCriteriaName));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.WithSubquery.WhereProperty(() => personAlias.Name).Eq(DetachedQueryOverName);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyAll()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEqAll("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtAll("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereProperty(p => p.Name).EqAll(DetachedQueryOverName)
					.WithSubquery.WhereProperty(p => p.Age).GeAll(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).GtAll(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).LeAll(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).LtAll(DetachedQueryOverAge);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertySome()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyGeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereProperty(p => p.Age).GeSome(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).GtSome(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).LeSome(DetachedQueryOverAge)
					.WithSubquery.WhereProperty(p => p.Age).LtSome(DetachedQueryOverAge);

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
		public void PropertyAsSyntaxAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Subqueries.PropertyEq("personAlias.Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGtSome("personAlias.Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtAll("personAlias.Age", DetachedCriteriaAge));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.WithSubquery.Where(() => personAlias.Name == DetachedQueryOverName.As<string>())
					.WithSubquery.WhereSome(() => personAlias.Age > DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereAll(() => personAlias.Age < DetachedQueryOverAge.As<int>());

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyAsAllSyntax()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEqAll("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtAll("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereAll(p => p.Name == DetachedQueryOverName.As<string>())
					.WithSubquery.WhereAll(p => p.Age >= DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereAll(p => p.Age > DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereAll(p => p.Age <= DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereAll(p => p.Age < DetachedQueryOverAge.As<int>());

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyAsSomeSyntax()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyGeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereSome(p => p.Age >= DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereSome(p => p.Age > DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereSome(p => p.Age <= DetachedQueryOverAge.As<int>())
					.WithSubquery.WhereSome(p => p.Age < DetachedQueryOverAge.As<int>());

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

		[Test]
		public void ValueAll()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.EqAll("Name", DetachedCriteriaName))
					.Add(Subqueries.GeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.GtAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.LeAll("Age", DetachedCriteriaAge))
					.Add(Subqueries.LtAll("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereValue("Name").EqAll(DetachedQueryOverName)
					.WithSubquery.WhereValue("Age").GeAll(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").GtAll(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").LeAll(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").LtAll(DetachedQueryOverAge);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void ValueSome()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.GeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.GtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.LeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.LtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereValue("Age").GeSome(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").GtSome(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").LeSome(DetachedQueryOverAge)
					.WithSubquery.WhereValue("Age").LtSome(DetachedQueryOverAge);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void UntypedSubqueries()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.Exists(DetachedCriteriaChild))
					.Add(Subqueries.NotExists(DetachedCriteriaChild));

			var actual =
				CreateTestQueryOver<Person>()
					.WithSubquery.WhereExists(DetachedQueryOverChild)
					.WithSubquery.WhereNotExists(DetachedQueryOverChild);

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}