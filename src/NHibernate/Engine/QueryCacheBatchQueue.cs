using System;
using System.Collections.Generic;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Engine
{
	/// <summary>
	/// A batcher used to retrieve a batch of entity or collection keys that are present in the cached query.
	/// </summary>
	internal class QueryCacheBatchQueue
	{
		private readonly IPersistenceContext _persistenceContext;

		/// <summary>
		/// Used to hold information about the entities that are currently eligible for batch-fetching. Ultimately
		/// used by <see cref="GetEntityBatch" /> to build entity load batches.
		/// </summary>
		private readonly IDictionary<string, HashSet<EntityKey>> _queryEntityKeys;

		/// <summary>
		/// Used to hold information about entity keys that were checked in the cache.
		/// </summary>
		private readonly IDictionary<string, HashSet<EntityKey>> _queryCheckedEntityKeys;

		/// <summary>
		/// Used to hold information about collection entries that are currently eligible for batch-fetching. Ultimately
		/// used by <see cref="GetCollectionBatch" /> to build collection load batches.
		/// </summary>
		private readonly IDictionary<string, IDictionary<CollectionKey, CollectionEntry>> _queryCollectionKeys;

		/// <summary>
		/// Used to hold information about collection keys that were checked in the cache.
		/// </summary>
		private readonly IDictionary<string, HashSet<CollectionKey>> _queryCheckedCollectionKeys;

		/// <summary>
		/// Used to hold information about collection entries that were checked in the cache.
		/// </summary>
		private readonly IDictionary<string, HashSet<CollectionEntry>> _queryCheckedCollectionEntries;

		internal QueryCacheBatchQueue(IPersistenceContext persistenceContext)
		{
			_persistenceContext = persistenceContext;
			_queryEntityKeys = new Dictionary<string, HashSet<EntityKey>>();
			_queryCheckedEntityKeys = new Dictionary<string, HashSet<EntityKey>>();
			_queryCollectionKeys = new Dictionary<string, IDictionary<CollectionKey, CollectionEntry>>();
			_queryCheckedCollectionKeys = new Dictionary<string, HashSet<CollectionKey>>();
			_queryCheckedCollectionEntries = new Dictionary<string, HashSet<CollectionEntry>>();
		}

		/// <summary>
		/// Get a batch of all unloaded identifiers for a given persister that are present in the cached query.
		/// Once this method is called the unloaded identifiers for a given persister will be cleared in order to prevent
		/// double checking the same identifier.
		/// </summary>
		/// <param name="persister">The persister for the entities being loaded.</param>
		/// <param name="id">The identifier of the entity currently demanding load.</param>
		/// <returns>
		/// An array of identifiers that can be empty if the identifier was already checked or <see langword="null" />
		/// if the identifier is not present in the cached query.
		/// </returns>
		internal object[] GetEntityBatch(IEntityPersister persister, object id)
		{
			if (!_queryEntityKeys.TryGetValue(persister.EntityName, out var entityKeys))
			{
				return null; // The entity was not present in the cached query
			}

			var entityKey = new EntityKey(id, persister);
			if (_queryCheckedEntityKeys.TryGetValue(persister.EntityName, out var checkedEntityKeys) &&
				checkedEntityKeys.Contains(entityKey))
			{
				return Array.Empty<object>();
			}

			if (!entityKeys.Contains(entityKey))
			{
				return null; // The entity was not present in the cached query
			}

			var result = new object[entityKeys.Count];
			var i = 0;
			result[i++] = id;

			if (checkedEntityKeys == null)
			{
				checkedEntityKeys = new HashSet<EntityKey>();
				_queryCheckedEntityKeys.Add(persister.EntityName, checkedEntityKeys);
			}

			foreach (var key in entityKeys)
			{
				if (persister.IdentifierType.IsEqual(id, key.Identifier) || _persistenceContext.ContainsEntity(key))
				{
					continue;
				}

				result[i++] = key.Identifier;
				checkedEntityKeys.Add(key);
			}

			entityKeys.Clear();

			return result;
		}

		/// <summary>
		/// Get a batch of all uninitialized collection keys for a given role that are present in the cached query.
		/// Once this method is called the uninitialized collection keys for a given role will be cleared in order to prevent
		/// double checking the same keys.
		/// </summary>
		/// <param name="collectionPersister">The persister for the collection role.</param>
		/// <param name="key">A key that must be included in the batch fetch.</param>
		/// <param name="collectionEntries">An array that will be filled with collection entries if set.</param>
		/// <returns>
		/// An array of collection keys that can be empty if the key was already checked or <see langword="null" />
		/// if the key is not present in the cached query.
		/// </returns>
		internal object[] GetCollectionBatch(ICollectionPersister collectionPersister, object key, out CollectionEntry[] collectionEntries)
		{
			if (!_queryCollectionKeys.TryGetValue(collectionPersister.Role, out var keys))
			{
				collectionEntries = null;
				return null; // The collection was not present in the cached query
			}

			var collectionKey = new CollectionKey(collectionPersister, key);
			if (_queryCheckedCollectionKeys.TryGetValue(collectionPersister.Role, out var checkedKeys) &&
				checkedKeys.Contains(collectionKey))
			{
				collectionEntries = null;
				return Array.Empty<object>();
			}

			if (!keys.TryGetValue(collectionKey, out var collectionEntry) || collectionEntry == null)
			{
				collectionEntries = null;
				return null; // The collection was not present in the cached query
			}

			if (checkedKeys == null)
			{
				checkedKeys = new HashSet<CollectionKey>();
				_queryCheckedCollectionKeys.Add(collectionPersister.Role, checkedKeys);
			}

			if (!_queryCheckedCollectionEntries.TryGetValue(collectionPersister.Role, out var checkedEntries))
			{
				checkedEntries = new HashSet<CollectionEntry>();
				_queryCheckedCollectionEntries.Add(collectionPersister.Role, checkedEntries);
			}

			var result = new object[keys.Count];
			collectionEntries = new CollectionEntry[result.Length];
			var i = 0;
			result[i++] = key;

			foreach (var pair in keys)
			{
				if (pair.Value == null || _persistenceContext.GetCollection(pair.Key)?.WasInitialized != false)
				{
					continue; // The collection was not registered or is already initialized
				}

				if (collectionPersister.KeyType.IsEqual(key, pair.Value.LoadedKey, collectionPersister.Factory))
				{
					collectionEntries[0] = pair.Value;
					checkedKeys.Add(pair.Key);
					checkedEntries.Add(pair.Value);
					continue;
				}

				collectionEntries[i] = pair.Value;
				result[i++] = pair.Value.LoadedKey;
				checkedKeys.Add(pair.Key);
				checkedEntries.Add(pair.Value);
			}

			keys.Clear();

			return result;
		}

		/// <summary>
		/// Adds the entity to the batch.
		/// </summary>
		/// <param name="key">The entity key.</param>
		internal void AddEntityKey(EntityKey key)
		{
			if (!_queryEntityKeys.TryGetValue(key.EntityName, out var querySet))
			{
				querySet = new HashSet<EntityKey>();
				_queryEntityKeys.Add(key.EntityName, querySet);
			}

			querySet.Add(key);
		}

		/// <summary>
		/// Adds the collection to the batch.
		/// </summary>
		/// <param name="persister">The collection persister.</param>
		/// <param name="ce">The collection entry.</param>
		internal void AddCollection(ICollectionPersister persister, CollectionKey ce)
		{
			if (!_queryCollectionKeys.TryGetValue(persister.Role, out var querySet))
			{
				querySet = new Dictionary<CollectionKey, CollectionEntry>();
				_queryCollectionKeys.Add(persister.Role, querySet);
			}

			if (!querySet.ContainsKey(ce))
			{
				querySet.Add(ce, null);
			}
		}

		/// <summary>
		/// Links the created collection entry with the stored collection key.
		/// </summary>
		/// <param name="ce">The collection entry.</param>
		internal void LinkCollectionEntry(CollectionEntry ce)
		{
			if (!_queryCollectionKeys.TryGetValue(ce.LoadedPersister.Role, out var keys) ||
				keys.Count <= 0)
			{
				return;
			}
			var key = new CollectionKey(ce.LoadedPersister, ce.LoadedKey);
			if (keys.ContainsKey(key))
			{
				keys[key] = ce;
			}
		}

		/// <summary>
		/// Checks whether the entity key was already checked in the cache.
		/// </summary>
		/// <param name="persister">The entity persister.</param>
		/// <param name="key">The entity key.</param>
		/// <returns><see langword="true"/> whether the entity key was checked, <see langword="false"/> otherwise.</returns>
		internal bool WasEntityKeyChecked(IEntityPersister persister, EntityKey key)
		{
			return _queryCheckedEntityKeys.TryGetValue(persister.EntityName, out var checkedKeys) &&
				   checkedKeys.Contains(key);
		}

		/// <summary>
		/// Checks whether the collection entry was already checked in the cache.
		/// </summary>
		/// <param name="persister">The collection persister.</param>
		/// <param name="entry">The collection entry.</param>
		/// <returns><see langword="true"/> whether the collection entry was checked, <see langword="false"/> otherwise.</returns>
		internal bool WasCollectionEntryChecked(ICollectionPersister persister, CollectionEntry entry)
		{
			return _queryCheckedCollectionEntries.TryGetValue(persister.Role, out var checkedEntries) &&
				   checkedEntries.Contains(entry);
		}
	}
}
