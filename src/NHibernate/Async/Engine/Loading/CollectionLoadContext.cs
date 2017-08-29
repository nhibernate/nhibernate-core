﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;

using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine.Loading
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class CollectionLoadContext
	{

		/// <summary> 
		/// Finish the process of collection-loading for this bound result set.  Mainly this
		/// involves cleaning up resources and notifying the collections that loading is
		/// complete. 
		/// </summary>
		/// <param name="persister">The persister for which to complete loading. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public async Task EndLoadingCollectionsAsync(ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!loadContexts.HasLoadingCollectionEntries && (localLoadingCollectionKeys.Count == 0))
			{
				return;
			}

			// in an effort to avoid concurrent-modification-exceptions (from
			// potential recursive calls back through here as a result of the
			// eventual call to PersistentCollection#endRead), we scan the
			// internal loadingCollections map for matches and store those matches
			// in a temp collection.  the temp collection is then used to "drive"
			// the #endRead processing.
			List<CollectionKey> toRemove = new List<CollectionKey>();
			List<LoadingCollectionEntry> matches =new List<LoadingCollectionEntry>();
			foreach (CollectionKey collectionKey in localLoadingCollectionKeys)
			{
				ISessionImplementor session = LoadContext.PersistenceContext.Session;

				LoadingCollectionEntry lce = loadContexts.LocateLoadingCollectionEntry(collectionKey);
				if (lce == null)
				{
					log.Warn("In CollectionLoadContext#endLoadingCollections, localLoadingCollectionKeys contained [" + collectionKey + "], but no LoadingCollectionEntry was found in loadContexts");
				}
				else if (lce.ResultSet == resultSet && lce.Persister == persister)
				{
					matches.Add(lce);
					if (lce.Collection.Owner == null)
					{
						session.PersistenceContext.AddUnownedCollection(new CollectionKey(persister, lce.Key),
																		lce.Collection);
					}
					if (log.IsDebugEnabled)
					{
						log.Debug("removing collection load entry [" + lce + "]");
					}

					// todo : i'd much rather have this done from #endLoadingCollection(CollectionPersister,LoadingCollectionEntry)...
					loadContexts.UnregisterLoadingCollectionXRef(collectionKey);
					toRemove.Add(collectionKey);
				}
			}
			localLoadingCollectionKeys.ExceptWith(toRemove);

			await (EndLoadingCollectionsAsync(persister, matches, cancellationToken)).ConfigureAwait(false);
			if ((localLoadingCollectionKeys.Count == 0))
			{
				// todo : hack!!!
				// NOTE : here we cleanup the load context when we have no more local
				// LCE entries.  This "works" for the time being because really
				// only the collection load contexts are implemented.  Long term,
				// this cleanup should become part of the "close result set"
				// processing from the (sandbox/jdbc) jdbc-container code.
				loadContexts.Cleanup(resultSet);
			}
		}

		private async Task EndLoadingCollectionsAsync(ICollectionPersister persister, IList<LoadingCollectionEntry> matchedCollectionEntries, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (matchedCollectionEntries == null || matchedCollectionEntries.Count == 0)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("no collections were found in result set for role: " + persister.Role);
				}
				return;
			}

			int count = matchedCollectionEntries.Count;
			if (log.IsDebugEnabled)
			{
				log.Debug(count + " collections were found in result set for role: " + persister.Role);
			}

			for (int i = 0; i < count; i++)
			{
				await (EndLoadingCollectionAsync(matchedCollectionEntries[i], persister, cancellationToken)).ConfigureAwait(false);
			}

			if (log.IsDebugEnabled)
			{
				log.Debug(count + " collections initialized for role: " + persister.Role);
			}
		}

		private async Task EndLoadingCollectionAsync(LoadingCollectionEntry lce, ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (log.IsDebugEnabled)
			{
				log.Debug("ending loading collection [" + lce + "]");
			}
			ISessionImplementor session = LoadContext.PersistenceContext.Session;

			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			bool hasNoQueuedAdds = lce.Collection.EndRead(persister); // warning: can cause a recursive calls! (proxy initialization)

			if (persister.CollectionType.HasHolder())
			{
				LoadContext.PersistenceContext.AddCollectionHolder(lce.Collection);
			}

			CollectionEntry ce = LoadContext.PersistenceContext.GetCollectionEntry(lce.Collection);
			if (ce == null)
			{
				ce = LoadContext.PersistenceContext.AddInitializedCollection(persister, lce.Collection, lce.Key);
			}
			else
			{
				ce.PostInitialize(lce.Collection);
			}

			bool addToCache = hasNoQueuedAdds && persister.HasCache && 
				session.CacheMode.HasFlag(CacheMode.Put) && !ce.IsDoremove; // and this is not a forced initialization during flush

			if (addToCache)
			{
				await (AddCollectionToCacheAsync(lce, persister, cancellationToken)).ConfigureAwait(false);
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("collection fully initialized: " + MessageHelper.CollectionInfoString(persister, lce.Collection, lce.Key, session));
			}

			if (statsEnabled)
			{
				stopWath.Stop();
				session.Factory.StatisticsImplementor.LoadCollection(persister.Role, stopWath.Elapsed);
			}
		}

		/// <summary> Add the collection to the second-level cache </summary>
		/// <param name="lce">The entry representing the collection to add </param>
		/// <param name="persister">The persister </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		private async Task AddCollectionToCacheAsync(LoadingCollectionEntry lce, ICollectionPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ISessionImplementor session = LoadContext.PersistenceContext.Session;
			ISessionFactoryImplementor factory = session.Factory;

			if (log.IsDebugEnabled)
			{
				log.Debug("Caching collection: " + MessageHelper.CollectionInfoString(persister, lce.Collection, lce.Key, session));
			}

			if (!(session.EnabledFilters.Count == 0) && persister.IsAffectedByEnabledFilters(session))
			{
				// some filters affecting the collection are enabled on the session, so do not do the put into the cache.
				log.Debug("Refusing to add to cache due to enabled filters");
				// todo : add the notion of enabled filters to the CacheKey to differentiate filtered collections from non-filtered;
				//      but CacheKey is currently used for both collections and entities; would ideally need to define two separate ones;
				//      currently this works in conjunction with the check on
				//      DefaultInitializeCollectionEventHandler.initializeCollectionFromCache() (which makes sure to not read from
				//      cache with enabled filters).
				return; // EARLY EXIT!!!!!
			}

			IComparer versionComparator;
			object version;
			if (persister.IsVersioned)
			{
				versionComparator = persister.OwnerEntityPersister.VersionType.Comparator;
				object collectionOwner = LoadContext.PersistenceContext.GetCollectionOwner(lce.Key, persister);
				version = LoadContext.PersistenceContext.GetEntry(collectionOwner).Version;
			}
			else
			{
				version = null;
				versionComparator = null;
			}

			CollectionCacheEntry entry = new CollectionCacheEntry(lce.Collection, persister);
			CacheKey cacheKey = session.GenerateCacheKey(lce.Key, persister.KeyType, persister.Role);
			bool put = await (persister.Cache.PutAsync(cacheKey, persister.CacheEntryStructure.Structure(entry), 
								session.Timestamp, version, versionComparator,
													factory.Settings.IsMinimalPutsEnabled && session.CacheMode != CacheMode.Refresh, cancellationToken)).ConfigureAwait(false);

			if (put && factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.SecondLevelCachePut(persister.Cache.RegionName);
			}
		}
	}
}
