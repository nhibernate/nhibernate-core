using System;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl 
{

	/// <summary>
	/// Concrete implementation of a Session, also the central, organizing component of
	/// Hibernate's internal implementaiton.
	/// </summary>
	/// <remarks>
	/// Exposes two interfaces: ISession itself, to the application and ISessionImplementor
	/// to other components of hibernate. This is where the hard stuff is...
	/// NOT THREADSAFE
	/// </remarks>
	[Serializable]
	internal class SessionImpl : ISessionImplementor, ISerializable, IDeserializationCallback
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SessionImpl));

		private SessionFactoryImpl factory;

		private bool autoClose;
		private long timestamp;

		private bool closed = false;
		private FlushMode flushMode = FlushMode.Auto;

		private bool callAfterTransactionCompletionFromDisconnect = true;

		private IDictionary entitiesByKey; //key=Key, value=Object
		private IDictionary proxiesByKey; //key=Key, value=HibernateProxy
		
		//IdentityMaps are serializable in NH 
		private IdentityMap entries;//key=Object, value=EntityEntry
		private IdentityMap arrayHolders; //key=array, value=ArrayHolder
		private IdentityMap collections; //key=PersistentCollection, value=CollectionEntry
		
		private IDictionary nullifiables = new Hashtable();  //set of Keys of deleted objects

		private IInterceptor interceptor;

		[NonSerialized] private IDbConnection connection;
		[NonSerialized] private bool connect;

		// TODO: find out if we want the reference to IDbTransaction or ITransaction - leaning
		// towards an ITransaction because we can get the IDbTransaction from that.
		[NonSerialized] private ITransaction transaction;

		// We keep scheduled insertions, deletions and updates in collections
		// and actually execute them as part of the flush() process. Actually,
		// not every flush() ends in execution of the scheduled actions. Auto-
		// flushes initiated by a query execution might be "shortcircuited".
	
		// Object insertions and deletions have list semantics because they
		// must happen in the right order so as to respect referential integrity
		[NonSerialized] private ArrayList insertions;
		[NonSerialized] private ArrayList deletions;
		// updates are kept in a Map because successive flushes might need to add
		// extra, new changes for an object thats already scheduled for update.
		// Note: we *could* treat updates the same way we treat collection actions
		// (discarding them at the end of a "shortcircuited" auto-flush) and then
		// we would keep them in a list
		[NonSerialized] private ArrayList updates;
		// Actually the semantics of the next three are really "Bag"
		// Note that, unlike objects, collection insertions, updates,
		// deletions are not really remembered between flushes. We
		// just re-use the same Lists for convenience.
		[NonSerialized] private ArrayList collectionCreations;
		[NonSerialized] private ArrayList collectionUpdates;
		[NonSerialized] private ArrayList collectionRemovals;

		[NonSerialized] private ArrayList executions;

		[NonSerialized] private int dontFlushFromFind = 0;
		[NonSerialized] private int cascading = 0;

		[NonSerialized] private IBatcher batcher;
		
		/// <summary>
		/// Represents the status of an entity with respect to 
		/// this session. These statuses are for internal 
		/// book-keeping only and are not intended to represent 
		/// any notion that is visible to the <b>application</b>. 
		/// </summary>
		[Serializable]
		internal enum Status 
		{
			/// <summary>
			/// The Entity is snapshotted in the Session with the same state as the database.
			/// </summary>
			Loaded,

			/// <summary>
			/// The Entity is in the Session and has been marked for deletion but not
			/// deleted from the database yet.
			/// </summary>
			Deleted,

			/// <summary>
			/// The Entity has been deleted from database.
			/// </summary>
			Gone,

			/// <summary>
			/// The Entity is in the process of being loaded.
			/// </summary>
			Loading,

			/// <summary>
			/// The Entity is in the process of being saved.
			/// </summary>
			Saving
		}

		/// <summary>
		/// An action that <see cref="ISession"/> can Execute during a
		/// <c>Flush</c>.
		/// </summary>
		internal interface IExecutable 
		{
			/// <summary>
			/// Execute the action required to write changes to the database.
			/// </summary>
			void Execute();

			/// <summary>
			/// Called after the Transaction has been completed.
			/// </summary>
			/// <remarks>
			/// Actions should make sure that the Cache is notified about
			/// what just happened.
			/// </remarks>
			void AfterTransactionCompletion();

			/// <summary>
			/// The spaces (tables) that are affectd by this Executable action.
			/// </summary>
			/// <remarks>
			/// This is used to determine if the ISession needs to be flushed before
			/// a query is executed so stale data is not returned.
			/// </remarks>
			object[] PropertySpaces { get; }
		}

		/// <summary>
		/// We need an entry to tell us all about the current state
		/// of an object with respect to its persistent state
		/// </summary>
		[Serializable]
		sealed internal class EntityEntry  
		{
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

		
		/// <summary>
		/// We need an entry to tell us all about the current state
		/// of a collection with respect to its persistent state
		/// </summary>
		[Serializable]
		public class CollectionEntry : ICollectionSnapshot 
		{
			internal bool dirty;
			
			/// <summary>
			/// Indicates that the Collection can still be reached by an Entity
			/// that exist in the <see cref="ISession"/>.
			/// </summary>
			/// <remarks>
			/// It is also used to ensure that the Collection is not shared between
			/// two Entities.  
			/// </remarks>
			[NonSerialized] internal bool reached;
			
			/// <summary>
			/// Indicates that the Collection has been processed and is ready
			/// to have its state synchronized with the database.
			/// </summary>
			[NonSerialized] internal bool processed;
			
			/// <summary>
			/// Indicates that a Collection needs to be updated.
			/// </summary>
			/// <remarks>
			/// A Collection needs to be updated whenever the contents of the Collection
			/// have been changed. 
			/// </remarks>
			[NonSerialized] internal bool doupdate;
			
			/// <summary>
			/// Indicates that a Collection has old elements that need to be removed.
			/// </summary>
			/// <remarks>
			/// A Collection needs to have removals performed whenever its role changes or
			/// the key changes and it has a loadedPersister - ie - it was loaded by NHibernate.
			/// </remarks>
			[NonSerialized] internal bool doremove;
			
			/// <summary>
			/// Indicates that a Collection needs to be recreated.
			/// </summary>
			/// <remarks>
			/// A Collection needs to be recreated whenever its role changes
			/// or the owner changes.
			/// </remarks>
			[NonSerialized] internal bool dorecreate;
			
			/// <summary>
			/// Indicates that the Collection has been fully initialized.
			/// </summary>
			internal bool initialized;
			
			/// <summary>
			/// The <see cref="CollectionPersister"/> that is currently responsible
			/// for the Collection.
			/// </summary>
			/// <remarks>
			/// This is set when NHibernate is updating a reachable or an
			/// unreachable collection.
			/// </remarks>
			[NonSerialized] internal CollectionPersister currentPersister;
			
			/// <summary>
			/// The <see cref="CollectionPersister"/> when the Collection was loaded.
			/// </summary>
			/// <remarks>
			/// This can be <c>null</c> if the Collection was not loaded by NHibernate and 
			/// was passed in along with a transient object.
			/// </remarks>
			[NonSerialized] internal CollectionPersister loadedPersister;
			[NonSerialized] internal object currentKey;
			internal object loadedKey;
			internal object snapshot; //session-start/post-flush persistent state
			internal  string role;
			
			/// <summary>
			/// Initializes a new instance of <see cref="CollectionEntry"/>.
			/// </summary>
			/// <remarks> 
			/// The CollectionEntry is for a Collection that is not dirty and 
			/// has already been initialized.
			/// </remarks>
			public CollectionEntry() 
			{
				this.dirty = false;
				this.initialized = true;
			}

			/// <summary>
			/// Initializes a new instance of <see cref="CollectionEntry"/>. 
			/// </summary>
			/// <param name="loadedPersister">The <see cref="CollectionPersister"/> that persists this Collection type.</param>
			/// <param name="loadedID">The identifier of the Entity that is the owner of this Collection.</param>
			/// <param name="initialized">A boolean indicating if the collection has been initialized.</param>
			public CollectionEntry(CollectionPersister loadedPersister, object loadedID, bool initialized) 
			{
				this.dirty = false;
				this.initialized = initialized;
				this.loadedKey = loadedID;
				SetLoadedPersister(loadedPersister);
			}

			/// <summary>
			/// Initializes a new instance of <see cref="CollectionEntry"/>. 
			/// </summary>
			/// <param name="cs">The <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/>.</param>
			/// <param name="factory">The <see cref="ISessionFactoryImplementor"/> that created this <see cref="ISession"/>.</param>
			/// <remarks>
			/// This takes an <see cref="ICollectionSnapshot"/> from another <see cref="ISession"/> and 
			/// creates an entry for it in this <see cref="ISession"/> by copying the values from the 
			/// <c>cs</c> parameter.
			/// </remarks>
			public CollectionEntry(ICollectionSnapshot cs, ISessionFactoryImplementor factory) 
			{
				this.dirty = cs.Dirty;
				this.snapshot = cs.Snapshot;
				this.loadedKey = cs.Key;
				SetLoadedPersister( factory.GetCollectionPersister( cs.Role ) );
				this.initialized = true;
			}

			/// <summary>
			/// Checks to see if the <see cref="PersistentCollection"/> has had any changes to the 
			/// collections contents or if any of the elements in the collection have been modified.
			/// </summary>
			/// <param name="coll"></param>
			/// <returns><c>true</c> if the <see cref="PersistentCollection"/> is dirty.</returns>
			/// <remarks>
			/// default behavior; will be overridden in deep lazy collections
			/// </remarks>
			public virtual bool IsDirty(PersistentCollection coll) 
			{
				// if this has already been marked as dirty or the collection can not 
				// be directly accessed (ie- we can guarantee that the NHibernate collection
				// wrappers are used) and the elements in the collection are not mutable 
				// then return the dirty flag.
				if ( dirty || (
					!coll.IsDirectlyAccessible && !loadedPersister.ElementType.IsMutable
					) ) 
				{
					return dirty;
				} 
				else 
				{
					// need to have the coll determine if it is the same as the snapshot
					// that was last taken.
					return !coll.EqualsSnapshot( loadedPersister.ElementType );
				}
			}

			/// <summary>
			/// Prepares this CollectionEntry for the Flush process.
			/// </summary>
			/// <param name="collection">The <see cref="PersistentCollection"/> that this CollectionEntry will be responsible for flushing.</param>
			public void PreFlush(PersistentCollection collection) 
			{
				// if the collection is initialized and it was previously persistent
				// initialize the dirty flag
				dirty = ( initialized && loadedPersister!=null && IsDirty(collection) ) ||
					(!initialized && dirty ); //only need this so collection with queued adds will be removed from JCS cache

				if ( log.IsDebugEnabled && dirty && loadedPersister!=null ) 
				{
					log.Debug("Collection dirty: " + MessageHelper.InfoString(loadedPersister, loadedKey) );
				}

				// reset all of these values so any previous flush status 
				// information is cleared from this CollectionEntry
				doupdate = false;
				doremove = false;
				dorecreate = false;
				reached = false;
				processed = false;
			}

			/// <summary>
			/// Updates the CollectionEntry to reflect that the <see cref="PersistentCollection"/>
			/// has been initialized.
			/// </summary>
			/// <param name="collection">The initialized <see cref="PersistentCollection"/> that this Entry is for.</param>
			public void PostInitialize(PersistentCollection collection) 
			{
				initialized = true;
				snapshot = collection.GetSnapshot(loadedPersister);
			}

			/// <summary>
			/// Updates the CollectionEntry to reflect that it is has been successfully flushed to the database.
			/// </summary>
			/// <param name="collection">The <see cref="PersistentCollection"/> that was flushed.</param>
			public void PostFlush(PersistentCollection collection) 
			{
				// the CollectionEntry should be processed if we are in the PostFlush()
				if( !processed ) 
				{
					throw new AssertionFailure("Hibernate has a bug processing collections");
				}

				// now that the flush has gone through move everything that is the current
				// over to the loaded fields and set dirty to false since the db & collection
				// are in synch.
				loadedKey = currentKey;
				SetLoadedPersister( currentPersister );
				dirty = false;
				
				// collection needs to know its' representation in memory and with
				// the db is now in synch - esp important for collections like a bag
				// that can add without initializing the collection.
				collection.PostFlush();

				// if it was initialized or any of the scheduled actions were performed then
				// need to resnpashot the contents of the collection.
				if ( initialized && ( doremove || dorecreate || doupdate ) ) 
				{
					snapshot = collection.GetSnapshot(loadedPersister); //re-snapshot
				}
			}

			#region Engine.ICollectionSnapshot Members

			public object Key 
			{
				get { return loadedKey; }
			}
			
			public string Role 
			{
				get { return role; }
			}
			
			public object Snapshot 
			{
				get { return snapshot; }
			}

			public bool Dirty 
			{
				get { return dirty; }
			}
			
			public void SetDirty() 
			{
				dirty = true;
			}
			public bool IsInitialized 
			{
				get { return initialized;}
			}

			
			#endregion

			/// <summary>
			/// Sets the information in this CollectionEntry that is specific to the
			/// <see cref="CollectionPersister"/>.
			/// </summary>
			/// <param name="persister">
			/// The <see cref="CollectionPersister"/> that is 
			/// responsible for the Collection.
			/// </param>
			private void SetLoadedPersister(CollectionPersister persister) 
			{
				loadedPersister = persister;
				if (persister!=null) 
				{
					role=persister.Role;
				}
			}

			public bool SnapshotIsEmpty 
			{
				get 
				{
					//TODO: implementation here is non-extensible ... 
					//should use polymorphism 
					//	return initialized && snapshot!=null && ( 
					//		( snapshot is IList && ( (IList) snapshot ).Count==0 ) || // if snapshot is a collection 
					//		( snapshot is Map && ( (Map) snapshot ).Count==0 ) || // if snapshot is a map 
					//		(snapshot.GetType().IsArray && ( (Array) snapshot).Length==0 )// if snapshot is an array 
					//		); 
					
					// TODO: in .NET an IList, IDictionary, and Array are all collections so we might be able
					// to just cast it to a ICollection instead of all the diff collections.
					return initialized && snapshot!=null && ( 
						( snapshot is IList && ( (IList) snapshot ).Count==0 ) || // if snapshot is a collection 
						( snapshot is IDictionary && ( (IDictionary) snapshot ).Count==0 ) || // if snapshot is a map 
						(snapshot.GetType().IsArray && ( (Array) snapshot).Length==0 )// if snapshot is an array 
						); 
				}
			} 
			public bool IsNew 
			{
				// TODO: is this correct implementation - h2.0.3
				get { return initialized && (snapshot==null); }
			}
		}
		
		
		#region System.Runtime.Serialization.ISerializable Members 

		/// <summary>
		/// Constructor used to recreate the Session during the deserialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <remarks>
		/// This is needed because we have to do some checking before the serialization process
		/// begins.  I don't know how to add logic in ISerializable.GetObjectData and have .net
		/// write all of the serializable fields out.
		/// </remarks>
		protected SessionImpl(SerializationInfo info, StreamingContext context)
		{
			this.factory = (SessionFactoryImpl)info.GetValue( "factory", typeof(SessionFactoryImpl) );
			this.autoClose = info.GetBoolean("autoClose");
			this.timestamp = info.GetInt64("timestamp");
			this.closed = info.GetBoolean("closed");
			this.flushMode = (FlushMode)info.GetValue( "flushMode", typeof(FlushMode) );
			this.callAfterTransactionCompletionFromDisconnect = info.GetBoolean("callAfterTransactionCompletionFromDisconnect");
			this.entitiesByKey = (IDictionary)info.GetValue( "entitiesByKey", typeof(IDictionary) );
			this.proxiesByKey = (IDictionary)info.GetValue( "proxiesByKey", typeof(IDictionary) );
			this.nullifiables = (IDictionary)info.GetValue( "nullifiables", typeof(IDictionary) );
			this.interceptor = (IInterceptor)info.GetValue( "interceptor", typeof(IInterceptor) );

			this.entries = (IdentityMap)info.GetValue( "entries", typeof(IdentityMap) );
			this.collections = (IdentityMap)info.GetValue( "collections", typeof(IdentityMap) );
			this.arrayHolders = (IdentityMap)info.GetValue( "arrayHolders", typeof(IdentityMap) );

		}

		/// <summary>
		/// Verify the ISession can be serialized and write the fields to the Serializer.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		/// <remarks>
		/// The fields are marked with [NonSerializable] as just a point of reference.  This method
		/// has complete control and what is serialized and those attributes are ignored.  However, 
		/// this method should be in synch with the attributes for easy readability.
		/// </remarks>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if ( IsConnected ) throw new InvalidOperationException("Cannot serialize a Session while connected");
			if ( insertions.Count!=0 || deletions.Count!=0 )
				throw new InvalidOperationException("Cannot serialize a Session which has work waiting to be flushed");
					
			log.Info("serializing session");

			info.AddValue( "factory", factory, typeof(SessionFactoryImpl) );
			info.AddValue( "autoClose", autoClose );
			info.AddValue( "timestamp", timestamp );
			info.AddValue( "closed", closed );
			info.AddValue( "flushMode", flushMode );
			info.AddValue( "callAfterTransactionCompletionFromDisconnect", callAfterTransactionCompletionFromDisconnect );
			info.AddValue( "entitiesByKey", entitiesByKey, typeof(IDictionary) );
			info.AddValue( "proxiesByKey", proxiesByKey, typeof(IDictionary) );
			info.AddValue( "nullifiables", nullifiables, typeof(IDictionary) );
			info.AddValue( "interceptor", interceptor, typeof(IInterceptor) );

			info.AddValue( "entries", entries, typeof(IdentityMap) );
			info.AddValue( "collections", collections, typeof(IdentityMap) );
			info.AddValue( "arrayHolders", arrayHolders, typeof(IdentityMap) );
		}
		#endregion

		#region System.Runtime.Serialization.IDeserializationCallback Members 

		/// <summary>
		/// Once the entire object graph has been deserialized then we can hook the
		/// collections, proxies, and entities back up to the ISession.
		/// </summary>
		/// <param name="sender"></param>
		void IDeserializationCallback.OnDeserialization(Object sender)
		{
			log.Info("deserializing session");
			
			// don't need any section for IdentityMaps because .net does not have a problem
			// serializing them like java does.

			InitTransientCollections();
			foreach(DictionaryEntry e in collections)
			{
				try
				{
					((PersistentCollection)e.Key).SetSession(this);
					CollectionEntry ce = (CollectionEntry)e.Value;
					ce.loadedPersister = factory.GetCollectionPersister(ce.Role);
				}
				catch (HibernateException he)
				{
					// Different from h2.0.3
					throw new InvalidOperationException(he.Message);
				}
			}
			
//			TODO: figure out why proxies are having problems.  The enumerator appears to be throwing
//			a null reference exception when the proxiesByKey.Count==0
//			foreach(object proxy in proxiesByKey.Values)
//			{
//				object proxy = proxyEnumer.Current;
//				if (proxy is HibernateProxy) 
//				{
//					HibernateProxyHelper.GetLazyInitializer(proxy as HibernateProxy).SetSession(this);
//				}
//				else 
//				{
//					// the proxy was pruned during the serialization process
//					proxiesByKey.Remove(proxy); 
//				}
//			}

			foreach(EntityEntry e in entries.Values)
			{
				try
				{
					e.Persister = factory.GetPersister(e.ClassName);
				}
				catch (MappingException me)
				{
					// Different from h2.0.3
					throw new InvalidOperationException(me.Message);
				}
			}
		}
		#endregion

		internal SessionImpl(IDbConnection connection, SessionFactoryImpl factory, bool autoClose, long timestamp, IInterceptor interceptor) 
		{
			this.connection = connection;
			connect = connection==null;
			this.interceptor = interceptor;

			this.autoClose = autoClose;
			this.timestamp = timestamp;
			
			this.factory = factory;

			entitiesByKey = new Hashtable(50);
			proxiesByKey = new Hashtable(10);
			//TODO: hack with this cast
			entries = (IdentityMap)IdentityMap.InstantiateSequenced();
			collections = (IdentityMap)IdentityMap.InstantiateSequenced();
			arrayHolders = (IdentityMap)IdentityMap.Instantiate();

			InitTransientCollections();

			log.Debug("opened session");
		}

		public IBatcher Batcher 
		{
			get 
			{
				if (batcher==null) 
				{
					batcher = new NonBatchingBatcher(this);
				}
				return batcher;
			}
		}

		public ISessionFactoryImplementor Factory 
		{
			get { return factory; }
		}

		public long Timestamp 
		{
			get { return timestamp; }
		}

		public IDbConnection Close() 
		{
			log.Debug("closing session");

			try 
			{
				return (connection==null) ? null : Disconnect();
			} 
			finally 
			{
				Cleanup();
			}
		}

		public void AfterTransactionCompletion() 
		{
			log.Debug("transaction completion");

			// downgrade locks
			foreach(EntityEntry entry in entries.Values) 
			{
				entry.LockMode = LockMode.None;
			}
			
			// release cache softlocks
			foreach(IExecutable executable in executions) 
			{
				try 
				{
					executable.AfterTransactionCompletion();
				} 
				catch (CacheException ce) 
				{
					log.Error("could not release a cache lock", ce);
					// continue loop
				} 
				catch (Exception e) 
				{
					throw new AssertionFailure("Exception releasing cache locks", e);
				}
			}
			executions.Clear();

			callAfterTransactionCompletionFromDisconnect = true; //not really necessary
		}

		private void InitTransientCollections() 
		{
			insertions = new ArrayList(20);
			deletions = new ArrayList(20);
			updates = new ArrayList(20);
			collectionCreations = new ArrayList(20);
			collectionRemovals = new ArrayList(20);
			collectionUpdates = new ArrayList(20);
			executions = new ArrayList(50);
		}

		private void Cleanup() 
		{
			closed = true;
			entitiesByKey.Clear();
			proxiesByKey.Clear();
			entries.Clear();
			arrayHolders.Clear();
			collections.Clear();
			nullifiables.Clear();
		}

		public LockMode GetCurrentLockMode(object obj) 
		{
			if ( obj is HibernateProxy ) 
			{
				obj = (HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj)).GetImplementation(this);
				if (obj==null) return LockMode.None;
			}

			EntityEntry e = GetEntry(obj);
			if (e==null) throw new TransientObjectException("Given object not associated with the session");
			
			if (e.Status!=Status.Loaded) throw new ObjectDeletedException("The given object was deleted", e.Id);
			return e.LockMode;
		}

		public LockMode GetLockMode(object entity) 	
		{
			return GetEntry(entity).LockMode;
		}

		private void AddEntity(Key key, object obj) 
		{
			entitiesByKey[key] = obj;
		}

		public object GetEntity(Key key) 
		{
			return entitiesByKey[key];
		}

		private object RemoveEntity(Key key) 
		{
			object retVal = entitiesByKey[key];
			entitiesByKey.Remove(key);
			return retVal;
		}

		public void SetLockMode(object entity, LockMode lockMode) 
		{
			GetEntry(entity).LockMode = lockMode;
		}

		private EntityEntry AddEntry(
			object obj,
			Status status,
			object[] loadedState,
			object id,
			object version,
			LockMode lockMode,
			bool existsInDatabase,
			IClassPersister persister) 
		{

			EntityEntry e = new EntityEntry(status, loadedState, id, version, lockMode, existsInDatabase, persister);
			entries[obj] = e;
			return e;
		}

		private EntityEntry GetEntry(object obj) 
		{
			return (EntityEntry) entries[obj];
		}
		private EntityEntry RemoveEntry(object obj) 
		{
			object retVal = entries[obj];
			entries.Remove(obj);
			return (EntityEntry) retVal;
		}
		private bool IsEntryFor(object obj)
		{
			return entries.Contains(obj);
		}

		/// <summary>
		/// Add a new collection (ie an initialized one, instantiated by the application)
		/// </summary>
		/// <param name="collection"></param>
		private void AddNewCollection(PersistentCollection collection) 
		{
			CollectionEntry ce = new CollectionEntry();
			collections[collection] = ce;
			collection.CollectionSnapshot = ce;
		}

		private CollectionEntry GetCollectionEntry(PersistentCollection coll) 
		{
			return (CollectionEntry) collections[coll];
		}

		public bool IsOpen 
		{
			get { return !closed; }
		}

		/// <summary>
		/// Save a transient object. An id is generated, assigned to the object and returned
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Save(object obj) 
		{
			
			if (obj==null) throw new NullReferenceException("attempted to save null");

			if ( !NHibernate.IsInitialized(obj) ) throw new PersistentObjectException("uninitialized proxy passed to save()"); 
			object theObj = UnproxyAndReassociate(obj); 


			EntityEntry e = GetEntry(theObj);
			if ( e!=null ) 
			{
				if ( e.Status==Status.Deleted) 
				{
					Flush();
				} 
				else 
				{
					log.Debug( "object already associated with session" );
					return e.Id;
				}
			}

			object id;
			try 
			{
				id = GetPersister(theObj).IdentifierGenerator.Generate(this, theObj);
				if( id == (object) IdentifierGeneratorFactory.ShortCircuitIndicator) return GetIdentifier(theObj); //TODO: yick!
			} 
			catch (Exception ex) 
			{
				throw new ADOException("Could not save object", ex);
			}

			return DoSave(theObj, id);
		}

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save(object obj, object id) {

			if (obj==null) throw new NullReferenceException("attemted to insert null");
			if (id==null) throw new NullReferenceException("null identifier passed to insert()");

			if ( !NHibernate.IsInitialized(obj) ) throw new PersistentObjectException("uninitialized proxy passed to save()"); 
			object theObj = UnproxyAndReassociate(obj); 

			EntityEntry e = GetEntry(theObj);
			if ( e!=null ) 
			{
				if ( e.Status==Status.Deleted ) 
				{
					Flush();
				} 
				else 
				{
					if ( !id.Equals(e.Id) ) 
						throw new PersistentObjectException(
							"object passed to save() was already persistent: " + 
							MessageHelper.InfoString(e.Persister, id)
							);
					log.Debug( "object already associated with session" );
				}
			}
			DoSave(theObj, id);
		}

		private object DoSave(object obj, object id) {
			IClassPersister persister = GetPersister(obj);

			Key key = null;
			bool identityCol;
			if (id==null) 
			{
				if ( persister.IsIdentifierAssignedByInsert ) 
				{
					identityCol = true;
				} 
				else 
				{
					throw new AssertionFailure("null id");
				}
			} 
			else 
			{
				identityCol = false;
			}

			if ( log.IsDebugEnabled ) log.Debug( "saving " + MessageHelper.InfoString(persister, id) );

			if (!identityCol) 
			{ 
				// if the id is generated by the db, we assign the key later
				key = new Key(id, persister);

				object old = GetEntity(key);
				if (old!=null) 
				{
					if ( GetEntry(old).Status==Status.Deleted) 
					{
						Flush();
					} 
					else 
					{
						throw new HibernateException(
							"The generated identifier is already in use: " + MessageHelper.InfoString(persister, id)
							);
					}
				}

				persister.SetIdentifier(obj, id);
			}

			// sub-insertions should occur befoer containing insertions so
			// try to do the callback now
			if ( persister.ImplementsLifecycle ) 
			{
				//TODO: H2.0.3 - verify this has same meaning as H2.0.3
				if ( ( (ILifecycle) obj ).OnSave(this) == LifecycleVeto.Veto ) return id;
			}

			if ( persister.ImplementsValidatable ) ( (IValidatable) obj ).Validate();

			// Put a placeholder in entries, so we don't recurse back and try to save() th
			// same object again.
			AddEntry(obj, Status.Saving, null, id, null, LockMode.Write, identityCol, persister);

			// cascade-save to many-to-one BEFORE the parent is saved
			cascading++;
			try 
			{
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeBeforeInsertAfterDelete);
			} 
			finally 
			{
				cascading--;
			}

			object[] values = persister.GetPropertyValues(obj);
			IType[] types = persister.PropertyTypes;

			bool substitute = interceptor.OnSave( obj, id, values, persister.PropertyNames, types );

			substitute = ( persister.IsVersioned && Versioning.SeedVersion(
				values, persister.VersionProperty, persister.VersionType ) ) || substitute;

			if ( Wrap( values, persister.PropertyTypes ) || substitute) 
			{ 
				//substitutes into values by side-effect
				persister.SetPropertyValues(obj, values);
			}

			TypeFactory.DeepCopy(values, types, persister.PropertyUpdateability, values);
			NullifyTransientReferences(values, types, identityCol, obj);

			if (identityCol) 
			{
				try 
				{
					id = persister.Insert(values, obj, this);
				} 
				catch (Exception e) 
				{
					throw new ADOException("Could not insert", e);
				}

				key = new Key(id, persister);

				if ( GetEntity(key) != null ) throw new HibernateException("The natively generated ID is already in use " + MessageHelper.InfoString(persister, id));

				persister.SetIdentifier(obj, id);
			}

			AddEntity(key, obj);
			AddEntry(obj, Status.Loaded, values, id, Versioning.GetVersion(values, persister), LockMode.Write, identityCol, persister);
			
			if (!identityCol) insertions.Add( new ScheduledInsertion( id, values, obj, persister, this ) );

			// cascade-save to collections AFTER the collection owner was saved
			cascading++;
			try 
			{
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeAfterInsertBeforeDelete);
			} 
			finally 
			{
				cascading--;
			}
			
			return id;
		}

		private void ReassociateProxy(Object value) 
		{ 
			HibernateProxy proxy = (HibernateProxy) value; 
			LazyInitializer li = HibernateProxyHelper.GetLazyInitializer(proxy); 
			ReassociateProxy(li, proxy); 
		} 
    
		private object UnproxyAndReassociate(object maybeProxy) 
		{ 
			if ( maybeProxy is HibernateProxy ) 
			{ 
				HibernateProxy proxy = (HibernateProxy) maybeProxy; 
				LazyInitializer li = HibernateProxyHelper.GetLazyInitializer(proxy); 
				ReassociateProxy(li, proxy); 
				return li.GetImplementation(); //initialize + unwrap the object 
			} 
			else 
			{ 
				return maybeProxy; 
			} 
		} 
    
		/// <summary>
		/// associate a proxy that was instantiated by another session with this session
		/// </summary>
		/// <param name="li"></param>
		/// <param name="proxy"></param>
		private void ReassociateProxy(LazyInitializer li, HibernateProxy proxy) 
		{ 
			if ( li.Session!=this ) 
			{ 
				IClassPersister persister = GetPersister( li.PersistentClass ); 
				Key key = new Key( li.Identifier, persister ); 
				if ( !proxiesByKey.Contains(key) ) proxiesByKey[key] = proxy; // any earlier proxy takes precedence 
				HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) proxy ).SetSession(this); 
			} 
		} 

		private void NullifyTransientReferences(object[] values, IType[] types, bool earlyInsert, object self) 
		{
			for (int i=0; i<types.Length; i++ ) 
			{
				values[i] = NullifyTransientReferences( values[i], types[i], earlyInsert, self );
			}
		}

		/// <summary>
		/// Return null if the argument is an "unsaved" entity (ie. one with no existing database row), 
		/// or the input argument otherwise. This is how Hibernate avoids foreign key constraint violations.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="earlyInsert"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		private object NullifyTransientReferences(object value, IType type, bool earlyInsert, object self) 
		{
			if ( value==null ) 
			{
				return null;
			} 
			else if ( type.IsEntityType || type.IsObjectType ) 
			{
				return ( IsUnsaved(value, earlyInsert, self) ) ? null : value;
			} 
			else if ( type.IsComponentType ) 
			{
				IAbstractComponentType actype = (IAbstractComponentType) type;
				object[] subvalues = actype.GetPropertyValues(value, this);
				IType[] subtypes = actype.Subtypes;
				bool substitute = false;
				for (int i=0; i<subvalues.Length; i++ ) 
				{
					object replacement = NullifyTransientReferences( subvalues[i], subtypes[i], earlyInsert, self );
					if ( replacement != subvalues[i] ) 
					{
						substitute = true;
						subvalues[i] = replacement;
					}
				}
				if (substitute) actype.SetPropertyValues(value, subvalues);
				return value;
			} 
			else 
			{
				return value;
			}
		}

		/// <summary>
		/// determine if the object already exists in the database, using a "best guess"
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="earlyInsert"></param>
		/// <param name="self"></param>
		/// <returns></returns>
		private bool IsUnsaved(object obj, bool earlyInsert, object self) 
		{
			if ( obj is HibernateProxy ) 
			{
				// if its an uninitialized proxy, it can't be transietn
				LazyInitializer li = HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj );
				if ( li.GetImplementation(this)==null ) 
				{
					return false;
					// ie we never have to null out a reference to an uninitialized proxy
				} 
				else 
				{
					try 
					{
						//unwrap it
						obj = li.GetImplementation(this);
					} 
					catch (HibernateException he) 
					{
						//does not occur
						throw new AssertionFailure("Unexpected HibernateException occurred in IsTransient()", he);
					}
				}
			}

			// if it was a reference to self, don't need to nullify
			// unless we are using native id generation, in which
			// case we definitely need to nullify
			if (obj==self) return earlyInsert;

			// See if the entity is already bound to this session, if not look at the
			// entity identifier and assume that the entity is persistent if the
			// id is not "unsaved" (that is, we rely on foreign keys to keep
			// database integrity)

			EntityEntry e = GetEntry(obj);
			if (e==null) 
			{
				IClassPersister persister = GetPersister(obj);
				if ( persister.HasIdentifierProperty ) 
				{
					object id = persister.GetIdentifier( obj );
					if (id!=null) 
					{
						// see if theres another object that *is* associated with the sesison for that id
						e = GetEntry( GetEntity( new Key(id, persister) ) );

						if (e==null) 
						{ 
							// look at the id value
							return persister.IsUnsaved(id);
						}
						// else use the other object's entry...
					} 
					else 
					{ 
						// null id, so have to assume transient (because that's safer)
						return true;
					}
				} 
				else 
				{ 
					//can't determine the id, so assume transient (because that's safer)
					return true;
				}
			}

		
			return e.Status==Status.Saving || (
				earlyInsert ? !e.ExistsInDatabase : nullifiables.Contains( new Key(e.Id, e.Persister) )
				);
		}

		/// <summary>
		/// Delete a persistent object
		/// </summary>
		/// <param name="obj"></param>
		public void Delete(object obj) 
		{
			if (obj==null) throw new NullReferenceException("attempted to delete null");

			//object theObj = UnproxyAndReassociate(obj);
			obj = UnproxyAndReassociate(obj);

			EntityEntry entry = GetEntry(obj);
			IClassPersister persister=null;
			if (entry==null) 
			{
				log.Debug("deleting a transient instance");

				persister = GetPersister(obj);
				object id = persister.GetIdentifier(obj);

				if (id==null) throw new HibernateException("the transient instance passed to Delete() has a null identifier");

				object old = GetEntity( new Key(id, persister) );

				if (old!=null) 
				{
					throw new HibernateException(
						"another object with the same id was already associated with the session: " +
						MessageHelper.InfoString(persister, id)
						);
				}

				RemoveCollectionsFor(persister, id, obj);

				AddEntity( new Key(id, persister), obj);
				entry = AddEntry(
					obj, 
					Status.Loaded, 
					persister.GetPropertyValues(obj),
					id,
					persister.GetVersion(obj),
					LockMode.None,
					true,
					persister
					);
				// not worth worrying about the proxy
			}
			else 
			{
				log.Debug("deleting a persistent instance");

				if ( entry.Status==Status.Deleted || entry.Status==Status.Gone) {
					log.Debug("object was already deleted");
					return;
				}
				persister = entry.Persister;
			}

			if ( !persister.IsMutable ) 
			{
				throw new HibernateException(
					"attempted to delete an object of immutable class: " + 
					MessageHelper.InfoString(persister)
					);
			}

			if ( log.IsDebugEnabled ) log.Debug( "deleting " + MessageHelper.InfoString(persister, entry.Id) );

			IType[] propTypes = persister.PropertyTypes;

			object version = entry.Version;

			if (entry.LoadedState==null ) 
			{ 
				//ie the object came in from Update()
				entry.DeletedState = persister.GetPropertyValues(obj);
			} 
			else 
			{
				entry.DeletedState = new object[entry.LoadedState.Length];
				TypeFactory.DeepCopy(entry.LoadedState, propTypes, persister.PropertyUpdateability, entry.DeletedState);
			}

			interceptor.OnDelete(obj, entry.Id, entry.DeletedState, persister.PropertyNames, propTypes);

			NullifyTransientReferences(entry.DeletedState, propTypes, false, obj);

			// in h2.0.3 this is a Set
			IDictionary oldNullifiables = null;
			ArrayList oldDeletions = null;
			if ( persister.HasCascades ) 
			{
				oldNullifiables = new Hashtable(nullifiables);
				oldDeletions = (ArrayList) deletions.Clone();
			}

			nullifiables[ new Key(entry.Id, persister) ] = new object();
			entry.Status = Status.Deleted; // before any callbacks, etc, so subdeletions see that this deletion happend first
			ScheduledDeletion delete = new ScheduledDeletion(entry.Id, version, obj, persister, this);
			deletions.Add(delete); // ensures that containing deletions happen before sub-deletions

			try 
			{
				// after nullify, because we don't want to nullify references to subdeletions
				// try to do callback + cascade
				if ( persister.ImplementsLifecycle ) 
				{
					if ( ( (ILifecycle)obj).OnDelete(this) == LifecycleVeto.Veto ) 
					{
						//rollback deletion
						RollbackDeletion(entry, delete);
						return; //don't let it cascade
					}
				}

				//BEGIN YUCKINESS:
				if ( persister.HasCascades ) 
				{
					int start = deletions.Count;

					IDictionary newNullifiables = nullifiables;
					nullifiables = oldNullifiables;

					cascading++;
					try 
					{
						// cascade-delete to collections "BEFORE" the collection owner is deleted
						Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeAfterInsertBeforeDelete);
					} 
					finally 
					{
						cascading--;
						foreach(DictionaryEntry oldNullifyDictEntry in oldNullifiables) 
						{
							newNullifiables[oldNullifyDictEntry.Key] = oldNullifyDictEntry.Value;
						}
						nullifiables = newNullifiables;
					}

					int end = deletions.Count;

					if ( end!=start ) 
					{ 
						//ie if any deletions occurred as a result of cascade

						//move them earlier. this is yucky code:
						
						// in h203 they used SubList where it takes the start and end indexes, in nh GetRange
						// takes the start index and quantity to get.
						IList middle = deletions.GetRange( oldDeletions.Count, (start - oldDeletions.Count) );
						IList tail = deletions.GetRange( start, (end - start) );

						oldDeletions.AddRange(tail);
						oldDeletions.AddRange(middle);

						if ( oldDeletions.Count != end ) throw new AssertionFailure("Bug cascading collection deletions");

						deletions = oldDeletions;
					}
				}
				//END YUCKINESS

				// cascade-save to many-to-one AFTER the parent was saved
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeBeforeInsertAfterDelete);
			} 
			catch (Exception e) 
			{ 
				//mainly a CallbackException
				RollbackDeletion(entry, delete);
				if (e is HibernateException) 
				{
					throw;
				} 
				else 
				{
					log.Error("unexpected exception", e);
					throw new HibernateException("unexpected exception", e);
				}
				
			}
		}

		private void RollbackDeletion(EntityEntry entry, ScheduledDeletion delete) 
		{
			entry.Status = Status.Loaded;
			entry.DeletedState = null;
			deletions.Remove(delete);
		}

		private void RemoveCollectionsFor(IClassPersister persister, object id, object obj) 
		{
			if ( persister.HasCollections ) 
			{
				IType[] types = persister.PropertyTypes;
				for (int i=0; i<types.Length; i++) 
				{
					RemoveCollectionsFor( types[i], id, persister.GetPropertyValue(obj, i) );
				}
			}
		}

		private void RemoveCollection(CollectionPersister role, object id) 
		{
			if ( log.IsDebugEnabled ) log.Debug( "collection dereferenced while transient " + MessageHelper.InfoString(role, id) ); 
			if(role.HasOrphanDelete) 
			{
				throw new HibernateException("You may not dereference a collection with cascade=\"all-delete-orphan\"");
			}
			collectionRemovals.Add( new ScheduledCollectionRemove(role, id, false, this) );
		}

		//TODO: rename this method - comment from H2.0.3
		/// <summary>
		/// When an entity is passed to update(), we must inspect all its collections and 
		/// 1. associate any uninitialized PersistentCollections with this session 
		/// 2. associate any initialized PersistentCollections with this session, using the		
		/// existing snapshot 
		/// 3. execute a collection removal (SQL DELETE) for each null collection property 
		///    or "new" collection 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <param name="value"></param>
		private void RemoveCollectionsFor(IType type, object id, object value) 
		{
			if ( type.IsPersistentCollectionType ) 
			{
				CollectionPersister persister = GetCollectionPersister( ( (PersistentCollectionType) type).Role );
				if ( value!=null && (value is PersistentCollection) ) 
				{
					PersistentCollection coll = (PersistentCollection) value;
					if ( coll.SetSession(this) ) 
					{
						ICollectionSnapshot snapshot = coll.CollectionSnapshot;
						if( snapshot != null &&
							snapshot.Role.Equals( persister.Role ) &&
							id.Equals(snapshot.Key)
							) 
						{
							// a transient collection that originally 
							// belonged to the same entity
							if(coll.WasInitialized) 
							{
								AddInitializedCollection(coll, snapshot);
							}
							else 
							{
								AddUninitializedCollection(coll, persister, id);
							}
						}
						else 
						{
							// a "transient" collection that belonged to
							// a different object
							RemoveCollection(persister, id);
						}
					} 
					else 
					{
						// a collection loaded in the current seession
						// can not possibly be the collection belonging
						// to the entity passed to update()
						RemoveCollection(persister, id);
					}
				} 
				else 
				{
					// null or brand new collection
					RemoveCollection(persister, id);
				}

			} 
			else if ( type.IsEntityType ) 
			{
				if ( value!=null ) 
				{
					IClassPersister persister = GetPersister( ( (EntityType) type).PersistentClass );
					if ( persister.HasProxy && !NHibernate.IsInitialized(value) )
						ReassociateProxy(value);
				}
			} 
			else if ( type.IsComponentType ) 
			{
				if ( value!=null ) 
				{
					IAbstractComponentType actype = (IAbstractComponentType) type;
					IType[] types = actype.Subtypes;
					for (int i=0; i<types.Length; i++ ) 
					{
						RemoveCollectionsFor( types[i], id, actype.GetPropertyValue(value, i, this) );
					}
				}
			}
		}
		
		public void Update(object obj) 
		{

			if (obj==null) throw new NullReferenceException("attempted to update null");
			
			if(!NHibernate.IsInitialized(obj)) 
			{
				ReassociateProxy(obj);
				return;
			}

			object theObj = UnproxyAndReassociate(obj);

			IClassPersister persister = GetPersister(theObj);

			if ( IsEntryFor(theObj) ) 
			{
				log.Debug("object already associated with session");
				// do nothing
			} 
			else 
			{
				// the object is transient
				object id = persister.GetIdentifier(theObj);

				if (id==null) 
				{
					// assume this is a newly instantiated transient object 
					throw new HibernateException("The given object has a null identifier property " + MessageHelper.InfoString(persister));
				} 
				else 
				{
					DoUpdate(theObj, id);
				}
			}
		}

		public void SaveOrUpdate(object obj) 
		{
			if (obj==null) throw new NullReferenceException("attempted to update null");
			
			if ( !NHibernate.IsInitialized(obj) ) 
			{
				ReassociateProxy(obj);
				return;
			}
			object theObj = UnproxyAndReassociate(obj);

			EntityEntry e = GetEntry(theObj);
			if (e!=null && e.Status!=Status.Deleted) 
			{
				// do nothing for persistent instances
				log.Debug("SaveOrUpdate() persistent instance");
			} 
			else if (e!=null) 
			{ 
				//ie status==DELETED
				log.Debug("SaveOrUpdate() deleted instance");
				Save(obj);
			} 
			else 
			{
				// the object is transient
				object isUnsaved = interceptor.IsUnsaved(theObj);
				if (isUnsaved==null)
				{
					// use unsaved-value
					IClassPersister persister = GetPersister(theObj);
					if ( persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier ) 
					{
						object id = persister.GetIdentifier(theObj);

						if ( persister.IsUnsaved(id) ) 
						{
							if ( log.IsDebugEnabled ) log.Debug("SaveOrUpdate() unsaved instance with id: " + id);
							Save(obj);
						} 
						else 
						{
							if ( log.IsDebugEnabled ) log.Debug("SaveOrUpdate() previously saved instance with id: " + id);
							DoUpdate(theObj, id);
						}
					} 
					else 
					{
						// no identifier property ... default to save()
						log.Debug("SaveOrUpdate() unsaved instance with no identifier property");
						Save(obj);
					}
				} 
				else 
				{
					if ( true.Equals(isUnsaved) ) 
					{
						log.Debug("SaveOrUpdate() unsaved instance");
						Save(obj);
					} 
					else 
					{
						log.Debug("SaveOrUpdate() previously saved instance");
						Update(obj);
					}
				}
			}
		}

		public void Update(object obj, object id) 
		{
			if (id==null) throw new NullReferenceException("null is not a valid identifier");
			if (obj==null) throw new NullReferenceException("attempted to update null");
			
			if ( obj is HibernateProxy ) 
			{
				object pid = HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj ).Identifier;
				if( !id.Equals(pid) )
					throw new HibernateException("The given proxy had a different identifier value to the given identifier: " + pid + "!=" + id);
			}

			if( !NHibernate.IsInitialized(obj) ) 
			{
				ReassociateProxy(obj);
				return;
			}

			object theObj = UnproxyAndReassociate(obj);

			EntityEntry e = GetEntry(theObj);
			if (e==null) 
			{
				DoUpdate(theObj, id);
			} 
			else 
			{
				if ( !e.Id.Equals(id) ) 
					throw new PersistentObjectException(
							"The instance passed to Update() was already persistent: " +
							MessageHelper.InfoString(e.Persister, id)
							);
			}
		}

		private void DoUpdate(object obj, object id) {
			
			IClassPersister persister = GetPersister(obj);

			if ( !persister.IsMutable ) 
				throw new HibernateException(
					"attempted to update an object of immutable class: " + 
					MessageHelper.InfoString(persister)
					);
			
			if ( log.IsDebugEnabled ) log.Debug( "updating " + MessageHelper.InfoString(persister, id) );

			Key key = new Key(id, persister);
			object old = GetEntity(key);
			if (old==obj) 
			{
				throw new AssertionFailure(
					"Hibernate has a bug in Update() ... or you are using an illegal id type: " +
					MessageHelper.InfoString(persister, id)
					);
			} 
			else if ( old!=null ) 
			{
				throw new HibernateException(
					"Another object was associated with this id ( the object with the given id was already loaded): " +
					MessageHelper.InfoString(persister, id)
					);
			}

			// this is a transient object with existing persistent state not loaded by the session

			if ( persister.ImplementsLifecycle && ((ILifecycle) obj ).OnUpdate(this) == LifecycleVeto.Veto ) return; // do callback

			RemoveCollectionsFor(persister, id, obj);

			AddEntity(key, obj);
			AddEntry(obj, Status.Loaded, null, id, persister.GetVersion(obj), LockMode.None, true, persister);

			cascading++;
			try 
			{
				Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate); // do cascade
			} 
			finally 
			{
				cascading--;
			}
		}

		private static object[] NoArgs = new object[0];
		private static IType[] NoTypes = new IType[0];

		public IList Find(string query) 
		{
			return Find(query, NoArgs, NoTypes);
		}

		public IList Find(string query, object value, IType type) 
		{
			return Find( query, new object[] { value }, new IType[] { type } );
		}

		public IList Find(string query, object[] values, IType[] types) 
		{
			return Find(query, new QueryParameters( types, values ) ) ;
		}

		public IList Find(string query, QueryParameters parameters) 
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "find: " + query);
				parameters.LogParameters();
			}

			parameters.ValidateParameters();

			QueryTranslator[] q = GetQueries(query, false);

			IList results = new ArrayList();

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called

			//execute the queries and return all result lists as a single list
			try 
			{
				for (int i=0; i<q.Length; i++ ) 
				{
					IList currentResults;
					try 
					{
						currentResults = q[i].FindList(this, parameters, true);
					} 
					catch (Exception e) 
					{
						throw new ADOException("Could not execute query", e);
					}
					
					for (int j=0;j<results.Count;j++) 
					{
						currentResults.Add( results[j] );
					}
					results = currentResults;
				}
			} 
			finally 
			{
				dontFlushFromFind--;
			}
			return results;

		}

		private QueryTranslator[] GetQueries(string query, bool scalar) 
		{
			// a query that naemes an interface or unmapped class in the from clause
			// is actually executed as multiple queries
			string[] concreteQueries = QueryTranslator.ConcreteQueries(query, factory);

			// take the union of the query spaces (ie the queried tables)
			QueryTranslator[] q = new QueryTranslator[concreteQueries.Length];
			ArrayList qs = new ArrayList();
			for (int i=0; i<concreteQueries.Length; i++ ) 
			{
				q[i] = scalar ? factory.GetShallowQuery( concreteQueries[i] ) : factory.GetQuery( concreteQueries[i] );
				qs.AddRange( q[i].QuerySpaces );
			}

			AutoFlushIfRequired(qs);

			return q;
		}

		public IEnumerable Enumerable(string query) 
		{
			return Enumerable(query, NoArgs, NoTypes);
		}

		public IEnumerable Enumerable(string query, object value, IType type) 
		{
			return Enumerable( query, new object[] { value }, new IType[] { type } );
		}

		public IEnumerable Enumerable(string query, object[] values, IType[] types) 
		{
			return Enumerable( query, new QueryParameters( types, values ) );
		}

		public IEnumerable Enumerable(string query, QueryParameters parameters) 
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "GetEnumerable: " + query );
				parameters.LogParameters();
			}

			QueryTranslator[] q = GetQueries(query, true);

			if (q.Length==0) return new ArrayList();

			IEnumerable result = null;
			IEnumerable[] results = null;
			bool many = q.Length>1;
			if( many ) 
			{
				results = new IEnumerable[q.Length];
			}

			//execute the queries and return all results as a single enumerable
			for (int i=0; i<q.Length; i++) 
			{
				try 
				{
					result = q[i].GetEnumerable( parameters, this );
				} 
				catch (Exception e) 
				{
					throw new ADOException("Could not execute query", e);
				}
				if(many) 
				{
					results[i] = result;
				}
			}

			return many ? new JoinedEnumerable(results) : result;
		}

		public int Delete(string query) 
		{
			return Delete(query, NoArgs, NoTypes);
		}

		public int Delete(string query, object value, IType type) 
		{
			return Delete( query, new object[] { value }, new IType[] { type } );
		}

		public int Delete(string query, object[] values, IType[] types) 
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug ( "delete: " + query );
				if ( values.Length!=0 ) log.Debug( "parameters: " + StringHelper.ToString(values) );
			}

			IList list = Find(query, values, types);
			int count = list.Count;
			for (int i=0; i<count; i++ ) Delete( list[i] );
			return count;
		}

		public void Lock(object obj, LockMode lockMode) {
			
			if (obj==null) throw new NullReferenceException("attempted to lock null");

			if (lockMode==LockMode.Write) throw new HibernateException("Invalid lock mode for Lock()");

			obj = UnproxyAndReassociate(obj);
			//TODO: if object was an uninitialized proxy, this is inefficient, 
			//resulting in two SQL selects 

			EntityEntry e = GetEntry(obj);
			if (e==null) throw new TransientObjectException("attempted to lock a transient instance");
			IClassPersister persister = e.Persister;

			if ( lockMode.GreaterThan(e.LockMode) ) 
			{
				if (e.Status!=Status.Loaded) throw new TransientObjectException("attempted to lock a deleted instance");

				if ( log.IsDebugEnabled ) log.Debug( "locking " + MessageHelper.InfoString(persister, e.Id) + " in mode: " + lockMode);

				if ( persister.HasCache ) persister.Cache.Lock(e.Id);
				try 
				{
					persister.Lock(e.Id, e.Version, obj, lockMode, this);
					e.LockMode = lockMode;
				} 
				catch (Exception exp) 
				{
					throw new ADOException("could not lock object", exp);
				} 
				finally 
				{
					// the database now holds a lock + the object is flushed from the cache,
					// so release the soft lock
					if ( persister.HasCache ) persister.Cache.Release(e.Id);
				}
			}
		}

		public IQuery CreateFilter(object collection, string queryString) 
		{
			return new FilterImpl(queryString, collection, this);
		}

		public IQuery CreateQuery(string queryString) 
		{
			return new QueryImpl(queryString, this);
		}
		public IQuery GetNamedQuery(string queryName) 
		{
			return CreateQuery( factory.GetNamedQuery(queryName) );
		}

		public object Instantiate(System.Type clazz, object id) 
		{
			return Instantiate( factory.GetPersister(clazz), id );
		}

		/// <summary>
		/// Give the interceptor an opportunity to override the default instantiation
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object Instantiate(IClassPersister persister, object id) 
		{
			object result = interceptor.Instantiate( persister.MappedClass, id );
			if (result==null) result = persister.Instantiate(id);
			return result;
		}

		public FlushMode FlushMode 
		{
			get { return flushMode; }
			set { flushMode = value; }
		}

		/// <summary>
		/// detect in-memory changes, determine if the changes are to tables
		/// named in the query and, if so, complete execution the flush
		/// </summary>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		private bool AutoFlushIfRequired(IList querySpaces) 
		{

			if ( flushMode==FlushMode.Auto && dontFlushFromFind==0 ) 
			{

				int oldSize = collectionRemovals.Count;

				FlushEverything();

				if ( AreTablesToBeUpdated(querySpaces) ) 
				{
					log.Debug("Need to execute flush");

					Execute();
					PostFlush();
					// note: Execute() clears all collectionXxxxtion collections
					return true;
				} 
				else 
				{
					log.Debug("dont need to execute flush");

					// sort of don't like this: we re-use the same collections each flush
					// even though their state is not kept between flushes. However, its
					// nice for performance since the collection sizes will be "nearly"
					// what we need them to be next time.
					collectionCreations.Clear();
					collectionUpdates.Clear();
					updates.Clear();
					// collection deletes are a special case since Update() can add
					// deletions of collections not loaded by the session.
					for (int i=collectionRemovals.Count-1; i>=oldSize; i-- ) 
					{
						collectionRemovals.RemoveAt(i);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// If the existing proxy is insufficiently "narrow" (derived), instantiate a 
		/// new proxy and overwrite the registration of the old one. This breaks == and 
		/// occurs only for "class" proxies rather than "interface" proxies.
		/// </summary>
		/// <param name="proxy"></param>
		/// <param name="p"></param>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object NarrowProxy(object proxy, IClassPersister p, Key key, object obj) 
		{

			if ( !p.ConcreteProxyClass.IsAssignableFrom( proxy.GetType() ) ) 
			{
				if ( log.IsWarnEnabled ) 
					log.Warn(
						 "Narrowing proxy to " + p.ConcreteProxyClass + " - this operation breaks =="
						 );

				if (obj!=null) 
				{
					proxiesByKey.Remove(key);
					return obj;
				} 
				else 
				{
					//TODO: Get the proxy - there is some CGLIB code here
					proxy = null; 

					proxiesByKey[key] = proxy;
					return proxy;
				}
			} 
			else 
			{
				return proxy;
			}
		}

		// Grab the existing proxy for an instance, if one exists
		public object ProxyFor(IClassPersister persister, Key key, object impl) 
		{
			if ( !persister.HasProxy ) return impl;
			object proxy = proxiesByKey[key];
			if (proxy!=null) 
			{
				return NarrowProxy(proxy, persister, key, impl);
			} 
			else 
			{
				return impl;
			}
		}

		public object ProxyFor(object impl) 
		{
			EntityEntry e = GetEntry(impl);

			// can't use e.persister since it is null after addUninitializedEntity 
			// (when this method is called)
			IClassPersister p = GetPersister(impl);
			return ProxyFor( p, new Key(e.Id, p), impl);
		}

		/// <summary>
		/// Create a "temporary" entry for a newly instantiated entity. The entity is 
		/// uninitialized, but we need the mapping from id to instance in order to guarantee 
		/// uniqueness.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void AddUninitializedEntity(Key key, object obj, LockMode lockMode) {
			//IClassPersister p = GetPersister(obj);
			AddEntity(key, obj);
			AddEntry(obj, Status.Loading, null, key.Identifier, null, lockMode, true, null); // p );
		}

		/// <summary>
		/// Add the "hydrated state" (an array) of an uninitialized entity to the session. 
		/// We don't try to resolve any associations yet, because there might be other entities 
		/// waiting to be read from the ADO datareader we are currently processing
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="values"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void PostHydrate(IClassPersister persister, object id, object[] values, object obj, LockMode lockMode) {
			persister.SetIdentifier(obj, id);
			object version = Versioning.GetVersion(values, persister);
			AddEntry(obj, Status.Loaded, values, id, version, lockMode, true, persister);

			if ( log.IsDebugEnabled && version!=null) log.Debug("Version: " + version);
		}

		private void ThrowObjectNotFound(object o, object id, System.Type clazz) 
		{
			if (o==null) throw new ObjectNotFoundException( "No row with the given identifier exists", id, clazz );
		}

		public void Load(object obj, object id) 
		{
			if (id==null) throw new NullReferenceException("null is not a valid identifier");
			DoLoadByObject(obj, id, true, LockMode.None);
		}

		public object Load(System.Type clazz, object id) 
		{
			if (id==null) throw new NullReferenceException("null is not a valid identifier");
			object result = DoLoadByClass(clazz, id, true, true);
			ThrowObjectNotFound(result, id, clazz);
			return result;
		}

		///<summary> 
		/// Load the data for the object with the specified id into a newly created object.
		/// Do NOT return a proxy.
		///</summary>
		public object ImmediateLoad(System.Type clazz, object id) 
		{
			object result = DoLoad(clazz, id, null, LockMode.None, false);
			ThrowObjectNotFound(result, id, clazz);
			return result;
		}

		///<summary>
		/// Return the object with the specified id or null if no row with that id exists. Do not defer the load
		/// or return a new proxy (but do return an existing proxy). Do not check if the object was deleted.
		///</summary>
		public object InternalLoadOneToOne(System.Type clazz, object id) 
		{
			return DoLoadByClass(clazz, id, false, false);
		}

		/**
		* Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		* return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		*/
		public object InternalLoad(System.Type clazz, object id) 
		{
			object result = DoLoadByClass(clazz, id, false, true);
			ThrowObjectNotFound(result, id, clazz);
			return result;
		}
		
		/**
		* Load the data for the object with the specified id into the supplied
		* instance. A new key will be assigned to the object. If there is an
		* existing uninitialized proxy, this will break identity equals as far
		* as the application is concerned.
		*/
		private void DoLoadByObject(object obj, object id, bool checkDeleted, LockMode lockMode) 
		{
			System.Type clazz = obj.GetType();
			if ( GetEntry(obj)!=null ) 
			{
				throw new PersistentObjectException(
					"attempted to load into an instance that was already associated with the Session: "+
					MessageHelper.InfoString(clazz, id)
					);
			}
			object result = DoLoad(clazz, id, obj, lockMode, checkDeleted);
			ThrowObjectNotFound(result, id, clazz);
			if (result!=obj) 
			{
				throw new HibernateException(
					"The object with that id was already loaded by the Session: " +
					MessageHelper.InfoString(clazz, id)
					);
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created
		/// object. A new key will be assigned to the object. If the class supports
		/// lazy initialization, return a proxy instead, leaving the real work for
		/// later. This should return an existing proxy where appropriate.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> of the object to load.</param>
		/// <param name="id">The identifier of the object in the database.</param>
		/// <param name="checkDeleted">
		/// A boolean indicating if NHiberate should check if the object has or has not been deleted.
		/// </param>
		/// <param name="allowProxyCreation">A boolean indicating if it is allowed to return a Proxy instead of an instance of the <see cref="System.Type"/>.</param>
		/// <returns>
		/// An loaded instance of the object or a proxy of the object is proxies are allowed.
		/// </returns>
		/// <remarks>
		/// If the parameter <c>checkDeleted</c> is <c>false</c> it is possible to return an object that has 
		/// been deleted by the user in this <see cref="ISession"/>.  If the parameter <c>checkDeleted</c> is
		/// <c>true</c> and the object has been deleted then an <see cref="ObjectDeletedException"/> will be
		/// thrown.
		/// </remarks>
		private object DoLoadByClass(System.Type clazz, object id, bool checkDeleted, bool allowProxyCreation) 
		{
			if ( log.IsDebugEnabled ) log.Debug( "loading " + MessageHelper.InfoString(clazz, id) );

			IClassPersister persister = GetPersister(clazz);
			if ( !persister.HasProxy ) 
			{
				// this class has no proxies (so do a shortcut)
				return DoLoad(clazz, id, null, LockMode.None, checkDeleted);
			} 
			else 
			{
				Key key = new Key(id, persister);
				object proxy = null;

				if ( GetEntity(key)!=null ) 
				{
					// return existing object or initialized proxy (unless deleted)
					return ProxyFor(
						persister,
						key,
						DoLoad(clazz, id, null, LockMode.None, checkDeleted)
						);
				} 
				else if ( ( proxy = proxiesByKey[key] ) != null ) 
				{
					// return existing uninitizlied proxy
					return NarrowProxy(proxy, persister, key, null);
				} 
				else if ( allowProxyCreation ) 
				{

					// return new uninitailzed proxy
					//TODO: commented this out so we could get all of the test running - this basically makes
					// the proxy of a class be ignored - which is fine until we have it working.
					if ( persister.HasProxy ) 
					{
						proxy = null; //TODO: Create the proxy
						// this is the spot that is causing the problems with FooBarTest.FetchInitializedCollection
						// when the following code "Assert.IsTrue( baz.fooBag.Count==2 );" is being executed.  This
						// is causing a null value to be returned when a "Proxied" version of the class is expected.
						// So the method ThrowObjectNotFound is throwing an exception because it is given a null object
						// - hence the error looks like it can't find a row in the DB.
					}
					proxiesByKey[key] = proxy;
					return proxy;
				} 
				else 
				{
					// return a newly loaded object
					return DoLoad(clazz, id, null, LockMode.None, checkDeleted);
				}
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		public object Load(System.Type clazz, object id, LockMode lockMode) 
		{
			if ( lockMode==LockMode.Write ) throw new HibernateException("invalid lock mode for Load()");

			if ( log.IsDebugEnabled ) log.Debug( "loading " + MessageHelper.InfoString(clazz, id) + " in lock mode: " + lockMode );
			if (id==null) throw new NullReferenceException("null is not a valid identifier");

			IClassPersister persister = GetPersister(clazz);
			if ( persister.HasCache ) persister.Cache.Lock(id); //increments the lock
			object result;
			try 
			{
				result = DoLoad(clazz, id, null, lockMode, true);
			} 
			finally 
			{
				// the datbase now hold a lock + the object is flushed from the cache,
				// so release the soft lock
				if ( persister.HasCache ) persister.Cache.Release(id);
			}

			ThrowObjectNotFound( result, id, persister.MappedClass );

			// retunr existing proxy (if one exists)
			return ProxyFor(persister, new Key(id, persister), result );
		}

		/// <summary>
		/// Actually do all the hard work of loading up an object
		/// </summary>
		/// <param name="theClass"></param>
		/// <param name="id"></param>
		/// <param name="optionalObject"></param>
		/// <param name="lockMode"></param>
		/// <param name="checkDeleted"></param>
		/// <returns></returns>
		/// <remarks>
		/// 1. see if it is already loaded
		/// 2. see if it is cached
		/// 3. actually go to the database
		/// </remarks>
		private object DoLoad(System.Type theClass, object id, object optionalObject, LockMode lockMode, bool checkDeleted) 
		{
			//DONT need to flush before a load by id

			if ( log.IsDebugEnabled ) log.Debug( "attempting to resolve " + MessageHelper.InfoString(theClass, id) );

			bool isOptionalObject = optionalObject!=null;

			IClassPersister persister = GetPersister(theClass);
			Key key = new Key(id, persister);

			// LOOK FOR LOADED OBJECT 
			// Look for Status.Loaded object
			object old = GetEntity(key);
			if (old!=null) 
			{ //if this object was already loaded
				Status status = GetEntry(old).Status;
				if ( checkDeleted && ( status==Status.Deleted || status==Status.Gone) ) 
				{
					throw new ObjectDeletedException("The object with that id was deleted", id);
				}
				Lock(old, lockMode);
				if ( log.IsDebugEnabled ) log.Debug( "resolved object in session cache " + MessageHelper.InfoString(persister, id) );
				return old;
			
			} 
			else 
			{
				// LOOK IN CACHE
				CacheEntry entry = persister.HasCache ? (CacheEntry) persister.Cache.Get(id, timestamp) : null;
				if (entry!=null) 
				{
					if ( log.IsDebugEnabled ) log.Debug( "resolved object in JCS cache " + MessageHelper.InfoString(persister, id) );
					IClassPersister subclassPersister = GetPersister( entry.Subclass );
					object result = (isOptionalObject) ? optionalObject : Instantiate(subclassPersister, id);
					AddEntry(result, Status.Loading, null, id, null, LockMode.None, true, subclassPersister);
					AddEntity( new Key(id, persister), result );
					object[] values = entry.Assemble(result, id, subclassPersister, this); // intializes result by side-effect

					IType[] types = subclassPersister.PropertyTypes;
					TypeFactory.DeepCopy(values, types, subclassPersister.PropertyUpdateability, values);
					object version = Versioning.GetVersion(values, subclassPersister);
					
					if ( log.IsDebugEnabled ) log.Debug("Cached Version: " + version);
					AddEntry(result, Status.Loaded, values, id, version, LockMode.None, true, subclassPersister);
					
					// upgrate lock if necessary;
					Lock(result, lockMode);

					return result;
				
				} 
				else 
				{
					//GO TO DATABASE
					if ( log.IsDebugEnabled ) log.Debug( "object not resolved in any cache " + MessageHelper.InfoString(persister, id) );
					try 
					{
						return persister.Load(id, optionalObject, lockMode, this);
					} 
						//TODO: change to some kind of SqlException
					catch (Exception e) 
					{
						throw new ADOException("could not load object", e);
					}
				}
			}
		}

		public void Refresh(object obj) 
		{
			Refresh(obj, LockMode.Read);
		}

		public void Refresh(object obj, LockMode lockMode) 
		{
			if (obj==null) throw new NullReferenceException("attempted to refresh null");

			if ( !NHibernate.IsInitialized(obj) ) 
			{ 
				ReassociateProxy(obj); 
				return; 
			} 

			object theObj = UnproxyAndReassociate(obj);
			EntityEntry e = RemoveEntry(theObj);

			if(e==null) 
			{
				IClassPersister persister = GetPersister(theObj);
				object id = persister.GetIdentifier(theObj);
				if(log.IsDebugEnabled) 
				{
					log.Debug( "refreshing transient " + MessageHelper.InfoString(persister, id) );
				}

				DoLoadByObject(theObj, id, true, lockMode);
			}
			else 
			{
				if ( log.IsDebugEnabled ) log.Debug( "refreshing " + MessageHelper.InfoString(e.Persister, e.Id) );

				if ( !e.ExistsInDatabase ) throw new HibernateException("this instance does not yet exist as a row in the database");

				Key key = new Key(e.Id, e.Persister);
				RemoveEntity( key );
				EvictCollections( e.Persister.GetPropertyValues(obj), e.Persister.PropertyTypes );

				try 
				{
					e.Persister.Load( e.Id, theObj, lockMode, this);
				} 
				catch (Exception exp) 
				{
					throw new ADOException("could not refresh object", exp);
				}
				GetEntry(theObj).LockMode = e.LockMode;
			}
		}

		/// <summary>
		/// After processing a JDBC result set, we "resolve" all the associations
		/// between the entities which were instantiated and had their state
		/// "hydrated" into an array
		/// </summary>
		/// <param name="obj"></param>
		public void InitializeEntity(object obj) 
		{
			EntityEntry e = GetEntry(obj);
			IClassPersister persister = e.Persister;
			object id = e.Id;
			object[] hydratedState = e.LoadedState;
			IType[] types = persister.PropertyTypes;

			if(log.IsDebugEnabled) 
			{
				log.Debug("resolving associations for: " + MessageHelper.InfoString(persister, id) );
			}

			interceptor.OnLoad( obj, id, hydratedState, persister.PropertyNames, types );

			for ( int i=0; i<hydratedState.Length; i++ ) 
			{
				hydratedState[i] = types[i].ResolveIdentifier( hydratedState[i], this, obj );
			}

			persister.SetPropertyValues(obj, hydratedState);
			TypeFactory.DeepCopy(hydratedState, persister.PropertyTypes, persister.PropertyUpdateability, hydratedState); 

			if ( persister.HasCache ) 
			{
				if ( log.IsDebugEnabled ) log.Debug( "adding entity to JCS cache " + MessageHelper.InfoString(persister, id) );
				persister.Cache.Put( id, new CacheEntry( obj, persister, this), timestamp );
			}

			// reentrantCallback=true;
			if ( persister.ImplementsLifecycle ) ((ILifecycle) obj).OnLoad(this, id);

			// reentrantCallback=false;
			if ( log.IsDebugEnabled ) log.Debug( "done materializing entity " + MessageHelper.InfoString(persister, id) );
		}

		public ITransaction BeginTransaction() 
		{
			callAfterTransactionCompletionFromDisconnect = false;

			transaction = factory.TransactionFactory.BeginTransaction(this);

			return transaction;
		}

		public ITransaction Transaction 
		{
			get {return transaction;}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This can be called from commit() or at the start of a Find() method.
		/// <para>
		/// Perform all the necessary SQL statements in a sensible order, to allow
		/// users to repect foreign key constraints:
		/// <list type="">
		///		<item>Inserts, in the order they were performed</item>
		///		<item>Updates</item>
		///		<item>Deletion of collection elements</item>
		///		<item>Insertion of collection elements</item>
		///		<item>Deletes, in the order they were performed</item>
		/// </list>
		/// </para>
		/// <para>
		/// Go through all the persistent objects and look for collections they might be
		/// holding. If they had a nonpersistable collection, substitute a persistable one
		/// </para>
		/// </remarks>
		public void Flush() 
		{
			if (cascading>0) 
			{
				throw new HibernateException(
					"Flush during cascade is dangerous - this might occur if an object as deleted and then re-saved by cascade"
					);
			}
			FlushEverything();
			Execute();
			PostFlush();
		}

		private void FlushEverything() 
		{
			log.Debug("flushing session");

			interceptor.PreFlush( entitiesByKey.Values );

			PreFlushEntities();
			PreFlushCollections();
			FlushEntities();
			FlushCollections();

			//some stats

			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "Flushed: " +
					insertions.Count + " insertions, " +
					updates.Count + " updates, " +
					deletions.Count + " deletions to " +
					entries.Count + " objects");
				log.Debug( "Flushed: " +
					collectionCreations.Count + " (re)creations, " +
					collectionUpdates.Count + " updates, " +
					collectionRemovals.Count + " removals to " +
					collections.Count + " collections");
			}
		}

		private bool AreTablesToBeUpdated(IList tables) 
		{
			return AreTablesToUpdated( updates, tables) ||
				AreTablesToUpdated( insertions, tables) ||
				AreTablesToUpdated( deletions, tables) ||
				AreTablesToUpdated( collectionUpdates, tables) ||
				AreTablesToUpdated( collectionCreations, tables) ||
				AreTablesToUpdated( collectionRemovals, tables);
		}

		private bool AreTablesToUpdated(ICollection coll, IList theSet) 
		{
			foreach( IExecutable executable in coll ) 
			{
				object[] spaces = executable.PropertySpaces;
				for (int i=0; i<spaces.Length; i++) 
				{
					if ( theSet.Contains( spaces[i] ) ) return true;
				}
			}
			return false;
		}

		private void Execute() 
		{
			log.Debug("executing flush");

			try 
			{
				// see the comments in ExecuteAll for why the Clear() has been added here...
				ExecuteAll( insertions );
				insertions.Clear();

				ExecuteAll( updates );
				updates.Clear();

				ExecuteAll( collectionRemovals );
				collectionRemovals.Clear();

				ExecuteAll( collectionUpdates );
				collectionUpdates.Clear();

				ExecuteAll( collectionCreations );
				collectionCreations.Clear();

				ExecuteAll( deletions );
				deletions.Clear();

			} 
			catch (Exception e) 
			{
				throw new ADOException("could not synchronize database state with session", e);
			}
		}

		public void PostInsert(object obj) 
		{
			GetEntry(obj).ExistsInDatabase = true;
		}

		public void PostDelete(object obj) 
		{
			EntityEntry e = RemoveEntry(obj);
			e.Status = Status.Gone;
			Key key = new Key(e.Id, e.Persister);
			RemoveEntity(key);
			proxiesByKey.Remove(key);
		}

		public void PostUpdate(object obj, object[] updatedState, object nextVersion) 
		{
			EntityEntry e = GetEntry(obj);
			e.LoadedState = updatedState;
			e.LockMode = LockMode.Write;
			if(e.Persister.IsVersioned) 
			{
				e.Version = nextVersion;
				e.Persister.SetPropertyValue(obj, e.Persister.VersionProperty, nextVersion);
			}
		}

		private void ExecuteAll(ICollection coll) 
		{
			foreach(IExecutable e in coll) 
			{
				executions.Add(e);
				e.Execute();
				// this was moved to Execute because there is no way to
				// remove an item from an enumerator...
				// iter.remove -> coll.Remove()??
			}
			
			if ( batcher!=null ) batcher.ExecuteBatch();
		}

		/// <summary>
		/// 1. detect any dirty entities
		/// 2. schedule any entity updates
		/// 3. search out any reachable collections
		/// </summary>
		private void FlushEntities() 
		{
			log.Debug("Flushing entities and processing referenced collections");

			// Among other things, updateReachables() will recursively load all
			// collections that are moving roles. This might cause entities to
			// be loaded.
		
			// So this needs to be safe from concurrent modification problems.
			// It is safe because of how IdentityMap implements entrySet()
			
			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries(entries);

			foreach(DictionaryEntry me in iterSafeCollection) 
			{	
				EntityEntry entry = (EntityEntry) me.Value;
				Status status = entry.Status;

				if (status != Status.Loading && status != Status.Gone) 
				{
					object obj = me.Key;
					IClassPersister persister = entry.Persister;

					// make sure user didn't mangle the id
					if ( persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier ) 
					{
						object oid = persister.GetIdentifier(obj);

						if ( !entry.Id.Equals(oid) ) 
						{
							throw new HibernateException(
								"identiifier of an instance of " +
								persister.ClassName +
								" altered from " +
								entry.Id + "(" + entry.Id.GetType().ToString() + ")" +
								" to " + oid + "(" + oid.GetType().ToString() + ")"
								);
						}
					}

					object[] values;
					if ( status==Status.Deleted) 
					{
						//grab its state saved at deletion
						values = entry.DeletedState;
					} 
					else 
					{
						//grab its current state
						values = persister.GetPropertyValues(obj);
					}
					IType[] types = persister.PropertyTypes;

					// wrap up any new collections directly referenced by the object
					// or its components

					// NOTE: we need to do the wrap here even if its not "dirty",
					// because nested collections need wrapping but changes to
					// _them_ don't dirty the container. Also, for versioned
					// data, we need to wrap before calling searchForDirtyCollections

					bool substitute = Wrap(values, types); // substitutes into values by side-effect

					bool cannotDirtyCheck;
					bool interceptorHandledDirtyCheck;

					int[] dirtyProperties = interceptor.FindDirty(obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types);

					if ( dirtyProperties==null ) 
					{
						// interceptor returned null, so do the dirtycheck ourself, if possible
						interceptorHandledDirtyCheck = false;
						cannotDirtyCheck = entry.LoadedState==null; // object loaded by update()
						if ( !cannotDirtyCheck ) 
						{
							dirtyProperties = persister.FindDirty(values, entry.LoadedState, obj, this);
						}
					} 
					else 
					{
						// the interceptor handled the dirty checking
						cannotDirtyCheck = false;
						interceptorHandledDirtyCheck = true;
					}

					// compare to cached state (ignoring nested collections)
					if ( persister.IsMutable &&
						(cannotDirtyCheck ||
						(dirtyProperties!=null && dirtyProperties.Length!=0 ) ||
						(status==Status.Loaded && persister.IsVersioned && persister.HasCollections && SearchForDirtyCollections(values, types) )
						)
						) 
					{
						// its dirty!

						if ( log.IsDebugEnabled ) 
						{
							if(status == Status.Deleted) 
							{ 
								log.Debug("Updating deleted entity: " + MessageHelper.InfoString(persister, entry.Id) );
							}
							else 
							{
								log.Debug("Updating entity: " + MessageHelper.InfoString(persister, entry.Id) );
							}
						}

						// give the Interceptor a chance to modify property values
						bool intercepted = interceptor.OnFlushDirty(
							obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types);

						//no we might need to recalculate the dirtyProperties array
						if(intercepted && !cannotDirtyCheck && !interceptorHandledDirtyCheck) 
						{
							dirtyProperties = persister.FindDirty(values, entry.LoadedState, obj, this);
						}
						// if the properties were modified by the Interceptor, we need to set them back to the object
						substitute = substitute || intercepted;

						// validate() instances of Validatable
						if(status == Status.Loaded && persister.ImplementsValidatable) 
						{
							((IValidatable)obj).Validate();
						}

						//increment the version number (if necessary)
						object nextVersion = entry.Version;
						if(persister.IsVersioned) 
						{
							if(status!=Status.Deleted) nextVersion = Versioning.Increment(entry.Version, persister.VersionType);
							Versioning.SetVersion(values, nextVersion, persister);
						}
						
						// get the updated snapshot by cloning current state
						object[] updatedState = null;
						if(status==Status.Loaded)
						{
							updatedState = new object[values.Length];
							TypeFactory.DeepCopy(values, types, persister.PropertyUpdateability, updatedState);
						}
						
						updates.Add(
							new ScheduledUpdate(entry.Id, values, dirtyProperties, entry.Version, nextVersion, obj, updatedState, persister, this)
							);
					}
					
					if(status==Status.Deleted) 
					{
						//entry.status = Status.Gone;
					}
					else 
					{
						// now update the object... has to be outside the main if block above (because of collections)
						if(substitute) persister.SetPropertyValues(obj, values);
						
						// search for collections by reachability, updating their role.
						// we don't want to touch collections reachable from a deleted object.
						UpdateReachables(values, types, obj);
					}
					
				}
			}
		}

		/// <summary>
		/// process cascade save/update at the start of a flush to discover
		/// any newly referenced entity that must be passed to saveOrUpdate()
		/// </summary>
		private void PreFlushEntities() 
		{
			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries(entries);

			// so that we can be safe from the enumerator & concurrent modifications
			foreach(DictionaryEntry me in iterSafeCollection) 
			{
				EntityEntry entry = (EntityEntry) me.Value;
				Status status = entry.Status;

				if ( status!=Status.Loading && status!=Status.Gone && status!=Status.Deleted) 
				{
					object obj = me.Key;
					cascading++;
					try 
					{
						Cascades.Cascade(this, entry.Persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate);
					} 
					finally 
					{
						cascading--;
					}
				}
			}
		}

		// this just does a table lookup, but cacheds the last result

		[NonSerialized] private System.Type lastClass;
		[NonSerialized] private IClassPersister lastResultForClass;

		private IClassPersister GetPersister(System.Type theClass) 
		{
			if ( lastClass!=theClass ) 
			{
				lastResultForClass = factory.GetPersister(theClass);
				lastClass = theClass;
			}
			return lastResultForClass;
		}

		public IClassPersister GetPersister(object obj) 
		{
			return GetPersister( obj.GetType() );
		}

		/// <summary>
		/// Not for internal use
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetIdentifier(object obj) 
		{
			if (obj is HibernateProxy) 
			{
				LazyInitializer li = HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj );
				if ( li.Session!=this ) throw new TransientObjectException("The proxy was not associated with this session");
				return li.Identifier;
			} 
			else 
			{
				EntityEntry entry = GetEntry(obj);
				if (entry==null) throw new TransientObjectException("the instance was not associated with this session");
				return entry.Id;
			}
		}

		/// <summary>
		/// Get the id value for an object that is actually associated with the session.
		/// This is a bit stricter than getEntityIdentifierIfNotUnsaved().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetEntityIdentifier(object obj) 
		{
			if (obj is HibernateProxy) 
			{
				return HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj ).Identifier;
			} 
			else 
			{
				EntityEntry entry = GetEntry(obj);
				return (entry!=null) ? entry.Id : null;
			}
		}

		public bool IsSaved(object obj) 
		{
			if(obj is HibernateProxy) return true;

			EntityEntry entry = GetEntry(obj);
			if(entry!=null) return true;

			object isUnsaved = interceptor.IsUnsaved(obj);
			if(isUnsaved!=null) return !(bool)isUnsaved;

			IClassPersister persister = GetPersister(obj);
			if(!persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier) return false; // I _think_ that this is reasonable!

			object id = persister.GetIdentifier(obj);
			return !persister.IsUnsaved(id);
		}

		
		/// <summary>
		/// Used by OneToOneType and ManyToOneType to determine what id value
		/// should be used for an object that may or may not be associated with
		/// the session. This does a "best guess" using any/all info available
		/// to use (not just the EntityEntry).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetEntityIdentifierIfNotUnsaved(object obj) 
		{
			if (obj==null) return null;

			if (obj is HibernateProxy) 
			{
				return HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj ).Identifier;
			} 
			else 
			{
				EntityEntry entry = GetEntry(obj);
				if(entry!=null) 
				{
					return entry.Id;
				}
				else 
				{
					object isUnsaved = interceptor.IsUnsaved(obj);
					
					if(isUnsaved!=null && ((bool)isUnsaved)) 
						ThrowTransientObjectException(obj);
				
					IClassPersister persister = GetPersister(obj);
					if(!persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier)
						ThrowTransientObjectException(obj);

					object id = persister.GetIdentifier(obj);
					if(persister.IsUnsaved(id)) 
						ThrowTransientObjectException(obj);

					return id;
				}

			}
		}

		private static void ThrowTransientObjectException(object obj) 
		{
			throw new TransientObjectException(
				"object references an unsaved transient instance - save the transient instance before flushing: " +
				obj.GetType().Name
				);
		}
		/// <summary>
		/// process any unreferenced collections and then inspect all known collections,
		/// scheduling creates/removes/updates
		/// </summary>
		private void FlushCollections() 
		{

			int unreferencedCount = 0;
			int scheduledCount = 0;
			int recreateCount = 0;
			int removeCount = 0;
			int updateCount = 0;

			log.Debug("Processing unreferenced collections");

			foreach(DictionaryEntry e in IdentityMap.ConcurrentEntries(collections))
			{
				if ( ! ( (CollectionEntry) e.Value ).reached ) 
				{
					UpdateUnreachableCollection( (PersistentCollection) e.Key );
					unreferencedCount++;
				}
					
			}
			log.Debug("Processed " + unreferencedCount + " unreachable collections.");

			// schedule updates to collections:

			log.Debug("scheduling collection removes/(re)creates/updates");

			foreach(DictionaryEntry me in IdentityMap.ConcurrentEntries(collections))
			{
				PersistentCollection coll = (PersistentCollection) me.Key;
				CollectionEntry ce = (CollectionEntry) me.Value;

				// TODO: move this to the entry

				if ( ce.dorecreate ) 
				{
					collectionCreations.Add( new ScheduledCollectionRecreate(coll, ce.currentPersister, ce.currentKey, this) );
					recreateCount++;
				}
				if ( ce.doremove ) 
				{
					collectionRemovals.Add( new ScheduledCollectionRemove(ce.loadedPersister, ce.loadedKey, ce.SnapshotIsEmpty, this) );
					removeCount++;
				}

				if ( ce.doupdate )
				{
					collectionUpdates.Add( new ScheduledCollectionUpdate(coll, ce.loadedPersister, ce.loadedKey, ce.SnapshotIsEmpty, this) );
					updateCount++;
				}

				scheduledCount++;
			}

			log.Debug("Processed " + scheduledCount + " for recreate (" + recreateCount + "), remove (" + removeCount + "), and update (" + updateCount + ")");
		}

		private void PostFlush() 
		{
			log.Debug("post flush");

			foreach(DictionaryEntry de in IdentityMap.ConcurrentEntries(collections)) 
			{
				((CollectionEntry) de.Value).PostFlush( (PersistentCollection) de.Key );
			}

			interceptor.PostFlush( entitiesByKey.Values );
		}

		private void PreFlushCollections() 
		{
			// initialize dirty flags for arrays + collections with composte elements
			// and reset reached, doupdate, etc

			foreach(DictionaryEntry de in IdentityMap.ConcurrentEntries(collections))
			{
				CollectionEntry ce = (CollectionEntry)de.Value;
				PersistentCollection pc = (PersistentCollection)de.Key;

				ce.PreFlush(pc);
			}
		}

		// Wrap all collections in an array of fields with PersistentCollections
		private bool Wrap(object[] fields, IType[] types) 
		{
			bool substitute=false;
			for( int i=0; i<fields.Length; i++) 
			{
				object result = Wrap(fields[i], types[i]);
				if ( result!=fields[i] ) 
				{
					fields[i] = result;
					substitute = true;
				}
			}
			return substitute;
		}

		/// <summary>
		/// If the given object is a collection that can be wrapped by
		/// some subclass of PersistentCollection, wrap it up and
		/// return the wrapper
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to wrap.</param>
		/// <param name="type">The <see cref="IType"/> of the Property the obj came in for.</param>
		/// <returns>
		/// An <see cref="Object"/> that NHibernate has now been made aware of.
		/// </returns>
		private object Wrap(object obj, IType type) {

			if (obj==null) return null;

			if ( type.IsComponentType ) 
			{
				IAbstractComponentType componentType = (IAbstractComponentType) type;
				object[] values = componentType.GetPropertyValues(obj, this);
				if ( Wrap( values, componentType.Subtypes ) ) componentType.SetPropertyValues(obj, values);
			}

			if ( type.IsPersistentCollectionType ) 
			{
				// a previously wrapped collection from another session has come back
				// into NHibernate 
				if ( obj is PersistentCollection ) 
				{
					PersistentCollection pc = (PersistentCollection) obj;
					if ( pc.SetSession(this) ) 
					{
						ICollectionSnapshot snapshot = pc.CollectionSnapshot;
						if(snapshot.IsInitialized) 
						{
							AddNewCollection(pc); 
						}
						else 
						{
							object id = snapshot.Key;
							if(id==null) throw new HibernateException("reference created to previously dereferenced uninitialized collection");
							AddUninitializedCollection(pc, GetCollectionPersister(snapshot.Role), id);
							pc.ForceLoad();
							// commented out in h2.0.3 also
							// ugly & inefficient little hack to force collection to be recreated
							// after "disconnected" collection replaces the "connected" one
							//GetCollectionEntry(pc).loadedKey = null;
							//GetCollectionEntry(pc).loadedPersister = null;
							AddNewCollection(pc);
							
						}	
					}
				} 

				// used to be code here to see if the obj.GetType().IsArray but don't want it
				// here because in .net an array is an IList - not true in java so NHibernate can
				// have an array passed to a type that is a List.

				// this is a new collection that NHibernate is not aware of.
				else 
				{
					// if the Type of the collection is an ArrayType then we need to add it 
					// to an ArrayHolder - TODO: figure out if this is really necessary since
					// Java and .NET treat arrays differently.
					if( type is ArrayType ) 
					{
						ArrayHolder ah = GetArrayHolder( obj );
						if( ah==null ) 
						{
							ah = new ArrayHolder( this, obj );
							AddNewCollection( ah );
							AddArrayHolder( ah );
						}
					}
					else 
					{
						PersistentCollection pc = ((PersistentCollectionType) type).Wrap(this, obj);
						if ( log.IsDebugEnabled ) log.Debug( "Wrapped collection in role: " + ((PersistentCollectionType) type).Role);
						AddNewCollection(pc);
						obj = pc;
					}
				}
			}

			return obj;
		}

		/// <summary>
		/// Initialize the role of the collection.
		/// The CollectionEntry.reached stuff is just to detect any silly users who set up
		/// circular or shared references between/to collections.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="type"></param>
		/// <param name="owner"></param>
		private void UpdateReachableCollection(PersistentCollection coll, IType type, object owner) 
		{
			CollectionEntry ce = GetCollectionEntry(coll);

			if (ce == null)
				throw new HibernateException("found reference to object that is not in session");

			if (ce.reached) 
			{
				// we've been here before
				throw new HibernateException("found shared references to a collection");
			}
			ce.reached = true;

			CollectionPersister persister = GetCollectionPersister( ((PersistentCollectionType)type).Role );
			ce.currentPersister = persister;
			ce.currentKey = GetEntityIdentifier(owner);

			if ( log.IsDebugEnabled ) 
			{
				log.Debug (
					"Collection found: " + MessageHelper.InfoString(persister, ce.currentKey) +
					", was: " +MessageHelper.InfoString(ce.loadedPersister, ce.loadedKey)
					);
			}

			PrepareCollectionForUpdate(coll, ce);
		}

		/// <summary>
		/// Given a reachable object, decide if it is a collection or a component holding collections.
		/// If so, recursively update contained collections
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <param name="owner"></param>
		private void UpdateReachable(object obj, IType type, object owner) 
		{
			// this method assmues wrap was already called on obj!

			if ( obj!=null ) 
			{
				if ( type.IsPersistentCollectionType ) 
				{
					if ( obj.GetType().IsArray ) 
					{
						UpdateReachableCollection( GetArrayHolder(obj), type, owner );
					} 
					else 
					{
						UpdateReachableCollection( (PersistentCollection) obj, type, owner );
					}
				}

				else if ( type.IsComponentType ) 
				{
					IAbstractComponentType componentType = (IAbstractComponentType) type;
					object[] values = componentType.GetPropertyValues(obj, this);
					IType[] types = componentType.Subtypes;
					if ( Wrap(values, types) ) componentType.SetPropertyValues(obj, values);
					UpdateReachables(values, types, owner);
				}
			}
		}

		/// <summary>
		/// record the fact that this collection was dereferenced
		/// </summary>
		/// <param name="coll"></param>
		private void UpdateUnreachableCollection(PersistentCollection coll) 
		{
			CollectionEntry entry = GetCollectionEntry(coll);
			
			if ( log.IsDebugEnabled && entry.loadedPersister!=null ) 
			{
				log.Debug("collection dereferenced: " + MessageHelper.InfoString(entry.loadedPersister, entry.loadedKey));
			}

			if(entry.loadedPersister!=null && entry.loadedPersister.HasOrphanDelete) 
			{
				EntityEntry e = GetEntry( new Key(entry.loadedKey, GetPersister(entry.loadedPersister.OwnerClass) ) );

				// only collections belonging to deleted entities are allowed to be dereferenced in 
				// the case of orphan delete
				if( e!=null && e.Status!=Status.Deleted && e.Status!=Status.Gone) 
				{
					throw new HibernateException("You may not dereference an collection with cascade=\"all-delete-orphan\"");
				}
			}
		
			entry.currentPersister=null;
			entry.currentKey=null;

			PrepareCollectionForUpdate(coll, entry);
		}

		/// <summary>
		/// 1. record the collection role that this collection is referenced by
		/// 2. decide if the collection needs deleting/creating/updating (but
		///    don't actually schedule the action yet)
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="entry"></param>
		private void PrepareCollectionForUpdate(PersistentCollection coll, CollectionEntry entry) {

			if ( entry.processed ) throw new AssertionFailure("hibernate has a bug processing collections");

			entry.processed = true;

			// it is or was referenced _somewhere_
			if ( entry.loadedPersister!=null || entry.currentPersister!=null ) { 

				if (
					entry.loadedPersister!=entry.currentPersister || //if either its role changed,
					!entry.currentPersister.KeyType.Equals(entry.loadedKey, entry.currentKey) // or its key changed
					) {

					// do a check
					if( entry.loadedPersister!=null && entry.currentPersister!=null && entry.loadedPersister.HasOrphanDelete ) 
					{
						throw new HibernateException("You may not change the reference to a collection with cascade=\"all-delete-orphan\"");
					}

					// do the work
					if (entry.currentPersister!=null) 
					{
						entry.dorecreate = true; //we will need to create new entry
					}

					if (entry.loadedPersister!=null)
					{
						entry.doremove = true; // we will need to remove the old entres
						if (entry.dorecreate ) 
						{
							log.Debug("forcing collection initialization");
							coll.ForceLoad();
						}
					}
				} 
				else if (entry.dirty ) { // else if it's elements changed
					entry.doupdate = true;
				}
			}
		}

		/// <summary>
		/// Given an array of fields, search recursively for collections and update them
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="types"></param>
		/// <param name="owner"></param>
		/// <remarks>This method assume Wrap was already called on fields.</remarks>
		private void UpdateReachables(object[] fields, IType[] types, object owner) 
		{
			// this method assumes wrap was already called on fields

			for (int i=0; i<types.Length; i++ ) 
			{
				UpdateReachable(fields[i], types[i], owner);
			}
		}

		/// <summary>
		/// ONLY near the end of the flush process, determine if the collection is dirty
		/// by checking its entry
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		private bool CollectionIsDirty(PersistentCollection coll) 
		{ 
			CollectionEntry entry = GetCollectionEntry(coll); 
			return entry.initialized && entry.dirty; //( entry.dirty || coll.hasQueuedAdds() ); 
		} 

		/// <summary>
		/// Given an array of fields, search recursively for dirty collections. 
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="types"></param>
		/// <returns>return true if we find one</returns>
		private bool SearchForDirtyCollections(object[] fields, IType[] types) 
		{
			for (int i=0; i<types.Length; i++ ) 
			{
				if ( SearchForDirtyCollections( fields[i], types[i] ) ) return true;
			}
			return false;
		}

		/// <summary>
		/// Do we have a dirty collection here?
		/// 1. if it is a new application-instantiated collection, return true (does not occur anymore!)
		/// 2. if it is a component, recurse
		/// 3. if it is a wrappered collection, ask the collection entry
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool SearchForDirtyCollections(object obj, IType type) 
		{
			if ( obj!=null ) 
			{				
				if ( type.IsPersistentCollectionType ) 
				{
					if ( obj.GetType().IsArray ) 
					{
						PersistentCollection ah = GetArrayHolder(obj);
						// if no array holder we found an unwrappered array (this can't occur, 
						//if no array holder we found an unwrappered array                                         // because we now always call wrap() before getting to here) 
						//return (ah==null) ? true : SearchForDirtyCollections(ah, type);
						return CollectionIsDirty(ah);
					} 
					else 
					{
						// if not wrappered yet, its dirty (this can't occur, because 
						// we now always call wrap() before getting to here) 
						// return ( ! (obj is PersistentCollection) ) ?
						//	true : SearchForDirtyCollections( (PersistentCollection) obj, type );
						return CollectionIsDirty( (PersistentCollection) obj );
					}
				}

				else if ( type.IsComponentType ) 
				{
					IAbstractComponentType componentType = (IAbstractComponentType) type;
					object[] values = componentType.GetPropertyValues(obj, this);
					IType[] types = componentType.Subtypes;
					for (int i=0; i<values.Length; i++) 
					{
						if ( SearchForDirtyCollections( values[i], types[i] ) ) return true;
					}
				}
			}
			return false;
		}

		private IDictionary loadingCollections = new Hashtable();
		private string loadingRole;

		private sealed class LoadingCollectionEntry 
		{
			private PersistentCollection _collection;
			private bool _initialize;
			private object _id;
			private object _owner;

			internal LoadingCollectionEntry(PersistentCollection collection, object id) 
			{
				_collection = collection;
				_id = id;
			}

			internal LoadingCollectionEntry(PersistentCollection collection, object id, object owner) 
			{
				_collection = collection;
				_id = id;
				_owner = owner;
			}

			public PersistentCollection Collection 
			{
				get { return _collection;}
				set { _collection = value; }
			}

			public object Id 
			{
				get { return _id; }
				set { _id = value; }
			}

			
			public object Owner 
			{
				get { return _owner; }
				set { _owner = value; }
			}
			
			public bool Initialize
			{
				get { return _initialize; }
				set { _initialize = value; }
			}
		}


		// TODO: replace with owner version of this method...
		[Obsolete("Use the one with CollectionPersister, id, owner) instead")]
		public PersistentCollection GetLoadingCollection(CollectionPersister persister, object id) 
		{
			LoadingCollectionEntry lce = (LoadingCollectionEntry)loadingCollections[id];
			if(lce==null) 
			{
				PersistentCollection pc = persister.CollectionType.Instantiate(this, persister);
				pc.BeforeInitialize(persister);
				pc.BeginRead();
				if(loadingRole!=null && !loadingRole.Equals(persister.Role)) 
				{
					throw new AssertionFailure("recursive collection load");
				}

				loadingCollections[id] = new LoadingCollectionEntry(pc, id);
				loadingRole = persister.Role;
				return pc;
			}
			else 
			{
				return lce.Collection;
			}
		}

		//NEW overloaded version that should replace the 2 object without owner
		public PersistentCollection GetLoadingCollection(CollectionPersister persister, object id, object owner) 
		{
			LoadingCollectionEntry lce = (LoadingCollectionEntry)loadingCollections[id];
			if(lce==null) 
			{
				PersistentCollection pc = persister.CollectionType.Instantiate(this, persister);
				pc.BeforeInitialize(persister);
				pc.BeginRead();
				if(loadingRole!=null && !loadingRole.Equals(persister.Role)) 
				{
					throw new AssertionFailure("recursive collection load");
				}

				loadingCollections[id]= new LoadingCollectionEntry(pc, id, owner);
				loadingRole = persister.Role;
				return pc;
			}
			else 
			{
				return lce.Collection;
			}
		}

		public void EndLoadingCollections() 
		{
			if(loadingRole!=null) 
			{
				CollectionPersister persister = GetCollectionPersister(loadingRole);
				foreach (LoadingCollectionEntry lce in loadingCollections.Values) 
				{
					if(lce.Initialize) 
					{
						lce.Collection.EndRead(persister, lce.Owner);
						AddInitializedCollection(lce.Collection, persister, lce.Id);
						persister.Cache(lce.Id, lce.Collection, this);
					}
				}

				loadingCollections.Clear();
				loadingRole = null;
			}
		}

		public PersistentCollection GetLoadingCollection(string role, object id) 
		{
			if(role.Equals(loadingRole)) 
			{
				LoadingCollectionEntry lce = (LoadingCollectionEntry) loadingCollections[id];
				if(lce==null) 
				{
					return null;
				}
				else 
				{
					lce.Initialize = true;
					return lce.Collection;
				}
			}
			else 
			{
				return null;
			}
		}

		/// <summary>
		/// add a collection we just loaded up (still needs initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		public void AddUninitializedCollection(PersistentCollection collection, CollectionPersister persister, object id) 
		{
			CollectionEntry ce = new CollectionEntry(persister, id, false);
			collections[collection] = ce;
			collection.CollectionSnapshot = ce;
		}

		/// <summary>
		/// add a collection we just pulled out of the cache (does not need initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		public void AddInitializedCollection(PersistentCollection collection, CollectionPersister persister, object id) 
		{
			CollectionEntry ce = new CollectionEntry(persister, id, true);
			ce.PostInitialize(collection);
			collections[collection] = ce;
			collection.CollectionSnapshot = ce;
		}

		/// <summary>
		/// add an (initialized) collection that was created by another session and passed into update()
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="cs"></param>
		private void AddInitializedCollection(PersistentCollection collection, ICollectionSnapshot cs) 
		{
			CollectionEntry ce = new CollectionEntry(cs, factory);
			collections[collection] = ce;
			collection.CollectionSnapshot = ce;
		}

		public ArrayHolder GetArrayHolder(object array) 
		{
			return (ArrayHolder) arrayHolders[array];
		}

		//must call after loading array (so array exists for key of map);
		public void AddArrayHolder(ArrayHolder holder) 
		{
			arrayHolders[holder.Array] = holder;
		}

		private CollectionPersister GetCollectionPersister(string role) 
		{
			return factory.GetCollectionPersister(role);
		}

		public void Dirty(PersistentCollection coll) 
		{
			GetCollectionEntry(coll).dirty = true;
		}

		public object GetSnapshot(PersistentCollection coll) 
		{
			return GetCollectionEntry(coll).snapshot;
		}

		public object GetLoadedCollectionKey(PersistentCollection coll) 
		{
			return GetCollectionEntry(coll).loadedKey;
		}

		public bool IsInverseCollection(PersistentCollection collection) 
		{
			CollectionEntry ce = GetCollectionEntry(collection);
			return ce!=null && ce.loadedPersister.IsInverse;
		}

		
		private static readonly ICollection EmptyCollection = new ArrayList(0);

		public ICollection GetOrphans(PersistentCollection coll) 
		{
			CollectionEntry ce = GetCollectionEntry(coll);
			return ce.IsNew ? EmptyCollection : coll.GetOrphans(ce.Snapshot);
		}
		/// <summary>
		/// called by a collection that wants to initialize itself
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		public void Initialize(PersistentCollection collection, bool writing) 
		{
			CollectionEntry ce = GetCollectionEntry(collection);

			if ( !ce.initialized ) 
			{
				if (log.IsDebugEnabled ) 
				{
					log.Debug( "initializing collection " + MessageHelper.InfoString(ce.loadedPersister, ce.loadedKey) );
				}

				CollectionPersister persister = ce.loadedPersister;
				object id = ce.loadedKey;

				object owner = GetEntity( new Key( id, GetPersister( persister.OwnerClass ) ) );

				collection.BeforeInitialize(persister);
				try 
				{
					persister.Initializer.Initialize(id, collection, owner, this);
				}
				catch(ADOException sqle) 
				{
					throw new ADOException("SQLException initializing collection", sqle);
				}

				ce.initialized = true;
				ce.PostInitialize(collection);

				//removed this because it is not in h2.1 - I was having problems with Bags and 
				// lazy additions and trying to cache uninitialized collections at this point.
				// Collections are still written to the Cache in EndLoadingCollection and that 
				// is probably the most appropriate place for that code anyway.
//				if (!writing) persister.Cache(id, collection, this);
			}
		}

		public IDbConnection Connection 
		{
			get 
			{
				if (connection==null) 
				{
					if (connect) 
					{
						connection = factory.OpenConnection();
						connect = false;
					} 
					else 
					{
						throw new HibernateException("session is currently disconnected");
					}
				}
				return connection;
			}
		}

		public bool IsConnected 
		{
			get { return connection!=null || connect; }
		}

		public IDbConnection Disconnect() 
		{
			log.Debug("disconnecting session");

			try 
			{ 
				if (connect) 
				{
					connect = false;
					return null;
				} 
				else 
				{
					if (connection==null) throw new HibernateException("session already disconnected");

					if (batcher!=null) batcher.CloseCommands();
					IDbConnection c = connection;
					connection=null;
					if (autoClose) 
					{
						factory.CloseConnection(c);
						return null;
					} 
					else 
					{
						return c;
					}
				}
			} 
			finally 
			{
				if ( callAfterTransactionCompletionFromDisconnect ) 
				{
					AfterTransactionCompletion();
				}
			}
		}

		public void Reconnect() 
		{
			if ( IsConnected ) throw new HibernateException("session already connected");

			log.Debug("reconnecting session");

			connect = true;
		}

		public void Reconnect(IDbConnection conn) 
		{
			if ( IsConnected ) throw new HibernateException("session already connected");
			this.connection = conn;
		}

		#region System.IDisposable Members

		/// <summary>
		/// Just in case the user forgot to Commit() or Close()
		/// </summary>
		void IDisposable.Dispose() 
		{
			log.Debug("running ISession.Dispose()");

			// it was never disconnected
			if (connection!=null) 
			{
				AfterTransactionCompletion();

				if ( connection.State == ConnectionState.Closed ) 
				{
					log.Warn("finalizing unclosed session with closed connection");
				} 
				else 
				{
					log.Warn("unclosed connection");
					if (autoClose) connection.Close();
				}
			}
		}

		#endregion

		public ICollection Filter(object collection, string filter) 
		{
			QueryParameters qp = new QueryParameters( new IType[1], new object[1] );
			return Filter( collection, filter, qp );
		}

		public ICollection Filter(object collection, string filter, object value, IType type) 
		{
			QueryParameters qp = new QueryParameters( new IType[] { null, type }, new object[] { null, value } );
			return Filter( collection, filter, qp );
		}

		public ICollection Filter(object collection, string filter, object[] values, IType[] types) 
		{
			object[] vals = new object[ values.Length + 1 ];
			IType[] typs = new IType[ values.Length + 1 ];
			Array.Copy( values, 0, vals, 1, values.Length );
			Array.Copy( types, 0, typs, 1, types.Length );
			QueryParameters qp = new QueryParameters( typs, vals );
			return Filter( collection, filter, qp );
		}

		/// <summary>
		/// 1. determine the collection role of the given collection (this may require a flush, if the collection is recorded as unreferenced)
		/// 2. obtain a compiled filter query
		/// 3. autoflush if necessary
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		private FilterTranslator GetFilterTranslator(object collection, string filter, QueryParameters parameters, bool scalar) 
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "filter: " + filter );
				parameters.LogParameters();
			}

			if ( !(collection is PersistentCollection) ) 
			{
				collection = GetArrayHolder(collection);
				if (collection==null) throw new TransientObjectException("collection was not yet persistent");
			}

			PersistentCollection coll = (PersistentCollection) collection;
			CollectionEntry e = GetCollectionEntry(coll);
			if (e==null) throw new TransientObjectException("collection was not persistent in this session");

			FilterTranslator q;
			CollectionPersister roleBeforeFlush = e.loadedPersister;
			if ( roleBeforeFlush==null ) 
			{ //ie. it was previously unreferenced
				Flush();
				if ( e.loadedPersister==null ) throw new QueryException("the collection was unreferenced");
				q = factory.GetFilter( filter, e.loadedPersister.Role, scalar);
			} 
			else 
			{
				q = factory.GetFilter( filter, roleBeforeFlush.Role, scalar );
				if ( AutoFlushIfRequired( q.QuerySpaces ) && roleBeforeFlush!=e.loadedPersister ) 
				{
					if ( e.loadedPersister==null ) throw new QueryException("the collection was dereferenced");
					// might need to recompile the query after the flush because the collection role may have changed.
					q = factory.GetFilter( filter, e.loadedPersister.Role, scalar);
				}
			}
			
			parameters.PositionalParameterValues[0] = e.loadedKey;
			parameters.PositionalParameterTypes[0] = e.loadedPersister.KeyType;

			return q;

		}

		public IList Filter(object collection, string filter, QueryParameters parameters) 
		{
			string[] concreteFilters = QueryTranslator.ConcreteQueries(filter, factory);
			FilterTranslator[] filters = new FilterTranslator[ concreteFilters.Length ];

			for ( int i=0; i<concreteFilters.Length; i++ ) 
			{
				filters[i] = GetFilterTranslator(
					collection, 
					concreteFilters[i], 
					parameters, 
					false );
					
			}

			dontFlushFromFind++; // stops flush being called multiple times if this method is recursively called

			IList results = new ArrayList();
			try 
			{
				for (int i=0; i<concreteFilters.Length; i++ ) 
				{
					IList currentResults;
					try 
					{
						currentResults = filters[i].FindList( this, parameters, true );
					} 
					catch (Exception e) 
					{
						throw new ADOException("could not execute query", e);
					}
					foreach(object res in results) 
					{
						currentResults.Add(res);
					}
					results = currentResults;
				}
			} 
			finally 
			{
				dontFlushFromFind--;
			}
			return results;
		}

		public IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters) 
		{
			string[] concreteFilters = QueryTranslator.ConcreteQueries(filter, factory);
			FilterTranslator[] filters = new FilterTranslator[ concreteFilters.Length ];

			for (int i=0; i<concreteFilters.Length; i++ ) 
			{
				filters[i] = GetFilterTranslator(
					collection, 
					concreteFilters[i], 
					parameters,
					true);
			}

			if (filters.Length==0) return new ArrayList(0);

			IEnumerable result = null;
			IEnumerable[] results = null;
			bool many = filters.Length>1;
			if (many) results = new IEnumerable[filters.Length];

			// execute the queries and return all results as a single enumerable
			for (int i=0; i<filters.Length; i++ ) 
			{
				try 
				{ 
					result = filters[i].GetEnumerable( parameters, this );
				} 
				catch (Exception e) 
				{
					throw new ADOException("could not execute query", e);
				}
				if (many) 
				{
					results[i] = result;
				}
			}

			return many ? new JoinedEnumerable(results) : result;
		}

		public ICriteria CreateCriteria(System.Type persistentClass) 
		{
			return new CriteriaImpl(persistentClass, this);
		}

		public IList Find(CriteriaImpl criteria) 
		{
			System.Type persistentClass = criteria.PersistentClass;

			if ( log.IsDebugEnabled ) 
			{
				log.Debug( "search: " + persistentClass.Name );
				log.Debug( "criteria: " + criteria );
			}

			ILoadable persister = (ILoadable) GetPersister(persistentClass);
			CriteriaLoader loader = new CriteriaLoader(persister, factory, criteria);
			object[] spaces = persister.PropertySpaces;
			ArrayList sett = new ArrayList();
			for (int i=0; i<spaces.Length; i++) 
			{
				sett.Add( spaces[i] );
			}
			AutoFlushIfRequired(sett);

			dontFlushFromFind++;
			try 
			{
				return loader.List(this);
			} 
			catch (Exception e) 
			{
				throw new ADOException("problem in find", e);
			} 
			finally 
			{
				dontFlushFromFind--;
			}
		}

		public bool Contains(object obj) 
		{ 
			if (obj is HibernateProxy) 
			{ 
				return HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj ).Session==this; 
			} 
			else 
			{ 
				return entries.Contains(obj); 
			}
		}
    
       /// <summary>
		/// remove any hard references to the entity that are held by the infrastructure
		/// (references held by application or other persistant instances are okay)
		/// </summary>
		/// <param name="obj"></param>
        public void Evict(object obj) 
		{ 
            if (obj is HibernateProxy) 
			{ 
                LazyInitializer li = HibernateProxyHelper.GetLazyInitializer( (HibernateProxy) obj ); 
                object id = li.Identifier; 
                IClassPersister persister = GetPersister( li.PersistentClass ); 
                Key key = new Key(id, persister); 
                proxiesByKey.Remove(key); 
                if ( !li.IsUninitialized ) 
				{ 
                    object entity = RemoveEntity(key); 
                    if (entity!=null) 
					{ 
                        RemoveEntry(entity); 
                        DoEvict(persister, entity); 
                    } 
                } 
            } 
            else 
			{ 
                EntityEntry e = (EntityEntry) RemoveEntry(obj); 
                if (e!=null) 
				{ 
                    RemoveEntity( new Key(e.Id, e.Persister) ); 
                    DoEvict(e.Persister, obj); 
                } 
            } 
        } 

        private void DoEvict(IClassPersister persister, object obj) 
		{ 
            if ( log.IsDebugEnabled ) log.Debug( "evicting " + MessageHelper.InfoString(persister) ); 

            //remove all collections for the entity 
            EvictCollections( persister.GetPropertyValues(obj), persister.PropertyTypes ); 
            Cascades.Cascade(this, persister, obj, Cascades.CascadingAction.ActionEvict, CascadePoint.CascadeOnEvict); 
        } 
    
          
		/// <summary>
		/// Evict any collections referenced by the object from the session cache. This will NOT 
		/// pick up any collections that were dereferenced, so they will be deleted (suboptimal 
		/// but not exactly incorrect). 
		/// </summary>
		/// <param name="values"></param>
		/// <param name="types"></param>
		private void EvictCollections(Object[] values, IType[] types) 
		{ 
			for ( int i=0; i<types.Length; i++ ) 
			{ 
				if( values[i]==null) 
				{
					// do nothing
				}
				else if ( types[i].IsPersistentCollectionType ) 
				{ 
					object pc=null; 
					if ( ( (PersistentCollectionType) types[i] ).IsArrayType ) 
					{ 
						pc = arrayHolders[ values[i] ];
						arrayHolders.Remove( values[i] ); 
					} 
					else if ( values[i] is PersistentCollection ) 
					{ 
						pc = values[i]; 
					} 

					if (pc!=null) 
					{ 
						if ( ( (PersistentCollection) pc ).UnsetSession(this) ) collections.Remove(pc); 
					} 
				} 
				else if ( types[i].IsComponentType ) 
				{ 
					IAbstractComponentType actype = (IAbstractComponentType) types[i]; 
					EvictCollections( 
						actype.GetPropertyValues( values[i], this ), 
						actype.Subtypes 
					); 
				} 
			} 
        } 
	
		public object GetVersion(object entity) 
		{
			return GetEntry(entity).Version;
		}
	}
}
