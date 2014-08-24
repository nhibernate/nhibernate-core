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
					.Add(Restrictions.Eq("class", typeof(Person)))
					.Add(Restrictions.Eq("class", typeof(Person).FullName));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Name == "test name")
					.And(p => p.Name != "not test name")
					.And(p => p.Age > 10)
					.And(p => p.Age >= 11)
					.And(p => p.Age < 50)
					.And(p => p.Age <= 49)
					.And(p => p.GetType() == typeof(Person))
					.And(p => p is Person);

			AssertCriteriaAreEqual(expected, actual);
		}

		public static int CompareString(string left, string right, bool textCompare)
		{
			// could consider calling Microsoft.VisualBasic.CompilerServices.Operators.CompareString
			throw new Exception("This is just here to allow us to simulate the VB.Net LINQ expression tree");
		}

		[Test]
		public void VisualBasicStringComparison()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq("Name", "test name"))
					.Add(Restrictions.Not(Restrictions.Eq("Name", "test name")))
					.Add(Restrictions.Gt("Name", "test name"))
					.Add(Restrictions.Ge("Name", "test name"))
					.Add(Restrictions.Lt("Name", "test name"))
					.Add(Restrictions.Le("Name", "test name"))
					.Add(Restrictions.EqProperty("Name", "Name"))
					.Add(Restrictions.Not(Restrictions.EqProperty("Name", "Name")))
					.Add(Restrictions.GtProperty("Name", "Name"))
					.Add(Restrictions.GeProperty("Name", "Name"))
					.Add(Restrictions.LtProperty("Name", "Name"))
					.Add(Restrictions.LeProperty("Name", "Name"));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => CompareString(p.Name, "test name", true) == 0)
					.And(p => CompareString(p.Name, "test name", true) != 0)
					.And(p => CompareString(p.Name, "test name", true) > 0)
					.And(p => CompareString(p.Name, "test name", true) >= 0)
					.And(p => CompareString(p.Name, "test name", true) < 0)
					.And(p => CompareString(p.Name, "test name", true) <= 0)
					.And(p => CompareString(p.Name, p.Name, true) == 0)
					.And(p => CompareString(p.Name, p.Name, true) != 0)
					.And(p => CompareString(p.Name, p.Name, true) > 0)
					.And(p => CompareString(p.Name, p.Name, true) >= 0)
					.And(p => CompareString(p.Name, p.Name, true) < 0)
					.And(p => CompareString(p.Name, p.Name, true) <= 0);

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
		public void SimpleCriterion_Char()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.Add(Restrictions.Eq("Blood", 'A'))
					.Add(Restrictions.Not(Restrictions.Eq("Blood", 'B')));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.And(p => p.Blood == 'A')
					.And(p => p.Blood != 'B');

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
		public void CustomMethodExpression()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Or(
						Restrictions.Not(Restrictions.Eq("Name", "test name")),
						Restrictions.Not(Restrictions.Like("personAlias.Name", "%test%"))));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(p => !(p.Name == "test name") || !personAlias.Name.IsLike("%test%"));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Negation()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.Add(Restrictions.Not(Restrictions.Eq("Name", "test name")))
					.Add(Restrictions.Not(Restrictions.Eq("personAlias.Name", "test name")));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.AndNot(p => p.Name == "test name")
					.AndNot(() => personAlias.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Where_BehavesTheSameAs_And()
		{
			Person personAlias = null;
			QueryOver<Person> expected = (QueryOver<Person>)
				CreateTestQueryOver<Person>(() => personAlias)
					.And(() => personAlias.Name == "test name")
					.And(p => p.Name == "test name")
					.AndNot(() => personAlias.Name == "test name")
					.AndNot(p => p.Name == "test name");

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name")
					.Where(p => p.Name == "test name")
					.WhereNot(() => personAlias.Name == "test name")
					.WhereNot(p => p.Name == "test name");

			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual);
		}

		[Test]
		public void PrivateProperties()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetProjection(Projections.Property("Name"))
					.Add(Restrictions.Eq("Name", "test name"))
					.Add(Restrictions.Not(Restrictions.Eq("Name", "not test name")))
					.AddOrder(Order.Desc(Projections.Property("Name")));

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Select(Projections.Property("Name"))
					.Where(Restrictions.Eq("Name", "test name"))
					.And(Restrictions.Not(Restrictions.Eq("Name", "not test name")))
					.OrderBy(Projections.Property("Name")).Desc;

			AssertCriteriaAreEqual(expected, actual);
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
					.Add(Restrictions.Eq("personAlias.class", typeof(Person)))
					.Add(Restrictions.Eq("personAlias.class", typeof(Person).FullName));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name")
					.And(() => personAlias.Name != "not test name")
					.And(() => personAlias.Age > 10)
					.And(() => personAlias.Age >= 11)
					.And(() => personAlias.Age < 50)
					.And(() => personAlias.Age <= 49)
					.And(() => personAlias.GetType() == typeof(Person))
					.And(() => personAlias is Person);

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
		public void SubCriteria_JoinQueryOver_ToOneAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.CreateCriteria("personAlias.Father")
						.Add(Expression.Eq("Name", "test name"));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.JoinQueryOver(() => personAlias.Father) // sub-criteria
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

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.JoinQueryOver(p => p.Children) // sub-criteria
						.Where(c => c.Nickname == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void SubCriteria_JoinQueryOver_ToManyAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.CreateCriteria("personAlias.Children", JoinType.InnerJoin)
						.Add(Expression.Eq("Nickname", "test name"));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Inner.JoinQueryOver(() => personAlias.Children) // sub-criteria
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
			IQueryOver<Relation> actual =
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
					.JoinAlias(p => p.Father, () => fatherAlias)
					.JoinAlias(p => p.Children, () => childAlias);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Alias_JoinAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.CreateAlias("personAlias.Father", "fatherAlias")
					.CreateAlias("personAlias.Children", "childAlias", JoinType.InnerJoin);

			Person personAlias = null;
			Person fatherAlias = null;
			Child childAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.JoinAlias(() => personAlias.Father, () => fatherAlias)
					.Inner.JoinAlias(() => personAlias.Children, () => childAlias);

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
					.Inner.JoinAlias(r => r.Related1, () => related1Alias)
					.Inner.JoinAlias(r => r.Collection1, () => collection1Alias)
					.Left.JoinAlias(r => r.Related2, () => related2Alias)
					.Left.JoinAlias(r => r.Collection2, () => collection2Alias)
					.Right.JoinAlias(r => r.Related3, () => related3Alias)
					.Right.JoinAlias(r => r.Collection3, () => collection3Alias)
					.Full.JoinAlias(r => r.Related4, () => related4Alias)
					.Full.JoinAlias(r => r.Collection4, () => collection4Alias);

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OnClause_SubCriteria()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateCriteria("PersonList", "alias1", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many func t,bool"))
					.CreateCriteria("PersonList", "alias2", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "many func bool"))
					.CreateCriteria("PersonList", "alias3", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many private"))
					.CreateCriteria("Father", "alias4", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one func t,bool"))
					.CreateCriteria("Father", "alias5", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "one func bool"))
					.CreateCriteria("Father", "alias6", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one private"))
					.CreateCriteria("alias1.PersonList", "alias7", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many func t,bool"))
					.CreateCriteria("alias2.PersonList", "alias8", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "a many func bool"))
					.CreateCriteria("alias3.PersonList", "alias9", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many private"))
					.CreateCriteria("alias4.Father", "alias10", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one func t,bool"))
					.CreateCriteria("alias5.Father", "alias11", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "a one func bool"))
					.CreateCriteria("alias6.Father", "alias12", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one private"));

			Person alias1 = null;
			Person alias2 = null;
			Person alias3 = null;
			Person alias4 = null;
			Person alias5 = null;
			Person alias6 = null;
			Person alias7 = null;
			Person alias8 = null;
			Person alias9 = null;
			Person alias10 = null;
			Person alias11 = null;
			Person alias12 = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Left.JoinQueryOver(p => p.PersonList, () => alias1, p => p.Name == "many func t,bool")
					.Left.JoinQueryOver(p => p.PersonList, () => alias2, () => alias1.Name == "many func bool")
					.Left.JoinQueryOver(p => p.PersonList, () => alias3, Restrictions.Eq("Name", "many private"))
					.Left.JoinQueryOver(p => p.Father, () => alias4, p => p.Name == "one func t,bool")
					.Left.JoinQueryOver(p => p.Father, () => alias5, () => alias4.Name == "one func bool")
					.Left.JoinQueryOver(p => p.Father, () => alias6, Restrictions.Eq("Name", "one private"))
					.Left.JoinQueryOver(() => alias1.PersonList, () => alias7, p => p.Name == "a many func t,bool")
					.Left.JoinQueryOver(() => alias2.PersonList, () => alias8, () => alias1.Name == "a many func bool")
					.Left.JoinQueryOver(() => alias3.PersonList, () => alias9, Restrictions.Eq("Name", "a many private"))
					.Left.JoinQueryOver(() => alias4.Father, () => alias10, p => p.Name == "a one func t,bool")
					.Left.JoinQueryOver(() => alias5.Father, () => alias11, () => alias4.Name == "a one func bool")
					.Left.JoinQueryOver(() => alias6.Father, () => alias12, Restrictions.Eq("Name", "a one private"));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OnClauseDetached_SubCriteria()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.CreateCriteria("PersonList", "alias1", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many func t,bool"))
					.CreateCriteria("PersonList", "alias2", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "many func bool"))
					.CreateCriteria("PersonList", "alias3", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many private"))
					.CreateCriteria("Father", "alias4", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one func t,bool"))
					.CreateCriteria("Father", "alias5", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "one func bool"))
					.CreateCriteria("Father", "alias6", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one private"))
					.CreateCriteria("alias1.PersonList", "alias7", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many func t,bool"))
					.CreateCriteria("alias2.PersonList", "alias8", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "a many func bool"))
					.CreateCriteria("alias3.PersonList", "alias9", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many private"))
					.CreateCriteria("alias4.Father", "alias10", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one func t,bool"))
					.CreateCriteria("alias5.Father", "alias11", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "a one func bool"))
					.CreateCriteria("alias6.Father", "alias12", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one private"));

			Person alias1 = null;
			Person alias2 = null;
			Person alias3 = null;
			Person alias4 = null;
			Person alias5 = null;
			Person alias6 = null;
			Person alias7 = null;
			Person alias8 = null;
			Person alias9 = null;
			Person alias10 = null;
			Person alias11 = null;
			Person alias12 = null;
			QueryOver<Person> actual =
				QueryOver.Of<Person>()
					.Left.JoinQueryOver(p => p.PersonList, () => alias1, p => p.Name == "many func t,bool")
					.Left.JoinQueryOver(p => p.PersonList, () => alias2, () => alias1.Name == "many func bool")
					.Left.JoinQueryOver(p => p.PersonList, () => alias3, Restrictions.Eq("Name", "many private"))
					.Left.JoinQueryOver(p => p.Father, () => alias4, p => p.Name == "one func t,bool")
					.Left.JoinQueryOver(p => p.Father, () => alias5, () => alias4.Name == "one func bool")
					.Left.JoinQueryOver(p => p.Father, () => alias6, Restrictions.Eq("Name", "one private"))
					.Left.JoinQueryOver(() => alias1.PersonList, () => alias7, p => p.Name == "a many func t,bool")
					.Left.JoinQueryOver(() => alias2.PersonList, () => alias8, () => alias1.Name == "a many func bool")
					.Left.JoinQueryOver(() => alias3.PersonList, () => alias9, Restrictions.Eq("Name", "a many private"))
					.Left.JoinQueryOver(() => alias4.Father, () => alias10, p => p.Name == "a one func t,bool")
					.Left.JoinQueryOver(() => alias5.Father, () => alias11, () => alias4.Name == "a one func bool")
					.Left.JoinQueryOver(() => alias6.Father, () => alias12, Restrictions.Eq("Name", "a one private"));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OnClause_Alias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.CreateAlias("PersonList", "alias1", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many func t,bool"))
					.CreateAlias("PersonList", "alias2", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "many func bool"))
					.CreateAlias("PersonList", "alias3", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many private"))
					.CreateAlias("Father", "alias4", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one func t,bool"))
					.CreateAlias("Father", "alias5", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "one func bool"))
					.CreateAlias("Father", "alias6", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one private"))
					.CreateAlias("alias1.PersonList", "alias7", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many func t,bool"))
					.CreateAlias("alias2.PersonList", "alias8", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "a many func bool"))
					.CreateAlias("alias3.PersonList", "alias9", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many private"))
					.CreateAlias("alias4.Father", "alias10", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one func t,bool"))
					.CreateAlias("alias5.Father", "alias11", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "a one func bool"))
					.CreateAlias("alias6.Father", "alias12", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one private"));

			Person alias1 = null;
			Person alias2 = null;
			Person alias3 = null;
			Person alias4 = null;
			Person alias5 = null;
			Person alias6 = null;
			Person alias7 = null;
			Person alias8 = null;
			Person alias9 = null;
			Person alias10 = null;
			Person alias11 = null;
			Person alias12 = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Left.JoinAlias(p => p.PersonList, () => alias1, p => p.Name == "many func t,bool")
					.Left.JoinAlias(p => p.PersonList, () => alias2, () => alias1.Name == "many func bool")
					.Left.JoinAlias(p => p.PersonList, () => alias3, Restrictions.Eq("Name", "many private"))
					.Left.JoinAlias(p => p.Father, () => alias4, p => p.Name == "one func t,bool")
					.Left.JoinAlias(p => p.Father, () => alias5, () => alias4.Name == "one func bool")
					.Left.JoinAlias(p => p.Father, () => alias6, Restrictions.Eq("Name", "one private"))
					.Left.JoinAlias(() => alias1.PersonList, () => alias7, p => p.Name == "a many func t,bool")
					.Left.JoinAlias(() => alias2.PersonList, () => alias8, () => alias1.Name == "a many func bool")
					.Left.JoinAlias(() => alias3.PersonList, () => alias9, Restrictions.Eq("Name", "a many private"))
					.Left.JoinAlias(() => alias4.Father, () => alias10, p => p.Name == "a one func t,bool")
					.Left.JoinAlias(() => alias5.Father, () => alias11, () => alias4.Name == "a one func bool")
					.Left.JoinAlias(() => alias6.Father, () => alias12, Restrictions.Eq("Name", "a one private"));

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OnClauseDetached_Alias()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>()
					.CreateAlias("PersonList", "alias1", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many func t,bool"))
					.CreateAlias("PersonList", "alias2", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "many func bool"))
					.CreateAlias("PersonList", "alias3", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "many private"))
					.CreateAlias("Father", "alias4", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one func t,bool"))
					.CreateAlias("Father", "alias5", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "one func bool"))
					.CreateAlias("Father", "alias6", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "one private"))
					.CreateAlias("alias1.PersonList", "alias7", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many func t,bool"))
					.CreateAlias("alias2.PersonList", "alias8", JoinType.LeftOuterJoin, Restrictions.Eq("alias1.Name", "a many func bool"))
					.CreateAlias("alias3.PersonList", "alias9", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a many private"))
					.CreateAlias("alias4.Father", "alias10", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one func t,bool"))
					.CreateAlias("alias5.Father", "alias11", JoinType.LeftOuterJoin, Restrictions.Eq("alias4.Name", "a one func bool"))
					.CreateAlias("alias6.Father", "alias12", JoinType.LeftOuterJoin, Restrictions.Eq("Name", "a one private"));

			Person alias1 = null;
			Person alias2 = null;
			Person alias3 = null;
			Person alias4 = null;
			Person alias5 = null;
			Person alias6 = null;
			Person alias7 = null;
			Person alias8 = null;
			Person alias9 = null;
			Person alias10 = null;
			Person alias11 = null;
			Person alias12 = null;
			QueryOver<Person> actual =
				QueryOver.Of<Person>()
					.Left.JoinAlias(p => p.PersonList, () => alias1, p => p.Name == "many func t,bool")
					.Left.JoinAlias(p => p.PersonList, () => alias2, () => alias1.Name == "many func bool")
					.Left.JoinAlias(p => p.PersonList, () => alias3, Restrictions.Eq("Name", "many private"))
					.Left.JoinAlias(p => p.Father, () => alias4, p => p.Name == "one func t,bool")
					.Left.JoinAlias(p => p.Father, () => alias5, () => alias4.Name == "one func bool")
					.Left.JoinAlias(p => p.Father, () => alias6, Restrictions.Eq("Name", "one private"))
					.Left.JoinAlias(() => alias1.PersonList, () => alias7, p => p.Name == "a many func t,bool")
					.Left.JoinAlias(() => alias2.PersonList, () => alias8, () => alias1.Name == "a many func bool")
					.Left.JoinAlias(() => alias3.PersonList, () => alias9, Restrictions.Eq("Name", "a many private"))
					.Left.JoinAlias(() => alias4.Father, () => alias10, p => p.Name == "a one func t,bool")
					.Left.JoinAlias(() => alias5.Father, () => alias11, () => alias4.Name == "a one func bool")
					.Left.JoinAlias(() => alias6.Father, () => alias12, Restrictions.Eq("Name", "a one private"));

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
					.AddOrder(Order.Asc("personAlias.Age"))
					.AddOrder(Order.Asc("summary"))
					.AddOrder(Order.Desc("Count"));

			Person personAlias = null;
			PersonSummary summary = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.OrderBy(p => p.Name).Asc
					.ThenBy(p => p.Age).Desc
					.ThenBy(() => personAlias.Name).Desc
					.ThenBy(() => personAlias.Age).Asc
					.OrderByAlias(() => summary).Asc
					.ThenByAlias(() => summary.Count).Desc;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OrderByYearPartFunction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.AddOrder(Order.Desc(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("personAlias.BirthDate"))));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.OrderBy(() => personAlias.BirthDate.YearPart()).Desc;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OrderByYearFunction()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.AddOrder(Order.Desc(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("personAlias.BirthDate"))));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.OrderBy(() => personAlias.BirthDate.Year).Desc;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void OrderByFunctionOfDateTimeOffset()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.AddOrder(Order.Desc(Projections.SqlFunction("year", NHibernateUtil.Int32, Projections.Property("personAlias.BirthDateAsDateTimeOffset"))));

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.OrderBy(() => personAlias.BirthDateAsDateTimeOffset.Year).Desc;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void AllowSingleCallSyntax()
		{
			ICriteria expected = CreateTestCriteria(typeof(Person));
			expected.Add(Restrictions.IsNotEmpty("Children"));
			expected.AddOrder(Order.Asc("Name"));
			expected.SetFetchMode("PersonList", FetchMode.Eager);
			expected.SetLockMode(LockMode.UpgradeNoWait);

			IQueryOver<Person,Person> actual = CreateTestQueryOver<Person>();
			actual.WhereRestrictionOn(p => p.Children).IsNotEmpty();
			actual.OrderBy(p => p.Name).Asc();
			actual.Fetch(p => p.PersonList).Eager();
			actual.Lock().UpgradeNoWait();

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

		[Test]
		public void Fetch()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetFetchMode("PersonList", FetchMode.Eager)
					.SetFetchMode("PersonList.PersonList", FetchMode.Lazy);

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Fetch(p => p.PersonList).Eager
					.Fetch(p => p.PersonList[0].PersonList).Lazy;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void LockAll()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetLockMode(LockMode.UpgradeNoWait);

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Lock().UpgradeNoWait;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void ResultTransformer()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetResultTransformer(Transformers.AliasToBean<Person>());

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.TransformUsing(Transformers.AliasToBean<Person>());

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void LockAlias()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person), "personAlias")
					.SetLockMode("personAlias", LockMode.UpgradeNoWait);

			Person personAlias = null;
			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>(() => personAlias)
					.Lock(() => personAlias).UpgradeNoWait;

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void Readonly()
		{
			ICriteria expected =
				CreateTestCriteria(typeof(Person))
					.SetReadOnly(true);

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.ReadOnly();

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void DetachedQueryOver()
		{
			DetachedCriteria expected =
				DetachedCriteria.For<Person>("personAlias")
					.Add(Restrictions.Eq("personAlias.Name", "test name"));

			Person personAlias = null;
			QueryOver<Person> actual =
				QueryOver.Of<Person>(() => personAlias)
					.Where(() => personAlias.Name == "test name");

			AssertCriteriaAreEqual(expected, actual);
		}

		[Test]
		public void CloneIQueryOver()
		{
			IQueryOver<Person> expected =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test")
					.Select(p => p.Name);

			IQueryOver<Person> actual = expected.Clone();

			Assert.That(actual, Is.Not.SameAs(expected));
			Assert.That(actual.UnderlyingCriteria, Is.Not.SameAs(expected.UnderlyingCriteria));
			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual.UnderlyingCriteria);
		}

		[Test]
		public void CloneIQueryOverWithSubType()
		{
			IQueryOver<Person,Child> expected =
				CreateTestQueryOver<Person>()
					.JoinQueryOver(p => p.Children);

			IQueryOver<Person,Person> actual = expected.Clone();

			ICriteria expectedCriteria = expected.UnderlyingCriteria.GetCriteriaByAlias("this");

			AssertCriteriaAreEqual(expectedCriteria, actual);
		}

		[Test]
		public void CloneQueryOver()
		{
			QueryOver<Person> expected =
				QueryOver.Of<Person>()
					.Where(p => p.Name == "test")
					.Select(p => p.Name);

			QueryOver<Person> actual = expected.Clone();

			Assert.That(actual, Is.Not.SameAs(expected));
			Assert.That(actual.UnderlyingCriteria, Is.Not.SameAs(expected.UnderlyingCriteria));
			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual.UnderlyingCriteria);
		}

		[Test]
		public void TransformQueryOverToRowCount()
		{
			IQueryOver<Person> expected =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test")
					.JoinQueryOver(p => p.Children)
						.Where((Child c) => c.Age == 5)
						.Select(Projections.RowCount());

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test")
					.JoinQueryOver(p => p.Children)
						.Where((Child c) => c.Age == 5)
						.OrderBy(c => c.Age).Asc
						.Skip(20)
						.Take(10);

			expected = expected.Clone();
			actual = actual.ToRowCountQuery();

			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual);
		}

		[Test]
		public void TransformQueryOverToRowCount64()
		{
			IQueryOver<Person> expected =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test")
					.JoinQueryOver(p => p.Children)
						.Where((Child c) => c.Age == 5)
						.Select(Projections.RowCountInt64());

			IQueryOver<Person> actual =
				CreateTestQueryOver<Person>()
					.Where(p => p.Name == "test")
					.JoinQueryOver(p => p.Children)
						.Where((Child c) => c.Age == 5)
						.OrderBy(c => c.Age).Asc
						.Skip(20)
						.Take(10);

			expected = expected.Clone();
			actual = actual.ToRowCountInt64Query();

			AssertCriteriaAreEqual(expected.UnderlyingCriteria, actual);
		}

	}

}