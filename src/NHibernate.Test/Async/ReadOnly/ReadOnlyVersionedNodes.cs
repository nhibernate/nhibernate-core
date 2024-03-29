﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	using System.Threading.Tasks;
	[TestFixture]
	public class ReadOnlyVersionedNodesAsync : AbstractReadOnlyTest
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		protected override string[] Mappings
		{
			get { return new string[] { "ReadOnly.VersionedNode.hbm.xml" }; }
		}
	
		[Test]
		public async Task SetReadOnlyTrueAndFalseAsync()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(node));
				await (t.CommitAsync());
			}

			ClearCounts();
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					node = await (s.GetAsync<VersionedNode>(node.Id));
					s.SetReadOnly(node, true);
					node.Name = "node-name";
					await (t.CommitAsync());
				}

				AssertUpdateCount(0);
				AssertInsertCount(0);

				// the changed name is still in node
				Assert.That(node.Name, Is.EqualTo("node-name"));

				using (var t = s.BeginTransaction())
				{
					node = await (s.GetAsync<VersionedNode>(node.Id));
					// the changed name is still in the session
					Assert.That(node.Name, Is.EqualTo("node-name"));
					await (s.RefreshAsync(node));
					// after refresh, the name reverts to the original value
					Assert.That(node.Name, Is.EqualTo("node"));
					node = await (s.GetAsync<VersionedNode>(node.Id));
					Assert.That(node.Name, Is.EqualTo("node"));
					await (t.CommitAsync());
				}
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				Assert.That(node.Name, Is.EqualTo("node"));
				s.SetReadOnly(node, true);
				node.Name = "diff-node-name";
				await (s.FlushAsync());
				Assert.That(node.Name, Is.EqualTo("diff-node-name"));
				await (s.RefreshAsync(node));
				Assert.That(node.Name, Is.EqualTo("node"));
				s.SetReadOnly(node, false);
				node.Name = "diff-node-name";
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				Assert.That(node.Name, Is.EqualTo("diff-node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				s.SetReadOnly(node, true);
				await (s.DeleteAsync(node));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public async Task UpdateSetReadOnlyTwiceAsync()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(node));
				await (t.CommitAsync());
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				node.Name = "node-name";
				s.SetReadOnly(node, true);
				s.SetReadOnly(node, true);
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				Assert.That(node.Name, Is.EqualTo("node"));
				Assert.That(node.Version, Is.EqualTo(1));
				s.SetReadOnly(node, true);
				await (s.DeleteAsync(node));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public async Task UpdateSetModifiableAsync()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(node));
				await (t.CommitAsync());
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				node.Name = "node-name";
				s.SetReadOnly(node, false);
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = await (s.GetAsync<VersionedNode>(node.Id));
				Assert.That(node.Name, Is.EqualTo("node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				//s.SetReadOnly(node, true);
				await (s.DeleteAsync(node));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public async Task AddNewChildToReadOnlyParentAsync()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				await (s.PersistAsync(parent));
				await (t.CommitAsync());
			}

			ClearCounts();

			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				VersionedNode parentManaged = await (s.GetAsync<VersionedNode>(parent.Id));
				s.SetReadOnly(parentManaged, true);
				parentManaged.Name = "new parent name";
				parentManaged.AddChild(child);
				await (s.SaveAsync(parentManaged));
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(child, Is.Not.Null);
				await (s.DeleteAsync(parent));
				await (t.CommitAsync());
			}
		}
	
		[Test]
		public async Task UpdateParentWithNewChildCommitWithReadOnlyParentAsync()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(parent));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.UpdateAsync(parent));
				s.SetReadOnly(parent, true);
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public async Task MergeDetachedParentWithNewChildCommitWithReadOnlyParentAsync()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(parent));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = (VersionedNode) await (s.MergeAsync(parent));
				s.SetReadOnly(parent, true);
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}

		[Test]
		public async Task GetParentMakeReadOnlyThenMergeDetachedParentWithNewChildCAsync()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(parent));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentManaged = await (s.GetAsync<VersionedNode>(parent.Id));
				s.SetReadOnly(parentManaged, true);
				VersionedNode parentMerged = (VersionedNode) await (s.MergeAsync(parent));
				Assert.That(parentManaged, Is.SameAs(parentMerged));
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public async Task MergeUnchangedDetachedParentChildrenAsync()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent.AddChild(child);
				await (s.PersistAsync(parent));
				await (t.CommitAsync());
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = (VersionedNode) await (s.MergeAsync(parent));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentGet = await (s.GetAsync<VersionedNode>(parent.Id));
				await (s.MergeAsync(parent));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentLoad = await (s.LoadAsync<VersionedNode>(parent.Id));
				await (s.MergeAsync(parent));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(1));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public async Task AddNewParentToReadOnlyChildAsync()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(child));
				await (t.CommitAsync());
			}

			ClearCounts();

			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode childManaged = await (s.GetAsync<VersionedNode>(child.Id));
				s.SetReadOnly(childManaged, true);
				childManaged.Name = "new child name";
				parent.AddChild(childManaged);
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(1);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				Assert.That(parent, Is.Not.Null);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public async Task UpdateChildWithNewParentCommitWithReadOnlyChildAsync()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(child));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.UpdateAsync(child));
				s.SetReadOnly(child, true);
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public async Task MergeDetachedChildWithNewParentCommitWithReadOnlyChildAsync()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(child));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				child = (VersionedNode) await (s.MergeAsync(child));
				s.SetReadOnly(child, true);
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(2));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public async Task GetChildMakeReadOnlyThenMergeDetachedChildWithNewParentAsync()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.PersistAsync(child));
				await (t.CommitAsync());
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode childManaged = await (s.GetAsync<VersionedNode>(child.Id));
				s.SetReadOnly(childManaged, true);
				VersionedNode childMerged = (VersionedNode) await (s.MergeAsync(child));
				Assert.That(childManaged, Is.SameAs(childMerged));
				await (t.CommitAsync());
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = await (s.GetAsync<VersionedNode>(parent.Id));
				child = await (s.GetAsync<VersionedNode>(child.Id));
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(2));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				await (s.DeleteAsync(parent));
				await (s.DeleteAsync(child));
				await (t.CommitAsync());
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from VersionedNode where parent is not null").ExecuteUpdate();
				s.CreateQuery("delete from VersionedNode").ExecuteUpdate();

				t.Commit();
			}

			base.OnTearDown();
		}
	}
}
