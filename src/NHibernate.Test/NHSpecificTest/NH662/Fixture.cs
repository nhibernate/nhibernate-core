using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH662
{
	[TestFixture, Ignore("Not supported.")]
	public class Fixture: BugTestCase
	{
		[Test]
		public void UseDerivedClass()
		{
			object savedId;
			var d = new Derived {Description = "something"};

			using (ISession s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				savedId = s.Save(d);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				d = s.Load<Derived>(savedId);
				Assert.That(d.Description, Is.EqualTo("something"));
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete(d);
				tx.Commit();
			}
		}
	}
}