using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH2278
{
	public class CustomIdentifierBagType<T> : IUserCollectionType
	{
		#region IUserCollectionType Members

		public bool Contains(object collection, object entity)
		{
			return ((IList<T>)collection).Contains((T)entity);
		}

		public IEnumerable GetElements(object collection)
		{
			return (IEnumerable)collection;
		}

		public object IndexOf(object collection, object entity)
		{
			return ((IList<T>)collection).IndexOf((T)entity);
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			IList<T> result = (IList<T>)target;
			result.Clear();
			foreach (object item in ((IEnumerable)original))
				result.Add((T)item);
			return result;
		}

		// return an instance of the inner collection type
		public object Instantiate(int anticipatedSize)
		{
			return new CustomList<T>();
		}

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new CustomPersistentIdentifierBag<T>(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new CustomPersistentIdentifierBag<T>(session, (ICollection<T>)collection);
		}

		#endregion
	}
}