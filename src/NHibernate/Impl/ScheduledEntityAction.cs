using System;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Cache;

namespace NHibernate.Impl {
	
	internal abstract class ScheduledEntityAction : SessionImpl.IExecutable {
		
		protected ISessionImplementor session;
		protected object id;
		protected IClassPersister persister;
		protected object instance;

		

		protected ScheduledEntityAction(ISessionImplementor session, object id, object instance, IClassPersister persister) {
			this.session = session;
			this.id = id;
			this.persister = persister;
			this.instance = instance;
		}


		public object[] PropertySpaces {
			get { return persister.PropertySpaces; }
		}

		public abstract void AfterTransactionCompletion();

		public abstract void Execute();

	}
}
