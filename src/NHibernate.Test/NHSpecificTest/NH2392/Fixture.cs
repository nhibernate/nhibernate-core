using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2392
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.Delete("from A");
				s.Flush();
			}
		}

		[Test]
		public void CompositeUserTypeSettability()
		{
			ISession s = OpenSession();
			try
			{
				s.Save(new A { StringData1 = "first", StringData2 = "second" });
				s.Flush();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			try
			{
				A a = s.CreateCriteria<A>().List<A>().First();
				a.MyPhone = new PhoneNumber(1, null);
				s.Save(a);
				s.Flush();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			try
			{
				A a = s.CreateCriteria<A>().List<A>().First();
				a.MyPhone = new PhoneNumber(1, "555-1234");
				s.Save(a);
				s.Flush();
			}
			finally
			{
				s.Close();
			}
		}
	}
}
