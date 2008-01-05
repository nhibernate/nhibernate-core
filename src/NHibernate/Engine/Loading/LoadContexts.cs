using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.Collection;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Engine.Loading
{	
	/// <summary> 
	/// Maps <see cref="IDataReader"/> to specific contextual data
	/// related to processing that <see cref="IDataReader"/>.
	/// </summary>
	/// <remarks>
	/// Implementation note: internally an <see cref="IdentityMap"/> is used to maintain
	/// the mappings; <see cref="IdentityMap"/> was chosen because I'd rather not be
	/// dependent upon potentially bad <see cref="IDataReader"/> and <see cref="IDataReader"/>
	/// implementations.
	/// Considering the JDBC-redesign work, would further like this contextual info
	/// not mapped seperately, but available based on the result set being processed.
	/// This would also allow maintaining a single mapping as we could reliably get
	/// notification of the result-set closing...
	/// </remarks>
	public class LoadContexts
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(LoadContexts));

		[NonSerialized]
		private readonly IPersistenceContext persistenceContext;
		private IDictionary collectionLoadContexts;
		private IDictionary entityLoadContexts;

		private Dictionary<CollectionKey, LoadingCollectionEntry> xrefLoadingCollectionEntries;

		/// <summary> Creates and binds this to the given persistence context. </summary>
		/// <param name="persistenceContext">The persistence context to which this will be bound. </param>
		public LoadContexts(IPersistenceContext persistenceContext)
		{
			this.persistenceContext = persistenceContext;
		}

		/// <summary> 
		/// Retrieves the persistence context to which this is bound.
		/// </summary>
		public IPersistenceContext PersistenceContext
		{
			get { return persistenceContext; }
		}

		private ISessionImplementor Session
		{
			get { return PersistenceContext.Session; }
		}

		internal IDictionary<CollectionKey, LoadingCollectionEntry> LoadingCollectionXRefs
		{
			get { return xrefLoadingCollectionEntries; }
		}

		/// <summary> 
		/// Release internal state associated with the given result set.
		///  </summary>
		/// <param name="resultSet">The result set for which it is ok to release associated resources. </param>
		/// <remarks>
		/// This should be called when we are done with processing said result set,
		/// ideally as the result set is being closed.
		/// </remarks>
		public virtual void Cleanup(IDataReader resultSet)
		{
			if (collectionLoadContexts != null)
			{
				object tempObject;
				tempObject = collectionLoadContexts[resultSet];
				collectionLoadContexts.Remove(resultSet);
				CollectionLoadContext collectionLoadContext = (CollectionLoadContext)tempObject;
				collectionLoadContext.Cleanup();
			}
			if (entityLoadContexts != null)
			{
				object tempObject2;
				tempObject2 = entityLoadContexts[resultSet];
				entityLoadContexts.Remove(resultSet);
				EntityLoadContext entityLoadContext = (EntityLoadContext)tempObject2;
				entityLoadContext.Cleanup();
			}
		}

		/// <summary> Release internal state associated with *all* result sets. </summary>
		/// <remarks>
		/// This is intended as a "failsafe" process to make sure we get everything
		/// cleaned up and released.
		/// </remarks>
		public void Cleanup()
		{
			if (collectionLoadContexts != null)
			{
				foreach (CollectionLoadContext collectionLoadContext in collectionLoadContexts.Values)
				{
					log.Warn("fail-safe cleanup (collections) : " + collectionLoadContext);
					collectionLoadContext.Cleanup();
				}
				collectionLoadContexts.Clear();
			}
			if (entityLoadContexts != null)
			{
				foreach (EntityLoadContext entityLoadContext in entityLoadContexts.Values)
				{
					log.Warn("fail-safe cleanup (entities) : " + entityLoadContext);
					entityLoadContext.Cleanup();
				}
				entityLoadContexts.Clear();
			}
		}

		/// <summary> 
		/// Do we currently have any internal entries corresponding to loading
		/// collections?
		/// </summary>
		/// <returns> True if we currently hold state pertaining to loading collections; false otherwise. </returns>
		public bool HasLoadingCollectionEntries
		{
			get { return (xrefLoadingCollectionEntries != null && !(xrefLoadingCollectionEntries.Count == 0)); }
		}

		/// <summary> 
		/// Get the {@link CollectionLoadContext} associated with the given
		/// {@link ResultSet}, creating one if needed. 
		/// </summary>
		/// <param name="resultSet">The result set for which to retrieve the context. </param>
		/// <returns> The processing context. </returns>
		public CollectionLoadContext GetCollectionLoadContext(IDataReader resultSet)
		{
			CollectionLoadContext context = null;
			if (collectionLoadContexts == null)
			{
				collectionLoadContexts = IdentityMap.Instantiate(8);
			}
			else
			{
				context = (CollectionLoadContext)collectionLoadContexts[resultSet];
			}
			if (context == null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("constructing collection load context for result set [" + resultSet + "]");
				}
				context = new CollectionLoadContext(this, resultSet);
				collectionLoadContexts[resultSet] = context;
			}
			return context;
		}

		/// <summary> 
		/// Attempt to locate the loading collection given the owner's key.  The lookup here
		/// occurs against all result-set contexts... 
		/// </summary>
		/// <param name="persister">The collection persister </param>
		/// <param name="ownerKey">The owner key </param>
		/// <returns> The loading collection, or null if not found. </returns>
		public IPersistentCollection LocateLoadingCollection(ICollectionPersister persister, object ownerKey)
		{
			LoadingCollectionEntry lce = LocateLoadingCollectionEntry(new CollectionKey(persister, ownerKey, Session.EntityMode));
			if (lce != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("returning loading collection:" + MessageHelper.InfoString(persister, ownerKey, Session.Factory));
				}
				return lce.Collection;
			}
			else
			{
				// todo : should really move this log statement to CollectionType, where this is used from...
				if (log.IsDebugEnabled)
				{
					log.Debug("creating collection wrapper:" + MessageHelper.InfoString(persister, ownerKey, Session.Factory));
				}
				return null;
			}
		}

		/// <summary> 
		/// Register a loading collection xref. 
		/// </summary>
		/// <param name="entryKey">The xref collection key </param>
		/// <param name="entry">The corresponding loading collection entry </param>
		/// <remarks>
		/// This xref map is used because sometimes a collection is in process of
		/// being loaded from one result set, but needs to be accessed from the
		/// context of another "nested" result set processing.
		/// Implementation note: package protected, as this is meant solely for use
		/// by {@link CollectionLoadContext} to be able to locate collections
		/// being loaded by other {@link CollectionLoadContext}s/{@link ResultSet}s.
		/// </remarks>
		internal void RegisterLoadingCollectionXRef(CollectionKey entryKey, LoadingCollectionEntry entry)
		{
			if (xrefLoadingCollectionEntries == null)
				xrefLoadingCollectionEntries = new Dictionary<CollectionKey, LoadingCollectionEntry>();

			xrefLoadingCollectionEntries[entryKey] = entry;
		}

		/// <summary> 
		/// The inverse of {@link #registerLoadingCollectionXRef}.  Here, we are done
		/// processing the said collection entry, so we remove it from the
		/// load context.
		/// </summary>
		/// <param name="key">The key of the collection we are done processing. </param>
		/// <remarks>
		/// The idea here is that other loading collections can now reference said
		/// collection directly from the {@link PersistenceContext} because it
		/// has completed its load cycle.
		/// Implementation note: package protected, as this is meant solely for use
		/// by {@link CollectionLoadContext} to be able to locate collections
		/// being loaded by other {@link CollectionLoadContext}s/{@link ResultSet}s.
		/// </remarks>
		internal void UnregisterLoadingCollectionXRef(CollectionKey key)
		{
			if (!HasLoadingCollectionEntries)
			{
				return;
			}
			xrefLoadingCollectionEntries.Remove(key);
		}

		/// <summary> 
		/// Locate the LoadingCollectionEntry within *any* of the tracked
		/// <see cref="CollectionLoadContext"/>s.
		/// </summary>
		/// <param name="key">The collection key. </param>
		/// <returns> The located entry; or null. </returns>
		/// <remarks>
		/// Implementation note: package protected, as this is meant solely for use
		/// by <see cref="CollectionLoadContext"/> to be able to locate collections
		/// being loaded by other <see cref="CollectionLoadContext"/>s/ResultSets. 
		/// </remarks>
		internal LoadingCollectionEntry LocateLoadingCollectionEntry(CollectionKey key)
		{
			if (xrefLoadingCollectionEntries == null)
			{
				return null;
			}
			if (log.IsDebugEnabled)
			{
				log.Debug("attempting to locate loading collection entry [" + key + "] in any result-set context");
			}
			LoadingCollectionEntry rtn;
			xrefLoadingCollectionEntries.TryGetValue(key, out rtn);
			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("collection [{0}] {1} in load context", key, (rtn == null ? "located" : "not located")));
			}
			return rtn;
		}

		internal void CleanupCollectionXRefs(IEnumerable entryKeys)
		{
			foreach (CollectionKey entryKey in entryKeys)
				xrefLoadingCollectionEntries.Remove(entryKey);
		}

		public EntityLoadContext GetEntityLoadContext(IDataReader resultSet)
		{
			EntityLoadContext context = null;
			if (entityLoadContexts == null)
			{
				entityLoadContexts = IdentityMap.Instantiate(8);
			}
			else
			{
				context = (EntityLoadContext)entityLoadContexts[resultSet];
			}
			if (context == null)
			{
				context = new EntityLoadContext(this, resultSet);
				entityLoadContexts[resultSet] = context;
			}
			return context;
		}
	}
}
