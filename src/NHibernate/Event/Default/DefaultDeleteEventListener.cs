using System;
using Iesi.Collections;
using log4net;
using NHibernate.Action;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;
using Status=NHibernate.Impl.Status;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default delete event listener used by hibernate for deleting entities
	/// from the datastore in response to generated delete events. 
	/// </summary>
	[Serializable]
	public class DefaultDeleteEventListener : IDeleteEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultDeleteEventListener));

		#region IDeleteEventListener Members

		/// <summary>Handle the given delete event. </summary>
		/// <param name="event">The delete event to be handled. </param>
		public void OnDelete(DeleteEvent @event)
		{
			OnDelete(@event, new IdentitySet());
		}

		public void OnDelete(DeleteEvent @event, ISet transientEntities)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary> 
		/// Perform the entity deletion.  Well, as with most operations, does not
		/// really perform it; just schedules an action/execution with the
		/// <see cref="ActionQueue"/> for execution during flush. 
		/// </summary>
		/// <param name="session">The originating session </param>
		/// <param name="entity">The entity to delete </param>
		/// <param name="entityEntry">The entity's entry in the <see cref="ISession"/> </param>
		/// <param name="isCascadeDeleteEnabled">Is delete cascading enabled? </param>
		/// <param name="persister">The entity persister. </param>
		/// <param name="transientEntities">A cache of already deleted entities. </param>
		protected internal void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled, IEntityPersister persister, ISet transientEntities)
		{

			if (log.IsDebugEnabled)
			{
				log.Debug("deleting " + MessageHelper.InfoString(persister, entityEntry.Id, session.Factory));
			}

			IType[] propTypes = persister.PropertyTypes;
			object version = entityEntry.Version;

			object[] currentState;
			if (entityEntry.LoadedState == null)
			{
				//ie. the entity came in from update()
				currentState = persister.GetPropertyValues(entity);
			}
			else
			{
				currentState = entityEntry.LoadedState;
			}

			object[] deletedState = CreateDeletedState(persister, currentState, session);
			entityEntry.DeletedState = deletedState;

			session.Interceptor.OnDelete(entity, entityEntry.Id, deletedState, persister.PropertyNames, propTypes);

			// before any callbacks, etc, so subdeletions see that this deletion happened first
			session.SetEntryStatus(entityEntry, Status.Deleted);
			EntityKey key = new EntityKey(entityEntry.Id, persister);

			CascadeBeforeDelete(session, persister, entity, entityEntry, transientEntities);

			new ForeignKeys.Nullifier(entity, true, false, session).NullifyTransientReferences(entityEntry.DeletedState, propTypes);
			new Nullability(session).CheckNullability(entityEntry.DeletedState, persister, true);
			session.NullifiableEntityKeys.Add(key);

			// Ensures that containing deletions happen before sub-deletions
			session.ActionQueue.AddAction(new EntityDeleteAction(entityEntry.Id, deletedState, version, entity, persister, isCascadeDeleteEnabled, session));

			CascadeAfterDelete(session, persister, entity, transientEntities);

			// the entry will be removed after the flush, and will no longer
			// override the stale snapshot
			// This is now handled by removeEntity() in EntityDeleteAction
			//persistenceContext.removeDatabaseSnapshot(key);
		}

		private object[] CreateDeletedState(IEntityPersister persister, object[] currentState, IEventSource session)
		{
			IType[] propTypes = persister.PropertyTypes;
			object[] deletedState = new object[propTypes.Length];
			//		TypeFactory.deepCopy( currentState, propTypes, persister.getPropertyUpdateability(), deletedState, session );
			bool[] copyability = new bool[propTypes.Length];
			ArrayHelper.Fill(copyability, true);
			TypeFactory.DeepCopy(currentState, propTypes, copyability, deletedState);
			return deletedState;
		}

		protected internal bool InvokeDeleteLifecycle(IEventSource session, object entity, IEntityPersister persister)
		{
			if (persister.ImplementsLifecycle)
			{
				log.Debug("calling onDelete()");
				if (((ILifecycle)entity).OnDelete(session) == LifecycleVeto.Veto)
				{
					log.Debug("deletion vetoed by onDelete()");
					return true;
				}
			}
			return false;
		}

		protected internal void CascadeBeforeDelete(IEventSource session, IEntityPersister persister, object entity, EntityEntry entityEntry, ISet transientEntities)
		{
			// TODO H3.2 : CacheMode not ported
			//CacheMode cacheMode = session.CacheMode;
			//session.CacheMode = CacheMode.GET;
			session.IncrementCascadeLevel();
			try
			{
				// cascade-delete to collections BEFORE the collection owner is deleted
				Cascades.Cascade(session, persister, entity, Cascades.CascadingAction.ActionDelete,
							 CascadePoint.CascadeAfterInsertBeforeDelete, null);
			}
			finally
			{
				session.DecrementCascadeLevel();
				//session.CacheMode = cacheMode;
			}
		}

		protected internal void CascadeAfterDelete(IEventSource session, IEntityPersister persister, object entity, ISet transientEntities)
		{
			// TODO H3.2 : CacheMode not ported
			//CacheMode cacheMode = session.CacheMode;
			//session.CacheMode = CacheMode.GET;
			session.IncrementCascadeLevel();
			try
			{
				// cascade-delete to many-to-one AFTER the parent was deleted
				Cascades.Cascade(session, persister, entity, Cascades.CascadingAction.ActionDelete,
								 CascadePoint.CascadeBeforeInsertAfterDelete);
			}
			finally
			{
				session.DecrementCascadeLevel();
				//session.CacheMode = cacheMode;
			}
		}
	}
}
