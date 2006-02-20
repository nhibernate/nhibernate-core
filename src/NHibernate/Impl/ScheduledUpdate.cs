using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled update of an object.
	/// </summary>
	[Serializable]
	internal class ScheduledUpdate : ScheduledEntityAction
	{
		private readonly object[] fields;
		private readonly object[] oldFields;
		private readonly object lastVersion;
		private readonly object nextVersion;
		private readonly int[ ] dirtyFields;
		private readonly object[ ] updatedState;
		private CacheEntry cacheEntry;
		private ISoftLock _lock;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledUpdate"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="fields">An array of objects that contains the value of each Property.</param>
		/// <param name="dirtyProperties">An array that contains the indexes of the dirty Properties.</param>
		/// <param name="oldFields"></param>
		/// <param name="lastVersion">The current version of the object.</param>
		/// <param name="nextVersion">The version the object should be after update.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="updatedState">A deep copy of the <c>fields</c> object array.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledUpdate( object id, object[ ] fields, int[ ] dirtyProperties, object[ ] oldFields, object lastVersion, object nextVersion, object instance, object[ ] updatedState, IClassPersister persister, ISessionImplementor session )
			: base( session, id, instance, persister )
		{
			this.fields = fields;
			this.oldFields = oldFields;
			this.lastVersion = lastVersion;
			this.nextVersion = nextVersion;
			this.dirtyFields = dirtyProperties;
			this.updatedState = updatedState;
		}

		/// <summary></summary>
		public override void Execute()
		{
			if( Persister.HasCache )
			{
				_lock = Persister.Cache.Lock( Id, lastVersion );
			}
			Persister.Update( Id, fields, dirtyFields, oldFields, lastVersion, Instance, Session );
			Session.PostUpdate( Instance, updatedState, nextVersion );

			if ( Persister.HasCache )
			{
				if ( Persister.IsCacheInvalidationRequired )
				{
					Persister.Cache.Evict( Id );
				}
				else
				{
					// TODO: Inefficient if that cache is just going to ignore the updated state!
					cacheEntry = new CacheEntry( Instance, Persister, Session );
					Persister.Cache.Update( Id, cacheEntry );
				}
			}
		}

		/// <summary></summary>
		public override void AfterTransactionCompletion( bool success )
		{
			if( Persister.HasCache )
			{
				if ( success && !Persister.IsCacheInvalidationRequired )
				{
					Persister.Cache.AfterUpdate( Id, cacheEntry, nextVersion, _lock );
				}
				else
				{
					Persister.Cache.Release( Id, _lock );
				}
			}
		}
	}
}