using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2361
{
	public class Fixture: BugTestCase
	{
		[Test]
		public void WhenDeleteMultiTableHierarchyThenNotThrows()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}