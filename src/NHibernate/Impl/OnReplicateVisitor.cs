using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// When an entity is passed to Update(), we must inspect all its collections and
	/// 1. associate any uninitialized PersistentCollections with this session
	/// 2. associate any initialized PersistentCollections with this session, using the existing snapshot
	/// 3. execute a collection removal (SQL DELETE) for each null collection property or "new" collection
	/// </summary>
	internal class OnReplicateVisitor : ReattachVisitor
	{
		public OnReplicateVisitor( SessionImpl session, object key ) : base( session, key )
		{
		}

		protected override object ProcessCollection( object collection, CollectionType type )
		{
			SessionImpl session = Session;
			object key = Key;
			ICollectionPersister persister = session.GetCollectionPersister( type.Role );

			session.RemoveCollection( persister, key );

			if ( collection != null && ( collection is IPersistentCollection ) )
			{
				IPersistentCollection wrapper = collection as IPersistentCollection;
				wrapper.SetCurrentSession( session );
				if ( wrapper.WasInitialized )
				{
					session.AddNewCollection( wrapper, persister );
				}
				else
				{
					session.ReattachCollection( wrapper, wrapper.CollectionSnapshot ) ;
				}
			}
			else
			{
				// otherwise a null or brand new collection 
				// this will also (inefficiently) handle arrays, which 
				// have no snapshot, so we can't do any better
				//processArrayOrNewCollection(collection, type);
			}

			return null;
		}
	}
}
