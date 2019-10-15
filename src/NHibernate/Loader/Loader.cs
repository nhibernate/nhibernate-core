using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.Intercept;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Abstract superclass of object loading (and querying) strategies.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class implements useful common functionality that concrete loaders would delegate to.
	/// It is not intended that this functionality would be directly accessed by client code (Hence,
	/// all methods of this class are declared <c>protected</c> or <c>private</c>.) This class relies heavily upon the
	/// <see cref="ILoadable" /> interface, which is the contract between this class and
	/// <see cref="IEntityPersister" />s that may be loaded by it.
	/// </para>
	/// <para>
	/// The present implementation is able to load any number of columns of entities and at most
	/// one collection role per query.
	/// </para>
	/// <para>
	/// All this class members are thread safe. Entity and collection loaders are held in persisters shared among
	/// sessions built from the same session factory. They must be thread safe.
	/// </para>
	/// </remarks>
	/// <seealso cref="NHibernate.Persister.Entity.ILoadable"/>
	public abstract partial class Loader
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(Loader));

		private readonly ISessionFactoryImplementor _factory;
		private readonly SessionFactoryHelper _helper;
		private ColumnNameCache _columnNameCache;

		/// <summary>
		/// Indicates whether the dialect is able to add limit and/or offset clauses to <see cref="SqlString"/>.
		/// Even if a dialect generally supports the addition of limit and/or offset clauses to SQL statements,
		/// there may (custom) SQL statements where this is not possible, for example in case of SQL Server 
		/// stored procedure invocations.
		/// </summary>
		private bool? _canUseLimits;

		/// <summary>
		/// Caches subclass entity aliases for given persister index in <see cref="EntityPersisters"/>  and subclass entity name
		/// </summary>
		private readonly ConcurrentDictionary<Tuple<int, string>, string[][]> _subclassEntityAliasesMap = new ConcurrentDictionary<Tuple<int, string>, string[][]>();

		protected Loader(ISessionFactoryImplementor factory)
		{
			_factory = factory;
			_helper = new SessionFactoryHelper(factory);
		}

		protected SessionFactoryHelper Helper
		{
			get { return _helper; }
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
		/// An array of hash sets indicating which lazy properties will be fetched for an entity persister.
		/// </summary>
		protected virtual HashSet<string>[] EntityFetchLazyProperties
		{
			get { return null; }
		}

		/// <summary>
		/// An array of indexes of the entity that owns an association
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
		/// returns. Indices indicating no owner would be null here. 
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

		/// <summary>
		/// The result types of the result set, for query loaders.
		/// </summary>
		public IType[] ResultTypes { get; protected set; }

		public bool[] EntityFetches { get; protected set; }

		public bool[] CollectionFetches { get; protected set; }

		public virtual IType[] CacheTypes => ResultTypes;

		public ISessionFactoryImplementor Factory
		{
			get { return _factory; }
		}

		/// <summary>
		/// The SqlString to be called; implemented by all subclasses
		/// </summary>
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
			return DoQueryAndInitializeNonLazyCollections(session, queryParameters, returnProxies, null, null);
		}


		private IList DoQueryAndInitializeNonLazyCollections(ISessionImplementor session, QueryParameters queryParameters, bool returnProxies, 
		                                                     IResultTransformer forcedResultTransformer,
		                                                     QueryCacheResultBuilder queryCacheResultBuilder)
		{
			IPersistenceContext persistenceContext = session.PersistenceContext;
			bool defaultReadOnlyOrig = persistenceContext.DefaultReadOnly;

			if (queryParameters.IsReadOnlyInitialized)
				persistenceContext.DefaultReadOnly = queryParameters.ReadOnly;
			else
				queryParameters.ReadOnly = persistenceContext.DefaultReadOnly;

			persistenceContext.BeforeLoad();
			IList result;
			try
			{
				try
				{
					result = DoQuery(session, queryParameters, returnProxies, forcedResultTransformer, queryCacheResultBuilder);
				}
				finally
				{
					persistenceContext.AfterLoad();
				}
				persistenceContext.InitializeNonLazyCollections();
			}
			finally
			{
				persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
			}

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
		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		protected object LoadSingleRow(DbDataReader resultSet, ISessionImplementor session, QueryParameters queryParameters,
									   bool returnProxies)
		{
			int entitySpan = EntityPersisters.Length;
			IList hydratedObjects = entitySpan == 0 ? null : new List<object>(entitySpan);
			var cacheBatcher = new CacheBatcher(session);

			object result;
			try
			{
				result =
					GetRowFromResultSet(resultSet, session, queryParameters, GetLockModes(queryParameters.LockModes), null,
					                    hydratedObjects, new EntityKey[entitySpan], returnProxies, null, null,
					                    (persister, data) => cacheBatcher.AddToBatch(persister, data));
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

			InitializeEntitiesAndCollections(hydratedObjects, resultSet, session, queryParameters.IsReadOnly(session), cacheBatcher);
			cacheBatcher.ExecuteBatch();
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
				return session.GenerateEntityKey(optionalId, session.GetEntityPersister(optionalEntityName, optionalObject));
			}
			else
			{
				return null;
			}
		}

		internal object GetRowFromResultSet(DbDataReader resultSet, ISessionImplementor session,
											QueryParameters queryParameters, LockMode[] lockModeArray,
											EntityKey optionalObjectKey, IList hydratedObjects, EntityKey[] keys,
											bool returnProxies, IResultTransformer forcedResultTransformer,
											QueryCacheResultBuilder queryCacheResultBuilder,
		                                    Action<IEntityPersister, CachePutData> cacheBatchingHandler)
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
					   hydratedObjects, session, !returnProxies, cacheBatchingHandler);

			var collections = ReadCollectionElements(row, resultSet, session);

			if (returnProxies)
			{
				// now get an existing proxy for each row element (if there is one)
				for (int i = 0; i < entitySpan; i++)
				{
					object entity = row[i];
					var key = keys[i];
					if (entity == null && key != null && IsChildFetchEntity(i))
					{
						// The entity was missing in the session, fallback on internal load (which will just yield a
						// proxy if the persister supports it).
						row[i] = session.InternalLoad(key.EntityName, key.Identifier, false, false);
					}
					else
					{
						object proxy = session.PersistenceContext.ProxyFor(persisters[i], keys[i], entity);

						if (entity != proxy)
						{
							// Force the proxy to resolve itself
							((INHibernateProxy) proxy).HibernateLazyInitializer.SetImplementation(entity);
							row[i] = proxy;
						}
					}
				}
			}

			var result = forcedResultTransformer == null
					   ? GetResultColumnOrRow(row, queryParameters.ResultTransformer, resultSet, session)
					   : forcedResultTransformer.TransformTuple(GetResultRow(row, resultSet, session),
																ResultRowAliases);

			queryCacheResultBuilder?.AddRow(result, row, collections);

			return result;
		}

		/// <summary>
		/// Read any collection elements contained in a single row of the result set
		/// </summary>
		private IPersistentCollection[] ReadCollectionElements(object[] row, DbDataReader resultSet, ISessionImplementor session)
		{
			//TODO: make this handle multiple collection roles!

			ICollectionPersister[] collectionPersisters = CollectionPersisters;

			if (collectionPersisters != null)
			{
				var result = new IPersistentCollection[collectionPersisters.Length];
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

					result[i] = ReadCollectionElement(owner, key, collectionPersister, descriptors[i], resultSet, session);
				}

				return result;
			}

			return null;
		}

		private IList DoQuery(ISessionImplementor session, QueryParameters queryParameters, bool returnProxies, 
		                      IResultTransformer forcedResultTransformer, QueryCacheResultBuilder queryCacheResultBuilder)
		{
			using (session.BeginProcess())
			{
				RowSelection selection = queryParameters.RowSelection;
				int maxRows = HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;

				int entitySpan = EntityPersisters.Length;

				List<object> hydratedObjects = entitySpan == 0 ? null : new List<object>(entitySpan*10);

				var st = PrepareQueryCommand(queryParameters, false, session);

				var rs = GetResultSet(st, queryParameters, session, forcedResultTransformer);
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
				var cacheBatcher = new CacheBatcher(session);

				try
				{
					HandleEmptyCollections(queryParameters.CollectionKeys, rs, session);
					EntityKey[] keys = new EntityKey[entitySpan]; // we can reuse it each time

					if (Log.IsDebugEnabled())
					{
						Log.Debug("processing result set");
					}

					int count;
					for (count = 0; count < maxRows && rs.Read(); count++)
					{
						if (Log.IsDebugEnabled())
						{
							Log.Debug("result set row: {0}", count);
						}

						object result = GetRowFromResultSet(rs, session, queryParameters, lockModeArray, optionalObjectKey,
															hydratedObjects,
															keys, returnProxies, forcedResultTransformer, queryCacheResultBuilder,
						                                    (persister, data) => cacheBatcher.AddToBatch(persister, data));
						results.Add(result);

						if (createSubselects)
						{
							subselectResultKeys.Add(keys);
							keys = new EntityKey[entitySpan]; //can't reuse in this case
						}
					}

					if (Log.IsDebugEnabled())
					{
						Log.Debug("done processing result set ({0} rows)", count);
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

				InitializeEntitiesAndCollections(hydratedObjects, rs, session, queryParameters.IsReadOnly(session), cacheBatcher);
				cacheBatcher.ExecuteBatch();

				if (createSubselects)
				{
					CreateSubselects(subselectResultKeys, queryParameters, session);
				}

				return results;
			}
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

		private static ISet<EntityKey>[] Transpose(List<EntityKey[]> keys)
		{
			ISet<EntityKey>[] result = new ISet<EntityKey>[keys[0].Length];
			for (int j = 0; j < result.Length; j++)
			{
				result[j] = new HashSet<EntityKey>();
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

		internal void CreateSubselects(List<EntityKey[]> keys, QueryParameters queryParameters, ISessionImplementor session)
		{
			if (keys.Count > 1)
			{
				//if we only returned one entity, query by key is more efficient
				var subSelects = CreateSubselects(keys, queryParameters).ToArray();

				foreach (EntityKey[] rowKeys in keys)
				{
					for (int i = 0; i < rowKeys.Length; i++)
					{
						if (rowKeys[i] != null && subSelects[i] != null)
						{
							session.PersistenceContext.BatchFetchQueue.AddSubselect(rowKeys[i], subSelects[i]);
						}
					}
				}
			}
		}

		private IEnumerable<SubselectFetch> CreateSubselects(List<EntityKey[]> keys, QueryParameters queryParameters)
		{
			// see NH-2123 NH-2125
			ISet<EntityKey>[] keySets = Transpose(keys);
			ILoadable[] loadables = EntityPersisters;
			string[] aliases = Aliases;

			for (int i = 0; i < loadables.Length; i++)
			{
				if (loadables[i].HasSubselectLoadableCollections)
				{
					yield return new SubselectFetch(aliases[i], loadables[i], queryParameters, keySets[i]);
				}
				else
				{
					yield return null;
				}
			}
		}

		internal void InitializeEntitiesAndCollections(
			IList hydratedObjects, DbDataReader reader, ISessionImplementor session, bool readOnly,
			CacheBatcher cacheBatcher)
		{
			ICollectionPersister[] collectionPersisters = CollectionPersisters;
			var ownCacheBatcher = cacheBatcher == null;
			if (ownCacheBatcher)
				cacheBatcher = new CacheBatcher(session);

			if (collectionPersisters != null)
			{
				foreach (var collectionPersister in collectionPersisters)
				{
					if (collectionPersister.IsArray)
					{
						//for arrays, we should end the collection load before resolving
						//the entities, since the actual array instances are not instantiated
						//during loading
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad(reader, session, collectionPersister, cacheBatcher);
					}
				}
			}
			//important: reuse the same event instances for performance!
			PreLoadEvent pre;
			PostLoadEvent post;
			if (session.IsEventSource)
			{
				var eventSourceSession = (IEventSource)session;
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

				if (Log.IsDebugEnabled())
				{
					Log.Debug("total objects hydrated: {0}", hydratedObjectsSize);
				}

				for (int i = 0; i < hydratedObjectsSize; i++)
				{
					TwoPhaseLoad.InitializeEntity(
						hydratedObjects[i], readOnly, session, pre, post,
						(persister, data) => cacheBatcher.AddToBatch(persister, data));
				}
			}

			if (collectionPersisters != null)
			{
				foreach (var collectionPersister in collectionPersisters)
				{
					if (!collectionPersister.IsArray)
					{
						//for sets, we should end the collection load after resolving
						//the entities, since we might call hashCode() on the elements
						//TODO: or we could do this polymorphically, and have two
						//      different operations implemented differently for arrays
						EndCollectionLoad(reader, session, collectionPersister, cacheBatcher);
					}
				}
			}

			if (ownCacheBatcher)
				cacheBatcher.ExecuteBatch();
		}

		/// <summary>
		/// Stops further collection population without actual collection initialization.
		/// </summary>
		internal void StopLoadingCollections(ISessionImplementor session, DbDataReader reader)
		{
			var collectionPersisters = CollectionPersisters;
			if (collectionPersisters == null || collectionPersisters.Length == 0)
				return;

			session.PersistenceContext.LoadContexts.GetCollectionLoadContext(reader).StopLoadingCollections(collectionPersisters);
		}

		private void EndCollectionLoad(DbDataReader reader, ISessionImplementor session, ICollectionPersister collectionPersister,
		                               CacheBatcher cacheBatcher)
		{
			//this is a query and we are loading multiple instances of the same collection role
			session.PersistenceContext.LoadContexts.GetCollectionLoadContext(reader).EndLoadingCollections(
				collectionPersister, !IsCollectionPersisterCacheable(collectionPersister), cacheBatcher);
		}

		protected virtual bool IsCollectionPersisterCacheable(ICollectionPersister collectionPersister)
		{
			return true;
		}

		/// <summary>
		/// Determine the actual ResultTransformer that will be used to transform query results.
		/// </summary>
		/// <param name="resultTransformer">The specified result transformer.</param>
		/// <returns>The actual result transformer.</returns>
		protected virtual IResultTransformer ResolveResultTransformer(IResultTransformer resultTransformer)
		{
			return resultTransformer;
		}


		/// <summary>
		/// Are rows transformed immediately after being read from the ResultSet?
		/// </summary>
		/// <returns>True, if getResultColumnOrRow() transforms the results; false, otherwise</returns>
		protected virtual bool AreResultSetRowsTransformedImmediately()
		{
			return false;
		}


		public virtual IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			return results;
		}


		/// <summary>
		/// Returns the aliases that correspond to a result row.
		/// </summary>
		/// <returns>Returns the aliases that correspond to a result row.</returns>
		protected virtual string[] ResultRowAliases
		{
			get { return null; }
		}


		/// <summary>
		/// Get the actual object that is returned in the user-visible result list.
		/// </summary>
		/// <remarks>
		/// This empty implementation merely returns its first argument. This is
		/// overridden by some subclasses.
		/// </remarks>
		protected virtual object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, DbDataReader rs, ISessionImplementor session)
		{
			return row;
		}

		protected virtual bool[] IncludeInResultRow
		{
			get { return null; }
		}

		protected virtual object[] GetResultRow(Object[] row, DbDataReader rs, ISessionImplementor session)
		{
			return row;
		}

		/// <summary>
		/// For missing objects associated with another object in the
		/// result set, register the fact that the the object is missing with the
		/// session.
		/// </summary>
		private void RegisterNonExists(EntityKey[] keys, ISessionImplementor session)
		{
			var owners = Owners;
			var ownerAssociationTypes = OwnerAssociationTypes;
			if (owners != null && ownerAssociationTypes != null)
			{
				for (var i = 0; i < keys.Length; i++)
				{
					if (keys[i] == null)
					{
						var ownerAssociationType = ownerAssociationTypes[i];
						if (ownerAssociationType?.PropertyName != null && ownerAssociationType.IsNullable)
						{
							var owner = owners[i];
							if (owner > -1)
							{
								var ownerKey = keys[owner];
								if (ownerKey != null)
								{
									session.PersistenceContext.AddNullProperty(ownerKey, ownerAssociationType.PropertyName);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Read one collection element from the current row of the ADO.NET result set
		/// </summary>
		private static IPersistentCollection ReadCollectionElement(object optionalOwner, object optionalKey, ICollectionPersister persister,
												  ICollectionAliases descriptor, DbDataReader rs, ISessionImplementor session)
		{
			IPersistenceContext persistenceContext = session.PersistenceContext;

			object collectionRowKey = persister.ReadKey(rs, descriptor.SuffixedKeyAliases, session);

			if (collectionRowKey != null)
			{
				// we found a collection element in the result set

				if (Log.IsDebugEnabled())
				{
					Log.Debug("found row of collection: {0}", MessageHelper.CollectionInfoString(persister, collectionRowKey));
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

				return rowCollection;
			}
			else if (optionalKey != null)
			{
				// we did not find a collection element in the result set, so we
				// ensure that a collection is created with the owner's identifier,
				// since what we have is an empty collection

				if (Log.IsDebugEnabled())
				{
					Log.Debug("result set contains (possibly empty) collection: {0}", MessageHelper.CollectionInfoString(persister, optionalKey));
				}

				// handle empty collection
				return persistenceContext.LoadContexts.GetCollectionLoadContext(rs).GetLoadingCollection(persister, optionalKey);
			}

			// else no collection element, but also no owner
			return null;
		}

		/// <summary>
		/// If this is a collection initializer, we need to tell the session that a collection
		/// is being initialized, to account for the possibility of the collection having
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
						if (Log.IsDebugEnabled())
						{
							Log.Debug("result set contains (possibly empty) collection: {0}",
							          MessageHelper.CollectionInfoString(collectionPersisters[j], keys[i]));
						}
						session.PersistenceContext.LoadContexts.GetCollectionLoadContext((DbDataReader)resultSetId).GetLoadingCollection(
							collectionPersisters[j], keys[i]);
					}
				}
			}
			// else this is not a collection initializer (and empty collections will
			// be detected by looking for the owner's identifier in the result set)
		}

		/// <summary>
		/// Read a row of <c>EntityKey</c>s from the <c>DbDataReader</c> into the given array.
		/// </summary>
		/// <remarks>
		/// Warning: this method is side-effecty. If an <c>id</c> is given, don't bother going
		/// to the <c>DbDataReader</c>
		/// </remarks>
		private EntityKey GetKeyFromResultSet(int i, IEntityPersister persister, object id, DbDataReader rs, ISessionImplementor session)
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

				bool idIsResultId = id != null && resultId != null && idType.IsEqual(id, resultId, _factory);

				if (idIsResultId)
				{
					resultId = id; //use the id passed in
				}
			}

			return resultId == null ? null : session.GenerateEntityKey(resultId, persister);
		}

		/// <summary>
		/// Check the version of the object in the <c>DbDataReader</c> against
		/// the object version in the session cache, throwing an exception
		/// if the version numbers are different.
		/// </summary>
		/// <exception cref="StaleObjectStateException"></exception>
		private void CheckVersion(int i, IEntityPersister persister, object id, object entity, DbDataReader rs, ISessionImplementor session)
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
		/// Resolve any ids for currently loaded objects, duplications within the <c>DbDataReader</c>,
		/// etc. Instantiate empty objects to be initialized from the <c>DbDataReader</c>. Return an
		/// array of objects (a row of results) and an array of booleans (by side-effect) that determine
		/// whether the corresponding object should be initialized
		/// </summary>
		private object[] GetRow(DbDataReader rs, ILoadable[] persisters, EntityKey[] keys, object optionalObject,
								EntityKey optionalObjectKey, LockMode[] lockModes, IList hydratedObjects,
								ISessionImplementor session, bool mustLoadMissingEntity, Action<IEntityPersister, CachePutData> cacheBatchingHandler)
		{
			int cols = persisters.Length;

			if (Log.IsDebugEnabled())
			{
				Log.Debug("result row: {0}", StringHelper.ToString(keys));
			}

			object[] rowResults = new object[cols];

			for (int i = 0; i < cols; i++)
			{
				object obj = null;
				EntityKey key = keys[i];

				// null keys are handled in RegisterNonExists
				if(key != null)
				{
					//If the object is already loaded, return the loaded one
					obj = session.GetEntityUsingInterceptor(key);
					var alreadyLoaded = obj != null;
					var persister = persisters[i];
					if (IsChildFetchEntity(i))
					{
						if (!alreadyLoaded && mustLoadMissingEntity)
						{
							// Missing in session while its data has not been selected: fallback on immediate load
							obj = session.ImmediateLoad(key.EntityName, key.Identifier);
						}
						rowResults[i] = obj;
						continue;
					}

					if (alreadyLoaded)
					{
						//its already loaded so dont need to hydrate it
						InstanceAlreadyLoaded(rs, i, persister, key, obj, lockModes[i], session, cacheBatchingHandler);
					}
					else
					{
						obj =
							InstanceNotYetLoaded(rs, i, persister, key, lockModes[i], optionalObjectKey,
												 optionalObject, hydratedObjects, session);

						// IUniqueKeyLoadable.CacheByUniqueKeys caches all unique keys of the entity, regardless of
						// associations loaded by the query. So if the entity is already loaded, it has forcibly already
						// been cached too for all its unique keys, provided its persister implement it. With this new
						// way of caching unique keys, it is no more needed to handle caching for alreadyLoaded path
						// too.
						(persister as IUniqueKeyLoadable)?.CacheByUniqueKeys(obj, session);
					}
					// 6.0 TODO: this call is nor more needed for up-to-date persisters, remove once CacheByUniqueKeys
					// is merged in IUniqueKeyLoadable interface instead of being an extension method
					// #1226 old fix: Even if it is already loaded, if it can be loaded from an association with a property ref,
					// make sure it is also cached by its unique key.
					CacheByUniqueKey(i, persister, obj, session, alreadyLoaded);
				}

				rowResults[i] = obj;
			}
			return rowResults;
		}

		/// <summary>
		/// The entity instance is already in the session cache
		/// </summary>
		private void InstanceAlreadyLoaded(DbDataReader rs, int i, ILoadable persister, EntityKey key, object obj,
										   LockMode lockMode, ISessionImplementor session, Action<IEntityPersister, CachePutData> cacheBatchingHandler)
		{
			if (!persister.IsInstance(obj))
			{
				string errorMsg = string.Format("loading object was of wrong class [{0}]", obj.GetType().FullName);
				throw new WrongClassException(errorMsg, key.Identifier, persister.EntityName);
			}

			EntityEntry entry = null;
			if (LockMode.None != lockMode && UpgradeLocks())
			{
				entry = session.PersistenceContext.GetEntry(obj);
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

			if (!persister.HasLazyProperties)
			{
				return;
			}

			UpdateLazyPropertiesFromResultSet(rs, i, obj, key, entry, persister, session, cacheBatchingHandler);
		}

		private void CacheByUniqueKey(int i, IEntityPersister persister, object obj, ISessionImplementor session, bool alreadyLoaded)
		{
			var ownerAssociationTypes = OwnerAssociationTypes;
			if (ownerAssociationTypes == null)
				return;
			var ukName = ownerAssociationTypes[i]?.RHSUniqueKeyPropertyName;
			if (ukName == null)
				return;
			var index = ((IUniqueKeyLoadable)persister).GetPropertyIndex(ukName);
			var ukValue = alreadyLoaded
				? persister.GetPropertyValue(obj, index)
				: session.PersistenceContext.GetEntry(obj).LoadedState[index];
			// ukValue can be null for two reasons:
			//  - Entity thought to be already loaded but indeed currently loading and not yet fully hydrated.
			//    In such case, it has already been handled by InstanceNotYetLoaded path on a previous row,
			//    there is nothing more to do. This case could also be detected with
			//    "session.PersistenceContext.GetEntry(obj).Status == Status.Loading", but since there
			//    is a second case, just test for ukValue null.
			//  - Entity association is unset in session but not yet persisted, autoflush disabled: ignore. We are
			//    already in an error case: querying entities changed in session without flushing them before querying.
			//    So here it gets loaded as if it were still associated, but we do not have the key anymore in session:
			//    we cannot cache it, so long for the additionnal round-trip this will cause. (Do not fallback on
			//    reading the key in rs, this is stale data in regard to the session state.)
			if (ukValue == null)
				return;
			var type = persister.PropertyTypes[index];
			if (!alreadyLoaded)
				type = type.GetSemiResolvedType(session.Factory);
			var euk = new EntityUniqueKey(persister.EntityName, ukName, ukValue, type, session.Factory);
			session.PersistenceContext.AddEntity(euk, obj);
		}

		/// <summary>
		/// The entity instance is not in the session cache
		/// </summary>
		private object InstanceNotYetLoaded(DbDataReader dr, int i, ILoadable persister, EntityKey key, LockMode lockMode,
											EntityKey optionalObjectKey, object optionalObject,
											IList hydratedObjects, ISessionImplementor session)
		{
			object obj;

			ILoadable concretePersister = GetConcretePersister(dr, i, persister, key.Identifier, session);

			if (optionalObjectKey != null && key.Equals(optionalObjectKey))
			{
				// its the given optional object
				obj = optionalObject;
			}
			else
			{
				obj = session.Instantiate(concretePersister, key.Identifier);
			}

			// need to hydrate it

			// grab its state from the DataReader and keep it in the Session
			// (but don't yet initialize the object itself)
			// note that we acquired LockMode.READ even if it was not requested
			LockMode acquiredLockMode = lockMode == LockMode.None ? LockMode.Read : lockMode;
			LoadFromResultSet(dr, i, obj, concretePersister, key, acquiredLockMode, persister, session);

			// materialize associations (and initialize the object) later
			hydratedObjects.Add(obj);

			return obj;
		}

		protected virtual bool IsChildFetchEntity(int i)
		{
			return false;
		}

		private bool IsEagerPropertyFetchEnabled(int i)
		{
			bool[] array = EntityEagerPropertyFetches;
			return array != null && array[i];
		}

		private HashSet<string> GetFetchLazyProperties(int i)
		{
			var array = EntityFetchLazyProperties;
			return array?[i];
		}

		private void UpdateLazyPropertiesFromResultSet(DbDataReader rs, int i, object obj, EntityKey key,
		                                               EntityEntry optionalEntry, ILoadable rootPersister, ISessionImplementor session,
		                                               Action<IEntityPersister, CachePutData> cacheBatchingHandler)
		{
			var fetchAllProperties = IsEagerPropertyFetchEnabled(i);
			var fetchLazyProperties = GetFetchLazyProperties(i);

			if (!fetchAllProperties && fetchLazyProperties == null)
			{
				return; // No lazy properties were loaded
			}

			var persister = GetConcretePersister(rs, i, rootPersister, key.Identifier, session);
			var entry = optionalEntry ?? session.PersistenceContext.GetEntry(obj);
			// The property values will not be set when the entry status is Loading so in that case we have to get
			// the uninitialized lazy properties from the loaded state
			var uninitializedProperties = entry.Status == Status.Loading
				? persister.EntityMetamodel.BytecodeEnhancementMetadata.GetUninitializedLazyProperties(entry.LoadedState)
				: persister.EntityMetamodel.BytecodeEnhancementMetadata.GetUninitializedLazyProperties(obj);

			var updateLazyProperties = fetchLazyProperties?.Intersect(uninitializedProperties).ToArray();
			if (updateLazyProperties?.Length == 0)
			{
				return; // No new lazy properites were loaded
			}

			var id = key.Identifier;

			if (Log.IsDebugEnabled())
			{
				Log.Debug("Updating lazy properites from DataReader: {0}", MessageHelper.InfoString(persister, id));
			}

			var cols = persister == rootPersister
				? EntityAliases[i].SuffixedPropertyAliases
				: GetSubclassEntityAliases(i, persister);

			if (!persister.InitializeLazyProperties(rs, id, obj, cols, updateLazyProperties, fetchAllProperties, session))
			{
				return;
			}

			UpdateCacheForEntity(obj, id, entry, persister, session, cacheBatchingHandler);
		}

		internal static void UpdateCacheForEntity(
			object obj, object id, EntityEntry entry, IEntityPersister persister, ISessionImplementor session,
			Action<IEntityPersister, CachePutData> cacheBatchingHandler)
		{
			if (entry.Status == Status.Loading || !persister.HasCache ||
			    !session.CacheMode.HasFlag(CacheMode.Put) || !persister.IsLazyPropertiesCacheable)
			{
				return;
			}

			if (Log.IsDebugEnabled())
			{
				Log.Debug("Updating entity to second-level cache: {0}", MessageHelper.InfoString(persister, id, session.Factory));
			}

			var factory = session.Factory;
			var state = persister.GetPropertyValues(obj);
			var version = Versioning.GetVersion(state, persister);
			var cacheEntry = CacheEntry.Create(state, persister, version, session, obj);
			var cacheKey = session.GenerateCacheKey(id, persister.IdentifierType, persister.RootEntityName);

			if (cacheBatchingHandler != null && persister.IsBatchLoadable)
			{
				cacheBatchingHandler(
					persister,
					new CachePutData(
						cacheKey,
						persister.CacheEntryStructure.Structure(cacheEntry),
						version,
						persister.IsVersioned ? persister.VersionType.Comparator : null,
						false));
			}
			else
			{
				var put =
					persister.Cache.Put(cacheKey, persister.CacheEntryStructure.Structure(cacheEntry), session.Timestamp, version,
										persister.IsVersioned ? persister.VersionType.Comparator : null,
										false);

				if (put && factory.Statistics.IsStatisticsEnabled)
				{
					factory.StatisticsImplementor.SecondLevelCachePut(persister.Cache.RegionName);
				}
			}
		}

		/// <summary>
		/// Hydrate the state of an object from the SQL <c>DbDataReader</c>, into
		/// an array of "hydrated" values (do not resolve associations yet),
		/// and pass the hydrated state to the session.
		/// </summary>
		private void LoadFromResultSet(DbDataReader rs, int i, object obj, ILoadable persister, EntityKey key,
									   LockMode lockMode, ILoadable rootPersister,
									   ISessionImplementor session)
		{
			object id = key.Identifier;

			if (Log.IsDebugEnabled())
			{
				Log.Debug("Initializing object from DataReader: {0}", MessageHelper.InfoString(persister, id));
			}

			bool fetchAllProperties = IsEagerPropertyFetchEnabled(i);
			var eagerFetchProperties = GetFetchLazyProperties(i);

			// add temp entry so that the next step is circular-reference
			// safe - only needed because some types don't take proper
			// advantage of two-phase-load (esp. components)
			TwoPhaseLoad.AddUninitializedEntity(key, obj, persister, lockMode, session);

			string[][] cols = persister == rootPersister
								? EntityAliases[i].SuffixedPropertyAliases
								: GetSubclassEntityAliases(i, persister);

			object[] values = persister.Hydrate(rs, id, obj, cols, eagerFetchProperties, fetchAllProperties, session);

			object rowId = persister.HasRowId ? rs[EntityAliases[i].RowIdAlias] : null;

			TwoPhaseLoad.PostHydrate(persister, id, values, rowId, obj, lockMode, session);
		}

		private string[][] GetSubclassEntityAliases(int i, ILoadable persister)
		{
			var cacheKey = System.Tuple.Create(i, persister.EntityName);
			return _subclassEntityAliasesMap.GetOrAdd(
				cacheKey,
				k => EntityAliases[i].GetSuffixedPropertyAliases(persister));
		}

		/// <summary>
		/// Determine the concrete class of an instance for the <c>DbDataReader</c>
		/// </summary>
		private ILoadable GetConcretePersister(DbDataReader rs, int i, ILoadable persister, object id, ISessionImplementor session)
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

				return persister.EntityName == result
					? persister
					: (ILoadable) Factory.GetEntityPersister(result);
			}
			return persister;
		}

		/// <summary>
		/// Advance the cursor to the first required row of the <c>DbDataReader</c>
		/// </summary>
		internal static void Advance(DbDataReader rs, RowSelection selection)
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

		private static bool HasOffset(RowSelection selection)
		{
			return selection != null && selection.FirstRow != RowSelection.NoValue;
		}

		internal static int GetFirstRow(RowSelection selection)
		{
			if (selection == null || !selection.DefinesLimits)
			{
				return 0;
			}
			return selection.FirstRow > 0 ? selection.FirstRow : 0;
		}

		/// <summary>
		/// Should we pre-process the SQL string, adding a dialect-specific
		/// LIMIT clause.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		internal bool UseLimit(RowSelection selection, Dialect.Dialect dialect)
		{
			return (_canUseLimits ?? true)
				&& dialect.SupportsLimit
				&& (HasMaxRows(selection) || HasOffset(selection));
		}

		/// <summary>
		/// Performs dialect-specific manipulations on the offset value before returning it.
		/// This method is applicable for use in limit statements only.
		/// </summary>
		internal static int? GetOffsetUsingDialect(RowSelection selection, Dialect.Dialect dialect)
		{
			int firstRow = GetFirstRow(selection);
			if (firstRow == 0)
				return null;
			return dialect.GetOffsetValue(firstRow);
		}

		/// <summary>
		/// Performs dialect-specific manipulations on the limit value before returning it.
		/// This method is applicable for use in limit statements only.
		/// </summary>
		internal static int? GetLimitUsingDialect(RowSelection selection, Dialect.Dialect dialect)
		{
			if (selection == null || selection.MaxRows == RowSelection.NoValue)
				return null;
			return dialect.GetLimitValue(GetFirstRow(selection), selection.MaxRows);
		}

		/// <summary>
		/// Obtain an <c>DbCommand</c> with all parameters pre-bound. Bind positional parameters,
		/// named parameters, and limit parameters.
		/// </summary>
		/// <remarks>
		/// Creates an DbCommand object and populates it with the values necessary to execute it against the 
		/// database to Load an Entity.
		/// </remarks>
		/// <param name="queryParameters">The <see cref="QueryParameters"/> to use for the DbCommand.</param>
		/// <param name="scroll">TODO: find out where this is used...</param>
		/// <param name="session">The SessionImpl this Command is being prepared in.</param>
		/// <returns>A CommandWrapper wrapping an DbCommand that is ready to be executed.</returns>
		protected internal virtual DbCommand PrepareQueryCommand(QueryParameters queryParameters, bool scroll, ISessionImplementor session)
		{
			ISqlCommand sqlCommand = CreateSqlCommand(queryParameters, session);
			SqlString sqlString = sqlCommand.Query;

			sqlCommand.ResetParametersIndexesForTheCommand(0);
			var command = session.Batcher.PrepareQueryCommand(CommandType.Text, sqlString, sqlCommand.ParameterTypes);

			try
			{
				RowSelection selection = queryParameters.RowSelection;
				if (selection != null && selection.Timeout != RowSelection.NoValue)
				{
					command.CommandTimeout = selection.Timeout;
				}

				sqlCommand.Bind(command, session);

				IDriver driver = _factory.ConnectionProvider.Driver;
				driver.RemoveUnusedCommandParameters(command, sqlString);
				driver.ExpandQueryParameters(command, sqlString, sqlCommand.ParameterTypes);
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

		/// <summary> 
		/// Some dialect-specific LIMIT clauses require the maximum last row number
		/// (aka, first_row_number + total_row_count), while others require the maximum
		/// returned row count (the total maximum number of rows to return). 
		/// </summary>
		/// <param name="selection">The selection criteria </param>
		/// <param name="dialect">The dialect </param>
		/// <returns> The appropriate value to bind into the limit clause. </returns>
		internal static int GetMaxOrLimit(Dialect.Dialect dialect, RowSelection selection)
		{
			int firstRow = GetFirstRow(selection);
			int rowCount = selection.MaxRows;

			if (rowCount == RowSelection.NoValue)
				return int.MaxValue;

			return dialect.GetLimitValue(firstRow, rowCount);
		}

		/// <summary>
		/// Fetch a <c>DbCommand</c>, call <c>SetMaxRows</c> and then execute it,
		/// advance to the first result and return an SQL <c>DbDataReader</c>
		/// </summary>
		/// <param name="st">The <see cref="DbCommand" /> to execute.</param>
		/// <param name="selection">The <see cref="RowSelection"/> to apply to the <see cref="DbCommand"/> and <see cref="DbDataReader"/>.</param>
		/// <param name="autoDiscoverTypes">true if result types need to be auto-discovered by the loader; false otherwise.</param>
		/// <param name="session">The <see cref="ISession" /> to load in.</param>
		/// <param name="callable"></param>
		/// <returns>An DbDataReader advanced to the first record in RowSelection.</returns>
		// Since v5.1
		[Obsolete("Please use overload with a QueryParameter parameter.")]
		protected DbDataReader GetResultSet(DbCommand st, bool autoDiscoverTypes, bool callable, RowSelection selection, ISessionImplementor session)
		{
			return GetResultSet(
				st,
				new QueryParameters
				{
					HasAutoDiscoverScalarTypes = autoDiscoverTypes, Callable = callable, RowSelection = selection
				},
				session,
				null);
		}

		/// <summary>
		/// Fetch a <c>DbCommand</c>, call <c>SetMaxRows</c> and then execute it,
		/// advance to the first result and return an SQL <c>DbDataReader</c>
		/// </summary>
		/// <param name="st">The <see cref="DbCommand" /> to execute.</param>
		/// <param name="queryParameters">The <see cref="QueryParameters"/>.</param>
		/// <param name="session">The <see cref="ISession" /> to load in.</param>
		/// <param name="forcedResultTransformer">The forced result transformer for the query.</param>
		/// <returns>A DbDataReader advanced to the first record in RowSelection.</returns>
		protected DbDataReader GetResultSet(
			DbCommand st, QueryParameters queryParameters, ISessionImplementor session, IResultTransformer forcedResultTransformer)
		{
			DbDataReader rs = null;
			try
			{
				// TODO NH: Callable
				rs = session.Batcher.ExecuteReader(st);

				//NH: this is checked outside the WrapResultSet because we
				// want to avoid the syncronization overhead in the vast majority
				// of cases where IsWrapResultSetsEnabled is set to false
				if (session.Factory.Settings.IsWrapResultSetsEnabled)
					rs = WrapResultSet(rs);

				Dialect.Dialect dialect = session.Factory.Dialect;
				if (!dialect.SupportsLimitOffset || !UseLimit(queryParameters.RowSelection, dialect))
				{
					Advance(rs, queryParameters.RowSelection);
				}

				if (queryParameters.HasAutoDiscoverScalarTypes)
				{
					AutoDiscoverTypes(rs, queryParameters, forcedResultTransformer);
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

		// Since v5.1
		[Obsolete("Please use overload with a QueryParameters parameter.")]
		protected internal virtual void AutoDiscoverTypes(DbDataReader rs)
		{
			AutoDiscoverTypes(rs, new QueryParameters(), null);
		}

		protected internal virtual void AutoDiscoverTypes(
			DbDataReader rs, QueryParameters queryParameters, IResultTransformer forcedResultTransformer)
		{
			throw new AssertionFailure("Auto discover types not supported in this loader");
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private DbDataReader WrapResultSet(DbDataReader rs)
		{
			// synchronized to avoid multi-thread access issues; defined as method synch to avoid
			// potential deadlock issues due to nature of code.
			try
			{
				if (Log.IsDebugEnabled())
				{
					// Do not log the result set as-is, it is an IEnumerable which may get enumerated by loggers.
					// (Serilog does that.) See #1667.
					Log.Debug("Wrapping result set [{0}]", rs.GetType());
				}

				return new ResultSetWrapper(rs, RetreiveColumnNameToIndexCache(rs));
			}
			catch (Exception e)
			{
				Log.Info(e, "Error wrapping result set");
				return rs;
			}
		}

		private ColumnNameCache RetreiveColumnNameToIndexCache(DbDataReader rs)
		{
			if (_columnNameCache == null)
			{
				Log.Debug("Building columnName->columnIndex cache");
				_columnNameCache = new ColumnNameCache(rs.FieldCount);
			}

			return _columnNameCache;
		}

		/// <summary>
		/// Called by subclasses that load entities
		/// </summary>
		protected IList LoadEntity(ISessionImplementor session, object id, IType identifierType, object optionalObject,
								   string optionalEntityName, object optionalIdentifier, IEntityPersister persister)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("loading entity: {0}", MessageHelper.InfoString(persister, id, identifierType, Factory));
			}

			IList result;

			try
			{
				QueryParameters qp =
					new QueryParameters(new IType[] { identifierType }, new object[] { id }, optionalObject, optionalEntityName,
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

			Log.Debug("done entity load");

			return result;
		}

		protected IList LoadEntity(ISessionImplementor session, object key, object index, IType keyType, IType indexType,
								   IEntityPersister persister)
		{
			Log.Debug("loading collection element by index");

			IList result;
			try
			{
				result =
					DoQueryAndInitializeNonLazyCollections(session,
														   new QueryParameters(new IType[] { keyType, indexType },
																			   new object[] { key, index }), false);
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(_factory.SQLExceptionConverter, sqle, "could not collection element by index",
												 SqlString);
			}

			Log.Debug("done entity load");

			return result;
		}

		/// <summary>
		/// Called by subclasses that batch load entities
		/// </summary>
		protected internal IList LoadEntityBatch(ISessionImplementor session, object[] ids, IType idType,
												 object optionalObject, string optionalEntityName, object optionalId,
												 IEntityPersister persister)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("batch loading entity: {0}", MessageHelper.InfoString(persister, ids, Factory));
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

			Log.Debug("done entity batch load");
			return result;
		}

		/// <summary>
		/// Called by subclasses that load collections
		/// </summary>
		public void LoadCollection(ISessionImplementor session, object id, IType type)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("loading collection: {0}", MessageHelper.CollectionInfoString(CollectionPersisters[0], id));
			}

			object[] ids = new object[] { id };
			try
			{
				DoQueryAndInitializeNonLazyCollections(session, new QueryParameters(new IType[] { type }, ids, ids), true);
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
												 + MessageHelper.CollectionInfoString(CollectionPersisters[0], id), SqlString);
			}

			Log.Debug("done loading collection");
		}

		/// <summary>
		/// Called by wrappers that batch initialize collections
		/// </summary>
		public void LoadCollectionBatch(ISessionImplementor session, object[] ids, IType type)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("batch loading collection: {0}", MessageHelper.CollectionInfoString(CollectionPersisters[0], ids));
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
												 + MessageHelper.CollectionInfoString(CollectionPersisters[0], ids), SqlString);
			}

			Log.Debug("done batch load");
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
												 + MessageHelper.CollectionInfoString(CollectionPersisters[0], ids), SqlString,
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
		// Since v5.1
		[Obsolete("Please use overload without resultTypes")]
		protected IList List(ISessionImplementor session, QueryParameters queryParameters, ISet<string> querySpaces, IType[] resultTypes)
		{
			ResultTypes = resultTypes;
			return List(session, queryParameters, querySpaces);
		}

		/// <summary>
		/// Return the query results, using the query cache, called
		/// by subclasses that implement cacheable queries
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <param name="querySpaces"></param>
		/// <returns></returns>
		protected IList List(ISessionImplementor session, QueryParameters queryParameters, ISet<string> querySpaces)
		{
			var cacheable = IsCacheable(queryParameters);

			if (cacheable)
			{
				return ListUsingQueryCache(session, queryParameters, querySpaces);
			}
			return ListIgnoreQueryCache(session, queryParameters);
		}

		internal bool IsCacheable(QueryParameters queryParameters)
		{
			return _factory.Settings.IsQueryCacheEnabled && queryParameters.Cacheable;
		}

		private IList ListIgnoreQueryCache(ISessionImplementor session, QueryParameters queryParameters)
		{
			return GetResultList(DoList(session, queryParameters), queryParameters.ResultTransformer);
		}

		private IList ListUsingQueryCache(ISessionImplementor session, QueryParameters queryParameters, ISet<string> querySpaces)
		{
			IQueryCache queryCache = _factory.GetQueryCache(queryParameters.CacheRegion);

			QueryKey key = GenerateQueryKey(session, queryParameters);
			var queryCacheBuilder = new QueryCacheResultBuilder(this);

			IList result = GetResultFromQueryCache(session, queryParameters, querySpaces, queryCache, key);

			if (result == null)
			{
				result = DoList(session, queryParameters, key.ResultTransformer, queryCacheBuilder);
				PutResultInQueryCache(session, queryParameters, queryCache, key, queryCacheBuilder.Result);
			}
			else
			{
				result = queryCacheBuilder.GetResultList(result);
			}

			result = TransformCacheableResults(queryParameters, key.ResultTransformer, result);

			return GetResultList(result, queryParameters.ResultTransformer);
		}

		internal IList TransformCacheableResults(QueryParameters queryParameters, CacheableResultTransformer transformer, IList result)
		{
			var resolvedTransformer = ResolveResultTransformer(queryParameters.ResultTransformer);
			if (resolvedTransformer == null)
				return result;

			return (AreResultSetRowsTransformedImmediately()
					? transformer.RetransformResults(
						result,
						ResultRowAliases,
						queryParameters.ResultTransformer,
						IncludeInResultRow)
					: transformer.UntransformToTuples(result)
				);
		}

		internal QueryKey GenerateQueryKey(ISessionImplementor session, QueryParameters queryParameters)
		{
			ISet<FilterKey> filterKeys = FilterKey.CreateFilterKeys(session.EnabledFilters);
			return new QueryKey(Factory, SqlString, queryParameters, filterKeys,
								CreateCacheableResultTransformer(queryParameters));
		}

		private CacheableResultTransformer CreateCacheableResultTransformer(QueryParameters queryParameters)
		{
			return CacheableResultTransformer.Create(
				queryParameters.ResultTransformer, ResultRowAliases, IncludeInResultRow,
				queryParameters.HasAutoDiscoverScalarTypes, SqlString);
		}

		private IList GetResultFromQueryCache(
			ISessionImplementor session, QueryParameters queryParameters, ISet<string> querySpaces,
			IQueryCache queryCache, QueryKey key)
		{
			if (!queryParameters.CanGetFromCache(session))
				return null;

			var result = queryCache.Get(
				key, queryParameters, 
				queryParameters.HasAutoDiscoverScalarTypes
					? null
					: key.ResultTransformer.GetCachedResultTypes(CacheTypes),
				querySpaces, session);

			if (_factory.Statistics.IsStatisticsEnabled)
			{
				if (result == null)
				{
					_factory.StatisticsImplementor.QueryCacheMiss(QueryIdentifier, queryCache.RegionName);
				}
				else
				{
					_factory.StatisticsImplementor.QueryCacheHit(QueryIdentifier, queryCache.RegionName);
				}
			}

			return result;
		}

		private void PutResultInQueryCache(ISessionImplementor session, QueryParameters queryParameters,
										   IQueryCache queryCache, QueryKey key, IList result)
		{
			if (!queryParameters.CanPutToCache(session))
				return;

			var put = queryCache.Put(
				key, queryParameters,
				key.ResultTransformer.GetCachedResultTypes(CacheTypes),
				result, session);

			if (put && _factory.Statistics.IsStatisticsEnabled)
			{
				_factory.StatisticsImplementor.QueryCachePut(QueryIdentifier, queryCache.RegionName);
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
			return DoList(session, queryParameters, null, null);
		}

		// Since 5.3
		[Obsolete("Use the overload with queryCacheResultBuilder parameter")]
		protected IList DoList(ISessionImplementor session, QueryParameters queryParameters, IResultTransformer forcedResultTransformer)
		{
			return DoList(session, queryParameters, forcedResultTransformer, null);
		}

		protected IList DoList(ISessionImplementor session, QueryParameters queryParameters, IResultTransformer forcedResultTransformer,
		                       QueryCacheResultBuilder queryCacheResultBuilder)
		{
			Stopwatch stopWatch = null;
			if (session.Factory.Statistics.IsStatisticsEnabled)
			{
				stopWatch = Stopwatch.StartNew();
			}

			IList result;
			try
			{
				result = DoQueryAndInitializeNonLazyCollections(session, queryParameters, true, forcedResultTransformer, queryCacheResultBuilder);
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
			if (stopWatch != null)
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
		protected virtual void PostInstantiate() { }

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

		public virtual ISqlCommand CreateSqlCommand(QueryParameters queryParameters, ISessionImplementor session)
		{
			// A distinct-copy of parameter specifications collected during query construction
			var parameterSpecs = new HashSet<IParameterSpecification>(GetParameterSpecifications());
			SqlString sqlString = SqlString.Copy();

			// dynamic-filter parameters: during the createion of the SqlString of allLoader implementation, filters can be added as SQL_TOKEN/string for this reason we have to re-parse the SQL.
			sqlString = ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
			AdjustQueryParametersForSubSelectFetching(sqlString, parameterSpecs, queryParameters);

			// Add limits
			sqlString = AddLimitsParametersIfNeeded(sqlString, parameterSpecs, queryParameters, session);

			// The PreprocessSQL method can modify the SqlString but should never add parameters (or we have to override it)
			sqlString = PreprocessSQL(sqlString, queryParameters, session.Factory.Dialect);

			// After the last modification to the SqlString we can collect all parameters types (there are cases where we can't infer the type during the creation of the query)
			ResetEffectiveExpectedType(parameterSpecs, queryParameters);

			return new SqlCommandImpl(sqlString, parameterSpecs, queryParameters, session.Factory);
		}

		protected virtual void ResetEffectiveExpectedType(IEnumerable<IParameterSpecification> parameterSpecs, QueryParameters queryParameters)
		{
			// Have to be overridden just by those loaders that can't infer the type during the parse process
		}

		protected abstract IEnumerable<IParameterSpecification> GetParameterSpecifications();

		protected void AdjustQueryParametersForSubSelectFetching(SqlString filteredSqlString, IEnumerable<IParameterSpecification> parameterSpecsWithFilters, QueryParameters queryParameters)
		{
			queryParameters.ProcessedSql = filteredSqlString;
			queryParameters.ProcessedSqlParameters = parameterSpecsWithFilters.ToList();
			if (queryParameters.RowSelection != null)
			{
				queryParameters.ProcessedRowSelection = new RowSelection { FirstRow = queryParameters.RowSelection.FirstRow, MaxRows = queryParameters.RowSelection.MaxRows };
			}
		}

		protected SqlString ExpandDynamicFilterParameters(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, ISessionImplementor session)
		{
			return FilterHelper.ExpandDynamicFilterParameters(sqlString, parameterSpecs, session);
		}

		protected SqlString AddLimitsParametersIfNeeded(SqlString sqlString, ICollection<IParameterSpecification> parameterSpecs, QueryParameters queryParameters, ISessionImplementor session)
		{
			var sessionFactory = session.Factory;
			Dialect.Dialect dialect = sessionFactory.Dialect;

			RowSelection selection = queryParameters.RowSelection;
			if (UseLimit(selection, dialect))
			{
				bool hasFirstRow = GetFirstRow(selection) > 0;
				bool useOffset = hasFirstRow && dialect.SupportsLimitOffset;
				int max = GetMaxOrLimit(dialect, selection);
				int? skip = useOffset ? (int?)dialect.GetOffsetValue(GetFirstRow(selection)) : null;
				int? take = max != int.MaxValue ? (int?)max : null;

				Parameter skipSqlParameter = null;
				Parameter takeSqlParameter = null;
				if (skip.HasValue)
				{
					var skipParameter = new QuerySkipParameterSpecification();
					skipSqlParameter = Parameter.Placeholder;
					skipSqlParameter.BackTrack = skipParameter.GetIdsForBackTrack(sessionFactory).First();
					parameterSpecs.Add(skipParameter);
				}
				if (take.HasValue)
				{
					var takeParameter = new QueryTakeParameterSpecification();
					takeSqlParameter = Parameter.Placeholder;
					takeSqlParameter.BackTrack = takeParameter.GetIdsForBackTrack(sessionFactory).First();
					parameterSpecs.Add(takeParameter);
				}
				// The dialect can move the given parameters where he need, what it can't do is generates new parameters loosing the BackTrack.
				SqlString result;
				if (TryGetLimitString(dialect, sqlString, skip, take, skipSqlParameter, takeSqlParameter, out result)) return result;
			}
			return sqlString;
		}

		protected bool TryGetLimitString(Dialect.Dialect dialect, SqlString queryString, int? offset, int? limit, Parameter offsetParameter, Parameter limitParameter, out SqlString result)
		{
			result = dialect.GetLimitString(queryString, offset, limit, offsetParameter, limitParameter);
			if (result != null) return true;

			_canUseLimits = false;
			return false;
		}

		#endregion
	}
}
