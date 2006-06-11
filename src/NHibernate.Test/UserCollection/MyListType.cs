using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserType;

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
            return new PersistentMylist(session, (IList) collection);
        }

        public IEnumerable GetElements(object collection)
        {
            return (IEnumerable) collection;
        }

        public bool Contains(object collection, object entity)
        {
            return ((IList) collection).Contains(entity);
        }

        public object IndexOf(object collection, object entity)
        {
            return ((IList) collection).IndexOf(entity);
        }

        public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner,
                                      IDictionary copyCache, ISessionImplementor session)
        {
            IList result = (IList) target;
            result.Clear();
            foreach (object o in ((IEnumerable)original))
            {
                result.Add(o);
            }
            return result;
        }

        public object Instantiate()
        {
            return new MyList();
        }
    }
}
