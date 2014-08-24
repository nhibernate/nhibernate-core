using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class FakeUserCollectionType: IUserCollectionType
	{
		#region Implementation of IUserCollectionType

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			throw new NotImplementedException();
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			throw new NotImplementedException();
		}

		public IEnumerable GetElements(object collection)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		#endregion
	}
}