using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transaction;
using NHibernate.Type;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;
using HibernateDialect = NHibernate.Dialect.Dialect;

namespace NHibernate.Impl
{
	/// <summary>
	///  Concrete implementation of a SessionFactory.
	/// </summary>
	/// <remarks>
	/// Has the following responsibilities:
	/// <list type="">
	/// <item>
	/// Caches configuration settings (immutably)</item>
	/// <item>
	/// Caches "compiled" mappings - ie. <see cref="IEntityPersister"/> 
	/// and <see cref="ICollectionPersister"/>
	/// </item>
	/// <item>
	/// Caches "compiled" queries (memory sensitive cache)
	/// </item>
	/// <item>
	/// Manages <c>PreparedStatements/IDbCommands</c> - how true in NH?
	/// </item>
	/// <item>
	/// Delegates <c>IDbConnection</c> management to the <see cref="IConnectionProvider"/>
	/// </item>
	/// <item>
	/// Factory for instances of <see cref="ISession"/>
	/// </item>
	/// </list>
	/// <para>
	/// This class must appear immutable to clients, even if it does all kinds of caching
	/// and pooling under the covers.  It is crucial that the class is not only thread safe
	/// , but also highly concurrent.  Synchronization must be used extremely sparingly.
	/// </para>
	/// </remarks>
	/// <seealso cref="NHibernate.Connection.IConnectionProvider"/>
	/// <seealso cref="NHibernate.ISession"/>
	/// <seealso cref="NHibernate.Hql.IQueryTranslator"/>
	/// <seealso cref="NHibernate.Persister.Entity.IEntityPersister"/>
	/// <seealso cref="NHibernate.Persister.Collection.ICollectionPersister"/>
	[Serializable]
	public sealed class SessionFactoryImpl : ISessionFactoryImplementor, IObjectReference
	{
		#region Default entity not found delegate

		private class DefaultEntityNotFoundDelegate : IEntityNotFoundDelegate
		{
			public void HandleEntityNotFound(string entityName, object id)
			{
				throw new ObjectNotFoundException(id, entityName);
			}
		}

		#endregion

		private readonly string name;
		private readonly string uuid;

		[NonSerialized] private readonly IDictionary<string, IEntityPersister> entityPersisters;

		[NonSerialized] private readonly IDictionary<string, IClassMetadata> classMetadata;

		[NonSerialized] private readonly Dictionary<string, ICollectionPersister> collectionPersisters;

		[NonSerialized] private readonly Dictionary<string, ICollectionMetadata> collectionMetadata;

		[NonSerialized] private readonly Dictionary<string, ISet<string>> collectionRolesByEntityParticipant;

		[NonSerialized] private readonly Dictionary<string, IIdentifierGenerator> identifierGenerators;

		[NonSerialized] private readonly Dictionary<string, NamedQueryDefinition> namedQueries;

		[NonSerialized] private readonly Dictionary<string, NamedSQLQueryDefinition> namedSqlQueries;

		[NonSerialized] private readonly Dictionary<string, ResultSetMappingDefinition> sqlResultSetMappings;

		[NonSerialized] private readonly Dictionary<string, FilterDefinition> filters;

		[NonSerialized] private readonly Dictionary<string, string> imports;

		[NonSerialized] private readonly IInterceptor interceptor;

		[NonSerialized] private readonly Settings settings;

		[NonSerialized] private readonly IDictionary<string, string> properties;

		[NonSerialized] private readonly SchemaExport schemaExport;

		[NonSerialized] private readonly IQueryCache queryCache;

		[NonSerialized] private readonly UpdateTimestampsCache updateTimestampsCache;

		[NonSerialized] private readonly IDictionary<string, IQueryCache> queryCaches;

		[NonSerialized] private readonly ThreadSafeDictionary<string, ICache> allCacheRegions =
			new ThreadSafeDictionary<string, ICache>(new Dictionary<string, ICache>());

		[NonSerialized] private readonly EventListeners eventListeners;

		[NonSerialized] private readonly SQLFunctionRegistry sqlFunctionRegistry;

		[NonSerialized] private readonly IEntityNotFoundDelegate entityNotFoundDelegate;

		[NonSerialized] private StatisticsImpl statistics;
		[NonSerialized] private bool isClosed = false;

		private QueryPlanCache queryPlanCache;

		private static readonly IIdentifierGenerator UuidGenerator = new UUIDHexGenerator();

		private static readonly ILog log = LogManager.GetLogger(typeof (SessionFactoryImpl));

		private void Init()
		{
			statistics = new StatisticsImpl(this);
			queryPlanCache = new QueryPlanCache(this);
		}

		public SessionFactoryImpl(Configuration cfg, IMapping mapping, Settings settings, EventListeners listeners)
		{
			Init();
			log.Info("building session factory");

			properties = cfg.Properties;
			interceptor = cfg.Interceptor;
			this.settings = settings;
			sqlFunctionRegistry = new SQLFunctionRegistry(settings.Dialect, cfg.SqlFunctions);
			eventListeners = listeners;

			if (log.IsDebugEnabled)
			{
				log.Debug("instantiating session factory with properties: " + CollectionPrinter.ToString(properties));
			}

			settings.CacheProvider.Start(properties);

			// Generators:
			identifierGenerators = new Dictionary<string, IIdentifierGenerator>();
			foreach (PersistentClass model in cfg.ClassMappings)
			{
				if (!model.IsInherited)
				{
					IIdentifierGenerator generator =
						model.Identifier.CreateIdentifierGenerator(settings.Dialect, settings.DefaultCatalogName,
						                                           settings.DefaultSchemaName, (RootClass) model);

					identifierGenerators[model.EntityName] = generator;
				}
			}

			// Persisters:

			Dictionary<string, ICacheConcurrencyStrategy> caches = new Dictionary<string, ICacheConcurrencyStrategy>();
			entityPersisters = new Dictionary<string, IEntityPersister>();
			Dictionary<string, IClassMetadata> classMeta = new Dictionary<string, IClassMetadata>();

			foreach (PersistentClass model in cfg.ClassMappings)
			{
				string cacheRegion = model.RootClazz.CacheRegionName;
				ICacheConcurrencyStrategy cache;
				if (!caches.TryGetValue(cacheRegion, out cache))
				{
					cache =
						CacheFactory.CreateCache(model.CacheConcurrencyStrategy, cacheRegion, model.IsMutable, settings, properties);
					if (cache != null)
					{
						caches.Add(cacheRegion, cache);
						allCacheRegions.Add(cache.RegionName, cache.Cache);
					}
				}
				IEntityPersister cp = PersisterFactory.CreateClassPersister(model, cache, this, mapping);
				entityPersisters[model.EntityName] = cp;
				classMeta[model.EntityName] = cp.ClassMetadata;
			}
			classMetadata = new UnmodifiableDictionary<string, IClassMetadata>(classMeta);
			Dictionary<string, ISet<string>> tmpEntityToCollectionRoleMap = new Dictionary<string, ISet<string>>();
			collectionPersisters = new Dictionary<string, ICollectionPersister>();
			foreach (Mapping.Collection map in cfg.CollectionMappings)
			{
				ICacheConcurrencyStrategy cache =
					CacheFactory.CreateCache(map.CacheConcurrencyStrategy, map.CacheRegionName, map.Owner.IsMutable, settings,
					                         properties);
				if (cache != null)
				{
					allCacheRegions[cache.RegionName] = cache.Cache;
				}

				ICollectionPersister persister = PersisterFactory.CreateCollectionPersister(cfg, map, cache, this);
				collectionPersisters[map.Role] = persister;
				IType indexType = persister.IndexType;
				if (indexType != null && indexType.IsAssociationType && !indexType.IsAnyType)
				{
					string entityName = ((IAssociationType) indexType).GetAssociatedEntityName(this);
					ISet<string> roles;
					if (!tmpEntityToCollectionRoleMap.TryGetValue(entityName, out roles))
					{
						roles = new HashedSet<string>();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
				IType elementType = persister.ElementType;
				if (elementType.IsAssociationType && !elementType.IsAnyType)
				{
					string entityName = ((IAssociationType) elementType).GetAssociatedEntityName(this);
					ISet<string> roles;
					if (!tmpEntityToCollectionRoleMap.TryGetValue(entityName, out roles))
					{
						roles = new HashedSet<string>();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
			}
			collectionMetadata = new Dictionary<string, ICollectionMetadata>(collectionPersisters.Count);
			foreach (KeyValuePair<string, ICollectionPersister> collectionPersister in collectionPersisters)
			{
				collectionMetadata.Add(collectionPersister.Key, collectionPersister.Value.CollectionMetadata);
			}

			collectionRolesByEntityParticipant = new Dictionary<string, ISet<string>>(tmpEntityToCollectionRoleMap);

			// serialization info
			name = settings.SessionFactoryName;
			try
			{
				uuid = (string) UuidGenerator.Generate(null, null);
			}
			catch (Exception)
			{
				throw new AssertionFailure("could not generate UUID");
			}

			SessionFactoryObjectFactory.AddInstance(uuid, name, this, properties);

			namedQueries = new Dictionary<string, NamedQueryDefinition>(cfg.NamedQueries);
			namedSqlQueries = new Dictionary<string, NamedSQLQueryDefinition>(cfg.NamedSQLQueries);
			sqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>(cfg.SqlResultSetMappings);
			filters = new Dictionary<string, FilterDefinition>(cfg.FilterDefinitions);

			imports = new Dictionary<string, string>(cfg.Imports);

			// after *all* persisters and named queries are registered
			foreach (IEntityPersister persister in entityPersisters.Values)
			{
				persister.PostInstantiate();
			}

			foreach (ICollectionPersister persister in collectionPersisters.Values)
			{
				persister.PostInstantiate();
			}

			log.Debug("Instantiated session factory");

			if (settings.IsAutoCreateSchema)
			{
				new SchemaExport(cfg).Create(false, true);
			}

			/*
			if ( settings.IsAutoUpdateSchema )
			{
				new SchemaUpdate( cfg ).Execute( false, true );
			}
			*/

			if (settings.IsAutoDropSchema)
			{
				schemaExport = new SchemaExport(cfg);
			}

			// Obtaining TransactionManager - not ported from H2.1

			currentSessionContext = BuildCurrentSessionContext();

			if (settings.IsQueryCacheEnabled)
			{
				updateTimestampsCache = new UpdateTimestampsCache(settings, properties);
				queryCache = settings.QueryCacheFactory.GetQueryCache(null, updateTimestampsCache, settings, properties);
				queryCaches = new ThreadSafeDictionary<string, IQueryCache>(new Dictionary<string, IQueryCache>());
			}
			else
			{
				updateTimestampsCache = null;
				queryCache = null;
				queryCaches = null;
			}

			//checking for named queries
			if (settings.IsNamedQueryStartupCheckingEnabled)
			{
				IDictionary<string, HibernateException> errors = CheckNamedQueries();
				if (errors.Count > 0)
				{
					StringBuilder failingQueries = new StringBuilder("Errors in named queries: ");
					foreach (KeyValuePair<string, HibernateException> pair in errors)
					{
						failingQueries.Append('{').Append(pair.Key).Append('}');
						log.Error("Error in named query: " + pair.Key, pair.Value);
					}
					throw new HibernateException(failingQueries.ToString());
				}
			}

			Statistics.IsStatisticsEnabled = settings.IsStatisticsEnabled;

			// EntityNotFoundDelegate
			IEntityNotFoundDelegate enfd = cfg.EntityNotFoundDelegate;
			if (enfd == null)
			{
				enfd = new DefaultEntityNotFoundDelegate();
			}
			entityNotFoundDelegate = enfd;
		}

		private IDictionary<string, HibernateException> CheckNamedQueries()
		{
			IDictionary<string, HibernateException> errors = new Dictionary<string, HibernateException>();

			// Check named HQL queries
			log.Debug("Checking " + namedQueries.Count + " named HQL queries");
			foreach (KeyValuePair<string, NamedQueryDefinition> entry in namedQueries)
			{
				string queryName = entry.Key;
				NamedQueryDefinition qd = entry.Value;
				// this will throw an error if there's something wrong.
				try
				{
					log.Debug("Checking named query: " + queryName);
					//TODO: BUG! this currently fails for named queries for non-POJO entities
					queryPlanCache.GetHQLQueryPlan(qd.QueryString, false, new CollectionHelper.EmptyMapClass<string, IFilter>());
				}
				catch (QueryException e)
				{
					errors[queryName] = e;
				}
				catch (MappingException e)
				{
					errors[queryName] = e;
				}
			}

			log.Debug("Checking " + namedSqlQueries.Count + " named SQL queries");
			foreach (KeyValuePair<string, NamedSQLQueryDefinition> entry in namedSqlQueries)
			{
				string queryName = entry.Key;
				NamedSQLQueryDefinition qd = entry.Value;
				// this will throw an error if there's something wrong.
				try
				{
					log.Debug("Checking named SQL query: " + queryName);
					// TODO : would be really nice to cache the spec on the query-def so as to not have to re-calc the hash;
					// currently not doable though because of the resultset-ref stuff...
					NativeSQLQuerySpecification spec;
					if (qd.ResultSetRef != null)
					{
						ResultSetMappingDefinition definition = sqlResultSetMappings[qd.ResultSetRef];
						if (definition == null)
						{
							throw new MappingException("Unable to find resultset-ref definition: " + qd.ResultSetRef);
						}
						spec = new NativeSQLQuerySpecification(qd.QueryString, definition.GetQueryReturns(), qd.QuerySpaces);
					}
					else
					{
						spec = new NativeSQLQuerySpecification(qd.QueryString, qd.QueryReturns, qd.QuerySpaces);
					}
					queryPlanCache.GetNativeSQLQueryPlan(spec);
				}
				catch (QueryException e)
				{
					errors[queryName] = e;
				}
				catch (MappingException e)
				{
					errors[queryName] = e;
				}
			}

			return errors;
		}

		// Emulates constant time LRU/MRU algorithms for cache
		// It is better to hold strong references on some (LRU/MRU) queries
		private const int MaxStrongRefCount = 128;

		[NonSerialized] private readonly object[] strongRefs = new object[MaxStrongRefCount];

		[NonSerialized] private int strongRefIndex = 0;

		[NonSerialized] private readonly ICurrentSessionContext currentSessionContext;

		/// <summary>
		/// A class that can be used as a Key in a Hashtable for 
		/// a Query Cache.
		/// </summary>
		[Serializable]
		private class QueryCacheKey
		{
			private readonly string _query;
			private readonly bool _scalar;
			private readonly ISet<string> _filterNames;
			private readonly int _hashCode;

			internal QueryCacheKey(string query, bool scalar, IDictionary<string, IFilter> enabledFilters)
			{
				_query = query;
				_scalar = scalar;
				if (enabledFilters == null || enabledFilters.Count == 0)
				{
					_filterNames = new HashedSet<string>();
				}
				else
				{
					_filterNames = new HashedSet<string>(enabledFilters.Keys);
				}

				unchecked
				{
					_hashCode = query.GetHashCode();
					_hashCode = 29 * _hashCode + (scalar ? 1 : 0);
					_hashCode = 29 * _hashCode + CollectionHelper.GetHashCode(_filterNames);
				}
			}

			public string Query
			{
				get { return _query; }
			}

			public bool Scalar
			{
				get { return _scalar; }
			}

			public ISet<string> FilterNames
			{
				get { return _filterNames; }
			}

			#region System.Object Members

			public override bool Equals(object obj)
			{
				QueryCacheKey other = obj as QueryCacheKey;
				if (other == null)
				{
					return false;
				}

				return Equals(other);
			}

			public bool Equals(QueryCacheKey obj)
			{
				return
					_hashCode == obj._hashCode && Query.Equals(obj.Query) && Scalar == obj.Scalar
					&& CollectionHelper.SetEquals(FilterNames, obj.FilterNames);
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}

			#endregion
		}

		private ISession OpenSession(IDbConnection connection, long timestamp, IInterceptor interceptor,
		                             ConnectionReleaseMode connectionReleaseMode)
		{
			SessionImpl sessionImpl = new SessionImpl(connection, this, timestamp, interceptor, connectionReleaseMode);
			bool isSessionScopedInterceptor = this.interceptor != interceptor;
			if (isSessionScopedInterceptor)
			{
				interceptor.SetSession(sessionImpl);
			}
			sessionImpl.Initialize();
			return sessionImpl;
		}

		public ISession OpenSession()
		{
			return OpenSession(interceptor);
		}

		public ISession OpenSession(IDbConnection connection)
		{
			return OpenSession(connection, interceptor);
		}

		public ISession OpenSession(IDbConnection connection, IInterceptor interceptor)
		{
			return OpenSession(connection, long.MinValue, interceptor, settings.ConnectionReleaseMode);
		}

		public ISession OpenSession(IInterceptor interceptor)
		{
			long timestamp = settings.CacheProvider.NextTimestamp();
			return OpenSession(null, timestamp, interceptor, settings.ConnectionReleaseMode);
		}

		public ISession OpenSession(IDbConnection connection, bool flushBeforeCompletionEnabled, bool autoCloseSessionEnabled,
		                            ConnectionReleaseMode connectionReleaseMode)
		{
			return
				new SessionImpl(connection, this, true, settings.CacheProvider.NextTimestamp(), interceptor,
				                settings.DefaultEntityMode, flushBeforeCompletionEnabled, autoCloseSessionEnabled,
				                connectionReleaseMode);
		}

		public IEntityPersister GetEntityPersister(string entityName)
		{
			try
			{
				return entityPersisters[entityName];
			}
			catch (KeyNotFoundException)
			{
				throw new MappingException("No persister for: " + entityName);
			}
		}

		public IEntityPersister TryGetEntityPersister(string entityName)
		{
			IEntityPersister result;
			entityPersisters.TryGetValue(entityName, out result);
			return result;
		}

		public ICollectionPersister GetCollectionPersister(string role)
		{
			ICollectionPersister result;
			if (!collectionPersisters.TryGetValue(role, out result))
			{
				throw new MappingException("Unknown collection role: " + role);
			}
			return result;
		}

		public ISet<string> GetCollectionRolesByEntityParticipant(string entityName)
		{
			ISet<string> result;
			collectionRolesByEntityParticipant.TryGetValue(entityName, out result);
			return result;
		}

		/// <summary></summary>
		public HibernateDialect Dialect
		{
			get { return settings.Dialect; }
		}

		public IInterceptor Interceptor
		{
			get { return interceptor; }
		}

		/// <summary></summary>
		public ITransactionFactory TransactionFactory
		{
			get { return settings.TransactionFactory; }
		}

		// TransactionManager - not ported

		public ISQLExceptionConverter SQLExceptionConverter
		{
			get { return settings.SqlExceptionConverter; }
		}

		#region System.Runtime.Serialization.IObjectReference Members

		public object GetRealObject(StreamingContext context)
		{
			// the SessionFactory that was serialized only has values in the properties
			// "name" and "uuid".  In here convert the serialized SessionFactory into
			// an instance of the SessionFactory in the current AppDomain.
			log.Debug("Resolving serialized SessionFactory");

			// look for the instance by uuid - this will work when a SessionFactory
			// is serialized and deserialized in the same AppDomain.
			ISessionFactory result = SessionFactoryObjectFactory.GetInstance(uuid);
			if (result == null)
			{
				// if we were deserialized into a different AppDomain, look for an instance with the
				// same name.
				result = SessionFactoryObjectFactory.GetNamedInstance(name);
				if (result == null)
				{
					throw new NullReferenceException("Could not find a SessionFactory named " + name + " or identified by uuid " + uuid);
				}
				else
				{
					log.Debug("resolved SessionFactory by name");
				}
			}
			else
			{
				log.Debug("resolved SessionFactory by uuid");
			}

			return result;
		}

		#endregion

		/// <summary>
		/// Gets the <c>hql</c> query identified by the <c>name</c>.
		/// </summary>
		/// <param name="queryName">The name of that identifies the query.</param>
		/// <returns>
		/// A <c>hql</c> query or <see langword="null" /> if the named
		/// query does not exist.
		/// </returns>
		public NamedQueryDefinition GetNamedQuery(string queryName)
		{
			NamedQueryDefinition result;
			namedQueries.TryGetValue(queryName, out result);
			return result;
		}

		public NamedSQLQueryDefinition GetNamedSQLQuery(string queryName)
		{
			NamedSQLQueryDefinition result;
			namedSqlQueries.TryGetValue(queryName, out result);
			return result;
		}

		public IType GetIdentifierType(string className)
		{
			return GetEntityPersister(className).IdentifierType;
		}

		public string GetIdentifierPropertyName(string className)
		{
			return GetEntityPersister(className).IdentifierPropertyName;
		}

		public IType[] GetReturnTypes(String queryString)
		{
			return
				queryPlanCache.GetHQLQueryPlan(queryString, false, new CollectionHelper.EmptyMapClass<string, IFilter>()).
					ReturnMetadata.ReturnTypes;
		}

		/// <summary> Get the return aliases of a query</summary>
		public string[] GetReturnAliases(string queryString)
		{
			return
				queryPlanCache.GetHQLQueryPlan(queryString, false, new CollectionHelper.EmptyMapClass<string, IFilter>()).
					ReturnMetadata.ReturnAliases;
		}

		public IClassMetadata GetClassMetadata(System.Type persistentClass)
		{
			return GetClassMetadata(persistentClass.FullName);
		}

		public IClassMetadata GetClassMetadata(string entityName)
		{
			IClassMetadata result;
			classMetadata.TryGetValue(entityName, out result);
			return result;
		}

		public ICollectionMetadata GetCollectionMetadata(string roleName)
		{
			return GetCollectionPersister(roleName) as ICollectionMetadata;
		}

		/// <summary>
		/// Return the names of all persistent (mapped) classes that extend or implement the
		/// given class or interface, accounting for implicit/explicit polymorphism settings
		/// and excluding mapped subclasses/joined-subclasses of other classes in the result.
		/// </summary>
		public string[] GetImplementors(string className)
		{
			System.Type clazz= null;
			// NH Different implementation: we have better performance checking, first of all, if we know the class
			// and take the System.Type directly from the persister (className have high probability to be entityName)
			IEntityPersister checkPersister;
			if (entityPersisters.TryGetValue(className, out checkPersister))
			{
				clazz = checkPersister.GetMappedClass(EntityMode.Poco);
			}

			if (clazz == null)
			{
				try
				{
					clazz = ReflectHelper.ClassForFullName(className);
				}
				catch (Exception)
				{
					return new string[] {className}; //for a dynamic-class
				}
			}

			List<string> results = new List<string>();
			foreach (IEntityPersister p in entityPersisters.Values)
			{
				IQueryable q = p as IQueryable;
				if (q != null)
				{
					string testClassName = q.EntityName;
					bool isMappedClass = className.Equals(testClassName);
					if (q.IsExplicitPolymorphism)
					{
						if (isMappedClass)
						{
							return new string[] { testClassName }; // NOTE EARLY EXIT
						}
					}
					else
					{
						if (isMappedClass)
						{
							results.Add(testClassName);
						}
						else
						{
							System.Type mappedClass = q.GetMappedClass(EntityMode.Poco);
							if (mappedClass != null && clazz.IsAssignableFrom(mappedClass))
							{
								bool assignableSuperclass;
								if (q.IsInherited)
								{
									System.Type mappedSuperclass = GetEntityPersister(q.MappedSuperclass).GetMappedClass(EntityMode.Poco);
									assignableSuperclass = clazz.IsAssignableFrom(mappedSuperclass);
								}
								else
								{
									assignableSuperclass = false;
								}
								if (!assignableSuperclass)
								{
									results.Add(testClassName);
								}
							}
						}
					}
				}
			}
			return results.ToArray();
		}

		public string GetImportedClassName(string className)
		{
			string result;
			if (className != null && imports.TryGetValue(className, out result))
				return result;
			else
				return className;
		}

		/// <summary></summary>
		public IDictionary<string, IClassMetadata> GetAllClassMetadata()
		{
			return classMetadata;
		}

		/// <summary></summary>
		public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
		{
			return collectionMetadata;
		}

		private bool disposed;

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;
			Close();
		}

		/// <summary>
		/// Closes the session factory, releasing all held resources.
		/// <list>
		/// <item>cleans up used cache regions and "stops" the cache provider.</item>
		/// <item>close the ADO.NET connection</item>
		/// </list>
		/// </summary>
		public void Close()
		{
			log.Info("Closing");

			isClosed = true;

			foreach (IEntityPersister p in entityPersisters.Values)
			{
				if (p.HasCache)
				{
					p.Cache.Destroy();
				}
			}

			foreach (ICollectionPersister p in collectionPersisters.Values)
			{
				if (p.HasCache)
				{
					p.Cache.Destroy();
				}
			}

			if (settings.IsQueryCacheEnabled)
			{
				queryCache.Destroy();

				foreach (IQueryCache cache in queryCaches.Values)
				{
					cache.Destroy();
				}

				updateTimestampsCache.Destroy();
			}

			settings.CacheProvider.Stop();

			try
			{
				settings.ConnectionProvider.Dispose();
			}
			finally
			{
				SessionFactoryObjectFactory.RemoveInstance(uuid, name, properties);
			}

			if (settings.IsAutoDropSchema)
			{
				schemaExport.Drop(false, true);
			}
		}

		public void Evict(System.Type persistentClass, object id)
		{
			IEntityPersister p = GetEntityPersister(persistentClass.FullName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + MessageHelper.InfoString(p, id));
				}
				CacheKey ck = new CacheKey(id, p.IdentifierType, p.RootEntityName, EntityMode.Poco, this);
				p.Cache.Remove(ck);
			}
		}

		public void Evict(System.Type persistentClass)
		{
			IEntityPersister p = GetEntityPersister(persistentClass.FullName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + p.EntityName);
				}
				p.Cache.Clear();
			}
		}

		public void EvictEntity(string entityName)
		{
			IEntityPersister p = GetEntityPersister(entityName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + p.EntityName);
				}
				p.Cache.Clear();
			}
		}

		public void EvictEntity(string entityName, object id)
		{
			IEntityPersister p = GetEntityPersister(entityName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + MessageHelper.InfoString(p, id, this));
				}
				CacheKey cacheKey = new CacheKey(id, p.IdentifierType, p.RootEntityName, EntityMode.Poco, this);
				p.Cache.Remove(cacheKey);
			}
		}

		public void EvictCollection(string roleName, object id)
		{
			ICollectionPersister p = GetCollectionPersister(roleName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + MessageHelper.InfoString(p, id));
				}
				CacheKey ck = new CacheKey(id, p.KeyType, p.Role, EntityMode.Poco, this);
				p.Cache.Remove(ck);
			}
		}

		public void EvictCollection(string roleName)
		{
			ICollectionPersister p = GetCollectionPersister(roleName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + p.Role);
				}
				p.Cache.Clear();
			}
		}

		public IType GetReferencedPropertyType(string className, string propertyName)
		{
			return GetEntityPersister(className).GetPropertyType(propertyName);
		}

		/// <summary></summary>
		public IConnectionProvider ConnectionProvider
		{
			get { return settings.ConnectionProvider; }
		}

		public bool IsClosed
		{
			get { return isClosed; }
		}

		public UpdateTimestampsCache UpdateTimestampsCache
		{
			get { return updateTimestampsCache; }
		}

		public IDictionary<string, ICache> GetAllSecondLevelCacheRegions()
		{
			lock (allCacheRegions.SyncRoot)
			{
				return new Dictionary<string, ICache>(allCacheRegions);
			}
		}

		public ICache GetSecondLevelCacheRegion(string regionName)
		{
			ICache result;
			allCacheRegions.TryGetValue(regionName, out result);
			return result;
		}

		/// <summary> Statistics SPI</summary>
		public IStatisticsImplementor StatisticsImplementor
		{
			get { return statistics; }
		}

		public IQueryCache QueryCache
		{
			get { return queryCache; }
		}

		public IQueryCache GetQueryCache(string cacheRegion)
		{
			if (cacheRegion == null)
			{
				return QueryCache;
			}
			if (!settings.IsQueryCacheEnabled)
			{
				return null;
			}

			lock (allCacheRegions.SyncRoot)
			{
				IQueryCache currentQueryCache;
				if (!queryCaches.TryGetValue(cacheRegion,out currentQueryCache))
				{
					currentQueryCache =
						settings.QueryCacheFactory.GetQueryCache(cacheRegion, updateTimestampsCache, settings, properties);
					queryCaches[cacheRegion] = currentQueryCache;
					allCacheRegions[currentQueryCache.RegionName] = currentQueryCache.Cache;
				}
				return currentQueryCache;
			}
		}

		public void EvictQueries()
		{
			if (queryCache != null)
			{
				queryCache.Clear();
				if (queryCaches.Count == 0)
				{
					updateTimestampsCache.Clear();
				}
			}
		}

		public void EvictQueries(string cacheRegion)
		{
			if (cacheRegion == null)
			{
				throw new ArgumentNullException("cacheRegion", "use the zero-argument form to evict the default query cache");
			}
			else
			{
				if (settings.IsQueryCacheEnabled)
				{
					IQueryCache currentQueryCache;
					if (queryCaches.TryGetValue(cacheRegion, out currentQueryCache))
					{
						currentQueryCache.Clear();
					}
				}
			}
		}

		/// <summary></summary>
		public IDbConnection OpenConnection()
		{
			try
			{
				return ConnectionProvider.GetConnection();
			}
			catch (Exception sqle)
			{
				throw new ADOException("cannot open connection", sqle);
			}
		}

		public void CloseConnection(IDbConnection conn)
		{
			try
			{
				ConnectionProvider.CloseConnection(conn);
			}
			catch (Exception e)
			{
				throw new ADOException("cannot close connection", e);
			}
		}

		public IIdentifierGenerator GetIdentifierGenerator(string rootEntityName)
		{
			IIdentifierGenerator result;
			identifierGenerators.TryGetValue(rootEntityName, out result);
			return result;
		}

		public ResultSetMappingDefinition GetResultSetMapping(string resultSetName)
		{
			ResultSetMappingDefinition result;
			sqlResultSetMappings.TryGetValue(resultSetName, out result);
			return result;
		}

		public FilterDefinition GetFilterDefinition(string filterName)
		{
			FilterDefinition def = filters[filterName];
			if (def == null)
			{
				throw new HibernateException("No such filter configured [" + filterName + "]");
			}
			return def;
		}

		public ICollection<string> DefinedFilterNames
		{
			get { return filters.Keys; }
		}

		public EventListeners EventListeners
		{
			get { return eventListeners; }
		}

		public Settings Settings
		{
			get { return settings; }
		}

		public ISession GetCurrentSession()
		{
			if (currentSessionContext == null)
			{
				throw new HibernateException("No CurrentSessionContext configured (set the property "
				                             + Environment.CurrentSessionContextClass + ")!");
			}
			return currentSessionContext.CurrentSession();
		}

		/// <summary> Get a new stateless session.</summary>
		public IStatelessSession OpenStatelessSession()
		{
			return new StatelessSessionImpl(null, this);
		}

		/// <summary> Get a new stateless session for the given ADO.NET connection.</summary>
		public IStatelessSession OpenStatelessSession(IDbConnection connection)
		{
			return new StatelessSessionImpl(connection, this);
		}

		/// <summary> Get the statistics for this session factory</summary>
		public IStatistics Statistics
		{
			get { return statistics; }
		}

		/// <summary>
		/// Gets the ICurrentSessionContext instance attached to this session factory.
		/// </summary>
		public ICurrentSessionContext CurrentSessionContext
		{
			get { return currentSessionContext; }
		}

		private ICurrentSessionContext BuildCurrentSessionContext()
		{
			string impl = PropertiesHelper.GetString(Environment.CurrentSessionContextClass, properties, null);

			switch (impl)
			{
				case null:
					return null;
				case "call":
					return new CallSessionContext(this);
				case "thread_static":
					return new ThreadStaticSessionContext(this);
				case "web":
					return new WebSessionContext(this);
				case "managed_web":
					return new ManagedWebSessionContext(this);
			}

			try
			{
				System.Type implClass = ReflectHelper.ClassForName(impl);
				return (ICurrentSessionContext) Activator.CreateInstance(implClass, new object[] {this});
			}
			catch (Exception e)
			{
				log.Error("Unable to construct current session context [" + impl + "]", e);
				return null;
			}
		}

		public SQLFunctionRegistry SQLFunctionRegistry
		{
			get { return sqlFunctionRegistry; }
		}

		public IEntityNotFoundDelegate EntityNotFoundDelegate
		{
			get { return entityNotFoundDelegate; }
		}

		public QueryPlanCache QueryPlanCache
		{
			get { return queryPlanCache; }
		}
	}
}