using System;
using System.Collections;
using System.Collections.Generic;

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
				s.CreateQuery("delete from Person").ExecuteUpdate();
				s.CreateQuery("delete from Child").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void ICriteria_SimpleCriterion()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1" });
				s.Save(new Person() { Name = "test person 2" });
				s.Save(new Person() { Name = "test person 3" });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList<Person> actual =
					s.CreateCriteria(typeof(Person))
						.Add<Person>(p => p.Name == "test person 2")
						.List<Person>();

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ICriteriaOfT_SimpleCriterion()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Person() { Name = "test person 1" });
				s.Save(new Person() { Name = "test person 2" });
				s.Save(new Person() { Name = "test person 3" });

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList<Person> actual =
					s.QueryOver<Person>()
						.And(p => p.Name == "test person 2")
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}

	}

}