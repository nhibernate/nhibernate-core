using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// When a transient entity is passed to lock(), we must inspect all its collections and
	/// 1. associate any uninitialized PersistentCollections with this session
	/// 2. associate any initialized PersistentCollections with this session, using the existing snapshot
	/// 3. throw an exception for each "new" collection 
	/// </summary>
	public class OnLockVisitor : ReattachVisitor
	{
		public OnLockVisitor(IEventSource session, object ownerIdentifier, object owner) : base(session, ownerIdentifier, owner) { }

		internal override object ProcessCollection(object collection, CollectionType type)
		{
			ISessionImplementor session = Session;
			ICollectionPersister persister = session.Factory.GetCollectionPersister(type.Role);

			if (collection == null)
			{
				//do nothing
			}
			else
			{
				IPersistentCollection persistentCollection = collection as IPersistentCollection;
				if (persistentCollection != null)
				{
					if (persistentCollection.SetCurrentSession(session))
					{
						if (IsOwnerUnchanged(persistentCollection, persister, ExtractCollectionKeyFromOwner(persister)))
						{
							// a "detached" collection that originally belonged to the same entity
							if (persistentCollection.IsDirty)
							{
								throw new HibernateException("reassociated object has dirty collection: " + persistentCollection.Role);
							}
							ReattachCollection(persistentCollection, type);
						}
						else
						{
							// a "detached" collection that belonged to a different entity
                            throw new HibernateException("reassociated object has dirty collection reference: " + persistentCollection.Role);
						}
					}
					else
					{
						// a collection loaded in the current session
						// can not possibly be the collection belonging
						// to the entity passed to update()
                        throw new HibernateException("reassociated object has dirty collection reference: " + persistentCollection.Role);
					}
				}
				else
				{
					// brand new collection
					//TODO: or an array!! we can't lock objects with arrays now??
					throw new HibernateException("reassociated object has dirty collection reference (or an array)");
				}
			}
			return null;
		}
	}
}
