using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1119
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1119"; }
		}

		[Test]
		public void SelectMinFromEmptyTable()
		{
			using (ISession s = OpenSession())
			{
				DateTime dt = s.CreateQuery("select max(tc.DateTimeProperty) from TestClass tc").UniqueResult<DateTime>();
				Assert.AreEqual(default(DateTime), dt);
				DateTime? dtn = s.CreateQuery("select max(tc.DateTimeProperty) from TestClass tc").UniqueResult<DateTime?>();
				Assert.IsFalse(dtn.HasValue);
			}
		}
	}
}
