using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2111
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using( ISession s = sessions.OpenSession() )
			{
				s.Delete( "from A" );
				s.Flush();
			}
		}

		[Test]
		public void SyncRootOnLazyLoad()
		{
			A a = new A();
			a.Name = "first generic type";
			a.LazyItems = new List<string>();
			a.LazyItems.Add("first string");
			a.LazyItems.Add("second string");
			a.LazyItems.Add("third string");

			ISession s = OpenSession();
			s.SaveOrUpdate(a);
			s.Flush();
			s.Close();

			Assert.IsNotNull(((ICollection) a.LazyItems).SyncRoot);
			Assert.AreEqual("first string", a.LazyItems[0]);

			s = OpenSession();
			a = (A)s.Load(typeof(A), a.Id);

			Assert.IsNotNull(((ICollection) a.LazyItems).SyncRoot);
			Assert.AreEqual("first string", a.LazyItems[0]);

			s.Close();
		}
	}
}