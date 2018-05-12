using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;
using System.Collections.Generic;
using System.Linq;
using Iesi.Collections.Generic;

namespace NHibernate.Engine
{
	public partial class BatchFetchQueue
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(BatchFetchQueue));

		/// <summary>
		/// Used to hold information about the entities that are currently eligible for batch-fetching.  Ultimately
		/// used by <see cref="GetEntityBatch" /> to build entity load batches.
		/// </summary>
		/// <remarks>
		/// A Map structure is used to segment the keys by entity type since loading can only be done for a particular entity
		/// type at a time.
		/// </remarks>
		private readonly IDictionary<string, LinkedHashSet<EntityKey>> batchLoadableEntityKeys = new Dictionary<string, LinkedHashSet<EntityKey>>(8);

		/// <summary>
		/// A map of <see cref="SubselectFetch">subselect-fetch descriptors</see>
		/// keyed by the <see cref="EntityKey" /> against which the descriptor is
		/// registered.
		/// </summary>
		private readonly IDictionary<EntityKey, SubselectFetch> subselectsByEntityKey = new Dictionary<EntityKey, SubselectFetch>(8);

		private readonly IDictionary<string, LinkedHashMap<CollectionEntry, IPersistentCollection>> batchLoadableCollections = new Dictionary<string, LinkedHashMap<CollectionEntry, IPersistentCollection>>(8);
		/// <summary>
		/// The owning persistence context.
		/// </summary>
		private readonly IPersistenceContext context;

		/// <summary>
		/// Constructs a queue for the given context.
		/// </summary>
		/// <param name="context">The owning persistence context.</param>
		public BatchFetchQueue(IPersistenceContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Clears all entries from this fetch queue.
		/// </summary>
		public void Clear()
		{
			batchLoadableEntityKeys.Clear();
			batchLoadableCollections.Clear();
			subselectsByEntityKey.Clear();
		}

		/// <summary>
		/// Retrieve the fetch descriptor associated with the given entity key.
		/// </summary>
		/// <param name="key">The entity key for which to locate any defined subselect fetch.</param>
		/// <returns>The fetch descriptor; may return null if no subselect fetch queued for
		/// this entity key.</returns>
		public SubselectFetch GetSubselect(EntityKey key)
		{
			SubselectFetch result;
			subselectsByEntityKey.TryGetValue(key, out result);
			return result;
		}

		/// <summary>
		/// Adds a subselect fetch decriptor for the given entity key.
		/// </summary>
		/// <param name="key">The entity for which to register the subselect fetch.</param>
		/// <param name="subquery">The fetch descriptor.</param>
		public void AddSubselect(EntityKey key, SubselectFetch subquery)
		{
			subselectsByEntityKey[key] = subquery;
		}

		/// <summary>
		/// After evicting or deleting an entity, we don't need to
		/// know the query that was used to load it anymore (don't
		/// call this after loading the entity, since we might still
		/// need to load its collections)
		/// </summary>
		public void RemoveSubselect(EntityKey key)
		{
			subselectsByEntityKey.Remove(key);
		}

		/// <summary>
		/// Clears all pending subselect fetches from the queue.
		/// </summary>
		/// <remarks>
		/// Called after flushing.
		/// </remarks>
		public void ClearSubselects()
		{
			subselectsByEntityKey.Clear();
		}

		/// <summary>
		/// If an EntityKey represents a batch loadable entity, add
		/// it to the queue.
		/// </summary>
		/// <remarks>
		/// Note that the contract here is such that any key passed in should
		/// previously have been been checked for existence within the
		/// <see cref="ISession" />; failure to do so may cause the
		/// referenced entity to be included in a batch even though it is
		/// already associated with the <see cref="ISession" />.
		/// </remarks>
		public void AddBatchLoadableEntityKey(EntityKey key)
		{
			if (key.IsBatchLoadable)
			{
				if (!batchLoadableEntityKeys.TryGetValue(key.EntityName, out var set))
				{
					set = new LinkedHashSet<EntityKey>();
					batchLoadableEntityKeys.Add(key.EntityName, set);
				}
				set.Add(key);
			}
		}

		/// <summary>
		/// After evicting or deleting or loading an entity, we don't
		/// need to batch fetch it anymore, remove it from the queue
		/// if necessary
		/// </summary>
		public void RemoveBatchLoadableEntityKey(EntityKey key)
		{
			if (key.IsBatchLoadable)
			{
				if (batchLoadableEntityKeys.TryGetValue(key.EntityName, out var set))
				{
					set.Remove(key);
				}
			}
			// A subclass will be added to the batch by the root entity name, when querying by the root entity.
			// When removing a subclass key, we need to consider that the subclass may not be batchable but
			// its root class may be. In order to prevent having in batch entity keys that are already loaded,
			// we have to try to remove the key by the root entity, even if the subclass is not batchable.
			if (key.RootEntityName != key.EntityName)
			{
				if (batchLoadableEntityKeys.TryGetValue(key.RootEntityName, out var set))
				{
					set.Remove(key);
				}
			}
		}

		/// <summary>
		/// If a CollectionEntry represents a batch loadable collection, add
		/// it to the queue.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="ce"></param>
		public void AddBatchLoadableCollection(IPersistentCollection collection, CollectionEntry ce)
		{
			var persister = ce.LoadedPersister;

			if (!batchLoadableCollections.TryGetValue(persister.Role, out var map))
			{
				map = new LinkedHashMap<CollectionEntry, IPersistentCollection>();
				batchLoadableCollections.Add(persister.Role, map);
			}
			map[ce] = collection;
		}

		/// <summary>
		/// Retrives the uninitialized persistent collection from the queue.
		/// </summary>
		/// <param name="persister">The collection persister.</param>
		/// <param name="ce">The collection entry.</param>
		/// <returns>A persistent collection if found, <see langword="null"/> otherwise.</returns>
		internal IPersistentCollection GetBatchLoadableCollection(ICollectionPersister persister, CollectionEntry ce)
		{
			if (!batchLoadableCollections.TryGetValue(persister.Role, out var map))
			{
				return null;
			}
			if (!map.TryGetValue(ce, out var collection))
			{
				return null;
			}
			return collection;
		}

		/// <summary>
		/// After a collection was initialized or evicted, we don't
		/// need to batch fetch it anymore, remove it from the queue
		/// if necessary
		/// </summary>
		/// <param name="ce"></param>
		public void RemoveBatchLoadableCollection(CollectionEntry ce)
		{
			if (batchLoadableCollections.TryGetValue(ce.LoadedPersister.Role, out var map))
			{
				map.Remove(ce);
			}
		}

		/// <summary>
		/// Get a batch of uninitialized collection keys for a given role
		/// </summary>
		/// <param name="collectionPersister">The persister for the collection role.</param>
		/// <param name="id">A key that must be included in the batch fetch</param>
		/// <param name="batchSize">the maximum number of keys to return</param>
		/// <returns>an array of collection keys, of length batchSize (padded with nulls)</returns>
		public object[] GetCollectionBatch(ICollectionPersister collectionPersister, object id, int batchSize)
		{
			return GetCollectionBatch(collectionPersister, id, batchSize, true, null);
		}

		/// <summary>
		/// Get a batch of uninitialized collection keys for a given role
		/// </summary>
		/// <param name="collectionPersister">The persister for the collection role.</param>
		/// <param name="id">A key that must be included in the batch fetch</param>
		/// <param name="batchSize">the maximum number of keys to return</param>
		/// <param name="checkCache">Whether to check the cache for uninitialized collection keys.</param>
		/// <param name="collectionEntries">An array that will be filled with collection entries if set.</param>
		/// <returns>An array of collection keys, of length <paramref name="batchSize"/> (padded with nulls)</returns>
		internal object[] GetCollectionBatch(ICollectionPersister collectionPersister, object id, int batchSize, bool checkCache,
		                                     CollectionEntry[] collectionEntries)
		{
			var keys = new object[batchSize];
			keys[0] = id; // The first element of array is reserved for the actual instance we are loading
			var i = 1; // The current index of keys array
			int? keyIndex = null; // The index of the demanding key in the linked hash set
			var checkForEnd = false; // Stores whether we found the demanded collection and reached the batchSize
			var index = 0; // The current index of the linked hash set iteration
			// List of collection entries that haven't been checked for their existance in the cache. Besides the collection entry,
			// the index where the entry was found is also stored in order to correctly order the returning keys.
			var collectionKeys = new List<KeyValuePair<KeyValuePair<CollectionEntry, IPersistentCollection>, int>>(batchSize);
			var batchableCache = collectionPersister.Cache?.Cache as IBatchableReadCache;

			if (!batchLoadableCollections.TryGetValue(collectionPersister.Role, out var map))
			{
				return keys;
			}

			foreach (KeyValuePair<CollectionEntry, IPersistentCollection> me in map)
			{
				if (ProcessKey(me))
				{
					return keys;
				}
				index++;
			}

			// If by the end of the iteration we haven't filled the whole array of keys to fetch,
			// we have to check the remaining collection keys.
			while (i != batchSize && collectionKeys.Count > 0)
			{
				if (CheckCacheAndProcessResult())
				{
					return keys;
				}
			}

			return keys; //we ran out of keys to try

			// Calls the cache to check if any of the keys is cached and continues the key processing for those
			// that are not stored in the cache.
			bool CheckCacheAndProcessResult()
			{
				var fromIndex = batchableCache != null
					? collectionKeys.Count - Math.Min(batchSize, collectionKeys.Count)
					: 0;
				var toIndex = collectionKeys.Count - 1;
				var indexes = GetSortedKeyIndexes(collectionKeys, keyIndex.Value, fromIndex, toIndex);
				if (batchableCache == null)
				{
					for (var j = 0; j < collectionKeys.Count; j++)
					{
						if (ProcessKey(collectionKeys[indexes[j]].Key))
						{
							return true;
						}
					}
				}
				else
				{
					var results = AreCached(collectionKeys, indexes, collectionPersister, batchableCache, checkCache);
					var k = toIndex;
					for (var j = 0; j < results.Length; j++)
					{
						if (!results[j] && ProcessKey(collectionKeys[indexes[j]].Key, true))
						{
							return true;
						}
					}
				}

				for (var j = toIndex; j >= fromIndex; j--)
				{
					collectionKeys.RemoveAt(j);
				}
				return false;
			}

			bool ProcessKey(KeyValuePair<CollectionEntry, IPersistentCollection> me, bool ignoreCache = false)
			{
				var ce = me.Key;
				var collection = me.Value;
				if (ce.LoadedKey == null)
				{
					// the LoadedKey of the CollectionEntry might be null as it might have been reset to null
					// (see for example Collections.ProcessDereferencedCollection()
					// and CollectionEntry.AfterAction())
					// though we clear the queue on flush, it seems like a good idea to guard
					// against potentially null LoadedKey:s
					return false;
				}

				if (collection.WasInitialized)
				{
					log.Warn("Encountered initialized collection in BatchFetchQueue, this should not happen.");
					return false;
				}

				if (checkForEnd && (index >= keyIndex.Value + batchSize || index == map.Count))
				{
					return true;
				}
				if (collectionPersister.KeyType.IsEqual(id, ce.LoadedKey, collectionPersister.Factory))
				{
					if (collectionEntries != null)
					{
						collectionEntries[0] = ce;
					}
					keyIndex = index;
				}
				else if (!checkCache || batchableCache == null)
				{
					if (!keyIndex.HasValue || index < keyIndex.Value)
					{
						collectionKeys.Add(new KeyValuePair<KeyValuePair<CollectionEntry, IPersistentCollection>, int>(me, index));
						return false;
					}

					if (!checkCache || !IsCached(ce.LoadedKey, collectionPersister))
					{
						if (collectionEntries != null)
						{
							collectionEntries[i] = ce;
						}
						keys[i++] = ce.LoadedKey;
					}
				}
				else if (ignoreCache)
				{
					if (collectionEntries != null)
					{
						collectionEntries[i] = ce;
					}
					keys[i++] = ce.LoadedKey;
				}
				else
				{
					collectionKeys.Add(new KeyValuePair<KeyValuePair<CollectionEntry, IPersistentCollection>, int>(me, index));
					// Check the cache only when we have collected as many keys as are needed to fill the batch,
					// that are after the demanded key.
					if (!keyIndex.HasValue || index < keyIndex.Value + batchSize)
					{
						return false;
					}
					return CheckCacheAndProcessResult();
				}
				if (i == batchSize)
				{
					i = 1; // End of array, start filling again from start
					if (keyIndex.HasValue)
					{
						checkForEnd = true;
						return index >= keyIndex.Value + batchSize || index == map.Count;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Get a batch of unloaded identifiers for this class, using a slightly
		/// complex algorithm that tries to grab keys registered immediately after
		/// the given key.
		/// </summary>
		/// <param name="persister">The persister for the entities being loaded.</param>
		/// <param name="id">The identifier of the entity currently demanding load.</param>
		/// <param name="batchSize">The maximum number of keys to return</param>
		/// <returns>an array of identifiers, of length batchSize (possibly padded with nulls)</returns>
		public object[] GetEntityBatch(IEntityPersister persister, object id, int batchSize)
		{
			return GetEntityBatch(persister, id, batchSize, true);
		}

		/// <summary>
		/// Get a batch of unloaded identifiers for this class, using a slightly
		/// complex algorithm that tries to grab keys registered immediately after
		/// the given key.
		/// </summary>
		/// <param name="persister">The persister for the entities being loaded.</param>
		/// <param name="id">The identifier of the entity currently demanding load.</param>
		/// <param name="batchSize">The maximum number of keys to return</param>
		/// <param name="checkCache">Whether to check the cache for uninitialized keys.</param>
		/// <returns>An array of identifiers, of length <paramref name="batchSize"/> (possibly padded with nulls)</returns>
		internal object[] GetEntityBatch(IEntityPersister persister, object id, int batchSize, bool checkCache)
		{
			var ids = new object[batchSize];
			ids[0] = id; // The first element of array is reserved for the actual instance we are loading
			var i = 1; // The current index of ids array
			int? idIndex = null; // The index of the demanding id in the linked hash set
			var checkForEnd = false; // Stores whether we found the demanded id and reached the batchSize
			var index = 0; // The current index of the linked hash set iteration
			// List of entity keys that haven't been checked for their existance in the cache. Besides the entity key,
			// the index where the key was found is also stored in order to correctly order the returning keys.
			var entityKeys = new List<KeyValuePair<EntityKey, int>>(batchSize);
			var batchableCache = persister.Cache?.Cache as IBatchableReadCache;

			if (!batchLoadableEntityKeys.TryGetValue(persister.EntityName, out var set))
			{
				return ids;
			}

			foreach (var key in set)
			{
				if (ProcessKey(key))
				{
					return ids;
				}
				index++;
			}

			// If by the end of the iteration we haven't filled the whole array of ids to fetch,
			// we have to check the remaining entity keys.
			while (i != batchSize && entityKeys.Count > 0)
			{
				if (CheckCacheAndProcessResult())
				{
					return ids;
				}
			}

			return ids;

			// Calls the cache to check if any of the keys is cached and continues the key processing for those
			// that are not stored in the cache.
			bool CheckCacheAndProcessResult()
			{
				var fromIndex = batchableCache != null
					? entityKeys.Count - Math.Min(batchSize, entityKeys.Count)
					: 0;
				var toIndex = entityKeys.Count - 1;
				var indexes = GetSortedKeyIndexes(entityKeys, idIndex.Value, fromIndex, toIndex);
				if (batchableCache == null)
				{
					for (var j = 0; j < entityKeys.Count; j++)
					{
						if (ProcessKey(entityKeys[indexes[j]].Key))
						{
							return true;
						}
					}
				}
				else
				{
					var results = AreCached(entityKeys, indexes, persister, batchableCache, checkCache);
					var k = toIndex;
					for (var j = 0; j < results.Length; j++)
					{
						if (!results[j] && ProcessKey(entityKeys[indexes[j]].Key, true))
						{
							return true;
						}
					}
				}

				for (var j = toIndex; j >= fromIndex; j--)
				{
					entityKeys.RemoveAt(j);
				}
				return false;
			}

			bool ProcessKey(EntityKey key, bool ignoreCache = false)
			{
				//TODO: this needn't exclude subclasses...
				if (checkForEnd && (index >= idIndex.Value + batchSize || index == set.Count))
				{
					return true;
				}
				if (persister.IdentifierType.IsEqual(id, key.Identifier))
				{
					idIndex = index;
				}
				else if (!checkCache || batchableCache == null)
				{
					if (!idIndex.HasValue || index < idIndex.Value)
					{
						entityKeys.Add(new KeyValuePair<EntityKey, int>(key, index));
						return false;
					}

					if (!checkCache || !IsCached(key, persister))
					{
						ids[i++] = key.Identifier;
					}
				}
				else if (ignoreCache)
				{
					ids[i++] = key.Identifier;
				}
				else
				{
					entityKeys.Add(new KeyValuePair<EntityKey, int>(key, index));
					// Check the cache only when we have collected as many keys as are needed to fill the batch,
					// that are after the demanded key.
					if (!idIndex.HasValue || index < idIndex.Value + batchSize)
					{
						return false;
					}
					return CheckCacheAndProcessResult();
				}
				if (i == batchSize)
				{
					i = 1; // End of array, start filling again from start
					if (idIndex.HasValue)
					{
						checkForEnd = true;
						return index >= idIndex.Value + batchSize || index == set.Count;
					}
				}
				return false;
			}
		}

		private bool IsCached(EntityKey entityKey, IEntityPersister persister)
		{
			if (persister.HasCache && context.Session.CacheMode.HasFlag(CacheMode.Get))
			{
				CacheKey key = context.Session.GenerateCacheKey(entityKey.Identifier, persister.IdentifierType, entityKey.EntityName);
				return persister.Cache.Cache.Get(key) != null;
			}
			return false;
		}

		private bool IsCached(object collectionKey, ICollectionPersister persister)
		{
			if (persister.HasCache && context.Session.CacheMode.HasFlag(CacheMode.Get))
			{
				CacheKey cacheKey = context.Session.GenerateCacheKey(collectionKey, persister.KeyType, persister.Role);
				return persister.Cache.Cache.Get(cacheKey) != null;
			}
			return false;
		}

		/// <summary>
		/// Checks whether the given entity key indexes are cached.
		/// </summary>
		/// <param name="entityKeys">The list of pairs of entity keys and thier indexes.</param>
		/// <param name="keyIndexes">The array of indexes of <paramref name="entityKeys"/> that have to be checked.</param>
		/// <param name="persister">The entity persister.</param>
		/// <param name="batchableCache">The batchable cache.</param>
		/// <param name="checkCache">Whether to check the cache or just return <see langword="false" /> for all keys.</param>
		/// <returns>An array of booleans that contains the result for each key.</returns>
		private bool[] AreCached(List<KeyValuePair<EntityKey, int>> entityKeys, int[] keyIndexes, IEntityPersister persister,
		                         IBatchableReadCache batchableCache, bool checkCache)
		{
			var result = new bool[keyIndexes.Length];
			if (!checkCache || !persister.HasCache || !context.Session.CacheMode.HasFlag(CacheMode.Get))
			{
				return result;
			}
			var cacheKeys = new object[keyIndexes.Length];
			var i = 0;
			foreach (var index in keyIndexes)
			{
				var entityKey = entityKeys[index].Key;
				cacheKeys[i++] = context.Session.GenerateCacheKey(
					entityKey.Identifier,
					persister.IdentifierType,
					entityKey.EntityName);
			}
			var cacheResult = batchableCache.GetMany(cacheKeys);
			for (var j = 0; j < result.Length; j++)
			{
				result[j] = cacheResult[j] != null;
			}

			return result;
		}

		/// <summary>
		/// Checks whether the given collection key indexes are cached.
		/// </summary>
		/// <param name="collectionKeys">The list of pairs of collection entries and thier indexes.</param>
		/// <param name="keyIndexes">The array of indexes of <paramref name="collectionKeys"/> that have to be checked.</param>
		/// <param name="persister">The collection persister.</param>
		/// <param name="batchableCache">The batchable cache.</param>
		/// <param name="checkCache">Whether to check the cache or just return <see langword="false" /> for all keys.</param>
		/// <returns>An array of booleans that contains the result for each key.</returns>
		private bool[] AreCached(List<KeyValuePair<KeyValuePair<CollectionEntry, IPersistentCollection>, int>> collectionKeys,
		                         int[] keyIndexes, ICollectionPersister persister, IBatchableReadCache batchableCache,
		                         bool checkCache)
		{
			var result = new bool[keyIndexes.Length];
			if (!checkCache || !persister.HasCache || !context.Session.CacheMode.HasFlag(CacheMode.Get))
			{
				return result;
			}
			var cacheKeys = new object[keyIndexes.Length];
			var i = 0;
			foreach (var index in keyIndexes)
			{
				var collectionKey = collectionKeys[index].Key;
				cacheKeys[i++] = context.Session.GenerateCacheKey(
					collectionKey.Key.LoadedKey,
					persister.KeyType,
					persister.Role);
			}
			var cacheResult = batchableCache.GetMany(cacheKeys);
			for (var j = 0; j < result.Length; j++)
			{
				result[j] = cacheResult[j] != null;
			}

			return result;
		}

		/// <summary>
		/// Sorts the given keys by thier indexes, where the keys that are after the demanded key will be located
		/// at the start and the remaining indexes at the end of the returned array.
		/// </summary>
		/// <typeparam name="T">The type of the key</typeparam>
		/// <param name="keys">The list of pairs of keys and thier indexes.</param>
		/// <param name="keyIndex">The index of the demanded key</param>
		/// <param name="fromIndex">The index where the sorting will begin.</param>
		/// <param name="toIndex">The index where the sorting will end.</param>
		/// <returns>An array of sorted key indexes.</returns>
		private static int[] GetSortedKeyIndexes<T>(List<KeyValuePair<T, int>> keys, int keyIndex, int fromIndex, int toIndex)
		{
			var result = new int[Math.Abs(toIndex - fromIndex) + 1];
			var lowerIndexes = new List<int>();
			var i = 0;
			for (var j = fromIndex; j <= toIndex; j++)
			{
				if (keys[j].Value < keyIndex)
				{
					lowerIndexes.Add(j);
				}
				else
				{
					result[i++] = j;
				}
			}
			for (var j = lowerIndexes.Count - 1; j >= 0; j--)
			{
				result[i++] = lowerIndexes[j];
			}
			return result;
		}
	}
}
