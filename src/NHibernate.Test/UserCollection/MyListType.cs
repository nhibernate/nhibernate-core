using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.UserCollection
{
	public class MyListType : IUserCollectionType
	{
		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentMylist(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new PersistentMylist(session, (IList<Email>) collection);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable) collection;
		}

		public bool Contains(object collection, object entity)
		{
			var email = entity as Email;
			if (entity != null && email == null)
				return false;

			return ((IList<Email>) collection).Contains(email);
		}

		public object IndexOf(object collection, object entity)
		{
			var email = entity as Email;
			if (entity != null && email == null)
				return -1;

			return ((IList<Email>)collection).IndexOf(email);
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner,
		                              IDictionary copyCache, ISessionImplementor session)
		{
			IList<Email> result = (IList<Email>) target;
			result.Clear();

			foreach (object o in ((IEnumerable) original))
				result.Add((Email) o);

			return result;
		}

		public object Instantiate(int anticipatedSize)
		{
			return new MyList();
		}
	}
}
