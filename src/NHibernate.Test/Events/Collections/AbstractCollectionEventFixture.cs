using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Event;
using NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections
{
	public abstract class AbstractCollectionEventFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		public abstract IParentWithCollection CreateParent(string name);

		public abstract ICollection<IChild> CreateCollection();

		protected override void OnTearDown()
		{
			IParentWithCollection dummyParent = CreateParent("dummyParent");
			dummyParent.NewChildren(CreateCollection());
			IChild dummyChild = dummyParent.AddChild("dummyChild");

			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IList children = s.CreateCriteria(dummyChild.GetType()).List();
					IList parents = s.CreateCriteria(dummyParent.GetType()).List();
					foreach (IParentWithCollection parent in parents)
					{
						parent.ClearChildren();
						s.Delete(parent);
					}
					foreach (IChild child in children)
					{
						s.Delete(child);
					}

					tx.Commit();
				}
			}
			base.OnTearDown();
		}

		[Test]
		public void SaveParentEmptyChildren()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNoChildren("parent");
			Assert.That(parent.Children.Count, Is.EqualTo(0));
			int index = 0;
			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			CheckNumberOfResults(listeners, index);
			listeners.Clear();
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
					tx.Commit();
				}
			}
			Assert.That(parent.Children, Is.Not.Null);
			CheckNumberOfResults(listeners, 0);
		}

		[Test]
		public virtual void SaveParentOneChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			int index = 0;
			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			ChildWithBidirectionalManyToMany child = GetFirstChild(parent.Children) as ChildWithBidirectionalManyToMany;
			if (child != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, child, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, child, index++);
			}

			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentNullToOneChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNullChildren("parent");
			listeners.Clear();
			Assert.That(parent.Children, Is.Null);
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			Assert.That(parent.Children, Is.Not.Null);
			ChildWithBidirectionalManyToMany newChild = parent.AddChild("new") as ChildWithBidirectionalManyToMany;
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (newChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, newChild, index++);
			}

			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentNoneToOneChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNoChildren("parent");
			listeners.Clear();
			Assert.That(parent.Children.Count, Is.EqualTo(0));
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			ChildWithBidirectionalManyToMany newChild = parent.AddChild("new") as ChildWithBidirectionalManyToMany;
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (newChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, newChild, index++);
			}

			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentOneToTwoChildren()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			ChildWithBidirectionalManyToMany newChild = parent.AddChild("new2") as ChildWithBidirectionalManyToMany;
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (newChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, newChild, index++);
			}

			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public virtual void UpdateParentOneToTwoSameChildren()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IChild child = GetFirstChild(parent.Children);
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			listeners.Clear();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
				IEntity e = child as IEntity;
				if (e != null)
				{
					child = (IChild) s.Get(child.GetType(), e.Id);
				}
				parent.AddChild(child);
				tx.Commit();
			}
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				if (((IPersistentCollection) childWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
				}
			}

			if (!(parent.Children is PersistentGenericSet<IChild>))
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			}
			if (childWithManyToMany != null && !(childWithManyToMany.Parents is PersistentGenericSet<ParentWithBidirectionalManyToMany>))
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}

			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentNullToOneChildDiffCollection()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNullChildren("parent");
			listeners.Clear();
			Assert.That(parent.Children, Is.Null);
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			ICollection<IChild> collectionOrig = parent.Children;
			parent.NewChildren(CreateCollection());
			ChildWithBidirectionalManyToMany newChild = parent.AddChild("new") as ChildWithBidirectionalManyToMany;
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) collectionOrig).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, collectionOrig, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, collectionOrig, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, collectionOrig, index++);
			if (newChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, newChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentNoneToOneChildDiffCollection()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNoChildren("parent");
			listeners.Clear();
			Assert.That(parent.Children.Count, Is.EqualTo(0));
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			ICollection<IChild> oldCollection = parent.Children;
			parent.NewChildren(CreateCollection());
			ChildWithBidirectionalManyToMany newChild = parent.AddChild("new") as ChildWithBidirectionalManyToMany;
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) oldCollection).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, oldCollection, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, oldCollection, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, oldCollection, index++);
			if (newChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionRecreate, newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, newChild, index++);
			}

			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentOneChildDiffCollectionSameChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = child as IEntity;
			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			ICollection<IChild> oldCollection = parent.Children;
			parent.NewChildren(CreateCollection());
			parent.AddChild(child);
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) oldCollection).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, oldCollection, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				if (((IPersistentCollection) childWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
				}
			}

			CheckResult(listeners, listeners.PreCollectionRemove, parent, oldCollection, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, oldCollection, index++);
			if (childWithManyToMany != null)
			{
				// hmmm, the same parent was removed and re-added to the child's collection;
				// should this be considered an update?
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentOneChildDiffCollectionDiffChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IChild oldChild = GetFirstChild(parent.Children);
			listeners.Clear();
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = oldChild as IEntity;
			ChildWithBidirectionalManyToMany oldChildWithManyToMany = null;
			if (e != null)
			{
				oldChildWithManyToMany = s.Get(oldChild.GetType(), e.Id) as ChildWithBidirectionalManyToMany;
			}
			ICollection<IChild> oldCollection = parent.Children;
			parent.NewChildren(CreateCollection());
			IChild newChild = parent.AddChild("new1");
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) oldCollection).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, oldCollection, index++);
			}
			if (oldChildWithManyToMany != null)
			{
				if (((IPersistentCollection) oldChildWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, oldChildWithManyToMany, index++);
				}
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, oldCollection, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, oldCollection, index++);
			if (oldChildWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, oldChildWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, oldChildWithManyToMany, index++);
				CheckResult(listeners, listeners.PreCollectionRecreate, (ChildWithBidirectionalManyToMany) newChild, index++);
				CheckResult(listeners, listeners.PostCollectionRecreate, (ChildWithBidirectionalManyToMany) newChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRecreate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentOneChildToNoneByRemove()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = child as IEntity;

			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			parent.RemoveChild(child);
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				if (((IPersistentCollection) childWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
				}
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentOneChildToNoneByClear()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = child as IEntity;
			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			parent.ClearChildren();
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				if (((IPersistentCollection) childWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
				}
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void UpdateParentTwoChildrenToOne()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			Assert.That(parent.Children.Count, Is.EqualTo(1));
			IChild oldChild = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			parent.AddChild("new");
			tx.Commit();
			s.Close();
			listeners.Clear();
			s = OpenSession();
			tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = oldChild as IEntity;
			if (e != null)
			{
				oldChild = (IChild) s.Get(oldChild.GetType(), e.Id);
			}

			parent.RemoveChild(oldChild);
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			ChildWithBidirectionalManyToMany oldChildWithManyToMany = oldChild as ChildWithBidirectionalManyToMany;
			if (oldChildWithManyToMany != null)
			{
				if (((IPersistentCollection) oldChildWithManyToMany.Parents).WasInitialized)
				{
					CheckResult(listeners, listeners.InitializeCollection, oldChildWithManyToMany, index++);
				}
			}

			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			if (oldChildWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, oldChildWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, oldChildWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void DeleteParentWithNullChildren()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNullChildren("parent");
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			s.Delete(parent);
			tx.Commit();
			s.Close();
			int index = 0;
			CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			CheckResult(listeners, listeners.PreCollectionRemove, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void DeleteParentWithNoChildren()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithNoChildren("parent");
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			s.Delete(parent);
			tx.Commit();
			s.Close();

			int index = 0;
			CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			CheckResult(listeners, listeners.PreCollectionRemove, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, index++);
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void DeleteParentAndChild()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			IEntity e = child as IEntity;
			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			parent.RemoveChild(child);
			if (e != null)
			{
				s.Delete(child);
			}
			s.Delete(parent);
			tx.Commit();
			s.Close();
			int index = 0;
			CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, index++);
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionRemove, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionRemove, childWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void MoveChildToDifferentParent()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IParentWithCollection otherParent = CreateParentWithOneChild("otherParent", "otherChild");
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			otherParent = (IParentWithCollection) s.Get(otherParent.GetType(), otherParent.Id);
			IEntity e = child as IEntity;
			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			parent.RemoveChild(child);
			otherParent.AddChild(child);
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
			}
			if (((IPersistentCollection) otherParent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherParent, index++);
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PreCollectionUpdate, otherParent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, otherParent, index++);
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void MoveAllChildrenToDifferentParent()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IParentWithCollection otherParent = CreateParentWithOneChild("otherParent", "otherChild");
			IChild child = GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			otherParent = (IParentWithCollection) s.Get(otherParent.GetType(), otherParent.Id);
			IEntity e = child as IEntity;
			if (e != null)
			{
				child = (IChild) s.Get(child.GetType(), e.Id);
			}
			otherParent.AddAllChildren(parent.Children);
			parent.ClearChildren();
			tx.Commit();
			s.Close();
			int index = 0;
			if (((IPersistentCollection) parent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			}
			if (((IPersistentCollection) otherParent.Children).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherParent, index++);
			}
			ChildWithBidirectionalManyToMany childWithManyToMany = child as ChildWithBidirectionalManyToMany;
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.InitializeCollection, childWithManyToMany, index++);
			}
			CheckResult(listeners, listeners.PreCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, parent, index++);
			CheckResult(listeners, listeners.PreCollectionUpdate, otherParent, index++);
			CheckResult(listeners, listeners.PostCollectionUpdate, otherParent, index++);
			if (childWithManyToMany != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, childWithManyToMany, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, childWithManyToMany, index++);
			}
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void MoveCollectionToDifferentParent()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IParentWithCollection otherParent = CreateParentWithOneChild("otherParent", "otherChild");
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			otherParent = (IParentWithCollection) s.Get(otherParent.GetType(), otherParent.Id);
			ICollection<IChild> otherCollectionOrig = otherParent.Children;
			otherParent.NewChildren(parent.Children);
			parent.NewChildren(null);
			tx.Commit();
			s.Close();
			int index = 0;
			ChildWithBidirectionalManyToMany otherChildOrig = null;
			if (((IPersistentCollection) otherCollectionOrig).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherParent, otherCollectionOrig, index++);
				otherChildOrig = GetFirstChild(otherCollectionOrig) as ChildWithBidirectionalManyToMany;
				if (otherChildOrig != null)
				{
					CheckResult(listeners, listeners.InitializeCollection, otherChildOrig, index++);
				}
			}
			CheckResult(listeners, listeners.InitializeCollection, parent, otherParent.Children, index++);
			ChildWithBidirectionalManyToMany otherChild = GetFirstChild(otherParent.Children) as ChildWithBidirectionalManyToMany;
			if (otherChild != null)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, otherParent.Children, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, otherParent.Children, index++);
			CheckResult(listeners, listeners.PreCollectionRemove, otherParent, otherCollectionOrig, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, otherParent, otherCollectionOrig, index++);
			if (otherChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, otherChildOrig, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, otherChildOrig, index++);
				CheckResult(listeners, listeners.PreCollectionUpdate, otherChild, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, otherChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRecreate, otherParent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, otherParent, index++);
			// there should also be pre- and post-recreate collection events for parent, but thats broken now;
			// this is covered in BrokenCollectionEventTest
			CheckNumberOfResults(listeners, index);
		}

		[Test]
		public void MoveCollectionToDifferentParentFlushMoveToDifferentParent()
		{
			CollectionListeners listeners = new CollectionListeners(Sfi);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			IParentWithCollection otherParent = CreateParentWithOneChild("otherParent", "otherChild");
			IParentWithCollection otherOtherParent = CreateParentWithNoChildren("otherParent");
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			otherParent = (IParentWithCollection) s.Get(otherParent.GetType(), otherParent.Id);
			otherOtherParent = (IParentWithCollection) s.Get(otherOtherParent.GetType(), otherOtherParent.Id);
			ICollection<IChild> otherCollectionOrig = otherParent.Children;
			ICollection<IChild> otherOtherCollectionOrig = otherOtherParent.Children;
			otherParent.NewChildren(parent.Children);
			parent.NewChildren(null);
			s.Flush();
			otherOtherParent.NewChildren(otherParent.Children);
			otherParent.NewChildren(null);
			tx.Commit();
			s.Close();
			int index = 0;
			ChildWithBidirectionalManyToMany otherChildOrig = null;
			if (((IPersistentCollection) otherCollectionOrig).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherParent, otherCollectionOrig, index++);
				otherChildOrig = GetFirstChild(otherCollectionOrig) as ChildWithBidirectionalManyToMany;
				if (otherChildOrig != null)
				{
					CheckResult(listeners, listeners.InitializeCollection, otherChildOrig, index++);
				}
			}
			CheckResult(listeners, listeners.InitializeCollection, parent, otherOtherParent.Children, index++);
			ChildWithBidirectionalManyToMany otherOtherChild =
				GetFirstChild(otherOtherParent.Children) as ChildWithBidirectionalManyToMany;
			if (otherOtherChild != null)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherOtherChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, otherOtherParent.Children, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, otherOtherParent.Children, index++);
			CheckResult(listeners, listeners.PreCollectionRemove, otherParent, otherCollectionOrig, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, otherParent, otherCollectionOrig, index++);
			if (otherOtherChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, otherChildOrig, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, otherChildOrig, index++);
				CheckResult(listeners, listeners.PreCollectionUpdate, otherOtherChild, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, otherOtherChild, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRecreate, otherParent, otherOtherParent.Children, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, otherParent, otherOtherParent.Children, index++);
			if (((IPersistentCollection) otherOtherCollectionOrig).WasInitialized)
			{
				CheckResult(listeners, listeners.InitializeCollection, otherOtherParent, otherOtherCollectionOrig, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, otherParent, otherOtherParent.Children, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, otherParent, otherOtherParent.Children, index++);
			CheckResult(listeners, listeners.PreCollectionRemove, otherOtherParent, otherOtherCollectionOrig, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, otherOtherParent, otherOtherCollectionOrig, index++);
			if (otherOtherChild != null)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, otherOtherChild, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, otherOtherChild, index++);
			}

			CheckResult(listeners, listeners.PreCollectionRecreate, otherOtherParent, index++);
			CheckResult(listeners, listeners.PostCollectionRecreate, otherOtherParent, index++);
			// there should also be pre- and post-recreate collection events for parent, and otherParent
			// but thats broken now; this is covered in BrokenCollectionEventTest
			CheckNumberOfResults(listeners, index);
		}

		protected IChild GetFirstChild(ICollection<IChild> children)
		{
			IChild result = null;
			IEnumerator<IChild> en = children.GetEnumerator();
			if (en.MoveNext())
			{
				result = en.Current;
			}
			return result;
		}

		protected IParentWithCollection CreateParentWithNullChildren(string parentName)
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IParentWithCollection parent = CreateParent(parentName);
					s.Save(parent);
					tx.Commit();
					return parent;
				}
			}
		}

		protected IParentWithCollection CreateParentWithNoChildren(string parentName)
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IParentWithCollection parent = CreateParent(parentName);
					parent.NewChildren(CreateCollection());
					s.Save(parent);
					tx.Commit();
					return parent;
				}
			}
		}

		protected IParentWithCollection CreateParentWithOneChild(string parentName, string ChildName)
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IParentWithCollection parent = CreateParent(parentName);
					parent.NewChildren(CreateCollection());
					parent.AddChild(ChildName);
					s.Save(parent);
					tx.Commit();
					return parent;
				}
			}
		}

		protected void CheckResult(CollectionListeners listeners, CollectionListeners.IListener listenerExpected,
		                           IParentWithCollection parent, int index)
		{
			CheckResult(listeners, listenerExpected, parent, parent.Children, index);
		}

		protected void CheckResult(CollectionListeners listeners, CollectionListeners.IListener listenerExpected,
		                           IEntity ownerExpected, object collExpected, int index)
		{
			Assert.That(listeners.ListenersCalled[index], Is.SameAs(listenerExpected));
			Assert.That(listeners.Events[index].AffectedOwnerOrNull, Is.SameAs(ownerExpected));
			Assert.That(listeners.Events[index].AffectedOwnerIdOrNull, Is.EqualTo(ownerExpected.Id));
			Assert.That(listeners.Events[index].GetAffectedOwnerEntityName(),
			            Is.EqualTo(ownerExpected.GetType().FullName));
			Assert.That(listeners.Events[index].Collection, Is.SameAs(collExpected));
		}

		protected void CheckNumberOfResults(CollectionListeners listeners, int nEventsExpected)
		{
			Assert.That(listeners.ListenersCalled.Count, Is.EqualTo(nEventsExpected));
			Assert.That(listeners.Events.Count, Is.EqualTo(nEventsExpected));
		}

		protected void CheckResult(CollectionListeners listeners, CollectionListeners.IListener listenerExpected,
		                           ChildWithBidirectionalManyToMany child, int index)
		{
			CheckResult(listeners, listenerExpected, child, child.Parents, index);
		}
	}
}