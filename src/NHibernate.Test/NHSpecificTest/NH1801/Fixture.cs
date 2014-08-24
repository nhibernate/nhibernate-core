using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1801
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			try
			{
				using (ISession s = OpenSession())
				{
					var a1 = new A {Id = 1, Name = "A1"};
					var a2 = new A {Id = 2, Name = "A2"};

					var b1 = new B {Id = 1, Name = "B1", A = a1};
					var b2 = new B {Id = 2, Name = "B2", A = a2};

					var c1 = new C {Name = "C1", A = a1};
					var c2 = new C {Name = "C2", A = a2};

					s.Save(a1);
					s.Save(a2);
					s.Save(b1);
					s.Save(b2);
					s.Save(c1);
					s.Save(c2);

					s.Flush();
				}

				using (ISession s = OpenSession())
				{
					IList res = s.CreateQuery("from B b, C c where b.A = c.A and b.Id = :id").SetInt32("id", 1).List();

					Assert.That(res, Has.Count.EqualTo(1));

					s.Flush();
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				{
					s.Delete("from B");
					s.Delete("from C");
					s.Delete("from A");

					s.Flush();
				}
			}
		}
	}
}