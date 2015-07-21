using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1895
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		[Test]
		public void SaveTest()
		{
			var o = new Order {Id = Guid.NewGuid(), Name = "Test Order"};
			for (int i = 0; i < 5; i++)
			{
				var d = new Detail {Id = Guid.NewGuid(), Name = "Test Detail " + i, Parent = o};
				o.Details.Add(d);
			}
			using (ISession session = OpenSession())
			{
				session.Save(o);
				session.Flush();
			}
			using (ISession session = OpenSession())
			{
				session.Delete(o);
				session.Flush();
			}
		}
	}
}
