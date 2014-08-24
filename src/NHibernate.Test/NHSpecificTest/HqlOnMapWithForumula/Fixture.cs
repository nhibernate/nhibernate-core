using System;
using System.Text;
using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.HqlOnMapWithForumula
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "HqlOnMapWithForumula"; }
		}


		[Test]
		public void TestBug()
		{
			using (ISession s = sessions.OpenSession())
			{
				s.CreateQuery("from A a where 1 in elements(a.MyMaps)")
					.List();
			}
		}
	}
}