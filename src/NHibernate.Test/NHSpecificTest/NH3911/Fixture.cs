using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3911
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3911"; }
		}

		[Test]
		public void TestNH3874()
		{
		    using (ISession session = OpenSession())
		    {
		        using (ITransaction tx = session.BeginTransaction())
		        {
		            var d1 = new Derived1();
		            var d2 = new Derived2();

		            d1.Ref = d2;
		            d2.Ref = d1;

		            session.Save(d1);

		            tx.Commit();
		        }

		        session.QueryOver<Base>().List();
		    }
        }

        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.Delete("from Derived1");

                tx.Commit();
            }
        }
    }
}
