using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1754
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
		}

		protected override void OnTearDown()
		{
			Sfi.Statistics.IsStatisticsEnabled = false;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Firebird does not like deleting tables with auto-fk.
				foreach (var e in session.Query<Entity>())
				{
					e.Children.Clear();
				}
				session.Flush();

				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void PersistIdentityDoNotImmediateExecuteQuery()
		{
			using (var session = OpenSession())
			{
				Sfi.Statistics.Clear();
				session.Persist(new Entity {Name = "Test"});

				Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(0));

				session.Flush();

				Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void PersistIdentityDoNotSaveIfSessionIsNotFlushed()
		{
			using (var session = OpenSession())
			{
				session.Persist(new Entity {Name = "Test"});
			}

			using (var session = OpenSession())
			{
				var count = session.Query<Entity>().Count();
				Assert.That(count, Is.EqualTo(0));
			}
		}

		// https://hibernate.atlassian.net/browse/HHH-12826
		[Test]
		public void CanAddChildAfterFlush()
		{
			using (var session = OpenSession())
			{
				var parent = new Entity { Name = "Parent" };
				var child = new Entity { Name = "Child" };
				using (var t = session.BeginTransaction())
				{
					session.Persist(parent);
					session.Flush();
					parent.Children.Add(child);
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(1));
				Assert.That(parent.Children, Does.Contain(child));
				Assert.That(parent.Children.Single().Id, Is.Not.EqualTo(0));
			}
		}

		[Test]
		public void CanAddChildAfterFlushWithoutTransaction()
		{
			using (var session = OpenSession())
			{
				var parent = new Entity { Name = "Parent" };
				var child = new Entity { Name = "Child" };
				session.Persist(parent);
				session.Flush();
				parent.Children.Add(child);
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(1));
				Assert.That(parent.Children, Does.Contain(child));
				Assert.That(parent.Children.Single().Id, Is.Not.EqualTo(0));
			}
		}

		// https://hibernate.atlassian.net/browse/HHH-12846
		[Test]
		public void CanMergeWithTransientChild()
		{
			using (var session = OpenSession())
			{
				var parent = new Entity { Name = "Parent" };
				using (var t = session.BeginTransaction())
				{
					session.Persist(parent);
					t.Commit();
				}

				var child = new Entity { Name = "Child" };
				using (var t = session.BeginTransaction())
				{
					parent.Children.Add(child);
					session.Merge(parent);
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(1));
				// Merge should duplicate child and leave original instance un-associated with the session.
				Assert.That(parent.Children, Does.Not.Contain(child));
				Assert.That(parent.Children.Single().Id, Is.Not.EqualTo(0));
			}
		}

		[Test]
		public void CanMergeWithTransientChildWithoutTransaction()
		{
			using (var session = OpenSession())
			{
				var parent = new Entity { Name = "Parent" };
				session.Persist(parent);
				session.Flush();

				var child = new Entity { Name = "Child" };
				parent.Children.Add(child);
				session.Merge(parent);
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(1));
				// Merge should duplicate child and leave original instance un-associated with the session.
				Assert.That(parent.Children, Does.Not.Contain(child));
				Assert.That(parent.Children.Single().Id, Is.Not.EqualTo(0));
			}
		}
	}
}
