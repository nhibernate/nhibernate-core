using System;
using System.Collections;
using System.Data;
using Iesi.Collections;

using log4net;

using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Transform;
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
	/// <see cref="IEntityPersister" />s that may be loaded by it.
	/// </p>
	/// <p>
	/// The present implementation is able to load any number of columns of entities and at most 
	/// one collection role per query.
	/// </p>
	/// </remarks>
	public abstract class Loader
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( Loader ) );

		private readonly ISessionFactoryImplementor factory;
		// TODO: private ColumnNameCache columnNameCache;

		public Loader( ISessionFactoryImplementor factory )
		{
			this.factory = factory;
		}

		/// <summary>
		/// The SqlString to be called; implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <c>setter</c> was added so that class inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </para>
		/// <para>
		/// The scope is <c>protected internal</c> because the <see cref="Hql.Classic.WhereParser"/> needs to
		/// be able to <c>get</c> the SqlString of the <see cref="Hql.Classic.QueryTranslator"/> when
		/// it is parsing a subquery.
		/// </para>
		/// </remarks>
		protected internal abstract SqlString SqlString { get; set; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// The <c>setter</c> was added so that classes inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </remarks>
		protected abstract ILoadable[ ] EntityPersisters { get; set; }

		// TODO H3: EntityEagerPropertyFetches

		/// <summary>
		/// An array of indexes of the entity that owns a one-to-one association
		/// to the entity at the given index (-1 if there is no "owner")
		/// </summary>
		protected virtual int[ ] Owners
		{
			get { return null; }
			set { throw new NotSupportedException( "Loader.set_Owners" ); }
		}

		protected virtual EntityType[ ] OwnerAssociationTypes
		{
			get { return null; }
		}

		/// <summary>
		/// An (optional) persister for a collection to be initialized; only collection loaders
		/// return a non-null value
		/// </summary>
		protected abstract ICollectionPersister[] CollectionPersisters { get; }

		/// <summary>
		/// Get the index of the entity that owns the collection, or -1
		/// if there is no owner in the query results (i.e. in the case of a 
		/// collection initializer) or no collection.
		/// </summary>
		protected virtual int[] CollectionOwners
		{
			get { return null; }
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
		protected virtual SqlString ApplyLocks( SqlString sql, IDictionary lockModes, Dialect.Dialect dialect )
		{
			return sql;
		}

		/// <summary>
		/// Does this query return objects that might be already cached by 
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
		/// Get the SQL table aliases of entities whose
		/// associations are subselect-loadable, returning
		/// null if this loader does not support subselect
		/// loading
		/// </summary>
		protected virtual string[] Aliases
		{
			get { return null; }
		}

		/// <summary>
		/// Modify the SQL, adding lock hints and comments, if necessary
		/// </summary>
		protected SqlString PreprocessSQL( SqlString sql, QueryParameters parameters, Dialect.Dialect dialect )
		{
			sql = ApplyLocks( sql, parameters.LockModes, dialect );
			
			// TODO H3: SQL comments
			return sql;
		}

		/// <summary>
		/// Execute an SQL query and attempt to instantiate instances of the class mapped by the given
		/// persister from each row of the <c>DataReader</c>. If an object is supplied, will attempt to
		/// initialize that object. If a collection is supplied, attempt to initialize that collection.
		/// </summary>
		private IList DoQueryAndInitializeNonLazyCollections(
			ISessionImplementor session,
			QueryParameters queryParameters,
			bool returnProxies )
		{
			session.BeforeLoad();
			IList result;
			try
			{
				result = DoQuery( session, queryParameters, returnProxies );
			}
			finally
			{
				session.AfterLoad();
			}
			session.InitializeNonLazyCollections();

			return result;
		}

		/// <summary>
		/// Loads a single row from the result set.  This is the processing used from the
		/// ScrollableResults where no collection fetches were encountered.
		/// </summary>
		/// <param name="resultSet">The result set from which to do the load.</param>
		/// <param name="session">The session from which the request originated.</param>
		/// <param name="queryParameters">The query parameters specified by the user.</param>
		/// <param name="returnProxies">Should proxies be generated</param>
		/// <returns>The loaded "row".</returns>
		/// <exception cref="HibernateException" />
		protected object LoadSingleRow(
			IDataReader resultSet,
			ISessionImplementor session,
			QueryParameters queryParameters,
			bool returnProxies )
		{
			int entitySpan = EntityPersisters.Length;
			IList hydratedObjects = entitySpan == 0 ?
				null : new ArrayList( entitySpan );

			object result;
			try
			{
				result = GetRowFromResultSet(
					resultSet,
					session,
					queryParameters,
					GetLockModes( queryParameters.LockModes ),
					null,
					hydratedObjects,
					new EntityKey[entitySpan],
					returnProxies );
			}
			catch( HibernateException )
			{
				throw; // Don't call Convert on HibernateExceptions
			}
			catch( Exception sqle )
			{
				throw ADOExceptionHelper.Convert( sqle, "could not read next row of results", SqlString );
			}

			InitializeEntitiesAndCollections(
				hydratedObjects,
				resultSet,
				session );
			session.InitializeNonLazyCollections();
			return result;
		}

		// Not ported: sequentialLoad, loadSequentialRowsForward, loadSequentialRowsReverse

		private static EntityKey GetOptionalObjectKey( QueryParameters queryParameters, ISessionImplementor session )
		{
			object optionalObject = queryParameters.OptionalObject;
			object optionalId = queryParameters.OptionalId;
			System.Type optionalEntityClass = queryParameters.OptionalEntityClass;

			if ( optionalObject != null && optionalEntityClass != null ) 
			{
				return new EntityKey( 
					optionalId,
					session.GetEntityPersister( optionalObject ) // TODO H3: session.GetEntityPersister( optionalEntityName, optionalObject )
					// TODO H3: session.getEntityMode()
					);
			}
			else 
			{
				return null;
			}

		}

		private object GetRowFromResultSet(
			IDataReader resultSet,
			ISessionImplementor session,
			QueryParameters queryParameters,
			LockMode[] lockModeArray,
			EntityKey optionalObjectKey,
			IList hydratedObjects,
			EntityKey[ ] keys,
			bool returnProxies )
		{
			ILoadable[ ] persisters = EntityPersisters;
			int entitySpan = persisters.Length;

			for( int i = 0; i < entitySpan; i++ )
			{
				keys[ i ] = GetKeyFromResultSet(
					i,
					persisters[ i ],
					i == entitySpan - 1 ?
						queryParameters.OptionalId : null,
					resultSet,
					session );
				//TODO: the i==entitySpan-1 bit depends upon subclass implementation (very bad)
			}

			RegisterNonExists( keys, persisters, session );

			// this call is side-effecty
			object[ ] row = GetRow(
				resultSet,
				persisters,
				keys,
				queryParameters.OptionalObject,
				optionalObjectKey,
				lockModeArray,
				hydratedObjects,
				session );

			ReadCollectionElements( row, resultSet, session );

			if( returnProxies )
			{
				// now get an existing proxy for each row element (if there is one)
				for( int i = 0; i < entitySpan; i++ )
				{
					object entity = row[ i ];
					object proxy = session.ProxyFor( persisters[ i ], keys[ i ], entity );
 
					if( entity != proxy )
					{
						// Force the proxy to resolve itself
						NHibernateProxyHelper.GetLazyInitializer( ( INHibernateProxy ) proxy )
							.SetImplementation( entity );
						row[ i ] = proxy;
					}
				}
			}

			return GetResultColumnOrRow( row, queryParameters.ResultTransformer, resultSet, session );
		}

		/// <summary>
		/// Read any collection elements contained in a single row of the result set
		/// </summary>
		private void ReadCollectionElements( object[] row, IDataReader resultSet, ISessionImplementor session )
		{
			//TODO: make this handle multiple collection roles!

			ICollectionPersister[] collectionPersisters = CollectionPersisters;

			if( collectionPersisters != null )
			{
				ICollectionAliases[] descriptors = CollectionAliases;
				int[] collectionOwners = CollectionOwners;

				for( int i = 0; i < collectionPersisters.Length; i++ )
				{
					bool hasCollectionOwners = collectionOwners != null
						&& collectionOwners[ i ] > -1;
					//true if this is a query and we are loading multiple instances of the same collection role
					//otherwise this is a CollectionInitializer and we are loading up a single collection or batch

					object owner = hasCollectionOwners ? row[ collectionOwners[ i ] ] :
						null; //if null, owner will be retrieved from session

					ICollectionPersister collectionPersister = collectionPersisters[ i ];
					object key;
					
					if( owner == null )
					{
						key = null;
					}
					else
					{
						key = collectionPersister.CollectionType.GetKeyOfOwner( owner, session );
						//TODO: old version did not require hashmap lookup:
						//keys[collectionOwner].getIdentifier()
					}
				
					ReadCollectionElement(
						owner,
						key,
						collectionPersister,
						descriptors[ i ],
						resultSet,
						session );
				}
			}
		}

		private IList DoQuery(
			ISessionImplementor session,
			QueryParameters queryParameters,
			bool returnProxies )
		{
			RowSelection selection = queryParameters.RowSelection;
			int maxRows = HasMaxRows( selection ) ?
				selection.MaxRows :
				int.MaxValue;

			int entitySpan = EntityPersisters.Length;

			ArrayList hydratedObjects = entitySpan > 0 ? new ArrayList() : null;
			SqlString sql = ApplyLocks( SqlString, queryParameters.LockModes, session.Factory.Dialect );
			IDbCommand st = PrepareQueryCommand(
				sql,
				queryParameters, false, session );

			IDataReader rs = GetResultSet( st, selection, session );

// would be great to move all this below here into another method that could also be used
// from the new scrolling stuff.
//
// Would need to change the way the max-row stuff is handled (i.e. behind an interface) so
// that I could do the control breaking at the means to know when to stop
			LockMode[] lockModeArray = GetLockModes( queryParameters.LockModes );
			EntityKey optionalObjectKey = GetOptionalObjectKey( queryParameters, session );

			bool createSubselects = IsSubselectLoadingEnabled;
			IList subselectResultKeys = createSubselects ? new ArrayList() : null;
			IList results = new ArrayList();

			try
			{
				HandleEmptyCollections( queryParameters.CollectionKeys, rs, session );
				EntityKey[ ] keys = new EntityKey[ entitySpan ]; // we can reuse it each time

				if( log.IsDebugEnabled )
				{
					log.Debug( "processing result set" );
				}

				int count;
				for( count = 0; count < maxRows && rs.Read(); count++ )
				{
					if( log.IsDebugEnabled )
					{
						log.Debug( "result set row: " + count );
					}

					object result = GetRowFromResultSet(
						rs,
						session,
						queryParameters,
						lockModeArray,
						optionalObjectKey,
						hydratedObjects,
						keys,
						returnProxies );
					results.Add( result );

					if (createSubselects)
					{
						subselectResultKeys.Add(keys);
						keys = new EntityKey[entitySpan]; //can't reuse in this case
					}
				}

				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "done processing result set ({0} rows)", count ) );
				}
			}
			finally
			{
				session.Batcher.CloseQueryCommand( st, rs );
			}

			InitializeEntitiesAndCollections( hydratedObjects, rs, session );

			if( createSubselects )
			{
				CreateSubselects( subselectResultKeys, queryParameters, session );
			}

			return results; // GetResultList( results );
		}

		protected virtual bool IsSubselectLoadingEnabled
		{
			get { return false; }
		}
		
		protected bool HasSubselectLoadableCollections()
		{
			foreach (ILoadable loadable in EntityPersisters)
			{
				if (loadable.HasSubselectLoadableCollections)
				{
					return true;
				}
			}

			return false;
		}

		private static ISet[] Transpose(IList keys)
		{
			ISet[] result = new ISet[((EntityKey[]) keys[0]).Length];
			for (int j = 0; j < result.Length; j++)
			{
				result[j] = new HashedSet();
				for (int i = 0; i < keys.Count; i++)
				{
					object key = ((EntityKey[]) keys[i])[j];
					if (key != null)
					{
						result[j].Add(key);
					}
				}
			}
			return result;
		}

		private void CreateSubselects(IList keys, QueryParameters queryParameters, ISessionImplementor session)
		{
			if (keys.Count > 1)
			{
				//if we only returned one entity, query by key is more efficient

				ISet[] keySets = Transpose(keys);

				IDictionary namedParameterLocMap = BuildNamedParameterLocMap(queryParameters);

				ILoadable[] loadables = EntityPersisters;
				string[] aliases = Aliases;

				foreach (EntityKey[] rowKeys in keys)
				{
					for (int i = 0; i < rowKeys.Length; i++)
					{

						if (rowKeys[i] != null && loadables[i].HasSubselectLoadableCollections)
						{
							SubselectFetch subselectFetch = new SubselectFetch(
								//getSQLString(), 
								aliases[i],
								loadables[i],
								queryParameters,
								keySets[i],
								namedParameterLocMap
								);

							session.BatchFetchQueue.AddSubselect(rowKeys[i], subselectFetch);
						}
					}
				}
			}
		}

		private IDictionary BuildNamedParameterLocMap(QueryParameters queryParameters)
		{
			if (queryParameters.NamedParameters != null)
			{
				IDictionary namedParameterLocMap = new Hashtable();
				foreach (string name in queryParameters.NamedParameters.Keys)
				{
					namedParameterLocMap[name] = GetNamedParameterLocs(name);
				}
				return namedParameterLocMap;
			}
			else
			{
				return null;
			}
		}

		private void InitializeEntitiesAndCollections(
			IList hydratedObjects,
			object resultSetId,
			ISessionImplementor session )
		{
			ICollectionPersister[] collectionPersisters = CollectionPersisters;
			if( collectionPersisters != null )
			{
				for( int i = 0; i < collectionPersisters.Length; i++)
				{
					if( collectionPersisters[ i ].IsArray )
					{
						//for arrays, we should end the collection load before resolving
						//the entities, since the actual array instances are not instantiated
						//during loading
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad( resultSetId, session, collectionPersisters[i] );
					}
				}
			}

			if( hydratedObjects != null )
			{
				int hydratedObjectsSize = hydratedObjects.Count;

				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "total objects hydrated: {0}", hydratedObjectsSize ) );
				}

				for( int i = 0; i < hydratedObjectsSize; i++ )
				{
					session.InitializeEntity( hydratedObjects[ i ] );
				}
			}

			if( collectionPersisters != null )
			{
				for( int i = 0; i < collectionPersisters.Length; i++ )
				{
					if ( !collectionPersisters[ i ].IsArray ) 
					{
						//for sets, we should end the collection load after resolving
						//the entities, since we might call hashCode() on the elements
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad( resultSetId, session, collectionPersisters[i] );
					}
				}
			}
		}

		private void EndCollectionLoad(
			object resultSetId,
			ISessionImplementor session,
			ICollectionPersister collectionPersister )
		{
			session.EndLoadingCollections( collectionPersister, resultSetId );
		}

		protected virtual IList GetResultList( IList results, IResultTransformer resultTransformer )
		{
			return results;
		}

		/// <summary>
		/// Get the actual object that is returned in the user-visible result list.
		/// </summary>
		/// <remarks>
		/// This empty implementation merely returns its first argument. This is
		/// overridden by some subclasses.
		/// </remarks>
		protected virtual object GetResultColumnOrRow( object[ ] row, IResultTransformer resultTransformer, IDataReader rs, ISessionImplementor session )
		{
			return row;
		}

		/// <summary>
		/// For missing objects associated by one-to-one with another object in the
		/// result set, register the fact that the the object is missing with the
		/// session.
		/// </summary>
		private void RegisterNonExists(
			EntityKey[ ] keys,
			ILoadable[ ] persisters,
			ISessionImplementor session
			)
		{
			int[] owners = Owners;
			if( owners != null )
			{
				EntityType[] ownerAssociationTypes = OwnerAssociationTypes;
				for( int i = 0; i < keys.Length; i++ )
				{
					int owner = owners[ i ];
					if( owner > -1 )
					{
						EntityKey ownerKey = keys[ owner ];
						if( keys[ i ] == null && ownerKey != null )
						{
							//bool isOneToOneAssociation = ownerAssociationTypes != null &&
							//	ownerAssociationTypes[ i ] != null &&
							//	ownerAssociationTypes[ i ].IsOneToOne;

							//if( isOneToOneAssociation )
							//{
							// Added to fix NH-687, not in Hibernate:
							bool isUniqueKeyReference = ownerAssociationTypes != null &&
								ownerAssociationTypes[i] != null &&
								ownerAssociationTypes[i].IsUniqueKeyReference;
							if (!isUniqueKeyReference)
							{
								session.AddNonExist(new EntityKey(ownerKey.Identifier, persisters[i]));
							}
							//}
						}
					}
				}
			}
		}

		/// <summary>
		/// Read one collection element from the current row of the ADO.NET result set
		/// </summary>
		private void ReadCollectionElement(
			object optionalOwner,
			object optionalKey,
			ICollectionPersister persister,
			ICollectionAliases descriptor,
			IDataReader rs,
			ISessionImplementor session
			)
		{
			object collectionRowKey = persister.ReadKey(
				rs,
				descriptor.SuffixedKeyAliases,
				session );

			if( collectionRowKey != null )
			{
				// we found a collection element in the result set

				if( log.IsDebugEnabled )
				{
					log.Debug(
						"found row of collection: " +
						MessageHelper.InfoString( persister, collectionRowKey ) );
				}
				
				object owner = optionalOwner;
				if( owner == null )
				{
					owner = session.GetCollectionOwner( collectionRowKey, persister );
					if( owner == null )
					{
						//TODO: This is assertion is disabled because there is a bug that means the
						//      original owner of a transient, uninitialized collection is not known 
						//      if the collection is re-referenced by a different object associated 
						//      with the current Session
						//throw new AssertionFailure("bug loading unowned collection");
					}
				}
				IPersistentCollection rowCollection = session.GetLoadingCollection( persister, collectionRowKey, rs );

				if( rowCollection != null )
				{
					rowCollection.ReadFrom( rs, persister, descriptor, owner );
				}
			}
			else if( optionalKey != null )
			{
				// we did not find a collection element in the result set, so we
				// ensure that a collection is created with the owner's identifier,
				// since what we have is an empty collection

				if( log.IsDebugEnabled )
				{
					log.Debug(
						"result set contains (possibly empty) collection: " +
						MessageHelper.InfoString( persister, optionalKey ) );
				}
				session.GetLoadingCollection( persister, optionalKey, rs ); // handle empty collection
			}
			
			// else no collection element, but also no owner
		}

		/// <summary>
		/// If this is a collection initializer, we need to tell the session that a collection
		/// is being initilized, to account for the possibility of the collection having
		/// no elements (hence no rows in the result set).
		/// </summary>
		private void HandleEmptyCollections(
			object[ ] keys,
			object resultSetId,
			ISessionImplementor session
			)
		{
			if( keys != null )
			{
				// this is a collection initializer, so we must create a collection
				// for each of the passed-in keys, to account for the possibility
				// that the collection is empty and has no rows in the result set

				ICollectionPersister[] collectionPersisters = CollectionPersisters;
				for( int j = 0; j < collectionPersisters.Length; j++ )
				{
					for( int i = 0; i < keys.Length; i++ )
					{
						// handle empty collections
						if( log.IsDebugEnabled )
						{
							log.Debug( "result set contains (possibly empty) collection: " +
								MessageHelper.InfoString( collectionPersisters[ j ], keys[ i ] ) );
						}
						session.GetLoadingCollection( collectionPersisters[ j ], keys[ i ], resultSetId );
					}
				}
			}
		}

		/// <summary>
		/// Read a row of <c>EntityKey</c>s from the <c>IDataReader</c> into the given array.
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
		private EntityKey GetKeyFromResultSet( int i, ILoadable persister, object id, IDataReader rs, ISessionImplementor session )
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
				resultId = persister.IdentifierType.NullSafeGet( rs, EntityAliases[ i ].SuffixedKeyAliases, session, null );

				if( id != null && resultId != null && id.Equals( resultId ) )
				{
					// use the id passed in
					resultId = id;
				}
			}

			return ( resultId == null ) ? null : new EntityKey( resultId, persister );
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
		private void CheckVersion(
			int i,
			ILoadable persister,
			object id,
			object version,
			IDataReader rs,
			ISessionImplementor session )
		{
			// null version means the object is in the process of being loaded somewhere
			// else in the ResultSet
			if( version != null )
			{
				IType versionType = persister.VersionType;
				object currentVersion = versionType.NullSafeGet( rs, EntityAliases[ i ].SuffixedVersionAliases, session, null );
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
		private object[ ] GetRow(
			IDataReader rs,
			ILoadable[ ] persisters,
			EntityKey[ ] keys,
			object optionalObject,
			EntityKey optionalObjectKey,
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
				EntityKey key = keys[ i ];

				if( keys[ i ] == null )
				{
					// do nothing
				}
				else
				{
					//If the object is already loaded, return the loaded one
					obj = session.GetEntity( key );
					if( obj != null )
					{
						//its already loaded so dont need to hydrate it
						InstanceAlreadyLoaded( rs, i, persisters[ i ], key, obj, lockModes[ i ], session );
					}
					else
					{
						obj = InstanceNotYetLoaded( rs, i, persisters[ i ], key, lockModes[ i ], optionalObjectKey, optionalObject, hydratedObjects, session );
					}
				}

				rowResults[ i ] = obj;
			}
			return rowResults;
		}

		/// <summary>
		/// The entity instance is already in the session cache
		/// </summary>
		private void InstanceAlreadyLoaded(
			IDataReader rs,
			int i,
			ILoadable persister,
			EntityKey key,
			object obj,
			LockMode lockMode,
			ISessionImplementor session )
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
		private object InstanceNotYetLoaded( IDataReader dr, int i, ILoadable persister, EntityKey key, LockMode lockMode, EntityKey optionalObjectKey, object optionalObject, IList hydratedObjects, ISessionImplementor session )
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
			LoadFromResultSet( dr, i, obj, instanceClass, key, acquiredLockMode, persister, session );

			// materialize associations (and initialize the object) later
			hydratedObjects.Add( obj );

			return obj;
		}

		/// <summary>
		/// Hydrate the state of an object from the SQL <c>IDataReader</c>, into
		/// an array of "hydrated" values (do not resolve associations yet),
		/// and pass the hydrated state to the session.
		/// </summary>
		private void LoadFromResultSet(
			IDataReader rs,
			int i,
			object obj,
			System.Type instanceClass,
			EntityKey key,
			LockMode lockMode,
			ILoadable rootPersister,
			ISessionImplementor session )
		{
			object id = key.Identifier;

			// Get the persister for the _subclass_
			ILoadable persister = ( ILoadable ) Factory.GetEntityPersister( instanceClass );

			if( log.IsDebugEnabled )
			{
				log.Debug( "Initializing object from DataReader: " + MessageHelper.InfoString( persister, id ) );
			}

			// add temp entry so that the next step is circular-reference
			// safe - only needed because some types don't take proper
			// advantage of two-phase-load (esp. components)
			session.AddUninitializedEntity( key, obj, lockMode );

			// This is not very nice (and quite slow):
			string[ ][ ] cols = persister == rootPersister ?
				EntityAliases[ i ].SuffixedPropertyAliases :
				EntityAliases[ i ].GetSuffixedPropertyAliases( persister );

			object[ ] values = Hydrate(
				rs,
				id,
				obj,
				persister,
				session,
				cols );

			// TODO H3:
//			IAssociationType[] ownerAssociationTypes = OwnerAssociationTypes;
//
//			if ( ownerAssociationTypes != null && ownerAssociationTypes[i] != null ) 
//			{
//				string ukName = ownerAssociationTypes[i].RHSUniqueKeyPropertyName;
//				if (ukName!=null) 
//				{
//					int index = ( (IUniqueKeyLoadable) persister ).GetPropertyIndex(ukName);
//					IType type = persister.PropertyTypes[index];
//	
//					// polymorphism not really handled completely correctly,
//					// perhaps...well, actually its ok, assuming that the
//					// entity name used in the lookup is the same as the
//					// the one used here, which it will be
//	
//					EntityUniqueKey euk = new EntityUniqueKey( 
//						rootPersister.MappedClass, //polymorphism comment above
//						ukName,
//						type.SemiResolve( values[ index ], session, obj ),
//						type,
//						session.EntityMode, session.Factory
//						);
//					session.AddEntity( euk, obj );
//				}
//			}

			session.PostHydrate( persister, id, values, obj, lockMode );
		}

		/// <summary>
		/// Determine the concrete class of an instance for the <c>IDataReader</c>
		/// </summary>
		private System.Type GetInstanceClass(
			IDataReader rs,
			int i,
			ILoadable persister,
			object id,
			ISessionImplementor session )
		{
			System.Type topClass = persister.MappedClass;

			if( persister.HasSubclasses )
			{
				// code to handle subclasses of topClass
				object discriminatorValue = persister.DiscriminatorType.NullSafeGet(
					rs, EntityAliases[ i ].SuffixedDiscriminatorAlias, session, null );

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
		/// Unmarshall the fields of a persistent instance from a result set,
		/// without resolving associations or collections
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <param name="suffixedPropertyColumns"></param>
		/// <returns></returns>
		private object[ ] Hydrate(
			IDataReader rs,
			object id,
			object obj,
			ILoadable persister,
			ISessionImplementor session,
			string[ ][ ] suffixedPropertyColumns )
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

		private static bool HasMaxRows( RowSelection selection )
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
			return dialect.SupportsLimit && HasMaxRows( selection );
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
			object[ ] values = queryParameters.FilteredPositionalParameterValues;
            IType[] types = queryParameters.FilteredPositionalParameterTypes;
            
            int span = 0;
			for( int i = 0; i < values.Length; i++ )
			{
				types[ i ].NullSafeSet( st, values[ i ], start + span, session );
                span += types[i].GetColumnSpan(session.Factory);
			}

			return span;
		}

		/// <summary>
		/// Obtain an <c>IDbCommand</c> with all parameters pre-bound. Bind positional parameters,
		/// named parameters, and limit parameters.
		/// </summary>
		/// <remarks>
		/// Creates an IDbCommand object and populates it with the values necessary to execute it against the 
		/// database to Load an Entity.
		/// </remarks>
		/// <param name="sqlString">The SqlString to convert into a prepared IDbCommand.</param>
		/// <param name="parameters">The <see cref="QueryParameters"/> to use for the IDbCommand.</param>
		/// <param name="scroll">TODO: find out where this is used...</param>
		/// <param name="session">The SessionImpl this Command is being prepared in.</param>
		/// <returns>A CommandWrapper wrapping an IDbCommand that is ready to be executed.</returns>
		protected virtual IDbCommand PrepareQueryCommand(
			SqlString sqlString,
			QueryParameters parameters,
			bool scroll,
			ISessionImplementor session )
		{
            parameters.ProcessFilters(sqlString, session);
            sqlString = parameters.FilteredSQL;
            Dialect.Dialect dialect = session.Factory.Dialect;

			RowSelection selection = parameters.RowSelection;
			bool useLimit = UseLimit( selection, dialect );
			bool hasFirstRow = GetFirstRow( selection ) > 0;
			bool useOffset = hasFirstRow && useLimit && dialect.SupportsLimitOffset;

			if( useLimit )
			{
				sqlString = dialect.GetLimitString( sqlString.Trim(),
				                                    useOffset ? GetFirstRow( selection ) : 0,
				                                    GetMaxOrLimit( dialect, selection ) );
			}
            IDbCommand command = session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, GetParameterTypes(parameters, useLimit, useOffset));

			try
			{
				// Added in NH - not in H2.1
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
				session.Batcher.CloseQueryCommand( command, null );
				throw;
			}
			catch( Exception sqle )
			{
				ADOExceptionReporter.LogExceptions( sqle );
				session.Batcher.CloseQueryCommand( command, null );
				throw;
			}

			return command;
		}

		/// <summary>
		/// Some dialect-specific LIMIT clauses require the maximum last row number,
		/// others require the maximum returned row count.
		/// </summary>
		private static int GetMaxOrLimit( Dialect.Dialect dialect, RowSelection selection )
		{
			int firstRow = GetFirstRow( selection );
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
		private int BindLimitParameters( IDbCommand st, int index, RowSelection selection, ISessionImplementor session )
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
				// TODO: Add WrapResultSetIfEnabled below
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
				session.Batcher.CloseQueryCommand( st, rs );
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
		/// Bind named parameters to the <c>IDbCommand</c>
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand"/> that contains the parameters.</param>
		/// <param name="namedParams">The named parameters (key) and the values to set.</param>
		/// <param name="session">The <see cref="ISession"/> this Loader is using.</param>
		/// <param name="start"></param>
		protected virtual int BindNamedParameters( IDbCommand st, IDictionary namedParams, int start, ISessionImplementor session )
		{
			if( namedParams != null )
			{
				// assumes that types are all of span 1
				int result = 0;
				foreach( DictionaryEntry e in namedParams )
				{
					string name = ( string ) e.Key;
					TypedValue typedval = ( TypedValue ) e.Value;
					int[] locs = GetNamedParameterLocs( name );
					for( int i = 0; i < locs.Length; i++ )
					{
						if( log.IsDebugEnabled )
						{
							log.Debug(
								"BindNamedParameters() " +
								typedval.Value + " -> " + name +
								" [" + ( locs[ i ] + start ) + "]" );
						}
						typedval.Type.NullSafeSet( st, typedval.Value, locs[ i ] + start, session );
					}
					result += locs.Length;
				}
				return result;
			}
			else
			{
				return 0;
			}
		}

		public virtual int[] GetNamedParameterLocs( string name )
		{
			throw new AssertionFailure( "no named parameters" );
		}

		/// <summary>
		/// Called by subclasses that load entities
		/// </summary>
		protected IList LoadEntity(
			ISessionImplementor session,
			object id,
			IType identifierType,
			object optionalObject,
			System.Type optionalEntityName,
			object optionalIdentifier,
			IEntityPersister persister )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug(
					"loading entity: " +
					MessageHelper.InfoString( persister, id, identifierType, Factory )
					);
			}

			IList result;

			try
			{
				QueryParameters qp = new QueryParameters(
					new IType[] { identifierType },
					new object[] { id },
					optionalObject,
					optionalEntityName, optionalIdentifier
					);
				result = DoQueryAndInitializeNonLazyCollections(
					session,
					qp,
					false );
			}
			catch( HibernateException )
			{
				throw;
			}
			catch( Exception sqle )
			{
				ILoadable[] persisters = EntityPersisters;
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not load an entity: " +
					MessageHelper.InfoString( persisters[ persisters.Length - 1 ], id, identifierType, Factory ),
					SqlString );
			}

			log.Debug( "done entity load" );

			return result;
		}

		/// <summary>
		/// Called by subclasses that batch load entities
		/// </summary>
		protected internal IList LoadEntityBatch(
			ISessionImplementor session,
			object[ ] ids,
			IType idType,
			object optionalObject,
			System.Type optionalEntityName,
			object optionalId,
			IEntityPersister persister )
		{
			if( log.IsDebugEnabled )
			{
				log.Debug(
					"batch loading entity: " +
					MessageHelper.InfoString( persister, ids, Factory )
					);
			}

			IType[ ] types = new IType[ids.Length];
			for( int i = 0; i < types.Length; i++ )
			{
				types[ i ] = idType;
			}
			
			IList result;
			
			try
			{
				result = DoQueryAndInitializeNonLazyCollections(
					session,
					new QueryParameters( types, ids, optionalObject, optionalEntityName, optionalId ),
					false
					);
			}
			catch( HibernateException )
			{
				throw;
			}
			catch( Exception sqle )
			{
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not load an entity batch: " +
					// NH: Hibernate3 passes EntityPersisters[0] instead of persister,
					// I think it's wrong.
					MessageHelper.InfoString( persister, ids, Factory ),
					SqlString );
			}
			
			log.Debug( "done entity batch load" );
			return result;
		}

		/// <summary>
		/// Called by subclasses that load collections
		/// </summary>
		public void LoadCollection(
			ISessionImplementor session,
			object id,
			IType type )
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( 
					"loading collection: "+ 
					MessageHelper.InfoString( CollectionPersisters[0], id )
					);
			}

			object[] ids = new object[]{ id };
			try 
			{
				DoQueryAndInitializeNonLazyCollections( 
					session,
					new QueryParameters( new IType[]{ type }, ids, ids ),
					true
					);
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle ) 
			{
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not initialize a collection: " + 
					MessageHelper.InfoString( CollectionPersisters[0], id ),
					SqlString
					);
			}
	
			log.Debug("done loading collection");
		}

		/// <summary>
		/// Called by wrappers that batch initialize collections
		/// </summary>
		public void LoadCollectionBatch(
			ISessionImplementor session,
			object[ ] ids,
			IType type )
		{
			if ( log.IsDebugEnabled ) 
			{
				log.Debug( 
					"batch loading collection: "+ 
					MessageHelper.InfoString( CollectionPersisters[0], ids )
					);
			}

			IType[] idTypes = ArrayHelper.FillArray( type, ids.Length );
			try 
			{
				DoQueryAndInitializeNonLazyCollections( 
					session,
					new QueryParameters( idTypes, ids, ids ),
					true 
					);
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle ) 
			{
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not initialize a collection batch: " + 
					MessageHelper.InfoString( CollectionPersisters[0], ids ),
					SqlString
					);
			}
		
			log.Debug("done batch load");
		}

		/// <summary>
		/// Called by subclasses that batch initialize collections
		/// </summary>
		protected void LoadCollectionSubselect(
				ISessionImplementor session,
				object[] ids,
				object[] parameterValues,
				IType[] parameterTypes,
				IDictionary namedParameters,
				IType type)
		{
			try
			{
				DoQueryAndInitializeNonLazyCollections(
					session,
					new QueryParameters(parameterTypes, parameterValues, namedParameters, ids),
					true
					);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not load collection by subselect: " +
					MessageHelper.InfoString(CollectionPersisters[0], ids),
					SqlString
					);
			}
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
			IType[ ] resultTypes )
		{
			bool cacheable = factory.IsQueryCacheEnabled &&
			                 queryParameters.Cacheable;

			if( cacheable )
			{
				return ListUsingQueryCache( session, queryParameters, querySpaces, resultTypes );
			}
			else
			{
				return ListIgnoreQueryCache( session, queryParameters );
			}
		}

		private IList ListIgnoreQueryCache( ISessionImplementor session, QueryParameters queryParameters )
		{
			return GetResultList( DoList( session, queryParameters ), queryParameters.ResultTransformer );
		}

		private IList ListUsingQueryCache(
			ISessionImplementor session,
			QueryParameters queryParameters,
			ISet querySpaces,
			IType[ ] resultTypes )
		{
			IQueryCache queryCache = factory.GetQueryCache( queryParameters.CacheRegion );

			ISet filterKeys = FilterKey.CreateFilterKeys( 
					session.EnabledFilters
				);
			QueryKey key = new QueryKey( SqlString, queryParameters, filterKeys );

			IList result = GetResultFromQueryCache( session, queryParameters, querySpaces, resultTypes, queryCache, key );
			
			if( result == null )
			{
				result = DoList( session, queryParameters );
				queryCache.Put( key, resultTypes, result, session );
			}

			return GetResultList( result, queryParameters.ResultTransformer );
		}

		private static IList GetResultFromQueryCache( ISessionImplementor session, QueryParameters queryParameters, ISet querySpaces, IType[ ] resultTypes, IQueryCache queryCache, QueryKey key )
		{
			if( !queryParameters.ForceCacheRefresh )
			{
				return queryCache.Get( key, resultTypes, querySpaces, session );
			}
			return null;
		}

		/// <summary>
		/// Actually execute a query, ignoring the query cache
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <returns></returns>
		protected IList DoList( ISessionImplementor session, QueryParameters queryParameters )
		{
			try
			{
				return DoQueryAndInitializeNonLazyCollections(
					session, queryParameters, true );
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw ADOExceptionHelper.Convert(
					sqle,
					"could not execute query",
					SqlString );
			}
		}

		/// <summary>
		/// Calculate and cache select-clause suffixes. Must be
		/// called by subclasses after instantiation.
		/// </summary>
		protected virtual void PostInstantiate()
		{
		}

		/// <summary>
		/// Get the result set descriptor
		/// </summary>
		protected abstract IEntityAliases[] EntityAliases { get; }
		protected abstract ICollectionAliases[] CollectionAliases { get; }

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + SqlString + ')';
		}

		protected SqlType[] ConvertITypesToSqlTypes(ArrayList nhTypes, int totalSpan)
		{
			SqlType[] result = new SqlType[totalSpan];

			int index = 0;
			foreach (IType type in nhTypes)
			{
				int span = type.SqlTypes(Factory).Length;
				Array.Copy(type.SqlTypes(Factory), 0, result, index, span);
				index += span;
			}

			return result;
		}
		
		/// <returns><see cref="IList" /> of <see cref="IType" /></returns>
		protected SqlType[] GetParameterTypes( QueryParameters parameters, bool addLimit, bool addOffset )
		{
			ArrayList paramTypeList = new ArrayList();
			int span = 0;

			foreach( IType type in parameters.PositionalParameterTypes )
			{
				paramTypeList.Add(type);
				span += type.GetColumnSpan(Factory);
			}

			if( parameters.NamedParameters != null && parameters.NamedParameters.Count > 0 )
			{
				int offset = paramTypeList.Count;

				// convert the named parameters to an array of types
				foreach( DictionaryEntry e in parameters.NamedParameters )
				{
					string name = ( string ) e.Key;
					TypedValue typedval = ( TypedValue ) e.Value;
					int[ ] locs = GetNamedParameterLocs( name );
					span += typedval.Type.GetColumnSpan(Factory) * locs.Length;

					for( int i = 0; i < locs.Length; i++ )
					{
						ArrayHelper.SafeSetValue(paramTypeList, locs[ i ] + offset, typedval.Type);
					}
				}
			}
			
			if (addLimit && Factory.Dialect.SupportsVariableLimit)
			{
				if (Factory.Dialect.BindLimitParametersFirst)
				{
					paramTypeList.Insert(0, NHibernateUtil.Int32);
					if (addOffset)
					{
						paramTypeList.Insert(0, NHibernateUtil.Int32);
					}
				}
				else
				{
					paramTypeList.Add(NHibernateUtil.Int32);
					if (addOffset)
					{
						paramTypeList.Add(NHibernateUtil.Int32);
					}
				}

				span += addOffset ? 2 : 1;
			}
			
			return ConvertITypesToSqlTypes(paramTypeList, span);
		}
	}
}