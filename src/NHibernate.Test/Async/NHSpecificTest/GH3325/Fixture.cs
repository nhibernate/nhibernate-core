﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3325
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from ChildEntity").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public async Task CanAddChildAfterFlushAsync()
		{
			Guid parentId;
			Guid childId;
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var parent = new Entity { Name = "Parent" };
				var child = new ChildEntity { Name = "Child" };
				var parentChildren = parent.Children;
				parentChildren.Add(child);
				await (session.SaveAsync(parent));
				parentChildren.Remove(child);
				parentId = parent.Id;
				childId = child.Id;
				await (t.CommitAsync());
			}

			using (var session = OpenSession())
			{
				var parent = await (session.GetAsync<Entity>(parentId));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children, Has.Count.EqualTo(0));

				var child = await (session.GetAsync<ChildEntity>(childId));
				Assert.That(child, Is.Null);
			}
		}
	}
}
