using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl 
{
	
	internal sealed class ScheduledCollectionRecreate : ScheduledCollectionAction 
	{
		private PersistentCollection collection;

		public ScheduledCollectionRecreate(PersistentCollection collection, CollectionPersister persister, object id, ISessionImplementor session) : base(persister, id, session) 
		{
			this.collection = collection;
		}

		public override void Execute() 
		{
			Persister.Softlock(Id);
			Persister.Recreate(collection, Id, Session);
		}
	}
}
