using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH940
{
	[TestFixture]
	public class NH940Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			A a = new A();
			B b = new B();
			a.B = b;

			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(a);
			t.Commit();
			s.Close();

			s = OpenSession();
			IList l = s.CreateCriteria(typeof(A)).List();
			try
			{
				((A) l[0]).Execute();
				Assert.Fail("Should have thrown MyException");
			}
			catch (MyException)
			{
				// OK
			}
			catch (Exception e)
			{
				Assert.Fail("Should have thrown MyException, thrown {0} instead", e);
			}

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(a);
			t.Commit();
			s.Close();
		}
	}
}