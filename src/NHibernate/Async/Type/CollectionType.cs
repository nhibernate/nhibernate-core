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
using System.Data.Common;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
using NHibernate.Util;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Impl;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class CollectionType : AbstractType, IAssociationType
	{

		public override Task<object> NullSafeGetAsync(DbDataReader rs, string name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return NullSafeGetAsync(rs, new string[] { name }, session, owner, cancellationToken);
		}

		public override Task<object> NullSafeGetAsync(DbDataReader rs, string[] name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return ResolveIdentifierAsync(null, session, owner, cancellationToken);
		}

		public override Task NullSafeSetAsync(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				NullSafeSet(st, value, index, settable, session);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task NullSafeSetAsync(DbCommand cmd, object value, int index, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				NullSafeSet(cmd, value, index, session);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> DisassembleAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				//remember the uk value

				//This solution would allow us to eliminate the owner arg to disassemble(), but
				//what if the collection was null, and then later had elements added? seems unsafe
				//session.getPersistenceContext().getCollectionEntry( (PersistentCollection) value ).getKey();

				object key = GetKeyOfOwner(owner, session);
				if (key == null)
				{
					return Task.FromResult<object>(null);
				}
				else
				{
					return GetPersister(session).KeyType.DisassembleAsync(key, session, owner, cancellationToken);
				}
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override async Task<object> AssembleAsync(object cached, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			//we must use the "remembered" uk value, since it is 
			//not available from the EntityEntry during assembly
			if (cached == null)
			{
				return null;
			}
			else
			{
				object key = await (GetPersister(session).KeyType.AssembleAsync(cached, session, owner, cancellationToken)).ConfigureAwait(false);
				return await (ResolveKeyAsync(key, session, owner, cancellationToken)).ConfigureAwait(false);
			}
		}

		public override async Task<bool> IsDirtyAsync(object old, object current, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// collections don't dirty an unversioned parent entity

			// TODO: I don't like this implementation; it would be better if this was handled by SearchForDirtyCollections();
			return IsOwnerVersioned(session) && await (base.IsDirtyAsync(old, current, session, cancellationToken)).ConfigureAwait(false);
		}

		public override Task<object> HydrateAsync(DbDataReader rs, string[] name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Hydrate(rs, name, session, owner));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> ResolveIdentifierAsync(object key, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return ResolveKeyAsync(GetKeyOfOwner(owner, session), session, owner, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		private Task<object> ResolveKeyAsync(object key, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return key == null ? Task.FromResult<object>(null ): GetCollectionAsync(key, session, owner, cancellationToken);
		}

		public async Task<object> GetCollectionAsync(object key, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ICollectionPersister persister = GetPersister(session);
			IPersistenceContext persistenceContext = session.PersistenceContext;

			// check if collection is currently being loaded
			IPersistentCollection collection = persistenceContext.LoadContexts.LocateLoadingCollection(persister, key);
			if (collection == null)
			{
				// check if it is already completely loaded, but unowned
				collection = persistenceContext.UseUnownedCollection(new CollectionKey(persister, key));
				if (collection == null)
				{
					// create a new collection wrapper, to be initialized later
					collection = Instantiate(session, persister, key);
					collection.Owner = owner;

					persistenceContext.AddUninitializedCollection(persister, collection, key);

					// some collections are not lazy:
					if (InitializeImmediately())
					{
						await (session.InitializeCollectionAsync(collection, false, cancellationToken)).ConfigureAwait(false);
					}
					else if (!persister.IsLazy)
					{
						persistenceContext.AddNonLazyCollection(collection);
					}

					if (HasHolder())
					{
						session.PersistenceContext.AddCollectionHolder(collection);
					}
				}

				if (log.IsDebugEnabled())
				{
					log.Debug("Created collection wrapper: {0}", MessageHelper.CollectionInfoString(persister, collection, key, session));
				}
			}
			collection.Owner = owner;
			return collection.GetValue();
		}

		public override Task<object> SemiResolveAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			throw new NotSupportedException("collection mappings may not form part of a property-ref");
		}

		public override async Task<object> ReplaceAsync(object original, object target, ISessionImplementor session, object owner,
									   IDictionary copyCache, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (original == null)
			{
				return null;
			}

			if (!NHibernateUtil.IsInitialized(original))
			{
				return target;
			}

			object result = target == null || target == original
								? InstantiateResult(original)
								: target;

			//for arrays, replaceElements() may return a different reference, since
			//the array length might not match
			result = await (ReplaceElementsAsync(original, result, owner, copyCache, session, cancellationToken)).ConfigureAwait(false);

			if (original == target)
			{
				//get the elements back into the target
				//TODO: this is a little inefficient, don't need to do a whole
				//	  deep replaceElements() call
				await (ReplaceElementsAsync(result, target, owner, copyCache, session, cancellationToken)).ConfigureAwait(false);
				result = target;
			}

			return result;
		}

		public virtual async Task<object> ReplaceElementsAsync(object original, object target, object owner, IDictionary copyCache, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var elemType = GetElementType(session.Factory);
			var targetPc = target as IPersistentCollection;
			var originalPc = original as IPersistentCollection;
			var iterOriginal = (IEnumerable)original;
			var clearTargetsDirtyFlag = ShouldTargetsDirtyFlagBeCleared(targetPc, originalPc, iterOriginal);

			// copy elements into newly empty target collection
			Clear(target);
			foreach (var obj in iterOriginal)
			{
				Add(target, await (elemType.ReplaceAsync(obj, null, session, owner, copyCache, cancellationToken)).ConfigureAwait(false));
			}

			if(clearTargetsDirtyFlag)
			{
				targetPc.ClearDirty();
			}

			return target;
		}

		public override Task<bool> IsDirtyAsync(object old, object current, bool[] checkable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			return IsDirtyAsync(old, current, session, cancellationToken);
		}

		public override Task<bool> IsModifiedAsync(object oldHydratedState, object currentState, bool[] checkable,
										ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(IsModified(oldHydratedState, currentState, checkable, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}
	}
}
