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
	public class QueryOverFixture : LambdaFixtureBase
	{

		[Test]
		public void SimpleCriterion_NoAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq("Name", "test name"))
					.Add(Restrictions.Not(Restrictions.Eq("Name", "not test name")))
					.Add(Restrictions.Gt("Age", 10))
					.Add(Restrictions.Ge("Age", 11))
					.Add(Restrictions.Lt("Age", 50))
					.Add(Restrictions.Le("Age", 49))
					.Add(Restrictions.Eq("class", typeof(Person)));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Name == "test name")
					.And(p => p.Name != "not test name")
					.And(p => p.Age > 10)
					.And(p => p.Age >= 11)
					.And(p => p.Age < 50)
					.And(p => p.Age <= 49)
					.And(p => p.GetType() == typeof(Person));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void PropertyCriterion_NoAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.EqProperty("Age", "Height"))
					.Add(Restrictions.NotEqProperty("Age", "Height"))
					.Add(Restrictions.GtProperty("Age", "Height"))
					.Add(Restrictions.GeProperty("Age", "Height"))
					.Add(Restrictions.LtProperty("Age", "Height"))
					.Add(Restrictions.LeProperty("Age", "Height"));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Age == p.Height)
					.And(p => p.Age != p.Height)
					.And(p => p.Age > p.Height)
					.And(p => p.Age >= p.Height)
					.And(p => p.Age < p.Height)
					.And(p => p.Age <= p.Height);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void MultipleCriterionExpression()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.And(
						Restrictions.Eq("Name", "test name"),
						Restrictions.Or(
							Restrictions.Gt("Age", 21),
							Restrictions.Eq("HasCar", true))));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test name" && (p.Age > 21 || p.HasCar));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Where_BehavesTheSameAs_And()
		{
			Person personAlias = null;
			QueryOver<Person> expected = (QueryOver<Person>)
				CreateTestQueryOver<Person>(() => personAlias)
					.And(() => personAlias.Name == "test name")
					.And(p => p.Name == "test name");

			IQueryOver<Person> actual =
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
					.Add(Restrictions.Eq("personAlias.Name", "test name"))
					.Add(Restrictions.Not(Restrictions.Eq("personAlias.Name", "not test name")))
					.Add(Restrictions.Gt("personAlias.Age", 10))
					.Add(Restrictions.Ge("personAlias.Age", 11))
					.Add(Restrictions.Lt("personAlias.Age", 50))
					.Add(Restrictions.Le("personAlias.Age", 49))
					.Add(Restrictions.Eq("personAlias.class", typeof(Person)));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name")
					.And(() => personAlias.Name != "not test name")
					.And(() => personAlias.Age > 10)
					.And(() => personAlias.Age >= 11)
					.And(() => personAlias.Age < 50)
					.And(() => personAlias.Age <= 49)
					.And(() => personAlias.GetType() == typeof(Person));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinQueryOver_ToOne()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Father")
						.Add(Expression.Eq("Name", "test name"));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.JoinQueryOver(p => p.Father) // sub-criteria
						.Where(f => f.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinQueryOver_ToMany()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("Children")
						.Add(Expression.Eq("Nickname", "test name"));

			IQueryOver<Child> actual =
				CreateTestQueryOver<Person>()
					.JoinQueryOver(p => p.Children) // sub-criteria
						.Where(c => c.Nickname == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinQueryOverCombinations()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Relation))
					.CreateCriteria("Related1")
					.CreateCriteria("Related2", JoinType.LeftOuterJoin)
					.CreateCriteria("Related3", JoinType.RightOuterJoin)
					.CreateCriteria("Related4", JoinType.FullJoin)
					.CreateCriteria("Collection1", "collection1Alias")
					.CreateCriteria("Collection2", "collection2Alias", JoinType.LeftOuterJoin)
					.CreateCriteria("Collection3", JoinType.RightOuterJoin)
					.CreateCriteria("People", "personAlias", JoinType.FullJoin);

			Relation collection1Alias = null, collection2Alias = null;
			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Relation>()
					.Inner.JoinQueryOver(r => r.Related1)
					.Left.JoinQueryOver(r => r.Related2)
					.Right.JoinQueryOver(r => r.Related3)
					.Full.JoinQueryOver(r => r.Related4)
					.JoinQueryOver(r => r.Collection1, () => collection1Alias)
					.Left.JoinQueryOver(r => r.Collection2, () => collection2Alias)
					.Right.JoinQueryOver(r => r.Collection3)
					.Full.JoinQueryOver(r => r.People, () => personAlias);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Alias_Join()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("Father", "fatherAlias")
					.CreateAlias("Children", "childAlias");

			Person fatherAlias = null;
			Child childAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Join(p => p.Father, () => fatherAlias)
					.Join(p => p.Children, () => childAlias);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Alias_JoinCombinations()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Relation))
					.CreateAlias("Related1", "related1Alias")
					.CreateAlias("Collection1", "collection1Alias")
					.CreateAlias("Related2", "related2Alias", JoinType.LeftOuterJoin)
					.CreateAlias("Collection2", "collection2Alias", JoinType.LeftOuterJoin)
					.CreateAlias("Related3", "related3Alias", JoinType.RightOuterJoin)
					.CreateAlias("Collection3", "collection3Alias", JoinType.RightOuterJoin)
					.CreateAlias("Related4", "related4Alias", JoinType.FullJoin)
					.CreateAlias("Collection4", "collection4Alias", JoinType.FullJoin);

			Relation related1Alias = null, related2Alias = null, related3Alias = null, related4Alias = null;
			Relation collection1Alias = null, collection2Alias = null, collection3Alias = null, collection4Alias = null;
			IQueryOver<Relation> actual =
				CreateTestQueryOver<Relation>()
					.Inner.Join(r => r.Related1, () => related1Alias)
					.Inner.Join(r => r.Collection1, () => collection1Alias)
					.Left.Join(r => r.Related2, () => related2Alias)
					.Left.Join(r => r.Collection2, () => collection2Alias)
					.Right.Join(r => r.Related3, () => related3Alias)
					.Right.Join(r => r.Collection3, () => collection3Alias)
					.Full.Join(r => r.Related4, () => related4Alias)
					.Full.Join(r => r.Collection4, () => collection4Alias);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OrderBy()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.AddOrder(Order.Asc("Name"))
					.AddOrder(Order.Desc("Age"))
					.AddOrder(Order.Desc("personAlias.Name"))
					.AddOrder(Order.Asc("personAlias.Age"));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.OrderBy(p => p.Name).Asc
					.ThenBy(p => p.Age).Desc
					.ThenBy(() => personAlias.Name).Desc
					.ThenBy(() => personAlias.Age).Asc;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Project()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetProjection(
						Projections.Property("Name"),
						Projections.Property("Age"),
						Projections.Property("personAlias.Gender"),
						Projections.Property("personAlias.HasCar"));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Select(p => p.Name,
							p => p.Age,
							p => personAlias.Gender,
							p => personAlias.HasCar);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Paging()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetFirstResult(90)
					.SetMaxResults(10);

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Skip(90)
					.Take(10);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Cachable()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Put)
					.SetCacheRegion("my cache region");

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Cacheable()
					.CacheMode(CacheMode.Put)
					.CacheRegion("my cache region");

			AssertCriteriaAreEqual(expected, actual);
		}

	}

}