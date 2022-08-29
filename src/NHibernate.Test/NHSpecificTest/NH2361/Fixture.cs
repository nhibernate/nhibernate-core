using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2361
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// DML Delete on multi-tables entity requires temp table.
			return Dialect.SupportsTemporaryTables;
		}

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
