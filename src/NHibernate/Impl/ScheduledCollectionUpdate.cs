using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl {

	internal sealed class ScheduledCollectionUpdate : ScheduledCollectionAction {
		
		private readonly PersistentCollection collection;
		private readonly bool emptySnapshot;
		
		public ScheduledCollectionUpdate(PersistentCollection collection, CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session) : base(persister, id, session) {
			this.collection = collection;
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute() {
			persister.Softlock(id);
			if ( !collection.WasInitialized ) {
				if ( !collection.HasQueuedAdds ) throw new AssertionFailure("bug processing queued adds");
				//do nothing - we only need to notify the cache...
			}
			else if ( collection.Empty ) {
				if( !emptySnapshot ) persister.Remove(id, session);
			}
			else if ( collection.NeedsRecreate() ) {
				if( !emptySnapshot ) persister.Remove(id, session);
				persister.Recreate(collection, id, session);
			}
			else {
				persister.DeleteRows(collection, id, session);
				persister.UpdateRows(collection, id, session);
				persister.InsertRows(collection, id, session);

			}
			
		}
	}
}
