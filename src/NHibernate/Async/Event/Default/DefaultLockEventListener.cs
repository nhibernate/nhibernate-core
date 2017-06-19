﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHibernate.Event.Default
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class DefaultLockEventListener : AbstractLockUpgradeEventListener, ILockEventListener
	{
		/// <summary>Handle the given lock event. </summary>
		/// <param name="event">The lock event to be handled.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public virtual Task OnLockAsync(LockEvent @event, CancellationToken cancellationToken)
		{
			if (@event.Entity == null)
			{
				throw new NullReferenceException("attempted to lock null");
			}

			if (@event.LockMode == LockMode.Write)
			{
				throw new HibernateException("Invalid lock mode for lock()");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return InternalOnLockAsync();
			async Task InternalOnLockAsync()
			{

				ISessionImplementor source = @event.Session;

				if (@event.LockMode == LockMode.None && source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
				{
					// NH-specific: shortcut for uninitialized proxies - reassociate
					// without initialization
					return;
				}

				object entity = await (source.PersistenceContext.UnproxyAndReassociateAsync(@event.Entity, cancellationToken)).ConfigureAwait(false);
				//TODO: if object was an uninitialized proxy, this is inefficient,resulting in two SQL selects

				EntityEntry entry = source.PersistenceContext.GetEntry(entity);
				if (entry == null)
				{
					IEntityPersister persister = source.GetEntityPersister(@event.EntityName, entity);
					object id = persister.GetIdentifier(entity);
					cancellationToken.ThrowIfCancellationRequested();
					if ((await (ForeignKeys.IsTransientFastAsync(@event.EntityName, entity, source)).ConfigureAwait(false)).GetValueOrDefault())
					{
						throw new TransientObjectException("cannot lock an unsaved transient instance: " + persister.EntityName);
					}

					entry = await (ReassociateAsync(@event, entity, id, persister, cancellationToken)).ConfigureAwait(false);

					await (CascadeOnLockAsync(@event, persister, entity, cancellationToken)).ConfigureAwait(false);
				}

				await (UpgradeLockAsync(entity, entry, @event.LockMode, source, cancellationToken)).ConfigureAwait(false);
			}
		}

		private async Task CascadeOnLockAsync(LockEvent @event, IEntityPersister persister, object entity, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IEventSource source = @event.Session;
			source.PersistenceContext.IncrementCascadeLevel();
			try
			{
				await (new Cascade(CascadingAction.Lock, CascadePoint.AfterLock, source).CascadeOnAsync(persister, entity, @event.LockMode, cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				source.PersistenceContext.DecrementCascadeLevel();
			}
		}
	}
}
