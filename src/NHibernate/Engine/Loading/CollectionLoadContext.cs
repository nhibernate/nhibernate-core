using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Engine.Loading
{
	/// <summary> 
	/// Represents state associated with the processing of a given <see cref="IDataReader"/>
	/// in regards to loading collections.
	/// </summary>
	/// <remarks>
	/// Another implementation option to consider is to not expose <see cref="IDataReader">ResultSets</see>
	/// directly (in the JDBC redesign) but to always "wrap" them and apply a [series of] context[s] to that wrapper.
	/// </remarks>
	public class CollectionLoadContext
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(CollectionLoadContext));
		private readonly LoadContexts loadContexts;
		private readonly IDataReader resultSet;
		private readonly ISet<CollectionKey> localLoadingCollectionKeys = new HashSet<CollectionKey>();

		/// <summary> 
		/// Creates a collection load context for the given result set. 
		/// </summary>
		/// <param name="loadContexts">Callback to other collection load contexts. </param>
		/// <param name="resultSet">The result set this is "wrapping".</param>
		public CollectionLoadContext(LoadContexts loadContexts, IDataReader resultSet)
		{
			this.loadContexts = loadContexts;
			this.resultSet = resultSet;
		}

		public LoadContexts LoadContext
		{
			get { return loadContexts; }
		}

		public IDataReader ResultSet
		{
			get { return resultSet; }
		}

		/// <summary> 
		/// Retrieve the collection that is being loaded as part of processing this result set. 
		/// </summary>
		/// <param name="persister">The persister for the collection being requested. </param>
		/// <param name="key">The key of the collection being requested. </param>
		/// <returns> The loading collection (see discussion above). </returns>
		/// <remarks>
		/// Basically, there are two valid return values from this method:<ul>
		/// <li>an instance of {@link PersistentCollection} which indicates to
		/// continue loading the result set row data into that returned collection
		/// instance; this may be either an instance already associated and in the
		/// midst of being loaded, or a newly instantiated instance as a matching
		/// associated collection was not found.</li>
		/// <li><i>null</i> indicates to ignore the corresponding result set row
		/// data relating to the requested collection; this indicates that either
		/// the collection was found to already be associated with the persistence
		/// context in a fully loaded state, or it was found in a loading state
		/// associated with another result set processing context.</li>
		/// </ul>
		/// </remarks>
		public IPersistentCollection GetLoadingCollection(ICollectionPersister persister, object key)
		{
			EntityMode em = loadContexts.PersistenceContext.Session.EntityMode;

			CollectionKey collectionKey = new CollectionKey(persister, key, em);
			if (log.IsDebugEnabled)
			{
				log.Debug("starting attempt to find loading collection [" + MessageHelper.InfoString(persister.Role, key) + "]");
			}
			LoadingCollectionEntry loadingCollectionEntry = loadContexts.LocateLoadingCollectionEntry(collectionKey);
			if (loadingCollectionEntry == null)
			{
				// look for existing collection as part of the persistence context
				IPersistentCollection collection = loadContexts.PersistenceContext.GetCollection(collectionKey);
				if (collection != null)
				{
					if (collection.WasInitialized)
					{
						log.Debug("collection already initialized; ignoring");
						return null; // ignore this row of results! Note the early exit
					}
					else
					{
						// initialize this collection
						log.Debug("collection not yet initialized; initializing");
					}
				}
				else
				{
					object owner = loadContexts.PersistenceContext.GetCollectionOwner(key, persister);
					bool newlySavedEntity = owner != null && loadContexts.PersistenceContext.GetEntry(owner).Status != Status.Loading && em != EntityMode.Xml;
					if (newlySavedEntity)
					{
						// important, to account for newly saved entities in query
						// todo : some kind of check for new status...
						log.Debug("owning entity already loaded; ignoring");
						return null;
					}
					else
					{
						// create one
						if (log.IsDebugEnabled)
						{
							log.Debug("instantiating new collection [key=" + key + ", rs=" + resultSet + "]");
						}
						collection = persister.CollectionType.Instantiate(loadContexts.PersistenceContext.Session, persister, key);
					}
				}
				collection.BeforeInitialize(persister, -1);
				collection.BeginRead();
				localLoadingCollectionKeys.Add(collectionKey);
				loadContexts.RegisterLoadingCollectionXRef(collectionKey, new LoadingCollectionEntry(resultSet, persister, key, collection));
				return collection;
			}
			else
			{
				if (loadingCollectionEntry.ResultSet == resultSet)
				{
					log.Debug("found loading collection bound to current result set processing; reading row");
					return loadingCollectionEntry.Collection;
				}
				else
				{
					// ignore this row, the collection is in process of
					// being loaded somewhere further "up" the stack
					log.Debug("collection is already being initialized; ignoring row");
					return null;
				}
			}
		}

		/// <summary> 
		/// Finish the process of collection-loading for this bound result set.  Mainly this
		/// involves cleaning up resources and notifying the collections that loading is
		/// complete. 
		/// </summary>
		/// <param name="persister">The persister for which to complete loading. </param>
		public void EndLoadingCollections(ICollectionPersister persister)
		{
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
						session.PersistenceContext.AddUnownedCollection(new CollectionKey(persister, lce.Key, session.EntityMode),
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

			EndLoadingCollections(persister, matches);
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

		private void EndLoadingCollections(ICollectionPersister persister, IList<LoadingCollectionEntry> matchedCollectionEntries)
		{
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
				EndLoadingCollection(matchedCollectionEntries[i], persister);
			}

			if (log.IsDebugEnabled)
			{
				log.Debug(count + " collections initialized for role: " + persister.Role);
			}
		}

		private void EndLoadingCollection(LoadingCollectionEntry lce, ICollectionPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("ending loading collection [" + lce + "]");
			}
			ISessionImplementor session = LoadContext.PersistenceContext.Session;
			EntityMode em = session.EntityMode;

			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			bool hasNoQueuedAdds = lce.Collection.EndRead(persister); // warning: can cause a recursive calls! (proxy initialization)

			if (persister.CollectionType.HasHolder(em))
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
				((session.CacheMode & CacheMode.Put) == CacheMode.Put) && !ce.IsDoremove; // and this is not a forced initialization during flush

			if (addToCache)
			{
				AddCollectionToCache(lce, persister);
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
		private void AddCollectionToCache(LoadingCollectionEntry lce, ICollectionPersister persister)
		{
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
			bool put = persister.Cache.Put(cacheKey, persister.CacheEntryStructure.Structure(entry), 
								session.Timestamp, version, versionComparator,
													factory.Settings.IsMinimalPutsEnabled && session.CacheMode != CacheMode.Refresh);

			if (put && factory.Statistics.IsStatisticsEnabled)
			{
				factory.StatisticsImplementor.SecondLevelCachePut(persister.Cache.RegionName);
			}
		}

		internal void Cleanup()
		{
			if (!(localLoadingCollectionKeys.Count == 0))
			{
				log.Warn("On CollectionLoadContext#cleanup, localLoadingCollectionKeys contained [" + localLoadingCollectionKeys.Count + "] entries");
			}
			LoadContext.CleanupCollectionXRefs(localLoadingCollectionKeys);
			localLoadingCollectionKeys.Clear();
		}

		public override string ToString()
		{
			return base.ToString() + "<rs=" + ResultSet + ">";
		}
	}
}
