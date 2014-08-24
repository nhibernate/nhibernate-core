using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
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
		
		protected override IList Mappings
		{
			get { return new string[] { "ReadOnly.VersionedNode.hbm.xml" }; }
		}
	
		[Test]
		public void SetReadOnlyTrueAndFalse()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode node = new VersionedNode("node", "node");
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			s.SetReadOnly(node, true);
			node.Name = "node-name";
			s.Transaction.Commit();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
	
			// the changed name is still in node
			Assert.That(node.Name, Is.EqualTo("node-name"));
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			// the changed name is still in the session
			Assert.That(node.Name, Is.EqualTo("node-name"));
			s.Refresh(node);
			// after refresh, the name reverts to the original value
			Assert.That(node.Name, Is.EqualTo("node"));
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("node"));
			s.Transaction.Commit();
	
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("diff-node-name"));
			Assert.That(node.Version, Is.EqualTo(2));
			s.SetReadOnly(node, true);
			s.Delete(node);
			s.Transaction.Commit();
			s.Close();
			
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateSetReadOnlyTwice()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode node = new VersionedNode("node", "node");
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();
	 
			ClearCounts();
	
			s = OpenSession();
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			node.Name = "node-name";
			s.SetReadOnly(node, true);
			s.SetReadOnly(node, true);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("node"));
			Assert.That(node.Version, Is.EqualTo(1));
			s.SetReadOnly(node, true);
			s.Delete(node);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateSetModifiable()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode node = new VersionedNode("node", "node");
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();
	 
			ClearCounts();
	
			s = OpenSession();
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			node.Name = "node-name";
			s.SetReadOnly(node, false);
			s.Transaction.Commit();
			s.Close();
			s.Dispose();
	
			AssertUpdateCount(1);
			AssertInsertCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("node-name"));
			Assert.That(node.Version, Is.EqualTo(2));
			//s.SetReadOnly(node, true);
			s.Delete(node);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		[Ignore("Failure expected")]
		public void UpdateSetReadOnlySetModifiableFailureExpected()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode node = new VersionedNode("node", "node");
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			node.Name = "node-name";
			s.SetReadOnly(node, true);
			s.SetReadOnly(node, false);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("node-name"));
			Assert.That(node.Version, Is.EqualTo(2));
			s.Delete(node);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		[Ignore("Failure expected")]
		public void SetReadOnlyUpdateSetModifiableFailureExpected()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode node = new VersionedNode("node", "node");
			s.Persist(node);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
	
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			s.SetReadOnly(node, true);
			node.Name = "node-name";
			s.SetReadOnly(node, false);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(0);
	
			s = OpenSession();
			s.BeginTransaction();
			node = s.Get<VersionedNode>(node.Id);
			Assert.That(node.Name, Is.EqualTo("node-name"));
			Assert.That(node.Version, Is.EqualTo(2));
			s.Delete(node);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void AddNewChildToReadOnlyParent()
		{
			ISession s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			VersionedNode parent = new VersionedNode("parent", "parent");
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			VersionedNode parentManaged = s.Get<VersionedNode>(parent.Id);
			s.SetReadOnly(parentManaged, true);
			parentManaged.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parentManaged.AddChild(child);
			s.Save(parentManaged);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(1);
	
			s = OpenSession();
			s.CacheMode = CacheMode.Ignore;
			s.BeginTransaction();
			parent = s.Get<VersionedNode>(parent.Id);
			Assert.That(parent.Name, Is.EqualTo("parent"));
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			Assert.That(parent.Version, Is.EqualTo(2));
			child = s.Get<VersionedNode>(child.Id);
			Assert.That(child, Is.Not.Null);
			s.Delete(parent);
			s.Transaction.Commit();
			s.Close();
		}
	
		[Test]
		public void UpdateParentWithNewChildCommitWithReadOnlyParent()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode parent = new VersionedNode("parent", "parent");
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			s.Update(parent);
			s.SetReadOnly(parent, true);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeDetachedParentWithNewChildCommitWithReadOnlyParent()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode parent = new VersionedNode("parent", "parent");
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			parent = (VersionedNode)s.Merge(parent);
			s.SetReadOnly(parent, true);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void GetParentMakeReadOnlyThenMergeDetachedParentWithNewChildC()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode parent = new VersionedNode("parent", "parent");
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			parent.Name = "new parent name";
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			VersionedNode parentManaged = s.Get<VersionedNode>(parent.Id);
			s.SetReadOnly(parentManaged, true);
			VersionedNode parentMerged = (VersionedNode) s.Merge(parent);
			Assert.That(parentManaged, Is.SameAs(parentMerged));
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(1);
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeUnchangedDetachedParentChildren()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode parent = new VersionedNode("parent", "parent");
			VersionedNode child = new VersionedNode("child", "child");
			parent.AddChild(child);
			s.Persist(parent);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			parent = (VersionedNode)s.Merge(parent);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			VersionedNode parentGet = s.Get<VersionedNode>(parent.Id);
			s.Merge(parent);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			VersionedNode parentLoad = s.Load<VersionedNode>(parent.Id);
			s.Merge(parent);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(0);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void AddNewParentToReadOnlyChild()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode child = new VersionedNode("child", "child");
			s.Persist(child);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			VersionedNode childManaged = s.Get<VersionedNode>(child.Id);
			s.SetReadOnly(childManaged, true);
			childManaged.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(childManaged);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(1);
	
			s = OpenSession();
			s.BeginTransaction();
			child = s.Get<VersionedNode>(child.Id);
			Assert.That(child.Name, Is.EqualTo("child"));
			Assert.That(child.Parent, Is.Null);
			Assert.That(child.Version, Is.EqualTo(1));
			parent = s.Get<VersionedNode>(parent.Id);
			Assert.That(parent, Is.Not.Null);
			s.SetReadOnly(child, true);
			s.Delete(child);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void UpdateChildWithNewParentCommitWithReadOnlyChild()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode child = new VersionedNode("child", "child");
			s.Persist(child);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			s.Update(child);
			s.SetReadOnly(child, true);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void MergeDetachedChildWithNewParentCommitWithReadOnlyChild()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode child = new VersionedNode("child", "child");
			s.Persist(child);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			child = (VersionedNode)s.Merge(child);
			s.SetReadOnly(child, true);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0); // NH-specific: Hibernate issues a separate UPDATE for the version number
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
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
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void GetChildMakeReadOnlyThenMergeDetachedChildWithNewParent()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			VersionedNode child = new VersionedNode("child", "child");
			s.Persist(child);
			s.Transaction.Commit();
			s.Close();
	
			ClearCounts();
	
			child.Name = "new child name";
			VersionedNode parent = new VersionedNode("parent", "parent");
			parent.AddChild(child);
	
			s = OpenSession();
			s.BeginTransaction();
			VersionedNode childManaged = s.Get<VersionedNode>(child.Id);
			s.SetReadOnly(childManaged, true);
			VersionedNode childMerged = (VersionedNode)s.Merge(child);
			Assert.That(childManaged, Is.SameAs(childMerged));
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0); // NH-specific: Hibernate issues a separate UPDATE for the version number
			AssertInsertCount(1);
			ClearCounts();
	
			s = OpenSession();
			s.BeginTransaction();
			parent = s.Get<VersionedNode>(parent.Id);
			child = s.Get<VersionedNode>(child.Id);
			Assert.That(child.Name, Is.EqualTo("child"));
			Assert.That(child.Parent, Is.Null);
			Assert.That(child.Version, Is.EqualTo(1));
			Assert.That(parent, Is.Not.Null);
			Assert.That(parent.Children.Count, Is.EqualTo(0));
			Assert.That(parent.Version, Is.EqualTo(1)); // NH-specific: Hibernate incorrectly increments version number, NH does not
			s.SetReadOnly(parent, true);
			s.SetReadOnly(child, true);
			s.Delete(parent);
			s.Delete(child);
			s.Transaction.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		protected override void OnTearDown()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
	
			s.CreateQuery("delete from VersionedNode where parent is not null").ExecuteUpdate();
			s.CreateQuery("delete from VersionedNode").ExecuteUpdate();
	
			s.Transaction.Commit();
			s.Close();
			
			base.OnTearDown();
		}
	}
}
