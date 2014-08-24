using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;
using System;
using System.Collections.Generic;

namespace NHibernate.Test.UserCollection.Parameterized
{
	public class DefaultableListType : IUserCollectionType, IParameterizedType
	{
		private string defaultValue;

		#region Implementation of IUserCollectionType

		public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
		{
			return new PersistentDefaultableList(session);
		}

		public IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			if (session.EntityMode == EntityMode.Xml)
			{
				throw new NotSupportedException("XML not supported");
			}
			else
			{
				return new PersistentDefaultableList(session, (IList<string>)collection);
			}
		}

		public IEnumerable GetElements(object collection)
		{
			return (IDefaultableList)collection;
		}

		public bool Contains(object collection, object entity)
		{
			var item = entity as string;
			if (entity != null && item == null)
				return false;

			return ((IDefaultableList)collection).Contains(item);
		}

		public object IndexOf(object collection, object entity)
		{
			var item = entity as string;
			if (entity != null && item == null)
				return null;

			int index = ((IDefaultableList)collection).IndexOf(item);
			return index >= 0 ? (object) index : null;
		}

		public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			IDefaultableList result = (IDefaultableList)target;
			result.Clear();

			foreach (string o in (IDefaultableList) original)
				result.Add(o);

			return result;
		}

		public object Instantiate(int anticipatedSize)
		{
			DefaultableListImpl list = anticipatedSize < 0 ? new DefaultableListImpl() : new DefaultableListImpl(anticipatedSize);
			list.DefaultValue= defaultValue;
			return list;
		}

		#endregion

		#region Implementation of IParameterizedType

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			defaultValue = parameters["default"];
		}

		#endregion
	}
}