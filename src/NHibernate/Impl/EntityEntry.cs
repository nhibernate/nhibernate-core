using System;

using NHibernate.Persister;

namespace NHibernate.Impl
{
	/// <summary>
	/// We need an entry to tell us all about the current state
	/// of an object with respect to its persistent state
	/// </summary>
	[Serializable]
	sealed internal class EntityEntry  
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(EntityEntry) );

		private LockMode _lockMode;
		private Status _status;
		private object _id;
		private object[] _loadedState;
		private object[] _deletedState;
		private bool _existsInDatabase;
		private object _version;
		// for convenience to save some lookups
		[NonSerialized] private IClassPersister _persister;
		private string _className;
			
		/// <summary>
		/// Initializes a new instance of EntityEntry.
		/// </summary>
		/// <param name="status">The current <see cref="Status"/> of the Entity.</param>
		/// <param name="loadedState">The snapshot of the Entity's state when it was loaded.</param>
		/// <param name="id">The identifier of the Entity in the database.</param>
		/// <param name="version">The version of the Entity.</param>
		/// <param name="lockMode">The <see cref="LockMode"/> for the Entity.</param>
		/// <param name="existsInDatabase">A boolean indicating if the Entity exists in the database.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for this Entity.</param>
		public EntityEntry(Status status, object[] loadedState, object id, object version, LockMode lockMode, bool existsInDatabase, IClassPersister persister) 
		{
			_status = status;
			_loadedState = loadedState;
			_id = id;
			_existsInDatabase = existsInDatabase;
			_version = version;
			_lockMode = lockMode;
			_persister = persister;
			if (_persister!=null) _className = _persister.ClassName;
		}

		/// <summary>
		/// Gets or sets the current <see cref="LockMode"/> of the Entity.
		/// </summary>
		/// <value>The <see cref="LockMode"/> of the Entity.</value>
		public LockMode LockMode
		{
			get { return _lockMode; }
			set { _lockMode = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="Status"/> of this Entity with respect to its 
		/// persistence in the database.
		/// </summary>
		/// <value>The <see cref="Status"/> of this Entity.</value>
		public Status Status
		{
			get { return _status; }
			set { _status = value; }
		}

		/// <summary>
		/// Gets or sets the identifier of the Entity in the database.
		/// </summary>
		/// <value>The identifier of the Entity in the database if one has been assigned.</value>
		/// <remarks>This might be <c>null</c> when the <see cref="EntityEntry.Status"/> is 
		/// <see cref="Status.Saving"/> and the database generates the id.</remarks>
		public object Id
		{
			get { return _id; }
			set { _id = value; }
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
			get { return _loadedState; }
			set { _loadedState = value; }
		}

		/// <summary>
		/// Gets or sets the snapshot of the Entity when it was marked as being ready for deletion.
		/// </summary>
		/// <value>The snapshot of the Entity.</value>
		/// <remarks>This will be <c>null</c> if the Entity is not being deleted.</remarks>
		public object[] DeletedState
		{
			get { return _deletedState; }
			set { _deletedState = value; }
		}

		/// <summary>
		/// Gets or sets a <see cref="Boolean"/> indicating if this Entity exists in the database.
		/// </summary>
		/// <value><c>true</c> if it is already in the database.</value>
		/// <remarks>
		/// It can also be <c>true</c> if it does not exists in the database yet and the 
		/// <see cref="IClassPersister.IsIdentifierAssignedByInsert"/> is <c>true</c>.
		/// </remarks>
		public bool ExistsInDatabase
		{
			get { return _existsInDatabase; }
			set { _existsInDatabase = value; }
		}

		/// <summary>
		/// Gets or sets the version of the Entity.
		/// </summary>
		/// <value>The version of the Entity.</value>
		public object Version
		{
			get { return _version; }
			set { _version = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="IClassPersister"/> that is responsible for this Entity.
		/// </summary>
		/// <value>The <see cref="IClassPersister"/> that is reponsible for this Entity.</value>
		public IClassPersister Persister
		{
			get { return _persister; }
			set { _persister = value; }
		}

		/// <summary>
		/// Gets the Fully Qualified Name of the class this Entity is an instance of.
		/// </summary>
		/// <value>The Fully Qualified Name of the class this Entity is an instance of.</value>
		public string ClassName
		{
			get { return _className; }
		}

	}
		
}
