using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2808
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void CheckExistanceOfEntity()
		{
			// save an instance of Entity1
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var a = new Entity { Name = "A" };
				session.Save("Entity1", a, 1);

				transaction.Commit();
			}

			// check that it is correctly stored in the Entity1 table and does not exist in the Entity2 table.
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var a = session.Get("Entity1", 1);
				Assert.IsNotNull(a);

				a = session.Get("Entity2", 1);
				Assert.IsNull(a);
			}
		}

		[Test]
		public void Update()
		{
			// save an instance of Entity1
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var a = new Entity { Name = "A" };
				session.Save("Entity1", a, 1);

				transaction.Commit();
			}

			// load the saved entity, change its name and update.
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var a = (Entity)session.Get("Entity1", 1);
				a.Name = "A'";

				session.Update("Entity1",a, 1);
				
				transaction.Commit();
			}

			// verify
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var a = (Entity)session.Get("Entity1", 1);

				Assert.AreEqual("A'", a.Name);
			}
		}

		[Test]
		public void SaveOrUpdate()
		{
			// save an instance of Entity1.
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var a = new Entity { Name = "A" };
				session.Save("Entity1", a, 1);

				transaction.Commit();
			}

			// load the entity and adjust its name, create a new entity and save or update both.
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var a = (Entity)session.Get("Entity1", 1);
				a.Name = "A'";

				var b = new Entity {Name = "B"};

				session.SaveOrUpdate("Entity1", a, 1);
				session.SaveOrUpdate("Entity1", b, 2);

				transaction.Commit();
			}

			// verify
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var a = (Entity)session.Get("Entity1", 1);
				var b = (Entity)session.Get("Entity1", 2);

				Assert.AreEqual("A'", a.Name);
				Assert.AreEqual("B", b.Name);
			}
		}
	}
}
