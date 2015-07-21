using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{
	[TestFixture]
	public class SimpleIntegrationFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person { Name = "test person 1", Age = 20 });
				s.Save(new Person { Name = "test person 2", Age = 30 });
				s.Save(new Person { Name = "test person 3", Age = 40 });

				t.Commit();
			}
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
		public void TestQueryOver()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Person personAlias = null;

				var actual =
					s.QueryOver<Person>(() => personAlias)
						.Where(() => personAlias.Name == "test person 2")
						.And(() => personAlias.Age == 30)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void TestQueryOverAlias()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Person personAlias = null;

				var actual =
					s.QueryOver<Person>(() => personAlias)
						.Where(() => personAlias.Name == "test person 2")
						.And(() => personAlias.Age == 30)
						.List();

				Assert.That(actual.Count, Is.EqualTo(1));
			}
		}
	}
}
