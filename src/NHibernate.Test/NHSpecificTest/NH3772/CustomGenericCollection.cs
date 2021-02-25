using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH3772
{
	public class CustomGenericCollection<T> : IUserCollectionType where T : class
	{
		public static bool InstantiateInitializedCollection = true;

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			if (InstantiateInitializedCollection)
				return new PersistentGenericSet<T>(session, new HashSet<T>(EqualityComparer<T>.Default));
			
			return new PersistentGenericSet<T>(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			var realCollection = (ISet<T>) collection;
			return new PersistentGenericSet<T>(session, realCollection);
		}

		public IEnumerable GetElements(object collection)
		{
			var realCollection = (ISet<T>) collection;
			return realCollection.ToList();
		}

		public bool Contains(object collection, object entity)
		{
			var realCollection = (ISet<T>) collection;
			return realCollection.Contains((T) entity);
		}

		public object IndexOf(object collection, object entity)
		{
			return -1; // no indexing supported
		}

		public object ReplaceElements(
			object original,
			object target,
			ICollectionPersister persister,
			object owner,
			IDictionary copyCache,
			ISessionImplementor session)
		{
			var originalCollection = (ISet<T>) original;
			var targetCollection = (ISet<T>) target;

			targetCollection.Clear();
			targetCollection.UnionWith(originalCollection);

			return targetCollection;
		}

		public object Instantiate(int anticipatedSize)
		{
			return new HashSet<T>(EqualityComparer<T>.Default);
		}
	}
}
