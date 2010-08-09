using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Iesi.Collections;
using Iesi.Collections.Generic;

using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql.Util;
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
	/// <seealso cref="NHibernate.Persister.Entity.ILoadable"/>
	public abstract class Loader
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (Loader));

		private readonly ISessionFactoryImplementor factory;
		private ColumnNameCache columnNameCache;
		protected SessionFactoryHelper helper;

		public Loader(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
			helper = new SessionFactoryHelper(factory);
		}

		/// <summary> 
		/// An array indicating whether the entities have eager property fetching
		/// enabled. 
		/// </summary>
		/// <value> Eager property fetching indicators. </value>
		protected virtual bool[] EntityEagerPropertyFetches
		{
			get { return null; }
		}

		/// <summary>
		/// An array of indexes of the entity that owns a one-to-one association
		/// to the entity at the given index (-1 if there is no "owner")
		/// </summary>
		/// <remarks>
		/// The indexes contained here are relative to the result of <see cref="EntityPersisters"/>.
		/// </remarks>
		protected virtual int[] Owners
		{
			get { return null; }
		}

		/// <summary> 
		/// An array of the owner types corresponding to the <see cref="Owners"/>
		/// returns.  Indices indicating no owner would be null here. 
		/// </summary>
		protected virtual EntityType[] OwnerAssociationTypes
		{
			get { return null; }
		}

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
		/// Return false is this loader is a batch entity loader
		/// </summary>
		protected virtual bool IsSingleRowLoader
		{
			get { return false; }
		}

		public virtual bool IsSubselectLoadingEnabled
		{
			get { return false; }
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
		public abstract SqlString SqlString { get; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		/// <remarks>
		/// The <c>setter</c> was added so that classes inheriting from Loader could write a 
		/// value using the Property instead of directly to the field.
		/// </remarks>
		public abstract ILoadable[] EntityPersisters { get; }

		/// <summary>
		/// An (optional) persister for a collection to be initialized; only collection loaders
		/// return a non-null value
		/// </summary>
		protected virtual ICollectionPersister[] CollectionPersisters
		{
			get { return null; }
		}

		/// <summary>
		/// What lock mode does this load entities with?
		/// </summary>
		/// <param name="lockModes">A Collection of lock modes specified dynamically via the Query Interface</param>
		/// <returns></returns>
		public abstract LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes);

		/// <summary>
		/// Append <c>FOR UPDATE OF</c> clause, if necessary. This
		/// empty superclass implementation merely returns its first
		/// argument.
		/// </summary>
		protected virtual SqlString ApplyLocks(SqlString sql, IDictionary<string, LockMode> lockModes, Dialect.Dialect dialect)
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
		protected virtual SqlString PreprocessSQL(SqlString sql, QueryParameters parameters, Dialect.Dialect dialect)
		{
			sql = ApplyLocks(sql, parameters.LockModes, dialect);

			return Factory.Settings.IsCommentsEnabled ? PrependComment(sql, parameters) : sql;
		}

		private static SqlString PrependComment(SqlString sql, QueryParameters parameters)
		{
			string comment = parameters.Comment;
			if (string.IsNullOrEmpty(comment))
			{
				return sql;
			}
			else
			{
				return sql.Insert(0, "/* " + comment + " */");
			}
		}

		/// <summary>
		/// Execute an SQL query and attempt to instantiate instances of the class mapped by the given
		/// persister from each row of the <c>DataReader</c>. If an object is supplied, will attempt to
		/// initialize that object. If a collection is supplied, attempt to initialize that collection.
		/// </summary>
		private IList DoQueryAndInitializeNonLazyCollections(ISessionImplementor session, QueryParameters queryParameters,
		                                                     bool returnProxies)
		{
			IPersistenceContext persistenceContext = session.PersistenceContext;

			persistenceContext.BeforeLoad();
			IList result;
			try
			{
				result = DoQuery(session, queryParameters, returnProxies);
			}
			finally
			{
				persistenceContext.AfterLoad();
			}
			persistenceContext.InitializeNonLazyCollections();

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
		protected object LoadSingleRow(IDataReader resultSet, ISessionImplementor session, QueryParameters queryParameters,
		                               bool returnProxies)
		{
			int entitySpan = EntityPersisters.Length;
			IList hydratedObjects = entitySpan == 0 ? null : new List<object>(entitySpan);

			object result;
			try
			{
				result =
					GetRowFromResultSet(resultSet, session, queryParameters, GetLockModes(queryParameters.LockModes), null,
					                    hydratedObjects, new EntityKey[entitySpan], returnProxies);
			}
			catch (HibernateException)
			{
				throw; // Don't call Convert on HibernateExceptions
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle, "could not read next row of results",
				                                 SqlString, queryParameters.PositionalParameterValues,
				                                 queryParameters.NamedParameters);
			}

			InitializeEntitiesAndCollections(hydratedObjects, resultSet, session, queryParameters.ReadOnly);
			session.PersistenceContext.InitializeNonLazyCollections();
			return result;
		}

		// Not ported: sequentialLoad, loadSequentialRowsForward, loadSequentialRowsReverse

		internal static EntityKey GetOptionalObjectKey(QueryParameters queryParameters, ISessionImplementor session)
		{
			object optionalObject = queryParameters.OptionalObject;
			object optionalId = queryParameters.OptionalId;
			string optionalEntityName = queryParameters.OptionalEntityName;

			if (optionalObject != null && !string.IsNullOrEmpty(optionalEntityName))
			{
				return new EntityKey(optionalId, session.GetEntityPersister(optionalEntityName, optionalObject), session.EntityMode);
			}
			else
			{
				return null;
			}
		}

		internal object GetRowFromResultSet(IDataReader resultSet, ISessionImplementor session,
		                                    QueryParameters queryParameters, LockMode[] lockModeArray,
		                                    EntityKey optionalObjectKey, IList hydratedObjects, EntityKey[] keys,
		                                    bool returnProxies)
		{
			ILoadable[] persisters = EntityPersisters;
			int entitySpan = persisters.Length;

			for (int i = 0; i < entitySpan; i++)
			{
				keys[i] =
					GetKeyFromResultSet(i, persisters[i], i == entitySpan - 1 ? queryParameters.OptionalId : null, resultSet, session);
				//TODO: the i==entitySpan-1 bit depends upon subclass implementation (very bad)
			}

			RegisterNonExists(keys, session);

			// this call is side-effecty
			object[] row =
				GetRow(resultSet, persisters, keys, queryParameters.OptionalObject, optionalObjectKey, lockModeArray,
				       hydratedObjects, session);

			ReadCollectionElements(row, resultSet, session);

			if (returnProxies)
			{
				// now get an existing proxy for each row element (if there is one)
				for (int i = 0; i < entitySpan; i++)
				{
					object entity = row[i];
					object proxy = session.PersistenceContext.ProxyFor(persisters[i], keys[i], entity);

					if (entity != proxy)
					{
						// Force the proxy to resolve itself
						((INHibernateProxy) proxy).HibernateLazyInitializer.SetImplementation(entity);
						row[i] = proxy;
					}
				}
			}

			return GetResultColumnOrRow(row, queryParameters.ResultTransformer, resultSet, session);
		}

		/// <summary>
		/// Read any collection elements contained in a single row of the result set
		/// </summary>
		private void ReadCollectionElements(object[] row, IDataReader resultSet, ISessionImplementor session)
		{
			//TODO: make this handle multiple collection roles!

			ICollectionPersister[] collectionPersisters = CollectionPersisters;

			if (collectionPersisters != null)
			{
				ICollectionAliases[] descriptors = CollectionAliases;
				int[] collectionOwners = CollectionOwners;

				for (int i = 0; i < collectionPersisters.Length; i++)
				{
					bool hasCollectionOwners = collectionOwners != null && collectionOwners[i] > -1;
					//true if this is a query and we are loading multiple instances of the same collection role
					//otherwise this is a CollectionInitializer and we are loading up a single collection or batch

					object owner = hasCollectionOwners ? row[collectionOwners[i]] : null;
						//if null, owner will be retrieved from session

					ICollectionPersister collectionPersister = collectionPersisters[i];
					object key;

					if (owner == null)
					{
						key = null;
					}
					else
					{
						key = collectionPersister.CollectionType.GetKeyOfOwner(owner, session);
						//TODO: old version did not require hashmap lookup:
						//keys[collectionOwner].getIdentifier()
					}

					ReadCollectionElement(owner, key, collectionPersister, descriptors[i], resultSet, session);
				}
			}
		}

		private IList DoQuery(ISessionImplementor session, QueryParameters queryParameters, bool returnProxies)
		{
			RowSelection selection = queryParameters.RowSelection;
			int maxRows = HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;

			int entitySpan = EntityPersisters.Length;

			List<object> hydratedObjects = entitySpan == 0 ? null : new List<object>(entitySpan * 10);

			IDbCommand st = PrepareQueryCommand(queryParameters, false, session);

			IDataReader rs = GetResultSet(st, queryParameters.HasAutoDiscoverScalarTypes, queryParameters.Callable, selection,
			                              session);

			// would be great to move all this below here into another method that could also be used
			// from the new scrolling stuff.
			//
			// Would need to change the way the max-row stuff is handled (i.e. behind an interface) so
			// that I could do the control breaking at the means to know when to stop
			LockMode[] lockModeArray = GetLockModes(queryParameters.LockModes);
			EntityKey optionalObjectKey = GetOptionalObjectKey(queryParameters, session);

			bool createSubselects = IsSubselectLoadingEnabled;
			List<EntityKey[]> subselectResultKeys = createSubselects ? new List<EntityKey[]>() : null;
			IList results = new List<object>();

			try
			{
				HandleEmptyCollections(queryParameters.CollectionKeys, rs, session);
				EntityKey[] keys = new EntityKey[entitySpan]; // we can reuse it each time

				if (log.IsDebugEnabled)
				{
					log.Debug("processing result set");
				}

				int count;
				for (count = 0; count < maxRows && rs.Read(); count++)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug("result set row: " + count);
					}

					object result = GetRowFromResultSet(rs, session, queryParameters, lockModeArray, optionalObjectKey, hydratedObjects,
					                                    keys, returnProxies);
					results.Add(result);

					if (createSubselects)
					{
						subselectResultKeys.Add(keys);
						keys = new EntityKey[entitySpan]; //can't reuse in this case
					}
				}

				if (log.IsDebugEnabled)
				{
					log.Debug(string.Format("done processing result set ({0} rows)", count));
				}
			}
			catch (Exception e)
			{
				e.Data["actual-sql-query"] = st.CommandText;
				throw;
			}
			finally
			{
				session.Batcher.CloseCommand(st, rs);
			}

			InitializeEntitiesAndCollections(hydratedObjects, rs, session, queryParameters.ReadOnly);

			if (createSubselects)
			{
				CreateSubselects(subselectResultKeys, queryParameters, session);
			}

			return results;
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

		private static ISet<EntityKey>[] Transpose(IList<EntityKey[]> keys)
		{
			ISet<EntityKey>[] result = new ISet<EntityKey>[keys[0].Length];
			for (int j = 0; j < result.Length; j++)
			{
				result[j] = new HashedSet<EntityKey>();
				for (int i = 0; i < keys.Count; i++)
				{
					EntityKey key = keys[i][j];
					if (key != null)
					{
						result[j].Add(key);
					}
				}
			}
			return result;
		}

		internal void CreateSubselects(IList<EntityKey[]> keys, QueryParameters queryParameters, ISessionImplementor session)
		{
			if (keys.Count > 1)
			{
				//if we only returned one entity, query by key is more efficient

				ISet<EntityKey>[] keySets = Transpose(keys);

				IDictionary<string, int[]> namedParameterLocMap = BuildNamedParameterLocMap(queryParameters);

				ILoadable[] loadables = EntityPersisters;
				string[] aliases = Aliases;

				foreach (EntityKey[] rowKeys in keys)
				{
					for (int i = 0; i < rowKeys.Length; i++)
					{
						if (rowKeys[i] != null && loadables[i].HasSubselectLoadableCollections)
						{
							SubselectFetch subselectFetch =
								new SubselectFetch(aliases[i], loadables[i], queryParameters, keySets[i], namedParameterLocMap);

							session.PersistenceContext.BatchFetchQueue.AddSubselect(rowKeys[i], subselectFetch);
						}
					}
				}
			}
		}

		private IDictionary<string, int[]> BuildNamedParameterLocMap(QueryParameters queryParameters)
		{
			if (queryParameters.NamedParameters != null)
			{
				IDictionary<string, int[]> namedParameterLocMap = new Dictionary<string, int[]>();
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

		internal void InitializeEntitiesAndCollections(IList hydratedObjects, object resultSetId, ISessionImplementor session,
		                                               bool readOnly)
		{
			ICollectionPersister[] collectionPersisters = CollectionPersisters;
			if (collectionPersisters != null)
			{
				for (int i = 0; i < collectionPersisters.Length; i++)
				{
					if (collectionPersisters[i].IsArray)
					{
						//for arrays, we should end the collection load before resolving
						//the entities, since the actual array instances are not instantiated
						//during loading
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad(resultSetId, session, collectionPersisters[i]);
					}
				}
			}
			//important: reuse the same event instances for performance!
			PreLoadEvent pre;
			PostLoadEvent post;
			if (session.IsEventSource)
			{
				var eventSourceSession = (IEventSource) session;
				pre = new PreLoadEvent(eventSourceSession);
				post = new PostLoadEvent(eventSourceSession);
			}
			else
			{
				pre = null;
				post = null;
			}

			if (hydratedObjects != null)
			{
				int hydratedObjectsSize = hydratedObjects.Count;

				if (log.IsDebugEnabled)
				{
					log.Debug(string.Format("total objects hydrated: {0}", hydratedObjectsSize));
				}

				for (int i = 0; i < hydratedObjectsSize; i++)
				{
					TwoPhaseLoad.InitializeEntity(hydratedObjects[i], readOnly, session, pre, post);
				}
			}

			if (collectionPersisters != null)
			{
				for (int i = 0; i < collectionPersisters.Length; i++)
				{
					if (!collectionPersisters[i].IsArray)
					{
						//for sets, we should end the collection load after resolving
						//the entities, since we might call hashCode() on the elements
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad(resultSetId, session, collectionPersisters[i]);
					}
				}
			}
		}

		private static void EndCollectionLoad(object resultSetId, ISessionImplementor session,
		                                      ICollectionPersister collectionPersister)
		{
			//this is a query and we are loading multiple instances of the same collection role
			session.PersistenceContext.LoadContexts.GetCollectionLoadContext((IDataReader) resultSetId).EndLoadingCollections(
				collectionPersister);
		}

		protected virtual IList GetResultList(IList results, IResultTransformer resultTransformer)
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
		protected virtual object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                              ISessionImplementor session)
		{
			return row;
		}

		/// <summary>
		/// For missing objects associated by one-to-one with another object in the
		/// result set, register the fact that the the object is missing with the
		/// session.
		/// </summary>
		private void RegisterNonExists(EntityKey[] keys, ISessionImplementor session)
		{
			int[] owners = Owners;
			if (owners != null)
			{
				EntityType[] ownerAssociationTypes = OwnerAssociationTypes;
				for (int i = 0; i < keys.Length; i++)
				{
					int owner = owners[i];
					if (owner > -1)
					{
						EntityKey ownerKey = keys[owner];
						if (keys[i] == null && ownerKey != null)
						{
							bool isOneToOneAssociation = ownerAssociationTypes != null && ownerAssociationTypes[i] != null
							                             && ownerAssociationTypes[i].IsOneToOne;
							if (isOneToOneAssociation)
							{
								session.PersistenceContext.AddNullProperty(ownerKey, ownerAssociationTypes[i].PropertyName);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Read one collection element from the current row of the ADO.NET result set
		/// </summary>
		private static void ReadCollectionElement(object optionalOwner, object optionalKey, ICollectionPersister persister,
		                                          ICollectionAliases descriptor, IDataReader rs, ISessionImplementor session)
		{
			IPersistenceContext persistenceContext = session.PersistenceContext;

			object collectionRowKey = persister.ReadKey(rs, descriptor.SuffixedKeyAliases, session);

			if (collectionRowKey != null)
			{
				// we found a collection element in the result set

				if (log.IsDebugEnabled)
				{
					log.Debug("found row of collection: " + MessageHelper.InfoString(persister, collectionRowKey));
				}

				object owner = optionalOwner;
				if (owner == null)
				{
					owner = persistenceContext.GetCollectionOwner(collectionRowKey, persister);
					if (owner == null)
					{
						//TODO: This is assertion is disabled because there is a bug that means the
						//      original owner of a transient, uninitialized collection is not known 
						//      if the collection is re-referenced by a different object associated 
						//      with the current Session
						//throw new AssertionFailure("bug loading unowned collection");
					}
				}
				IPersistentCollection rowCollection =
					persistenceContext.LoadContexts.GetCollectionLoadContext(rs).GetLoadingCollection(persister, collectionRowKey);

				if (rowCollection != null)
				{
					rowCollection.ReadFrom(rs, persister, descriptor, owner);
				}
			}
			else if (optionalKey != null)
			{
				// we did not find a collection element in the result set, so we
				// ensure that a collection is created with the owner's identifier,
				// since what we have is an empty collection

				if (log.IsDebugEnabled)
				{
					log.Debug("result set contains (possibly empty) collection: " + MessageHelper.InfoString(persister, optionalKey));
				}
				persistenceContext.LoadContexts.GetCollectionLoadContext(rs).GetLoadingCollection(persister, optionalKey);
				// handle empty collection
			}

			// else no collection element, but also no owner
		}

		/// <summary>
		/// If this is a collection initializer, we need to tell the session that a collection
		/// is being initilized, to account for the possibility of the collection having
		/// no elements (hence no rows in the result set).
		/// </summary>
		internal void HandleEmptyCollections(object[] keys, object resultSetId, ISessionImplementor session)
		{
			if (keys != null)
			{
				// this is a collection initializer, so we must create a collection
				// for each of the passed-in keys, to account for the possibility
				// that the collection is empty and has no rows in the result set

				ICollectionPersister[] collectionPersisters = CollectionPersisters;
				for (int j = 0; j < collectionPersisters.Length; j++)
				{
					for (int i = 0; i < keys.Length; i++)
					{
						// handle empty collections
						if (log.IsDebugEnabled)
						{
							log.Debug("result set contains (possibly empty) collection: "
							          + MessageHelper.InfoString(collectionPersisters[j], keys[i]));
						}
						session.PersistenceContext.LoadContexts.GetCollectionLoadContext((IDataReader) resultSetId).GetLoadingCollection(
							collectionPersisters[j], keys[i]);
					}
				}
			}
			// else this is not a collection initializer (and empty collections will
			// be detected by looking for the owner's identifier in the result set)
		}

		/// <summary>
		/// Read a row of <c>EntityKey</c>s from the <c>IDataReader</c> into the given array.
		/// </summary>
		/// <remarks>
		/// Warning: this method is side-effecty. If an <c>id</c> is given, don't bother going
		/// to the <c>IDataReader</c>
		/// </remarks>
		private EntityKey GetKeyFromResultSet(int i, IEntityPersister persister, object id, IDataReader rs,
		                                      ISessionImplementor session)
		{
			object resultId;

			// if we know there is exactly 1 row, we can skip.
			// it would be great if we could _always_ skip this;
			// it is a problem for <key-many-to-one>

			if (IsSingleRowLoader && id != null)
			{
				resultId = id;
			}
			else
			{
				IType idType = persister.IdentifierType;
				resultId = idType.NullSafeGet(rs, EntityAliases[i].SuffixedKeyAliases, session, null);

				bool idIsResultId = id != null && resultId != null && idType.IsEqual(id, resultId, session.EntityMode, factory);

				if (idIsResultId)
				{
					resultId = id; //use the id passed in
				}
			}

			return resultId == null ? null : new EntityKey(resultId, persister, session.EntityMode);
		}

		/// <summary>
		/// Check the version of the object in the <c>IDataReader</c> against
		/// the object version in the session cache, throwing an exception
		/// if the version numbers are different.
		/// </summary>
		/// <exception cref="StaleObjectStateException"></exception>
		private void CheckVersion(int i, IEntityPersister persister, object id, object entity, IDataReader rs,
		                          ISessionImplementor session)
		{
			object version = session.PersistenceContext.GetEntry(entity).Version;

			// null version means the object is in the process of being loaded somewhere else in the ResultSet
			if (version != null)
			{
				IVersionType versionType = persister.VersionType;
				object currentVersion = versionType.NullSafeGet(rs, EntityAliases[i].SuffixedVersionAliases, session, null);
				if (!versionType.IsEqual(version, currentVersion))
				{
					if (session.Factory.Statistics.IsStatisticsEnabled)
					{
						session.Factory.StatisticsImplementor.OptimisticFailure(persister.EntityName);
					}

					throw new StaleObjectStateException(persister.EntityName, id);
				}
			}
		}

		/// <summary>
		/// Resolve any ids for currently loaded objects, duplications within the <c>IDataReader</c>,
		/// etc. Instanciate empty objects to be initialized from the <c>IDataReader</c>. Return an
		/// array of objects (a row of results) and an array of booleans (by side-effect) that determine
		/// wheter the corresponding object should be initialized
		/// </summary>
		private object[] GetRow(IDataReader rs, ILoadable[] persisters, EntityKey[] keys, object optionalObject,
		                        EntityKey optionalObjectKey, LockMode[] lockModes, IList hydratedObjects,
		                        ISessionImplementor session)
		{
			int cols = persisters.Length;
			IEntityAliases[] descriptors = EntityAliases;

			if (log.IsDebugEnabled)
			{
				log.Debug("result row: " + StringHelper.ToString(keys));
			}

			object[] rowResults = new object[cols];

			for (int i = 0; i < cols; i++)
			{
				object obj = null;
				EntityKey key = keys[i];

				if (keys[i] == null)
				{
					// do nothing
					/* TODO NH-1001 : if (persisters[i]...EntityType) is an OneToMany or a ManyToOne and
					 * the keys.length > 1 and the relation IsIgnoreNotFound probably we are in presence of
					 * an load with "outer join" the relation can be considerer loaded even if the key is null (mean not found)
					*/
				}
				else
				{
					//If the object is already loaded, return the loaded one
					obj = session.GetEntityUsingInterceptor(key);
					if (obj != null)
					{
						//its already loaded so dont need to hydrate it
						InstanceAlreadyLoaded(rs, i, persisters[i], key, obj, lockModes[i], session);
					}
					else
					{
						obj =
							InstanceNotYetLoaded(rs, i, persisters[i], key, lockModes[i], descriptors[i].RowIdAlias, optionalObjectKey,
							                     optionalObject, hydratedObjects, session);
					}
				}

				rowResults[i] = obj;
			}
			return rowResults;
		}

		/// <summary>
		/// The entity instance is already in the session cache
		/// </summary>
		private void InstanceAlreadyLoaded(IDataReader rs, int i, IEntityPersister persister, EntityKey key, object obj,
		                                   LockMode lockMode, ISessionImplementor session)
		{
			if (!persister.IsInstance(obj, session.EntityMode))
			{
				string errorMsg = string.Format("loading object was of wrong class [{0}]", obj.GetType().FullName);
				throw new WrongClassException(errorMsg, key.Identifier, persister.EntityName);
			}

			if (LockMode.None != lockMode && UpgradeLocks())
			{
				EntityEntry entry = session.PersistenceContext.GetEntry(obj);
				bool isVersionCheckNeeded = persister.IsVersioned && entry.LockMode.LessThan(lockMode);

				// we don't need to worry about existing version being uninitialized
				// because this block isn't called by a re-entrant load (re-entrant
				// load _always_ have lock mode NONE
				if (isVersionCheckNeeded)
				{
					// we only check the version when _upgrading_ lock modes
					CheckVersion(i, persister, key.Identifier, obj, rs, session);
					// we need to upgrade the lock mode to the mode requested
					entry.LockMode = lockMode;
				}
			}
		}

		/// <summary>
		/// The entity instance is not in the session cache
		/// </summary>
		private object InstanceNotYetLoaded(IDataReader dr, int i, ILoadable persister, EntityKey key, LockMode lockMode,
		                                    string rowIdAlias, EntityKey optionalObjectKey, object optionalObject,
		                                    IList hydratedObjects, ISessionImplementor session)
		{
			object obj;

			string instanceClass = GetInstanceClass(dr, i, persister, key.Identifier, session);

			if (optionalObjectKey != null && key.Equals(optionalObjectKey))
			{
				// its the given optional object
				obj = optionalObject;
			}
			else
			{
				obj = session.Instantiate(instanceClass, key.Identifier);
			}

			// need to hydrate it

			// grab its state from the DataReader and keep it in the Session
			// (but don't yet initialize the object itself)
			// note that we acquired LockMode.READ even if it was not requested
			LockMode acquiredLockMode = lockMode == LockMode.None ? LockMode.Read : lockMode;
			LoadFromResultSet(dr, i, obj, instanceClass, key, rowIdAlias, acquiredLockMode, persister, session);

			// materialize associations (and initialize the object) later
			hydratedObjects.Add(obj);

			return obj;
		}

		private bool IsEagerPropertyFetchEnabled(int i)
		{
			bool[] array = EntityEagerPropertyFetches;
			return array != null && array[i];
		}

		/// <summary>
		/// Hydrate the state of an object from the SQL <c>IDataReader</c>, into
		/// an array of "hydrated" values (do not resolve associations yet),
		/// and pass the hydrated state to the session.
		/// </summary>
		private void LoadFromResultSet(IDataReader rs, int i, object obj, string instanceClass, EntityKey key,
		                               string rowIdAlias, LockMode lockMode, ILoadable rootPersister,
		                               ISessionImplementor session)
		{
			object id = key.Identifier;

			// Get the persister for the _subclass_
			ILoadable persister = (ILoadable) Factory.GetEntityPersister(instanceClass);

			if (log.IsDebugEnabled)
			{
				log.Debug("Initializing object from DataReader: " + MessageHelper.InfoString(persister, id));
			}

			bool eagerPropertyFetch = IsEagerPropertyFetchEnabled(i);

			// add temp entry so that the next step is circular-reference
			// safe - only needed because some types don't take proper
			// advantage of two-phase-load (esp. components)
			TwoPhaseLoad.AddUninitializedEntity(key, obj, persister, lockMode, !eagerPropertyFetch, session);

			// This is not very nice (and quite slow):
			string[][] cols = persister == rootPersister
			                  	? EntityAliases[i].SuffixedPropertyAliases
			                  	: EntityAliases[i].GetSuffixedPropertyAliases(persister);

			object[] values = persister.Hydrate(rs, id, obj, rootPersister, cols, eagerPropertyFetch, session);

			object rowId = persister.HasRowId ? rs[rowIdAlias] : null;

			IAssociationType[] ownerAssociationTypes = OwnerAssociationTypes;
			if (ownerAssociationTypes != null && ownerAssociationTypes[i] != null)
			{
				string ukName = ownerAssociationTypes[i].RHSUniqueKeyPropertyName;
				if (ukName != null)
				{
					int index = ((IUniqueKeyLoadable) persister).GetPropertyIndex(ukName);
					IType type = persister.PropertyTypes[index];

					// polymorphism not really handled completely correctly,
					// perhaps...well, actually its ok, assuming that the
					// entity name used in the lookup is the same as the
					// the one used here, which it will be

					EntityUniqueKey euk =
						new EntityUniqueKey(rootPersister.EntityName, ukName, type.SemiResolve(values[index], session, obj), type,
						                    session.EntityMode, session.Factory);
					session.PersistenceContext.AddEntity(euk, obj);
				}
			}

			TwoPhaseLoad.PostHydrate(persister, id, values, rowId, obj, lockMode, !eagerPropertyFetch, session);
		}

		/// <summary>
		/// Determine the concrete class of an instance for the <c>IDataReader</c>
		/// </summary>
		private string GetInstanceClass(IDataReader rs, int i, ILoadable persister, object id, ISessionImplementor session)
		{
			if (persister.HasSubclasses)
			{
				// code to handle subclasses of topClass
				object discriminatorValue =
					persister.DiscriminatorType.NullSafeGet(rs, EntityAliases[i].SuffixedDiscriminatorAlias, session, null);

				string result = persister.GetSubclassForDiscriminatorValue(discriminatorValue);

				if (result == null)
				{
					// woops we got an instance of another class hierarchy branch.
					throw new WrongClassException(string.Format("Discriminator was: '{0}'", discriminatorValue), id,
					                              persister.EntityName);
				}

				return result;
			}
			else
			{
				return persister.EntityName;
			}
		}

		/// <summary>
		/// Advance the cursor to the first required row of the <c>IDataReader</c>
		/// </summary>
		internal static void Advance(IDataReader rs, RowSelection selection)
		{
			int firstRow = GetFirstRow(selection);

			if (firstRow != 0)
			{
				// DataReaders are forward-only, readonly, so we have to step through
				for (int i = 0; i < firstRow; i++)
				{
					rs.Read();
				}
			}
		}

		internal static bool HasMaxRows(RowSelection selection)
		{
			// it used to be selection.MaxRows != null -> since an Int32 will always
			// have a value I'll compare it to the static field NoValue used to initialize 
			// max rows to nothing
			return selection != null && selection.MaxRows != RowSelection.NoValue;
		}

		internal static int GetFirstRow(RowSelection selection)
		{
			if (selection == null || !selection.DefinesLimits)
			{
				return 0;
			}
			else
			{
				return selection.FirstRow > 0 ? selection.FirstRow : 0;
			}
		}

		/// <summary>
		/// Should we pre-process the SQL string, adding a dialect-specific
		/// LIMIT clause.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		internal static bool UseLimit(RowSelection selection, Dialect.Dialect dialect)
		{
			return dialect.SupportsLimit && HasMaxRows(selection);
		}

		/// <summary>
		/// Obtain an <c>IDbCommand</c> with all parameters pre-bound. Bind positional parameters,
		/// named parameters, and limit parameters.
		/// </summary>
		/// <remarks>
		/// Creates an IDbCommand object and populates it with the values necessary to execute it against the 
		/// database to Load an Entity.
		/// </remarks>
		/// <param name="queryParameters">The <see cref="QueryParameters"/> to use for the IDbCommand.</param>
		/// <param name="scroll">TODO: find out where this is used...</param>
		/// <param name="session">The SessionImpl this Command is being prepared in.</param>
		/// <returns>A CommandWrapper wrapping an IDbCommand that is ready to be executed.</returns>
		protected internal virtual IDbCommand PrepareQueryCommand(QueryParameters queryParameters, bool scroll,
		                                                          ISessionImplementor session)
		{
			SqlString sqlString = ProcessFilters(queryParameters, session);
			Dialect.Dialect dialect = session.Factory.Dialect;

			RowSelection selection = queryParameters.RowSelection;
			bool useLimit = UseLimit(selection, dialect);
			bool hasFirstRow = GetFirstRow(selection) > 0;
			bool useOffset = hasFirstRow && useLimit && dialect.SupportsLimitOffset;
			int startIndex = GetFirstLimitParameterCount(dialect, useLimit, hasFirstRow, useOffset);
			// TODO NH bool callable = queryParameters.Callable;

			SqlType[] parameterTypes = queryParameters.PrepareParameterTypes(sqlString, Factory, GetNamedParameterLocs, startIndex, useLimit, useOffset);

			if (useLimit)
			{
				sqlString =
					dialect.GetLimitString(
						sqlString.Trim(),
						useOffset ? GetFirstRow(selection) : 0,
						GetMaxOrLimit(dialect, selection),
						queryParameters.OffsetParameterIndex,
						queryParameters.LimitParameterIndex);
			}

			sqlString = PreprocessSQL(sqlString, queryParameters, dialect);

			// TODO NH: Callable for SP -> PrepareCallableQueryCommand
			IDbCommand command =
				session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, parameterTypes);

			try
			{
				// Added in NH - not in H2.1
				if (selection != null && selection.Timeout != RowSelection.NoValue)
				{
					command.CommandTimeout = selection.Timeout;
				}

				int colIndex = 0;

				if (useLimit && dialect.BindLimitParametersFirst)
				{
					colIndex += BindLimitParameters(command, colIndex, selection, session);
				}
				// TODO NH
				//if (callable)
				//{
				//  colIndex = dialect.RegisterResultSetOutParameter(command, col);
				//}

				colIndex += BindParameterValues(command, queryParameters, colIndex, session);

				if (useLimit && !dialect.BindLimitParametersFirst)
				{
					BindLimitParameters(command, colIndex, selection, session);
				}

				session.Batcher.ExpandQueryParameters(command, sqlString);

				if (!useLimit)
				{
					SetMaxRows(command, selection);
				}
				if (selection != null)
				{
					if (selection.Timeout != RowSelection.NoValue)
					{
						command.CommandTimeout = selection.Timeout;
					}
					// H2.1 handles FetchSize here - not ported
				}
			}
			catch (HibernateException)
			{
				session.Batcher.CloseCommand(command, null);
				throw;
			}
			catch (Exception sqle)
			{
				session.Batcher.CloseCommand(command, null);
				ADOExceptionReporter.LogExceptions(sqle);
				throw;
			}

			return command;
		}

		protected virtual SqlString ProcessFilters(QueryParameters parameters, ISessionImplementor session)
		{
			parameters.ProcessFilters(SqlString, session);
			return parameters.FilteredSQL;
		}

		/// <summary> 
		/// Some dialect-specific LIMIT clauses require the maximium last row number
		/// (aka, first_row_number + total_row_count), while others require the maximum
		/// returned row count (the total maximum number of rows to return). 
		/// </summary>
		/// <param name="selection">The selection criteria </param>
		/// <param name="dialect">The dialect </param>
		/// <returns> The appropriate value to bind into the limit clause. </returns>
		internal static int GetMaxOrLimit(Dialect.Dialect dialect, RowSelection selection)
		{
			int firstRow = GetFirstRow(selection);
			int lastRow = selection.MaxRows;

			if (dialect.UseMaxForLimit)
			{
				return lastRow + firstRow;
			}
			else
			{
				return lastRow;
			}
		}

		private int GetFirstLimitParameterCount(Dialect.Dialect dialect, bool useLimit, bool hasFirstRow, bool useOffset)
		{
			if (!useLimit) return 0;
			if (!dialect.BindLimitParametersFirst) return 0;
			return (hasFirstRow && useOffset) ? 2 : 1;
		}

		/// <summary>
		/// Bind parameters needed by the dialect-specific LIMIT clause
		/// </summary>
		/// <returns>The number of parameters bound</returns>
		internal static int BindLimitParameters(IDbCommand st, int index, RowSelection selection, ISessionImplementor session)
		{
			Dialect.Dialect dialect = session.Factory.Dialect;
			if (!dialect.SupportsVariableLimit)
			{
				return 0;
			}
			if (!HasMaxRows(selection))
			{
				throw new AssertionFailure("max results not set");
			}
			int firstRow = GetFirstRow(selection);
			int lastRow = GetMaxOrLimit(dialect, selection);

			bool hasFirstRow = firstRow > 0 && dialect.SupportsLimitOffset;
			bool reverse = dialect.BindLimitParametersInReverseOrder;

			if (hasFirstRow)
			{
				((IDataParameter) st.Parameters[index + (reverse ? 1 : 0)]).Value = firstRow;
			}
			((IDataParameter) st.Parameters[index + ((reverse || !hasFirstRow) ? 0 : 1)]).Value = lastRow;

			return hasFirstRow ? 2 : 1;
		}

		/// <summary>
		/// Limits the number of rows returned by the Sql query if necessary.
		/// </summary>
		/// <param name="st">The IDbCommand to limit.</param>
		/// <param name="selection">The RowSelection that contains the MaxResults info.</param>
		/// <remarks>TODO: This does not apply to ADO.NET at all</remarks>
		protected void SetMaxRows(IDbCommand st, RowSelection selection)
		{
			//TODO: H2.0.3 - do we need this method??
			//if (HasMaxRows(selection))
			//{
			//  // there is nothing in ADO.NET to do anything  similar
			//  // to Java's PreparedStatement.setMaxRows(int)
			//}
		}

		/// <summary> 
		/// Bind all parameter values into the prepared statement in preparation for execution. 
		/// </summary>
		/// <param name="statement">The ADO prepared statement </param>
		/// <param name="queryParameters">The encapsulation of the parameter values to be bound. </param>
		/// <param name="startIndex">The position from which to start binding parameter values. </param>
		/// <param name="session">The originating session. </param>
		/// <returns> The number of ADO bind positions actually bound during this method execution. </returns>
		protected virtual int BindParameterValues(IDbCommand statement, QueryParameters queryParameters,
		                                                   int startIndex, ISessionImplementor session)
		{
			// NH Different behavior:
			// The responsibility of parameter binding was entirely moved to QueryParameters
			// to deal with positionslParameter+NamedParameter+ParameterOfFilters
			return queryParameters.BindParameters(statement, startIndex, session);
		}

		public virtual int[] GetNamedParameterLocs(string name)
		{
			throw new AssertionFailure("no named parameters");
		}

		/// <summary>
		/// Fetch a <c>IDbCommand</c>, call <c>SetMaxRows</c> and then execute it,
		/// advance to the first result and return an SQL <c>IDataReader</c>
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand" /> to execute.</param>
		/// <param name="selection">The <see cref="RowSelection"/> to apply to the <see cref="IDbCommand"/> and <see cref="IDataReader"/>.</param>
		/// <param name="autoDiscoverTypes">true if result types need to be auto-discovered by the loader; false otherwise.</param>
		/// <param name="session">The <see cref="ISession" /> to load in.</param>
		/// <param name="callable"></param>
		/// <returns>An IDataReader advanced to the first record in RowSelection.</returns>
		protected IDataReader GetResultSet(IDbCommand st, bool autoDiscoverTypes, bool callable, RowSelection selection,
		                                   ISessionImplementor session)
		{
			IDataReader rs = null;
			try
			{
				log.Info(st.CommandText);
				// TODO NH: Callable
				rs = session.Batcher.ExecuteReader(st);

				//NH: this is checked outside the WrapResultSet because we
				// want to avoid the syncronization overhead in the vast majority
				// of cases where IsWrapResultSetsEnabled is set to false
				if (session.Factory.Settings.IsWrapResultSetsEnabled)
					rs = WrapResultSet(rs);

				Dialect.Dialect dialect = session.Factory.Dialect;
				if (!dialect.SupportsLimitOffset || !UseLimit(selection, dialect))
				{
					Advance(rs, selection);
				}

				if (autoDiscoverTypes)
				{
					AutoDiscoverTypes(rs);
				}
				return rs;
			}
			catch (Exception sqle)
			{
				ADOExceptionReporter.LogExceptions(sqle);
				session.Batcher.CloseCommand(st, rs);
				throw;
			}
		}

		protected virtual void AutoDiscoverTypes(IDataReader rs)
		{
			throw new AssertionFailure("Auto discover types not supported in this loader");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private IDataReader WrapResultSet(IDataReader rs)
		{
			// synchronized to avoid multi-thread access issues; defined as method synch to avoid
			// potential deadlock issues due to nature of code.
			try
			{
				log.Debug("Wrapping result set [" + rs + "]");
				return new ResultSetWrapper(rs, RetreiveColumnNameToIndexCache(rs));
			}
			catch (Exception e)
			{
				log.Info("Error wrapping result set", e);
				return rs;
			}
		}

		private ColumnNameCache RetreiveColumnNameToIndexCache(IDataReader rs)
		{
			if (columnNameCache == null)
			{
				log.Debug("Building columnName->columnIndex cache");
				columnNameCache = new ColumnNameCache(rs.GetSchemaTable().Rows.Count);
			}

			return columnNameCache;
		}

		/// <summary>
		/// Called by subclasses that load entities
		/// </summary>
		protected IList LoadEntity(ISessionImplementor session, object id, IType identifierType, object optionalObject,
		                           string optionalEntityName, object optionalIdentifier, IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("loading entity: " + MessageHelper.InfoString(persister, id, identifierType, Factory));
			}

			IList result;

			try
			{
				QueryParameters qp =
					new QueryParameters(new IType[] {identifierType}, new object[] {id}, optionalObject, optionalEntityName,
					                    optionalIdentifier);
				result = DoQueryAndInitializeNonLazyCollections(session, qp, false);
			}
			catch (HibernateException)
			{
				throw;
			}
			catch (Exception sqle)
			{
				ILoadable[] persisters = EntityPersisters;
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
				                                 "could not load an entity: "
				                                 +
				                                 MessageHelper.InfoString(persisters[persisters.Length - 1], id, identifierType,
				                                                          Factory), SqlString);
			}

			log.Debug("done entity load");

			return result;
		}

		protected IList LoadEntity(ISessionImplementor session, object key, object index, IType keyType, IType indexType,
		                           IEntityPersister persister)
		{
			log.Debug("loading collection element by index");

			IList result;
			try
			{
				result =
					DoQueryAndInitializeNonLazyCollections(session,
					                                       new QueryParameters(new IType[] {keyType, indexType},
					                                                           new object[] {key, index}), false);
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(factory.SQLExceptionConverter, sqle, "could not collection element by index",
				                                 SqlString);
			}

			log.Debug("done entity load");

			return result;
		}

		/// <summary>
		/// Called by subclasses that batch load entities
		/// </summary>
		protected internal IList LoadEntityBatch(ISessionImplementor session, object[] ids, IType idType,
		                                         object optionalObject, string optionalEntityName, object optionalId,
		                                         IEntityPersister persister)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("batch loading entity: " + MessageHelper.InfoString(persister, ids, Factory));
			}

			IType[] types = new IType[ids.Length];
			ArrayHelper.Fill(types, idType);
			IList result;
			try
			{
				result =
					DoQueryAndInitializeNonLazyCollections(session,
					                                       new QueryParameters(types, ids, optionalObject, optionalEntityName,
					                                                           optionalId), false);
			}
			catch (HibernateException)
			{
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
				                                 "could not load an entity batch: "
				                                 + MessageHelper.InfoString(persister, ids, Factory), SqlString);
				// NH: Hibernate3 passes EntityPersisters[0] instead of persister, I think it's wrong.
			}

			log.Debug("done entity batch load");
			return result;
		}

		/// <summary>
		/// Called by subclasses that load collections
		/// </summary>
		public void LoadCollection(ISessionImplementor session, object id, IType type)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("loading collection: " + MessageHelper.InfoString(CollectionPersisters[0], id));
			}

			object[] ids = new object[] {id};
			try
			{
				DoQueryAndInitializeNonLazyCollections(session, new QueryParameters(new IType[] {type}, ids, ids), true);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
				                                 "could not initialize a collection: "
				                                 + MessageHelper.InfoString(CollectionPersisters[0], id), SqlString);
			}

			log.Debug("done loading collection");
		}

		/// <summary>
		/// Called by wrappers that batch initialize collections
		/// </summary>
		public void LoadCollectionBatch(ISessionImplementor session, object[] ids, IType type)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("batch loading collection: " + MessageHelper.InfoString(CollectionPersisters[0], ids));
			}

			IType[] idTypes = new IType[ids.Length];
			ArrayHelper.Fill(idTypes, type);
			try
			{
				DoQueryAndInitializeNonLazyCollections(session, new QueryParameters(idTypes, ids, ids), true);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
				                                 "could not initialize a collection batch: "
				                                 + MessageHelper.InfoString(CollectionPersisters[0], ids), SqlString);
			}

			log.Debug("done batch load");
		}

		/// <summary>
		/// Called by subclasses that batch initialize collections
		/// </summary>
		protected void LoadCollectionSubselect(ISessionImplementor session, object[] ids, object[] parameterValues,
		                                       IType[] parameterTypes, IDictionary<string, TypedValue> namedParameters,
		                                       IType type)
		{
			try
			{
				DoQueryAndInitializeNonLazyCollections(session,
				                                       new QueryParameters(parameterTypes, parameterValues, namedParameters, ids),
				                                       true);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle,
				                                 "could not load collection by subselect: "
				                                 + MessageHelper.InfoString(CollectionPersisters[0], ids), SqlString,
				                                 parameterValues, namedParameters);
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
		protected IList List(ISessionImplementor session, QueryParameters queryParameters, ISet<string> querySpaces,
		                     IType[] resultTypes)
		{
			bool cacheable = factory.Settings.IsQueryCacheEnabled && queryParameters.Cacheable;

			if (cacheable)
			{
				return ListUsingQueryCache(session, queryParameters, querySpaces, resultTypes);
			}
			else
			{
				return ListIgnoreQueryCache(session, queryParameters);
			}
		}

		private IList ListIgnoreQueryCache(ISessionImplementor session, QueryParameters queryParameters)
		{
			return GetResultList(DoList(session, queryParameters), queryParameters.ResultTransformer);
		}

		private IList ListUsingQueryCache(ISessionImplementor session, QueryParameters queryParameters,
		                                  ISet<string> querySpaces, IType[] resultTypes)
		{
			IQueryCache queryCache = factory.GetQueryCache(queryParameters.CacheRegion);

			ISet filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters, session.EntityMode);
			QueryKey key = new QueryKey(Factory, SqlString, queryParameters, filterKeys);

			IList result = GetResultFromQueryCache(session, queryParameters, querySpaces, resultTypes, queryCache, key);

			if (result == null)
			{
				result = DoList(session, queryParameters);
				PutResultInQueryCache(session, queryParameters, resultTypes, queryCache, key, result);
			}

			return GetResultList(result, queryParameters.ResultTransformer);
		}

		private IList GetResultFromQueryCache(ISessionImplementor session, QueryParameters queryParameters,
		                                      ISet<string> querySpaces, IType[] resultTypes, IQueryCache queryCache,
		                                      QueryKey key)
		{
			IList result = null;
			if ((!queryParameters.ForceCacheRefresh) && (session.CacheMode & CacheMode.Get) == CacheMode.Get)
			{
				result = queryCache.Get(key, resultTypes, queryParameters.NaturalKeyLookup, querySpaces, session);
				if (factory.Statistics.IsStatisticsEnabled)
				{
					if (result == null)
					{
						factory.StatisticsImplementor.QueryCacheMiss(QueryIdentifier, queryCache.RegionName);
					}
					else
					{
						factory.StatisticsImplementor.QueryCacheHit(QueryIdentifier, queryCache.RegionName);
					}
				}
			}
			return result;
		}

		private void PutResultInQueryCache(ISessionImplementor session, QueryParameters queryParameters, IType[] resultTypes,
		                                   IQueryCache queryCache, QueryKey key, IList result)
		{
			if ((session.CacheMode & CacheMode.Put) == CacheMode.Put)
			{
				bool put = queryCache.Put(key, resultTypes, result, queryParameters.NaturalKeyLookup, session);
				if (put && factory.Statistics.IsStatisticsEnabled)
				{
					factory.StatisticsImplementor.QueryCachePut(QueryIdentifier, queryCache.RegionName);
				}
			}
		}

		/// <summary>
		/// Actually execute a query, ignoring the query cache
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <returns></returns>
		protected IList DoList(ISessionImplementor session, QueryParameters queryParameters)
		{
			bool statsEnabled = Factory.Statistics.IsStatisticsEnabled;
			var stopWatch = new Stopwatch();
			if (statsEnabled)
			{
				stopWatch.Start();
			}

			IList result;
			try
			{
				result = DoQueryAndInitializeNonLazyCollections(session, queryParameters, true);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(Factory.SQLExceptionConverter, sqle, "could not execute query", SqlString,
				                                 queryParameters.PositionalParameterValues, queryParameters.NamedParameters);
			}
			if (statsEnabled)
			{
				stopWatch.Stop();
				Factory.StatisticsImplementor.QueryExecuted(QueryIdentifier, result.Count, stopWatch.Elapsed);
			}
			return result;
		}

		/// <summary>
		/// Calculate and cache select-clause suffixes. Must be
		/// called by subclasses after instantiation.
		/// </summary>
		protected virtual void PostInstantiate() {}

		/// <summary> 
		/// Identifies the query for statistics reporting, if null,
		/// no statistics will be reported
		/// </summary>
		public virtual string QueryIdentifier
		{
			get { return null; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + SqlString + ')';
		}

		#region NHibernate specific

		public virtual SqlCommandInfo GetQueryStringAndTypes(ISessionImplementor session, QueryParameters parameters, int startParameterIndex)
		{
			SqlString sqlString = ProcessFilters(parameters, session);
			Dialect.Dialect dialect = session.Factory.Dialect;

			RowSelection selection = parameters.RowSelection;
			bool useLimit = UseLimit(selection, dialect);
			bool hasFirstRow = GetFirstRow(selection) > 0;
			bool useOffset = hasFirstRow && useLimit && dialect.SupportsLimitOffset;
			int limitParameterCount = GetFirstLimitParameterCount(dialect, useLimit, hasFirstRow, useOffset);

			SqlType[] sqlTypes = parameters.PrepareParameterTypes(sqlString, Factory, GetNamedParameterLocs, startParameterIndex + limitParameterCount, useLimit, useOffset);

			if (useLimit)
			{
				sqlString =
					dialect.GetLimitString(
						sqlString.Trim(),
						useOffset ? GetFirstRow(selection) : 0,
						GetMaxOrLimit(dialect, selection),
						parameters.OffsetParameterIndex,
						parameters.LimitParameterIndex);
			}

			sqlString = PreprocessSQL(sqlString, parameters, dialect);
			return new SqlCommandInfo(sqlString, sqlTypes);
		}

		#endregion
	}
}
