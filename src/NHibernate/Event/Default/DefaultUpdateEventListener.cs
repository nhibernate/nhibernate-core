using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Event.Default
{
	/// <summary> An event handler for update() events</summary>
	[Serializable]
	public partial class DefaultUpdateEventListener : DefaultSaveOrUpdateEventListener
	{
		protected override object PerformSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			// this implementation is supposed to tolerate incorrect unsaved-value
			// mappings, for the purpose of backward-compatibility
			EntityEntry entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
			if (entry != null)
			{
				if (entry.Status == Status.Deleted)
				{
					throw new ObjectDeletedException("deleted instance passed to update()", null, @event.EntityName);
				}
				else
				{
					return EntityIsPersistent(@event);
				}
			}
			else
			{
				EntityIsDetached(@event);
				return null;
			}
		}

		protected override object SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent @event)
		{
			return SaveWithGeneratedId(@event.Entity, @event.EntityName, null, @event.Session, true);
		}

		/// <summary> 
		/// If the user specified an id, assign it to the instance and use that, 
		/// otherwise use the id already assigned to the instance
		/// </summary>
		protected override object GetUpdateId(object entity, IEntityPersister persister, object requestedId)
		{
			if (requestedId == null)
			{
				return base.GetUpdateId(entity, persister, requestedId);
			}
			else
			{
				persister.SetIdentifier(entity, requestedId);
				return requestedId;
			}
		}
	}
}
