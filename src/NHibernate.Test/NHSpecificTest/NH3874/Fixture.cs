using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3874
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3874"; }
		}

		[Test]
		public void TestNH3874()
		{
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                Two two = new Two { One = new One() };
                two.One.Twos = new[] { two };

                session.Save(two);

                tx.Commit();
            }
		}

        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Delete("from Two");
                session.Delete("from One");

                tx.Commit();
            }
        }
    }
}
