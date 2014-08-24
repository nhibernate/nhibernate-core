using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH887
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void PropertyRefReferencingParentProperty()
		{
			using (ISession s = OpenSession())
			{
				Child child = new Child();
				child.UniqueKey = 10;
				s.Save(child);

				Consumer consumer = new Consumer();
				consumer.Child = child;
				s.Save(consumer);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Consumer");
				s.Delete("from Child");
				s.Flush();
			}
		}
	}
}