using System;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Cache;

namespace NHibernate.Impl 
{
	
	internal abstract class ScheduledEntityAction : SessionImpl.IExecutable 
	{
		
		private readonly ISessionImplementor _session;
		private readonly object _id;
		private readonly IClassPersister _persister;
		private readonly object _instance;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledEntityAction"/>.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for the persisting the object.</param>
		protected ScheduledEntityAction(ISessionImplementor session, object id, object instance, IClassPersister persister) 
		{
			_session = session;
			_id = id;
			_persister = persister;
			_instance = instance;
		}


		public object[] PropertySpaces 
		{
			get { return _persister.PropertySpaces; }
		}

		protected ISessionImplementor Session 
		{
			get { return _session;}
		}

		protected object Id 
		{
			get { return _id; }
		}

		protected IClassPersister Persister 
		{
			get { return _persister;}
		}


		protected object Instance 
		{
			get { return _instance; }
		}

		/// <summary>
		/// Called when the Transaction this action occurred in has completed.
		/// </summary>
		public abstract void AfterTransactionCompletion();

		/// <summary>
		/// Execute the action using the <see cref="IClassPersister"/>.
		/// </summary>
		public abstract void Execute();

	}
}
