using System;
using NHibernate.Engine;
using NHibernate.Collection;
using NHibernate.Cache;

namespace NHibernate.Impl {
	
	internal abstract class ScheduledCollectionAction : SessionImpl.IExecutable {
		protected CollectionPersister persister;
		protected object id;
		protected ISessionImplementor session;

		public ScheduledCollectionAction(CollectionPersister persister, object id, ISessionImplementor session) {
			this.persister = persister;
			this.session = session;
			this.id = id;
		}

		public void AfterTransactionCompletion() {
			persister.ReleaseSoftlock(id);
		}

		public object[] PropertySpaces {
			get { return new string[] { persister.QualifiedTableName }; } //TODO: cache the array on the persister
		}

		public object Id 
		{
			get { return id;}
		}

		public ISessionImplementor Session 
		{
			get { return session;}
		}

		public abstract void Execute();
	}
}
