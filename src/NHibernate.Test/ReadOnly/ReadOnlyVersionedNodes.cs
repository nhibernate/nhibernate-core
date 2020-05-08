using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	[TestFixture]
	public class ReadOnlyVersionedNodes : AbstractReadOnlyTest
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
		public void SetReadOnlyTrueAndFalse()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(node);
				t.Commit();
			}

			ClearCounts();
			using (var s = OpenSession())
			{
				using (var t = s.BeginTransaction())
				{
					node = s.Get<VersionedNode>(node.Id);
					s.SetReadOnly(node, true);
					node.Name = "node-name";
					t.Commit();
				}

				AssertUpdateCount(0);
				AssertInsertCount(0);

				// the changed name is still in node
				Assert.That(node.Name, Is.EqualTo("node-name"));

				using (var t = s.BeginTransaction())
				{
					node = s.Get<VersionedNode>(node.Id);
					// the changed name is still in the session
					Assert.That(node.Name, Is.EqualTo("node-name"));
					s.Refresh(node);
					// after refresh, the name reverts to the original value
					Assert.That(node.Name, Is.EqualTo("node"));
					node = s.Get<VersionedNode>(node.Id);
					Assert.That(node.Name, Is.EqualTo("node"));
					t.Commit();
				}
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("node"));
				s.SetReadOnly(node, true);
				node.Name = "diff-node-name";
				s.Flush();
				Assert.That(node.Name, Is.EqualTo("diff-node-name"));
				s.Refresh(node);
				Assert.That(node.Name, Is.EqualTo("node"));
				s.SetReadOnly(node, false);
				node.Name = "diff-node-name";
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("diff-node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				s.SetReadOnly(node, true);
				s.Delete(node);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateSetReadOnlyTwice()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(node);
				t.Commit();
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				node.Name = "node-name";
				s.SetReadOnly(node, true);
				s.SetReadOnly(node, true);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("node"));
				Assert.That(node.Version, Is.EqualTo(1));
				s.SetReadOnly(node, true);
				s.Delete(node);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateSetModifiable()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(node);
				t.Commit();
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				node.Name = "node-name";
				s.SetReadOnly(node, false);
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				//s.SetReadOnly(node, true);
				s.Delete(node);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		[Ignore("Failure expected")]
		public void UpdateSetReadOnlySetModifiableFailureExpected()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(node);
				t.Commit();
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				node.Name = "node-name";
				s.SetReadOnly(node, true);
				s.SetReadOnly(node, false);
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				s.Delete(node);
				t.Commit();
			}
		}
	
		[Test]
		[Ignore("Failure expected")]
		public void SetReadOnlyUpdateSetModifiableFailureExpected()
		{
			VersionedNode node = new VersionedNode("node", "node");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(node);
				t.Commit();
				s.Close();
			}

			ClearCounts();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				s.SetReadOnly(node, true);
				node.Name = "node-name";
				s.SetReadOnly(node, false);
				t.Commit();
				s.Close();
			}

			AssertUpdateCount(1);
			AssertInsertCount(0);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				node = s.Get<VersionedNode>(node.Id);
				Assert.That(node.Name, Is.EqualTo("node-name"));
				Assert.That(node.Version, Is.EqualTo(2));
				s.Delete(node);
				t.Commit();
				s.Close();
			}
		}
	
		[Test]
		public void AddNewChildToReadOnlyParent()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				s.Persist(parent);
				t.Commit();
			}

			ClearCounts();

			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				VersionedNode parentManaged = s.Get<VersionedNode>(parent.Id);
				s.SetReadOnly(parentManaged, true);
				parentManaged.Name = "new parent name";
				parentManaged.AddChild(child);
				s.Save(parentManaged);
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CacheMode = CacheMode.Ignore;
				parent = s.Get<VersionedNode>(parent.Id);
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(child, Is.Not.Null);
				s.Delete(parent);
				t.Commit();
			}
		}
	
		[Test]
		public void UpdateParentWithNewChildCommitWithReadOnlyParent()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(parent);
				t.Commit();
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Update(parent);
				s.SetReadOnly(parent, true);
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeDetachedParentWithNewChildCommitWithReadOnlyParent()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(parent);
				t.Commit();
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = (VersionedNode) s.Merge(parent);
				s.SetReadOnly(parent, true);
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}

		[Test]
		public void GetParentMakeReadOnlyThenMergeDetachedParentWithNewChildC()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(parent);
				t.Commit();
			}

			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentManaged = s.Get<VersionedNode>(parent.Id);
				s.SetReadOnly(parentManaged, true);
				VersionedNode parentMerged = (VersionedNode) s.Merge(parent);
				Assert.That(parentManaged, Is.SameAs(parentMerged));
				t.Commit();
			}

			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(2));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeUnchangedDetachedParentChildren()
		{
			VersionedNode parent = new VersionedNode("parent", "parent");
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent.AddChild(child);
				s.Persist(parent);
				t.Commit();
			}

			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = (VersionedNode) s.Merge(parent);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentGet = s.Get<VersionedNode>(parent.Id);
				s.Merge(parent);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode parentLoad = s.Load<VersionedNode>(parent.Id);
				s.Merge(parent);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(parent.Name, Is.EqualTo("parent"));
				Assert.That(parent.Children.Count, Is.EqualTo(1));
				Assert.That(parent.Version, Is.EqualTo(1));
				Assert.That(child.Parent, Is.SameAs(parent));
				Assert.That(parent.Children.First(), Is.SameAs(child));
				Assert.That(child.Version, Is.EqualTo(1));
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void AddNewParentToReadOnlyChild()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(child);
				t.Commit();
			}

			ClearCounts();

			VersionedNode parent = new VersionedNode("parent", "parent");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode childManaged = s.Get<VersionedNode>(child.Id);
				s.SetReadOnly(childManaged, true);
				childManaged.Name = "new child name";
				parent.AddChild(childManaged);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(1);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				parent = s.Get<VersionedNode>(parent.Id);
				Assert.That(parent, Is.Not.Null);
				s.SetReadOnly(child, true);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateChildWithNewParentCommitWithReadOnlyChild()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(child);
				t.Commit();
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Update(child);
				s.SetReadOnly(child, true);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeDetachedChildWithNewParentCommitWithReadOnlyChild()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(child);
				t.Commit();
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				child = (VersionedNode) s.Merge(child);
				s.SetReadOnly(child, true);
				t.Commit();
			}

			AssertUpdateCount(0); // NH-specific: Hibernate issues a separate UPDATE for the version number
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(1));
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
			}

			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void GetChildMakeReadOnlyThenMergeDetachedChildWithNewParent()
		{
			VersionedNode child = new VersionedNode("child", "child");
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Persist(child);
				t.Commit();
			}

			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				VersionedNode childManaged = s.Get<VersionedNode>(child.Id);
				s.SetReadOnly(childManaged, true);
				VersionedNode childMerged = (VersionedNode) s.Merge(child);
				Assert.That(childManaged, Is.SameAs(childMerged));
				t.Commit();
			}

			AssertUpdateCount(0); // NH-specific: Hibernate issues a separate UPDATE for the version number
			AssertInsertCount(1);
			ClearCounts();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				parent = s.Get<VersionedNode>(parent.Id);
				child = s.Get<VersionedNode>(child.Id);
				Assert.That(child.Name, Is.EqualTo("child"));
				Assert.That(child.Parent, Is.Null);
				Assert.That(child.Version, Is.EqualTo(1));
				Assert.That(parent, Is.Not.Null);
				Assert.That(parent.Children.Count, Is.EqualTo(0));
				Assert.That(parent.Version, Is.EqualTo(1));
					// NH-specific: Hibernate incorrectly increments version number, NH does not
				s.SetReadOnly(parent, true);
				s.SetReadOnly(child, true);
				s.Delete(parent);
				s.Delete(child);
				t.Commit();
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
