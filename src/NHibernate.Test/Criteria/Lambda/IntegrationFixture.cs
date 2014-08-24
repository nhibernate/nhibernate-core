using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using SharpTestsEx;

using NHibernate.Criterion;

namespace NHibernate.Test.Criteria.Lambda
{
	[TestFixture]
	public class IntegrationFixture : TestCase
	{
		protected override string MappingsAssembly { get { return "NHibernate.Test"; } }

		protected override IList Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Child").ExecuteUpdate();
				s.CreateQuery("update Person p set p.Father = null").ExecuteUpdate();
				s.CreateQuery("delete from Person").ExecuteUpdate();
				s.CreateQuery("delete from JoinedChild").ExecuteUpdate();
				s.CreateQuery("delete from Parent").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void DetachedQuery_SimpleCriterion()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				QueryOver<Person> personQuery =
					QueryOver.Of<Person>()
						.Where(p => p.Name == "test person 1");

				IList<Person> actual =
					personQuery.GetExecutableQueryOver(s)
						.List();

				Assert.That(actual[0].Age, Is.EqualTo(20));
			}
		}

		[Test]
		public void FilterNullComponent()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var p1 = new Person() { Detail = new PersonDetail() { MaidenName = "test", Anniversary = new DateTime(2007, 06, 05) } };
				var p2 = new Person() { Detail = null };

				s.Save(p1);
				s.Save(p2);

				var nullDetails =
					s.QueryOver<Person>()
						.Where(p => p.Detail == null)
						.List();

				Assert.That(nullDetails.Count, Is.EqualTo(1));
				Assert.That(nullDetails[0].Id, Is.EqualTo(p2.Id));
			}
		}

		[Test]
		public void OnClause()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "John" }
						.AddChild(new Child() { Nickname = "John"})
						.AddChild(new Child() { Nickname = "Judy"}));

				s.Save(new Person() { Name = "Jean" });
				s.Save(new Child() { Nickname = "James" });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				Child childAlias = null;
				Person parentAlias = null;
				var children =
					s.QueryOver(() => childAlias)
						.Left.JoinQueryOver(c => c.Parent, () => parentAlias, p => p.Name == childAlias.Nickname)
							.WhereRestrictionOn(p => p.Name).IsNotNull
							.List();

				children.Should().Have.Count.EqualTo(1);
			}

			using (ISession s = OpenSession())
			{
				Child childAlias = null;
				Person parentAlias = null;
				var parentNames =
					s.QueryOver<Child>(() => childAlias)
						.Left.JoinAlias(c => c.Parent, () => parentAlias, p => p.Name == childAlias.Nickname)
						.Select(c => parentAlias.Name)
						.List<string>();

				parentNames
					.Where(n => !string.IsNullOrEmpty(n))
					.Should().Have.Count.EqualTo(1);
			}

			using (ISession s = OpenSession())
			{
				Person personAlias = null;
				Child childAlias = null;
				var people =
					s.QueryOver<Person>(() => personAlias)
						.Left.JoinQueryOver(p => p.Children, () => childAlias, c => c.Nickname == personAlias.Name)
						.WhereRestrictionOn(c => c.Nickname).IsNotNull
						.List();

				people.Should().Have.Count.EqualTo(1);
			}

			using (ISession s = OpenSession())
			{
				Person personAlias = null;
				Child childAlias = null;
				var childNames =
					s.QueryOver<Person>(() => personAlias)
						.Left.JoinAlias(p => p.Children, () => childAlias, c => c.Nickname == personAlias.Name)
						.Select(p => childAlias.Nickname)
						.List<string>();

				childNames
					.Where(n => !string.IsNullOrEmpty(n))
					.Should().Have.Count.EqualTo(1);
			}
		}

		[Test]
		public void UniqueResult()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				Person actual =
					s.QueryOver<Person>()
						.SingleOrDefault();

				Assert.That(actual.Name, Is.EqualTo("test person 1"));
			}

			using (ISession s = OpenSession())
			{
				string actual =
					s.QueryOver<Person>()
						.Select(p => p.Name)
						.SingleOrDefault<string>();

				Assert.That(actual, Is.EqualTo("test person 1"));
			}
		}

		[Test]
		public void IsType()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var father1 = new Person() { Name = "Father 1" };
				var father2 = new CustomPerson() { Name = "Father 2" };

				var person1 = new Person() { Name = "Person 1", Father = father2 };
				var person2 = new CustomPerson() { Name = "Person 2", Father = father1 };

				s.Save(father1);
				s.Save(father2);

				s.Save(person1);
				s.Save(person2);

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var actual =
					s.QueryOver<Person>()
						.Where(p => p is CustomPerson)
						.And(p => p.Father != null)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
				Assert.That(actual[0].Name, Is.EqualTo("Person 2"));
			}

			using (ISession s = OpenSession())
			{
				var actual =
					s.QueryOver<Person>()
						.Where(p => p.GetType() == typeof(CustomPerson))
						.And(p => p.Father != null)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
				Assert.That(actual[0].Name, Is.EqualTo("Person 2"));
			}

			using (ISession s = OpenSession())
			{
				Person f = null;
				var actual =
					s.QueryOver<Person>()
						.JoinAlias(p => p.Father, () => f)
						.Where(() => f is CustomPerson)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
				Assert.That(actual[0].Name, Is.EqualTo("Person 1"));
			}

			using (ISession s = OpenSession())
			{
				Person f = null;
				var actual =
					s.QueryOver<Person>()
						.JoinAlias(p => p.Father, () => f)
						.Where(() => f.GetType() == typeof(CustomPerson))
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
				Assert.That(actual[0].Name, Is.EqualTo("Person 1"));
			}
		}

		[Test]
		public void OverrideEagerJoin()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Parent()
						.AddChild(new JoinedChild())
						.AddChild(new JoinedChild()));

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Parent>()
						.List();

				Assert.That(NHibernateUtil.IsInitialized(persons[0].Children), "Default query did not eagerly load children");
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Parent>()
						.Fetch(p => p.Children).Lazy
						.List();

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(!NHibernateUtil.IsInitialized(persons[0].Children), "Children not lazy loaded");
			}
		}

		[Test]
		public void RowCount()
		{
			SetupPagingData();

			using (ISession s = OpenSession())
			{
				IQueryOver<Person> query =
					s.QueryOver<Person>()
						.JoinQueryOver(p => p.Children)
						.OrderBy(c => c.Age).Desc
						.Skip(2)
						.Take(1);

				IList<Person> results = query.List();
				int rowCount = query.RowCount();
				object bigRowCount = query.RowCountInt64();

				Assert.That(results.Count, Is.EqualTo(1));
				Assert.That(results[0].Name, Is.EqualTo("Name 3"));
				Assert.That(rowCount, Is.EqualTo(4));
				Assert.That(bigRowCount, Is.TypeOf<long>());
				Assert.That(bigRowCount, Is.EqualTo(4));
			}
		}

		[Test]
		public void FunctionsOrder()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "p2", BirthDate = new DateTime(2008, 07, 06) });
				s.Save(new Person() { Name = "p1", BirthDate = new DateTime(2009, 08, 07) });
				s.Save(new Person() { Name = "p3", BirthDate = new DateTime(2007, 06, 05) });

				t.Commit();
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
			    var persons =
			        s.QueryOver<Person>()
			            .OrderBy(p => p.BirthDate.Year).Desc
			            .List();

			    persons.Count.Should().Be(3);
			    persons[0].Name.Should().Be("p1");
			    persons[1].Name.Should().Be("p2");
			    persons[2].Name.Should().Be("p3");
			}
		}

		[Test]
		public void MultiCriteria()
		{
			var driver = sessions.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);

			SetupPagingData();

			using (ISession s = OpenSession())
			{
				IQueryOver<Person> query =
					s.QueryOver<Person>()
						.JoinQueryOver(p => p.Children)
						.OrderBy(c => c.Age).Desc
						.Skip(2)
						.Take(1);

				var multiCriteria =
					s.CreateMultiCriteria()
						.Add("page", query)
						.Add<int>("count", query.ToRowCountQuery());

				var pageResults = (IList<Person>) multiCriteria.GetResult("page");
				var countResults = (IList<int>) multiCriteria.GetResult("count");

				Assert.That(pageResults.Count, Is.EqualTo(1));
				Assert.That(pageResults[0].Name, Is.EqualTo("Name 3"));
				Assert.That(countResults.Count, Is.EqualTo(1));
				Assert.That(countResults[0], Is.EqualTo(4));
			}

			using (ISession s = OpenSession())
			{
				QueryOver<Person> query =
					QueryOver.Of<Person>()
						.JoinQueryOver(p => p.Children)
						.OrderBy(c => c.Age).Desc
						.Skip(2)
						.Take(1);

				var multiCriteria =
					s.CreateMultiCriteria()
						.Add("page", query)
						.Add<int>("count", query.ToRowCountQuery());

				var pageResults = (IList<Person>) multiCriteria.GetResult("page");
				var countResults = (IList<int>) multiCriteria.GetResult("count");

				Assert.That(pageResults.Count, Is.EqualTo(1));
				Assert.That(pageResults[0].Name, Is.EqualTo("Name 3"));
				Assert.That(countResults.Count, Is.EqualTo(1));
				Assert.That(countResults[0], Is.EqualTo(4));
			}
		}

		private void SetupPagingData()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "Name 1", Age = 1 }
						.AddChild(new Child() { Nickname = "Name 1.1", Age = 1}));

				s.Save(new Person() { Name = "Name 2", Age = 2 }
						.AddChild(new Child() { Nickname = "Name 2.1", Age = 3}));

				s.Save(new Person() { Name = "Name 3", Age = 3 }
						.AddChild(new Child() { Nickname = "Name 3.1", Age = 2}));

				s.Save(new Person() { Name = "Name 4", Age = 4 }
						.AddChild(new Child() { Nickname = "Name 4.1", Age = 4}));

				t.Commit();
			}
		}

		[Test]
		public void StatelessSession()
		{
			int personId;
			using (var ss = sessions.OpenStatelessSession())
			using (var t = ss.BeginTransaction())
			{
				var person = new Person { Name = "test1" };
				ss.Insert(person);
				personId = person.Id;
				t.Commit();
			}

			using (var ss = sessions.OpenStatelessSession())
			using (ss.BeginTransaction())
			{
				var statelessPerson1 = ss.QueryOver<Person>()
					.List()[0];

				Assert.That(statelessPerson1.Id, Is.EqualTo(personId));

				var statelessPerson2 = QueryOver.Of<Person>()
					.GetExecutableQueryOver(ss)
					.List()[0];

				Assert.That(statelessPerson2.Id, Is.EqualTo(personId));
			}
		}
	}
}