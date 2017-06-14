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
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class AnyType : AbstractType, IAbstractComponentType, IAssociationType
	{

		public override Task<object> NullSafeGetAsync(DbDataReader rs, string name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			throw new NotSupportedException("object is a multicolumn type");
		}

		public override async Task<object> NullSafeGetAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (ResolveAnyAsync((string)await (metaType.NullSafeGetAsync(rs, names[0], session, owner, cancellationToken)).ConfigureAwait(false), 
				await (identifierType.NullSafeGetAsync(rs, names[1], session, owner, cancellationToken)).ConfigureAwait(false), session, cancellationToken)).ConfigureAwait(false);
		}

		public override async Task<object> HydrateAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			string entityName = (string)await (metaType.NullSafeGetAsync(rs, names[0], session, owner, cancellationToken)).ConfigureAwait(false);
			object id = await (identifierType.NullSafeGetAsync(rs, names[1], session, owner, cancellationToken)).ConfigureAwait(false);
			return new ObjectTypeCacheEntry(entityName, id);
		}

		public override Task<object> ResolveIdentifierAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				ObjectTypeCacheEntry holder = (ObjectTypeCacheEntry) value;
				return ResolveAnyAsync(holder.entityName, holder.id, session, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> SemiResolveAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			throw new NotSupportedException("any mappings may not form part of a property-ref");
		}

		public override Task<object> AssembleAsync(object cached, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				ObjectTypeCacheEntry e = cached as ObjectTypeCacheEntry;
				return (e == null) ? Task.FromResult<object>(null ): session.InternalLoadAsync(e.entityName, e.id, false, false, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> ReplaceAsync(object original, object current, ISessionImplementor session, object owner,
									   IDictionary copiedAlready, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (original == null)
				{
					return Task.FromResult<object>(null);
				}
				else
				{
					string entityName = session.BestGuessEntityName(original);
					object id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, original, session);
					return session.InternalLoadAsync(entityName, id, false, false, cancellationToken);
				}
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		private Task<object> ResolveAnyAsync(string entityName, object id, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return entityName == null || id == null ? Task.FromResult<object>(null ): session.InternalLoadAsync(entityName, id, false, false, cancellationToken);
		}

	}
}
