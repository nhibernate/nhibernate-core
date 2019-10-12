using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2922
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				var a = new Store {Id = 1, Name = "A"};
				var b = new Store {Id = 2, Name = "B"};
				var jack = new Employee {Id = 3, Name = "Jack", Store = a};
				a.Staff.Add(jack);

				session.Save(a);
				session.Save(b);
				session.Save(jack);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public void ReparentedCase1()
		{
			using (var session = OpenSession())
			{
				var a = session.Get<Store>(1); // Order of loading affects outcome
				var b = session.Get<Store>(2);

				// Move employee to different store
				var jack = a.Staff.Single(x => x.Name == "Jack");

				a.Staff.Remove(jack);
				jack.Store = b;
				b.Staff.Add(jack);

				session.Flush();
				session.Clear();
			}

			using (var session = OpenSession())
			{
				var a = session.Get<Store>(1);
				var b = session.Get<Store>(2);

				Assert.That(a.Staff.Count, Is.EqualTo(0));
				Assert.That(b.Staff.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReparentedCase2()
		{
			using (var session = OpenSession())
			{
				var b = session.Get<Store>(2);
				var a = session.Get<Store>(1); // Order of loading affects outcome

				// Move employee to different store
				var jack = a.Staff.Single(x => x.Name == "Jack");

				a.Staff.Remove(jack);
				jack.Store = b;
				b.Staff.Add(jack);

				session.Flush();
				session.Clear();
			}

			using (var session = OpenSession())
			{
				var jack = session.Get<Employee>(3);
				var b = session.Get<Store>(2);
				var a = session.Get<Store>(1);

				Assert.That(jack, Is.Not.Null);
				Assert.That(a.Staff.Count, Is.EqualTo(0));
				Assert.That(b.Staff.Count, Is.EqualTo(1));
			}
		}
	}
}
