using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// The base class for a scheduled action to perform on a Collection during a
	/// flush.
	/// </summary>
	internal abstract class ScheduledCollectionAction : IExecutable
	{
		private ICollectionPersister _persister;
		private object _id;
		private ISessionImplementor _session;
		private ISoftLock _lock = null;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionAction"/>.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionAction( ICollectionPersister persister, object id, ISessionImplementor session )
		{
			_persister = persister;
			_session = session;
			_id = id;
		}

		/// <summary>
		/// Gets the <see cref="ICollectionPersister"/> that is responsible for persisting the Collection.
		/// </summary>
		public ICollectionPersister Persister
		{
			get { return _persister; }
		}

		/// <summary>
		/// Gets the identifier of the Collection owner.
		/// </summary>
		public object Id
		{
			get { return _id; }
		}

		/// <summary>
		/// Gets the <see cref="ISessionImplementor"/> the action is executing in.
		/// </summary>
		public ISessionImplementor Session
		{
			get { return _session; }
		}

		#region SessionImpl.IExecutable Members

		/// <summary></summary>
		public void AfterTransactionCompletion()
		{
			if ( _persister.HasCache )
			{
				_persister.Cache.Release( _id, _lock );
			}
		}

		public abstract void Execute();

		/// <summary></summary>
		public object[ ] PropertySpaces
		{
			get { return new object[ ] {_persister.CollectionSpace}; } //TODO: cache the array on the persister
		}

		#endregion
	}
}