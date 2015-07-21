using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default lock event listeners used by hibernate to lock entities
	/// in response to generated lock events. 
	/// </summary>
	[Serializable]
	public class DefaultLockEventListener : AbstractLockUpgradeEventListener, ILockEventListener
	{
		/// <summary>Handle the given lock event. </summary>
		/// <param name="event">The lock event to be handled.</param>
		public virtual void OnLock(LockEvent @event)
		{
			if (@event.Entity == null)
			{
				throw new NullReferenceException("attempted to lock null");
			}

			if (@event.LockMode == LockMode.Write)
			{
				throw new HibernateException("Invalid lock mode for lock()");
			}

			ISessionImplementor source = @event.Session;

			if (@event.LockMode == LockMode.None && source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
			{
				// NH-specific: shortcut for uninitialized proxies - reassociate
				// without initialization
				return;
			}

			object entity = source.PersistenceContext.UnproxyAndReassociate(@event.Entity);
			//TODO: if object was an uninitialized proxy, this is inefficient,resulting in two SQL selects

			EntityEntry entry = source.PersistenceContext.GetEntry(entity);
			if (entry == null)
			{
				IEntityPersister persister = source.GetEntityPersister(@event.EntityName, entity);
				object id = persister.GetIdentifier(entity, source.EntityMode);
				if (!ForeignKeys.IsNotTransient(@event.EntityName, entity, false, source))
				{
					throw new TransientObjectException("cannot lock an unsaved transient instance: " + persister.EntityName);
				}

				entry = Reassociate(@event, entity, id, persister);

				CascadeOnLock(@event, persister, entity);
			}

			UpgradeLock(entity, entry, @event.LockMode, source);
		}

		private void CascadeOnLock(LockEvent @event, IEntityPersister persister, object entity)
		{
			IEventSource source = @event.Session;
			source.PersistenceContext.IncrementCascadeLevel();
			try
			{
				new Cascade(CascadingAction.Lock, CascadePoint.AfterLock, source).CascadeOn(persister, entity, @event.LockMode);
			}
			finally
			{
				source.PersistenceContext.DecrementCascadeLevel();
			}
		}
	}
}
