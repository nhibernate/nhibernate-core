using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl 
{
	internal class ScheduledDeletion : ScheduledEntityAction 
	{		
		private object version;

		public ScheduledDeletion(object id, object version, object instance, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) 
		{
			this.version = version;
		}

		public override void Execute() 
		{
			if ( Persister.HasCache ) Persister.Cache.Lock(Id);
			Persister.Delete(Id, version, Instance, Session);
			Session.PostDelete(Instance);
		}

		public override void AfterTransactionCompletion() 
		{
			if ( Persister.HasCache ) Persister.Cache.Release(Id);
		}
	}
}
