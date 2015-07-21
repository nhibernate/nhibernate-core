using NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Test.Events.Collections.Association
{
	public abstract class AbstractAssociationCollectionEventFixture : AbstractCollectionEventFixture
	{
		[Test]
		public void DeleteParentButNotChild()
		{
			CollectionListeners listeners = new CollectionListeners(sessions);
			IParentWithCollection parent = CreateParentWithOneChild("parent", "child");
			ChildEntity child = (ChildEntity) GetFirstChild(parent.Children);
			listeners.Clear();
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			parent = (IParentWithCollection) s.Get(parent.GetType(), parent.Id);
			child = (ChildEntity) s.Get(child.GetType(), child.Id);
			parent.RemoveChild(child);
			s.Delete(parent);
			tx.Commit();
			s.Close();
			int index = 0;
			CheckResult(listeners, listeners.InitializeCollection, parent, index++);
			if (child is ChildWithBidirectionalManyToMany)
			{
				CheckResult(listeners, listeners.InitializeCollection, (ChildWithBidirectionalManyToMany) child, index++);
			}
			CheckResult(listeners, listeners.PreCollectionRemove, parent, index++);
			CheckResult(listeners, listeners.PostCollectionRemove, parent, index++);
			if (child is ChildWithBidirectionalManyToMany)
			{
				CheckResult(listeners, listeners.PreCollectionUpdate, (ChildWithBidirectionalManyToMany) child, index++);
				CheckResult(listeners, listeners.PostCollectionUpdate, (ChildWithBidirectionalManyToMany) child, index++);
			}
			CheckNumberOfResults(listeners, index);
		}
	}
}