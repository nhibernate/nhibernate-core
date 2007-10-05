using System;
using log4net;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default replicate event listener used by Hibernate to replicate
	/// entities in response to generated replicate events. 
	/// </summary>
	[Serializable]
	public class DefaultReplicateEventListener : AbstractSaveEventListener, IReplicateEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultReplicateEventListener));

		public void OnReplicate(ReplicateEvent @event)
		{
			IEventSource source = @event.Session;
			if (source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
			{
				log.Debug("uninitialized proxy passed to replicate()");
				return;
			}

			object entity = source.PersistenceContext.UnproxyAndReassociate(@event.Entity);

			if (source.PersistenceContext.IsEntryFor(entity))
			{
				log.Debug("ignoring persistent instance passed to replicate()");
				//hum ... should we cascade anyway? throw an exception? fine like it is?
				return;
			}

			IEntityPersister persister = source.GetEntityPersister(entity);

			// get the id from the object
			/*if ( persister.isUnsaved(entity, source) ) {
			throw new TransientObjectException("transient instance passed to replicate()");
			}*/
			object id = persister.GetIdentifier(entity);
			if (id == null)
			{
				throw new TransientObjectException("instance with null id passed to replicate()");
			}

			ReplicationMode replicationMode = @event.ReplicationMode;
			object oldVersion;
			if (replicationMode == ReplicationMode.Exception)
			{
				//always do an INSERT, and let it fail by constraint violation
				oldVersion = null;
			}
			else
			{
				//what is the version on the database?
				oldVersion = persister.GetCurrentVersion(id, source);
			}

			if (oldVersion != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("found existing row for " + MessageHelper.InfoString(persister, id, source.Factory));
				}

				/// HHH-2378
				object realOldVersion = persister.IsVersioned ? oldVersion : null;

				bool canReplicate = replicationMode.ShouldOverwriteCurrentVersion(entity, realOldVersion, persister.GetVersion(entity), persister.VersionType);

				if (canReplicate)
				{
					//will result in a SQL UPDATE:
					PerformReplication(entity, id, realOldVersion, persister, replicationMode, source);
				}
				else
				{
					//else do nothing (don't even reassociate object!)
					log.Debug("no need to replicate");
				}

				//TODO: would it be better to do a refresh from db?
			}
			else
			{
				// no existing row - do an insert
				if (log.IsDebugEnabled)
				{
					log.Debug("no existing row, replicating new instance " + MessageHelper.InfoString(persister, id, source.Factory));
				}

				bool regenerate = persister.IsIdentifierAssignedByInsert; // prefer re-generation of identity!
				EntityKey key = regenerate ? null : new EntityKey(id, persister);

				PerformSaveOrReplicate(entity, key, persister, regenerate, replicationMode, source, true);
			}
		}

		private void PerformReplication(object entity, object id, object version, IEntityPersister persister, ReplicationMode replicationMode, IEventSource source)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("replicating changes to " + MessageHelper.InfoString(persister, id, source.Factory));
			}

			new OnReplicateVisitor(source, id, entity, true).Process(entity, persister);

			source.PersistenceContext.AddEntity(entity, Status.Loaded, null, new EntityKey(id, persister), version, LockMode.None, true, persister, true, false);

			CascadeAfterReplicate(entity, persister, replicationMode, source);
		}

		private void CascadeAfterReplicate(object entity, IEntityPersister persister, ReplicationMode replicationMode, IEventSource source)
		{
			source.PersistenceContext.IncrementCascadeLevel();
			try
			{
				Cascades.Cascade(source, persister, entity, Cascades.CascadingAction.ActionReplicate, CascadePoint.CascadeOnUpdate,
				                 replicationMode);
			}
			finally
			{
				source.PersistenceContext.DecrementCascadeLevel();
			}
		}

		protected internal override bool VersionIncrementDisabled
		{
			get { return true; }
		}

		protected internal override Cascades.CascadingAction CascadeAction
		{
			get { return Cascades.CascadingAction.ActionReplicate; }
		}

		protected internal override bool SubstituteValuesIfNecessary(object entity, object id, object[] values, IEntityPersister persister, ISessionImplementor source)
		{
			return false;
		}

		protected internal override bool VisitCollectionsBeforeSave(object entity, object id, object[] values, Type.IType[] types, IEventSource source)
		{
			//TODO: we use two visitors here, inefficient!
			OnReplicateVisitor visitor = new OnReplicateVisitor(source, id, entity, false);
			visitor.ProcessEntityPropertyValues(values, types);
			return base.VisitCollectionsBeforeSave(entity, id, values, types, source);
		}
	}
}
