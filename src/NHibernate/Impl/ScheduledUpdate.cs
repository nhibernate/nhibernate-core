using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl 
{
	internal class ScheduledUpdate : ScheduledEntityAction 
	{
		private object[] fields;
		private object lastVersion;
		private object nextVersion;
		private int[] dirtyFields;
		private object[] updatedState;

		public ScheduledUpdate(object id, object[] fields, int[] dirtyProperties, object lastVersion, object nextVersion, object instance, object[] updatedState, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) 
		{
			this.fields = fields;
			this.lastVersion = lastVersion;
			this.nextVersion = nextVersion;
			this.dirtyFields = dirtyFields;
			this.updatedState = updatedState;
		}

		public override void Execute() 
		{
			if ( Persister.HasCache ) Persister.Cache.Lock(Id);
			Persister.Update(Id, fields, dirtyFields, lastVersion, Instance, Session);
			Session.PostUpdate(Instance, updatedState, nextVersion);
		}

		public override void AfterTransactionCompletion() 
		{
			if ( Persister.HasCache ) Persister.Cache.Release(Id);
		}
	}
}
