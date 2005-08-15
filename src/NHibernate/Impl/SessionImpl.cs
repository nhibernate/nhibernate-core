using System;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
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
		private static readonly ILog log = LogManager.GetLogger( typeof( SessionImpl ) );

		private SessionFactoryImpl factory;

		private readonly bool autoClose;
		private readonly long timestamp;

		/// <summary>
		/// Indicates if the Session has been closed.
		/// </summary>
		/// <value>
		/// <c>false</c> (by default) if the Session is Open and can be used, 
		/// <c>true</c> if the Session has had the methods <c>Close()</c> or
		/// <c>Dispose()</c> invoked.</value>
		private bool closed = false;
		private FlushMode flushMode = FlushMode.Auto;

		private bool isCurrentTransaction;		// a bit dodgy!

		/// <summary>
		/// An <see cref="IDictionary"/> with the <see cref="Key"/> as the key
		/// and an <see cref="Object"/> as the value.
		/// </summary>
		private readonly IDictionary entitiesByKey; 

		/// <summary>
		/// An <see cref="IDictionary"/> with the <see cref="Key"/> as the key
		/// and an <see cref="INHibernateProxy"/> as the value.
		/// </summary>
		private IDictionary proxiesByKey; 

		// these are used to serialize the proxiesByKey Dictionary - I was not able to
		// have a Hashtable serialize fully by the time that SessionImpl OnDeserialization
		// was getting called - I think I'm not completely understanding the sequence of 
		// deserialization events.  When SessionImpl was getting the OnDeserialization called
		// the proxies were not fully deserialized. 
		private ArrayList tmpProxiesKey;
		private ArrayList tmpProxiesProxy;

		//IdentityMaps are serializable in NH 
		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="Object"/> as the key
		/// and an <see cref="EntityEntry"/> as the value.
		/// </summary>
		private IdentityMap entityEntries; 
		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="Array"/> as the key
		/// and an <see cref="ArrayHolder"/> as the value.
		/// </summary>
		private IdentityMap arrayHolders; 
		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="PersistentCollection"/> as the key
		/// and an <see cref="CollectionEntry"/> as the value.
		/// </summary>
		private IdentityMap collectionEntries;

		/// <summary>
		/// An <see cref="IdentityMap"/> with the <see cref="CollectionKey"/> as the key
		/// and an <see cref="PersistentCollection"/> as the value.
		/// </summary>
		private readonly IDictionary collectionsByKey; 

		/// <summary>
		/// An <see cref="ISet"/> of <see cref="Key"/> objects of the deleted entities.
		/// </summary>
		private ISet nullifiables = new HashedSet(); 

		private readonly ISet nonExists;

		private IInterceptor interceptor;

		[NonSerialized]
		private IDbConnection connection;

		/// <summary>
		/// A boolean indicating if the Session should automattically connect to the
		/// database - ie, open a new <see cref="IDbConnection"/> for the operation if
		/// <c>this.Connection==null</c>.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This will be initialzed to <c>true</c> by the ctor if NHibernate is managing
		/// the Connections, <c>false</c> if the user passes in their own connection - 
		/// indicating they will be responsible for managing connections.
		/// </p>
		/// <p>
		/// This can also be set to <c>false</c> when NHibernate has opened the connection
		/// on its own and the Session has had the methods <c>Close()</c> or 
		/// <c>Disconnect()</c> invoked.
		/// </p>
		/// <p>
		/// This can also be set to <c>true</c> when the Session has had the method 
		/// <c>Reconnect()</c> invoked.
		/// </p>
		/// </remarks>
		[NonSerialized]
		private bool connect;

		[NonSerialized]
		private ITransaction transaction;

		// We keep scheduled insertions, deletions and updates in collections
		// and actually execute them as part of the flush() process. Actually,
		// not every flush() ends in execution of the scheduled actions. Auto-
		// flushes initiated by a query execution might be "shortcircuited".

		// Object insertions and deletions have list semantics because they
		// must happen in the right order so as to respect referential integrity
		[NonSerialized]
		private ArrayList insertions;

		[NonSerialized]
		private ArrayList deletions;

		// updates are kept in a Map because successive flushes might need to add
		// extra, new changes for an object thats already scheduled for update.
		// Note: we *could* treat updates the same way we treat collection actions
		// (discarding them at the end of a "shortcircuited" auto-flush) and then
		// we would keep them in a list
		[NonSerialized]
		private ArrayList updates;

		// Actually the semantics of the next three are really "Bag"
		// Note that, unlike objects, collection insertions, updates,
		// deletions are not really remembered between flushes. We
		// just re-use the same Lists for convenience.
		[NonSerialized]
		private ArrayList collectionCreations;

		[NonSerialized]
		private ArrayList collectionUpdates;

		[NonSerialized]
		private ArrayList collectionRemovals;

		[NonSerialized]
		private ArrayList executions;

		[NonSerialized]
		private int dontFlushFromFind = 0;

		[NonSerialized]
		private int cascading = 0;

		[NonSerialized]
		private int loadCounter = 0;

		[NonSerialized]
		private bool flushing;

		[NonSerialized]
		private IBatcher batcher;

		[NonSerialized]
		private IList nonlazyCollections;

		[NonSerialized]
		private IDictionary batchLoadableEntityKeys; // actually, a Set
		private static readonly object Marker = new object();

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
		protected SessionImpl( SerializationInfo info, StreamingContext context )
		{
			this.autoClose = info.GetBoolean( "autoClose" );
			this.timestamp = info.GetInt64( "timestamp" );

			this.factory = ( SessionFactoryImpl ) info.GetValue( "factory", typeof( SessionFactoryImpl ) );

			this.entitiesByKey = ( IDictionary ) info.GetValue( "entitiesByKey", typeof( IDictionary ) );
			// we did not actually serializing the IDictionary but instead the proxies in an arraylist
			//this.proxiesByKey = (IDictionary)info.GetValue( "proxiesByKey", typeof(IDictionary) );
			tmpProxiesKey = ( ArrayList ) info.GetValue( "tmpProxiesKey", typeof( ArrayList ) );
			tmpProxiesProxy = ( ArrayList ) info.GetValue( "tmpProxiesProxy", typeof( ArrayList ) );
			this.entityEntries = ( IdentityMap ) info.GetValue( "entityEntries", typeof( IdentityMap ) );
			this.collectionEntries = ( IdentityMap ) info.GetValue( "collectionEntries", typeof( IdentityMap ) );
			this.collectionsByKey = ( IDictionary ) info.GetValue( "collectionsByKey", typeof( IDictionary ) );
			this.arrayHolders = ( IdentityMap ) info.GetValue( "arrayHolders", typeof( IdentityMap ) );
			this.nonExists = ( ISet) info.GetValue( "nonExists", typeof( ISet ) );

			this.closed = info.GetBoolean( "closed" );
			this.flushMode = ( FlushMode ) info.GetValue( "flushMode", typeof( FlushMode ) );
			this.isCurrentTransaction = info.GetBoolean( "isCurrentTransaction" );

			this.nullifiables = ( ISet ) info.GetValue( "nullifiables", typeof( ISet ) );
			this.interceptor = ( IInterceptor ) info.GetValue( "interceptor", typeof( IInterceptor ) );
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
		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			log.Info( "writting session to serializer" );

			if( IsConnected )
			{
				throw new InvalidOperationException( "Cannot serialize a Session while connected" );
			}

			if( insertions.Count != 0 || deletions.Count != 0 )
			{
				throw new InvalidOperationException( "Cannot serialize a Session which has work waiting to be flushed" );
			}

			info.AddValue( "factory", factory, typeof( SessionFactoryImpl ) );
			info.AddValue( "autoClose", autoClose );
			info.AddValue( "timestamp", timestamp );
			info.AddValue( "closed", closed );
			info.AddValue( "flushMode", flushMode );
			info.AddValue( "isCurrentTransaction", isCurrentTransaction );
			info.AddValue( "entitiesByKey", entitiesByKey, typeof( IDictionary ) );

			// the IDictionary should not be serialized because the objects inside of it are not
			// fully deserialized until after the session is deserialized. Instead use two ArrayList 
			// to hold the values because they don't have the deserialization complexities that
			// hashtables do.
			tmpProxiesKey = new ArrayList( proxiesByKey.Count );
			tmpProxiesProxy = new ArrayList( proxiesByKey.Count );
			foreach( DictionaryEntry de in proxiesByKey )
			{
				tmpProxiesKey.Add( de.Key );
				tmpProxiesProxy.Add( de.Value );
			}

			info.AddValue( "tmpProxiesKey", tmpProxiesKey );
			info.AddValue( "tmpProxiesProxy", tmpProxiesProxy );

			info.AddValue( "nullifiables", nullifiables, typeof( ISet ) );
			info.AddValue( "interceptor", interceptor, typeof( IInterceptor ) );

			info.AddValue( "entityEntries", entityEntries, typeof( IdentityMap ) );
			info.AddValue( "collectionEntries", collectionEntries, typeof( IdentityMap ) );
			info.AddValue( "collectionsByKey", collectionsByKey, typeof( IDictionary ) );
			info.AddValue( "arrayHolders", arrayHolders, typeof( IdentityMap ) );
			info.AddValue( "nonExists", nonExists, typeof( ISet ) );
		}

		#endregion

		#region System.Runtime.Serialization.IDeserializationCallback Members 

		/// <summary>
		/// Once the entire object graph has been deserialized then we can hook the
		/// collections, proxies, and entities back up to the ISession.
		/// </summary>
		/// <param name="sender"></param>
		void IDeserializationCallback.OnDeserialization( Object sender )
		{
			log.Info( "OnDeserialization of the session." );

			// don't need any section for IdentityMaps because .net does not have a problem
			// serializing them like java does.

			InitTransientState();

			// we need to reconnect all proxies and collections to this session
			// the association is transient because serialization is used for
			// different things.

			foreach( DictionaryEntry e in collectionEntries )
			{
				try
				{
					( ( PersistentCollection ) e.Key ).SetCurrentSession( this );
					CollectionEntry ce = ( CollectionEntry ) e.Value;
					if (ce.Role != null)
						ce.SetLoadedPersister( factory.GetCollectionPersister( ce.Role ) );
				}
				catch( HibernateException he )
				{
					// Different from h2.0.3
					throw new InvalidOperationException( he.Message );
				}
			}

			// recreate the proxiesByKey hashtable from the two arraylists.
			proxiesByKey = new Hashtable( tmpProxiesKey.Count );
			for( int i = 0; i < tmpProxiesKey.Count; i++ )
			{
				proxiesByKey.Add( tmpProxiesKey[ i ], tmpProxiesProxy[ i ] );
			}

			// we can't remove an entry from an IDictionary while enumerating so store the ones
			// to remove in this list
			ArrayList keysToRemove = new ArrayList();

			foreach( DictionaryEntry de in proxiesByKey )
			{
				object key = de.Key;
				object proxy = de.Value;

				if( proxy is INHibernateProxy )
				{
					NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) proxy ).Session = this;
				}
				else
				{
					// the proxy was pruned during the serialization process because
					// the target had been instantiated.
					keysToRemove.Add( key );
				}
			}

			for( int i = 0; i < keysToRemove.Count; i++ )
			{
				proxiesByKey.Remove( keysToRemove[ i ] );
			}

			foreach( EntityEntry e in entityEntries.Values )
			{
				try
				{
					e.Persister = factory.GetPersister( e.ClassName );
				}
				catch( MappingException me )
				{
					// Different from h2.0.3
					throw new InvalidOperationException( me.Message );
				}
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="factory"></param>
		/// <param name="autoClose"></param>
		/// <param name="timestamp"></param>
		/// <param name="interceptor"></param>
		internal SessionImpl( IDbConnection connection, SessionFactoryImpl factory, bool autoClose, long timestamp, IInterceptor interceptor )
		{
			this.connection = connection;
			connect = connection == null;
			this.interceptor = interceptor;

			this.autoClose = autoClose;
			this.timestamp = timestamp;

			this.factory = factory;

			entitiesByKey = new Hashtable( 50 );
			proxiesByKey = new Hashtable( 10 );
			nonExists = new HashedSet( );
			//TODO: hack with this cast
			entityEntries = ( IdentityMap ) IdentityMap.InstantiateSequenced();
			collectionEntries = ( IdentityMap ) IdentityMap.InstantiateSequenced();
			collectionsByKey = new Hashtable( 30 );
			arrayHolders = ( IdentityMap ) IdentityMap.Instantiate();

			InitTransientState();

			log.Debug( "opened session" );
		}

		/// <summary></summary>
		public IBatcher Batcher
		{
			get	{ return batcher; }
		}

		/// <summary></summary>
		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		/// <summary></summary>
		public long Timestamp
		{
			get { return timestamp; }
		}

		/// <summary></summary>
		public IDbConnection Close()
		{
			log.Debug( "closing session" );

			try
			{
				// when the connection is null nothing needs to be done - if there
				// is a value for connection then Disconnect() was not called - so we
				// need to ensure it gets called.
				if( connection == null )
				{
					connect = false;
					return null;
				}
				else
				{
					return Disconnect();
				}
			}
			finally
			{
				Cleanup();
			}
		}

		/// <summary>
		/// Ensure that the locks are downgraded to <see cref="LockMode.None"/>
		/// and that all of the softlocks in the <see cref="Cache"/> have
		/// been released.
		/// </summary>
		/// <param name="success"></param>
		public void AfterTransactionCompletion( bool success )
		{
			log.Debug( "transaction completion" );

			isCurrentTransaction = false;

			// downgrade locks
			foreach( EntityEntry entry in entityEntries.Values )
			{
				entry.LockMode = LockMode.None;
			}

			// release cache softlocks
			foreach( IExecutable executable in executions )
			{
				try
				{
					executable.AfterTransactionCompletion( success );
				}
				catch( CacheException ce )
				{
					log.Error( "could not release a cache lock", ce );
					// continue loop
				}
				catch( Exception e )
				{
					throw new AssertionFailure( "Exception releasing cache locks", e );
				}
			}
			executions.Clear();
		}

		private void InitTransientState()
		{
			insertions = new ArrayList( 20 );
			deletions = new ArrayList( 20 );
			updates = new ArrayList( 20 );
			collectionCreations = new ArrayList( 20 );
			collectionRemovals = new ArrayList( 20 );
			collectionUpdates = new ArrayList( 20 );

			executions = new ArrayList( 50 );
			batchLoadableEntityKeys = new Hashtable( 30 );
			loadingCollections = new Hashtable();
			nonlazyCollections = new ArrayList( 20 );

			batcher = factory.IsBatchUpdateEnabled ? (IBatcher) new BatchingBatcher( this ) : (IBatcher) new NonBatchingBatcher( this );
		}

		/// <summary>
		/// Mark the Session as being closed and Clear out the HashTables of
		/// entities and proxies along with the Identity Maps for entries, array
		/// holders, collections, and nullifiables.
		/// </summary>
		private void Cleanup()
		{
			closed = true;
			entitiesByKey.Clear();
			proxiesByKey.Clear();
			entityEntries.Clear();
			arrayHolders.Clear();
			collectionEntries.Clear();
			nullifiables.Clear();
			batchLoadableEntityKeys.Clear();
			collectionsByKey.Clear();
			nonExists.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public LockMode GetCurrentLockMode( object obj )
		{
			if( obj is INHibernateProxy )
			{
				obj = ( NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj ) ).GetImplementation( this );
				if( obj == null )
				{
					return LockMode.None;
				}
			}

			EntityEntry e = GetEntry( obj );
			if( e == null )
			{
				throw new TransientObjectException( "Given object not associated with the session" );
			}

			if( e.Status != Status.Loaded )
			{
				throw new ObjectDeletedException( "The given object was deleted", e.Id );
			}
			return e.LockMode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public LockMode GetLockMode( object entity )
		{
			return GetEntry( entity ).LockMode;
		}

		private void AddEntity( Key key, object obj )
		{
			entitiesByKey[ key ] = obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetEntity( Key key )
		{
			return entitiesByKey[ key ];
		}

		private object RemoveEntity( Key key )
		{
			object retVal = entitiesByKey[ key ];
			entitiesByKey.Remove( key );
			return retVal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="lockMode"></param>
		public void SetLockMode( object entity, LockMode lockMode )
		{
			GetEntry( entity ).LockMode = lockMode;
		}

		private EntityEntry AddEntry(
			object obj,
			Status status,
			object[ ] loadedState,
			object id,
			object version,
			LockMode lockMode,
			bool existsInDatabase,
			IClassPersister persister,
			bool disableVersionIncrement )
		{
			EntityEntry e = new EntityEntry( status, loadedState, id, version, lockMode, existsInDatabase, persister, disableVersionIncrement );
			entityEntries[ obj ] = e;
			return e;
		}

		private EntityEntry GetEntry( object obj )
		{
			return ( EntityEntry ) entityEntries[ obj ];
		}

		private EntityEntry RemoveEntry( object obj )
		{
			object retVal = entityEntries[ obj ];
			entityEntries.Remove( obj );
			return ( EntityEntry ) retVal;
		}

		private bool IsEntryFor( object obj )
		{
			return entityEntries.Contains( obj );
		}

		private CollectionEntry GetCollectionEntry( PersistentCollection coll )
		{
			return ( CollectionEntry ) collectionEntries[ coll ];
		}

		/// <summary></summary>
		public bool IsOpen
		{
			get { return !closed; }
		}

		/// <summary>
		/// Save a transient object. An id is generated, assigned to the object and returned
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Save( object obj )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to save null" );
			}

			object theObj = Unproxy( obj );

			EntityEntry e = GetEntry( theObj );
			if( e != null )
			{
				if( e.Status == Status.Deleted )
				{
					ForceFlush( e );
				}
				else
				{
					log.Debug( "object already associated with session" );
					return e.Id;
				}
			}

			//id might be generated by SQL insert
			object id = SaveWithGeneratedIdentifier( obj, Cascades.CascadingAction.ActionSaveUpdate, null );
			ReassociateProxy( obj, id );

			return id;
		}

		private void ForceFlush( EntityEntry e )
		{
			if ( log.IsDebugEnabled )
			{
				log.Debug( "flushing to force deletion of re-saved object" + MessageHelper.InfoString( e.Persister, e.Id ) );
			}

			if ( cascading > 0 )
			{
				throw new ObjectDeletedException( string.Format( "deleted object would be re-saved by cascade (remove deleted object from associations) {0} {1}", e.Id, e.Persister.MappedClass.Name ) );
			}

			Flush();
		}

		private object SaveWithGeneratedIdentifier( object obj, Cascades.CascadingAction action, object anything )
		{
			IClassPersister persister = GetPersister( obj );
			try
			{
				object id = persister.IdentifierGenerator.Generate( this, obj );

				if ( id == null )
				{
					throw new IdentifierGenerationException( string.Format( "null id generated for: {0}", obj.GetType().FullName ) );
				}
				else if ( id == IdentifierGeneratorFactory.ShortCircuitIndicator ) 
				{
					return GetIdentifier( obj ); //yick!!
				}
				else if ( id == IdentifierGeneratorFactory.IdentityColumnIndicator )
				{
					return DoSave( obj, null, persister, true, action, anything );
				}
				else
				{
					if ( log.IsDebugEnabled )
					{
						log.Debug( string.Format( "generated identifier: {0}", id ) );
					}
					return DoSave( obj, id, persister, false, action, anything );
				}
			}
			catch( Exception ex )
			{
				throw new ADOException( "Could not save object", ex );
			}
		}

		/// <summary>
		/// Save a transient object with a manually assigned ID
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Save( object obj, object id )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attemted to insert null" );
			}
			if( id == null )
			{
				throw new NullReferenceException( "null identifier passed to insert()" );
			}

			object theObj = Unproxy( obj );

			EntityEntry e = GetEntry( theObj );
			if( e != null )
			{
				if( e.Status == Status.Deleted )
				{
					ForceFlush( e );
				}
				else
				{
					if( !id.Equals( e.Id ) )
					{
						throw new PersistentObjectException(
							"object passed to save() was already persistent: " +
								MessageHelper.InfoString( e.Persister, id )
							);
					}
					log.Debug( "object already associated with session" );
				}
			}

			DoSave( theObj, id, GetPersister( theObj ), false, Cascades.CascadingAction.ActionSaveUpdate, null );

			ReassociateProxy( obj, id );
		}

		/*
		private object DoSave( object obj, object id )
		{
			IClassPersister persister = GetPersister( obj );

			Key key = null;
			bool identityCol;
			if( id == null )
			{
				if( persister.IsIdentifierAssignedByInsert )
				{
					identityCol = true;
				}
				else
				{
					throw new AssertionFailure( "null id" );
				}
			}
			else
			{
				identityCol = false;
			}

			if( log.IsDebugEnabled )
			{
				log.Debug( "saving " + MessageHelper.InfoString( persister, id ) );
			}

			if( !identityCol )
			{
				// if the id is generated by the db, we assign the key later
				key = new Key( id, persister );

				object old = GetEntity( key );
				if( old != null )
				{
					if( GetEntry( old ).Status == Status.Deleted )
					{
						Flush();
					}
					else
					{
						throw new HibernateException(
							"The generated identifier is already in use: " + MessageHelper.InfoString( persister, id )
							);
					}
				}

				persister.SetIdentifier( obj, id );
			}

			// sub-insertions should occur befoer containing insertions so
			// try to do the callback now
			if( persister.ImplementsLifecycle )
			{
				//TODO: H2.0.3 - verify this has same meaning as H2.0.3
				if( ( ( ILifecycle ) obj ).OnSave( this ) == LifecycleVeto.Veto )
				{
					return id;
				}
			}

			if( persister.ImplementsValidatable )
			{
				( ( IValidatable ) obj ).Validate();
			}

			// Put a placeholder in entries, so we don't recurse back and try to save() th
			// same object again.
			AddEntry( obj, Status.Saving, null, id, null, LockMode.Write, identityCol, persister, false );

			// cascade-save to many-to-one BEFORE the parent is saved
			cascading++;
			try
			{
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeBeforeInsertAfterDelete, null );
			}
			finally
			{
				cascading--;
			}

			object[ ] values = persister.GetPropertyValues( obj );
			IType[ ] types = persister.PropertyTypes;

			bool substitute = interceptor.OnSave( obj, id, values, persister.PropertyNames, types );

			if ( persister.IsVersioned )
				substitute = Versioning.SeedVersion( values, persister.VersionProperty, persister.VersionType, persister.IsDefaultVersion( obj ) ) || substitute;

			if ( persister.HasCollections )
			{
				// TODO - 2.1 has some extra code here for OnReplicateVisitor - is a new setting
				// that is only in h2.1 because of the method Replicate(object, ReplicateMode)
				WrapVisitor visitor = new WrapVisitor(this);
				// substitutes into values by side-effect
				visitor.ProcessValues(values, types);
				substitute = substitute || visitor.IsSubstitutionRequired;
			}

            if ( substitute )
			{
				//substitutes into values by side-effect
				persister.SetPropertyValues( obj, values );
			}

			TypeFactory.DeepCopy( values, types, persister.PropertyUpdateability, values );
			NullifyTransientReferences( values, types, identityCol, obj );

			if( identityCol )
			{
				try
				{
					id = persister.Insert( values, obj, this );
				}
				catch( Exception e )
				{
					throw new ADOException( "Could not insert", e );
				}

				key = new Key( id, persister );

				if( GetEntity( key ) != null )
				{
					throw new HibernateException( "The natively generated ID is already in use " + MessageHelper.InfoString( persister, id ) );
				}

				persister.SetIdentifier( obj, id );
			}

			AddEntity( key, obj );
			AddEntry( obj, Status.Loaded, values, id, Versioning.GetVersion( values, persister ), LockMode.Write, identityCol, persister, false );

			if( !identityCol )
			{
				insertions.Add( new ScheduledInsertion( id, values, obj, null, persister, this ) );
			}

			// cascade-save to collections AFTER the collection owner was saved
			cascading++;
			try
			{
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeAfterInsertBeforeDelete, null );
			}
			finally
			{
				cascading--;
			}

			return id;
		}
		*/

		private object DoSave( object obj, object id, IClassPersister persister, bool useIdentityColumn, Cascades.CascadingAction cascadeAction, object anything )
		{
			Key key;
			if ( useIdentityColumn )
			{
				// if the id is generated by the database, we assign the key later
				key = null;
			}
			else
			{
				key = new Key( id, persister );

				object old = GetEntity( key );
				if( old != null )
				{
					if( GetEntry( old ).Status == Status.Deleted )
					{
						Flush();
					}
					else
					{
						throw new HibernateException(
							"The generated identifier is already in use: " + MessageHelper.InfoString( persister, id )
							);
					}
				}

				persister.SetIdentifier( obj, id );
			}

			// Sub-insertions should occur before containing insertsion so
			// Try to do the callback not
			if ( persister.ImplementsLifecycle )
			{
				log.Debug( "calling OnSave()" );
				if ( ( (ILifecycle) obj).OnSave( this ) == LifecycleVeto.Veto )
				{
					log.Debug( "insertion vetoed by OnSave()" );
					return id;
				}
			}

			return DoSave( obj, key, persister, false, useIdentityColumn, cascadeAction, anything );
		}

		private object DoSave( object obj, Key key, IClassPersister persister, bool replicate, bool useIdentityColumn, Cascades.CascadingAction cascadeAction, object anything )
		{
			if ( persister.ImplementsValidatable )
			{
				( (IValidatable) obj ).Validate();
			}

			object id;
			if ( useIdentityColumn )
			{
				id = null;
				ExecuteInserts( );
			}
			else
			{
				id = key.Identifier;
			}

			// Put a placeholder in entries, so we don't recurse back to try and Save() the same object again.
			// QUESTION: Should this be done before OnSave()/OnUpdate() is called?
			AddEntry( obj, Status.Saving, null, id, null, LockMode.Write, useIdentityColumn, persister, false );

			// cascade-save to many-to-one BEFORE the parent is saved
			cascading++;
			try
			{
				Cascades.Cascade( this, persister, obj, cascadeAction, CascadePoint.CascadeBeforeInsertAfterDelete, anything );
			}
			finally
			{
				cascading--;
			}

			object[] values = persister.GetPropertyValues( obj );
			IType[] types = persister.PropertyTypes;

			bool substitute = false;
			if ( !replicate )
			{
				substitute = interceptor.OnSave( obj, id, values, persister.PropertyNames, types );

				// Keep the existing version number in the case of replicate!
				if ( persister.IsVersioned ) 
				{
					substitute = Versioning.SeedVersion( values, persister.VersionProperty, persister.VersionType, persister.IsDefaultVersion( obj ) ) || substitute;
				}
			}

			if ( persister.HasCollections )
			{
				// TODO: make OnReplicateVisitor extend WrapVisitor
				if ( replicate )
				{
					OnReplicateVisitor visitor = new OnReplicateVisitor( this, id );
					visitor.ProcessValues( values, types );
				}
				WrapVisitor visitor2 = new WrapVisitor( this );
				// substitutes into values by side-effect
				visitor2.ProcessValues( values, types );
				substitute = substitute || visitor2.IsSubstitutionRequired;
			}

			if ( substitute )
			{
				persister.SetPropertyValues( obj, values );
			}

			TypeFactory.DeepCopy( values, types, persister.PropertyUpdateability, values );
			NullifyTransientReferences( values, types, useIdentityColumn, obj );
			CheckNullability(values, persister, false );

			if ( useIdentityColumn )
			{
				ScheduledIdentityInsertion insert = new ScheduledIdentityInsertion( values, obj, persister, this );
				insert.Execute();
				if ( insert.HasAfterTransactionCompletion )
				{
					executions.Add( insert );
				}
				id = insert.GeneratedId;
				persister.SetIdentifier( obj, id );
				key = new Key( id, persister );
				CheckUniqueness( key, obj );
			}
			
			object version = Versioning.GetVersion( values, persister );
			AddEntity( key, obj );
			AddEntry( obj, Status.Loaded, values, id, version, LockMode.Write, useIdentityColumn, persister, replicate );
			nonExists.Remove( key );

			if ( !useIdentityColumn )
			{
				insertions.Add( new ScheduledInsertion( id, values, obj, version, persister, this ) );
			}

			// cascade-save to collections AFTER the collection owner was saved
			cascading++;
			try
			{
				Cascades.Cascade( this, persister, obj, cascadeAction, CascadePoint.CascadeAfterInsertBeforeDelete, anything );
			}
			finally
			{
				cascading--;
			}

			return id;
		}

		/// <summary>
		/// If the parameter <c>value</c> is an unitialized proxy then it will be reassociated
		/// with the session. 
		/// </summary>
		/// <param name="value">A persistable object, proxy, persistent collection or null</param>
		/// <returns>
		/// <c>true</c> when an uninitialized proxy was passed into this method, <c>false</c> otherwise.
		/// </returns>
		internal bool ReassociateIfUninitializedProxy(object value)
		{
			if( !NHibernateUtil.IsInitialized( value ) )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( proxy );
				ReassociateProxy( li, proxy );
				return true;
			}
			else 
			{
				return false;
			}
		}

		private void ReassociateProxy( Object value, object id )
		{
			if ( value is INHibernateProxy )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) value;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( proxy );
				li.Identifier = id;
				ReassociateProxy( li, proxy );
			}
		}

		private object Unproxy( object maybeProxy )
		{
			if ( maybeProxy is INHibernateProxy )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) maybeProxy;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( proxy );
				if ( li.IsUninitialized )
				{
					throw new PersistentObjectException( string.Format( "object was an uninitialized proxy for: {0}", li.PersistentClass.Name ) );
				}
				return li.GetImplementation(); //unwrap the object 
			}
			else
			{
				return maybeProxy;
			}
		}

		private object UnproxyAndReassociate( object maybeProxy )
		{
			if( maybeProxy is INHibernateProxy )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) maybeProxy;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( proxy );
				ReassociateProxy( li, proxy );
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
		private void ReassociateProxy( LazyInitializer li, INHibernateProxy proxy )
		{
			if( li.Session != this )
			{
				IClassPersister persister = GetPersister( li.PersistentClass );
				Key key = new Key( li.Identifier, persister );
				if( !proxiesByKey.Contains( key ) )
				{
					proxiesByKey[ key ] = proxy; // any earlier proxy takes precedence 
				}
				NHibernateProxyHelper.GetLazyInitializer( proxy ).Session = this;
			}
		}

		private void NullifyTransientReferences( object[ ] values, IType[ ] types, bool earlyInsert, object self )
		{
			for( int i = 0; i < types.Length; i++ )
			{
				values[ i ] = NullifyTransientReferences( values[ i ], types[ i ], earlyInsert, self );
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
		private object NullifyTransientReferences( object value, IType type, bool earlyInsert, object self )
		{
			if( value == null )
			{
				return null;
			}
			else if( type.IsEntityType || type.IsObjectType )
			{
				return ( IsUnsaved( value, earlyInsert, self ) ) ? null : value;
			}
			else if( type.IsComponentType )
			{
				IAbstractComponentType actype = ( IAbstractComponentType ) type;
				object[ ] subvalues = actype.GetPropertyValues( value, this );
				IType[ ] subtypes = actype.Subtypes;
				bool substitute = false;
				for( int i = 0; i < subvalues.Length; i++ )
				{
					object replacement = NullifyTransientReferences( subvalues[ i ], subtypes[ i ], earlyInsert, self );
					if( replacement != subvalues[ i ] )
					{
						substitute = true;
						subvalues[ i ] = replacement;
					}
				}
				if( substitute )
				{
					actype.SetPropertyValues( value, subvalues );
				}
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
		private bool IsUnsaved( object obj, bool earlyInsert, object self )
		{
			if( obj is INHibernateProxy )
			{
				// if its an uninitialized proxy, it can't be transient
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj );
				if( li.GetImplementation( this ) == null )
				{
					return false;
					// ie we never have to null out a reference to an uninitialized proxy
				}
				else
				{
					try
					{
						//unwrap it
						obj = li.GetImplementation( this );
					}
					catch( HibernateException he )
					{
						//does not occur
						throw new AssertionFailure( "Unexpected HibernateException occurred in IsTransient()", he );
					}
				}
			}

			// if it was a reference to self, don't need to nullify
			// unless we are using native id generation, in which
			// case we definitely need to nullify
			if( obj == self )
			{
				return earlyInsert;
			}

			// See if the entity is already bound to this session, if not look at the
			// entity identifier and assume that the entity is persistent if the
			// id is not "unsaved" (that is, we rely on foreign keys to keep
			// database integrity)
			EntityEntry e = GetEntry( obj );
			if( e == null )
			{
				return GetPersister( obj ).IsUnsaved( obj );
			}

			return e.Status == Status.Saving || (
				earlyInsert ? !e.ExistsInDatabase : nullifiables.Contains( new Key( e.Id, e.Persister ) )
				);
		}

		/// <summary>
		/// Delete a persistent object
		/// </summary>
		/// <param name="obj"></param>
		public void Delete( object obj )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to delete null" );
			}

			obj = UnproxyAndReassociate( obj );

			EntityEntry entry = GetEntry( obj );
			IClassPersister persister = null;
			if( entry == null )
			{
				log.Debug( "deleting a transient instance" );

				persister = GetPersister( obj );
				object id = persister.GetIdentifier( obj );

				if( id == null )
				{
					throw new HibernateException( "the transient instance passed to Delete() has a null identifier" );
				}

				object old = GetEntity( new Key( id, persister ) );

				if( old != null )
				{
					throw new HibernateException(
						"another object with the same id was already associated with the session: " +
							MessageHelper.InfoString( persister, id )
						);
				}

				new OnUpdateVisitor(this, id).Process( obj, persister );

				AddEntity( new Key( id, persister ), obj );
				entry = AddEntry(
					obj,
					Status.Loaded,
					persister.GetPropertyValues( obj ),
					id,
					persister.GetVersion( obj ),
					LockMode.None,
					true,
					persister,
					false
					);
				// not worth worrying about the proxy
			}
			else
			{
				log.Debug( "deleting a persistent instance" );

				if( entry.Status == Status.Deleted || entry.Status == Status.Gone )
				{
					log.Debug( "object was already deleted" );
					return;
				}
				persister = entry.Persister;
			}

			DoDelete( obj, entry, persister );
		}

		private void DoDelete( object obj, EntityEntry entry, IClassPersister persister )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "deleting " + MessageHelper.InfoString( persister, entry.Id ) );
			}

			IType[ ] propTypes = persister.PropertyTypes;

			object version = entry.Version;

			object[] loadedState;
			if( entry.LoadedState == null )
			{
				//ie the object came in from Update()
				loadedState = persister.GetPropertyValues( obj );
			}
			else
			{
				loadedState = entry.LoadedState;
			}
			entry.DeletedState = new object[ loadedState.Length ];
			TypeFactory.DeepCopy( loadedState, propTypes, persister.PropertyUpdateability, entry.DeletedState );

			interceptor.OnDelete( obj, entry.Id, entry.DeletedState, persister.PropertyNames, propTypes );

			entry.Status = Status.Deleted; // before cascade and Lifecycle callback, so we are circular-reference safe
			Key key = new Key( entry.Id, persister );

			IList deletionsByOnDelete = null;
			ISet nullifiablesAfterOnDelete = null;

			// do Lifecycle callback before cascades, since this can veto
			if ( persister.ImplementsLifecycle ) 
			{
				ISet oldNullifiables = (ISet) nullifiables.Clone();
				ArrayList oldDeletions = (ArrayList) deletions.Clone();

				nullifiables.Add( key ); //the deletion of the parent is actually executed BEFORE any deletion from onDelete()

				try
				{
					log.Debug( "calling onDelete()" );
					if ( ( (ILifecycle) obj).OnDelete( this ) == LifecycleVeto.Veto )
					{
						//rollback deletion
						entry.Status = Status.Loaded;
						entry.DeletedState = null;
						nullifiables = oldNullifiables;
						log.Debug( "deletion vetoed by onDelete()" );
						return; // don't let it cascade
					}
				}
				catch ( Exception )
				{
					//rollback deletion
					entry.Status = Status.Loaded;
					entry.DeletedState = null;
					nullifiables = oldNullifiables;
					throw;
				}

				//note, the following assumes that onDelete() didn't cause the session to be flushed! 
				// TODO: add a better check that it doesn't
				if ( oldDeletions.Count > deletions.Count )
				{
					throw new HibernateException( "session was flushed during onDelete()" );
				}
				deletionsByOnDelete = Sublist( deletions, oldDeletions.Count, deletions.Count );
				deletions = oldDeletions;
				nullifiablesAfterOnDelete = nullifiables;
				nullifiables = oldNullifiables;
			}

			cascading++;
			try
			{
				// cascade-delete to collections "BEFORE" the collection owner is deleted
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeAfterInsertBeforeDelete, null );
			}
			finally
			{
				cascading--;
			}

			NullifyTransientReferences( entry.DeletedState, propTypes, false, obj );
			//CheckNullability( entry.DeletedState, persister, true );
			nullifiables.Add( key );

			ScheduledDeletion delete = new ScheduledDeletion( entry.Id, version, obj, persister, this );
			deletions.Add( delete ); // Ensures that containing deletions happen before sub-deletions

			if ( persister.ImplementsLifecycle )
			{
				// after nullify, because we don't want to nullify references to subdeletions
				nullifiables.AddAll( nullifiablesAfterOnDelete );
				// after deletions.add(), to respect foreign key constraints
				deletions.AddRange( deletionsByOnDelete );
			}

			cascading++;
			try
			{
				// cascade-delete to many-to-one AFTER the parent was deleted
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionDelete, CascadePoint.CascadeBeforeInsertAfterDelete, null );
			}
			finally
			{
				cascading--;
			}
		}

		private IList Sublist( IList list, int fromIx, int toIx )
		{
			IList newList = new ArrayList( toIx - fromIx );

			for ( int i = fromIx; i < toIx; i++ )
			{
				newList.Add( list[ i ] );
			}

			return newList;
		}

		private void RollbackDeletion( EntityEntry entry, ScheduledDeletion delete )
		{
			entry.Status = Status.Loaded;
			entry.DeletedState = null;
			deletions.Remove( delete );
		}

		/// <summary>
		/// Checks to see if there are any Properties that should not be null 
		/// are references to null or to a transient object.
		/// </summary>
		/// <param name="values">An object array of values that should be validated.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that describes which values can be null.</param>
		/// <param name="isUpdate">A <see cref="Boolean"/> indicating if this is an Update operation.</param>
		/// <exception cref="HibernateException">
		/// Thrown when a non-nullable property contains a value that would
		/// persist the value of null to the database.
		/// </exception>
		private static void CheckNullability( object[] values, IClassPersister persister, bool isUpdate )
		{
			bool[] nullability = persister.PropertyNullability;
			bool[] checkability = isUpdate ? persister.PropertyUpdateability : persister.PropertyInsertability;

			for ( int i = 0; i < values.Length; i++ )
			{
				if ( !nullability[ i ] && checkability[ i ] && values[ i ] == null )
				{
					throw new HibernateException( string.Format( "not-null property references a null or transient value: {0}, {1}", persister.MappedClass.Name, persister.PropertyNames[ i ] ) );
				}
			}
		}

		internal void RemoveCollection( ICollectionPersister role, object id )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "collection dereferenced while transient " + MessageHelper.InfoString( role, id ) );
			}
			// 2.1 comments this out
			/*
			if( role.HasOrphanDelete )
			{
				throw new HibernateException( "You may not dereference a collection with cascade=\"all-delete-orphan\"" );
			}
			*/
			collectionRemovals.Add( new ScheduledCollectionRemove( role, id, false, this ) );
		}

		private static bool IsCollectionSnapshotValid( ICollectionSnapshot snapshot )
		{
			return snapshot != null &&
				snapshot.Role != null &&
				snapshot.Key != null;
		}

		internal static bool IsOwnerUnchanged( ICollectionSnapshot snapshot, ICollectionPersister persister, object id )
		{
			return IsCollectionSnapshotValid( snapshot ) &&
				persister.Role.Equals( snapshot.Role ) &&
				id.Equals( snapshot.Key );
		}

		/// <summary>
		/// Reattach a detached (disassociated) initialized or uninitialized collection wrapper
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="snapshot"></param>
		internal void ReattachCollection( PersistentCollection collection, ICollectionSnapshot snapshot )
		{
			if( collection.WasInitialized )
			{
				AddInitializedDetachedCollection( collection, snapshot );
			}
			else 
			{
				if( !IsCollectionSnapshotValid( snapshot) ) 
				{
					throw new HibernateException("could not reassociate uninitialized transient collection");
				}
				AddUninitializedDetachedCollection(
					collection,
					GetCollectionPersister( snapshot.Role ),
					snapshot.Key);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void Update( object obj )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to update null" );
			}

			if( ReassociateIfUninitializedProxy( obj ) )
			{
				return;
			}

			object theObj = UnproxyAndReassociate( obj );

			IClassPersister persister = GetPersister( theObj );

			if( IsEntryFor( theObj ) )
			{
				log.Debug( "object already associated with session" );
				// do nothing
			}
			else
			{
				// the object is transient
				object id = persister.GetIdentifier( theObj );

				if( id == null )
				{
					// assume this is a newly instantiated transient object 
					throw new HibernateException( "The given object has a null identifier property " + MessageHelper.InfoString( persister ) );
				}
				else
				{
					DoUpdate( theObj, id, persister );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void SaveOrUpdate( object obj )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to update null" );
			}

			if( ReassociateIfUninitializedProxy( obj ) )
			{
				return;
			}

			object theObj = UnproxyAndReassociate( obj ); //a proxy is always "update", never "save"

			EntityEntry e = GetEntry( theObj );
			if( e != null && e.Status != Status.Deleted )
			{
				// do nothing for persistent instances
				log.Debug( "SaveOrUpdate() persistent instance" );
			}
			else if( e != null )
			{
				//ie status==DELETED
				log.Debug( "SaveOrUpdate() deleted instance" );
				Save( obj );
			}
			else
			{
				// the object is transient
				object isUnsaved = interceptor.IsUnsaved( theObj );
				IClassPersister persister = GetPersister( theObj );
				if( isUnsaved == null )
				{
					// use unsaved-value
					if( persister.IsUnsaved( theObj ) )
					{
						if( log.IsDebugEnabled )
						{
							log.Debug( "SaveOrUpdate() unsaved instance" );
						}
						Save( obj );
					}
					else
					{
						object id = persister.GetIdentifier( theObj );
						if( log.IsDebugEnabled )
						{
							log.Debug( "SaveOrUpdate() previously saved instance with id: " + id );
						}
						DoUpdate( theObj, id, persister );
					}
				}
				else
				{
					if( true.Equals( isUnsaved ) )
					{
						log.Debug( "SaveOrUpdate() unsaved instance" );
						Save( obj );
					}
					else
					{
						log.Debug( "SaveOrUpdate() previously saved instance" );
						DoUpdate( theObj, persister.GetIdentifier( theObj ), persister );
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Update( object obj, object id )
		{
			if( id == null )
			{
				throw new NullReferenceException( "null is not a valid identifier" );
			}
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to update null" );
			}

			if( obj is INHibernateProxy )
			{
				 NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj ).Identifier = id;
			}

			if( ReassociateIfUninitializedProxy( obj ) )
			{
				return;
			}

			object theObj = UnproxyAndReassociate( obj );

			EntityEntry e = GetEntry( theObj );
			if( e == null )
			{
				IClassPersister persister = GetPersister( theObj );
				persister.SetIdentifier( theObj, id );
				DoUpdate( theObj, id, persister );
			}
			else
			{
				if( !e.Id.Equals( id ) )
				{
					throw new PersistentObjectException(
						"The instance passed to Update() was already persistent: " +
							MessageHelper.InfoString( e.Persister, id )
						);
				}
			}
		}

		private void DoUpdateMutable( object obj, object id, IClassPersister persister )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "updating " + MessageHelper.InfoString( persister, id ) );
			}

			Key key = new Key( id, persister );
			CheckUniqueness( key, obj );

			// TODO: 2.1 does not contain this
			/*
			object old = GetEntity( key );
			if( old == obj )
			{
				throw new AssertionFailure(
					"Hibernate has a bug in Update() ... or you are using an illegal id type: " +
					MessageHelper.InfoString( persister, id )
					);
			}
			else if( old != null )
			{
				throw new HibernateException(
					"Another object was associated with this id ( the object with the given id was already loaded): " +
					MessageHelper.InfoString( persister, id )
					);
			}
			*/

			if( persister.ImplementsLifecycle )	
			{
				log.Debug( "calling onUpdate()" );
				if( ( ( ILifecycle ) obj ).OnUpdate( this ) == LifecycleVeto.Veto )
				{
					log.Debug( "update vetoed by onUpdate()" );
					Reassociate( obj, id, persister );
					return;
				}																 
			} 

			// this is a transient object with existing persistent state not loaded by the session

			new OnUpdateVisitor(this, id).Process( obj, persister );

			AddEntity( key, obj );
			AddEntry( obj, Status.Loaded, null, id, persister.GetVersion( obj ), LockMode.None, true, persister, false );
		}

		private void DoUpdate( object obj, object id, IClassPersister persister )
		{
			if( !persister.IsMutable )
			{
				log.Debug( "immutable instance passed to doUpdate(), locking" );
				Reassociate( obj, id, persister );
			}
			else
			{
				DoUpdateMutable( obj, id, persister );
			}

			cascading++;
			try
			{
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate, null ); // do cascade
			}
			finally
			{
				cascading--;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="replicationMode"></param>
		/// <param name="persister"></param>
		/// <remarks>Only used by replicate</remarks>
		private void DoReplicate( object obj, object id, object version, ReplicationMode replicationMode, IClassPersister persister )
		{
			if ( log.IsInfoEnabled )
			{
				log.Info( "replicating changes to " + MessageHelper.InfoString( persister, id ) );
			}

			new OnReplicateVisitor( this, id ).Process( obj, persister );
			Key key = new Key( id, persister );
			AddEntity( key, obj );
			AddEntry( obj, Status.Loaded, null, id, version, LockMode.None, true, persister, true );

			cascading++;
			try
			{
				// do cascade
				Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionReplicate, CascadePoint.CascadeOnUpdate, replicationMode ); 
			}
			finally
			{
				cascading--;
			}
		}

		private static object[ ] NoArgs = new object[0];
		private static IType[ ] NoTypes = new IType[0];

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IList Find( string query )
		{
			return Find( query, NoArgs, NoTypes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IList Find( string query, object value, IType type )
		{
			return Find( query, new object[ ] {value}, new IType[ ] {type} );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public IList Find( string query, object[ ] values, IType[ ] types )
		{
			return Find( query, new QueryParameters( types, values ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList Find( string query, QueryParameters parameters )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "find: " + query );
				parameters.LogParameters();
			}

			parameters.ValidateParameters();

			QueryTranslator[ ] q = GetQueries( query, false );

			IList results = new ArrayList();

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called

			//execute the queries and return all result lists as a single list
			try
			{
				for( int i = 0; i < q.Length; i++ )
				{
					IList currentResults;
					try
					{
						currentResults = q[ i ].List( this, parameters );
					}
					catch( Exception e )
					{
						throw new ADOException( "Could not execute query", e );
					}

					for( int j = 0; j < results.Count; j++ )
					{
						currentResults.Add( results[ j ] );
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

		private QueryTranslator[ ] GetQueries( string query, bool scalar )
		{
			// a query that naemes an interface or unmapped class in the from clause
			// is actually executed as multiple queries
			string[ ] concreteQueries = QueryTranslator.ConcreteQueries( query, factory );

			// take the union of the query spaces (ie the queried tables)
			QueryTranslator[ ] q = new QueryTranslator[concreteQueries.Length];
			HashedSet qs = new HashedSet();
			for( int i = 0; i < concreteQueries.Length; i++ )
			{
				q[ i ] = scalar ? factory.GetShallowQuery( concreteQueries[ i ] ) : factory.GetQuery( concreteQueries[ i ] );
				qs.AddAll( q[ i ].QuerySpaces );
			}

			AutoFlushIfRequired( qs );

			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IEnumerable Enumerable( string query )
		{
			return Enumerable( query, NoArgs, NoTypes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable Enumerable( string query, object value, IType type )
		{
			return Enumerable( query, new object[ ] {value}, new IType[ ] {type} );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public IEnumerable Enumerable( string query, object[ ] values, IType[ ] types )
		{
			return Enumerable( query, new QueryParameters( types, values ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IEnumerable Enumerable( string query, QueryParameters parameters )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "GetEnumerable: " + query );
				parameters.LogParameters();
			}

			QueryTranslator[ ] q = GetQueries( query, true );

			if( q.Length == 0 )
			{
				return new ArrayList();
			}

			IEnumerable[ ] results = new IEnumerable[q.Length];

			dontFlushFromFind++; //stops flush being called multiple times if this method is recursively called
			//execute the queries and return all results as a single enumerable
			try
			{
				for( int i = 0; i < q.Length; i++ )	
				{
					results[i] = q[ i ].GetEnumerable( parameters, this );
				}
			} 				
			catch( Exception e )
			{
				throw new ADOException( "Could not execute query", e );
			} 
			finally 
			{
				dontFlushFromFind--; 
			}

			return results.Length > 0 ? new JoinedEnumerable( results ) : results[0];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public int Delete( string query )
		{
			return Delete( query, NoArgs, NoTypes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public int Delete( string query, object value, IType type )
		{
			return Delete( query, new object[ ] {value}, new IType[ ] {type} );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public int Delete( string query, object[ ] values, IType[ ] types )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "delete: " + query );
				if( values.Length != 0 )
				{
					log.Debug( "parameters: " + StringHelper.ToString( values ) );
				}
			}

			IList list = Find( query, values, types );
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				Delete( list[ i ] );
			}
			return count;
		}

		private void CheckUniqueness(Key key, object obj)
		{
			object entity = GetEntity(key);
			if (entity == obj) throw new AssertionFailure("object already associated in DoSave()");
			if (entity != null)
			{
				throw new HibernateException(
					"another object with the same id was already associated with the session: " +
					MessageHelper.InfoString( obj.GetType(), key.Identifier ));
			}
		}

		private EntityEntry Reassociate(object obj, object id, IClassPersister persister)
		{
			if ( log.IsDebugEnabled ) log.Debug( "reassociating transient instance: " + MessageHelper.InfoString(persister, id) );
			Key key = new Key(id, persister);
			CheckUniqueness(key, obj);
			AddEntity(key, obj);
			Object[] values = persister.GetPropertyValues(obj);
			TypeFactory.DeepCopy(values, persister.PropertyTypes, persister.PropertyUpdateability, values);
			object version = Versioning.GetVersion(values, persister);
			EntityEntry newEntry = AddEntry(obj, Status.Loaded, values, id, version, LockMode.None,	true, persister, false );
			new OnLockVisitor(this, id).Process(obj, persister);
			return newEntry;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void Lock( object obj, LockMode lockMode )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to lock null" );
			}

			if( lockMode == LockMode.Write )
			{
				throw new HibernateException( "Invalid lock mode for Lock()" );
			}

			obj = UnproxyAndReassociate( obj );
			//TODO: if object was an uninitialized proxy, this is inefficient, 
			//resulting in two SQL selects 

			EntityEntry e = GetEntry( obj );
			if( e == null )
			{
				IClassPersister objPersister = GetPersister(obj);
				object id = objPersister.GetIdentifier(obj);

				if (!IsSaved(obj))
				{
					throw new HibernateException(
						"cannot lock an unsaved transient instance: "
						+ MessageHelper.InfoString(objPersister) );
				}

				e = Reassociate(obj, id, objPersister);

				cascading++;
				try
				{
					Cascades.Cascade(this, objPersister, obj, Cascades.CascadingAction.ActionLock,
						CascadePoint.CascadeOnLock, lockMode);
				}
				finally 
				{
					cascading--;
				}
			}

			UpgradeLock( obj, e, lockMode );
		}

		private void UpgradeLock( object obj, EntityEntry entry, LockMode lockMode )
		{
			if( lockMode.GreaterThan( entry.LockMode ) )
			{
				if( entry.Status != Status.Loaded )
				{
					throw new TransientObjectException( "attempted to lock a deleted instance" );
				}

				IClassPersister persister = entry.Persister;
				ISoftLock myLock = null;

				if( log.IsDebugEnabled )
				{
					log.Debug( "locking " + MessageHelper.InfoString( persister, entry.Id ) + " in mode: " + lockMode );
				}

				if( persister.HasCache )
				{
					myLock = persister.Cache.Lock( entry.Id, entry.Version );
				}
				try
				{
					persister.Lock( entry.Id, entry.Version, obj, lockMode, this );
					entry.LockMode = lockMode;
				}
				catch( Exception exp )
				{
					throw new ADOException( "could not lock object", exp );
				}
				finally
				{
					// the database now holds a lock + the object is flushed from the cache,
					// so release the soft lock
					if( persister.HasCache )
					{
						persister.Cache.Release( entry.Id, myLock );
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateFilter( object collection, string queryString )
		{
			return new FilterImpl( queryString, collection, this );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public IQuery CreateQuery( string queryString )
		{
			return new QueryImpl( queryString, this );
		}

		/// <summary>
		/// Obtain an instance of <see cref="IQuery" /> for a named query string defined in the
		/// mapping file.
		/// </summary>
		/// <param name="queryName">The name of a query defined externally.</param>
		/// <returns>An <see cref="IQuery"/> fro a named query string.</returns>
		/// <remarks>
		/// The query can be either in <c>hql</c> or <c>sql</c> format.
		/// </remarks>
		public IQuery GetNamedQuery( string queryName )
		{
			string queryString = factory.GetNamedQuery( queryName );
			if( queryString!=null )
			{
				return CreateQuery( queryString );
			}

			SessionFactoryImpl.InternalNamedSQLQuery nq = factory.GetNamedSQLQuery( queryName );
			if( nq==null )
			{
				throw new MappingException( "Named query not known: " + queryName );
			}

			return CreateSQLQuery( nq.QueryString, nq.ReturnAliases, nq.ReturnClasses, nq.QuerySpaces );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object Instantiate( System.Type clazz, object id )
		{
			return Instantiate( factory.GetPersister( clazz ), id );
		}

		/// <summary>
		/// Give the interceptor an opportunity to override the default instantiation
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object Instantiate( IClassPersister persister, object id )
		{
			object result = interceptor.Instantiate( persister.MappedClass, id );
			if( result == null )
			{
				result = persister.Instantiate( id );
			}
			return result;
		}

		/// <summary></summary>
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
		private bool AutoFlushIfRequired( ISet querySpaces )
		{
			if( flushMode == FlushMode.Auto && dontFlushFromFind == 0 )
			{
				int oldSize = collectionRemovals.Count;

				FlushEverything();

				if( AreTablesToBeUpdated( querySpaces ) )
				{
					log.Debug( "Need to execute flush" );

					Execute();
					PostFlush();
					// note: Execute() clears all collectionXxxxtion collections
					return true;
				}
				else
				{
					log.Debug( "dont need to execute flush" );

					// sort of don't like this: we re-use the same collections each flush
					// even though their state is not kept between flushes. However, its
					// nice for performance since the collection sizes will be "nearly"
					// what we need them to be next time.
					collectionCreations.Clear();
					collectionUpdates.Clear();
					updates.Clear();
					// collection deletes are a special case since Update() can add
					// deletions of collections not loaded by the session.
					for( int i = collectionRemovals.Count - 1; i >= oldSize; i-- )
					{
						collectionRemovals.RemoveAt( i );
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
		public object NarrowProxy( object proxy, IClassPersister p, Key key, object obj )
		{
			if( !p.ConcreteProxyClass.IsAssignableFrom( proxy.GetType() ) )
			{
				if( log.IsWarnEnabled )
				{
					log.Warn(
						"Narrowing proxy to " + p.ConcreteProxyClass + " - this operation breaks =="
						);
				}

				if( obj != null )
				{
					proxiesByKey.Remove( key );
					return obj;
				}
				else
				{
					IProxyGenerator generator = ProxyGeneratorFactory.GetProxyGenerator();
					proxy = generator.GetProxy(
						p.MappedClass
						, p.ConcreteProxyClass
						, p.ProxyInterfaces
						, p.ProxyIdentifierProperty
						, key.Identifier
						, this );

					proxiesByKey[ key ] = proxy;
					return proxy;
				}
			}
			else
			{
				return proxy;
			}
		}

		/// <summary>
		/// Grab the existing proxy for an instance, if one exists
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="key"></param>
		/// <param name="impl"></param>
		/// <returns></returns>
		public object ProxyFor( IClassPersister persister, Key key, object impl )
		{
			if( !persister.HasProxy || key == null )
			{
				return impl;
			}

			object proxy = proxiesByKey[ key ];
			if( proxy != null )
			{
				return NarrowProxy( proxy, persister, key, impl );
			}
			else
			{
				return impl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="impl"></param>
		/// <returns></returns>
		public object ProxyFor( object impl )
		{
			EntityEntry e = GetEntry( impl );

			// can't use e.persister since it is null after addUninitializedEntity 
			// (when this method is called)
			IClassPersister p = GetPersister( impl );
			return ProxyFor( p, new Key( e.Id, p ), impl );
		}

		/// <summary>
		/// Create a "temporary" entry for a newly instantiated entity. The entity is 
		/// uninitialized, but we need the mapping from id to instance in order to guarantee 
		/// uniqueness.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void AddUninitializedEntity( Key key, object obj, LockMode lockMode )
		{
			//IClassPersister p = GetPersister(obj);
			AddEntity( key, obj );
			AddEntry( obj, Status.Loading, null, key.Identifier, null, lockMode, true, null, false ); // p );
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
		public void PostHydrate( IClassPersister persister, object id, object[ ] values, object obj, LockMode lockMode )
		{
			persister.SetIdentifier( obj, id );
			object version = Versioning.GetVersion( values, persister );
			AddEntry( obj, Status.Loading, values, id, version, lockMode, true, persister, false );

			if( log.IsDebugEnabled && version != null )
			{
				log.Debug( "Version: " + version );
			}
		}

		private void ThrowObjectNotFound( object o, object id, System.Type clazz )
		{
			if( o == null )
			{
				throw new ObjectNotFoundException( "No row with the given identifier exists", id, clazz );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		public void Load( object obj, object id )
		{
			if( id == null )
			{
				throw new NullReferenceException( "null is not a valid identifier" );
			}
			DoLoadByObject( obj, id, true, LockMode.None );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object Load( System.Type clazz, object id )
		{
			if( id == null )
			{
				throw new NullReferenceException( "null is not a valid identifier" );
			}
			object result = DoLoadByClass( clazz, id, true, true );
			ThrowObjectNotFound( result, id, clazz );
			return result;
		}

		public object Get(System.Type clazz, object id)
		{
			if (id==null) throw new NullReferenceException("null is not a valid identifier");
			object result = DoLoadByClass(clazz, id, true, false);
			return result;
		}

		///<summary> 
		/// Load the data for the object with the specified id into a newly created object.
		/// Do NOT return a proxy.
		///</summary>
		public object ImmediateLoad( System.Type clazz, object id )
		{
			object result = DoLoad( clazz, id, null, LockMode.None, false );
			ThrowObjectNotFound( result, id, clazz );
			return result;
		}

		///<summary>
		/// Return the object with the specified id or null if no row with that id exists. Do not defer the load
		/// or return a new proxy (but do return an existing proxy). Do not check if the object was deleted.
		///</summary>
		public object InternalLoadOneToOne( System.Type clazz, object id )
		{
			return DoLoadByClass( clazz, id, false, false );
		}

		/// <summary>
		/// Return the object with the specified id or throw exception if no row with that id exists. Defer the load,
		/// return a new proxy or return an existing proxy if possible. Do not check if the object was deleted.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object InternalLoad( System.Type clazz, object id )
		{
			object result = DoLoadByClass( clazz, id, false, true );
			ThrowObjectNotFound( result, id, clazz );
			return result;
		}

		/**
		* Load the data for the object with the specified id into the supplied
		* instance. A new key will be assigned to the object. If there is an
		* existing uninitialized proxy, this will break identity equals as far
		* as the application is concerned.
		*/

		private void DoLoadByObject( object obj, object id, bool checkDeleted, LockMode lockMode )
		{
			System.Type clazz = obj.GetType();
			if( GetEntry( obj ) != null )
			{
				throw new PersistentObjectException(
					"attempted to load into an instance that was already associated with the Session: " +
						MessageHelper.InfoString( clazz, id )
					);
			}
			object result = DoLoad( clazz, id, obj, lockMode, checkDeleted );
			ThrowObjectNotFound( result, id, clazz );
			if( result != obj )
			{
				throw new HibernateException(
					"The object with that id was already loaded by the Session: " +
						MessageHelper.InfoString( clazz, id )
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
		private object DoLoadByClass( System.Type clazz, object id, bool checkDeleted, bool allowProxyCreation )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "loading " + MessageHelper.InfoString( clazz, id ) );
			}

			IClassPersister persister = GetPersister( clazz );
			if( !persister.HasProxy )
			{
				// this class has no proxies (so do a shortcut)
				return DoLoad( clazz, id, null, LockMode.None, checkDeleted );
			}
			else
			{
				Key key = new Key( id, persister );
				object proxy = null;

				if( GetEntity( key ) != null )
				{
					// return existing object or initialized proxy (unless deleted)
					return ProxyFor(
						persister,
						key,
						DoLoad( clazz, id, null, LockMode.None, checkDeleted )
						);
				}
				else if( ( proxy = proxiesByKey[ key ] ) != null )
				{
					// return existing uninitizlied proxy
					return NarrowProxy( proxy, persister, key, null );
				}
				else if( allowProxyCreation )
				{
					// return new uninitailzed proxy
					if( persister.HasProxy )
					{
						IProxyGenerator generator = ProxyGeneratorFactory.GetProxyGenerator();
						proxy = generator.GetProxy( clazz, persister.ConcreteProxyClass, persister.ProxyInterfaces, persister.ProxyIdentifierProperty, id, this );
					}
					proxiesByKey[ key ] = proxy;
					return proxy;
				}
				else
				{
					// return a newly loaded object
					return DoLoad( clazz, id, null, LockMode.None, checkDeleted );
				}
			}
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This method always hits the db, and does not create proxies. It should return
		/// an existing proxy where appropriate.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <param name="allowNull"></param>
		/// <returns></returns>
		private object DoLoad( System.Type clazz, object id, LockMode lockMode, bool allowNull )
		{
			ISoftLock myLock = null;

			if( id == null )
			{
				throw new NullReferenceException( "null is not a valid identifier" );
			}
			if( log.IsDebugEnabled )
			{
				log.Debug( "loading " + MessageHelper.InfoString( clazz, id ) + " in lock mode: " + lockMode );
			}

			IClassPersister persister = GetPersister( clazz );
			if( persister.HasCache )
			{
				//increments the lock
				myLock = persister.Cache.Lock( id, null );
			} 
			object result;
			try
			{
				result = DoLoad( clazz, id, null, lockMode, true );
			}
			finally
			{
				// the datbase now hold a lock + the object is flushed from the cache,
				// so release the soft lock
				if( persister.HasCache )
				{
					persister.Cache.Release( id, myLock );
				}
			}

			if( !allowNull ) ThrowObjectNotFound( result, id, persister.MappedClass );

			// return existing proxy (if one exists)
			return ProxyFor( persister, new Key( id, persister ), result );
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		/// 
		/// If the object does not exist in the database, an exception is thrown.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		/// <exception cref="ObjectNotFoundException">
		/// Thrown when the object with the specified id does not exist in the database.
		/// </exception>
		public object Load( System.Type clazz, object id, LockMode lockMode )
		{
			if( lockMode == LockMode.Write )
			{
				throw new HibernateException( "invalid lock mode for Load()" );
			}

			if( lockMode == LockMode.None )
			{
				// we don't necessarily need to hit the db in this case
				return Load( clazz, id );
			}

			return DoLoad( clazz, id, lockMode, false );
		}

		/// <summary>
		/// Load the data for the object with the specified id into a newly created object
		/// using "for update", if supported. A new key will be assigned to the object.
		/// This should return an existing proxy where appropriate.
		/// 
		/// If the object does not exist in the database, null is returned.
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="lockMode"></param>
		/// <returns></returns>
		public object Get( System.Type clazz, object id, LockMode lockMode )
		{
			if( lockMode == LockMode.Write )
			{
				throw new HibernateException( "invalid lock mode for Get()" );
			}

			if( lockMode == LockMode.None )
			{
				// we don't necessarily need to hit the db in this case
				return Load( clazz, id );
			}

			return DoLoad( clazz, id, lockMode, true );
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
		private object DoLoad( System.Type theClass, object id, object optionalObject, LockMode lockMode, bool checkDeleted )
		{
			//DONT need to flush before a load by id

			if( log.IsDebugEnabled )
			{
				log.Debug( "attempting to resolve " + MessageHelper.InfoString( theClass, id ) );
			}

			bool isOptionalObject = optionalObject != null;

			IClassPersister persister = GetPersister( theClass );
			Key key = new Key( id, persister );

			// LOOK FOR LOADED OBJECT 
			// Look for Status.Loaded object
			object old = GetEntity( key );
			if( old != null )
			{ //if this object was already loaded
				Status status = GetEntry( old ).Status;
				if( checkDeleted && ( status == Status.Deleted || status == Status.Gone ) )
				{
					throw new ObjectDeletedException( "The object with that id was deleted", id );
				}
				Lock( old, lockMode );
				if( log.IsDebugEnabled )
				{
					log.Debug( "resolved object in session cache " + MessageHelper.InfoString( persister, id ) );
				}
				return old;

			}
			else
			{
				// check to see if we know already that it does not exist:
				if ( nonExists.Contains(key) ) return null;

				// LOOK IN CACHE
				CacheEntry entry = persister.HasCache ? ( CacheEntry ) persister.Cache.Get( id, timestamp ) : null;
				if( entry != null )
				{
					return AssembleCacheEntry( entry, id, persister, optionalObject, lockMode );
				}
				else
				{
					//GO TO DATABASE
					if( log.IsDebugEnabled )
					{
						log.Debug( "object not resolved in any cache " + MessageHelper.InfoString( persister, id ) );
					}
					try
					{
						object o = persister.Load( id, optionalObject, lockMode, this );
						if( o == null ) AddNonExist( key );
						return o;
					} 
						//TODO: change to some kind of SqlException
					catch( Exception e )
					{
						throw new ADOException( "could not load object", e );
					}
				}
			}
		}

		private object AssembleCacheEntry(CacheEntry entry, object id, IClassPersister persister, object optionalObject, LockMode lockMode)
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "resolved object in second-level cache " + MessageHelper.InfoString( persister, id ) );
			}
			IClassPersister subclassPersister = GetPersister( entry.Subclass );
			object result = optionalObject != null ? optionalObject : Instantiate( subclassPersister, id );
			AddEntry( result, Status.Loading, null, id, null, LockMode.None, true, subclassPersister, false );
			AddEntity( new Key( id, persister ), result );

			IType[ ] types = subclassPersister.PropertyTypes;
			object[ ] values = entry.Assemble( result, id, subclassPersister, this ); // intializes result by side-effect

			TypeFactory.DeepCopy( values, types, subclassPersister.PropertyUpdateability, values );
			object version = Versioning.GetVersion( values, subclassPersister );

			if( log.IsDebugEnabled )
			{
				log.Debug( "Cached Version: " + version );
			}

			AddEntry( result, Status.Loaded, values, id, version, LockMode.None, true, subclassPersister, false );
			InitializeNonLazyCollections();

			// upgrate lock if necessary;
			Lock( result, lockMode );

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void Refresh( object obj )
		{
			Refresh( obj, LockMode.Read );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="lockMode"></param>
		public void Refresh( object obj, LockMode lockMode )
		{
			if( obj == null )
			{
				throw new NullReferenceException( "attempted to refresh null" );
			}

			if( ReassociateIfUninitializedProxy( obj ) )
			{
				return;
			}

			object theObj = UnproxyAndReassociate( obj );
			EntityEntry e = RemoveEntry( theObj );
			IClassPersister persister;
			object id;

			if( e==null )
			{
				persister = GetPersister( theObj );
				id = persister.GetIdentifier( theObj );
				if( log.IsDebugEnabled )
				{
					log.Debug( "refreshing transient " + MessageHelper.InfoString( persister, id ) );
				}

				// TODO: add another check here about refreshing transient instance when persisted
				// instance already associated with the session.

				DoLoadByObject( theObj, id, true, lockMode );
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "refreshing " + MessageHelper.InfoString( e.Persister, e.Id ) );
				}

				if( !e.ExistsInDatabase )
				{
					throw new HibernateException( "this instance does not yet exist as a row in the database" );
				}

				persister = e.Persister;
				id = e.Id;
				Key key = new Key( id, persister );
				RemoveEntity( key );
				if( persister.HasCollections )
				{
					new EvictVisitor( this ).Process( obj, persister );
				}

				// this is from h2.1 - the code around here has some more interaction with the factory
				// and collection cache.
				if( persister.HasCache )
				{
					persister.Cache.Remove( id );
				}
				EvictCachedCollections( persister, id );

				// h2.1 has some differences with how it interacts with Load and errors thrown.
				try
				{
					e.Persister.Load( e.Id, theObj, lockMode, this );
				}
				catch( Exception exp )
				{
					throw new ADOException( "could not refresh object", exp );
				}
				GetEntry( theObj ).LockMode = e.LockMode;
			}
		}

		/// <summary>
		/// After processing a JDBC result set, we "resolve" all the associations
		/// between the entities which were instantiated and had their state
		/// "hydrated" into an array
		/// </summary>
		/// <param name="obj"></param>
		public void InitializeEntity( object obj )
		{
			EntityEntry e = GetEntry( obj );
			IClassPersister persister = e.Persister;
			object id = e.Id;
			object[ ] hydratedState = e.LoadedState;
			IType[ ] types = persister.PropertyTypes;

			if( log.IsDebugEnabled )
			{
				log.Debug( "resolving associations for: " + MessageHelper.InfoString( persister, id ) );
			}

			interceptor.OnLoad( obj, id, hydratedState, persister.PropertyNames, types );

			for( int i = 0; i < hydratedState.Length; i++ )
			{
				hydratedState[ i ] = types[ i ].ResolveIdentifier( hydratedState[ i ], this, obj );
			}

			persister.SetPropertyValues( obj, hydratedState );

			if( persister.HasCache )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "adding entity to second-level cache " + MessageHelper.InfoString( persister, id ) );
				}
				persister.Cache.Put( id, new CacheEntry( obj, persister, this ), timestamp );
			}

			// reentrantCallback=true;
			if( persister.ImplementsLifecycle )
			{
				( ( ILifecycle ) obj ).OnLoad( this, id );
			}

			// after setting values to object
			TypeFactory.DeepCopy( hydratedState, persister.PropertyTypes, persister.PropertyUpdateability, hydratedState );
			e.Status = Status.Loaded;

			// reentrantCallback=false;
			if( log.IsDebugEnabled )
			{
				log.Debug( "done materializing entity " + MessageHelper.InfoString( persister, id ) );
			}
		}

		/// <summary></summary>
		public ITransaction BeginTransaction( IsolationLevel isolationLevel )
		{
			transaction = factory.TransactionFactory.BeginTransaction( this, isolationLevel );
			isCurrentTransaction = true;
			return transaction;
		}

		/// <summary></summary>
		public ITransaction BeginTransaction()
		{
			transaction = factory.TransactionFactory.BeginTransaction( this );
			isCurrentTransaction = true;
			return transaction;
		}

		/// <summary></summary>
		public ITransaction Transaction
		{
			get { return transaction; }
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
			if( cascading > 0 )
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
			log.Debug( "flushing session" );

			interceptor.PreFlush( entitiesByKey.Values );

			PreFlushEntities();
			
			// We could move this inside if we wanted to
			// tolerate collection initializations during
			// collection dirty checking:
			PreFlushCollections();

			// Now, any collections that are initialized
			// inside this block do not get updated - they
			// are ignored until the next flush
			flushing = true;
			try
			{
				FlushEntities();
				FlushCollections();
			}
			finally
			{
				flushing = false;
			}

			//some stats

			if( log.IsDebugEnabled )
			{
				log.Debug( "Flushed: " +
					insertions.Count + " insertions, " +
					updates.Count + " updates, " +
					deletions.Count + " deletions to " +
					entityEntries.Count + " objects" );
				log.Debug( "Flushed: " +
					collectionCreations.Count + " (re)creations, " +
					collectionUpdates.Count + " updates, " +
					collectionRemovals.Count + " removals to " +
					collectionEntries.Count + " collections" );
			}
		}

		private bool AreTablesToBeUpdated( ISet tables )
		{
			return AreTablesToUpdated( updates, tables ) ||
				AreTablesToUpdated( insertions, tables ) ||
				AreTablesToUpdated( deletions, tables ) ||
				AreTablesToUpdated( collectionUpdates, tables ) ||
				AreTablesToUpdated( collectionCreations, tables ) ||
				AreTablesToUpdated( collectionRemovals, tables );
		}

		private bool AreTablesToUpdated( ICollection coll, ISet theSet )
		{
			foreach( IExecutable executable in coll )
			{
				object[ ] spaces = executable.PropertySpaces;
				for( int i = 0; i < spaces.Length; i++ )
				{
					if( theSet.Contains( spaces[ i ] ) )
					{
						if ( log.IsDebugEnabled )
						{
							log.Debug( string.Format( "changes must be flushed to space: {0}", spaces[ i ] ) );
						}
						return true;
					}
				}
			}
			return false;
		}

		private void Execute()
		{
			log.Debug( "executing flush" );

			try
			{
				// we need to lock the collection caches before
				// executing entity insert/updates in order to
				// account for bidi associations
				BeforeExecutionsAll( collectionRemovals );
				BeforeExecutionsAll( collectionUpdates );
				BeforeExecutionsAll( collectionCreations );

				// now actually execute SQL and update the
				// second-level cache
				ExecuteAll( insertions );
				ExecuteAll( updates );
				ExecuteAll( collectionRemovals );
				ExecuteAll( collectionUpdates );
				ExecuteAll( collectionCreations );
				ExecuteAll( deletions );
			}
			catch( Exception e )
			{
				throw new ADOException( "could not synchronize database state with session", e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void PostInsert( object obj )
		{
			GetEntry( obj ).ExistsInDatabase = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public void PostDelete( object obj )
		{
			EntityEntry e = RemoveEntry( obj );
			e.Status = Status.Gone;
			Key key = new Key( e.Id, e.Persister );
			RemoveEntity( key );
			proxiesByKey.Remove( key );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="updatedState"></param>
		/// <param name="nextVersion"></param>
		public void PostUpdate( object obj, object[ ] updatedState, object nextVersion )
		{
			EntityEntry e = GetEntry( obj );
			e.LoadedState = updatedState;
			e.LockMode = LockMode.Write;
			if( e.Persister.IsVersioned )
			{
				e.Version = nextVersion;
				e.Persister.SetPropertyValue( obj, e.Persister.VersionProperty, nextVersion );
			}
		}

		private void ExecuteInserts()
		{
			log.Info( "executing insertions" );
			ExecuteAll( insertions );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsDirty( )
		{
			log.Debug( "checking session dirtiness" );
			if ( insertions.Count > 0 || deletions.Count > 0 ) 
			{
				log.Debug( "session dirty (scheduled updates and insertions)" );
				return true;
			}
			else
			{
				int oldSize = collectionRemovals.Count;
				try
				{
					FlushEverything();
					bool result = updates.Count > 0 ||
						insertions.Count > 0 ||
						deletions.Count > 0 ||
						collectionUpdates.Count > 0 ||
						collectionRemovals.Count > 0 ||
						collectionCreations.Count > 0;
					log.Debug( result ? "session dirty" : "session not dirty" );
					return result;
				}
				finally
				{
					collectionCreations.Clear();
					collectionUpdates.Clear();
					updates.Clear();
					// collection deletions are a special case since update() can add
					// deletions of collections not loaded by the session.
					for ( int i = collectionRemovals.Count - 1; i >= oldSize; i-- )
					{
						collectionRemovals.RemoveAt( i );
					}
				}
			}
		}

		private void ExecuteAll( IList list )
		{
			foreach( IExecutable e in list )
			{
				if ( e.HasAfterTransactionCompletion )
				{
					executions.Add( e );
				}
				/*
				if ( lockQueryCache )
				{
					factory.GetUpdateTimestampsCache.PreInvalidate( e.PropertySpaces );
				}
				*/
				e.Execute();
			}
			list.Clear();

			if( batcher != null )
			{
				batcher.ExecuteBatch();
			}
		}

		private void BeforeExecutionsAll( IList list )
		{
			foreach( IExecutable e in list )
			{
				e.BeforeExecutions( );
			}
		}

		/// <summary>
		/// 1. detect any dirty entities
		/// 2. schedule any entity updates
		/// 3. search out any reachable collections
		/// </summary>
		private void FlushEntities()
		{
			log.Debug( "Flushing entities and processing referenced collections" );

			// Among other things, updateReachables() will recursively load all
			// collections that are moving roles. This might cause entities to
			// be loaded.

			// So this needs to be safe from concurrent modification problems.
			// It is safe because of how IdentityMap implements entrySet()

			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries( entityEntries );

			foreach( DictionaryEntry me in iterSafeCollection )
			{
				EntityEntry entry = ( EntityEntry ) me.Value;
				Status status = entry.Status;

				if( status != Status.Loading && status != Status.Gone )
				{
					FlushEntity(me.Key, entry);
				}
			}
		}

		private void FlushEntity( object obj, EntityEntry entry )
		{
			IClassPersister persister = entry.Persister;
			Status status = entry.Status;
			CheckId( obj, persister, entry.Id );

			object[ ] values;
			if( status == Status.Deleted )
			{
				//grab its state saved at deletion
				values = entry.DeletedState;
			}
			else
			{
				//grab its current state
				values = persister.GetPropertyValues( obj );
			}
			IType[ ] types = persister.PropertyTypes;

			bool substitute = false;

			if ( persister.HasCollections )
			{
				// wrap up any new collections directly referenced by the object
				// or its components

				// NOTE: we need to do the wrap here even if its not "dirty",
				// because nested collections need wrapping but changes to
				// _them_ don't dirty the container. Also, for versioned
				// data, we need to wrap before calling searchForDirtyCollections

				WrapVisitor visitor = new WrapVisitor( this );
				// substitutes into values by side-effect
				visitor.ProcessValues( values, types ); 
				substitute = visitor.IsSubstitutionRequired;
			}

			bool cannotDirtyCheck;
			bool interceptorHandledDirtyCheck;
			bool dirtyCheckDoneBySelect = false;
			object[] currentPersistentState = null;

			int[ ] dirtyProperties = interceptor.FindDirty( obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types );

			if( dirtyProperties == null )
			{
				// interceptor returned null, so do the dirtycheck ourself, if possible
				interceptorHandledDirtyCheck = false;
				cannotDirtyCheck = entry.LoadedState == null; // object loaded by update()
				if( !cannotDirtyCheck )
				{
					dirtyProperties = persister.FindDirty( values, entry.LoadedState, obj, this );
				}
				else
				{
					currentPersistentState = persister.GetCurrentPersistentState( entry.Id, entry.Version, this );
					if ( currentPersistentState != null )
					{
						dirtyProperties = persister.FindModified( currentPersistentState, values, obj, this );
						cannotDirtyCheck = false;
						dirtyCheckDoneBySelect = true;
					}
				}
			}
			else
			{
				// the interceptor handled the dirty checking
				cannotDirtyCheck = false;
				interceptorHandledDirtyCheck = true;
			}

			// compare to cached state (ignoring nested collections)
			if ( IsUpdateNecessary(persister, cannotDirtyCheck, status, dirtyProperties, values, types) )
			{
				// its dirty!

				if( log.IsDebugEnabled )
				{
					if( status == Status.Deleted )
					{
						log.Debug( "Updating deleted entity: " + MessageHelper.InfoString( persister, entry.Id ) );
					}
					else
					{
						log.Debug( "Updating entity: " + MessageHelper.InfoString( persister, entry.Id ) );
					}
				}

				if ( !entry.IsBeingReplicated )
				{
					// give the Interceptor a chance to modify property values
					bool intercepted = interceptor.OnFlushDirty(
						obj, entry.Id, values, entry.LoadedState, persister.PropertyNames, types );

					//now we might need to recalculate the dirtyProperties array
					if( intercepted && !cannotDirtyCheck && !interceptorHandledDirtyCheck )
					{
						if (dirtyCheckDoneBySelect )
						{
							dirtyProperties = persister.FindModified( currentPersistentState, values, obj, this );
						}
						else
						{
							dirtyProperties = persister.FindDirty( values, entry.LoadedState, obj, this );
						}
					}
					// if the properties were modified by the Interceptor, we need to set them back to the object
					substitute = substitute || intercepted;
				}

				// validate() instances of Validatable
				if( status == Status.Loaded && persister.ImplementsValidatable )
				{
					( ( IValidatable ) obj ).Validate();
				}

				//increment the version number (if necessary)
				object nextVersion = entry.Version;
				if( persister.IsVersioned )
				{
					if( status != Status.Deleted )
					{
						nextVersion = Versioning.Increment( entry.Version, persister.VersionType );
					}
					Versioning.SetVersion( values, nextVersion, persister );
				}

				// get the updated snapshot by cloning current state
				object[ ] updatedState = null;
				if( status == Status.Loaded )
				{
					updatedState = new object[values.Length];
					TypeFactory.DeepCopy( values, types, persister.PropertyUpdateability, updatedState );
				}

				// if it was dirtied by a collection only
				if ( !cannotDirtyCheck && dirtyProperties == null )
				{
					dirtyProperties = new int[] { };
				}

				CheckNullability( values, persister, true );

				//note that we intentionally did _not_ pass in currentPersistentState!
				updates.Add(
					new ScheduledUpdate( entry.Id, values, dirtyProperties, entry.LoadedState, entry.Version, nextVersion, obj, updatedState, persister, this )
					);
			}

			if( status == Status.Deleted )
			{
				//entry.status = Status.Gone;
			}
			else
			{
				// now update the object... has to be outside the main if block above (because of collections)
				if( substitute )
				{
					persister.SetPropertyValues( obj, values );
				}

				// search for collections by reachability, updating their role.
				// we don't want to touch collections reachable from a deleted object.
				if( persister.HasCollections ) 
				{
					new FlushVisitor( this, obj ).ProcessValues( values, types );
				}
			}
		}

		private bool IsUpdateNecessary( IClassPersister persister, bool cannotDirtyCheck, Status status, int[] dirtyProperties, 
			object[] values, IType[] types) 
		{
			if( persister.IsMutable==false ) return false;
			if( cannotDirtyCheck ) return true;

			if( dirtyProperties!=null && dirtyProperties.Length!=0 ) return true;

			if( status==Status.Loaded && persister.IsVersioned && persister.HasCollections )
			{
				DirtyCollectionSearchVisitor visitor = new DirtyCollectionSearchVisitor( this );
				visitor.ProcessValues( values, types );
				return visitor.WasDirtyCollectionFound;
			}
			else
			{
				return false;
			}
		}

		private static void CheckId( object obj, IClassPersister persister, object id )
		{
			// make sure user didn't mangle the id
			if( persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier )
			{
				object oid = persister.GetIdentifier( obj );
				if ( id == null )
				{
					throw new AssertionFailure( "null id in entry (don't flush the Session after an exception occurs)" );
				}

				if( !id.Equals( oid ) )
				{
					throw new HibernateException(
						"identiifier of an instance of " +
						persister.ClassName +
						" altered from " +
						id + "(" + id.GetType().ToString() + ")" +
						" to " + oid + "(" + oid.GetType().ToString() + ")"
						);
				}
			}
		}

		/// <summary>
		/// Process cascade save/update at the start of a flush to discover
		/// any newly referenced entity that must be passed to
		/// <see cref="SaveOrUpdate" /> and also apply orphan delete
		/// </summary>
		private void PreFlushEntities()
		{
			ICollection iterSafeCollection = IdentityMap.ConcurrentEntries( entityEntries );

			// so that we can be safe from the enumerator & concurrent modifications
			foreach( DictionaryEntry me in iterSafeCollection )
			{
				EntityEntry entry = ( EntityEntry ) me.Value;
				Status status = entry.Status;

				if( status != Status.Loading && status != Status.Gone && status != Status.Deleted )
				{
					object obj = me.Key;
					cascading++;
					try
					{
						Cascades.Cascade( this, entry.Persister, obj, Cascades.CascadingAction.ActionSaveUpdate, CascadePoint.CascadeOnUpdate, null );
					}
					finally
					{
						cascading--;
					}
				}
			}
		}

		// this just does a table lookup, but cacheds the last result

		[NonSerialized]
		private System.Type lastClass;

		[NonSerialized]
		private IClassPersister lastResultForClass;

		private IClassPersister GetPersister( System.Type theClass )
		{
			if( lastClass != theClass )
			{
				lastResultForClass = factory.GetPersister( theClass );
				lastClass = theClass;
			}
			return lastResultForClass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public IClassPersister GetPersister( object obj )
		{
			return GetPersister( obj.GetType() );
		}

		/// <summary>
		/// Not for internal use
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetIdentifier( object obj )
		{
			if( obj is INHibernateProxy )
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj );
				if( li.Session != this )
				{
					throw new TransientObjectException( "The proxy was not associated with this session" );
				}
				return li.Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry( obj );
				if( entry == null )
				{
					throw new TransientObjectException( "the instance was not associated with this session" );
				}
				return entry.Id;
			}
		}

		/// <summary>
		/// Get the id value for an object that is actually associated with the session.
		/// This is a bit stricter than getEntityIdentifierIfNotUnsaved().
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetEntityIdentifier( object obj )
		{
			if( obj is INHibernateProxy )
			{
				return NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj ).Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry( obj );
				return ( entry != null ) ? entry.Id : null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool IsSaved( object obj )
		{
			if( obj is INHibernateProxy )
			{
				return true;
			}

			EntityEntry entry = GetEntry( obj );
			if( entry != null )
			{
				return true;
			}

			object isUnsaved = interceptor.IsUnsaved( obj );
			if( isUnsaved != null )
			{
				return !( bool ) isUnsaved;
			}

			return !GetPersister( obj ).IsUnsaved( obj );
		}

		/// <summary>
		/// Used by OneToOneType and ManyToOneType to determine what id value
		/// should be used for an object that may or may not be associated with
		/// the session. This does a "best guess" using any/all info available
		/// to use (not just the EntityEntry).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object GetEntityIdentifierIfNotUnsaved( object obj )
		{
			if( obj == null )
			{
				return null;
			}

			if( obj is INHibernateProxy )
			{
				return NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj ).Identifier;
			}
			else
			{
				EntityEntry entry = GetEntry( obj );
				if( entry != null )
				{
					return entry.Id;
				}
				else
				{
					object isUnsaved = interceptor.IsUnsaved( obj );

					if( isUnsaved != null && ( ( bool ) isUnsaved ) )
					{
						ThrowTransientObjectException( obj );
					}

					IClassPersister persister = GetPersister( obj );
					if( persister.IsUnsaved( obj ) )
					{
						ThrowTransientObjectException( obj );
					}

					return persister.GetIdentifier( obj );
				}

			}
		}

		private static void ThrowTransientObjectException( object obj )
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

			log.Debug( "Processing unreferenced collections" );

			foreach( DictionaryEntry e in IdentityMap.ConcurrentEntries( collectionEntries ) )
			{
				CollectionEntry ce = (CollectionEntry) e.Value;
				if( ! ce.reached && ! ce.ignore )
				{
					UpdateUnreachableCollection( ( PersistentCollection ) e.Key );
					unreferencedCount++;
				}

			}
			log.Debug( "Processed " + unreferencedCount + " unreachable collections." );

			// schedule updates to collections:

			log.Debug( "scheduling collection removes/(re)creates/updates" );

			foreach( DictionaryEntry me in IdentityMap.ConcurrentEntries( collectionEntries ) )
			{
				PersistentCollection coll = ( PersistentCollection ) me.Key;
				CollectionEntry ce = ( CollectionEntry ) me.Value;

				// TODO: move this to the entry

				if( ce.dorecreate )
				{
					collectionCreations.Add( new ScheduledCollectionRecreate( coll, ce.currentPersister, ce.currentKey, this ) );
					recreateCount++;
				}
				if( ce.doremove )
				{
					collectionRemovals.Add( new ScheduledCollectionRemove( ce.loadedPersister, ce.loadedKey, ce.SnapshotIsEmpty, this ) );
					removeCount++;
				}

				if( ce.doupdate )
				{
					collectionUpdates.Add( new ScheduledCollectionUpdate( coll, ce.loadedPersister, ce.loadedKey, ce.SnapshotIsEmpty, this ) );
					updateCount++;
				}

				scheduledCount++;
			}

			log.Debug( "Processed " + scheduledCount + " for recreate (" + recreateCount + "), remove (" + removeCount + "), and update (" + updateCount + ")" );
		}


		private void PostFlush()
		{
			log.Debug( "post flush" );

			foreach( DictionaryEntry de in IdentityMap.ConcurrentEntries( collectionEntries ) )
			{
				( ( CollectionEntry ) de.Value ).PostFlush( ( PersistentCollection ) de.Key );
			}

			interceptor.PostFlush( entitiesByKey.Values );
		}

		private void PreFlushCollections()
		{
			// initialize dirty flags for arrays + collections with composte elements
			// and reset reached, doupdate, etc

			foreach( DictionaryEntry de in IdentityMap.ConcurrentEntries( collectionEntries ) )
			{
				CollectionEntry ce = ( CollectionEntry ) de.Value;
				PersistentCollection pc = ( PersistentCollection ) de.Key;

				ce.PreFlush( pc );
			}
		}

		/// <summary>
		/// Initialize the role of the collection.
		/// The CollectionEntry.reached stuff is just to detect any silly users who set up
		/// circular or shared references between/to collections.
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="type"></param>
		/// <param name="owner"></param>
		internal void UpdateReachableCollection( PersistentCollection coll, IType type, object owner )
		{
			CollectionEntry ce = GetCollectionEntry( coll );

			if( ce == null )
			{
				// refer to comment in addCollection()
				throw new HibernateException( string.Format( "Found two representations of same collection: {0}", coll.CollectionSnapshot.Role ) );
			}

			if( ce.reached )
			{
				// we've been here before
				throw new HibernateException( string.Format( "Found shared references to a collection: {0}", coll.CollectionSnapshot.Role ) );
			}
			ce.reached = true;

			ICollectionPersister persister = GetCollectionPersister( ( ( PersistentCollectionType ) type ).Role );
			ce.currentPersister = persister;
			ce.currentKey = GetEntityIdentifier( owner ); //TODO: better to pass the id in as an argument?

			if( log.IsDebugEnabled )
			{
				log.Debug(
					"Collection found: " + MessageHelper.InfoString( persister, ce.currentKey ) +
						", was: " + MessageHelper.InfoString( ce.loadedPersister, ce.loadedKey )
					);
			}

			PrepareCollectionForUpdate( coll, ce );
		}

		/// <summary>
		/// record the fact that this collection was dereferenced
		/// </summary>
		/// <param name="coll"></param>
		private void UpdateUnreachableCollection( PersistentCollection coll )
		{
			CollectionEntry entry = GetCollectionEntry( coll );

			if( log.IsDebugEnabled && entry.loadedPersister != null )
			{
				log.Debug( "collection dereferenced: " + MessageHelper.InfoString( entry.loadedPersister, entry.loadedKey ) );
			}

			if( entry.loadedPersister != null && entry.loadedPersister.HasOrphanDelete )
			{
				EntityEntry e = GetEntry( new Key( entry.loadedKey, GetPersister( entry.loadedPersister.OwnerClass ) ) );

				// only collections belonging to deleted entities are allowed to be dereferenced in 
				// the case of orphan delete
				if( e != null && e.Status != Status.Deleted && e.Status != Status.Gone )
				{
					throw new HibernateException( "You may not dereference an collection with cascade=\"all-delete-orphan\"" );
				}
			}

			entry.currentPersister = null;
			entry.currentKey = null;

			PrepareCollectionForUpdate( coll, entry );
		}

		/// <summary>
		/// 1. record the collection role that this collection is referenced by
		/// 2. decide if the collection needs deleting/creating/updating (but
		///    don't actually schedule the action yet)
		/// </summary>
		/// <param name="coll"></param>
		/// <param name="entry"></param>
		private void PrepareCollectionForUpdate( PersistentCollection coll, CollectionEntry entry )
		{
			if( entry.processed )
			{
				throw new AssertionFailure( "collection was processed twice by flush()" );
			}

			entry.processed = true;

			// it is or was referenced _somewhere_
			if( entry.loadedPersister != null || entry.currentPersister != null )
			{
				if(
					entry.loadedPersister != entry.currentPersister || //if either its role changed,
						!entry.currentPersister.KeyType.Equals( entry.loadedKey, entry.currentKey ) // or its key changed
					)
				{
					// do a check
					if( entry.loadedPersister != null && entry.currentPersister != null && entry.loadedPersister.HasOrphanDelete )
					{
						throw new HibernateException( "You may not change the reference to a collection with cascade=\"all-delete-orphan\"" );
					}

					// do the work
					if( entry.currentPersister != null )
					{
						entry.dorecreate = true; //we will need to create new entry
					}

					if( entry.loadedPersister != null )
					{
						entry.doremove = true; // we will need to remove the old entres
						if( entry.dorecreate )
						{
							log.Debug( "forcing collection initialization" );
							coll.ForceInitialization();
						}
					}
				}
				else if( entry.Dirty )
				{ // else if it's elements changed
					entry.doupdate = true;
				}
			}
		}

		/// <summary>
		/// ONLY near the end of the flush process, determine if the collection is dirty
		/// by checking its entry
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		internal bool CollectionIsDirty( PersistentCollection coll )
		{
			CollectionEntry entry = GetCollectionEntry( coll );
			return entry.initialized && entry.Dirty; //( entry.dirty || coll.hasQueuedAdds() ); 
		}

		private IDictionary loadingCollections = new Hashtable();

		private sealed class LoadingCollectionEntry
		{
			private readonly CollectionKey key;
			private readonly PersistentCollection collection;
			private readonly object id;
			private readonly object resultSetId;

			internal LoadingCollectionEntry( CollectionKey key, PersistentCollection collection, object id, object resultSetId )
			{
				this.key = key;
				this.collection = collection;
				this.id = id;
				this.resultSetId = resultSetId;
			}

			public CollectionKey Key
			{
				get { return key; }
			}

			public PersistentCollection Collection
			{
				get { return collection; }
			}

			public object Id
			{
				get { return id; }
			}

			public object ResultSetId
			{
				get { return resultSetId; }
			}
		}


		private LoadingCollectionEntry GetLoadingCollectionEntry( CollectionKey collectionKey )
		{
			return (LoadingCollectionEntry) loadingCollections[ collectionKey ] ;
		}

		private void AddLoadingCollectionEntry( CollectionKey key, PersistentCollection collection, object id, object resultSetId )
		{
			loadingCollections.Add( key, new LoadingCollectionEntry( key, collection, id, resultSetId ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="resultSetId"></param>
		/// <returns></returns>
		public PersistentCollection GetLoadingCollection( ICollectionPersister persister, object id, object resultSetId )
		{
			CollectionKey ckey = new CollectionKey( persister, id );
			LoadingCollectionEntry lce = GetLoadingCollectionEntry( ckey );
			if( lce == null )
			{
				// look for existing collection
				PersistentCollection pc = GetCollection( ckey );
				if ( pc != null )
				{
					CollectionEntry ce = GetCollectionEntry( pc );
					if ( ce.initialized )
					{
						log.Info( "collection already initialized: ignoring" );
						return null; //ignore this row of results! Note the early exit
					}
					else
					{
						log.Info( "uninitialized collection: initializing" );
					}
				}
				else
				{
					object entity = GetCollectionOwner( id, persister );
					// TODO: 2.1 - Has the following code, but it breaks MasterDetailTest.MasterDetail as the entities are loaded, but the collection is not instatiated
					if ( entity != null && GetEntry( entity ).Status != Status.Loading )
					{
						// important, to account for newly saved entities in query
						log.Info( "owning entity already loaded: ignoring" );
						return null;
					}
					else
					{
						//create one
						log.Info( "new collection: instantiating" );
						pc = persister.CollectionType.Instantiate( this, persister );
					}
				}
				pc.BeforeInitialize( persister );
				pc.BeginRead();
				AddLoadingCollectionEntry( ckey, pc, id, resultSetId );
				return pc;
			}
			else
			{
				if ( lce.ResultSetId == resultSetId )
				{
					log.Info( "reading row" );
					return lce.Collection;
				}
				else
				{
					//ignore this row, the collection is in process of being loaded somewhere further "up" the stack
					log.Info( "collection is already being initialized: ignoring row" );
					return null;
				}
			}
		}

		/// <summary></summary>
		public void EndLoadingCollections( ICollectionPersister persister, object resultSetId )
		{
			// scan the loading collections for collections from this result set
			// put them in a new temp collection so that we are safe from concurrent
			// modification when the call to endRead() causes a proxy to be
			// initialized
			IList resultSetCollections = new ArrayList();
			foreach( LoadingCollectionEntry lce in loadingCollections.Values )
			{
				if ( lce.ResultSetId == resultSetId )
				{
					resultSetCollections.Add( lce );
				}
			}

			// Remove them from the original collection - expanded LoadingCollectionEntry to know it's key to do this
			foreach ( LoadingCollectionEntry lce in resultSetCollections )
			{
				loadingCollections.Remove( lce.Key );
			}

			EndLoadingCollections( persister, resultSetCollections );
		}

		private void EndLoadingCollections( ICollectionPersister persister, IList resultSetCollections )
		{
			int count = resultSetCollections.Count;

			if ( log.IsDebugEnabled )
			{
				log.Debug( string.Format( "{0} collections were found in result set", count ) );
			}

			// now finish them
			for ( int i = 0; i < count; i++ )
			{
				LoadingCollectionEntry lce = (LoadingCollectionEntry) resultSetCollections[ i ];
				bool noQueueAdds = lce.Collection.EndRead(); // warning: can cause recursive query! (proxy initialization)
				CollectionEntry ce = GetCollectionEntry( lce.Collection );
				if ( ce == null )
				{
					ce = AddInitializedCollection( lce.Collection, persister, lce.Id );
				}
				else
				{
					ce.PostInitialize( lce.Collection );
				}

				if ( noQueueAdds && persister.HasCache && !ce.doremove )
				{
					if ( log.IsDebugEnabled )
					{
						log.Debug( "caching collection: " + MessageHelper.InfoString( persister, lce.Id ) );
					}
					// TODO: 2.1 - implemented versioned cache
					/*
					IClassPersister ownerPersister = factory.GetPersister( persister.OwnerClass );
					object version;
					if ( ownerPersister.IsVersioned ) 
					{
						version = GetEntry( GetCollectionOwner( ce ) ).Version;
					}
					else
					{
						version = null;
					}
					persister.Cache.Put( lce.Id, lce.Collection.Disassemble( persister ), Timestamp, version );
					*/
					persister.Cache.Put( lce.Id, lce.Collection.Disassemble( persister ), Timestamp );
				}
				if ( log.IsDebugEnabled )
				{
					log.Debug( "collection fully initialized: " + MessageHelper.InfoString( persister, lce.Id ) );
				}
			}

			if ( log.IsDebugEnabled )
			{
				log.Debug( string.Format( "{0} collections initialized", count ) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="role"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public PersistentCollection GetLoadingCollection( string role, object id )
		{
			LoadingCollectionEntry lce = GetLoadingCollectionEntry( new CollectionKey( role, id ) );
			return ( lce != null ) ? lce.Collection : null;
		}

		private void AddCollection( PersistentCollection collection, CollectionEntry entry, object key )
		{
			collectionEntries[collection] = entry;

			CollectionKey ck = new CollectionKey( entry.loadedPersister, key);
			PersistentCollection old = (PersistentCollection) collectionsByKey[ck];
			collectionsByKey[ck] = collection;

			if (old != null)
			{
				if (old == collection) throw new AssertionFailure("collection added twice");
				// or should it actually throw an exception?
				old.UnsetSession(this);
				collectionEntries.Remove(old);
			}
		}

		public void BeforeLoad()
		{
			loadCounter++;
		}

		public void AfterLoad()
		{
			loadCounter--;
		}

		public void InitializeNonLazyCollections()
		{
			if( loadCounter==0 )
			{
				log.Debug( "initializing non-lazy collections" );
				// Do this work only at the very highest level of the load
				
				// Don't let this method be called recursively
				loadCounter++;
				try 
				{
					while ( nonlazyCollections.Count > 0 )
					{
						//note that each iteration of the loop may add new elements
						PersistentCollection collection = (PersistentCollection) nonlazyCollections[ nonlazyCollections.Count - 1 ];
						nonlazyCollections.RemoveAt( nonlazyCollections.Count - 1 );
						collection.ForceInitialization();
					}
				}
				finally 
				{
					loadCounter--;
				}
			}
		}

		private PersistentCollection GetCollection(CollectionKey key)
		{
			return (PersistentCollection) collectionsByKey[key];
		}

		/// <summary>
		/// add a collection we just loaded up (still needs initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		private void AddUninitializedCollection( PersistentCollection collection, ICollectionPersister persister, object id )
		{
			CollectionEntry ce = new CollectionEntry( persister, id, flushing );
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);
		}

		private void AddUninitializedDetachedCollection( PersistentCollection collection, ICollectionPersister persister, object id )
		{
			CollectionEntry ce = new CollectionEntry( persister, id );
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);
		}

		/// <summary>
		/// add a collection we just pulled out of the cache (does not need initializing)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		public CollectionEntry AddInitializedCollection( PersistentCollection collection, ICollectionPersister persister, object id )
		{
			CollectionEntry ce = new CollectionEntry( persister, id, flushing );
			ce.PostInitialize( collection );
			collection.CollectionSnapshot = ce;
			AddCollection(collection, ce, id);

			return ce;
		}

		private CollectionEntry AddCollection( PersistentCollection collection )
		{
			CollectionEntry ce = new CollectionEntry();
			collectionEntries[collection] = ce;
			collection.CollectionSnapshot = ce;
			return ce;
		}

		/// <summary>
		/// Add a new collection (i.e. a newly created one, just instantiated by
		/// the application, with no database state or snapshot)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="persister"></param>
		internal void AddNewCollection(PersistentCollection collection, ICollectionPersister persister)
		{
			CollectionEntry ce = AddCollection(collection);
			if ( persister.HasOrphanDelete ) ce.InitSnapshot(collection, persister);

		}

		/// <summary>
		/// Add an (initialized) collection that was created by another session and passed
		/// into update() (i.e. one with a snapshot and existing state on the database)
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="cs"></param>
		private void AddInitializedDetachedCollection( PersistentCollection collection, ICollectionSnapshot cs )
		{
			if ( cs.WasDereferenced )
			{
				AddCollection(collection);
			}
			else
			{
				CollectionEntry ce = new CollectionEntry(cs, factory);
				collection.CollectionSnapshot = ce;
				AddCollection( collection, ce, cs.Key );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public ArrayHolder GetArrayHolder( object array )
		{
			return ( ArrayHolder ) arrayHolders[ array ];
		}

		//must call after loading array (so array exists for key of map);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="holder"></param>
		public void AddArrayHolder( ArrayHolder holder )
		{
			arrayHolders[ holder.Array ] = holder;
		}

		internal ICollectionPersister GetCollectionPersister( string role )
		{
			return factory.GetCollectionPersister( role );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		public void Dirty( PersistentCollection coll )
		{
			GetCollectionEntry( coll ).SetDirty();
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public object GetSnapshot( PersistentCollection coll )
		{
			return GetCollectionEntry( coll ).snapshot;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public object GetLoadedCollectionKey( PersistentCollection coll )
		{
			return GetCollectionEntry( coll ).loadedKey;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public bool IsInverseCollection( PersistentCollection collection )
		{
			CollectionEntry ce = GetCollectionEntry( collection );
			return ce != null && ce.loadedPersister.IsInverse;
		}


		private static readonly ICollection EmptyCollection = new ArrayList( 0 );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="coll"></param>
		/// <returns></returns>
		public ICollection GetOrphans( PersistentCollection coll )
		{
			CollectionEntry ce = GetCollectionEntry( coll );
			return ce.IsNew ? EmptyCollection : coll.GetOrphans( ce.Snapshot );
		}

		/// <summary>
		/// called by a collection that wants to initialize itself
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="writing"></param>
		public void InitializeCollection( PersistentCollection collection, bool writing )
		{
			CollectionEntry ce = GetCollectionEntry( collection );
			
			if ( ce == null )
			{
				throw new HibernateException( "collection was evicted" );
			}

			if( !ce.initialized )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "initializing collection " + MessageHelper.InfoString( ce.loadedPersister, ce.loadedKey ) );
				}

				log.Debug( "checking second-level cache" );

				bool foundInCache = InitializeCollectionFromCache(ce.loadedKey, GetCollectionOwner(ce), ce.loadedPersister, collection);
				
				if (foundInCache) 
				{
					log.Debug("collection initialized from cache");
				}
				else 
				{
					log.Debug("collection not cached");
					// TODO: 2.1+ - Get the owner argument here
					ce.loadedPersister.Initialize( ce.loadedKey, null, this );
					log.Debug("collection initialized");
				}
			}
		}

		private object GetCollectionOwner(CollectionEntry ce)
		{
			return GetCollectionOwner(ce.loadedKey, ce.loadedPersister);
		}

		public object GetCollectionOwner(object key, ICollectionPersister collectionPersister)
		{
			//TODO: Give collection persister a reference to the owning class persister
			return GetEntity( new Key(key, factory.GetPersister( collectionPersister.OwnerClass ) ) );
		}

		/// <summary></summary>
		public IDbConnection Connection
		{
			get
			{
				if( connection == null )
				{
					if( connect )
					{
						connection = factory.OpenConnection();
						connect = false;
					}
					else
					{
						throw new HibernateException( "session is currently disconnected" );
					}
				}
				return connection;
			}
		}

		/// <summary>
		/// Gets if the ISession is connected.
		/// </summary>
		/// <value>
		/// <c>true</c> if the ISession is connected.
		/// </value>
		/// <remarks>
		/// An ISession is considered connected if there is an <see cref="IDbConnection"/> (regardless
		/// of its state) or if it the field <c>connect</c> is true.  Meaning that it will connect
		/// at the next operation that requires a connection.
		/// </remarks>
		public bool IsConnected
		{
			get { return connection != null || connect; }
		}

		/// <summary></summary>
		public IDbConnection Disconnect()
		{
			log.Debug( "disconnecting session" );

			try
			{
				// the Session is flagged as needing to create a Connection for the
				// next operation
				if( connect )
				{
					// a Disconnected Session should not automattically "connect"
					connect = false;
					return null;
				}
				else
				{
					if( connection == null )
					{
						throw new HibernateException( "session already disconnected" );
					}

					if( batcher != null )
					{
						batcher.CloseCommands();
					}
					
					// get a new reference to the the Session's connection before 
					// closing it - and set the existing to Session's connection to
					// null but don't close it yet
					IDbConnection c = connection;
					connection = null;
					
					// if Session is supposed to auto-close the connection then
					// the Sesssion is managing the IDbConnection. 
					if( autoClose )
					{
						// let the SessionFactory close it and return null
						// because the connection is internal to the Session
						factory.CloseConnection( c );
						return null;
					}
					else
					{
						// return the connection the user provided - at this point
						// it has been disassociated with the NHibernate session. 
						return c;
					}
				}
			}
			finally
			{
				// ensure that AfterTransactionCompletion gets called since
				// it takes care of the Locks and Cache.
				if( !isCurrentTransaction )
				{
					// We don't know the state of the transaction
					AfterTransactionCompletion( false );
				}
			}
		}

		/// <summary></summary>
		public void Reconnect()
		{
			if( !IsOpen )
			{
				throw new HibernateException( "Session is closed" );
			}

			if( IsConnected )
			{
				throw new HibernateException( "session already connected" );
			}

			log.Debug( "reconnecting session" );

			connect = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		public void Reconnect( IDbConnection conn )
		{
			if( IsConnected )
			{
				throw new HibernateException( "session already connected" );
			}
			this.connection = conn;
		}

		#region System.IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~SessionImpl()
		{
			Dispose( false );
		}

		/// <summary>
		/// Just in case the user forgot to Commit() or Close()
		/// </summary>
		public void Dispose()
		{
			log.Debug( "running ISession.Dispose()" );
			Dispose( true );
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this Session is being Disposed of or Finalized.</param>
		/// <remarks>
		/// If this Session is being Finalized (<c>isDisposing==false</c>) then make sure not
		/// to call any methods that could potentially bring this Session back to life.
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if( _isAlreadyDisposed )
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the session if we
			// know this call came through Dispose()
			if( isDisposing )
			{

				if( transaction != null )
				{
					transaction.Dispose();
				}

				if( batcher != null )
				{
					batcher.Dispose();
				}

				// we are not reusing the Close() method because that sets the connection==null
				// during the Close() - if the connection is null we can't get to it to Dispose
				// of it.
				if( connection != null )
				{
					if ( connection.State == ConnectionState.Closed )
					{
						log.Warn( "finalizing unclosed session with closed connection" );
					}
					else
					{
						// if the Session is responsible for managing the connection then make sure
						// the connection is disposed of.
						if( autoClose )
						{
							factory.CloseConnection( connection );
						}
					}
				}
				// it is important to call Cleanup because that marks the Session as being
				// closed - the Session could still be associated with a Proxy that is attempting
				// to be reassociated with another Session.  If the Proxy sees ISession.IsOpen==true
				// then an exception will be thrown for trying to associate it with 2 open sessions.

				Cleanup();	
			}

			// free unmanaged resources here
			
			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize( this );
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public ICollection Filter( object collection, string filter )
		{
			QueryParameters qp = new QueryParameters( new IType[1], new object[1] );
			return Filter( collection, filter, qp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public ICollection Filter( object collection, string filter, object value, IType type )
		{
			QueryParameters qp = new QueryParameters( new IType[ ] {null, type}, new object[ ] {null, value} );
			return Filter( collection, filter, qp );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public ICollection Filter( object collection, string filter, object[ ] values, IType[ ] types )
		{
			object[ ] vals = new object[values.Length + 1];
			IType[ ] typs = new IType[values.Length + 1];
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
		private FilterTranslator GetFilterTranslator( object collection, string filter, QueryParameters parameters, bool scalar )
		{
			if( collection == null ) throw new ArgumentNullException( "collection", "null collection passed to Filter" );

			if( log.IsDebugEnabled )
			{
				log.Debug( "filter: " + filter );
				parameters.LogParameters();
			}

			CollectionEntry entry = GetCollectionEntryOrNull( collection );
			ICollectionPersister roleBeforeFlush = ( entry == null ) ? null : entry.loadedPersister;

			FilterTranslator filterTranslator;
			if( roleBeforeFlush == null )
			{
				// if it was previously unreferenced, we need
				// to flush in order to get its state into the
				// database to query
				Flush();
				entry = GetCollectionEntryOrNull( collection );
				ICollectionPersister roleAfterFlush = ( entry == null ) ? null : entry.loadedPersister;

				if( roleAfterFlush == null )
				{
					throw new QueryException( "the collection was unreferenced" );
				}
				filterTranslator = factory.GetFilter( filter, roleAfterFlush.Role, scalar );
			}
			else
			{
				// otherwise, we only need to flush if there are
				// in-memory changes to the queried tables
				filterTranslator = factory.GetFilter( filter, roleBeforeFlush.Role, scalar );
				if( AutoFlushIfRequired( filterTranslator.QuerySpaces ) )
				{
					// might need to run a different filter entirely after the flush
					// because the collection role may have changed
					entry = GetCollectionEntryOrNull( collection );
					ICollectionPersister roleAfterFlush = ( entry == null ) ? null : entry.loadedPersister;
					if( roleBeforeFlush != roleAfterFlush )
					{
						if( roleAfterFlush == null ) throw new QueryException( "The collection was dereferenced" );
					}
					filterTranslator = factory.GetFilter( filter, roleAfterFlush.Role, scalar );
				}
			}

			parameters.PositionalParameterValues[ 0 ] = entry.loadedKey;
			parameters.PositionalParameterTypes[ 0 ] = entry.loadedPersister.KeyType;

			return filterTranslator;

		}

		/// <summary>
		/// Get the collection entry for a collection passed to filter,
		/// which might be a collection wrapper, an array, or an unwrapped
		/// collection. Return <c>null</c> if there is no entry.
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		private CollectionEntry GetCollectionEntryOrNull( object collection )
		{
			PersistentCollection coll;
			if ( collection is PersistentCollection )
			{
				coll = (PersistentCollection) collection;
			}
			else
			{
				coll = GetArrayHolder( collection );
				if ( coll == null )
				{
					// it might be an unwrapped collection reference!
					// try to find a wrapper (slowish)
					foreach( PersistentCollection pc in collectionEntries.Keys )
					{
						if( pc.IsWrapper( collection ) )
						{
							coll = pc;
							break;
						}
					}
				}
			}

			return (coll == null) ? null : GetCollectionEntry( coll );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IList Filter( object collection, string filter, QueryParameters parameters )
		{
			string[ ] concreteFilters = QueryTranslator.ConcreteQueries( filter, factory );
			FilterTranslator[ ] filters = new FilterTranslator[concreteFilters.Length];

			for( int i = 0; i < concreteFilters.Length; i++ )
			{
				filters[ i ] = GetFilterTranslator(
					collection,
					concreteFilters[ i ],
					parameters,
					false );

			}

			dontFlushFromFind++; // stops flush being called multiple times if this method is recursively called

			IList results = new ArrayList();
			try
			{
				for( int i = 0; i < concreteFilters.Length; i++ )
				{
					IList currentResults;
					try
					{
						currentResults = filters[ i ].List( this, parameters );
					}
					catch( Exception e )
					{
						throw new ADOException( "could not execute query", e );
					}
					foreach( object res in results )
					{
						currentResults.Add( res );
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public IEnumerable EnumerableFilter( object collection, string filter, QueryParameters parameters )
		{
			string[ ] concreteFilters = QueryTranslator.ConcreteQueries( filter, factory );
			FilterTranslator[ ] filters = new FilterTranslator[concreteFilters.Length];

			for( int i = 0; i < concreteFilters.Length; i++ )
			{
				filters[ i ] = GetFilterTranslator(
					collection,
					concreteFilters[ i ],
					parameters,
					true );
			}

			if( filters.Length == 0 )
			{
				return new ArrayList( 0 );
			}

			IEnumerable result = null;
			IEnumerable[ ] results = null;
			bool many = filters.Length > 1;
			if( many )
			{
				results = new IEnumerable[filters.Length];
			}

			// execute the queries and return all results as a single enumerable
			for( int i = 0; i < filters.Length; i++ )
			{
				try
				{
					result = filters[ i ].GetEnumerable( parameters, this );
				}
				catch( Exception e )
				{
					throw new ADOException( "could not execute query", e );
				}
				if( many )
				{
					results[ i ] = result;
				}
			}

			return many ? new JoinedEnumerable( results ) : result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public ICriteria CreateCriteria( System.Type persistentClass )
		{
			return new CriteriaImpl( persistentClass, this );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		public IList Find( CriteriaImpl criteria )
		{
			System.Type persistentClass = criteria.PersistentClass;

			if( log.IsDebugEnabled )
			{
				log.Debug( "search: " + persistentClass.Name );
				log.Debug( "criteria: " + criteria );
			}

			IOuterJoinLoadable persister = GetPersister( persistentClass ) as IOuterJoinLoadable;
			CriteriaLoader loader = new CriteriaLoader( persister, factory, criteria );
			object[ ] spaces = persister.PropertySpaces;
			HashedSet sett = new HashedSet();
			for( int i = 0; i < spaces.Length; i++ )
			{
				sett.Add( spaces[ i ] );
			}
			AutoFlushIfRequired( sett );

			dontFlushFromFind++;
			try
			{
				return loader.List( this );
			}
			catch( Exception e )
			{
				throw new ADOException( "problem in find", e );
			}
			finally
			{
				dontFlushFromFind--;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Contains( object obj )
		{
			if( obj is INHibernateProxy )
			{
				return NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj ).Session == this;
			}
			else
			{
				return entityEntries.Contains( obj );
			}
		}

		/// <summary>
		/// remove any hard references to the entity that are held by the infrastructure
		/// (references held by application or other persistant instances are okay)
		/// </summary>
		/// <param name="obj"></param>
		public void Evict( object obj )
		{
			if( obj is INHibernateProxy )
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) obj );
				object id = li.Identifier;
				IClassPersister persister = GetPersister( li.PersistentClass );
				Key key = new Key( id, persister );
				proxiesByKey.Remove( key );
				if( !li.IsUninitialized )
				{
					object entity = RemoveEntity( key );
					if( entity != null )
					{
						RemoveEntry( entity );
						DoEvict( persister, entity );
					}
				}
			}
			else
			{
				EntityEntry e = RemoveEntry( obj );
				if( e != null )
				{
					RemoveEntity( new Key( e.Id, e.Persister ) );
					DoEvict( e.Persister, obj );
				}
			}
		}

		private void DoEvict( IClassPersister persister, object obj )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "evicting " + MessageHelper.InfoString( persister ) );
			}

			//remove all collections for the entity from the session-level cache
			if( persister.HasCollections )
			{
				new EvictVisitor(this).Process( obj, persister );
			}
			
			Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionEvict, CascadePoint.CascadeOnEvict, null );
		}

		internal void EvictCollection(object value, PersistentCollectionType type)
		{
			object pc;
			if( type.IsArrayType )
			{
				pc = arrayHolders[ value ];
				arrayHolders.Remove(value);
			}
			else if ( value is PersistentCollection )
			{
				pc = value;
			}
			else
			{
				return; //EARLY EXIT!
			}

			PersistentCollection collection = pc as PersistentCollection;
			if( collection.UnsetSession( this) )
			{
				EvictCollection( collection );
			}

		}

		private void EvictCollection(PersistentCollection collection)
		{
			CollectionEntry ce = (CollectionEntry)collectionEntries[collection];
			collectionEntries.Remove( collection );
			if( log.IsDebugEnabled )
			{
				log.Debug( "evicting collection: " + MessageHelper.InfoString( ce.loadedPersister, ce.loadedKey ) );
			}
			if( ce.loadedPersister!=null && ce.loadedKey!=null )
			{
				//TODO: is this 100% correct?
				collectionsByKey.Remove( new CollectionKey( ce.loadedPersister.Role, ce.loadedKey ) );
			}
		}

		
		private void EvictCachedCollections(IClassPersister persister, object id)
		{
			EvictCachedCollections( persister.PropertyTypes, id );
		}

		private void EvictCachedCollections(IType[] types, object id)
		{
			foreach( IType type in types )
			{
				if( type.IsPersistentCollectionType )
				{
					factory.EvictCollection( ( (PersistentCollectionType) type).Role, id );
				}
				else if ( type.IsComponentType )
				{
					IAbstractComponentType acType = (IAbstractComponentType)type;
					EvictCachedCollections( acType.Subtypes, id );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public object GetVersion( object entity )
		{
			return GetEntry( entity ).Version;
		}

		/// <summary>
		/// Instantiate a collection wrapper (called when loading an object)
		/// </summary>
		/// <param name="role"></param>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public object GetCollection( string role, object id, object owner )
		{
			// note: there cannot possibly be a collection already registered,
			// because this method is called while first loading the entity
			// that references it

			ICollectionPersister persister = factory.GetCollectionPersister( role );
			PersistentCollection collection = GetLoadingCollection( role, id );

			if (collection != null)
			{
				if ( log.IsDebugEnabled ) 
				{
					log.Debug( "returning loading collection:"
						+ MessageHelper.InfoString(persister, id) );
				}
				return collection.GetValue();
			}
			else 
			{
				if ( log.IsDebugEnabled ) 
				{
					log.Debug( "creating collection wrapper:" + MessageHelper.InfoString(persister, id) );
				}
				//TODO: suck into CollectionPersister.instantiate()
				collection = persister.CollectionType.Instantiate(this, persister);
				AddUninitializedCollection(collection, persister, id);
				if ( persister.IsArray ) 
				{
					InitializeCollection(collection, false);
					AddArrayHolder( (ArrayHolder) collection );
				}
				else if ( !persister.IsLazy ) 
				{
					nonlazyCollections.Add(collection);
				}
				return collection.GetValue();
			}
		}

		/// <summary>
		/// Try to initialize a Collection from the cache.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="owner"></param>
		/// <param name="persister"></param>
		/// <param name="collection"></param>
		/// <returns><c>true</c> if the collection was initialized from the cache, otherwise <c>false</c>.</returns>
		private bool InitializeCollectionFromCache(object id, object owner, ICollectionPersister persister, PersistentCollection collection)
		{
			if( persister.HasCache==false )
			{
				return false;
			}

			object cached = persister.Cache.Get( id, Timestamp );
			if( cached==null )
			{
				return false;
			}

			collection.InitializeFromCache( persister, cached, owner );
			GetCollectionEntry( collection ).PostInitialize(  collection  );
			//addInitializedCollection(collection, persister, id); h2.1 - commented out
			return true;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object SavOrUpdateCopy( object obj )
		{
			return SaveOrUpdateCopy( obj, null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object SaveOrUpdateCopy( object obj, object id )
		{
			return DoCopy( obj, id, new Hashtable( 10 ) );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collectionPersister"></param>
		/// <param name="id"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public object[] GetCollectionBatch( ICollectionPersister collectionPersister, object id, int batchSize )
		{
			object[] keys = new object[ batchSize ];
			keys[ 0 ] = id;
			int i = 0;
			foreach ( CollectionEntry ce in collectionEntries )
			{
				if ( !ce.initialized && ce.loadedPersister == collectionPersister && !id.Equals( ce.loadedKey ) )
				{
					keys[ ++i ] = ce.loadedKey;
					if ( i == batchSize - 1 )
					{
						return keys;
					}
				}
			}
			return keys;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		/// <param name="batchSize"></param>
		/// <returns></returns>
		public object[] GetClassBatch( System.Type clazz, object id, int batchSize )
		{
			object[] ids = new object[ batchSize ];
			ids[ 0 ] = id;
			int i = 0;
			foreach ( Key key in batchLoadableEntityKeys.Keys )
			{
				if ( key.MappedClass == clazz && !id.Equals( key.Identifier ) )
				{
					ids[ ++i ] = key.Identifier ;
					if ( i == batchSize - 1)
					{
						return ids;
					}
				}
			}
			return ids;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="id"></param>
		public void ScheduleBatchLoad( System.Type clazz, object id )
		{
			IClassPersister persister = GetPersister( clazz );
			if ( persister.IsBatchLoadable )
			{
				batchLoadableEntityKeys.Add( new Key( id, persister ), Marker );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="returnAlias"></param>
		/// <param name="returnClass"></param>
		/// <returns></returns>
		public IQuery CreateSQLQuery( string sql, string returnAlias, System.Type returnClass )
		{
			return new SqlQueryImpl( sql, new string[] { returnAlias }, new System.Type[] { returnClass }, this, null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="returnAliases"></param>
		/// <param name="returnClasses"></param>
		/// <returns></returns>
		public IQuery CreateSQLQuery( string sql, string[] returnAliases, System.Type[] returnClasses )
		{
			return new SqlQueryImpl( sql, returnAliases, returnClasses, this, null );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="returnAliases"></param>
		/// <param name="returnClasses"></param>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		public IQuery CreateSQLQuery( string sql, string[] returnAliases, System.Type[] returnClasses, ICollection querySpaces )
		{
			return new SqlQueryImpl( sql, returnAliases, returnClasses, this, querySpaces );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="aliases"></param>
		/// <param name="classes"></param>
		/// <param name="queryParameters"></param>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		public IList FindBySQL( string sqlQuery, string[] aliases, System.Type[] classes, QueryParameters queryParameters, ICollection querySpaces )
		{
			if ( log.IsInfoEnabled )
			{
				log.Info( "SQL Query: " + sqlQuery );
			}

			SqlType[] positionalTypes = new SqlType[ queryParameters.PositionalParameterTypes.Length ];
			for ( int i = 0; i < queryParameters.PositionalParameterTypes.Length; i++ )
			{
				IType type = queryParameters.PositionalParameterTypes[ i ] ;
				positionalTypes[ i ] = type.SqlTypes( factory )[ 0 ];
			}

			IDictionary namedTypes = new Hashtable( queryParameters.NamedParameters.Count );
			foreach( DictionaryEntry de in queryParameters.NamedParameters )
			{
				string name = (string) de.Key;
				TypedValue typedval = (TypedValue) de.Value;
				IType type = typedval.Type;
				namedTypes[ name ] = type.SqlTypes( factory )[ 0 ];
			}

			ISqlLoadable[] persisters = new ISqlLoadable[ classes.Length ] ;
			for ( int i = 0; i < classes.Length; i++ )
			{
				persisters[ i ] = GetSqlLoadable( classes[ i ] );
			}

			// TODO: 2.1+ We could cache these
			// NB In NHibernate we need the queryParameters to obtain the types of positional parameters
			SqlLoader loader = new SqlLoader( aliases, persisters, factory, sqlQuery, querySpaces, positionalTypes, namedTypes );
			
			AutoFlushIfRequired( loader.QuerySpaces );

			// TODO: Do we need to use the .NET synchronised methods here?
			dontFlushFromFind++;
			try
			{
				return loader.List( this, queryParameters );
			}
			catch ( ADOException sqle )
			{
				throw new ADOException( "error in FindBySQL", sqle );
			}
			finally
			{
				// TODO: Do we need to use the .NET synchronised methods here?
				dontFlushFromFind--;
			}
		}

		private ISqlLoadable GetSqlLoadable( System.Type clazz )
		{
			IClassPersister cp = GetPersister( clazz );
			if ( ! ( cp is ISqlLoadable ) )
			{
				throw new MappingException( string.Format( "class persister is not ISqlLoadable: {0}", clazz.FullName ) );
			}
			return (ISqlLoadable) cp;
		}

		/// <summary></summary>
		public void Clear()
		{
			arrayHolders.Clear();
			entitiesByKey.Clear();
			entityEntries.Clear();
			collectionsByKey.Clear();
			collectionEntries.Clear();
			proxiesByKey.Clear();
			batchLoadableEntityKeys.Clear();
			nonExists.Clear();

			updates.Clear();
			insertions.Clear();
			deletions.Clear();
			collectionCreations.Clear();
			collectionRemovals.Clear();
			collectionUpdates.Clear();
			
			// TODO: Find out why this is missing from H2.1
			nullifiables.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clazz"></param>
		/// <param name="uniqueKeyPropertyName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object LoadByUniqueKey( System.Type clazz, string uniqueKeyPropertyName, object id )
		{
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable) Factory.GetPersister( clazz );
			try
			{
				// TODO: Implement caching?! proxies?!
				object result = persister.LoadByUniqueKey( uniqueKeyPropertyName, id, this );
				return result == null ? null : ProxyFor( result );
			}
			catch ( ADOException sqle )
			{
				throw new ADOException( "Error performing LoadByUniqueKey", sqle );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="replicationMode"></param>
		public void Replicate( object obj, ReplicationMode replicationMode )
		{
			if ( obj == null )
			{
				throw new NullReferenceException( "attempt to replicate null" );
			}

			if ( ReassociateIfUninitializedProxy( obj ) )
			{
				return;
			}

			object theObj = UnproxyAndReassociate( obj );

			if ( IsEntryFor( theObj ) )
			{
				return;
			}

			IClassPersister persister = GetPersister( theObj );
			if ( persister.IsUnsaved( theObj ) )
			{
				//TODO: generate a new id value for brand new objects
				throw new TransientObjectException( "unsaved object passed to Replicate()" );
			}

			object id = persister.GetIdentifier( theObj );
			object oldVersion;
			if ( replicationMode == ReplicationMode.Exception )
			{
				//always do an INSERT, and let it fail by constraint violation
				oldVersion = null;
			}
			else
			{
				//what is the version on the database?
				oldVersion = persister.CurrentVersion( id, this );
			}

			if( oldVersion != null )
			{
				// existing row - do an update if appropriate
				if ( replicationMode.ShouldOverwriteCurrentVersion( theObj, oldVersion, persister.GetVersion( obj ), persister.VersionType ) )
				{
					//will result in a SQL UPDATE:
					DoReplicate( theObj, id, oldVersion, replicationMode, persister );
				}
				//else do nothing (don't even reassociate object!)
				//TODO: would it be better to do a refresh from db?
			}
			else
			{
				// no existing row - do an insert
				bool regenerate = persister.IsIdentifierAssignedByInsert; // prefer re-generation of identity!
				DoSave( 
					theObj, 
					regenerate ? null : new Key( id, persister ), 
					persister, 
					true, //!persister.isUnsaved(object), //TODO: Do an ordinary save in the case of an "unsaved" object
					// TODO: currently ignores interceptor definition of isUnsaved()
					regenerate,
					Cascades.CascadingAction.ActionReplicate, // not quite an ordinary save(), since we cascade back to replicate()
					replicationMode
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public void AddNonExist( Key key )
		{
			nonExists.Add( key );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object SaveOrUpdateCopy( object obj )
		{
			return DoCopy( obj, null, IdentityMap.Instantiate() );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public object Copy( object obj, IDictionary copiedAlready )
		{
			return DoCopy( obj, null, copiedAlready );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <param name="copiedAlready"></param>
		/// <returns></returns>
		public object DoCopy( object obj, object id, IDictionary copiedAlready )
		{
			if ( obj == null )
			{
				return null;
			}

			if ( obj is INHibernateProxy )
			{
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( (INHibernateProxy) obj );
				if ( li.IsUninitialized )
				{
					return Load( li.PersistentClass, li.Identifier ); // EARLY EXIT!
				}
			}

			if ( copiedAlready.Contains( obj ) )
			{
				return obj;		// EARLY EXIT!
			}

			EntityEntry entry = GetEntry( obj );
			if ( entry != null )
			{
				if ( id != null && entry.Id.Equals( id ) ) 
				{
					return obj;	//EARLY EXIT!
				}
				// else copy from one persistent instance to another!
			}

			System.Type clazz = obj.GetType() ;
			IClassPersister persister = GetPersister( clazz );

			object result;
			object target;
			if ( id == null && persister.IsUnsaved( obj ) )
			{
				copiedAlready[ obj ] = obj;
				SaveWithGeneratedIdentifier( obj, Cascades.CascadingAction.ActionCopy, copiedAlready );
				result = obj;		// TODO: Handle it's proxy (reassociate it, I suppose)
				target = obj;
			}
			else
			{
				if ( id == null )
				{
					id = persister.GetIdentifier( obj );
				}
				result = Get( clazz, id );
				if ( result == null )
				{
					copiedAlready[ obj ] = obj;
					SaveWithGeneratedIdentifier( obj, Cascades.CascadingAction.ActionCopy, copiedAlready );
					result = obj;		// TODO: Could it have a proxy?
					target = obj;
				}
				else
				{
					target = Unproxy( result );
					copiedAlready[ obj ] = target;
					if ( target == obj )
					{
						return result;
					}
					else if ( NHibernateUtil.GetClass( result ) != clazz )
					{
						throw new WrongClassException( "class of the given object did not match class of persistent copy", id, clazz );
					}
					else if ( persister.IsVersioned && !persister.VersionType.Equals( persister.GetVersion( result ), persister.GetVersion( obj ) ) )
					{
						throw new StaleObjectStateException( clazz, id );
					}

					// cascade first, so that all unsaved objected get saved before we actually copy
					Cascades.Cascade( this, persister, obj, Cascades.CascadingAction.ActionCopy, Engine.CascadePoint.CascadeOnCopy, copiedAlready );
				}
			}

			// no need to handle the version differently
			object[] copiedValues = TypeFactory.Copy(
				persister.GetPropertyValues( obj ),
				persister.GetPropertyValues( target ),
				persister.PropertyTypes,
				this,
				target, copiedAlready );

			persister.SetPropertyValues( target, copiedValues );
			return result;
		}
	}
}