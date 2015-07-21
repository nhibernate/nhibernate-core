using System.Collections.Generic;
using System.Linq;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2721
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using( ISession s = sessions.OpenSession() )
			{
                s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
        [Ignore("Does not work because of extraneous update that sets reference to A to null.  Can be worked-around by removing not-null on reference to A.")]
		public void ListRemoveInsert()
		{
			A a = new A("a");

            B b1 = new B("b1");
            a.Bs.Add(b1);
		    b1.A = a;

            B b2 = new B("b2");
            a.Bs.Add(b2);
            b2.A = a;

            B b3 = new B("b3");
            a.Bs.Add(b3);
            b3.A = a;

            using (ISession s = OpenSession())
            {
                s.SaveOrUpdate(a);
                s.Flush();
            }

            using (ISession s = OpenSession())
            {
                a = (A) s.Load(typeof (A), a.Id);
                CollectionAssert.AreEquivalent(new[] {"b1", "b2", "b3"}, a.Bs.Select(b => b.Name));

                B removed = a.Bs[2];
                a.Bs.Remove(removed);
                a.Bs.Insert(1, removed);
                Assert.IsNotNull(a.Bs[0].A);
                Assert.IsNotNull(a.Bs[1].A);
                Assert.IsNotNull(a.Bs[2].A);
                CollectionAssert.AreEquivalent(new[] {"b1", "b3", "b2"}, a.Bs.Select(b => b.Name));
                s.SaveOrUpdate(a);
                s.Flush();
            }

            using (ISession s = OpenSession())
            {
                a = (A) s.Load(typeof (A), a.Id);
                CollectionAssert.AreEquivalent(new[] { "b1", "b3", "b2" }, a.Bs.Select(b => b.Name));
            }
		}
	}
}