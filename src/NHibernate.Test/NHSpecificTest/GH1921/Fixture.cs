using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1921
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.NativeGeneratorSupportsBulkInsertion;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				var me1 = new MultiTableEntity { Name = "Bob", OtherName = "Bob" };
				session.Save(me1);

				var me2 = new MultiTableEntity { Name = "Sally", OtherName = "Sally" };
				session.Save(me2);

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
				var rowCount = session.CreateQuery("insert into Entity (Name) select e.Name from Entity e")
				                      .ExecuteUpdate();
				transaction.Commit();

				Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
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

				Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
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

				Assert.That(rowCount, Is.EqualTo(filtered ? 1 : 2));
			}
		}

		[TestCase(null)]
		[TestCase("NameFilter")]
		[TestCase("OtherNameFilter")]
		public void MultiTableDmlInsert(string filter)
		{
			var isFiltered = !string.IsNullOrEmpty(filter);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (isFiltered)
					session.EnableFilter(filter).SetParameter("name", "Bob");
				var rowCount =
					session
						.CreateQuery(
							// No insert of OtherName: not supported (INSERT statements cannot refer to superclass/joined properties)
							"insert into MultiTableEntity (Name) select e.Name from MultiTableEntity e")
						.ExecuteUpdate();
				transaction.Commit();

				Assert.That(rowCount, Is.EqualTo(isFiltered ? 1 : 2), "ResultCount");
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.Name != "Bob"),
					Is.EqualTo(isFiltered ? 1 : 2), "Name != \"Bob\"");
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.OtherName == null),
					Is.EqualTo(isFiltered ? 1 : 2), "OtherName is null");
				transaction.Commit();
			}
		}

		[TestCase(null)]
		[TestCase("NameFilter")]
		[TestCase("OtherNameFilter")]
		public void MultiTableDmlUpdate(string filter)
		{
			var isFiltered = !string.IsNullOrEmpty(filter);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (isFiltered)
					session.EnableFilter(filter).SetParameter("name", "Bob");
				var rowCount =
					session
						.CreateQuery(
							"update MultiTableEntity e" +
							" set Name = 'newName', OtherName = 'newOtherName'" +
							// Check referencing columns is supported
							" where e.Name is not null and e.OtherName is not null")
						.ExecuteUpdate();
				transaction.Commit();

				Assert.That(rowCount, Is.EqualTo(isFiltered ? 1 : 2), "ResultCount");
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.Name == "newName"),
					Is.EqualTo(isFiltered ? 1 : 2), "Name == \"newName\"");
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.OtherName == "newOtherName"),
					Is.EqualTo(isFiltered ? 1 : 2), "Name == \"newOtherName\"");
				transaction.Commit();
			}
		}

		[TestCase(null)]
		[TestCase("NameFilter")]
		[TestCase("OtherNameFilter")]
		public void MultiTableDmlDelete(string filter)
		{
			var isFiltered = !string.IsNullOrEmpty(filter);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (isFiltered)
					session.EnableFilter(filter).SetParameter("name", "Bob");
				var rowCount =
					session
						.CreateQuery(
							"delete MultiTableEntity e" +
							// Check referencing columns is supported
							" where e.Name is not null and e.OtherName is not null")
						.ExecuteUpdate();
				transaction.Commit();

				Assert.That(rowCount, Is.EqualTo(isFiltered ? 1 : 2), "ResultCount");
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.Name != "Bob"),
					Is.EqualTo(isFiltered ? 1 : 0), "Name != \"Bob\"");
				Assert.That(
					session.Query<MultiTableEntity>().Count(e => e.OtherName != "Bob"),
					Is.EqualTo(isFiltered ? 1 : 0), "OtherName != \"Bob\"");
				transaction.Commit();
			}
		}
	}
}
