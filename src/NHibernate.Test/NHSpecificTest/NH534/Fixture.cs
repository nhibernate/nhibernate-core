using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH534
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH534"; }
		}

		[Test]
		public void Bug()
		{
			Parent p;
			Child c;
			using (ISession s = OpenSession())
			{
				p = new Parent();
				s.Save(p);
				c = new Child(p);
				s.Save(c);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Delete(c);
				s.Delete(p);
				s.Flush();
			}
		}
	}
}