using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister;

namespace NHibernate.Impl 
{
	
	internal class ScheduledInsertion : ScheduledEntityAction 
	{
		
		private readonly object[] state;

		public ScheduledInsertion(object id, object[] state, object instance, IClassPersister persister, ISessionImplementor session) : base(session, id, instance, persister) 
		{
			this.state = state;
		}

		public override void Execute() 
		{
			Persister.Insert( Id, state, Instance, Session);
			Session.PostInsert(Instance);
		}

		public override void AfterTransactionCompletion() 
		{
			// do nothing
		}
	}
}
