using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1719
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private static readonly Guid RootGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var rootEntry = new FileEntryEntity
				{
					Id = RootGuid,
					Name = "root",
				};

				session.Save(rootEntry);

				var rootText = new FileEntryEntity
				{
					Id = Guid.NewGuid(),
					ParentId = rootEntry.Id,
					Name = "text1.txt",
				};

				var rootTextData = new FileDataEntity
				{
					Entry = rootText
				};

				rootText.Data = rootTextData;

				session.Save(rootText);
				session.Save(rootTextData);

				var subEntry = new FileEntryEntity
				{
					Id = Guid.NewGuid(),
					ParentId = rootEntry.Id,
					Name = "test1",
				};

				session.Save(subEntry);

				var subText = new FileEntryEntity
				{
					Id = Guid.NewGuid(),
					ParentId = subEntry.Id,
					Name = "text1.txt",
				};

				var subTextData = new FileDataEntity
				{
					Entry = subText
				};

				subText.Data = subTextData;

				session.Save(subText);
				session.Save(subTextData);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Order is important and we need both queries so that we don't trigger the exception
				session.CreateQuery("delete from FileDataEntity").ExecuteUpdate();
				session.CreateQuery("delete from FileEntryEntity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void TestDeleteAfterRecursiveQueries()
		{
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var path = new[] { "test1", "text1.txt" };
				var found = session.Load<FileEntryEntity>(RootGuid);
				foreach (var pathPart in path)
				{
					var next = session.Query<FileEntryEntity>()
									  .SingleOrDefault(x => x.ParentId == found.Id && x.Name == pathPart);
					Assert.That(next, Is.Not.Null);
					found = next;
				}

				session.Delete(found);
				trans.Commit();
			}
		}
	}
}
