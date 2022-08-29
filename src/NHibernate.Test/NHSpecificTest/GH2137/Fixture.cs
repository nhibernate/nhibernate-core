using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2137
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Entity e1;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHibernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void TestUpdateDetachedEntity()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				e1.Name = "Sally";
				session.Update(e1);
				transaction.Commit();
			}
		}
	}
}
