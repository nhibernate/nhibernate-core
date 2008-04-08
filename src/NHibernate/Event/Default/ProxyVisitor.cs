using NHibernate.Collection;
using NHibernate.Engine;
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
		protected internal static bool IsOwnerUnchanged(ICollectionSnapshot snapshot, ICollectionPersister persister, object id)
		{
			return IsCollectionSnapshotValid(snapshot) &&
					 persister.Role.Equals(snapshot.Role) &&
					 id.Equals(snapshot.Key);
		}

		private static bool IsCollectionSnapshotValid(ICollectionSnapshot snapshot)
		{
			return snapshot != null &&
					 snapshot.Role != null &&
					 snapshot.Key != null;
		}

		/// <summary> 
		/// Reattach a detached (disassociated) initialized or uninitialized
		/// collection wrapper, using a snapshot carried with the collection wrapper
		/// </summary>
		protected internal void ReattachCollection(IPersistentCollection collection, ICollectionSnapshot snapshot)
		{
			if (collection.WasInitialized)
			{
				Session.PersistenceContext.AddInitializedDetachedCollection(collection, snapshot);
			}
			else
			{
				if (!IsCollectionSnapshotValid(snapshot))
					throw new HibernateException("could not reassociate uninitialized transient collection");

				Session.PersistenceContext.AddUninitializedDetachedCollection(collection, snapshot);
			}
		}
	}
}
