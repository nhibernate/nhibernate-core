using System;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Cache;

namespace NHibernate.Impl 
{
	
	internal abstract class ScheduledEntityAction : SessionImpl.IExecutable 
	{
		
		private readonly ISessionImplementor session;
		private readonly object id;
		private readonly IClassPersister persister;
		private readonly object instance;

		protected ScheduledEntityAction(ISessionImplementor session, object id, object instance, IClassPersister persister) 
		{
			this.session = session;
			this.id = id;
			this.persister = persister;
			this.instance = instance;
		}


		public object[] PropertySpaces {
			get { return persister.PropertySpaces; }
		}

		protected ISessionImplementor Session 
		{
			get { return session;}
		}

		protected object Id 
		{
			get { return id; }
		}

		protected IClassPersister Persister 
		{
			get { return persister;}
		}


		protected object Instance 
		{
			get { return instance; }
		}

		public abstract void AfterTransactionCompletion();

		public abstract void Execute();

	}
}
