using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1284
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using var s = OpenSession();
			using var tx = s.BeginTransaction();
			s.Delete("from Person");
			tx.Commit();
		}

		[Test]
		public void EmptyValueTypeComponent()
		{
			Person jimmy;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = new Person("Jimmy Hendrix");
				s.Save(p);
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				jimmy = s.Get<Person>("Jimmy Hendrix");
				tx.Commit();
			}

			Assert.That(jimmy.Address, Is.Null);
		}
	}
}
