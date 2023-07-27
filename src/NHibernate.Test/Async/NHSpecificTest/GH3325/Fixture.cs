﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3325
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
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
		public async Task CanRemoveChildAfterSaveAsync()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			parent.Children.Add(child);
			await (session.SaveAsync(parent));
			parent.Children.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			await (t.CommitAsync());

			await (AssertParentIsChildlessAsync(parentId, childId));
		}

		[Test]
		public async Task CanRemoveChildFromUnwrappedCollectionAfterSaveAsync()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			var parentChildren = parent.Children;
			parentChildren.Add(child);
			await (session.SaveAsync(parent));
			parentChildren.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			await (t.CommitAsync());

			await (AssertParentIsChildlessAsync(parentId, childId));
		}

		[Test]
		public async Task CanRemoveChildAfterSaveAndExplicitFlushAsync()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			parent.Children.Add(child);
			await (session.SaveAsync(parent));
			await (session.FlushAsync());
			parent.Children.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			await (t.CommitAsync());

			await (AssertParentIsChildlessAsync(parentId, childId));
		}

		[Test]
		public async Task CanRemoveChildFromUnwrappedCollectionAfterSaveAndExplicitFlushAsync()
		{
			using var session = OpenSession();
			using var t = session.BeginTransaction();

			var parent = new Entity { Name = "Parent" };
			var child = new ChildEntity { Name = "Child" };
			var parentChildren = parent.Children;
			parentChildren.Add(child);
			await (session.SaveAsync(parent));
			await (session.FlushAsync());
			parentChildren.Remove(child);
			var parentId = parent.Id;
			var childId = child.Id;
			await (t.CommitAsync());

			await (AssertParentIsChildlessAsync(parentId, childId));
		}

		private async Task AssertParentIsChildlessAsync(Guid parentId, Guid childId, CancellationToken cancellationToken = default(CancellationToken))
		{
			using var session = OpenSession();

			var parent = await (session.GetAsync<Entity>(parentId, cancellationToken));
			Assert.That(parent, Is.Not.Null);
			Assert.That(parent.Children, Has.Count.EqualTo(0));

			var child = await (session.GetAsync<ChildEntity>(childId, cancellationToken));
			Assert.That(child, Is.Null);
		}
	}
}
