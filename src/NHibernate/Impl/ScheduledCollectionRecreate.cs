using System;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl 
{
	
	internal sealed class ScheduledCollectionRecreate : ScheduledCollectionAction 
	{
		private PersistentCollection _collection;

		public ScheduledCollectionRecreate(PersistentCollection collection, CollectionPersister persister, object id, ISessionImplementor session) : base(persister, id, session) 
		{
			_collection = collection;
		}

		public override void Execute() 
		{
			Persister.Softlock( Id );
			Persister.Recreate( _collection, Id, Session );
		}
	}
}
