using System;
using NHibernate.Persister.Entity;

namespace NHibernate.Impl
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
		private object id;
		private object[] loadedState;
		private object[] deletedState;
		private bool existsInDatabase;
		private object version;
		// for convenience to save some lookups
		[NonSerialized]
		private IEntityPersister persister;

		private string className;
		private bool isBeingReplicated;

		/// <summary>
		/// Initializes a new instance of EntityEntry.
		/// </summary>
		/// <param name="status">The current <see cref="Status"/> of the Entity.</param>
		/// <param name="loadedState">The snapshot of the Entity's state when it was loaded.</param>
		/// <param name="id">The identifier of the Entity in the database.</param>
		/// <param name="version">The version of the Entity.</param>
		/// <param name="lockMode">The <see cref="LockMode"/> for the Entity.</param>
		/// <param name="existsInDatabase">A boolean indicating if the Entity exists in the database.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for this Entity.</param>
		/// <param name="disableVersionIncrement"></param>
		public EntityEntry(Status status, object[] loadedState, object id, object version, LockMode lockMode,
		                   bool existsInDatabase, IEntityPersister persister, bool disableVersionIncrement)
		{
			this.status = status;
			this.loadedState = loadedState;
			this.id = id;
			this.existsInDatabase = existsInDatabase;
			this.version = version;
			this.lockMode = lockMode;
			this.isBeingReplicated = disableVersionIncrement;
			this.persister = persister;
			if (persister != null)
			{
				className = persister.ClassName;
			}
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
			set { status = value; }
		}

		/// <summary>
		/// Gets or sets the identifier of the Entity in the database.
		/// </summary>
		/// <value>The identifier of the Entity in the database if one has been assigned.</value>
		/// <remarks>This might be <see langword="null" /> when the <see cref="EntityEntry.Status"/> is 
		/// <see cref="Impl.Status.Saving"/> and the database generates the id.</remarks>
		public object Id
		{
			get { return id; }
			set { id = value; }
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
			set { loadedState = value; }
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
			set { existsInDatabase = value; }
		}

		/// <summary>
		/// Gets or sets the version of the Entity.
		/// </summary>
		/// <value>The version of the Entity.</value>
		public object Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IEntityPersister"/> that is responsible for this Entity.
		/// </summary>
		/// <value>The <see cref="IEntityPersister"/> that is reponsible for this Entity.</value>
		public IEntityPersister Persister
		{
			get { return persister; }
			set { persister = value; }
		}

		/// <summary>
		/// Gets the Fully Qualified Name of the class this Entity is an instance of.
		/// </summary>
		/// <value>The Fully Qualified Name of the class this Entity is an instance of.</value>
		public string ClassName
		{
			get { return className; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsBeingReplicated
		{
			get { return isBeingReplicated; }
		}

		public object GetLoadedValue(string propertyName)
		{
			int propertyIndex = ((IUniqueKeyLoadable) persister).GetPropertyIndex(propertyName);
			return loadedState[propertyIndex];
		}
	}
}