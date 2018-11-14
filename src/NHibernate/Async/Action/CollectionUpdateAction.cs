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
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	using System.Threading.Tasks;
	using System.Threading;
	public sealed partial class CollectionUpdateAction : CollectionAction
	{

		public override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object id = Key;
			ISessionImplementor session = Session;
			ICollectionPersister persister = Persister;
			IPersistentCollection collection = Collection;
			bool affectedByFilters = persister.IsAffectedByEnabledFilters(session);

			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}

			await (PreUpdateAsync(cancellationToken)).ConfigureAwait(false);

			if (!collection.WasInitialized)
			{
				if (!collection.HasQueuedOperations)
				{
					throw new AssertionFailure("no queued adds");
				}
				//do nothing - we only need to notify the cache...
			}
			else if (!affectedByFilters && collection.Empty)
			{
				if (!emptySnapshot)
				{
					await (persister.RemoveAsync(id, session, cancellationToken)).ConfigureAwait(false);
				}
			}
			else if (collection.NeedsRecreate(persister))
			{
				if (affectedByFilters)
				{
					throw new HibernateException("cannot recreate collection while filter is enabled: "
												 + MessageHelper.CollectionInfoString(persister, collection, id, session));
				}
				if (!emptySnapshot)
				{
					await (persister.RemoveAsync(id, session, cancellationToken)).ConfigureAwait(false);
				}
				await (persister.RecreateAsync(collection, id, session, cancellationToken)).ConfigureAwait(false);
			}
			else
			{
				await (persister.DeleteRowsAsync(collection, id, session, cancellationToken)).ConfigureAwait(false);
				await (persister.UpdateRowsAsync(collection, id, session, cancellationToken)).ConfigureAwait(false);
				await (persister.InsertRowsAsync(collection, id, session, cancellationToken)).ConfigureAwait(false);
			}

			await (Session.PersistenceContext.GetCollectionEntry(collection).AfterActionAsync(collection, session, cancellationToken)).ConfigureAwait(false);

			await (EvictAsync(cancellationToken)).ConfigureAwait(false);

			await (PostUpdateAsync(cancellationToken)).ConfigureAwait(false);

			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.UpdateCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private async Task PreUpdateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPreCollectionUpdateEventListener[] preListeners = Session.Listeners.PreCollectionUpdateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionUpdateEvent preEvent = new PreCollectionUpdateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					await (preListeners[i].OnPreUpdateCollectionAsync(preEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		private async Task PostUpdateAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPostCollectionUpdateEventListener[] postListeners = Session.Listeners.PostCollectionUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionUpdateEvent postEvent = new PostCollectionUpdateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					await (postListeners[i].OnPostUpdateCollectionAsync(postEvent, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		public override async Task ExecuteAfterTransactionCompletionAsync(bool success, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// NH Different behavior: to support unlocking collections from the cache.(r3260)

			CacheKey ck = Session.GenerateCacheKey(Key, Persister.KeyType, Persister.Role);

			if (success)
			{
				// we can't disassemble a collection if it was uninitialized 
				// or detached from the session
				if (Collection.WasInitialized && Session.PersistenceContext.ContainsCollection(Collection))
				{
					CollectionCacheEntry entry = await (CollectionCacheEntry.CreateAsync(Collection, Persister, cancellationToken)).ConfigureAwait(false);
					bool put = await (Persister.Cache.AfterUpdateAsync(ck, entry, null, Lock, cancellationToken)).ConfigureAwait(false);

					if (put && Session.Factory.Statistics.IsStatisticsEnabled)
					{
						Session.Factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
					}
				}
			}
			else
			{
				await (Persister.Cache.ReleaseAsync(ck, Lock, cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}
