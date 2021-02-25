using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1565
{
	[TestFixture]
	public class LockEntityWithOuterJoinTest : BugTestCase
	{
		[Test]
		public void LockWithOuterJoin_ShouldBePossible()
		{
			// For this test to succeed with SQL Anywhere, ansi_update_constraints must be off.
			// In I-SQL: set option ansi_update_constraints = 'Off'
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var entity = session.Get<MainEntity>(id, LockMode.Upgrade);
					Assert.That(entity.Id, Is.EqualTo(id));
					transaction.Commit();
				}
			}
		}

		private int id;
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.FlushMode = FlushMode.Auto;
					var entity = new MainEntity();
					session.Save(entity);
					transaction.Commit();
					id = entity.Id;
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.CreateSQLQuery("delete from MainEntity").ExecuteUpdate();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}
	}

	public class MainEntity
	{
		public virtual int Id { get; set; } = 0;

		public virtual string Data { get; set; }
	}
}
