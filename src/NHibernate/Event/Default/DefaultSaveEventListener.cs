using System;
using NHibernate.Engine;

namespace NHibernate.Event.Default
{
	/// <summary> An event handler for save() events</summary>
	[Serializable]
	public class DefaultSaveEventListener : DefaultSaveOrUpdateEventListener
	{
		protected internal override object PerformSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			// this implementation is supposed to tolerate incorrect unsaved-value
			// mappings, for the purpose of backward-compatibility
			EntityEntry entry = @event.Session.PersistenceContext.GetEntry(@event.Entity);
			if (entry != null && entry.Status != Status.Deleted)
			{
				return EntityIsPersistent(@event);
			}
			else
			{
				return EntityIsTransient(@event);
			}
		}

		protected internal override object SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent @event)
		{
			if (@event.RequestedId == null)
			{
				return base.SaveWithGeneratedOrRequestedId(@event);
			}
			else
			{
				return SaveWithRequestedId(@event.Entity, @event.RequestedId, @event.EntityName, null, @event.Session);
			}
		}

		protected internal override bool ReassociateIfUninitializedProxy(object obj, ISessionImplementor source)
		{
			if (!NHibernateUtil.IsInitialized(obj))
			{
				throw new PersistentObjectException("uninitialized proxy passed to save()");
			}
			else
			{
				return false;
			}
		}
	}
}
