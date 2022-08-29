using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3964
{
	[TestFixture]
	public class Fixture
	{
		[Test(Description = "Test for removal of a workaround for an old Fx bug (<v4)")]
		public void AddingNullToNonGenericListShouldNotThrow()
		{
			var a1 = new ArrayList { null };
			var a2 = new ArrayList();
			Assert.DoesNotThrow(() => ArrayHelper.AddAll(a2, a1));
		}
	}
}
