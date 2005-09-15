using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled update of the Collection in the database.
	/// </summary>
	/// <remarks>
	/// Entities in the Collection or the contents of the Collection have been modified
	/// and the database should be updated accordingly.
	/// </remarks>
	[Serializable]
	internal sealed class ScheduledCollectionUpdate : ScheduledCollectionAction
	{
		private readonly PersistentCollection _collection;
		private readonly bool _emptySnapshot;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionUpdate"/>.
		/// </summary>
		/// <param name="collection">The <see cref="PersistentCollection"/> to update.</param>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="emptySnapshot">Indicates if the Collection was empty when it was loaded.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionUpdate( PersistentCollection collection, ICollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session )
			: base( persister, id, session )
		{
			_collection = collection;
			_emptySnapshot = emptySnapshot;
		}

		/// <summary></summary>
		public override void Execute()
		{
			if( !_collection.WasInitialized )
			{
				if( !_collection.HasQueuedAdds )
				{
					throw new AssertionFailure( "bug processing queued adds" );
				}

				// do nothing - collection was not initialized 
				// we only need to notify the cache...
			}
			else if( _collection.Empty )
			{
				// the collection had all elements removed - check to see if it
				// was empty when it was loaded or if the contents were actually
				// deleted
				if( !_emptySnapshot )
				{
					Persister.Remove( Id, Session );
				}
			}
			else if( _collection.NeedsRecreate( Persister ) )
			{
				// certain collections (Bag) have to be recreated in the db each
				// time - if the snapshot was not empty then there are some existing
				// rows that need to be removed.
				if( !_emptySnapshot )
				{
					Persister.Remove( Id, Session );
				}

				Persister.Recreate( _collection, Id, Session );
			}
			else
			{
				// this is a normal collection that needs to have its state
				// synched with the database.
				Persister.DeleteRows( _collection, Id, Session );
				Persister.UpdateRows( _collection, Id, Session );
				Persister.InsertRows( _collection, Id, Session );
			}
			Evict();
		}
	}
}