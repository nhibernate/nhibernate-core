using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1706
{
	[TestFixture]
	public class KeyPropertyRefFixture : BugTestCase
	{
		[Test]
		public void PropertyRefUsesOtherColumn()
		{
			const string ExtraId = "extra";

			var a = new A { Name = "First", ExtraIdA = ExtraId };

			var b = new B { Name = "Second", ExtraIdB = ExtraId };

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(a);
				s.Save(b);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				var newA = s.Get<A>(a.Id);

				Assert.AreEqual(1, newA.Items.Count);
			}

			// cleanup
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from B");
				s.Delete("from A");
				tx.Commit();
			}
		}
	}
}