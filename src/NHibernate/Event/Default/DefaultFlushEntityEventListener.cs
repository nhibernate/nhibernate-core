using System;

using NHibernate.Action;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// An event that occurs for each entity instance at flush time
	/// </summary>
	[Serializable]
	public class DefaultFlushEntityEventListener : IFlushEntityEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultFlushEntityEventListener));

		/// <summary>
		/// Flushes a single entity's state to the database, by scheduling an update action, if necessary
		/// </summary>
		public virtual void OnFlushEntity(FlushEntityEvent @event)
		{
			object entity = @event.Entity;
			EntityEntry entry = @event.EntityEntry;
			IEventSource session = @event.Session;
			IEntityPersister persister = entry.Persister;
			Status status = entry.Status;
			EntityMode entityMode = session.EntityMode;
			IType[] types = persister.PropertyTypes;

			bool mightBeDirty = entry.RequiresDirtyCheck(entity);

			object[] values = GetValues(entity, entry, entityMode, mightBeDirty, session);

			@event.PropertyValues = values;

			//TODO: avoid this for non-new instances where mightBeDirty==false
			bool substitute = WrapCollections(session, persister, types, values);

			if (IsUpdateNecessary(@event, mightBeDirty))
			{
				substitute = ScheduleUpdate(@event) || substitute;
			}

			if (status != Status.Deleted)
			{
				// now update the object .. has to be outside the main if block above (because of collections)
				if (substitute)
					persister.SetPropertyValues(entity, values, entityMode);

				// Search for collections by reachability, updating their role.
				// We don't want to touch collections reachable from a deleted object
				if (persister.HasCollections)
				{
					new FlushVisitor(session, entity).ProcessEntityPropertyValues(values, types);
				}
			}
		}

		private object[] GetValues(object entity, EntityEntry entry, EntityMode entityMode, bool mightBeDirty, ISessionImplementor session)
		{
			object[] loadedState = entry.LoadedState;
			Status status = entry.Status;
			IEntityPersister persister = entry.Persister;

			object[] values;
			if (status == Status.Deleted)
			{
				//grab its state saved at deletion
				values = entry.DeletedState;
			}
			else if (!mightBeDirty && loadedState != null)
			{
				values = loadedState;
			}
			else
			{
				CheckId(entity, persister, entry.Id, entityMode);

				// grab its current state
				values = persister.GetPropertyValues(entity, entityMode);

				CheckNaturalId(persister, entry, values, loadedState, entityMode, session);
			}
			return values;
		}

		/// <summary>
		/// make sure user didn't mangle the id
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="persister">The persister.</param>
		/// <param name="id">The id.</param>
		/// <param name="entityMode">The entity mode.</param>
		public virtual void CheckId(object obj, IEntityPersister persister, object id, EntityMode entityMode)
		{
			if (id != null && id is DelayedPostInsertIdentifier)
			{
				// this is a situation where the entity id is assigned by a post-insert generator
				// and was saved outside the transaction forcing it to be delayed
				return;
			}

			if (persister.CanExtractIdOutOfEntity)
			{
				if (id == null)
				{
					throw new AssertionFailure("null id in " + persister.EntityName + " entry (don't flush the Session after an exception occurs)");
				}

				object oid = persister.GetIdentifier(obj, entityMode);
				if (!persister.IdentifierType.IsEqual(id, oid, EntityMode.Poco))
				{
					throw new HibernateException("identifier of an instance of " + persister.EntityName + " was altered from " + id + " to " + oid);
				}
			}
		}

		private void CheckNaturalId(IEntityPersister persister, EntityEntry entry, object[] current, object[] loaded, EntityMode entityMode, ISessionImplementor session)
		{
			if (persister.HasNaturalIdentifier && entry.Status != Status.ReadOnly)
			{
				object[] snapshot = null;
				IType[] types = persister.PropertyTypes;
				int[] props = persister.NaturalIdentifierProperties;
				bool[] updateable = persister.PropertyUpdateability;
				for (int i = 0; i < props.Length; i++)
				{
					int prop = props[i];
					if (!updateable[prop])
					{
						object loadedVal;
						if (loaded == null)
						{
							if (snapshot == null)
							{
								snapshot = session.PersistenceContext.GetNaturalIdSnapshot(entry.Id, persister);
							}
							loadedVal = snapshot[i];
						}
						else
						{
							loadedVal = loaded[prop];
						}
						if (!types[prop].IsEqual(current[prop], loadedVal, entityMode))
						{
							throw new HibernateException("immutable natural identifier of an instance of " + persister.EntityName
							                             + " was altered");
						}
					}
				}
			}
		}

		private bool WrapCollections(IEventSource session, IEntityPersister persister, IType[] types, object[] values)
		{
			if (persister.HasCollections)
			{
				// wrap up any new collections directly referenced by the object
				// or its components

				// NOTE: we need to do the wrap here even if its not "dirty",
				// because collections need wrapping but changes to _them_
				// don't dirty the container. Also, for versioned data, we
				// need to wrap before calling searchForDirtyCollections

				WrapVisitor visitor = new WrapVisitor(session);
				// substitutes into values by side-effect
				visitor.ProcessEntityPropertyValues(values, types);
				return visitor.SubstitutionRequired;
			}
			else
			{
				return false;
			}
		}

		private bool IsUpdateNecessary(FlushEntityEvent @event, bool mightBeDirty)
		{
			Status status = @event.EntityEntry.Status;
			if (mightBeDirty || status == Status.Deleted)
			{
				// compare to cached state (ignoring collections unless versioned)
				DirtyCheck(@event);
				if (IsUpdateNecessary(@event))
				{
					return true;
				}
				else
				{
					// TODO H3.2 Different behaviour
					//FieldInterceptionHelper.clearDirty(@event.Entity);
					return false;
				}
			}
			else
			{
				return HasDirtyCollections(@event, @event.EntityEntry.Persister, status);
			}
		}

		private bool ScheduleUpdate(FlushEntityEvent @event)
		{
			EntityEntry entry = @event.EntityEntry;
			IEventSource session = @event.Session;
			EntityMode entityMode = session.EntityMode;
			object entity = @event.Entity;
			Status status = entry.Status;
			IEntityPersister persister = entry.Persister;
			object[] values = @event.PropertyValues;

			if (log.IsDebugEnabled)
			{
				if (status == Status.Deleted)
				{
					if (!persister.IsMutable)
					{
						log.Debug("Updating immutable, deleted entity: " + MessageHelper.InfoString(persister, entry.Id, session.Factory));
					}
					else if (!entry.IsModifiableEntity())
					{
						log.Debug("Updating non-modifiable, deleted entity: " + MessageHelper.InfoString(persister, entry.Id, session.Factory));
					}
					else
					{
						log.Debug("Updating deleted entity: " + MessageHelper.InfoString(persister, entry.Id, session.Factory));
					}
				}
				else
				{
					log.Debug("Updating entity: " + MessageHelper.InfoString(persister, entry.Id, session.Factory));
				}
			}

			bool intercepted;

			if (!entry.IsBeingReplicated)
			{
				// give the Interceptor a chance to process property values, if the properties
				// were modified by the Interceptor, we need to set them back to the object
				intercepted = HandleInterception(@event);
			}
			else
			{
				intercepted = false;
			}

			Validate(entity, persister, status, entityMode);

			// increment the version number (if necessary)
			object nextVersion = GetNextVersion(@event);

			// if it was dirtied by a collection only
			int[] dirtyProperties = @event.DirtyProperties;
			if (@event.DirtyCheckPossible && dirtyProperties == null)
			{
				if (!intercepted && !@event.HasDirtyCollection)
				{
					throw new AssertionFailure("dirty, but no dirty properties");
				}
				dirtyProperties = ArrayHelper.EmptyIntArray;
			}

			// check nullability but do not perform command execute
			// we'll use scheduled updates for that.
			new Nullability(session).CheckNullability(values, persister, true);

			// schedule the update
			// note that we intentionally do _not_ pass in currentPersistentState!
			session.ActionQueue.AddAction(
				new EntityUpdateAction(
					entry.Id, 
					values, 
					dirtyProperties,
					@event.HasDirtyCollection, 
					status == Status.Deleted && !entry.IsModifiableEntity() ? persister.GetPropertyValues(entity, entityMode) : entry.LoadedState,
					entry.Version,
					nextVersion, 
					entity, 
					persister, 
					session));

			return intercepted;
		}

		protected virtual void Validate(object entity, IEntityPersister persister, Status status, EntityMode entityMode)
		{
			// validate() instances of Validatable
			if (status == Status.Loaded && persister.ImplementsValidatable(entityMode))
			{
				((IValidatable)entity).Validate();
			}
		}

		protected virtual bool HandleInterception(FlushEntityEvent @event)
		{
			ISessionImplementor session = @event.Session;
			EntityEntry entry = @event.EntityEntry;
			IEntityPersister persister = entry.Persister;
			object entity = @event.Entity;

			//give the Interceptor a chance to modify property values
			object[] values = @event.PropertyValues;
			bool intercepted = InvokeInterceptor(session, entity, entry, values, persister);

			//now we might need to recalculate the dirtyProperties array
			if (intercepted && @event.DirtyCheckPossible && !@event.DirtyCheckHandledByInterceptor)
			{
				int[] dirtyProperties;
				if (@event.HasDatabaseSnapshot)
				{
					dirtyProperties = persister.FindModified(@event.DatabaseSnapshot, values, entity, session);
				}
				else
				{
					dirtyProperties = persister.FindDirty(values, entry.LoadedState, entity, session);
				}
				@event.DirtyProperties = dirtyProperties;
			}

			return intercepted;
		}

		protected virtual bool InvokeInterceptor(ISessionImplementor session, object entity, EntityEntry entry, object[] values, IEntityPersister persister)
		{
			return session.Interceptor.OnFlushDirty(entity, entry.Id, values, entry.LoadedState, persister.PropertyNames, persister.PropertyTypes);
		}

		private object GetNextVersion(FlushEntityEvent @event)
		{
			// Convience method to retrieve an entities next version value
			EntityEntry entry = @event.EntityEntry;
			IEntityPersister persister = entry.Persister;
			if (persister.IsVersioned)
			{
				object[] values = @event.PropertyValues;

				if (entry.IsBeingReplicated)
				{
					return Versioning.GetVersion(values, persister);
				}
				else
				{
					int[] dirtyProperties = @event.DirtyProperties;

					bool isVersionIncrementRequired = IsVersionIncrementRequired(@event, entry, persister, dirtyProperties);

					object nextVersion = isVersionIncrementRequired ?
						Versioning.Increment(entry.Version, persister.VersionType, @event.Session) :
						entry.Version; //use the current version

					Versioning.SetVersion(values, nextVersion, persister);

					return nextVersion;
				}
			}
			else
			{
				return null;
			}
		}

		private bool IsVersionIncrementRequired(FlushEntityEvent @event, EntityEntry entry, IEntityPersister persister, int[] dirtyProperties)
		{
			// NH different behavior: because NH-1756 when PostInsertId is used with a generated version
			// the version is read inmediately after save and does not need to be incremented.
			// BTW, in general, a generated version does not need to be incremented by NH.

			bool isVersionIncrementRequired =
				entry.Status != Status.Deleted && !persister.IsVersionPropertyGenerated &&
				(dirtyProperties == null ||
					Versioning.IsVersionIncrementRequired(dirtyProperties, @event.HasDirtyCollection, persister.PropertyVersionability));
			return isVersionIncrementRequired;
		}

		/// <summary>
		/// Performs all necessary checking to determine if an entity needs an SQL update
		/// to synchronize its state to the database. Modifies the event by side-effect!
		/// Note: this method is quite slow, avoid calling if possible!
		/// </summary>
		protected bool IsUpdateNecessary(FlushEntityEvent @event)
		{
			IEntityPersister persister = @event.EntityEntry.Persister;
			Status status = @event.EntityEntry.Status;

			if (!@event.DirtyCheckPossible)
			{
				return true;
			}
			else
			{

				int[] dirtyProperties = @event.DirtyProperties;
				if (dirtyProperties != null && dirtyProperties.Length != 0)
				{
					return true; //TODO: suck into event class
				}
				else
				{
					return HasDirtyCollections(@event, persister, status);
				}
			}
		}

		private bool HasDirtyCollections(FlushEntityEvent @event, IEntityPersister persister, Status status)
		{
			if (IsCollectionDirtyCheckNecessary(persister, status))
			{
				DirtyCollectionSearchVisitor visitor = new DirtyCollectionSearchVisitor(@event.Session, persister.PropertyVersionability);
				visitor.ProcessEntityPropertyValues(@event.PropertyValues, persister.PropertyTypes);
				bool hasDirtyCollections = visitor.WasDirtyCollectionFound;
				@event.HasDirtyCollection = hasDirtyCollections;
				return hasDirtyCollections;
			}
			else
			{
				return false;
			}
		}

		private bool IsCollectionDirtyCheckNecessary(IEntityPersister persister, Status status)
		{
			return (status == Status.Loaded || status == Status.ReadOnly) && persister.IsVersioned && persister.HasCollections;
		}

		/// <summary> Perform a dirty check, and attach the results to the event</summary>
		protected virtual void DirtyCheck(FlushEntityEvent @event)
		{
			object entity = @event.Entity;
			object[] values = @event.PropertyValues;
			ISessionImplementor session = @event.Session;
			EntityEntry entry = @event.EntityEntry;
			IEntityPersister persister = entry.Persister;
			object id = entry.Id;
			object[] loadedState = entry.LoadedState;

			int[] dirtyProperties = session.Interceptor.FindDirty(entity, id, values, loadedState, persister.PropertyNames, persister.PropertyTypes);

			@event.DatabaseSnapshot = null;

			bool interceptorHandledDirtyCheck;
			bool cannotDirtyCheck;

			if (dirtyProperties == null)
			{
				// Interceptor returned null, so do the dirtycheck ourself, if possible
				interceptorHandledDirtyCheck = false;

				cannotDirtyCheck = loadedState == null; // object loaded by update()
				if (!cannotDirtyCheck)
				{
					// dirty check against the usual snapshot of the entity
					dirtyProperties = persister.FindDirty(values, loadedState, entity, session);
				}
				else if (entry.Status == Status.Deleted && !@event.EntityEntry.IsModifiableEntity())
				{
					// A non-modifiable (e.g., read-only or immutable) entity needs to be have
					// references to transient entities set to null before being deleted. No other
					// fields should be updated.
					if (values != entry.DeletedState ) 
					{
						throw new InvalidOperationException("Entity has status Status.Deleted but values != entry.DeletedState");
					}
					// Even if loadedState == null, we can dirty-check by comparing currentState and
					// entry.getDeletedState() because the only fields to be updated are those that
					// refer to transient entities that are being set to null.
					// - currentState contains the entity's current property values.
					// - entry.getDeletedState() contains the entity's current property values with
					//   references to transient entities set to null.
					// - dirtyProperties will only contain properties that refer to transient entities
					object[] currentState = persister.GetPropertyValues(@event.Entity, @event.Session.EntityMode);
					dirtyProperties = persister.FindDirty(entry.DeletedState, currentState, entity, session);
					cannotDirtyCheck = false;
				}
				else
				{
					// dirty check against the database snapshot, if possible/necessary
					object[] databaseSnapshot = GetDatabaseSnapshot(session, persister, id);
					if (databaseSnapshot != null)
					{
						dirtyProperties = persister.FindModified(databaseSnapshot, values, entity, session);
						cannotDirtyCheck = false;
						@event.DatabaseSnapshot = databaseSnapshot;
					}
				}
			}
			else
			{
				// the Interceptor handled the dirty checking
				cannotDirtyCheck = false;
				interceptorHandledDirtyCheck = true;
			}

			@event.DirtyProperties = dirtyProperties;
			@event.DirtyCheckHandledByInterceptor = interceptorHandledDirtyCheck;
			@event.DirtyCheckPossible = !cannotDirtyCheck;
		}


		private object[] GetDatabaseSnapshot(ISessionImplementor session, IEntityPersister persister, object id)
		{
			if (persister.IsSelectBeforeUpdateRequired)
			{
				object[] snapshot = session.PersistenceContext.GetDatabaseSnapshot(id, persister);
				if (snapshot == null)
				{
					//do we even really need this? the update will fail anyway....
					if (session.Factory.Statistics.IsStatisticsEnabled)
					{
						session.Factory.StatisticsImplementor.OptimisticFailure(persister.EntityName);
					}
					throw new StaleObjectStateException(persister.EntityName, id);
				}
				else
				{
					return snapshot;
				}
			}
			else
			{
				//TODO: optimize away this lookup for entities w/o unsaved-value="undefined"
				EntityKey entityKey = session.GenerateEntityKey(id, persister);
				return session.PersistenceContext.GetCachedDatabaseSnapshot(entityKey);
			}
		}

	}
}
