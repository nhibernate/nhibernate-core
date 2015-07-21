using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1044
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void Crud()
		{
			// Only as a quick check that is can work with the idbag inside the component
			var p = new Person {Name = "Fiamma", Delivery = new Delivery()};
			p.Delivery.Adresses.Add("via Parenzo 96");
			p.Delivery.Adresses.Add("viale Don Bosco 192");
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(p);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var pp = s.Get<Person>(p.Id);
				pp.Delivery.Adresses.RemoveAt(0);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var pp = s.Get<Person>(p.Id);
				Assert.That(pp.Delivery.Adresses.Count, Is.EqualTo(1));
				s.Delete(pp);
				t.Commit();
			}
		}
	}
}