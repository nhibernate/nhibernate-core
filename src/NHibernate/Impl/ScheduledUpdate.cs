using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl {
	
	internal class ScheduledUpdate : ScheduledEntityAction {
		
		private object[] fields;
		private object lastVersion;
		private object nextVersion;
		private int[] dirtyFields;
		private object[] updatedState;

		[Obsolete("The ctor with more params should be used")]
		public ScheduledUpdate(object id, object[] fields, int[] dirtyProperties, object lastVersion, object instance, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) {
			this.fields = fields;
			this.lastVersion = lastVersion;
			this.dirtyFields = dirtyProperties;
		}

		public ScheduledUpdate(object id, object[] fields, int[] dirtyProperties, object lastVersion, object nextVersion, object instance, object[] updateState, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) 
		{
			this.fields = fields;
			this.lastVersion = lastVersion;
			this.nextVersion = nextVersion;
			this.dirtyFields = dirtyFields;
			this.updatedState = updatedState;
		}

		public override void Execute() {
			if ( persister.HasCache ) persister.Cache.Lock(id);
			persister.Update(id, fields, dirtyFields, lastVersion, instance, session);
			session.PostUpdate(instance, updatedState, nextVersion);
		}

		public override void AfterTransactionCompletion() {
			if ( persister.HasCache ) persister.Cache.Release(id);
		}
	}
}
