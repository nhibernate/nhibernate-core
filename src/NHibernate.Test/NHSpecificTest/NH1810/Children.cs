using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public interface IChildrenBehaviour
	{
		void AddChild(Child child);
	}

	public class ChildrenBehaviour : IChildrenBehaviour
	{
		private readonly ISet<Child> children;

		public ChildrenBehaviour(ISet<Child> children)
		{
			this.children = children;
		}

		public void AddChild(Child child)
		{
			children.Add(child);
		}
	}

	public interface IChildren : IChildrenBehaviour
	{
	}

	// Using a HashSet<Child> instead SortedSet<Child> all work fine.
	public class Children : SortedSet<Child>, IChildren
	{
		private readonly IChildrenBehaviour behaviour;

		public Children()
		{
			behaviour = new ChildrenBehaviour(this);
		}

		public Children(IComparer<Child> comparer) : base(comparer)
		{
			behaviour = new ChildrenBehaviour(this);
		}

		public Children(ICollection<Child> initialValues) : base(initialValues)
		{
			behaviour = new ChildrenBehaviour(this);
		}


		public Children(ICollection<Child> initialValues, IComparer<Child> comparer) : base(initialValues, comparer)
		{
			behaviour = new ChildrenBehaviour(this);
		}

		public void AddChild(Child child)
		{
			behaviour.AddChild(child);
		}
	}

	public class PersistentChildren : PersistentGenericSet<Child>, IChildren
	{
		private readonly IChildrenBehaviour behaviour;

		public PersistentChildren(ISessionImplementor session)
			: base(session)
		{
			behaviour = new ChildrenBehaviour(this);
		}

		public PersistentChildren(ISessionImplementor session, ISet<Child> collection)
			: base(session, collection)
		{
			behaviour = new ChildrenBehaviour(this);
		}

		public void AddChild(Child child)
		{
			behaviour.AddChild(child);
		}
	}

	public class Factory : SetFactory<Child>
	{
		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentChildren(session);
		}

		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentChildren(session, (ISet<Child>)collection);
		}

		protected override object Instantiate()
		{
			return new Children();
		}
	}

	public class SetFactory<T> : IUserCollectionType
	{
		public virtual IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentGenericSet<T>(session);
		}

		public virtual IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentGenericSet<T>(session, (ISet<T>)collection);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable)collection;
		}

		public bool Contains(object collection, object entity)
		{
			return ((ISet<T>)collection).Contains((T)entity);
		}

		public object IndexOf(object collection, object entity)
		{
			return new List<T>((ISet<T>)collection).IndexOf((T)entity);
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			var result = (ISet<T>)target;
			result.Clear();

			foreach (var o in (IEnumerable)original)
				result.Add((T)o);

			return result;
		}

		public object Instantiate(int anticipatedSize)
		{
			return Instantiate();
		}

		protected virtual object Instantiate()
		{
			return new HashSet<T>();
		}
	}
}
