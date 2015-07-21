using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1905
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Query()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("select d from Det d left join d.Mas m where (SELECT count(e) FROM d.Mas.Els e WHERE e.Descr='e1')>0")
					.List();
			}
		}
	}
}
