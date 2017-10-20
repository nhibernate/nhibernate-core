using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// When an entity is passed to update(), we must inspect all its collections and
	/// 1. associate any uninitialized PersistentCollections with this session
	/// 2. associate any initialized PersistentCollections with this session, using the existing snapshot
	/// 3. execute a collection removal (SQL DELETE) for each null collection property or "new" collection 
	/// </summary>
	public partial class OnUpdateVisitor : ReattachVisitor
	{
		public OnUpdateVisitor(IEventSource session, object ownerIdentifier, object owner) : base(session, ownerIdentifier, owner) { }

		internal override object ProcessCollection(object collection, CollectionType type)
		{
			if (collection == CollectionType.UnfetchedCollection)
			{
				return null;
			}

			IEventSource session = Session;
			ICollectionPersister persister = session.Factory.GetCollectionPersister(type.Role);

			object collectionKey = ExtractCollectionKeyFromOwner(persister);
			IPersistentCollection wrapper = collection as IPersistentCollection;
			if (wrapper != null)
			{
				if (wrapper.SetCurrentSession(session))
				{
					//a "detached" collection!
					if (!IsOwnerUnchanged(wrapper, persister, collectionKey))
					{
						// if the collection belonged to a different entity,
						// clean up the existing state of the collection
						RemoveCollection(persister, collectionKey, session);
					}
					ReattachCollection(wrapper, type);
				}
				else
				{
					// a collection loaded in the current session
					// can not possibly be the collection belonging
					// to the entity passed to update()
					RemoveCollection(persister, collectionKey, session);
				}
			}
			else
			{
				// null or brand new collection
				// this will also (inefficiently) handle arrays, which have
				// no snapshot, so we can't do any better
				RemoveCollection(persister, collectionKey, session);
			}

			return null;
		}
	}
}
