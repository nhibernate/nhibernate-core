﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class DefaultRefreshEventListener : IRefreshEventListener
	{

		public virtual Task OnRefreshAsync(RefreshEvent @event, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return OnRefreshAsync(@event, IdentityMap.Instantiate(10), cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public virtual async Task OnRefreshAsync(RefreshEvent @event, IDictionary refreshedAlready, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IEventSource source = @event.Session;

			bool isTransient = !source.Contains(@event.Entity);
			if (source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
			{
				if (isTransient)
					source.SetReadOnly(@event.Entity, source.DefaultReadOnly);
				return;
			}

			object obj = await (source.PersistenceContext.UnproxyAndReassociateAsync(@event.Entity, cancellationToken)).ConfigureAwait(false);

			if (refreshedAlready.Contains(obj))
			{
				log.Debug("already refreshed");
				return;
			}

			EntityEntry e = source.PersistenceContext.GetEntry(obj);
			IEntityPersister persister;
			object id;

			if (e == null)
			{
				persister = source.GetEntityPersister(null, obj); //refresh() does not pass an entityName
				id = persister.GetIdentifier(obj);
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing transient " + MessageHelper.InfoString(persister, id, source.Factory));
				}
				EntityKey key = source.GenerateEntityKey(id, persister);
				if (source.PersistenceContext.GetEntry(key) != null)
				{
					throw new PersistentObjectException("attempted to refresh transient instance when persistent instance was already associated with the Session: " + 
						MessageHelper.InfoString(persister, id, source.Factory));
				}
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing " + MessageHelper.InfoString(e.Persister, e.Id, source.Factory));
				}
				if (!e.ExistsInDatabase)
				{
					throw new HibernateException("this instance does not yet exist as a row in the database");
				}

				persister = e.Persister;
				id = e.Id;
			}

			// cascade the refresh prior to refreshing this entity
			refreshedAlready[obj] = obj;
			await (new Cascade(CascadingAction.Refresh, CascadePoint.BeforeRefresh, source).CascadeOnAsync(persister, obj, refreshedAlready, cancellationToken)).ConfigureAwait(false);

			if (e != null)
			{
				EntityKey key = source.GenerateEntityKey(id, persister);
				source.PersistenceContext.RemoveEntity(key);
				if (persister.HasCollections)
					await (new EvictVisitor(source).ProcessAsync(obj, persister, cancellationToken)).ConfigureAwait(false);
			}

			if (persister.HasCache)
			{
				CacheKey ck = source.GenerateCacheKey(id, persister.IdentifierType, persister.RootEntityName);
				await (persister.Cache.RemoveAsync(ck, cancellationToken)).ConfigureAwait(false);
			}

			await (EvictCachedCollectionsAsync(persister, id, source.Factory, cancellationToken)).ConfigureAwait(false);

			// NH Different behavior : NH-1601
			// At this point the entity need the real refresh, all elementes of collections are Refreshed,
			// the collection state was evicted, but the PersistentCollection (in the entity state)
			// is associated with a possible previous session.
			await (new WrapVisitor(source).ProcessAsync(obj, persister, cancellationToken)).ConfigureAwait(false);

			string previousFetchProfile = source.FetchProfile;
			source.FetchProfile = "refresh";
			object result = await (persister.LoadAsync(id, obj, @event.LockMode, source, cancellationToken)).ConfigureAwait(false);
			
			if (result != null)
				if (!persister.IsMutable)
					source.SetReadOnly(result, true);
				else
					source.SetReadOnly(result, (e == null ? source.DefaultReadOnly : e.IsReadOnly));
			
			source.FetchProfile = previousFetchProfile;

			// NH Different behavior : we are ignoring transient entities without throw any kind of exception
			// because a transient entity is "self refreshed"
			if (!(await (ForeignKeys.IsTransientFastAsync(persister.EntityName, obj, @event.Session, cancellationToken)).ConfigureAwait(false)).GetValueOrDefault(result == null))
				UnresolvableObjectException.ThrowIfNull(result, id, persister.EntityName);
		}

		// Evict collections from the factory-level cache
		private Task EvictCachedCollectionsAsync(IEntityPersister persister, object id, ISessionFactoryImplementor factory, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return EvictCachedCollectionsAsync(persister.PropertyTypes, id, factory, cancellationToken);
		}

		private async Task EvictCachedCollectionsAsync(IType[] types, object id, ISessionFactoryImplementor factory, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsCollectionType)
				{
					await (factory.EvictCollectionAsync(((CollectionType)types[i]).Role, id, cancellationToken)).ConfigureAwait(false);
				}
				else if (types[i].IsComponentType)
				{
					IAbstractComponentType actype = (IAbstractComponentType)types[i];
					await (EvictCachedCollectionsAsync(actype.Subtypes, id, factory, cancellationToken)).ConfigureAwait(false);
				}
			}
		}
	}
}
