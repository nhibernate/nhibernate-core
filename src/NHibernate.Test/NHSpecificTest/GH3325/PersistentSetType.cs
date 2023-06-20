using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.GH3325
{
	public class PersistentSetType<T> : IUserCollectionType {
		IPersistentCollection IUserCollectionType.Instantiate(ISessionImplementor session, ICollectionPersister persister) {
			return new PersistentGenericSet<T>(session);
		}

		IPersistentCollection IUserCollectionType.Wrap(ISessionImplementor session, object collection) {
			return new PersistentGenericSet<T>(session, (ISet<T>) collection);
		}

		object IUserCollectionType.ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session) {
			object result = target;
			Clear(result);

			foreach (object obj in (IEnumerable) original) {
				Add(result, CopyElement(persister, obj, session, owner, copyCache));
			}

			return result;
		}

		protected virtual object CopyElement(ICollectionPersister persister, object element, ISessionImplementor session,
		                                     object owner, IDictionary copiedAlready) {
			return persister.ElementType.Replace(element, null, session, owner, copiedAlready);
		}

		object IUserCollectionType.Instantiate(int anticipatedSize) {
			return new HashSetWithICollection<T>();
		}

		public IEnumerable GetElements(object collection) {
			return (ISet<T>) collection;
		}

		public bool Contains(object collection, object entity) {
			return ((ISet<T>) collection).Contains((T) entity);
		}

		public object IndexOf(object collection, object entity) {
			throw new NotSupportedException();
		}

		protected virtual void Add(object collection, object element) {
			((ISet<T>) collection).Add((T) element);
		}

		protected virtual void Clear(object collection) {
			((ISet<T>) collection).Clear();
		}
	}

	public class HashSetWithICollection<T> : HashSet<T>, ICollection {
		public void CopyTo(Array array, int index) {
			this.Cast<object>().ToArray().CopyTo(array, index);
		}

		public object SyncRoot => this;
		public bool IsSynchronized => false;
	}
}
