using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled insertion of an object.
	/// </summary>
	[Serializable]
	internal class ScheduledInsertion : ScheduledEntityAction
	{
		private readonly object[ ] state;
		private CacheEntry cacheEntry;
		private readonly object version;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledInsertion"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="state">An object array that contains the state of the object being inserted.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="version">The version of the object instance.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledInsertion( object id, object[ ] state, object instance, object version, IClassPersister persister, ISessionImplementor session )
			: base( session, id, instance, persister )
		{
			this.state = state;
			this.version = version;
		}

		/// <summary></summary>
		public override void Execute()
		{
			// Don't need to lock the cache here, since if someone
			// else inserted the same pk first, the insert would fail
			Persister.Insert( Id, state, Instance, Session );
			Session.PostInsert( Instance );

			if ( Persister.HasCache && !Persister.IsCacheInvalidationRequired )
			{
				cacheEntry = new CacheEntry( Instance, Persister, Session );
				Persister.Cache.Insert( Id, cacheEntry );
			}
		}

		/// <summary></summary>
		public override void AfterTransactionCompletion( bool success )
		{
			// Make 100% certain that this is called before any subsequent ScheduledUpdate.AfterTransactionCompletion()!!
			if ( success && Persister.HasCache && !Persister.IsCacheInvalidationRequired )
			{
				Persister.Cache.AfterInsert( Id, cacheEntry, version );
			}
		}
	}
}