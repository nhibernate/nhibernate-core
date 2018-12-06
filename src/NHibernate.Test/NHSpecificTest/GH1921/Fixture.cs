using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1921
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "Bob"};
				session.Save(e1);

				var e2 = new Entity {Name = "Sally"};
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Theory]
		public void DmlInsert(bool filtered)
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (filtered)
					session.EnableFilter("NameFilter").SetParameter("name", "Bob");
				var rowCount = session.CreateQuery("insert into Entity (Name) select e.Name from Entity e").ExecuteUpdate();
				transaction.Commit();

				// If the DML has to take the filter into account, then below Assert should be
				// Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
				Assert.That(rowCount, Is.EqualTo(2));
			}
		}

		[Theory]
		public void DmlUpdate(bool filtered)
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (filtered)
					session.EnableFilter("NameFilter").SetParameter("name", "Bob");
				var rowCount = session.CreateQuery("update Entity e set Name = 'newName'").ExecuteUpdate();
				transaction.Commit();

				// If the DML has to take the filter into account, then below Assert should be
				// Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
				Assert.That(rowCount, Is.EqualTo(2));
			}
		}

		[Theory]
		public void DmlDelete(bool filtered)
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (filtered)
					session.EnableFilter("NameFilter").SetParameter("name", "Bob");
				var rowCount = session.CreateQuery("delete Entity").ExecuteUpdate();
				transaction.Commit();

				// If the DML has to take the filter into account, then below Assert should be
				// Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
				Assert.That(rowCount, Is.EqualTo(2));
			}
		}
	}
}
