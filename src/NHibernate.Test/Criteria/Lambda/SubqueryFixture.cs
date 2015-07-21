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
					QueryOver.Of<Child>(() => _subqueryChildAlias)
						.Where(() => _subqueryChildAlias.Nickname == "subquery name");
			}
		}

		private QueryOver<Child> DetachedQueryOverName
		{
			get
			{
				return
					QueryOver.Of<Child>(() => _subqueryChildAlias)
						.Where(() => _subqueryChildAlias.Nickname == "subquery name")
						.Select(p => p.Nickname);
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
		public void DetachedSubquery()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>("personAlias")
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyEq("personAlias.Name", DetachedCriteriaName));

			Person personAlias = null;
			QueryOver<Person> actual =
				QueryOver.Of<Person>(() => personAlias)
					.WithSubquery.WhereProperty(p => p.Name).Eq(DetachedQueryOverName)
					.WithSubquery.WhereProperty(() => personAlias.Name).Eq(DetachedQueryOverName);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyCriterion()
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
					.And(Subqueries.WhereProperty<Person>(p => p.Name).Eq(DetachedQueryOverName))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).Ge(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).Gt(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Name).In(DetachedQueryOverName))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).Le(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).Lt(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Name).Ne(DetachedQueryOverName))
					.And(Subqueries.WhereProperty<Person>(p => p.Name).NotIn(DetachedQueryOverName));

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
		public void PropertyAliasCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Subqueries.PropertyEq("personAlias.Name", DetachedCriteriaName));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.And(Subqueries.WhereProperty(() => personAlias.Name).Eq(DetachedQueryOverName));

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
		public void PropertyAllCriterion()
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
					.And(Subqueries.WhereProperty<Person>(p => p.Name).EqAll(DetachedQueryOverName))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).GeAll(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).GtAll(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).LeAll(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).LtAll(DetachedQueryOverAge));

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
		public void PropertySomeCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyGeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.And(Subqueries.WhereProperty<Person>(p => p.Age).GeSome(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).GtSome(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).LeSome(DetachedQueryOverAge))
					.And(Subqueries.WhereProperty<Person>(p => p.Age).LtSome(DetachedQueryOverAge));

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
		public void PropertyAsSyntaxCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyEq("Name", DetachedCriteriaName));

			var actual =
				CreateTestQueryOver<Person>()
					.And(Subqueries.Where<Person>(p => p.Name == DetachedQueryOverName.As<string>()));

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
		public void PropertyAsSyntaxAliasCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Subqueries.PropertyEq("personAlias.Name", DetachedCriteriaName))
					.Add(Subqueries.PropertyGtSome("personAlias.Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtAll("personAlias.Age", DetachedCriteriaAge));

			Person personAlias = null;
			var actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.And(Subqueries.Where(() => personAlias.Name == DetachedQueryOverName.As<string>()))
					.And(Subqueries.WhereSome(() => personAlias.Age > DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereAll(() => personAlias.Age < DetachedQueryOverAge.As<int>()));

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
		public void PropertyAsAllSyntaxCriterion()
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
					.And(Subqueries.WhereAll<Person>(p => p.Name == DetachedQueryOverName.As<string>()))
					.And(Subqueries.WhereAll<Person>(p => p.Age >= DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereAll<Person>(p => p.Age > DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereAll<Person>(p => p.Age <= DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereAll<Person>(p => p.Age < DetachedQueryOverAge.As<int>()));

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
		public void PropertyAsSomeSyntaxCrtierion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.PropertyGeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyGtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.PropertyLtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.And(Subqueries.WhereSome<Person>(p => p.Age >= DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereSome<Person>(p => p.Age > DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereSome<Person>(p => p.Age <= DetachedQueryOverAge.As<int>()))
					.And(Subqueries.WhereSome<Person>(p => p.Age < DetachedQueryOverAge.As<int>()));

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
		public void ValueCriterion()
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
					.And(Subqueries.WhereValue("Name").Eq(DetachedQueryOverName))
					.And(Subqueries.WhereValue("Age").Ge(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").Gt(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Name").In(DetachedQueryOverName))
					.And(Subqueries.WhereValue("Age").Le(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").Lt(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Name").Ne(DetachedQueryOverName))
					.And(Subqueries.WhereValue("Name").NotIn(DetachedQueryOverName));

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
		public void ValueAllCriterion()
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
					.And(Subqueries.WhereValue("Name").EqAll(DetachedQueryOverName))
					.And(Subqueries.WhereValue("Age").GeAll(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").GtAll(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").LeAll(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").LtAll(DetachedQueryOverAge));

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
		public void ValueSomeCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.GeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.GtSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.LeSome("Age", DetachedCriteriaAge))
					.Add(Subqueries.LtSome("Age", DetachedCriteriaAge));

			var actual =
				CreateTestQueryOver<Person>()
					.And(Subqueries.WhereValue("Age").GeSome(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").GtSome(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").LeSome(DetachedQueryOverAge))
					.And(Subqueries.WhereValue("Age").LtSome(DetachedQueryOverAge));

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

		[Test]
		public void UntypedSubqueriesCriterion()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Subqueries.Exists(DetachedCriteriaChild))
					.Add(Subqueries.NotExists(DetachedCriteriaChild));

			var actual =
				CreateTestQueryOver<Person>()
					.And(Subqueries.WhereExists(DetachedQueryOverChild))
					.And(Subqueries.WhereNotExists(DetachedQueryOverChild));

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}