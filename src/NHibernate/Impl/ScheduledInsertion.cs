using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl {
	
	internal class ScheduledInsertion : ScheduledEntityAction {
		
		private object[] state;

		public ScheduledInsertion(object id, object[] state, object instance, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) {
			this.state = state;
		}

		public override void Execute() {
			persister.Insert( id, state, instance, session);
			session.PostInsert(instance);
		}

		public override void AfterTransactionCompletion() {
			
		}
	}
}
