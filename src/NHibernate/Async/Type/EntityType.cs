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
using System.Text;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;
using System.Collections.Generic;
using NHibernate.Action;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class EntityType : AbstractType, IAssociationType
	{

		public override Task<object> NullSafeGetAsync(DbDataReader rs, string name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return NullSafeGetAsync(rs, new string[] {name}, session, owner, cancellationToken);
		}

		protected internal Task<object> GetIdentifierAsync(object value, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return ForeignKeys.GetEntityIdentifierIfNotUnsavedAsync(GetAssociatedEntityName(), value, session, cancellationToken); //tolerates nulls
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		protected internal async Task<object> GetReferenceValueAsync(object value, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (value == null)
			{
				return null;
			}
			else if (IsReferenceToPrimaryKey)
			{
				return await (ForeignKeys.GetEntityIdentifierIfNotUnsavedAsync(GetAssociatedEntityName(), value, session, cancellationToken)).ConfigureAwait(false); //tolerates nulls
			}
			else
			{
				IEntityPersister entityPersister = session.Factory.GetEntityPersister(GetAssociatedEntityName());
				object propertyValue = entityPersister.GetPropertyValue(value, uniqueKeyPropertyName);

				// We now have the value of the property-ref we reference.  However,
				// we need to dig a little deeper, as that property might also be
				// an entity type, in which case we need to resolve its identitifier
				IType type = entityPersister.GetPropertyType(uniqueKeyPropertyName);
				if (type.IsEntityType)
				{
					propertyValue = await (((EntityType) type).GetReferenceValueAsync(propertyValue, session, cancellationToken)).ConfigureAwait(false);
				}

				return propertyValue;
			}
		}

		protected internal async Task<object> GetReferenceValueAsync(object value, ISessionImplementor session, bool forbidDelayed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var referenceValue = await (GetReferenceValueAsync(value, session, cancellationToken)).ConfigureAwait(false);
			if (forbidDelayed && referenceValue is DelayedPostInsertIdentifier)
			{
				throw new UnresolvableObjectException(
					$"Cannot resolve the identifier from a {nameof(DelayedPostInsertIdentifier)}. Consider flushing the session first.",
					referenceValue, returnedClass);
			}

			return referenceValue;
		}

		public override async Task<object> ReplaceAsync(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (original == null)
			{
				return null;
			}
			object cached = copyCache[original];
			if (cached != null)
			{
				return cached;
			}
			else
			{
				if (original == target)
				{
					return target;
				}
				if (session.GetContextEntityIdentifier(original) == null && (await (ForeignKeys.IsTransientFastAsync(associatedEntityName, original, session, cancellationToken)).ConfigureAwait(false)).GetValueOrDefault())
				{
					object copy = session.Factory.GetEntityPersister(associatedEntityName).Instantiate(null);
					//TODO: should this be Session.instantiate(Persister, ...)?
					copyCache.Add(original, copy);
					return copy;
				}
				else
				{
					object id = await (GetReferenceValueAsync(original, session, cancellationToken)).ConfigureAwait(false);
					if (id == null)
					{
						throw new AssertionFailure("non-transient entity has a null id");
					}
					id = await (GetIdentifierOrUniqueKeyType(session.Factory).ReplaceAsync(id, null, session, owner, copyCache, cancellationToken)).ConfigureAwait(false);
					return await (ResolveIdentifierAsync(id, session, owner, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Converts the id contained in the <see cref="DbDataReader"/> to an object.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names that contain the id.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns>
		/// An instance of the object or <see langword="null" /> if the identifer was null.
		/// </returns>
		public override sealed async Task<object> NullSafeGetAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (ResolveIdentifierAsync(await (HydrateAsync(rs, names, session, owner, cancellationToken)).ConfigureAwait(false), session, owner, cancellationToken)).ConfigureAwait(false);
		}

		public abstract override Task<object> HydrateAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken);

		/// <summary>
		/// Resolves the identifier to the actual object.
		/// </summary>
		protected async Task<object> ResolveIdentifierAsync(object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			string entityName = GetAssociatedEntityName();
			bool isProxyUnwrapEnabled = unwrapProxy && session.Factory
			                                                  .GetEntityPersister(entityName).IsInstrumented;

			object proxyOrEntity = await (session.InternalLoadAsync(entityName, id, eager, IsNullable && !isProxyUnwrapEnabled, cancellationToken)).ConfigureAwait(false);

			if (proxyOrEntity.IsProxy())
			{
				INHibernateProxy proxy = (INHibernateProxy) proxyOrEntity;
				proxy.HibernateLazyInitializer.Unwrap = isProxyUnwrapEnabled;
			}

			return proxyOrEntity;
		}

		/// <summary>
		/// Resolve an identifier or unique key value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns></returns>
		public override Task<object> ResolveIdentifierAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (value == null)
				{
					return Task.FromResult<object>(null);
				}
				else
				{
					if (IsNull(owner, session))
					{
						return Task.FromResult<object>(null); //EARLY EXIT!
					}

					if (IsReferenceToPrimaryKey)
					{
						return ResolveIdentifierAsync(value, session, cancellationToken);
					}
					else
					{
						return LoadByUniqueKeyAsync(GetAssociatedEntityName(), uniqueKeyPropertyName, value, session, cancellationToken);
					}
				}
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <summary> 
		/// Load an instance by a unique key that is not the primary key. 
		/// </summary>
		/// <param name="entityName">The name of the entity to load </param>
		/// <param name="uniqueKeyPropertyName">The name of the property defining the unique key. </param>
		/// <param name="key">The unique key property value. </param>
		/// <param name="session">The originating session. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns> The loaded entity </returns>
		public async Task<object> LoadByUniqueKeyAsync(string entityName, string uniqueKeyPropertyName, object key, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ISessionFactoryImplementor factory = session.Factory;
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable) factory.GetEntityPersister(entityName);

			//TODO: implement caching?! proxies?!

			var keyType = GetIdentifierOrUniqueKeyType(factory)
				.GetSemiResolvedType(factory);
			EntityUniqueKey euk =
				new EntityUniqueKey(
					entityName,
					uniqueKeyPropertyName,
					key,
					keyType,
					session.Factory);

			IPersistenceContext persistenceContext = session.PersistenceContext;
			try
			{
				object result = persistenceContext.GetEntity(euk);
				if (result == null)
				{
					result = await (persister.LoadByUniqueKeyAsync(uniqueKeyPropertyName, key, session, cancellationToken)).ConfigureAwait(false);
					if (result == null && !IsNullable)
					{
						factory.EntityNotFoundDelegate.HandleEntityNotFound(entityName, uniqueKeyPropertyName, key);
					}
				}
				return result == null ? null : persistenceContext.ProxyFor(result);
			}
			catch (OperationCanceledException) { throw; }
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(factory.SQLExceptionConverter, sqle, "Error performing LoadByUniqueKey");
			}
		}
	}
}
