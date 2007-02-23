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
		private readonly int[] dirtyFields;
		private readonly object[] updatedState;
		private readonly bool hasDirtyCollection;
		private CacheEntry cacheEntry;
		private ISoftLock _lock;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledUpdate"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="fields">An array of objects that contains the value of each Property.</param>
		/// <param name="dirtyProperties">An array that contains the indexes of the dirty Properties.</param>
		/// <param name="hasDirtyCollection">Whether the object contains a dirty collection.</param>
		/// <param name="oldFields"></param>
		/// <param name="lastVersion">The current version of the object.</param>
		/// <param name="nextVersion">The version the object should be after update.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="updatedState">A deep copy of the <c>fields</c> object array.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledUpdate(object id, object[] fields, int[] dirtyProperties, bool hasDirtyCollection, object[] oldFields,
		                       object lastVersion, object nextVersion, object instance, object[] updatedState,
		                       IEntityPersister persister, ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			this.fields = fields;
			this.oldFields = oldFields;
			this.lastVersion = lastVersion;
			this.nextVersion = nextVersion;
			this.dirtyFields = dirtyProperties;
			this.hasDirtyCollection = hasDirtyCollection;
			this.updatedState = updatedState;
		}

		/// <summary></summary>
		public override void Execute()
		{
			CacheKey ck = null;
			if (Persister.HasCache)
			{
				ck = new CacheKey(
					Id,
					Persister.IdentifierType,
					(string) Persister.IdentifierSpace,
					Session.Factory
					);
				_lock = Persister.Cache.Lock(ck, lastVersion);
			}
			Persister.Update(Id, fields, dirtyFields, hasDirtyCollection, oldFields, lastVersion, Instance, Session);
			Session.PostUpdate(Instance, updatedState, nextVersion);

			if (Persister.HasCache)
			{
				if (Persister.IsCacheInvalidationRequired)
				{
					Persister.Cache.Evict(ck);
				}
				else
				{
					// TODO: Inefficient if that cache is just going to ignore the updated state!
					cacheEntry = new CacheEntry(Instance, Persister, Session);
					Persister.Cache.Update(ck, cacheEntry);
				}
			}
		}

		/// <summary></summary>
		public override void AfterTransactionCompletion(bool success)
		{
			if (Persister.HasCache)
			{
				CacheKey ck = new CacheKey(
					Id,
					Persister.IdentifierType,
					(string) Persister.IdentifierSpace,
					Session.Factory
					);
				if (success && !Persister.IsCacheInvalidationRequired)
				{
					Persister.Cache.AfterUpdate(ck, cacheEntry, nextVersion, _lock);
				}
				else
				{
					Persister.Cache.Release(ck, _lock);
				}
			}
		}
	}
}