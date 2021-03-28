using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2631
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = new Person
				{
					Name = "Testing"
				};
				person.Address = new Address
				{
					Person = person,
					Street = "Mulberry"
				};
				session.Save(person);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Address").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void IndexOutOfRangeOnDelete()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var persons = session.Query<Person>().ToList();

				foreach (var person in persons)
				{
					session.Delete(person);
				}

				t.Commit();
			}
		}

		[Test]
		public void Update()
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var persons = session.Query<Person>().ToList();

				foreach (var person in persons)
				{
					person.Name = "x";
					person.Address = null;
				}

				t.Commit();
			}
		}
	}
}
