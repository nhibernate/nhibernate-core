using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
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
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.MultiTenancy;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transaction;
using NHibernate.Type;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;
using HibernateDialect = NHibernate.Dialect.Dialect;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

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
	/// Manages <c>PreparedStatements/DbCommands</c> - how true in NH?
	/// </item>
	/// <item>
	/// Delegates <c>DbConnection</c> management to the <see cref="IConnectionProvider"/>
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
	/// <seealso cref="IConnectionProvider"/>
	/// <seealso cref="ISession"/>
	/// <seealso cref="IQueryTranslator"/>
	/// <seealso cref="IEntityPersister"/>
	/// <seealso cref="ICollectionPersister"/>
	[Serializable]
	public sealed partial class SessionFactoryImpl : ISessionFactoryImplementor, IObjectReference
	{
		#region Default entity not found delegate

		internal class DefaultEntityNotFoundDelegate : IEntityNotFoundDelegate, IEntityNotFoundPropertyDelegate
		{
			#region IEntityNotFoundDelegate Members

			public void HandleEntityNotFound(string entityName, object id)
			{
				throw new ObjectNotFoundException(id, entityName);
			}

			public void HandleEntityNotFound(string entityName, string propertyName, object key)
			{
				throw new ObjectNotFoundByUniqueKeyException(entityName, propertyName, key);
			}

			#endregion
		}

		#endregion

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(SessionFactoryImpl));
		private static readonly IIdentifierGenerator UuidGenerator = new UUIDHexGenerator();

		[NonSerialized]
		private readonly ConcurrentDictionary<string, CacheBase> _allCacheRegions =
			new ConcurrentDictionary<string, CacheBase>();

		[NonSerialized]
		private readonly IDictionary<string, IClassMetadata> classMetadata;

		[NonSerialized]
		private readonly IDictionary<string, ICollectionMetadata> collectionMetadata;
		[NonSerialized]
		private readonly Dictionary<string, ICollectionPersister> collectionPersisters;

		[NonSerialized]
		private readonly IDictionary<string, ISet<string>> collectionRolesByEntityParticipant;
		[NonSerialized]
		private readonly ICurrentSessionContext currentSessionContext;
		[NonSerialized]
		private readonly IEntityNotFoundDelegate entityNotFoundDelegate;
		[NonSerialized]
		private readonly IDictionary<string, IEntityPersister> entityPersisters;

		/// <summary>
		/// NH specific : to avoid the use of entityName for generic implementation
		/// </summary>
		/// <remarks>this is a shortcut.</remarks>
		[NonSerialized]
		private readonly IDictionary<System.Type, string> implementorToEntityName;

		[NonSerialized]
		private readonly EventListeners eventListeners;

		[NonSerialized]
		private readonly Dictionary<string, FilterDefinition> filters;
		[NonSerialized]
		private readonly Dictionary<string, IIdentifierGenerator> identifierGenerators;

		[NonSerialized]
		private readonly Dictionary<string, string> imports;

		[NonSerialized]
		private readonly IInterceptor interceptor;
		private readonly string name;
		[NonSerialized]
		private readonly Dictionary<string, NamedQueryDefinition> namedQueries;

		[NonSerialized]
		private readonly Dictionary<string, NamedSQLQueryDefinition> namedSqlQueries;

		[NonSerialized]
		private readonly IDictionary<string, string> properties;

		[NonSerialized]
		private readonly IQueryCache queryCache;

		[NonSerialized]
		private readonly ConcurrentDictionary<string, Lazy<IQueryCache>> queryCaches;
		[NonSerialized]
		private readonly SchemaExport schemaExport;
		[NonSerialized]
		private readonly Settings settings;

		[NonSerialized]
		private readonly SQLFunctionRegistry sqlFunctionRegistry;
		[NonSerialized]
		private readonly Dictionary<string, ResultSetMappingDefinition> sqlResultSetMappings;
		[NonSerialized]
		private readonly UpdateTimestampsCache updateTimestampsCache;
		[NonSerialized]
		private readonly ConcurrentDictionary<string, string[]> entityNameImplementorsMap = new ConcurrentDictionary<string, string[]>(4 * System.Environment.ProcessorCount, 100);
		private readonly string uuid;

		[NonSerialized]
		private bool disposed;

		[NonSerialized]
		private bool isClosed = false;

		[NonSerialized]
		private QueryPlanCache queryPlanCache;
		[NonSerialized]
		private StatisticsImpl statistics;

		public SessionFactoryImpl(Configuration cfg, IMapping mapping, Settings settings, EventListeners listeners)
		{
			Init();
			log.Info("building session factory");

			properties = new Dictionary<string, string>(cfg.Properties);
			interceptor = cfg.Interceptor;
			this.settings = settings;
			sqlFunctionRegistry = new SQLFunctionRegistry(settings.Dialect, cfg.SqlFunctions);
			eventListeners = listeners;
			filters = new Dictionary<string, FilterDefinition>(cfg.FilterDefinitions);
			if (log.IsDebugEnabled())
			{
				log.Debug("Session factory constructed with filter configurations : {0}", CollectionPrinter.ToString(filters));
			}

			if (log.IsDebugEnabled())
			{
				log.Debug("instantiating session factory with properties: {0}", CollectionPrinter.ToString(properties));
			}

			try
			{
				if (settings.IsKeywordsImportEnabled)
				{
					SchemaMetadataUpdater.Update(this);
				}
				if (settings.IsAutoQuoteEnabled)
				{
					SchemaMetadataUpdater.QuoteTableAndColumns(cfg, Dialect);
				}
			}
			catch (NotSupportedException ex)
			{
				// Ignore if the Dialect does not provide DataBaseSchema
				log.Warn(ex, "Dialect does not provide DataBaseSchema, but keywords import or auto quoting is enabled.");
			}

			#region Serialization info

			name = settings.SessionFactoryName;
			try
			{
				uuid = (string)UuidGenerator.Generate(null, null);
			}
			catch (Exception ex)
			{
				throw new AssertionFailure("Could not generate UUID", ex);
			}

			SessionFactoryObjectFactory.AddInstance(uuid, name, this, properties);

			#endregion

			#region Caches
			settings.CacheProvider.Start(properties);
			#endregion

			#region Generators
			identifierGenerators = new Dictionary<string, IIdentifierGenerator>();
			foreach (PersistentClass model in cfg.ClassMappings)
			{
				if (!model.IsInherited)
				{
					IIdentifierGenerator generator =
						model.Identifier.CreateIdentifierGenerator(settings.Dialect, settings.DefaultCatalogName,
																   settings.DefaultSchemaName, (RootClass)model);

					identifierGenerators[model.EntityName] = generator;
				}
			}
			#endregion

			#region Persisters

			var caches = new Dictionary<Tuple<string, string>, ICacheConcurrencyStrategy>();
			entityPersisters = new Dictionary<string, IEntityPersister>();
			implementorToEntityName = new Dictionary<System.Type, string>();

			Dictionary<string, IClassMetadata> classMeta = new Dictionary<string, IClassMetadata>();

			foreach (PersistentClass model in cfg.ClassMappings)
			{
				model.PrepareTemporaryTables(mapping, settings.Dialect);
				var cache = GetCacheConcurrencyStrategy(
					model.RootClazz.CacheRegionName,
					model.CacheConcurrencyStrategy,
					model.IsMutable,
					caches);
				var cp = PersisterFactory.CreateClassPersister(model, cache, this, mapping);
				entityPersisters[model.EntityName] = cp;
				classMeta[model.EntityName] = cp.ClassMetadata;

				if (model.HasPocoRepresentation)
				{
					implementorToEntityName[model.MappedClass] = model.EntityName;
				}
			}
			classMetadata = new ReadOnlyDictionary<string, IClassMetadata>(classMeta);

			Dictionary<string, ISet<string>> tmpEntityToCollectionRoleMap = new Dictionary<string, ISet<string>>();
			collectionPersisters = new Dictionary<string, ICollectionPersister>();
			foreach (Mapping.Collection model in cfg.CollectionMappings)
			{
				var cache = GetCacheConcurrencyStrategy(
					model.CacheRegionName,
					model.CacheConcurrencyStrategy,
					model.Owner.IsMutable,
					caches);
				var persister = PersisterFactory.CreateCollectionPersister(model, cache, this);
				collectionPersisters[model.Role] = persister;
				IType indexType = persister.IndexType;
				if (indexType != null && indexType.IsAssociationType && !indexType.IsAnyType)
				{
					string entityName = ((IAssociationType)indexType).GetAssociatedEntityName(this);
					ISet<string> roles;
					if (!tmpEntityToCollectionRoleMap.TryGetValue(entityName, out roles))
					{
						roles = new HashSet<string>();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
				IType elementType = persister.ElementType;
				if (elementType.IsAssociationType && !elementType.IsAnyType)
				{
					string entityName = ((IAssociationType)elementType).GetAssociatedEntityName(this);
					ISet<string> roles;
					if (!tmpEntityToCollectionRoleMap.TryGetValue(entityName, out roles))
					{
						roles = new HashSet<string>();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
			}
			Dictionary<string, ICollectionMetadata> tmpcollectionMetadata = new Dictionary<string, ICollectionMetadata>(collectionPersisters.Count);
			foreach (KeyValuePair<string, ICollectionPersister> collectionPersister in collectionPersisters)
			{
				tmpcollectionMetadata.Add(collectionPersister.Key, collectionPersister.Value.CollectionMetadata);
			}
			collectionMetadata = new ReadOnlyDictionary<string, ICollectionMetadata>(tmpcollectionMetadata);
			collectionRolesByEntityParticipant = new ReadOnlyDictionary<string, ISet<string>>(tmpEntityToCollectionRoleMap);
			#endregion

			#region Named Queries
			namedQueries = new Dictionary<string, NamedQueryDefinition>(cfg.NamedQueries);
			namedSqlQueries = new Dictionary<string, NamedSQLQueryDefinition>(cfg.NamedSQLQueries);
			sqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>(cfg.SqlResultSetMappings);
			#endregion

			imports = new Dictionary<string, string>(cfg.Imports);

			#region after *all* persisters and named queries are registered
			foreach (IEntityPersister persister in entityPersisters.Values)
			{
				persister.PostInstantiate();
			}
			foreach (ICollectionPersister persister in collectionPersisters.Values)
			{
				persister.PostInstantiate();
			}
			#endregion

			log.Debug("Instantiated session factory");

			#region Schema management
			if (settings.IsAutoCreateSchema)
			{
				new SchemaExport(cfg).Create(false, true);
			}

			if (settings.IsAutoUpdateSchema)
			{
				var schemaUpdate = new SchemaUpdate(cfg);
				schemaUpdate.Execute(false, true);
				if (settings.ThrowOnSchemaUpdate)
				{
					if (schemaUpdate.Exceptions.Any())
					{
						throw new AggregateHibernateException(
							"Schema update has failed, see inner exceptions for details", schemaUpdate.Exceptions);
					}
				}
			}
			
			if (settings.IsAutoValidateSchema)
			{
				new SchemaValidator(cfg, settings).Validate();
			}
			
			if (settings.IsAutoDropSchema)
			{
				schemaExport = new SchemaExport(cfg);
			}
			#endregion

			#region Obtaining TransactionManager
			// not ported yet
			#endregion

			currentSessionContext = BuildCurrentSessionContext();

			if (settings.IsQueryCacheEnabled)
			{
				var updateTimestampsCacheName = nameof(Cache.UpdateTimestampsCache);
				updateTimestampsCache = new UpdateTimestampsCache(GetCache(updateTimestampsCacheName), settings.CacheReadWriteLockFactory.Create());
				var queryCacheName = typeof(StandardQueryCache).FullName;
				queryCache = BuildQueryCache(queryCacheName);
				queryCaches = new ConcurrentDictionary<string, Lazy<IQueryCache>>();
				queryCaches.TryAdd(queryCacheName, new Lazy<IQueryCache>(() => queryCache));
			}
			else
			{
				updateTimestampsCache = null;
				queryCache = null;
				queryCaches = null;
			}

			#region Checking for named queries
			if (settings.IsNamedQueryStartupCheckingEnabled)
			{
				IDictionary<string, HibernateException> errors = CheckNamedQueries();
				if (errors.Count > 0)
				{
					StringBuilder failingQueries = new StringBuilder("Errors in named queries: ");
					foreach (KeyValuePair<string, HibernateException> pair in errors)
					{
						failingQueries.Append('{').Append(pair.Key).Append('}');
						log.Error(pair.Value, "Error in named query: {0}", pair.Key);
					}
					throw new AggregateHibernateException(failingQueries.ToString(), errors.Values);
				}
			}
			#endregion

			Statistics.IsStatisticsEnabled = settings.IsStatisticsEnabled;

			// EntityNotFoundDelegate
			IEntityNotFoundDelegate enfd = cfg.EntityNotFoundDelegate;
			if (enfd == null)
			{
				enfd = new DefaultEntityNotFoundDelegate();
			}
			entityNotFoundDelegate = enfd;
		}

		private IQueryCache BuildQueryCache(string queryCacheName)
		{
			return
				settings.QueryCacheFactory.GetQueryCache(
					updateTimestampsCache,
					properties,
					GetCache(queryCacheName))
				// 6.0 TODO: remove the coalesce once IQueryCacheFactory todos are done
#pragma warning disable 618
				?? settings.QueryCacheFactory.GetQueryCache(
#pragma warning restore 618
					queryCacheName,
					updateTimestampsCache,
					settings,
					properties);
		}

		private ICacheConcurrencyStrategy GetCacheConcurrencyStrategy(
			string cacheRegion,
			string strategy,
			bool isMutable,
			Dictionary<Tuple<string, string>, ICacheConcurrencyStrategy> caches)
		{
			if (strategy == null || !settings.IsSecondLevelCacheEnabled)
				return null;

			var cacheKey = new Tuple<string, string>(cacheRegion, strategy);
			if (caches.TryGetValue(cacheKey, out var cache)) 
				return cache;

			cache = CacheFactory.CreateCache(strategy, GetCache(cacheRegion), settings);
			caches.Add(cacheKey, cache);
			if (isMutable && strategy == CacheFactory.ReadOnly)
				log.Warn("read-only cache configured for mutable: {0}", name);

			return cache;
		}

		public EventListeners EventListeners
		{
			get { return eventListeners; }
		}

		#region IObjectReference Members

		[SecurityCritical]
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

		#region ISessionFactoryImplementor Members

		public ISessionBuilder WithOptions()
		{
			return new SessionBuilderImpl(this);
		}

		public ISession OpenSession()
		{
			return WithOptions().OpenSession();
		}

		// Obsolete since v5
		[Obsolete("Please use WithOptions instead.")]
		public ISession OpenSession(DbConnection connection)
		{
			return WithOptions()
				.Connection(connection)
				.OpenSession();
		}

		// Obsolete since v5
		[Obsolete("Please use WithOptions instead.")]
		public ISession OpenSession(DbConnection connection, IInterceptor sessionLocalInterceptor)
		{
			return WithOptions()
				.Connection(connection)
				.Interceptor(sessionLocalInterceptor)
				.OpenSession();
		}

		// Obsolete since v5
		[Obsolete("Please use WithOptions instead.")]
		public ISession OpenSession(IInterceptor sessionLocalInterceptor)
		{
			return WithOptions()
				.Interceptor(sessionLocalInterceptor)
				.OpenSession();
		}

		// Obsolete since v5
		[Obsolete("Please use WithOptions instead.")]
		public ISession OpenSession(DbConnection connection, bool flushBeforeCompletionEnabled, bool autoCloseSessionEnabled,
			ConnectionReleaseMode connectionReleaseMode)
		{
			return WithOptions()
				.Connection(connection)
				.AutoClose(autoCloseSessionEnabled)
				.ConnectionReleaseMode(connectionReleaseMode)
				.OpenSession();
		}

		public IStatelessSessionBuilder WithStatelessOptions()
		{
			return new StatelessSessionBuilderImpl(this);
		}

		public IStatelessSession OpenStatelessSession()
		{
			return WithStatelessOptions().OpenStatelessSession();
		}

		public IStatelessSession OpenStatelessSession(DbConnection connection)
		{
			return WithStatelessOptions()
				.Connection(connection)
				.OpenStatelessSession();
		}

		public IEntityPersister GetEntityPersister(string entityName)
		{
			IEntityPersister value;
			if (entityPersisters.TryGetValue(entityName, out value) == false)
				throw new MappingException("No persister for: " + entityName);
			return value;
		}

		public IEntityPersister TryGetEntityPersister(string entityName)
		{
			IEntityPersister result;
			entityPersisters.TryGetValue(entityName, out result);
			return result;
		}

		public ICollectionPersister GetCollectionPersister(string role)
		{
			ICollectionPersister value;
			if (collectionPersisters.TryGetValue(role, out value) == false)
				throw new MappingException("Unknown collection role: " + role);
			return value;
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
				queryPlanCache.GetHQLQueryPlan(queryString.ToQueryExpression(), false, CollectionHelper.EmptyDictionary<string, IFilter>()).
					ReturnMetadata.ReturnTypes;
		}

		/// <summary> Get the return aliases of a query</summary>
		public string[] GetReturnAliases(string queryString)
		{
			return
				queryPlanCache.GetHQLQueryPlan(queryString.ToQueryExpression(), false, CollectionHelper.EmptyDictionary<string, IFilter>()).
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
			ICollectionMetadata result;
			collectionMetadata.TryGetValue(roleName, out result);
			return result;
		}

		/// <summary>
		/// Return the names of all persistent (mapped) classes that extend or implement the
		/// given class or interface, accounting for implicit/explicit polymorphism settings
		/// and excluding mapped subclasses/joined-subclasses of other classes in the result.
		/// </summary>
		public string[] GetImplementors(string entityOrClassName)
		{
			string[] knownMap;
			if (entityNameImplementorsMap.TryGetValue(entityOrClassName, out knownMap))
			{
				return knownMap;
			}
			System.Type clazz = null;

			// NH Different implementation for performance: a class without at least a namespace sure can't be found by reflection
			if (entityOrClassName.IndexOf('.') > 0)
			{
				IEntityPersister checkPersister;
				// NH Different implementation: we have better performance checking, first of all, if we know the class
				// and take the System.Type directly from the persister (className have high probability to be entityName at least using Criteria or Linq)
				if (entityPersisters.TryGetValue(entityOrClassName, out checkPersister))
				{
					if (!checkPersister.EntityMetamodel.HasPocoRepresentation)
					{
						// we found the persister but it is a dynamic entity without class
						knownMap = new[] { entityOrClassName };
						entityNameImplementorsMap[entityOrClassName] = knownMap;
						return knownMap;
					}
					// NH : take care with this because we are forcing the Poco EntityMode
					clazz = checkPersister.MappedClass;
				}

				if (clazz == null)
				{
					try
					{
						clazz = ReflectHelper.ClassForFullNameOrNull(entityOrClassName);
					}
					catch (Exception)
					{
						clazz = null;
					}
				}
			}

			if (clazz == null)
			{
				// try to get the class from imported names
				string importedName = GetImportedClassName(entityOrClassName);
				if (importedName != entityOrClassName)
				{
					clazz = System.Type.GetType(importedName, false);
				}
			}

			if (clazz == null)
			{
				knownMap = new[] { entityOrClassName };
				entityNameImplementorsMap[entityOrClassName] = knownMap;
				return knownMap; //for a dynamic-class
			}

			var results = new List<string>();
			foreach (var q in entityPersisters.Values.OfType<IQueryable>())
			{
				string registeredEntityName = q.EntityName;
				// NH: as entity-name we are using the FullName but in HQL we allow just the Name, the class is mapped even when its FullName match the entity-name
				bool isMappedClass = entityOrClassName.Equals(registeredEntityName) || clazz.FullName.Equals(registeredEntityName);
				if (q.IsExplicitPolymorphism)
				{
					if (isMappedClass)
					{
						knownMap = new[] { registeredEntityName };
						entityNameImplementorsMap[entityOrClassName] = knownMap;
						return knownMap; // NOTE EARLY EXIT
					}
				}
				else
				{
					if (isMappedClass)
					{
						results.Add(registeredEntityName);
					}
					else
					{
						if (IsMatchingImplementor(entityOrClassName, clazz, q))
						{
							bool assignableSuperclass;
							if (q.IsInherited)
							{
								System.Type mappedSuperclass = GetEntityPersister(q.MappedSuperclass).MappedClass;
								assignableSuperclass = clazz.IsAssignableFrom(mappedSuperclass);
							}
							else
							{
								assignableSuperclass = false;
							}
							if (!assignableSuperclass)
							{
								results.Add(registeredEntityName);
							}
						}
					}
				}
			}
			knownMap = results.ToArray();
			entityNameImplementorsMap[entityOrClassName] = knownMap;
			return knownMap;
		}

		private static bool IsMatchingImplementor(string entityOrClassName, System.Type entityClass, IQueryable implementor)
		{
			var implementorClass = implementor.MappedClass;
			if (implementorClass == null)
			{
				return false;
			}
			if (entityClass == implementorClass)
			{
				// It is possible to have multiple mappings for the same entity class, but with different entity names.
				// When querying for a specific entity name, we should only return entities for the requested entity name
				// and not return entities for any other entity names that may map to the same entity class.
				bool isEntityName = !entityOrClassName.Equals(entityClass.FullName);
				return !isEntityName || entityOrClassName.Equals(implementor.EntityName);
			}
			return entityClass.IsAssignableFrom(implementorClass);
		}

		public string GetImportedClassName(string className)
		{
			string result;
			if (className != null && imports.TryGetValue(className, out result))
			{
				return result;
			}
			else
			{
				return className;
			}
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
			if (isClosed)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("Already closed");
				}

				return;
			}

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
				foreach (var cache in queryCaches.Values)
				{
					cache.Value.Destroy();
				}
			}

			foreach (var cache in _allCacheRegions.Values)
			{
				cache.Destroy();
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

			eventListeners.DestroyListeners();
		}

		public void Evict(System.Type persistentClass, object id)
		{
			EvictEntity(persistentClass.FullName, id);
		}

		public void Evict(System.Type persistentClass)
		{
			IEntityPersister p = GetEntityPersister(persistentClass.FullName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("evicting second-level cache: {0}", p.EntityName);
				}
				p.Cache.Clear();
			}
		}

		public void Evict(IEnumerable<System.Type> persistentClasses)
		{
			if (persistentClasses == null)
				throw new ArgumentNullException(nameof(persistentClasses));
			EvictEntity(persistentClasses.Select(x => x.FullName));
		}

		public void EvictEntity(string entityName)
		{
			IEntityPersister p = GetEntityPersister(entityName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("evicting second-level cache: {0}", p.EntityName);
				}
				p.Cache.Clear();
			}
		}

		public void EvictEntity(IEnumerable<string> entityNames)
		{
			if (entityNames == null)
				throw new ArgumentNullException(nameof(entityNames));

			foreach (var cacheGroup in entityNames.Select(GetEntityPersister).Where(x => x.HasCache).GroupBy(x => x.Cache))
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("evicting second-level cache for: {0}",
					          string.Join(", ", cacheGroup.Select(p => p.EntityName)));
				}
				cacheGroup.Key.Clear();
			}
		}

		public void EvictEntity(string entityName, object id)
		{
			EvictEntity(entityName, id, null);
		}

		public void EvictEntity(string entityName, object id, string tenantIdentifier)
		{
			IEntityPersister p = GetEntityPersister(entityName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled())
				{
					LogEvict(tenantIdentifier, MessageHelper.InfoString(p, id, this));
				}
				CacheKey cacheKey = GenerateCacheKeyForEvict(id, p.IdentifierType, p.RootEntityName, tenantIdentifier);
				p.Cache.Remove(cacheKey);
			}
		}

		public void EvictCollection(string roleName, object id)
		{
			EvictCollection(roleName, id, null);
		}

		public void EvictCollection(string roleName, object id, string tenantIdentifier)
		{
			ICollectionPersister p = GetCollectionPersister(roleName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled())
				{
					LogEvict(tenantIdentifier, MessageHelper.CollectionInfoString(p, id));
				}

				CacheKey ck = GenerateCacheKeyForEvict(id, p.KeyType, p.Role, tenantIdentifier);
				p.Cache.Remove(ck);
			}
		}

		private CacheKey GenerateCacheKeyForEvict(object id, IType type, string entityOrRoleName, string tenantIdentifier)
		{
			// if there is a session context, use that to generate the key.
			if (CurrentSessionContext != null)
			{
				return CurrentSessionContext
					.CurrentSession()
					.GetSessionImplementation()
					.GenerateCacheKey(id, type, entityOrRoleName);
			}

			if (settings.MultiTenancyStrategy != MultiTenancyStrategy.None && tenantIdentifier == null)
			{
				throw new ArgumentException("Use overload with tenantIdentifier or initialize CurrentSessionContext.");
			}

			return new CacheKey(id, type, entityOrRoleName, this, tenantIdentifier);
		}

		public void EvictCollection(string roleName)
		{
			ICollectionPersister p = GetCollectionPersister(roleName);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("evicting second-level cache: {0}", p.Role);
				}
				p.Cache.Clear();
			}
		}

		public void EvictCollection(IEnumerable<string> roleNames)
		{
			if (roleNames == null)
				throw new ArgumentNullException(nameof(roleNames));

			foreach (var cacheGroup in roleNames.Select(GetCollectionPersister).Where(x => x.HasCache).GroupBy(x => x.Cache))
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("evicting second-level cache for: {0}",
					          string.Join(", ", cacheGroup.Select(p => p.Role)));
				}
				cacheGroup.Key.Clear();
			}
		}

		public IType GetReferencedPropertyType(string className, string propertyName)
		{
			return GetEntityPersister(className).GetPropertyType(propertyName);
		}

		public bool HasNonIdentifierPropertyNamedId(string className)
		{
			return "id".Equals(GetIdentifierPropertyName(className));
		}

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

		// 6.0 TODO: type as CacheBase instead
#pragma warning disable 618
		public IDictionary<string, ICache> GetAllSecondLevelCacheRegions()
#pragma warning restore 618
		{
			return
				_allCacheRegions
					// ToArray creates a moment in time snapshot
					.ToArray()
#pragma warning disable 618
					.ToDictionary(kv => kv.Key, kv => (ICache) kv.Value);
#pragma warning restore 618
		}

		// 6.0 TODO: return CacheBase instead
#pragma warning disable 618
		public ICache GetSecondLevelCacheRegion(string regionName)
#pragma warning restore 618
		{
			_allCacheRegions.TryGetValue(regionName, out var result);
			return result;
		}

		private CacheBase GetCache(string cacheRegion)
		{
			// If run concurrently for the same region and type, this may built many caches for the same region and type.
			// Currently only GetQueryCache may be run concurrently, and its implementation prevents
			// concurrent creation call for the same region, so this will not happen.
			// Otherwise the dictionary will have to be changed for using a lazy, see
			// https://stackoverflow.com/a/31637510/1178314
			cacheRegion = settings.GetFullCacheRegionName(cacheRegion);

			return _allCacheRegions.GetOrAdd(
				cacheRegion,
				cr => CacheFactory.BuildCacheBase(cr, settings, properties));
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

			// The factory may be run concurrently by threads trying to get the same region.
			// But the GetOrAdd will yield the same lazy for all threads, so only one will
			// initialize. https://stackoverflow.com/a/31637510/1178314
			return
				queryCaches
					.GetOrAdd(
						cacheRegion,
						cr => new Lazy<IQueryCache>(() => BuildQueryCache(cr)))
					.Value;
		}

		public void EvictQueries()
		{
			// NH Different implementation
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
			if (string.IsNullOrEmpty(cacheRegion))
			{
				throw new ArgumentNullException("cacheRegion", "use the zero-argument form to evict the default query cache");
			}
			else
			{
				if (settings.IsQueryCacheEnabled)
				{
					if (queryCaches.TryGetValue(cacheRegion, out var currentQueryCache))
					{
						currentQueryCache.Value.Clear();
					}
				}
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
			FilterDefinition value;
			if (filters.TryGetValue(filterName, out value) == false)
				throw new HibernateException("No such filter configured [" + filterName + "]");
			return value;
		}

		public ICollection<string> DefinedFilterNames
		{
			get { return filters.Keys; }
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

		#endregion

		private static void LogEvict(string tenantIdentifier, string infoString)
		{
			if (string.IsNullOrEmpty(tenantIdentifier))
			{
				log.Debug("evicting second-level cache: {0}", infoString);
				return;
			}

			log.Debug("evicting second-level cache for tenant '{1}': {0}", infoString, tenantIdentifier);
		}

		private void Init()
		{
			statistics = new StatisticsImpl(this);
			queryPlanCache = new QueryPlanCache(this);
		}

		private IDictionary<string, HibernateException> CheckNamedQueries()
		{
			IDictionary<string, HibernateException> errors = new Dictionary<string, HibernateException>();

			// Check named HQL queries
			log.Debug("Checking {0} named HQL queries", namedQueries.Count);
			foreach (var entry in namedQueries)
			{
				string queryName = entry.Key;
				NamedQueryDefinition qd = entry.Value;
				// this will throw an error if there's something wrong.
				try
				{
					log.Debug("Checking named query: {0}", queryName);
					//TODO: BUG! this currently fails for named queries for non-POJO entities
					queryPlanCache.GetHQLQueryPlan(qd.QueryString.ToQueryExpression(), false, CollectionHelper.EmptyDictionary<string, IFilter>());
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

			log.Debug("Checking {0} named SQL queries", namedSqlQueries.Count);
			foreach (KeyValuePair<string, NamedSQLQueryDefinition> entry in namedSqlQueries)
			{
				string queryName = entry.Key;
				NamedSQLQueryDefinition qd = entry.Value;
				// this will throw an error if there's something wrong.
				try
				{
					log.Debug("Checking named SQL query: {0}", queryName);
					// TODO : would be really nice to cache the spec on the query-def so as to not have to re-calc the hash;
					// currently not doable though because of the resultset-ref stuff...
					NativeSQLQuerySpecification spec;
					if (qd.ResultSetRef != null)
					{
						ResultSetMappingDefinition definition;
						if (!sqlResultSetMappings.TryGetValue(qd.ResultSetRef, out definition))
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

		private ICurrentSessionContext BuildCurrentSessionContext()
		{
			string impl = PropertiesHelper.GetString(Environment.CurrentSessionContextClass, properties, null);

			switch (impl)
			{
				case null:
					return null;
				case "async_local":
					return new AsyncLocalSessionContext(this);
				case "call":
					return new CallSessionContext(this);
				case "thread_static":
					return new ThreadStaticSessionContext(this);
				case "web":
					return new WebSessionContext(this);
				case "wcf_operation":
#if NETFX
					return new WcfOperationSessionContext(this);
#else
					// There is no support of WCF Server under .Net Core, so it makes little sense to provide
					// a WCF OperationContext for it. Since it adds additional heavy dependencies, it has been
					// considered not desirable to provide it for .Net Standard. (It could be useful in case some
					// WCF server becames available in another frameworks or if a .Net Framework application
					// consumes the .Net Standard distribution of NHibernate instead of the .Net Framework one.)
					// See https://github.com/dotnet/wcf/issues/1200 and #1842
					throw new PlatformNotSupportedException(
						"WcfOperationSessionContext is not supported for the current framework");
#endif
			}

			try
			{
				var implClass = ReflectHelper.ClassForName(impl);
				var constructor = implClass.GetConstructor(new [] { typeof(ISessionFactoryImplementor) });
				ICurrentSessionContext context;
				if (constructor != null)
				{
					context = (ICurrentSessionContext) constructor.Invoke(new object[] { this });
				}
				else
				{
					context = (ICurrentSessionContext) Environment.ObjectsFactory.CreateInstance(implClass);
				}
				if (context is ISessionFactoryAwareCurrentSessionContext sessionFactoryAwareContext)
				{
					sessionFactoryAwareContext.SetFactory(this);
				}
				return context;
			}
			catch (Exception e)
			{
				log.Error(e, "Unable to construct current session context [{0}]", impl);
				return null;
			}
		}

		#region NHibernate specific

		public string TryGetGuessEntityName(System.Type implementor)
		{
			string result;
			implementorToEntityName.TryGetValue(implementor, out result);
			return result;
		}

		public string Name
		{
			get { return name; }
		}

		public string Uuid
		{
			get { return uuid; }
		}

		#endregion

		// NH specific: implementing return type covariance with interface is a mess in .Net.
		internal class SessionBuilderImpl : SessionBuilderImpl<ISessionBuilder>, ISessionBuilder
		{
			public SessionBuilderImpl(SessionFactoryImpl sessionFactory) : base(sessionFactory)
			{
				SetSelf(this);
			}
		}

		internal class SessionBuilderImpl<T> : ISessionBuilder<T>, ISessionCreationOptions, ISessionCreationOptionsWithMultiTenancy where T : ISessionBuilder<T>
		{
			// NH specific: implementing return type covariance with interface is a mess in .Net.
			private T _this;
			private static readonly INHibernateLogger _log = NHibernateLogger.For(typeof(SessionBuilderImpl<T>));

			private readonly SessionFactoryImpl _sessionFactory;
			private IInterceptor _interceptor;
			private DbConnection _connection;
			// Todo: port PhysicalConnectionHandlingMode
			private ConnectionReleaseMode _connectionReleaseMode;
			private FlushMode _flushMode;
			private bool _autoClose;
			private bool _autoJoinTransaction;

			public SessionBuilderImpl(SessionFactoryImpl sessionFactory)
			{
				_sessionFactory = sessionFactory;

				// set up default builder values...
				_connectionReleaseMode = sessionFactory.Settings.ConnectionReleaseMode;
				_autoClose = sessionFactory.Settings.IsAutoCloseSessionEnabled;
				_autoJoinTransaction = sessionFactory.Settings.AutoJoinTransaction;
				// NH different implementation: not using Settings.IsFlushBeforeCompletionEnabled
				_flushMode = sessionFactory.Settings.DefaultFlushMode;
			}

			protected void SetSelf(T self)
			{
				_this = self;
			}

			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// SessionCreationOptions

			public virtual FlushMode InitialSessionFlushMode => _flushMode;

			public virtual bool ShouldAutoClose => _autoClose;

			public virtual bool ShouldAutoJoinTransaction => _autoJoinTransaction;

			public DbConnection UserSuppliedConnection => _connection;

			// NH different implementation: Hibernate here ignore EmptyInterceptor.Instance too, resulting
			// in the "NoInterceptor" being unable to override a session factory interceptor.
			public virtual IInterceptor SessionInterceptor => _interceptor ?? _sessionFactory.Interceptor;

			public virtual ConnectionReleaseMode SessionConnectionReleaseMode => _connectionReleaseMode;

			// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// SessionBuilder

			public virtual ISession OpenSession()
			{
				_log.Debug("Opening Hibernate Session.");
				var session = new SessionImpl(_sessionFactory, this);
				if (_interceptor != null)
				{
					// NH specific feature
					// _interceptor may be the shared accros threads EmptyInterceptor.Instance, but that is
					// not an issue, SetSession is no-op on it.
					_interceptor.SetSession(session);
				}

				return session;
			}

			public virtual T Interceptor(IInterceptor interceptor)
			{
				// NH different implementation: Hibernate accepts null.
				_interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
				return _this;
			}

			public virtual T NoInterceptor()
			{
				_interceptor = EmptyInterceptor.Instance;
				return _this;
			}

			public virtual T Connection(DbConnection connection)
			{
				_connection = connection;
				return _this;
			}

			public virtual T ConnectionReleaseMode(ConnectionReleaseMode connectionReleaseMode)
			{
				_connectionReleaseMode = connectionReleaseMode;
				return _this;
			}

			public virtual T AutoClose(bool autoClose)
			{
				_autoClose = autoClose;
				return _this;
			}

			public virtual T AutoJoinTransaction(bool autoJoinTransaction)
			{
				_autoJoinTransaction = autoJoinTransaction;
				return _this;
			}

			public virtual T FlushMode(FlushMode flushMode)
			{
				_flushMode = flushMode;
				return _this;
			}

			public TenantConfiguration TenantConfiguration
			{
				get;
				//TODO 6.0: Make protected
				set;
			}
		}

		// NH specific: implementing return type covariance with interface is a mess in .Net.
		internal class StatelessSessionBuilderImpl : StatelessSessionBuilderImpl<IStatelessSessionBuilder>
		{
			public StatelessSessionBuilderImpl(SessionFactoryImpl sessionFactory) : base(sessionFactory)
			{
				SetSelf(this);
			}
		}

		internal class StatelessSessionBuilderImpl<T> : IStatelessSessionBuilder, ISessionCreationOptionsWithMultiTenancy, ISessionCreationOptions where T : IStatelessSessionBuilder
		{
			// NH specific: implementing return type covariance with interface is a mess in .Net.
			private T _this;
			private readonly SessionFactoryImpl _sessionFactory;

			public StatelessSessionBuilderImpl(SessionFactoryImpl sessionFactory)
			{
				_sessionFactory = sessionFactory;
			}

			protected void SetSelf(T self)
			{
				_this = self;
			}

			public virtual IStatelessSession OpenStatelessSession() => new StatelessSessionImpl(_sessionFactory, this);

			public virtual IStatelessSessionBuilder Connection(DbConnection connection)
			{
				UserSuppliedConnection = connection;
				return _this;
			}

			public IStatelessSessionBuilder AutoJoinTransaction(bool autoJoinTransaction)
			{
				ShouldAutoJoinTransaction = autoJoinTransaction;
				return _this;
			}

			public FlushMode InitialSessionFlushMode => FlushMode.Always;

			public bool ShouldAutoClose => false;

			public bool ShouldAutoJoinTransaction { get; private set; } = true;

			public DbConnection UserSuppliedConnection { get; private set; }

			public IInterceptor SessionInterceptor => EmptyInterceptor.Instance;

			public ConnectionReleaseMode SessionConnectionReleaseMode => ConnectionReleaseMode.AfterTransaction;

			public TenantConfiguration TenantConfiguration
			{
				get;
				//TODO 6.0: Make protected
				set;
			}
		}
	}
}
