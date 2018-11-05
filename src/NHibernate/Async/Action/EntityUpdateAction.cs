﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Diagnostics;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Action
{
	using System.Threading.Tasks;
	using System.Threading;
	public sealed partial class EntityUpdateAction : EntityAction
	{

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ISessionImplementor session = Session;
			object id = Id;
			IEntityPersister persister = Persister;
			object instance = Instance;

			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}

			bool veto = await (PreUpdateAsync(cancellationToken)).ConfigureAwait(false);

			ISessionFactoryImplementor factory = Session.Factory;

			if (persister.IsVersionPropertyGenerated)
			{
				// we need to grab the version value from the entity, otherwise
				// we have issues with generated-version entities that may have
				// multiple actions queued during the same flush
				previousVersion = persister.GetVersion(instance);
			}

			CacheKey ck = null;
			if (persister.HasCache)
			{
				ck = session.GenerateCacheKey(id, persister.IdentifierType, persister.RootEntityName);
				slock = await (persister.Cache.LockAsync(ck, previousVersion, cancellationToken)).ConfigureAwait(false);
			}

			if (!veto)
			{
				await (persister.UpdateAsync(id, state, dirtyFields, hasDirtyCollection, previousState, previousVersion, instance, null, session, cancellationToken)).ConfigureAwait(false);
			}

			EntityEntry entry = Session.PersistenceContext.GetEntry(instance);
			if (entry == null)
			{
				throw new AssertionFailure("Possible nonthreadsafe access to session");
			}

			if (entry.Status == Status.Loaded || persister.IsVersionPropertyGenerated)
			{
				// get the updated snapshot of the entity state by cloning current state;
				// it is safe to copy in place, since by this time no-one else (should have)
				// has a reference  to the array
				TypeHelper.DeepCopy(state, persister.PropertyTypes, persister.PropertyCheckability, state, Session);
				if (persister.HasUpdateGeneratedProperties)
				{
					// this entity defines property generation, so process those generated
					// values...
					await (persister.ProcessUpdateGeneratedPropertiesAsync(id, instance, state, Session, cancellationToken)).ConfigureAwait(false);
					if (persister.IsVersionPropertyGenerated)
					{
						nextVersion = Versioning.GetVersion(state, persister);
					}
				}
				// have the entity entry perform post-update processing, passing it the
				// update state and the new version (if one).
				entry.PostUpdate(instance, state, nextVersion);
			}

			if (persister.HasCache)
			{
				if (persister.IsCacheInvalidationRequired || entry.Status != Status.Loaded)
				{
					await (persister.Cache.EvictAsync(ck, cancellationToken)).ConfigureAwait(false);
				}
				else
				{
					CacheEntry ce = await (CacheEntry.CreateAsync(state, persister, persister.HasUninitializedLazyProperties(instance), nextVersion, Session, instance, cancellationToken)).ConfigureAwait(false);
					cacheEntry = persister.CacheEntryStructure.Structure(ce);

					bool put = await (persister.Cache.UpdateAsync(ck, cacheEntry, nextVersion, previousVersion, cancellationToken)).ConfigureAwait(false);

					if (put && factory.Statistics.IsStatisticsEnabled)
					{
						factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
					}
				}
			}

			await (PostUpdateAsync(cancellationToken)).ConfigureAwait(false);

			if (statsEnabled && !veto)
			{
				stopwatch.Stop();
				factory.StatisticsImplementor.UpdateEntity(Persister.EntityName, stopwatch.Elapsed);
			}
		}

		protected override async Task AfterTransactionCompletionProcessImplAsync(bool success, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IEntityPersister persister = Persister;
			if (persister.HasCache)
			{
				CacheKey ck = Session.GenerateCacheKey(Id, persister.IdentifierType, persister.RootEntityName);

				if (success && cacheEntry != null)
				{
					bool put = await (persister.Cache.AfterUpdateAsync(ck, cacheEntry, nextVersion, slock, cancellationToken)).ConfigureAwait(false);

					if (put && Session.Factory.Statistics.IsStatisticsEnabled)
					{
						Session.Factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
					}
				}
				else
				{
					await (persister.Cache.ReleaseAsync(ck, slock, cancellationToken)).ConfigureAwait(false);
				}
			}
			if (success)
			{
				await (PostCommitUpdateAsync(cancellationToken)).ConfigureAwait(false);
			}
		}
		
		private async Task PostUpdateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPostUpdateEventListener[] postListeners = Session.Listeners.PostUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostUpdateEvent postEvent = new PostUpdateEvent(Instance, Id, state, previousState, Persister, (IEventSource)Session);
				foreach (IPostUpdateEventListener listener in postListeners)
				{
					await (listener.OnPostUpdateAsync(postEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		private async Task PostCommitUpdateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPostUpdateEventListener[] postListeners = Session.Listeners.PostCommitUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostUpdateEvent postEvent = new PostUpdateEvent(Instance, Id, state, previousState, Persister, (IEventSource)Session);
				foreach (IPostUpdateEventListener listener in postListeners)
				{
					await (listener.OnPostUpdateAsync(postEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		private async Task<bool> PreUpdateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPreUpdateEventListener[] preListeners = Session.Listeners.PreUpdateEventListeners;
			bool veto = false;
			if (preListeners.Length > 0)
			{
				var preEvent = new PreUpdateEvent(Instance, Id, state, previousState, Persister, (IEventSource) Session);
				foreach (IPreUpdateEventListener listener in preListeners)
				{
					veto |= await (listener.OnPreUpdateAsync(preEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
			return veto;
		}
	}
}
