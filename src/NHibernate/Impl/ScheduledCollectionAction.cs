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
		private ICollectionPersister persister;
		private object id;
		private ISessionImplementor session;
		private ISoftLock lck = null;
		private string collectionRole;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledCollectionAction"/>.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="id">The identifier of the Collection owner.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledCollectionAction( ICollectionPersister persister, object id, ISessionImplementor session )
		{
			this.persister = persister;
			this.session = session;
			this.id = id;
			this.collectionRole = persister.Role;
		}

		/// <summary>
		/// Gets the <see cref="ICollectionPersister"/> that is responsible for persisting the Collection.
		/// </summary>
		public ICollectionPersister Persister
		{
			get { return persister; }
		}

		/// <summary>
		/// Gets the identifier of the Collection owner.
		/// </summary>
		public object Id
		{
			get { return id; }
		}

		/// <summary>
		/// Gets the <see cref="ISessionImplementor"/> the action is executing in.
		/// </summary>
		public ISessionImplementor Session
		{
			get { return session; }
		}

		#region SessionImpl.IExecutable Members

		/// <summary></summary>
		/// <param name="success"></param>
		public void AfterTransactionCompletion( bool success )
		{
			if ( persister.HasCache )
			{
				persister.Cache.Release( id, lck );
			}
		}

		public bool HasAfterTransactionCompletion
		{
			get { return persister.HasCache; }
		}

		public abstract void Execute();

		/// <summary>
		/// 
		/// </summary>
		public void BeforeExecutions( )
		{
			// we need to obtain the lock before any actions are
			// executed, since this may be an inverse="true"
			// bidirectional association and it is one of the
			// earlier entity actions which actually updates
			// the database (this action is resposible for
			// second-level cache invalidation only)
			if ( persister.HasCache ) 
			{
				lck = persister.Cache.Lock( id, null ); //collections don't have version numbers :-(
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Evict( )
		{
			if ( persister.HasCache )
			{
				persister.Cache.Evict( id );
			}
		}

		/// <summary></summary>
		public object[ ] PropertySpaces
		{
			get { return new object[ ] { persister.CollectionSpace }; } //TODO: cache the array on the persister
		}

		#endregion
	}
}