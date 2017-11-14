﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Runtime.Serialization;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Action
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class CollectionAction : IExecutable, IComparable<CollectionAction>, IDeserializationCallback
	{

		#region IExecutable Members

		/// <summary> Called before executing any actions</summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public virtual async Task BeforeExecutionsAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// we need to obtain the lock before any actions are
			// executed, since this may be an inverse="true"
			// bidirectional association and it is one of the
			// earlier entity actions which actually updates
			// the database (this action is responsible for
			// second-level cache invalidation only)
			if (persister.HasCache)
			{
				CacheKey ck = session.GenerateCacheKey(key, persister.KeyType, persister.Role);
				softLock = await (persister.Cache.LockAsync(ck, null, cancellationToken)).ConfigureAwait(false);
			}
		}

		/// <summary>Execute this action</summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public abstract Task ExecuteAsync(CancellationToken cancellationToken);

		#endregion

		protected internal Task EvictAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				if (persister.HasCache)
				{
					CacheKey ck = session.GenerateCacheKey(key, persister.KeyType, persister.Role);
					return persister.Cache.EvictAsync(ck, cancellationToken);
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
