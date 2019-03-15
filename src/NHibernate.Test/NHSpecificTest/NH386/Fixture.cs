using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH386
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Query()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("from _Parent _p left join _p._Children _c where _c._Id > 0")
					.List();
			}
		}
	}
}
