using System;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Cache;

namespace NHibernate.Impl 
{
	/// <summary>
	/// The base class for a scheduled action to perform on an entity during a
	/// flush.
	/// </summary>
	internal abstract class ScheduledEntityAction : IExecutable 
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


		/// <summary>
		/// Gets the <see cref="ISessionImplementor"/> the action is executing in.
		/// </summary>
		protected ISessionImplementor Session 
		{
			get { return _session;}
		}

		/// <summary>
		/// Gets the identifier of the object.
		/// </summary>
		protected object Id 
		{
			get { return _id; }
		}

		/// <summary>
		/// Gets the <see cref="IClassPersister"/> that is responsible for persisting the object.
		/// </summary>
		protected IClassPersister Persister 
		{
			get { return _persister;}
		}

		/// <summary>
		/// Gets the object that is having the scheduled action performed against it.
		/// </summary>
		protected object Instance 
		{
			get { return _instance; }
		}
		
		#region SessionImpl.IExecutable Members

		
		/// <summary>
		/// Called when the Transaction this action occurred in has completed.
		/// </summary>
		public abstract void AfterTransactionCompletion();

		/// <summary>
		/// Execute the action using the <see cref="IClassPersister"/>.
		/// </summary>
		public abstract void Execute();

		public object[] PropertySpaces 
		{
			get { return _persister.PropertySpaces; }
		}

		#endregion

	}
}
