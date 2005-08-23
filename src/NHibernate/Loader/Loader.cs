using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

using log4net;
using Iesi.Collections;

using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Abstract superclass of object loading (and querying) strategies.
	/// </summary>
	/// <remarks>
	/// <p>
	/// This class implements useful common functionality that concrete loaders would delegate to.
	/// It is not intended that this functionality would be directly accessed by client code (Hence,
	/// all methods of this class are declared <c>protected</c> or <c>private</c>.) This class relies heavily upon the
	/// <see cref="ILoadable" /> interface, which is the contract between this class and 
	/// <see cref="IClassPersister" />s that may be loaded by it.
	/// </p>
	/// <p>
	/// The present implementation is able to load any number of columns of entities and at most 
	/// one collection role per query.
	/// </p>
	/// </remarks>
	public abstract class Loader
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( Loader ) );

		// TODO: private ColumnNameCache columnNameCache;

		private Dialect.Dialect dialect;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		protected Loader( Dialect.Dialect dialect )
		{
			this.dialect = dialect;
		}

		internal Dialect.Dialect Dialect
		{
			get { return dialect; }
		}

		/// <summary>
		/// The SqlString to be called; implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// <p>
		/// The <c>setter</c> was added so that class inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </p>
		/// <p>
		/// The scope is <c>internal</c> because the <see cref="Hql.WhereParser"/> needs to
		/// be able to <c>get</c> the SqlString of the <see cref="Hql.QueryTranslator"/> when
		/// it is parsing a subquery.
		/// </p>
		/// </remarks>
		protected internal abstract SqlString SqlString { get; set; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// <p>
		/// The <c>setter</c> was added so that classes inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </p>
		/// </remarks>
		protected abstract ILoadable[ ] Persisters { get; set; }

		/// <summary>
		/// The suffix identifies a particular column of results in the SQL <c>IDataReader</c>;
		/// implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// <p>
		/// The <c>setter</c> was added so that classes inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </p>
		/// </remarks>
		protected abstract string[ ] Suffixes { get; set; }

		/// <summary>
		/// An array of indexes of the entity that owns a one-to-one association
		/// to the entity at the given index (-1 if there is no "owner")
		/// </summary>
		protected abstract int[ ] Owners { get; }

		/// <summary>
		/// An (optional) persister for a collection to be initialized; only collection loaders
		/// return a non-null value
		/// </summary>
		protected abstract ICollectionPersister CollectionPersister { get; }

		/// <summary>
		/// Get the index of the entity that owns the collection, or -1
		/// if there is no owner in the query results (i.e. in the case of a 
		/// collection initializer) or no collection.
		/// </summary>
		protected virtual int CollectionOwner
		{
			get { return -1; }
		}

		/// <summary>
		/// What lock mode does this load entities with?
		/// </summary>
		/// <param name="lockModes">A Collection of lock modes specified dynamically via the Query Interface</param>
		/// <returns></returns>
		protected abstract LockMode[ ] GetLockModes( IDictionary lockModes );


		/// <summary>
		/// Append <c>FOR UPDATE OF</c> clause, if necessary. This
		/// empty superclass implementation merely returns its first
		/// argument.
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="lockModes"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		protected virtual SqlString ApplyLocks( SqlString sql, IDictionary lockModes, Dialect.Dialect dialect )
		{
			return sql;
		}

		/// <summary>
		/// Does this Query return objects that might be already cached by 
		/// the session, whose lock mode may need upgrading.
		/// </summary>
		/// <returns></returns>
		protected virtual bool UpgradeLocks()
		{
			return false;
		}

		/// <summary>
		/// Return false is this loader is a batch entity loader
		/// </summary>
		protected virtual bool IsSingleRowLoader
		{
			get { return false; }
		}

		/// <summary>
		/// Execute an SQL query and attempt to instantiate instances of the class mapped by the given
		/// persister from each row of the <c>DataReader</c>. If an object is supplied, will attempt to
		/// initialize that object. If a collection is supplied, attempt to initialize that collection.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalId"></param>
		/// <param name="optionalCollectionKeys"></param>
		/// <param name="returnProxies"></param>
		/// <returns></returns>
		private IList DoQueryAndInitializeNonLazyCollections(
			ISessionImplementor session,
			QueryParameters queryParameters,
			object optionalObject,
			object optionalId,
			object[] optionalCollectionKeys,
			bool returnProxies )
		{
			session.BeforeLoad();
			IList result;
			try
			{
				result = DoQuery( session, queryParameters, optionalObject, optionalId, optionalCollectionKeys, returnProxies );
			}
			finally
			{
				session.AfterLoad();
			}
			session.InitializeNonLazyCollections();

			return result;
		}

		protected object LoadSingleRow( IDataReader resultSet, ISessionImplementor session, QueryParameters queryParameters, bool returnProxies )
		{
			int cols = Persisters.Length;
			IList hydratedObjects = cols == 0 ? null : new ArrayList();
			object result = GetRowFromResultSet( resultSet, session, queryParameters, hydratedObjects, null, null, new Key[cols], returnProxies );

			InitializeEntitiesAndCollections( hydratedObjects, resultSet, session );
			session.InitializeNonLazyCollections( );
			return result;
		}

		private object GetRowFromResultSet( 
			IDataReader resultSet, 
			ISessionImplementor session, 
			QueryParameters queryParameters, 
			IList hydratedObjects, 
			object optionalObject, 
			object optionalId, 
			Key[] keys, 
			bool returnProxies )
		{
			int cols = Persisters.Length;
			LockMode[] lockModeArray = GetLockModes( queryParameters.LockModes );

			// this is a CollectionInitializer and we are loading up a single collection
			bool hasCollections = CollectionPersister != null;
			// this is a query and we are loading multiple instances of the same collection role
			bool hasCollectionOwners = hasCollections && CollectionOwner >= 0;

			Key optionalObjectKey;

			if ( optionalObject != null )
			{
				optionalObjectKey = new Key( optionalId, session.GetPersister( optionalObject ) );
			}
			else
			{
				optionalObjectKey = null;
			}

			for ( int i = 0; i < cols; i++ )
			{
				//TODO: the i==cols-1 bit depends upon subclass implementation (very bad)
				keys[ i ] = GetKeyFromResultSet( i, Persisters[ i ], (i == cols - 1) ? optionalId : null, resultSet, session );
			}

			if ( Owners != null )
			{
				RegisterNonExists( keys, Owners, Persisters, session );
			}

			// this call is side-effecty
			object[] row = GetRow( resultSet, Persisters, Suffixes, keys, optionalObject, optionalObjectKey, lockModeArray, hydratedObjects, session );

			if ( returnProxies )
			{
				for ( int i = 0; i < cols; i++ )
				{
					// now get an existing proxy for each row element (if there is one)
					row[ i ] = session.ProxyFor( Persisters[ i ], keys[ i ], row[ i ] );
				}
			}

			if ( hasCollections )
			{
				//if null, owner will be retrieved from session
				object owner = hasCollectionOwners ? row[ CollectionOwner ] : null;
				object key = owner != null ? keys[ CollectionOwner ].Identifier : null;
				ReadCollectionElement( owner, key, resultSet, session );
			}

			return GetResultColumnOrRow( row, resultSet, session );
		}

		private IList DoQuery(
			ISessionImplementor session,
			QueryParameters queryParameters,
			object optionalObject,
			object optionalId,
			object[] optionalCollectionKeys,
			bool returnProxies
			)
		{
			RowSelection selection = queryParameters.RowSelection;
			int maxRows = HasMaxRows( selection ) ? selection.MaxRows : int.MaxValue;

			ILoadable[ ] persisters = Persisters;
			int cols = persisters.Length;
			
			ArrayList hydratedObjects = cols > 0 ? new ArrayList() : null;
			IList results = new ArrayList();

			IDbCommand st = PrepareQueryCommand(
				ApplyLocks( SqlString, queryParameters.LockModes, session.Factory.Dialect ),
				queryParameters, false, session );

			IDataReader rs = GetResultSet( st, selection, session );

			try
			{
				if ( optionalCollectionKeys != null )
				{
					HandleEmptyCollections( optionalCollectionKeys, rs, session );
				}

				Key[] keys = new Key[ cols ]; // we can reuse it each time

				if ( log.IsDebugEnabled )
				{
					log.Debug( "processing result set" );
				}

				int count;
				for ( count = 0; count < maxRows && rs.Read(); count++ )
				{
					object result = GetRowFromResultSet( rs, session, queryParameters, hydratedObjects, optionalObject, optionalId, keys, returnProxies );
					results.Add( result );
				}

				if ( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "done processing result set ({0} rows)", count ) ) ;
				}
			}
			catch ( Exception sqle )
			{
				ADOExceptionReporter.LogExceptions( sqle );
				throw;
			}
			finally
			{
				session.Batcher.CloseQueryCommand( st, rs );
			}

			InitializeEntitiesAndCollections( hydratedObjects, rs, session );

			return results;  // GetResultList( results );
		}

		private void InitializeEntitiesAndCollections( IList hydratedObjects, object resultSetId, ISessionImplementor session )
		{
			if ( Persisters.Length > 0 )
			{
				int hydratedObjectsSize = hydratedObjects.Count;

				if ( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "total objects hydrated: {0}", hydratedObjectsSize ) );
				}

				for ( int i = 0; i < hydratedObjectsSize; i++ )
				{
					session.InitializeEntity( hydratedObjects[ i ] );
				}
			}

			if ( CollectionPersister != null )
			{
				//this is a query and we are loading multiple instances of the same collection role
				session.EndLoadingCollections( CollectionPersister, resultSetId );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		protected virtual IList GetResultList( IList results )
		{
			return results;
		}

		/// <summary>
		/// Get the actual object that is returned in the user-visible result list.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		/// <remarks>
		/// This empty implementation merely returns its first argument. This is
		/// overridden by some subclasses.
		/// </remarks>
		protected virtual object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			return row;
		}

		private void RegisterNonExists(
			Key[] keys,
			int[] owners,
			ILoadable[] persisters,
			ISessionImplementor session
			)
		{
			for ( int i = 0; i < keys.Length; i++ )
			{
				int owner = owners[ i ];
				if ( owner > -1 )
				{
					Key ownerKey = keys[ owner ];
					if ( keys[ i ] == null && ownerKey != null )
					{
						session.AddNonExist( new Key( ownerKey.Identifier, persisters[ i ] ) );
					}
				}
			}
		}

		/// <summary>
		/// Read one collection element from the current row of the ADO.NET result set
		/// </summary>
		/// <param name="optionalOwner"></param>
		/// <param name="optionalKey"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		private void ReadCollectionElement(
			object optionalOwner,
			object optionalKey,
			IDataReader rs,
			ISessionImplementor session
			)
		{
			object collectionRowKey = CollectionPersister.ReadKey( rs, session );
			if ( collectionRowKey != null )
			{
				if ( log.IsDebugEnabled )
				{
					log.Debug( "found row of collection: " + MessageHelper.InfoString( CollectionPersister, collectionRowKey ) );
				}
				object owner = optionalOwner;
				if ( owner == null )
				{
					owner = session.GetCollectionOwner( collectionRowKey, CollectionPersister );
					if ( owner == null )
					{
						//TODO: This is assertion is disabled because there is a bug that means the
						//      original owner of a transient, uninitialized collection is not known 
						//      if the collection is re-referenced by a different object associated 
						//      with the current Session
						//throw new AssertionFailure("bug loading unowned collection");
					}
				}
				PersistentCollection rowCollection = session.GetLoadingCollection( CollectionPersister, collectionRowKey, rs );
				if ( rowCollection != null )
				{
					rowCollection.ReadFrom( rs, CollectionPersister, owner );
				}
			}
			else if ( optionalKey != null )
			{
				if ( log.IsDebugEnabled )
				{
					log.Debug( "result set contains (possibly empty) collection: " + MessageHelper.InfoString( CollectionPersister, optionalKey ) );
				}
				session.GetLoadingCollection( CollectionPersister, optionalKey, rs );	// handle empty collection
			}
		}

		/// <summary>
		/// If this is a collection initializer, we need to tell the session that a collection
		/// is being initilized, to account for the possibility of the collection having
		/// no elements (hence no rows in the result set).
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="resultSetId"></param>
		/// <param name="session"></param>
		private void HandleEmptyCollections(
			object[] keys,
			object resultSetId,
			ISessionImplementor session
			)
		{
			for ( int i = 0; i < keys.Length; i++ )
			{
				if ( log.IsDebugEnabled )
				{
					log.Debug( "result set contains (possibly empty) collection: " + MessageHelper.InfoString( CollectionPersister, keys[ i ] ) );
				}
				session.GetLoadingCollection( CollectionPersister, keys[ i ], resultSetId );
			}
		}

		/// <summary>
		/// Read a row of <c>Key</c>s from the <c>IDataReader</c> into the given array.
		/// </summary>
		/// <remarks>
		/// Warning: this method is side-effecty. If an <c>id</c> is given, don't bother going
		/// to the <c>IDataReader</c>
		/// </remarks>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		private Key GetKeyFromResultSet( int i, ILoadable persister, object id, IDataReader rs, ISessionImplementor session )
		{
			object resultId;

			// if we know there is exactly 1 row, we can skip.
			// it would be great if we could _always_ skip this;
			// it is a problem for <key-many-to-one>

			if( IsSingleRowLoader && id != null )
			{
				resultId = id;
			}
			else
			{
				// problematic for <key-many-to-one>!
				resultId = persister.IdentifierType.NullSafeGet( rs, suffixedKeyColumns[ i ], session, null );

				if ( id != null && resultId != null && id.Equals( resultId ) )
				{
					// use the id passed in
					resultId = id;
				}
			}

			return ( resultId == null ) ? null : new Key( resultId, persister );
		}

		/// <summary>
		/// Check the version of the object in the <c>IDataReader</c> against
		/// the object version in the session cache, throwing an exception
		/// if the vesrion numbers are different.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="version"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <exception cref="StaleObjectStateException"></exception>
		private void CheckVersion( int i, ILoadable persister, object id, object version, IDataReader rs, ISessionImplementor session )
		{
			// null version means the object is in the process of being loaded somewhere
			// else in the ResultSet
			if( version != null )
			{
				IType versionType = persister.VersionType;
				object currentVersion = versionType.NullSafeGet( rs, suffixedVersionColumnNames[ i ], session, null );
				if( !versionType.Equals( version, currentVersion ) )
				{
					throw new StaleObjectStateException( persister.MappedClass, id );
				}
			}
		}

		/// <summary>
		/// Resolve any ids for currently loaded objects, duplications within the <c>IDataReader</c>,
		/// etc. Instanciate empty objects to be initialized from the <c>IDataReader</c>. Return an
		/// array of objects (a row of results) and an array of booleans (by side-effect) that determine
		/// wheter the corresponding object should be initialized
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persisters"></param>
		/// <param name="suffixes"></param>
		/// <param name="keys"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalObjectKey"></param>
		/// <param name="session"></param>
		/// <param name="hydratedObjects"></param>
		/// <param name="lockModes"></param>
		/// <returns></returns>
		private object[ ] GetRow(
			IDataReader rs,
			ILoadable[ ] persisters,
			string[ ] suffixes,
			Key[ ] keys,
			object optionalObject,
			Key optionalObjectKey,
			LockMode[ ] lockModes,
			IList hydratedObjects,
			ISessionImplementor session )
		{
			int cols = persisters.Length;

			if( log.IsDebugEnabled )
			{
				log.Debug( "result row: " + StringHelper.ToString( keys ) );
			}

			object[ ] rowResults = new object[cols];

			for( int i = 0; i < cols; i++ )
			{
				object obj = null;
				Key key = keys[ i ];

				if( keys[ i ] == null )
				{
					// do nothing - used to have hydrate[i] = false;
				}
				else
				{
					//If the object is already loaded, return the loaded one
					obj = session.GetEntity( key );
					if( obj != null )
					{
						//its already loaded so dont need to hydrate it
						InstanceAlreadyLoaded( rs, i, persisters[ i ], suffixes[ i ], key, obj, lockModes[ i ], session );
					}
					else
					{
						obj = InstanceNotYetLoaded( rs, i, persisters[ i ], suffixes[ i ], key, lockModes[ i ], optionalObjectKey, optionalObject, hydratedObjects, session );
					}
				}

				rowResults[ i ] = obj;
			}
			return rowResults;
		}

		/// <summary>
		/// The entity instance is already in the session cache
		/// </summary>
		private void InstanceAlreadyLoaded( IDataReader rs, int i, ILoadable persister, string suffix, Key key, object obj, LockMode lockMode, ISessionImplementor session )
		{
			if( !persister.MappedClass.IsAssignableFrom( obj.GetType() ) )
			{
				throw new WrongClassException( "loading object was of wrong class", key.Identifier, persister.MappedClass );
			}

			if( LockMode.None != lockMode && UpgradeLocks() )
			{
				// we don't need to worry about existing version being uninitialized
				// because this block isn't called by a re-entrant load (re-entrant
				// load _always_ have lock mode NONE
				if( persister.IsVersioned && session.GetLockMode( obj ).LessThan( lockMode ) )
				{
					// we only check the version when _upgrading_ lock modes
					CheckVersion( i, persister, key.Identifier, session.GetVersion( obj ), rs, session );
					// we need to upgrade the lock mode to the mode requested
					session.SetLockMode( obj, lockMode );
				}
			}
		}

		/// <summary>
		/// The entity instance is not in the session cache
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="i"></param>
		/// <param name="persister"></param>
		/// <param name="suffix"></param>
		/// <param name="key"></param>
		/// <param name="lockMode"></param>
		/// <param name="optionalObjectKey"></param>
		/// <param name="optionalObject"></param>
		/// <param name="hydratedObjects"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private object InstanceNotYetLoaded( IDataReader dr, int i, ILoadable persister, string suffix, Key key, LockMode lockMode, Key optionalObjectKey, object optionalObject, IList hydratedObjects, ISessionImplementor session )
		{
			object obj;

			System.Type instanceClass = GetInstanceClass( dr, i, persister, key.Identifier, session );

			if( optionalObjectKey != null && key.Equals( optionalObjectKey ) )
			{
				// its the given optional object
				obj = optionalObject;
			}
			else
			{
				obj = session.Instantiate( instanceClass, key.Identifier );
			}

			// need to hydrate it

			// grab its state from the DataReader and keep it in the Session
			// (but don't yet initialize the object itself)
			// note that we acquired LockMode.READ even if it was not requested
			LockMode acquiredLockMode = lockMode == LockMode.None ? LockMode.Read : lockMode;
			LoadFromResultSet( dr, i, obj, key, suffix, acquiredLockMode, persister, session );

			// materialize associations (and initialize the object) later
			hydratedObjects.Add( obj );

			return obj;
		}

		/// <summary>
		/// Hydrate the state of an object from the SQL <c>IDataReader</c>, into
		/// an array of "hydrated" values (do not resolve associations yet),
		/// and pass the hydrated state to the session.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="i"></param>
		/// <param name="obj"></param>
		/// <param name="key"></param>
		/// <param name="suffix"></param>
		/// <param name="lockMode"></param>
		/// <param name="rootPersister"></param>
		/// <param name="session"></param>
		private void LoadFromResultSet( IDataReader rs, int i, object obj, Key key, string suffix, LockMode lockMode, ILoadable rootPersister, ISessionImplementor session )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Initializing object from DataReader: " + key );
			}

			// add temp entry so that the next step is circular-reference
			// safe - only needed because some types don't take proper
			// advantage of two-phase-load (esp. components)
			session.AddUninitializedEntity( key, obj, lockMode );

			// Get the persister for the _subclass_
			ILoadable persister = ( ILoadable ) session.GetPersister( obj );

			// This is not very nice (and quite slow):
			string[ ][ ] cols = persister == rootPersister ?
				suffixedPropertyColumns[ i ] :
				GetSuffixedPropertyAliases( persister, suffix );

			object id = key.Identifier;
			object[ ] values = Hydrate( rs, id, obj, persister, session, cols );
			session.PostHydrate( persister, id, values, obj, lockMode );
		}

		/// <summary>
		/// Determine the concrete class of an instance for the <c>IDataReader</c>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="i"></param>
		/// <param name="persister"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private System.Type GetInstanceClass( IDataReader rs, int i, ILoadable persister, object id, ISessionImplementor session )
		{
			System.Type topClass = persister.MappedClass;

			if( persister.HasSubclasses )
			{
				// code to handle subclasses of topClass
				object discriminatorValue = persister.DiscriminatorType.NullSafeGet( rs, suffixedDiscriminatorColumn[ i ], session, null );

				System.Type result = persister.GetSubclassForDiscriminatorValue( discriminatorValue );

				if( result == null )
				{
					// woops we got an instance of another class hierarchy branch.
					throw new WrongClassException( "Discriminator: " + discriminatorValue, id, topClass );
				}

				return result;
			}
			else
			{
				return topClass;
			}
		}

		/// <summary>
		/// Unmarshall the fields of a persistent instance from a result set
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <param name="suffixedPropertyColumns"></param>
		/// <returns></returns>
		private object[ ] Hydrate( IDataReader rs, object id, object obj, ILoadable persister, ISessionImplementor session, string[ ][ ] suffixedPropertyColumns )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug( "Hydrating entity: " + persister.ClassName + '#' + id );
			}

			IType[ ] types = persister.PropertyTypes;
			object[ ] values = new object[types.Length];

			for( int i = 0; i < types.Length; i++ )
			{
				values[ i ] = types[ i ].Hydrate( rs, suffixedPropertyColumns[ i ], session, obj );
			}
			return values;
		}

		/// <summary>
		/// Advance the cursor to the first required row of the <c>IDataReader</c>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="selection"></param>
		private void Advance( IDataReader rs, RowSelection selection )
		{
			int firstRow = GetFirstRow( selection );

			if( firstRow != 0 )
			{
				// DataReaders are forward-only, readonly, so we have to step through
				for( int i = 0; i < firstRow; i++ )
				{
					rs.Read();
				}
			}
		}

		private static bool HasMaxRows(RowSelection selection)
		{
			// it used to be selection.MaxRows != null -> since an Int32 will always
			// have a value I'll compare it to the static field NoValue used to initialize 
			// max rows to nothing
			return selection != null && selection.MaxRows != RowSelection.NoValue;
		}

		private static int GetFirstRow( RowSelection selection )
		{
			return ( selection == null ) ? 0 : selection.FirstRow;		
		}

		/// <summary>
		/// Should we pre-process the SQL string, adding a dialect-specific
		/// LIMIT clause.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		private static bool UseLimit( RowSelection selection, Dialect.Dialect dialect )
		{
			return dialect.SupportsLimit && HasMaxRows(selection);
		}

		/// <summary>
		/// Bind positional parameter values to the <c>IDbCommand</c>
		/// (these are parameters specified by ?).
		/// </summary>
		/// <param name="st"></param>
		/// <param name="queryParameters"></param>
		/// <param name="start"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected int BindPositionalParameters(
			IDbCommand st,
			QueryParameters queryParameters,
			int start,
			ISessionImplementor session )
		{
			object[ ] values = queryParameters.PositionalParameterValues;
			IType[ ] types = queryParameters.PositionalParameterTypes;

			int span = 0;
			for( int i = 0; i < values.Length; i++ )
			{
				types[ i ].NullSafeSet( st, values[ i ], start + span, session );
				span += types[ i ].GetColumnSpan( session.Factory );
			}

			return span;
		}

		/// <summary>
		/// Creates an IDbCommand object and populates it with the values necessary to execute it against the 
		/// database to Load an Entity.
		/// </summary>
		/// <param name="sqlString">The SqlString to convert into a prepared IDbCommand.</param>
		/// <param name="parameters">The <see cref="QueryParameters"/> to use for the IDbCommand.</param>
		/// <param name="scroll">TODO: find out where this is used...</param>
		/// <param name="session">The SessionImpl this Command is being prepared in.</param>
		/// <returns>An IDbCommand that is ready to be executed.</returns>
		protected virtual IDbCommand PrepareQueryCommand( SqlString sqlString, QueryParameters parameters, bool scroll, ISessionImplementor session )
		{
			Dialect.Dialect dialect = session.Factory.Dialect;

			RowSelection selection = parameters.RowSelection;
			bool useLimit = UseLimit( selection, dialect );
			bool hasFirstRow = GetFirstRow( selection ) > 0;
			bool useOffset = hasFirstRow && useLimit && dialect.SupportsLimitOffset;
			//TODO: In .Net all resultsets are scrollable (we receive an IDataReader), so this is not needed
			bool scrollable = session.Factory.IsScrollableResultSetsEnabled &&
				(scroll || ( hasFirstRow && !useOffset ) );

			if( useLimit )
			{
				sqlString = dialect.GetLimitString( sqlString.Trim(),
					useOffset ? GetFirstRow( selection ) : 0,
					GetMaxOrLimit( dialect, selection ) );
			}

			IDbCommand command = session.Batcher.PrepareQueryCommand( sqlString, scrollable );

			try
			{
				if( selection != null && selection.Timeout != RowSelection.NoValue )
				{
					command.CommandTimeout = selection.Timeout;
				}

				int colIndex = 0;

				if( useLimit && dialect.BindLimitParametersFirst )
				{
					colIndex += BindLimitParameters( command, colIndex, selection, session );
				}

				colIndex += BindPositionalParameters( command, parameters, colIndex, session );
				colIndex += BindNamedParameters( command, parameters.NamedParameters, colIndex, session );

				if( useLimit && !dialect.BindLimitParametersFirst )
				{
					colIndex += BindLimitParameters( command, colIndex, selection, session );
				}

				if( !useLimit )
				{
					SetMaxRows( command, selection );
				}
				if( selection != null )
				{
					if( selection.Timeout != RowSelection.NoValue )
					{
						command.CommandTimeout = selection.Timeout;
					}
					// H2.1 handles FetchSize here - not ported
				}

			}
			catch( HibernateException )
			{
				ClosePreparedCommand( command, null, session );
				throw;
			}
			catch( Exception sqle )
			{
				ADOExceptionReporter.LogExceptions( sqle );
				ClosePreparedCommand( command, null, session );
				throw;
			}

			return command;
		}

		/// <summary>
		/// Some dialect-specific LIMIT clauses require the maximum last row number,
		/// others require the maximum returned row count.
		/// </summary>
		private static int GetMaxOrLimit(Dialect.Dialect dialect, RowSelection selection)
		{
			int firstRow = GetFirstRow(selection);
			int lastRow = selection.MaxRows;

			if( dialect.UseMaxForLimit )
			{
				return lastRow + firstRow;
			}
			else
			{
				return lastRow;
			}
		}

		/// <summary>
		/// Bind parameters needed by the dialect-specific LIMIT clause
		/// </summary>
		/// <returns>The number of parameters bound</returns>
		private int BindLimitParameters(IDbCommand st, int index, RowSelection selection, ISessionImplementor session)
		{
			Dialect.Dialect dialect = session.Factory.Dialect;
			if( !dialect.SupportsVariableLimit )
			{
				return 0;
			}
			if( !HasMaxRows( selection ) )
			{
				throw new AssertionFailure( "max results not set" );
			}
			int firstRow = GetFirstRow( selection );
			int lastRow = GetMaxOrLimit( dialect, selection );

			bool hasFirstRow = firstRow > 0 && dialect.SupportsLimitOffset;
			bool reverse = dialect.BindLimitParametersInReverseOrder;

			if( hasFirstRow )
			{ 
				( ( IDataParameter ) st.Parameters[ index + ( reverse ? 1 : 0 ) ] ).Value = firstRow;
			}
			( ( IDataParameter ) st.Parameters[ index + ( ( reverse || !hasFirstRow ) ? 0 : 1 ) ] ).Value = lastRow;

			return hasFirstRow ? 2 : 1;
		}

		/// <summary>
		/// Limits the number of rows returned by the Sql query if necessary.
		/// </summary>
		/// <param name="st">The IDbCommand to limit.</param>
		/// <param name="selection">The RowSelection that contains the MaxResults info.</param>
		/// <remarks>TODO: This does not apply to ADO.NET at all</remarks>
		protected void SetMaxRows( IDbCommand st, RowSelection selection )
		{
			if( HasMaxRows( selection ) )
			{
				//TODO: H2.0.3 - do we need this method??
				// there is nothing in ADO.NET to do anything  similar
				// to Java's PreparedStatement.setMaxRows(int)
			}
		}

		/// <summary>
		/// Fetch a <c>IDbCommand</c>, call <c>SetMaxRows</c> and then execute it,
		/// advance to the first result and return an SQL <c>IDataReader</c>
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand" /> to execute.</param>
		/// <param name="selection">The <see cref="RowSelection"/> to apply to the <see cref="IDbCommand"/> and <see cref="IDataReader"/>.</param>
		/// <param name="session">The <see cref="ISession" /> to load in.</param>
		/// <returns>An IDataReader advanced to the first record in RowSelection.</returns>
		protected IDataReader GetResultSet( IDbCommand st, RowSelection selection, ISessionImplementor session )
		{
			IDataReader rs = null;
			try
			{
				log.Info( st.CommandText );
				rs = session.Batcher.ExecuteReader( st );

				Dialect.Dialect dialect = session.Factory.Dialect;
				if( !dialect.SupportsLimitOffset || !UseLimit( selection, dialect ) )
				{
					Advance( rs, selection );
				}

				return rs;
			}
			catch( Exception sqle )
			{
				ADOExceptionReporter.LogExceptions( sqle );
				ClosePreparedCommand( st, rs, session );
				throw;
			}
		}

		/* TODO:
		[MethodImpl(MethodImplOptions.Synchronized)]
		private IDataReader WrapDataReaderIfEnabled( IDataReader rs, ISessionImplementor session )
		{
			if( session.Factory.IsWrapResultSetsEnabled )
			{
				try
				{
					log.Debug( "Wrapping data reader [" + rs + "]" );
					return new DataReaderWrapper( rs, RetrieveColumnNameToIndexCache( rs ) );
				}
				catch( Exception e )
				{
					log.Info( "Error wrapping result set; using naked result set", e );
				}
			}
			return rs;
		}

		private ColumnNameCache RetrieveColumnNameToIndexCache( IDataReader rs )
		{
			if( columnNameCache == null )
			{
				log.Debug( "Building columnName->columnIndex cache" );
				columnNameCache = ColumnNameCache.Construct( rs.GetSchemaTable() );
			}

			return columnNameCache;
		}
		*/

		/// <summary>
		/// Cleans up the resources used by this Loader.
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand"/> to close.</param>
		/// <param name="reader">The <see cref="IDataReader"/> to close.</param>
		/// <param name="session">The <see cref="ISession"/> this Loader is using.</param>
		protected void ClosePreparedCommand( IDbCommand st, IDataReader reader, ISessionImplementor session )
		{
			session.Batcher.CloseQueryCommand( st, reader );
		}

		/// <summary>
		/// Bind named parameters to the <c>IDbCommand</c>
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand"/> that contains the parameters.</param>
		/// <param name="namedParams">The named parameters (key) and the values to set.</param>
		/// <param name="session">The <see cref="ISession"/> this Loader is using.</param>
		/// <param name="start"></param>
		/// <remarks>
		/// This has an empty implementation on this superclass and should be implemented by
		/// sublcasses (queries) which allow named parameters.
		/// </remarks>
		protected virtual int BindNamedParameters( IDbCommand st, IDictionary namedParams, int start, ISessionImplementor session )
		{
			return 0;
		}

		/// <summary>
		/// Called by subclasses that load entities.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalID"></param>
		/// <returns></returns>
		protected IList LoadEntity(
			ISessionImplementor session,
			object[ ] values,
			IType[ ] types,
			object optionalObject,
			object optionalID
			)
		{
			QueryParameters qp = new QueryParameters( types, values );
			return DoQueryAndInitializeNonLazyCollections( session, qp, optionalObject, optionalID, null, false );
		}
		
		/// <summary>
		/// Called by subclasses that load entities
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="identifierType"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalIdentifier"></param>
		/// <returns></returns>
		protected IList LoadEntity(
			ISessionImplementor session,
			object id,
			IType identifierType,
			object optionalObject,
			object optionalIdentifier
			)
		{
			return LoadEntity( session, new object[] { id }, new IType[] { identifierType }, optionalObject, optionalIdentifier );
		}

		/// <summary>
		/// Called by subclasses that batch load entities
		/// </summary>
		/// <param name="session"></param>
		/// <param name="ids"></param>
		/// <param name="idType"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalID"></param>
		/// <returns></returns>
		protected IList LoadEntityBatch(
			ISessionImplementor session,
			object[] ids,
			IType idType,
			object optionalObject,
			object optionalID
			)
		{
			IType[] types = new IType[ ids.Length ];
			for( int i = 0; i < types.Length; i++ )
			{
				types[ i ] = idType;
			}
			return LoadEntity( session, ids, types, optionalObject, optionalID  );
		}

		/// <summary>
		/// Called by subclasses that load collections
		/// </summary>
		/// <param name="session"></param>
		/// <param name="id"></param>
		/// <param name="type"></param>
		protected internal void LoadCollection(
			ISessionImplementor session,
			object id,
			IType type )
		{
			LoadCollection( session, new object[] { id }, new IType[] { type } );
		}
			
		/// <summary>
		/// Called by subclasses that batch initialize collections
		/// </summary>
		/// <param name="session"></param>
		/// <param name="ids"></param>
		/// <param name="type"></param>
		protected internal void LoadCollectionBatch(
			ISessionImplementor session,
			object[] ids,
			IType type )
		{
			IType[] idTypes = new IType[ ids.Length ];
			for( int i = 0; i < idTypes.Length; i++ )
			{
				idTypes[ i ] = type;
			}
			LoadCollection( session, ids, idTypes );
		}

		/// <summary>
		/// Called by subclasses that initialize collections
		/// </summary>
		/// <param name="session"></param>
		/// <param name="ids"></param>
		/// <param name="types"></param>
		private void LoadCollection(
			ISessionImplementor session,
			object[] ids,
			IType[] types )
		{
			QueryParameters qp = new QueryParameters( types, ids );
			DoQueryAndInitializeNonLazyCollections( session, qp, null, null, ids, true );
		}

		/// <summary>
		/// Return the query results, using the query cache, called
		/// by subclasses that implement cacheable queries
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <param name="querySpaces"></param>
		/// <param name="resultTypes"></param>
		/// <returns></returns>
		protected IList List(
			ISessionImplementor session,
			QueryParameters queryParameters,
			ISet querySpaces,
			IType[] resultTypes )
		{
			ISessionFactoryImplementor factory = session.Factory;

			bool cacheable = factory.IsQueryCacheEnabled && queryParameters.Cacheable;

			if ( cacheable )
			{
				IQueryCache queryCache = factory.GetQueryCache( queryParameters.CacheRegion ) ;
				QueryKey key = new QueryKey( SqlString, queryParameters );
				IList result = null;
				if ( !queryParameters.ForceCacheRefresh )
				{
					result = queryCache.Get( key, resultTypes, querySpaces, session );
				}
				if ( result == null )
				{
					result = DoList( session, queryParameters );
					queryCache.Put( key, resultTypes, result, session );
				}

				return GetResultList( result );
			}
			else
			{
				return GetResultList( DoList( session, queryParameters ) );
			}
		}

		/// <summary>
		/// Actually execute a query, ignoring the query cache
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <returns></returns>
		protected IList DoList( ISessionImplementor session, QueryParameters queryParameters )
		{
			return DoQueryAndInitializeNonLazyCollections( session, queryParameters, null, null, null, true );
		}

		private string[ ][ ] suffixedKeyColumns;
		private string[ ][ ] suffixedVersionColumnNames;
		private string[ ][ ][ ] suffixedPropertyColumns;
		private string[ ] suffixedDiscriminatorColumn;

		protected static readonly string[] NoSuffix = { string.Empty };

		/// <summary>
		/// Calculate and cache select-clause suffixes. Must be
		/// called by subclasses after instantiation.
		/// </summary>
		protected void PostInstantiate()
		{
			ILoadable[ ] persisters = Persisters;
			string[ ] suffixes = Suffixes;
			suffixedKeyColumns = new string[persisters.Length][ ];
			suffixedPropertyColumns = new string[persisters.Length][ ][ ];
			suffixedVersionColumnNames = new string[persisters.Length][ ];
			suffixedDiscriminatorColumn = new string[persisters.Length];

			for( int i = 0; i < persisters.Length; i++ )
			{
				ILoadable persister = persisters[ i ];
				suffixedKeyColumns[ i ] = persister.GetIdentifierAliases( suffixes[ i ] );
				suffixedPropertyColumns[ i ] = GetSuffixedPropertyAliases( persister, suffixes[ i ] );
				suffixedDiscriminatorColumn[ i ] = persister.GetDiscriminatorAlias( suffixes[ i ] );
				if( persister.IsVersioned )
				{
					suffixedVersionColumnNames[ i ] = suffixedPropertyColumns[ i ][ persister.VersionProperty ];
				}
			}
		}

		private static string[ ][ ] GetSuffixedPropertyAliases( ILoadable persister, string suffix  )
		{
			int size = persister.PropertyNames.Length;
			string[ ][ ] suffixedPropertyAliases = new string[size][ ];
			for( int j = 0; j < size; j++ )
			{
				suffixedPropertyAliases[ j ] = persister.GetPropertyAliases( suffix, j );
			}
			return suffixedPropertyAliases;
		}

		/// <summary>
		/// Utility method that generate 0_, 1_ suffixes. Subclasses don't
		/// necessarily need to use this algorithm, but it is intended that
		/// they will in most cases.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		protected static string[] GenerateSuffixes( int length )
		{
			if ( length == 0 )
			{
				return NoSuffix;
			}

			string[] suffixes = new string[ length ];

			for ( int i = 0; i < length; i++ )
			{
				suffixes[ i ] = i.ToString() + StringHelper.Underscore;
			}

			return suffixes;
		}

		/// <summary>
		/// Generate a nice alias for the given class name or collection role
		/// name and unique integer. Subclasses do <em>not</em> have to use
		/// aliases of this form.
		/// </summary>
		/// <param name="description"></param>
		/// <param name="unique"></param>
		/// <returns>an alias of the form <c>foo1_</c></returns>
		protected string GenerateAlias( string description, int unique )
		{
			// NB Java has this as static but we need the Dialect to achieve correct quoting.
			return new Alias( 10, unique.ToString() + StringHelper.Underscore )
				.ToAliasString( StringHelper.Unqualify( description ).ToLower().Replace( "$", "_" ), Dialect );
		}
	}
}