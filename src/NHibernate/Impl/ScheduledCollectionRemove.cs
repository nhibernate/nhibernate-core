using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl {

	internal sealed class ScheduledCollectionRemove : ScheduledCollectionAction {
		
		private readonly bool emptySnapshot;
		
		public ScheduledCollectionRemove(CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session) : base(persister, id, session) {
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute() {
			persister.Softlock(id);
			if(!emptySnapshot) persister.Remove(id, session);
		}
	}
}
