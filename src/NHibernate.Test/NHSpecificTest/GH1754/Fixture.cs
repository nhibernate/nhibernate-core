using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1754
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		// Disable second level cache
		protected override string CacheConcurrencyStrategy => null;

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
				session.CreateQuery("delete from ChildEntity").ExecuteUpdate();
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
				session.Persist(new Entity { Name = "Test" });

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
				session.Persist(new Entity { Name = "Test" });
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
				var child = new ChildEntity { Name = "Child" };
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
				var child = new ChildEntity { Name = "Child" };
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

				var child = new ChildEntity { Name = "Child" };
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

				var child = new ChildEntity { Name = "Child" };
				parent.Children.Add(child);
				session.Merge(parent);
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(1));
				// Merge should duplicate child and leave original instance un-associated with the session.
				Assert.That(parent.Children, Does.Not.Contain(child));
				Assert.That(parent.Children.Single().Id, Is.Not.EqualTo(0));
			}
		}

		[Test]
		public void CanChangeOwnershipOnFlushedParents()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				using (var t = session.BeginTransaction())
				{
					session.Persist(parent);
					session.Persist(nextParent);
					parent.Children.Add(child);
					session.Flush();
					nextParent.Children = parent.Children;
					parent.Children = new HashSet<ChildEntity>();
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipOnFlushedParentsWithoutTransaction()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				session.Persist(parent);
				session.Persist(nextParent);
				parent.Children.Add(child);
				session.Flush();
				nextParent.Children = parent.Children;
				parent.Children = new HashSet<ChildEntity>();
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipFromFlushedParentToNonFlushed()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				using (var t = session.BeginTransaction())
				{
					session.Persist(parent);
					parent.Children.Add(child);
					session.Flush();
					session.Persist(nextParent);
					nextParent.Children = parent.Children;
					parent.Children = new HashSet<ChildEntity>();
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipFromFlushedParentToNonFlushedWithoutTransaction()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				session.Persist(parent);
				parent.Children.Add(child);
				session.Flush();
				session.Persist(nextParent);
				nextParent.Children = parent.Children;
				parent.Children = new HashSet<ChildEntity>();
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipFromNonFlushedParentToFlushed()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				using (var t = session.BeginTransaction())
				{
					session.Persist(nextParent);
					session.Flush();
					session.Persist(parent);
					parent.Children.Add(child);
					nextParent.Children = parent.Children;
					parent.Children = new HashSet<ChildEntity>();
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipFromNonFlushedParentToFlushedWithoutTransaction()
		{
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				session.Persist(nextParent);
				session.Flush();
				session.Persist(parent);
				parent.Children.Add(child);
				nextParent.Children = parent.Children;
				parent.Children = new HashSet<ChildEntity>();
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipOnNonFlushedParents()
		{
			// Seems moot but why not still checking this?
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				using (var t = session.BeginTransaction())
				{
					session.Persist(parent);
					parent.Children.Add(child);
					session.Persist(nextParent);
					nextParent.Children = parent.Children;
					parent.Children = new HashSet<ChildEntity>();
					t.Commit();
				}

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}

		[Test]
		public void CanChangeOwnershipOnNonFlushedParentsWithoutTransaction()
		{
			// Seems moot but why not still checking this?
			var parent = new EntityWithoutDeleteOrphan { Name = "Parent" };
			var nextParent = new EntityWithoutDeleteOrphan { Name = "NextParent" };
			var child = new ChildEntity { Name = "Child" };
			using (var session = OpenSession())
			{
				session.Persist(parent);
				parent.Children.Add(child);
				session.Persist(nextParent);
				nextParent.Children = parent.Children;
				parent.Children = new HashSet<ChildEntity>();
				session.Flush();

				Assert.That(parent.Children, Has.Count.EqualTo(0));
				Assert.That(nextParent.Children, Has.Count.EqualTo(1));
				Assert.That(nextParent.Children, Does.Contain(child));
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0));
			}

			using (var session = OpenSession())
			{
				// Check after a reload
				parent = session.Load<EntityWithoutDeleteOrphan>(parent.Id);
				nextParent = session.Load<EntityWithoutDeleteOrphan>(nextParent.Id);
				child = session.Load<ChildEntity>(child.Id);

				Assert.That(parent.Children, Has.Count.EqualTo(0), "Reloaded data");
				Assert.That(nextParent.Children, Has.Count.EqualTo(1), "Reloaded data");
				Assert.That(nextParent.Children, Does.Contain(child), "Reloaded data");
				Assert.That(nextParent.Children.Single().Id, Is.Not.EqualTo(0), "Reloaded data");
			}
		}
	}
}
