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
						.UniqueResult();

				Assert.That(actual.Name, Is.EqualTo("test person 1"));
			}

			using (ISession s = OpenSession())
			{
				string actual =
					s.QueryOver<Person>()
						.Select(p => p.Name)
						.UniqueResult<string>();

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

	}

}