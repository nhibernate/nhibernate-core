using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl 
{

	internal sealed class ScheduledCollectionUpdate : ScheduledCollectionAction 
	{
		
		private readonly PersistentCollection collection;
		private readonly bool emptySnapshot;
		
		public ScheduledCollectionUpdate(PersistentCollection collection, CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session) : base(persister, id, session) 
		{
			this.collection = collection;
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute() 
		{
			Persister.Softlock(Id);
			if ( !collection.WasInitialized ) 
			{
				if ( !collection.HasQueuedAdds ) throw new AssertionFailure("bug processing queued adds");
				//do nothing - we only need to notify the cache...
			}
			else if ( collection.Empty ) 
			{
				if( !emptySnapshot ) Persister.Remove(Id, Session);
			}
			else if ( collection.NeedsRecreate(Persister) ) 
			{
				if( !emptySnapshot ) Persister.Remove(Id, Session);
				Persister.Recreate(collection, Id, Session);
			}
			else 
			{
				Persister.DeleteRows(collection, Id, Session);
				Persister.UpdateRows(collection, Id, Session);
				Persister.InsertRows(collection, Id, Session);

			}
			
		}
	}
}
