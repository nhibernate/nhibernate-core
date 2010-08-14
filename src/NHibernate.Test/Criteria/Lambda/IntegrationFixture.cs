using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

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

				var listProjection =
					s.QueryOver<Child>()
						.JoinQueryOver(c => c.Parent)
							.Where(p => p.Name == "Name 1" && p.Age == 33)
							.SelectList(list => list
								.Select(c => c.Nickname)
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
		public void MultiCriteria()
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

	}

}