using System;
using NHibernate.Engine;
using NHibernate.Collection;
using NHibernate.Cache;

namespace NHibernate.Impl 
{
	/// <summary>
	/// The base class for a scheduled action to perform on a Collection during a
	/// flush.
	/// </summary>
	internal abstract class ScheduledCollectionAction : IExecutable 
	{
		private CollectionPersister _persister;
		private object _id;
		private ISessionImplementor _session;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionAction"/>.
		/// </summary>
		/// <param name="persister">The <see cref="CollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionAction(CollectionPersister persister, object id, ISessionImplementor session) 
		{
			_persister = persister;
			_session = session;
			_id = id;
		}

		/// <summary>
		/// Gets the <see cref="CollectionPersister"/> that is responsible for persisting the Collection.
		/// </summary>
		public CollectionPersister Persister 
		{
			get { return _persister;}
		}

		/// <summary>
		/// Gets the identifier of the Collection owner.
		/// </summary>
		public object Id 
		{
			get { return _id;}
		}

		/// <summary>
		/// Gets the <see cref="ISessionImplementor"/> the action is executing in.
		/// </summary>
		public ISessionImplementor Session 
		{
			get { return _session;}
		}

		#region SessionImpl.IExecutable Members

		public void AfterTransactionCompletion() 
		{
			_persister.ReleaseSoftlock( _id );
		}

		public abstract void Execute();
		
		public object[] PropertySpaces 
		{
			get { return new string[] { _persister.QualifiedTableName }; } //TODO: cache the array on the persister
		}
		
		#endregion

	}
}
