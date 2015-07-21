using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// Reassociates uninitialized proxies with the session
	/// </summary>
	public abstract class ProxyVisitor : AbstractVisitor
	{
		public ProxyVisitor(IEventSource session) : base(session) { }

		/// <summary>
		///  Visit a many-to-one or one-to-one associated entity. Default superclass implementation is a no-op.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="entityType"></param>
		/// <returns></returns>
		internal override object ProcessEntity(object value, EntityType entityType)
		{

			if (value != null)
			{
				Session.PersistenceContext.ReassociateIfUninitializedProxy(value);
				// if it is an initialized proxy, let cascade
				// handle it later on
			}

			return null;
		}

		/// <summary> 
		/// Has the owner of the collection changed since the collection was snapshotted and detached?
		/// </summary>
		protected internal static bool IsOwnerUnchanged(IPersistentCollection snapshot, ICollectionPersister persister, object id)
		{
			return IsCollectionSnapshotValid(snapshot) && persister.Role.Equals(snapshot.Role) && id.Equals(snapshot.Key);
		}

		private static bool IsCollectionSnapshotValid(IPersistentCollection snapshot)
		{
			return snapshot != null && snapshot.Role != null && snapshot.Key != null;
		}

		/// <summary> 
		/// Reattach a detached (disassociated) initialized or uninitialized
		/// collection wrapper, using a snapshot carried with the collection wrapper
		/// </summary>
		protected internal void ReattachCollection(IPersistentCollection collection, CollectionType type)
		{
			if (collection.WasInitialized)
			{
				ICollectionPersister collectionPersister = Session.Factory.GetCollectionPersister(type.Role);
				Session.PersistenceContext.AddInitializedDetachedCollection(collectionPersister, collection);
			}
			else
			{
				if (!IsCollectionSnapshotValid(collection))
				{
					throw new HibernateException("could not reassociate uninitialized transient collection");
				}
				ICollectionPersister collectionPersister = Session.Factory.GetCollectionPersister(collection.Role);
				Session.PersistenceContext.AddUninitializedDetachedCollection(collectionPersister, collection);
			}
		}
	}
}
