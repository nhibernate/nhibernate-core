using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3325
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from ChildEntity").ExecuteUpdate();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void CanRemoveChildAfterSave()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			parent.Children.Add(child);
			session.Save(parent);
			parent.Children.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			t.Commit();

			AssertParentIsChildless(parentId, childId);
		}

		[Test]
		public void CanRemoveChildFromUnwrappedCollectionAfterSave()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			var parentChildren = parent.Children;
			parentChildren.Add(child);
			session.Save(parent);
			parentChildren.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			t.Commit();

			AssertParentIsChildless(parentId, childId);
		}

		[Test]
		public void CanRemoveChildAfterSaveAndExplicitFlush()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			parent.Children.Add(child);
			session.Save(parent);
			session.Flush();
			parent.Children.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			t.Commit();

			AssertParentIsChildless(parentId, childId);
		}

		[Test]
		public void CanRemoveChildFromUnwrappedCollectionAfterSaveAndExplicitFlush()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			var parentChildren = parent.Children;
			parentChildren.Add(child);
			session.Save(parent);
			session.Flush();
			parentChildren.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			t.Commit();

			AssertParentIsChildless(parentId, childId);
		}

		private void AssertParentIsChildless(Guid parentId, Guid childId)
		{
			using var session = OpenSession();

			var parent = session.Get<Entity>(parentId);
			Assert.That(parent, Is.Not.Null);
			Assert.That(parent.Children, Has.Count.EqualTo(0));

			var child = session.Get<ChildEntity>(childId);
			Assert.That(child, Is.Null);
		}
	}
}
