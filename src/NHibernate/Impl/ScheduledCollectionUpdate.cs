using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl 
{

	internal sealed class ScheduledCollectionUpdate : ScheduledCollectionAction 
	{
		
		private readonly PersistentCollection _collection;
		private readonly bool _emptySnapshot;
		
		public ScheduledCollectionUpdate(PersistentCollection collection, CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session) : base(persister, id, session) 
		{
			_collection = collection;
			_emptySnapshot = emptySnapshot;
		}

		public override void Execute() 
		{
			Persister.Softlock( Id );
			if( !_collection.WasInitialized ) 
			{
				if ( !_collection.HasQueuedAdds ) throw new AssertionFailure("bug processing queued adds");
				//do nothing - we only need to notify the cache...
			}
			else if( _collection.Empty ) 
			{
				if( !_emptySnapshot ) Persister.Remove( Id, Session );
			}
			else if( _collection.NeedsRecreate( Persister ) ) 
			{
				if( !_emptySnapshot ) Persister.Remove( Id, Session );
				Persister.Recreate( _collection, Id, Session );
			}
			else 
			{
				Persister.DeleteRows( _collection, Id, Session );
				Persister.UpdateRows( _collection, Id, Session );
				Persister.InsertRows( _collection, Id, Session );

			}
			
		}
	}
}
