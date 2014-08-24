using System.Collections;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Engine
{
	public class BatchFetchQueue
	{
		private static readonly object Marker = new object();

		/// <summary>
		/// Defines a sequence of <see cref="EntityKey" /> elements that are currently
		/// eligible for batch fetching.
		/// </summary>
		/// <remarks>
		/// Even though this is a map, we only use the keys.  A map was chosen in
		/// order to utilize a <see cref="LinkedHashMap{K, V}" /> to maintain sequencing
		/// as well as uniqueness.
		/// </remarks>
		private readonly IDictionary<EntityKey, object> batchLoadableEntityKeys = new LinkedHashMap<EntityKey, object>(8);

		/// <summary>
		/// A map of <see cref="SubselectFetch">subselect-fetch descriptors</see>
		/// keyed by the <see cref="EntityKey" /> against which the descriptor is
		/// registered.
		/// </summary>
		private readonly IDictionary<EntityKey, SubselectFetch> subselectsByEntityKey = new Dictionary<EntityKey, SubselectFetch>(8);

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
				batchLoadableEntityKeys[key] = Marker;
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
				batchLoadableEntityKeys.Remove(key);
		}

		/// <summary>
		/// Get a batch of uninitialized collection keys for a given role
		/// </summary>
		/// <param name="collectionPersister">The persister for the collection role.</param>
		/// <param name="id">A key that must be included in the batch fetch</param>
		/// <param name="batchSize">the maximum number of keys to return</param>
		/// <param name="entityMode">The entity mode.</param>
		/// <returns>an array of collection keys, of length batchSize (padded with nulls)</returns>
		public object[] GetCollectionBatch(ICollectionPersister collectionPersister, object id, int batchSize)
		{
			object[] keys = new object[batchSize];
			keys[0] = id;
			int i = 1;
			int end = -1;
			bool checkForEnd = false;

			// this only works because collection entries are kept in a sequenced
			// map by persistence context (maybe we should do like entities and
			// keep a separate sequences set...)
			foreach (DictionaryEntry me in context.CollectionEntries)
			{
				CollectionEntry ce = (CollectionEntry) me.Value;
				IPersistentCollection collection = (IPersistentCollection) me.Key;
				if (!collection.WasInitialized && ce.LoadedPersister == collectionPersister)
				{
					if (checkForEnd && i == end)
					{
						return keys; //the first key found after the given key
					}

					//if ( end == -1 && count > batchSize*10 ) return keys; //try out ten batches, max

					bool isEqual = collectionPersister.KeyType.IsEqual(id, ce.LoadedKey, context.Session.EntityMode, collectionPersister.Factory);

					if (isEqual)
					{
						end = i;
						//checkForEnd = false;
					}
					else if (!IsCached(ce.LoadedKey, collectionPersister))
					{
						keys[i++] = ce.LoadedKey;
						//count++;
					}

					if (i == batchSize)
					{
						i = 1; //end of array, start filling again from start
						if (end != -1)
						{
							checkForEnd = true;
						}
					}
				}
			}
			return keys; //we ran out of keys to try
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
		public object[] GetEntityBatch(IEntityPersister persister,object id,int batchSize)
		{
			object[] ids = new object[batchSize];
			ids[0] = id; //first element of array is reserved for the actual instance we are loading!
			int i = 1;
			int end = -1;
			bool checkForEnd = false;

			foreach (EntityKey key in batchLoadableEntityKeys.Keys)
			{
				if (key.EntityName.Equals(persister.EntityName))
				{
					//TODO: this needn't exclude subclasses...
					if (checkForEnd && i == end)
					{
						//the first id found after the given id
						return ids;
					}
					if (persister.IdentifierType.IsEqual(id, key.Identifier, context.Session.EntityMode))
					{
						end = i;
					}
					else
					{
						if (!IsCached(key, persister))
						{
							ids[i++] = key.Identifier;
						}
					}
					if (i == batchSize)
					{
						i = 1; //end of array, start filling again from start
						if (end != -1)
							checkForEnd = true;
					}
				}
			}
			return ids; //we ran out of ids to try
		}

		private bool IsCached(EntityKey entityKey, IEntityPersister persister)
		{
			if (persister.HasCache)
			{
				CacheKey key = context.Session.GenerateCacheKey(entityKey.Identifier, persister.IdentifierType, entityKey.EntityName);
				return persister.Cache.Cache.Get(key) != null;
			}
			return false;
		}

		private bool IsCached(object collectionKey, ICollectionPersister persister)
		{
			if (persister.HasCache)
			{
				CacheKey cacheKey = context.Session.GenerateCacheKey(collectionKey, persister.KeyType, persister.Role);
				return persister.Cache.Cache.Get(cacheKey) != null;
			}
			return false;
		}
	}
}