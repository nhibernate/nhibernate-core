using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl {

	internal sealed class ScheduledCollectionUpdate : ScheduledCollectionAction {
		private PersistentCollection collection;
		
		public ScheduledCollectionUpdate(PersistentCollection collection, CollectionPersister persister, object id, ISessionImplementor session) : base(persister, id, session) {
			this.collection = collection;
		}

		public override void Execute() {
			persister.Softlock(id);
			if ( collection.Empty ) {
				persister.Remove(id, session);
			} else if ( collection.NeedsRecreate( persister.ElementType ) ) {
				persister.Remove(id, session);
				persister.Recreate(collection, id, session);
			} else {
				persister.DeleteRows(collection, id, session);
				persister.UpdateRows(collection, id, session);
				persister.InsertRows(collection, id, session);

			}
			
		}
	}
}
