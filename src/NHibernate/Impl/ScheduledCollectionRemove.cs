using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl {

	internal sealed class ScheduledCollectionRemove : ScheduledCollectionAction 
	{
		
		private bool emptySnapshot;
		
		public ScheduledCollectionRemove(CollectionPersister persister, object id, bool emptySnapshot, ISessionImplementor session) : base(persister, id, session) 
		{
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute() 
		{
			Persister.Softlock(Id);
			if(!emptySnapshot) Persister.Remove(Id, Session);
		}
	}
}
