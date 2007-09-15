using log4net;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Wrap collections in a Hibernate collection wrapper.
	/// </summary>
	public class WrapVisitor : ProxyVisitor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(WrapVisitor));

		internal WrapVisitor(IEventSource session) : base(session) { }

		internal override object ProcessCollection(object collection, CollectionType collectionType)
		{
			IPersistentCollection coll = collection as IPersistentCollection;
			if (coll != null)
			{
				ISessionImplementor session = Session;
				if (coll.SetCurrentSession(session))
				{
					ReattachCollection(coll, coll.CollectionSnapshot);
				}
				return null;
			}
			else
			{
				return ProcessArrayOrNewCollection(collection, collectionType);
			}
		}

		private object ProcessArrayOrNewCollection(object collection, CollectionType collectionType)
		{
			if (collection == null)
			{
				//do nothing
				return null;
			}

			ISessionImplementor session = Session;

			ICollectionPersister persister = session.Factory.GetCollectionPersister(collectionType.Role);

			//TODO: move into collection type, so we can use polymorphism!
			if (collectionType.IsArrayType)
			{
				//if (collection == CollectionType.UNFETCHED_COLLECTION)
				//  return null;

				PersistentArrayHolder ah = session.GetCollectionHolder(collection);
				if (ah == null)
				{
					//ah = collectionType.Wrap(session, collection);
					ah = new PersistentArrayHolder(session, collection);
					session.AddNewCollection(persister, ah);
					session.AddCollectionHolder(ah);
				}
				return null;
			}
			else
			{
				IPersistentCollection persistentCollection = collectionType.Wrap(session, collection);
				session.AddNewCollection(persister, persistentCollection);

				if (log.IsDebugEnabled)
					log.Debug("Wrapped collection in role: " + collectionType.Role);

				return persistentCollection; //Force a substitution!
			}
		}
	}
}
