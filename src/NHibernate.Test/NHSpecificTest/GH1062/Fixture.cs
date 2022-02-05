using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1062
{
	[TestFixture]
	public class TriggerIdentityDinamicInsertFixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Oracle8iDialect;
		}

		[Test]
		public void CanSaveEnity()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var e = new MyEntity { Name = "entity-1" };
				session.Save(e);

				Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

				session.Delete(e);
				tran.Commit();
				session.Close();
			}
		}
	}
}
