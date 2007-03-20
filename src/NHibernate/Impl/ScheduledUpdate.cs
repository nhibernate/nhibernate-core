using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// A scheduled update of an object.
	/// </summary>
	[Serializable]
	internal class ScheduledUpdate : ScheduledEntityAction
	{
		private readonly object[] state;
		private readonly object[] previousState;
		private readonly object lastVersion;
		private object nextVersion;
		private readonly int[] dirtyFields;
		private readonly bool hasDirtyCollection;
		private CacheEntry cacheEntry;
		private ISoftLock _lock;

		/// <summary>
		/// Initializes a new instance of <see cref="ScheduledUpdate"/>.
		/// </summary>
		/// <param name="id">The identifier of the object.</param>
		/// <param name="state">An array of objects that contains the value of each Property.</param>
		/// <param name="dirtyProperties">An array that contains the indexes of the dirty Properties.</param>
		/// <param name="hasDirtyCollection">Whether the object contains a dirty collection.</param>
		/// <param name="previousState"></param>
		/// <param name="lastVersion">The current version of the object.</param>
		/// <param name="nextVersion">The version the object should be after update.</param>
		/// <param name="instance">The actual object instance.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for the persisting the object.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occuring in.</param>
		public ScheduledUpdate(object id, object[] state, int[] dirtyProperties, bool hasDirtyCollection, object[] previousState,
		                       object lastVersion, object nextVersion, object instance,
		                       IEntityPersister persister, ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			this.state = state;
			this.previousState = previousState;
			this.lastVersion = lastVersion;
			this.nextVersion = nextVersion;
			this.dirtyFields = dirtyProperties;
			this.hasDirtyCollection = hasDirtyCollection;
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
			Persister.Update(Id, state, dirtyFields, hasDirtyCollection, previousState, lastVersion, Instance, Session);
			
			EntityEntry entry = Session.GetEntry(Instance);
			if (entry == null)
			{
				throw new AssertionFailure("possible nonthreadsafe access to session");
			}

			if (entry.Status == Status.Loaded || Persister.IsVersionPropertyGenerated)
			{
				// get the updated snapshot of the entity state by cloning current state;
				// it is safe to copy in place, since by this time no-one else (should have)
				// has a reference  to the array
				TypeFactory.DeepCopy(
						state,
						Persister.PropertyTypes,
						Persister.PropertyCheckability,
						state);
				if (Persister.HasUpdateGeneratedProperties)
				{
					// this entity defines proeprty generation, so process those generated
					// values...
					Persister.ProcessUpdateGeneratedProperties(Id, Instance, state, Session);
					if (Persister.IsVersionPropertyGenerated)
					{
						nextVersion = Versioning.GetVersion(state, Persister);
					}
				}
				// have the entity entry perform post-update processing, passing it the
				// update state and the new version (if one).
				entry.PostUpdate(Instance, state, nextVersion);
			}

			if (Persister.HasCache)
			{
				if (Persister.IsCacheInvalidationRequired || entry.Status != Status.Loaded)
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