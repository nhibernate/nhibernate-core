using System;
using NHibernate.Engine;
using NHibernate.Collection;
using NHibernate.Cache;

namespace NHibernate.Impl 
{
	
	internal abstract class ScheduledCollectionAction : SessionImpl.IExecutable 
	{
		private CollectionPersister _persister;
		private object _id;
		private ISessionImplementor _session;

		public ScheduledCollectionAction(CollectionPersister persister, object id, ISessionImplementor session) 
		{
			_persister = persister;
			_session = session;
			_id = id;
		}

		public void AfterTransactionCompletion() 
		{
			_persister.ReleaseSoftlock( _id );
		}

		public object[] PropertySpaces 
		{
			get { return new string[] { _persister.QualifiedTableName }; } //TODO: cache the array on the persister
		}

		public CollectionPersister Persister 
		{
			get { return _persister;}
		}

		public object Id 
		{
			get { return _id;}
		}

		public ISessionImplementor Session 
		{
			get { return _session;}
		}

		public abstract void Execute();
	}
}
