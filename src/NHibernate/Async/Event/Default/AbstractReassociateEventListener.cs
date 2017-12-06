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
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using Status=NHibernate.Engine.Status;

namespace NHibernate.Event.Default
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class AbstractReassociateEventListener
	{

		/// <summary>
		/// Associates a given entity (either transient or associated with another session) to the given session.
		/// </summary>
		/// <param name="event">The event triggering the re-association </param>
		/// <param name="entity">The entity to be associated </param>
		/// <param name="id">The id of the entity. </param>
		/// <param name="persister">The entity's persister instance. </param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <returns> An EntityEntry representing the entity within this session. </returns>
		protected async Task<EntityEntry> ReassociateAsync(AbstractEvent @event, object entity, object id, IEntityPersister persister, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (log.IsDebugEnabled())
			{
				log.Debug("Reassociating transient instance: {0}", MessageHelper.InfoString(persister, id, @event.Session.Factory));
			}

			IEventSource source = @event.Session;
			EntityKey key = source.GenerateEntityKey(id, persister);

			source.PersistenceContext.CheckUniqueness(key, entity);

			//get a snapshot
			object[] values = persister.GetPropertyValues(entity);
			TypeHelper.DeepCopy(values, persister.PropertyTypes, persister.PropertyUpdateability, values, source);
			object version = Versioning.GetVersion(values, persister);

			EntityEntry newEntry = source.PersistenceContext.AddEntity(
				entity,
				persister.IsMutable ? Status.Loaded : Status.ReadOnly,
				values,
				key,
				version,
				LockMode.None,
				true,
				persister,
				false,
				true);

			await (new OnLockVisitor(source, id, entity).ProcessAsync(entity, persister, cancellationToken)).ConfigureAwait(false);

			persister.AfterReassociate(entity, source);

			return newEntry;
		}
	}
}
