using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using SharpTestsEx;

using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Test.Criteria.Lambda
{

	[TestFixture]
	public class IntegrationFixture : TestCase
	{

		protected override string MappingsAssembly { get { return "NHibernate.Test"; } }

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Criteria.Lambda.Mappings.hbm.xml",
					};
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
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
		public void ICriteriaOfT_SimpleCriterion()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				s.Save(new Person() { Name = "test person 2", Age = 30 });
				s.Save(new Person() { Name = "test person 3", Age = 40 });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList<Person> actual =
					s.QueryOver<Person>()
						.Where(p => p.Name == "test person 2")
						.And(p => p.Age == 30)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
			}

			using (ISession s = OpenSession())
			{
				Person personAlias = null;

				IList<Person> actual =
					s.QueryOver<Person>(() => personAlias)
						.Where(() => personAlias.Name == "test person 2")
						.And(() => personAlias.Age == 30)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
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
		public void Project_SingleProperty()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				s.Save(new Person() { Name = "test person 2", Age = 30 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var actual =
					s.QueryOver<Person>()
						.Select(p => p.Age)
						.OrderBy(p => p.Age).Asc
						.List<int>();

				Assert.That(actual[0], Is.EqualTo(20));
			}
		}

		[Test]
		public void Project_MultipleProperties()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				s.Save(new Person() { Name = "test person 2", Age = 30 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				Person personAlias = null;
				var actual =
					s.QueryOver<Person>(() => personAlias)
						.Select(p => p.Name,
								p => personAlias.Age)
						.OrderBy(p => p.Age).Asc
						.List<object[]>()
						.Select(props => new {
							TestName = (string)props[0],
							TestAge = (int)props[1],
							});

				Assert.That(actual.ElementAt(0).TestName, Is.EqualTo("test person 1"));
				Assert.That(actual.ElementAt(1).TestAge, Is.EqualTo(30));
			}
		}

		[Test]
		public void Project_TransformToDto()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1", Age = 20 });
				s.Save(new Person() { Name = "test person 1", Age = 30 });
				s.Save(new Person() { Name = "test person 2", Age = 40 });
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				PersonSummary summary = null;
				var actual =
					s.QueryOver<Person>()
						.SelectList(list => list
							.SelectGroup(p => p.Name).WithAlias(() => summary.Name)
							.Select(Projections.RowCount()).WithAlias(() => summary.Count))
						.OrderByAlias(() => summary.Name).Asc
						.TransformUsing(Transformers.AliasToBean<PersonSummary>())
						.List<PersonSummary>();

				Assert.That(actual.Count, Is.EqualTo(2));
				Assert.That(actual[0].Name, Is.EqualTo("test person 1"));
				Assert.That(actual[0].Count, Is.EqualTo(2));
				Assert.That(actual[1].Name, Is.EqualTo("test person 2"));
				Assert.That(actual[1].Count, Is.EqualTo(1));
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
		public void SubCriteria()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "Name 1" }
						.AddChild(new Child() { Nickname = "Name 1.1"})
						.AddChild(new Child() { Nickname = "Name 1.2"}));

				s.Save(new Person() { Name = "Name 2" }
						.AddChild(new Child() { Nickname = "Name 2.1"})
						.AddChild(new Child() { Nickname = "Name 2.2"}));

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Person>()
						.JoinQueryOver(p => p.Children)
							.Where(c => c.Nickname == "Name 2.1")
							.List();

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("Name 2"));
			}
		}

		[Test]
		public void SubCriteriaProjections()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "Name 1", Age = 33 }
						.AddChild(new Child() { Nickname = "Name 1.1", Age = 3}));

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var simpleProjection =
					s.QueryOver<Child>()
						.JoinQueryOver(c => c.Parent)
							.Where(p => p.Name == "Name 1" && p.Age == 33)
							.Select(c => c.Nickname, c => c.Age)
							.List<object[]>()
							.Select(props => new
								{
									Name = (string)props[0],
									Age = (int)props[1],
								});

				Assert.That(simpleProjection.Count(), Is.EqualTo(1));
				Assert.That(simpleProjection.First().Name, Is.EqualTo("Name 1.1"));
				Assert.That(simpleProjection.First().Age, Is.EqualTo(3));

				Child childAlias = null;
				var listProjection =
					s.QueryOver<Child>(() => childAlias)
						.JoinQueryOver(c => c.Parent)
							.Where(p => p.Name == "Name 1" && p.Age == 33)
							.SelectList(list => list
								.Select(c => childAlias.Nickname)
								.Select(c => c.Age))
							.List<object[]>()
							.Select(props => new
								{
									Name = (string)props[0],
									Age = (int)props[1],
								});

				Assert.That(listProjection.Count(), Is.EqualTo(1));
				Assert.That(listProjection.First().Name, Is.EqualTo("Name 1.1"));
				Assert.That(listProjection.First().Age, Is.EqualTo(3));
			}
		}

		[Test]
		public void SubQuery()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "Name 1", Age = 1 }
						.AddChild(new Child() { Nickname = "Name 1.1", Age = 1}));

				s.Save(new Person() { Name = "Name 2", Age = 2 }
						.AddChild(new Child() { Nickname = "Name 2.1", Age = 2})
						.AddChild(new Child() { Nickname = "Name 2.2", Age = 2}));

				s.Save(new Person() { Name = "Name 3", Age = 3 }
						.AddChild(new Child() { Nickname = "Name 3.1", Age = 3}));

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				Person personAlias = null;
				object childCountAlias = null;

				QueryOver<Child> averageChildAge =
					QueryOver.Of<Child>()
						.SelectList(list => list.SelectAvg(c => c.Age));

				QueryOver<Child> childCountQuery =
					QueryOver.Of<Child>()
						.Where(c => c.Parent.Id == personAlias.Id)
						.Select(Projections.RowCount());

				var nameAndChildCount =
					s.QueryOver<Person>(() => personAlias)
						.WithSubquery.Where(p => p.Age <= averageChildAge.As<int>())
						.SelectList(list => list
							.Select(p => p.Name)
							.SelectSubQuery(childCountQuery).WithAlias(() => childCountAlias))
						.OrderByAlias(() => childCountAlias).Desc
						.List<object[]>()
						.Select(props => new {
							Name = (string)props[0],
							ChildCount = (int)props[1],
							})
						.ToList();

				Assert.That(nameAndChildCount.Count, Is.EqualTo(2));

				Assert.That(nameAndChildCount[0].Name, Is.EqualTo("Name 2"));
				Assert.That(nameAndChildCount[0].ChildCount, Is.EqualTo(2));

				Assert.That(nameAndChildCount[1].Name, Is.EqualTo("Name 1"));
				Assert.That(nameAndChildCount[1].ChildCount, Is.EqualTo(1));
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
		public void Functions()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "p1", BirthDate = new DateTime(2009, 08, 07), Age = 90 });
				s.Save(new Person() { Name = "p2", BirthDate = new DateTime(2008, 07, 06) });
				s.Save(new Person() { Name = "pP3", BirthDate = new DateTime(2007, 06, 05) });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Person>()
						.Where(p => p.BirthDate.YearPart() == 2008)
						.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Person>()
						.Where(p => p.BirthDate.YearPart().IsIn(new [] { 2008, 2009 }))
						.OrderBy(p => p.Name).Asc
						.List();

				persons.Count.Should().Be(2);
				persons[0].Name.Should().Be("p1");
				persons[1].Name.Should().Be("p2");
			}

			using (ISession s = OpenSession())
			{
				var yearOfBirth =
					s.QueryOver<Person>()
						.Where(p => p.Name == "p2")
						.Select(p => p.BirthDate.YearPart())
						.SingleOrDefault<object>();

				yearOfBirth.GetType().Should().Be(typeof(int));
				yearOfBirth.Should().Be(2008);
			}

			using (ISession s = OpenSession())
			{
				var avgYear =
					s.QueryOver<Person>()
						.SelectList(list => list.SelectAvg(p => p.BirthDate.YearPart()))
						.SingleOrDefault<object>();

				avgYear.GetType().Should().Be(typeof(double));
				string.Format("{0:0}", avgYear).Should().Be("2008");
			}

			using (ISession s = OpenSession())
			{
				var sqrtOfAge =
					s.QueryOver<Person>()
						.Where(p => p.Name == "p1")
						.Select(p => p.Age.Sqrt())
						.SingleOrDefault<object>();

				sqrtOfAge.Should().Be.InstanceOf<double>();
				string.Format("{0:0.00}", sqrtOfAge).Should().Be((9.49).ToString());
			}

			using (ISession s = OpenSession())
			{
				var names =
					s.QueryOver<Person>()
						.Where(p => p.Name == "pP3")
						.Select(p => p.Name.Lower(), p => p.Name.Upper())
						.SingleOrDefault<object[]>();

				names[0].Should().Be("pp3");
				names[1].Should().Be("PP3");
			}

			using (ISession s = OpenSession())
			{
				var name =
					s.QueryOver<Person>()
						.Where(p => p.Name == "p1")
						.Select(p => Projections.Concat(p.Name, ", ", p.Name))
						.SingleOrDefault<string>();

				name.Should().Be("p1, p1");
			}
		}

		[Test]
		public void FunctionsProperty()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "p1", BirthDate = new DateTime(2009, 08, 07) });
				s.Save(new Person() { Name = "p2", BirthDate = new DateTime(2008, 07, 07) });
				s.Save(new Person() { Name = "p3", BirthDate = new DateTime(2007, 06, 07) });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				var persons =
					s.QueryOver<Person>()
						.Where(p => p.BirthDate.MonthPart() == p.BirthDate.DayPart())
						.List();

				persons.Count.Should().Be(1);
				persons[0].Name.Should().Be("p2");
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
			using (ITransaction t = s.BeginTransaction())
			{
				var persons =
					s.QueryOver<Person>()
						.OrderBy(p => p.BirthDate.YearPart()).Desc
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
			using (var ss = sessions.OpenStatelessSession())
			{
				using (var tx = ss.BeginTransaction())
				{
					var person = new Person() { Name = "test1" };
					ss.Insert(person);

					var statelessPerson1 =
						ss.QueryOver<Person>()
							.List()
							[0];

					Assert.That(statelessPerson1.Id, Is.EqualTo(person.Id));

					var statelessPerson2 =
						QueryOver.Of<Person>()
							.GetExecutableQueryOver(ss)
							.List()
							[0];

					Assert.That(statelessPerson2.Id, Is.EqualTo(person.Id));
				}
			}
		}

	}

}