using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl {
	
	internal class ScheduledDeletion : ScheduledEntityAction {
		
		private object version;

		public ScheduledDeletion(object id, object version, object instance, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) {
			this.version = version;
		}

		public override void Execute() {
			if ( persister.HasCache ) persister.Cache.Lock(id);
			persister.Delete(id, version, instance, session);
			session.PostDelete(instance);
		}

		public override void AfterTransactionCompletion() {
			if ( persister.HasCache ) persister.Cache.Release(id);
		}
	}
}
