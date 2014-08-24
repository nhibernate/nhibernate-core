using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class MilestoneCollectionType<TKey, TValue> : IUserCollectionType where TKey : IComparable<TKey>
	{
		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentMilestoneCollection<TKey, TValue>(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentMilestoneCollection<TKey, TValue>(session, (IMilestoneCollection <TKey, TValue>) collection);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable)((IMilestoneCollection<TKey, TValue>) collection).Values;
		}

		public bool Contains(object collection, object entity)
		{
			throw new NotImplementedException();
		}

		public object IndexOf(object collection, object entity)
		{
			throw new NotImplementedException();
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Instantiate(int anticipatedSize)
		{
			return new MilestoneCollection<TKey, TValue>();
		}
	}
}