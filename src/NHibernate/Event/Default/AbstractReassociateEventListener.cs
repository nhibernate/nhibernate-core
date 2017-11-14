using System;

using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using Status=NHibernate.Engine.Status;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// A convenience base class for listeners that respond to requests to reassociate an entity
	/// to a session ( such as through lock() or update() ).
	/// </summary>
	[Serializable]
	public partial class AbstractReassociateEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AbstractReassociateEventListener));

		/// <summary>
		/// Associates a given entity (either transient or associated with another session) to the given session.
		/// </summary>
		/// <param name="event">The event triggering the re-association </param>
		/// <param name="entity">The entity to be associated </param>
		/// <param name="id">The id of the entity. </param>
		/// <param name="persister">The entity's persister instance. </param>
		/// <returns> An EntityEntry representing the entity within this session. </returns>
		protected EntityEntry Reassociate(AbstractEvent @event, object entity, object id, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Reassociating transient instance: " + MessageHelper.InfoString(persister, id, @event.Session.Factory));
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

			new OnLockVisitor(source, id, entity).Process(entity, persister);

			persister.AfterReassociate(entity, source);

			return newEntry;
		}
	}
}
