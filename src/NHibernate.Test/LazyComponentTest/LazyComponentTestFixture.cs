using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.LazyComponentTest
{
	[TestFixture]
	public class LazyComponentTestFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"LazyComponentTest.Person.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var person = new Person
				{
					Name = "Gabor",
					Address = new Address
					{
						Country = "HUN",
						City = "Budapest"
					}
				};
				s.Persist(person);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void LazyLoadTest()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var p = s.CreateQuery("from Person p where name='Gabor'").UniqueResult<Person>();
				// make sure component has not been initialized yet
				Assert.That(NHibernateUtil.IsPropertyInitialized(p, "Address"), Is.False);

				t.Commit();
			}
		}

		[Test]
		public void LazyDeleteTest()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var p = s.CreateQuery("from Person p where name='Gabor'").UniqueResult<Person>();
				// make sure component has not been initialized yet
				Assert.That(NHibernateUtil.IsPropertyInitialized(p, "Address"), Is.False);
				s.Delete(p);
				t.Commit();
			}
		}

		[Test]
		public void LazyUpdateTest()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var p = s.CreateQuery("from Person p where name='Gabor'").UniqueResult<Person>();
				// make sure component has not been initialized yet
				Assert.That(!NHibernateUtil.IsPropertyInitialized(p, "Address"));

				p.Address.City = "Baja";
				s.Update(p);

				t.Commit();
			}
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var p = s.CreateQuery("from Person p where name='Gabor'").UniqueResult<Person>();
				Assert.That(p.Address.City, Is.EqualTo("Baja"));

				t.Commit();
			}
		}
	}
}
