using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl {

	internal sealed class ScheduledCollectionRemove : ScheduledCollectionAction {
		
		public ScheduledCollectionRemove(CollectionPersister persister, object id, ISessionImplementor session) : base(persister, id, session) {}

		public override void Execute() {
			persister.Softlock(id);
			persister.Remove(id, session);
		}
	}
}
