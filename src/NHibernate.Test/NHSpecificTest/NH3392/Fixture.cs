using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3392
{
    [TestFixture]
    public class Fixture : BugTestCase
	{
        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete("from Kid");
                session.Delete("from FriendOfTheFamily");
                session.Delete("from Dad");
                session.Delete("from Mum");
                session.Flush();
                transaction.Commit();
            }
        }

        [Test]
        public void ExpandSubCollectionWithEmbeddedCompositeID()
        {
            using (ISession s = OpenSession())
            {

                var jenny = new Mum { Name = "Jenny" };
                s.Save(jenny);
                var benny = new Dad { Name = "Benny" };
                s.Save(benny);
                var lenny = new Dad { Name = "Lenny" };
                s.Save(lenny);
                var jimmy = new Kid { Name = "Jimmy", MumId = jenny.Id, DadId = benny.Id };
                s.Save(jimmy);
                var timmy = new Kid { Name = "Timmy", MumId = jenny.Id, DadId = lenny.Id };
                s.Save(timmy);
                s.Flush();
            }

            using (var s = OpenSession())
            {
                var result=s.Query<Mum>().Select(x => new { x, x.Kids }).ToList();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].x.Kids, Is.EquivalentTo(result[0].Kids));
            }
        }

        [Test]
        public void ExpandSubCollectionWithCompositeID()
        {
            using (ISession s = OpenSession())
            {

                var jenny = new Mum { Name = "Jenny" };
                s.Save(jenny);
                var benny = new Dad { Name = "Benny" };
                s.Save(benny);
                var lenny = new Dad { Name = "Lenny" };
                s.Save(lenny);
                var jimmy = new FriendOfTheFamily { Name = "Jimmy", Id = new MumAndDadId { MumId = jenny.Id, DadId = benny.Id } };
                s.Save(jimmy);
                var timmy = new FriendOfTheFamily { Name = "Timmy", Id = new MumAndDadId { MumId = jenny.Id, DadId = lenny.Id } };
                s.Save(timmy);
                s.Flush();
            }

            using (var s = OpenSession())
            {
                var result=s.Query<Mum>().Select(x => new { x, x.Friends }).ToList();
                Assert.That(result.Count, Is.EqualTo(1));
                Assert.That(result[0].x.Friends, Is.EquivalentTo(result[0].Friends));
            }
        }

		
	}
}
