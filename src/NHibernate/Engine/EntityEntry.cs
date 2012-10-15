using System;
using NHibernate.Impl;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;

namespace NHibernate.Engine
{
	/// <summary>
	/// We need an entry to tell us all about the current state
	/// of an object with respect to its persistent state
	/// </summary>
	[Serializable]
	public sealed class EntityEntry
	{
		private LockMode lockMode;
		private Status status;
		private Status? previousStatus;
		private readonly object id;
		private object[] loadedState;
		private object[] deletedState;
		private bool existsInDatabase;
		private object version;
		
		[NonSerialized]
		private IEntityPersister persister; // for convenience to save some lookups

		private readonly EntityMode entityMode;
		private readonly string entityName;
		private EntityKey cachedEntityKey;
		private readonly bool isBeingReplicated;
		private readonly bool loadedWithLazyPropertiesUnfetched;

		[NonSerialized]
		private readonly object rowId;

		/// <summary>
		/// Initializes a new instance of EntityEntry.
		/// </summary>
		/// <param name="status">The current <see cref="Status"/> of the Entity.</param>
		/// <param name="loadedState">The snapshot of the Entity's state when it was loaded.</param>
		/// <param name="rowId"></param>
		/// <param name="id">The identifier of the Entity in the database.</param>
		/// <param name="version">The version of the Entity.</param>
		/// <param name="lockMode">The <see cref="LockMode"/> for the Entity.</param>
		/// <param name="existsInDatabase">A boolean indicating if the Entity exists in the database.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for this Entity.</param>
		/// <param name="entityMode"></param>
		/// <param name="disableVersionIncrement"></param>
		/// <param name="lazyPropertiesAreUnfetched"></param>
		internal EntityEntry(Status status, object[] loadedState, object rowId, object id, object version, LockMode lockMode,
			bool existsInDatabase, IEntityPersister persister, EntityMode entityMode,
			bool disableVersionIncrement, bool lazyPropertiesAreUnfetched)
		{
			this.status = status;
			this.previousStatus = null;
			// only retain loaded state if the status is not Status.ReadOnly
			if (status != Status.ReadOnly) { this.loadedState = loadedState; }
			this.id = id;
			this.rowId = rowId;
			this.existsInDatabase = existsInDatabase;
			this.version = version;
			this.lockMode = lockMode;
			isBeingReplicated = disableVersionIncrement;
			loadedWithLazyPropertiesUnfetched = lazyPropertiesAreUnfetched;
			this.persister = persister;
			this.entityMode = entityMode;
			entityName = persister == null ? null : persister.EntityName;
		}

		/// <summary>
		/// Gets or sets the current <see cref="LockMode"/> of the Entity.
		/// </summary>
		/// <value>The <see cref="LockMode"/> of the Entity.</value>
		public LockMode LockMode
		{
			get { return lockMode; }
			set { lockMode = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="Status"/> of this Entity with respect to its
		/// persistence in the database.
		/// </summary>
		/// <value>The <see cref="Status"/> of this Entity.</value>
		public Status Status
		{
			get { return status; }
			set
			{
				if (value == Status.ReadOnly)
					loadedState = null; //memory optimization

				if (this.status != value)
				{
					previousStatus = this.status;
					this.status = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the identifier of the Entity in the database.
		/// </summary>
		/// <value>The identifier of the Entity in the database if one has been assigned.</value>
		/// <remarks>This might be <see langword="null" /> when the <see cref="EntityEntry.Status"/> is
		/// <see cref="Engine.Status.Saving"/> and the database generates the id.</remarks>
		public object Id
		{
			get { return id; }
		}

		/// <summary>
		/// Gets or sets the snapshot of the Entity when it was loaded from the database.
		/// </summary>
		/// <value>The snapshot of the Entity.</value>
		/// <remarks>
		/// There will only be a value when the Entity was loaded in the current Session.
		/// </remarks>
		public object[] LoadedState
		{
			get { return loadedState; }
		}

		/// <summary>
		/// Gets or sets the snapshot of the Entity when it was marked as being ready for deletion.
		/// </summary>
		/// <value>The snapshot of the Entity.</value>
		/// <remarks>This will be <see langword="null" /> if the Entity is not being deleted.</remarks>
		public object[] DeletedState
		{
			get { return deletedState; }
			set { deletedState = value; }
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> indicating if this Entity exists in the database.
		/// </summary>
		/// <value><see langword="true" /> if it is already in the database.</value>
		/// <remarks>
		/// It can also be <see langword="true" /> if it does not exists in the database yet and the
		/// <see cref="IEntityPersister.IsIdentifierAssignedByInsert"/> is <see langword="true" />.
		/// </remarks>
		public bool ExistsInDatabase
		{
			get { return existsInDatabase; }
		}

		/// <summary>
		/// Gets or sets the version of the Entity.
		/// </summary>
		/// <value>The version of the Entity.</value>
		public object Version
		{
			get { return version; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IEntityPersister"/> that is responsible for this Entity.
		/// </summary>
		/// <value>The <see cref="IEntityPersister"/> that is responsible for this Entity.</value>
		public IEntityPersister Persister
		{
			get { return persister; }
			internal set { persister = value; } // For deserialization callback
		}

		/// <summary>
		/// Gets the Fully Qualified Name of the class this Entity is an instance of.
		/// </summary>
		/// <value>The Fully Qualified Name of the class this Entity is an instance of.</value>
		public string EntityName
		{
			get { return entityName; }
		}

		public bool IsBeingReplicated
		{
			get { return isBeingReplicated; }
		}

		public object RowId
		{
			get { return rowId; }
		}

		public bool LoadedWithLazyPropertiesUnfetched
		{
			get { return loadedWithLazyPropertiesUnfetched; }
		}
		
		/// <summary>
		/// Get the EntityKey based on this EntityEntry.
		/// </summary>
		public EntityKey EntityKey
		{
			get
			{
				if (cachedEntityKey == null)
				{
					if (id == null)
						throw new InvalidOperationException("cannot generate an EntityKey when id is null.");

					cachedEntityKey = new EntityKey(id, persister, entityMode);
				}
				return cachedEntityKey;
			}
		}

		public object GetLoadedValue(string propertyName)
		{
			int propertyIndex = ((IUniqueKeyLoadable) persister).GetPropertyIndex(propertyName);
			return loadedState[propertyIndex];
		}

		/// <summary>
		/// After actually inserting a row, record the fact that the instance exists on the
		/// database (needed for identity-column key generation)
		/// </summary>
		public void PostInsert()
		{
			existsInDatabase = true;
		}

		/// <summary>
		/// After actually updating the database, update the snapshot information,
		/// and escalate the lock mode.
		/// </summary>
		public void PostUpdate(object entity, object[] updatedState, object nextVersion)
		{
			loadedState = updatedState;

			LockMode = LockMode.Write;

			if (Persister.IsVersioned)
			{
				version = nextVersion;
				Persister.SetPropertyValue(entity, Persister.VersionProperty, nextVersion, entityMode);
			}
			FieldInterceptionHelper.ClearDirty(entity);
		}

		/// <summary>
		/// After actually deleting a row, record the fact that the instance no longer
		/// exists in the database
		/// </summary>
		public void PostDelete()
		{
			previousStatus = status;
			status = Status.Gone;
			existsInDatabase = false;
		}

		public void ForceLocked(object entity, object nextVersion)
		{
			version = nextVersion;
			loadedState[persister.VersionProperty] = version;
			LockMode = LockMode.Force;
			persister.SetPropertyValue(entity, Persister.VersionProperty, nextVersion, entityMode);
		}
		
		public bool IsNullifiable(bool earlyInsert, ISessionImplementor session)
		{
			return Status == Status.Saving || (earlyInsert ? !ExistsInDatabase : session.PersistenceContext.NullifiableEntityKeys.Contains(session.GenerateEntityKey(Id, Persister)));
		}

		public bool RequiresDirtyCheck(object entity)
		{
			return
				IsModifiableEntity()
				&& (Persister.HasMutableProperties || !FieldInterceptionHelper.IsInstrumented(entity)
				|| FieldInterceptionHelper.ExtractFieldInterceptor(entity).IsDirty);
		}
		
		/// <summary>
		/// Can the entity be modified?
		/// The entity is modifiable if all of the following are true:
		/// - the entity class is mutable
		/// - the entity is not read-only
		/// - if the current status is Status.Deleted, then the entity was not read-only when it was deleted
		/// </summary>
		/// <returns>true, if the entity is modifiable; false, otherwise</returns>
		public bool IsModifiableEntity()
		{
			return (status != Status.ReadOnly) && !(status == Status.Deleted && previousStatus == Status.ReadOnly) && Persister.IsMutable;
		}
		
		public bool IsReadOnly
		{
			get
			{
				if (status != Status.Loaded && status != Status.ReadOnly)
				{
					throw new HibernateException("instance was not in a valid state");
				}
				return status == Status.ReadOnly;
			}
		}

		public void SetReadOnly(bool readOnly, object entity)
		{
			if (readOnly == IsReadOnly)
				return; // simply return since the status is not being changed
			
			if (readOnly)
			{
				Status = Status.ReadOnly;
				loadedState = null;
			}
			else
			{
				if (!persister.IsMutable)
					throw new InvalidOperationException("Cannot make an immutable entity modifiable.");

				Status = Status.Loaded;
				loadedState = Persister.GetPropertyValues(entity, entityMode);
			}
		}

		public override string ToString()
		{
			return string.Format("EntityEntry{0}({1})", MessageHelper.InfoString(entityName, id), status);
		}
	}
}
