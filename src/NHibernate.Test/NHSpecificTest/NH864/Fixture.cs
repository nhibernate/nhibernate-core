using System;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH864
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void OptimisticLocking()
		{
			using (ISession s = OpenSession())
			{
				ObjectWithNullableInt32 obj = new ObjectWithNullableInt32();
				s.Save(obj);
				s.Flush();

				obj.NullInt32 = 1;
				s.Flush();

				obj.NullInt32 = NullableInt32.Default;
				s.Flush();

				s.Delete(obj);
				s.Flush();
			}
		}
	}
}