using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;
using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.NH2244
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
		public void LinqComponentTypeEquality()
		{
			ISession s = OpenSession();
			try
			{
				s.Save(new A { Phone = new PhoneNumber(1, "555-1111") });
				s.Save(new A { Phone = new PhoneNumber(1, "555-2222") });
				s.Save(new A { Phone = new PhoneNumber(1, "555-3333") });
				s.Flush();
			}
			finally
			{
				s.Close();
			}

			s = OpenSession();
			try
			{
				A item = s.Query<A>().Where(a => a.Phone == new PhoneNumber(1, "555-2222")).Single();
				Assert.AreEqual("555-2222", item.Phone.Number);
			}
			finally
			{
				s.Close();
			}
		}
	}
}
