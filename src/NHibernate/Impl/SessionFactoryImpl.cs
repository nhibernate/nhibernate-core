using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Iesi.Collections;
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
using NHibernate.Hql;
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

		[NonSerialized]
		private readonly IDictionary classPersisters;

		[NonSerialized]
		private readonly IDictionary classPersistersByName;

		[NonSerialized]
		private readonly IDictionary classMetadata;

		[NonSerialized] 
		private readonly IDictionary collectionPersisters;

		[NonSerialized]
		private readonly IDictionary collectionMetadata;

		[NonSerialized]
		private readonly IDictionary collectionRolesByEntityParticipant;

		[NonSerialized]
		private readonly IDictionary identifierGenerators;

		[NonSerialized]
		private readonly IDictionary<string, NamedQueryDefinition> namedQueries;

		[NonSerialized]
		private readonly IDictionary<string, NamedSQLQueryDefinition> namedSqlQueries;

		[NonSerialized]
		private readonly IDictionary<string, ResultSetMappingDefinition> sqlResultSetMappings;

		[NonSerialized]
		private readonly IDictionary<string, FilterDefinition> filters;

		[NonSerialized]
		private readonly IDictionary<string, string> imports;

		// templates are related to XmlDatabinder - nothing like that yet 
		// in NHibernate.
		//[NonSerialized] private Templates templates;

		[NonSerialized]
		private readonly IInterceptor interceptor;

		[NonSerialized]
		private readonly Settings settings;

		[NonSerialized]
		private readonly IDictionary<string, string> properties;

		[NonSerialized]
		private readonly SchemaExport schemaExport;

		[NonSerialized]
		private readonly IQueryCache queryCache;

		[NonSerialized]
		private readonly UpdateTimestampsCache updateTimestampsCache;

		[NonSerialized]
		private readonly IDictionary queryCaches;

		[NonSerialized]
		private readonly IDictionary allCacheRegions = new Hashtable();

		[NonSerialized]
		private readonly EventListeners eventListeners;

		[NonSerialized]
		private readonly SQLFunctionRegistry sqlFunctionRegistry;

		[NonSerialized]
		private readonly IEntityNotFoundDelegate entityNotFoundDelegate;

		[NonSerialized]
		private StatisticsImpl statistics;

		private QueryPlanCache queryPlanCache;

		private static readonly IIdentifierGenerator UuidGenerator = new UUIDHexGenerator();

		private static readonly ILog log = LogManager.GetLogger(typeof(SessionFactoryImpl));

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
				log.Debug("instantiating session factory with properties: "
				          + CollectionPrinter.ToString(properties));
			}

			settings.CacheProvider.Start(properties);

			// Generators:
			identifierGenerators = new Hashtable();
			foreach (PersistentClass model in cfg.ClassMappings)
			{
				if (!model.IsInherited)
				{
					IIdentifierGenerator generator =
						model.Identifier.CreateIdentifierGenerator(settings.Dialect, settings.DefaultCatalogName,
						                                           settings.DefaultSchemaName, (RootClass) model);

					identifierGenerators[model.MappedClass] = generator;
				}
			}


			// Persisters:

			IDictionary caches = new Hashtable();
			classPersisters = new Hashtable();
			classPersistersByName = new Hashtable();
			IDictionary classMeta = new Hashtable();

			foreach (PersistentClass model in cfg.ClassMappings)
			{
				string cacheRegion = model.RootClazz.CacheRegionName;
				ICacheConcurrencyStrategy cache = (ICacheConcurrencyStrategy) caches[cacheRegion];
				if (cache == null)
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
				classPersisters[model.MappedClass] = cp;

				// Adds the "Namespace.ClassName" (FullClassname) as a lookup to get to the Persiter.
				// Most of the internals of NHibernate use this method to get to the Persister since
				// Model.Name is used in so many places.  It would be nice to fix it up to be Model.TypeName
				// instead of just FullClassname
				classPersistersByName[model.EntityName] = cp;

				// Add in the AssemblyQualifiedName (includes version) as a lookup to get to the Persister.  
				// In HQL the Imports are used to get from the Classname to the Persister.  The
				// Imports provide the ability to jump from the Classname to the AssemblyQualifiedName.
				classPersistersByName[model.MappedClass.AssemblyQualifiedName] = cp;

				classMeta[model.MappedClass] = cp.ClassMetadata;
			}
			classMetadata = new Hashtable(classMeta);
			IDictionary tmpEntityToCollectionRoleMap= new Hashtable();
			collectionPersisters = new Hashtable();
			foreach (Mapping.Collection map in cfg.CollectionMappings)
			{
				ICacheConcurrencyStrategy cache =
					CacheFactory.CreateCache(map.CacheConcurrencyStrategy, map.CacheRegionName, map.Owner.IsMutable, settings,
					                         properties);
				if (cache != null)
					allCacheRegions[cache.RegionName] = cache.Cache;

				collectionPersisters[map.Role] =
					PersisterFactory.CreateCollectionPersister(cfg, map, cache, this).CollectionMetadata;
				ICollectionPersister persister = collectionPersisters[map.Role] as ICollectionPersister;
				IType indexType = persister.IndexType;
				if (indexType != null && indexType.IsAssociationType && !indexType.IsAnyType)
				{
					string entityName = ((IAssociationType)indexType).GetAssociatedEntityName(this);
					ISet roles = tmpEntityToCollectionRoleMap[entityName] as ISet;
					if (roles == null)
					{
						roles = new HashedSet();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
				IType elementType = persister.ElementType;
				if (elementType.IsAssociationType && !elementType.IsAnyType)
				{
					string entityName = ((IAssociationType)elementType).GetAssociatedEntityName(this);
					ISet roles = tmpEntityToCollectionRoleMap[entityName] as ISet;
					if (roles == null)
					{
						roles = new HashedSet();
						tmpEntityToCollectionRoleMap[entityName] = roles;
					}
					roles.Add(persister.Role);
				}
			}
			collectionMetadata = new Hashtable(collectionPersisters);
			collectionRolesByEntityParticipant = new Hashtable(tmpEntityToCollectionRoleMap);
			//TODO:
			// For databinding:
			//templates = XMLDatabinder.GetOutputStyleSheetTemplates( properties );


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

			// Named queries:
			// TODO: precompile and cache named queries

			namedQueries = new Dictionary<string, NamedQueryDefinition>(cfg.NamedQueries);
			namedSqlQueries = new Dictionary<string, NamedSQLQueryDefinition>(cfg.NamedSQLQueries);
			sqlResultSetMappings = new Dictionary<string, ResultSetMappingDefinition>(cfg.SqlResultSetMappings);
			filters = new Dictionary<string, FilterDefinition>(cfg.FilterDefinitions);

			imports = new Dictionary<string, string>(cfg.Imports);

			// after *all* persisters and named queries are registered
			foreach (IEntityPersister persister in classPersisters.Values)
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
				queryCache = settings.QueryCacheFactory
					.GetQueryCache(null, updateTimestampsCache, settings, properties);
				queryCaches = Hashtable.Synchronized(new Hashtable());
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

		[NonSerialized]
		private readonly object[] strongRefs = new object[MaxStrongRefCount];

		[NonSerialized]
		private int strongRefIndex = 0;

		// both keys and values may be soft since value keeps a hard ref to the key (and there is a hard ref to MRU values)
		[NonSerialized]
		private readonly IDictionary softQueryCache = new WeakHashtable();

		[NonSerialized]
		private readonly ICurrentSessionContext currentSessionContext;

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
				return _hashCode == obj._hashCode &&
				       Query.Equals(obj.Query) &&
				       Scalar == obj.Scalar &&
				       CollectionHelper.SetEquals(FilterNames, obj.FilterNames);
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}

			#endregion
		}

		/// <summary>
		/// A class that can be used as a Key in a Hashtable for 
		/// a Query Cache.
		/// </summary>
		private class FilterCacheKey
		{
			private readonly string _role;
			private readonly string _query;
			private readonly bool _scalar;

			internal FilterCacheKey(string role, string query, bool scalar)
			{
				_role = role;
				_query = query;
				_scalar = scalar;
			}

			public string Role
			{
				get { return _role; }
			}

			public string Query
			{
				get { return _query; }
			}

			public bool Scalar
			{
				get { return _scalar; }
			}

			#region System.Object Members

			public override bool Equals(object obj)
			{
				FilterCacheKey other = obj as FilterCacheKey;
				if (other == null)
				{
					return false;
				}

				return Equals(other);
			}

			public bool Equals(FilterCacheKey obj)
			{
				return Role.Equals(obj.Role) && Query.Equals(obj.Query) && Scalar == obj.Scalar;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return Role.GetHashCode() + Query.GetHashCode() + Scalar.GetHashCode();
				}
			}

			#endregion
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		private object Get(object key)
		{
			object result = softQueryCache[key];
			if (result != null)
			{
				strongRefs[++strongRefIndex % MaxStrongRefCount] = result;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Put(object key, object value)
		{
			softQueryCache[key] = value;
			strongRefs[++strongRefIndex % MaxStrongRefCount] = value;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private IQueryTranslator[] CreateQueryTranslators(string hql, string[] concreteQueryStrings, QueryCacheKey cacheKey,
																											IDictionary<string, IFilter> enabledFilters)
		{
			int length = concreteQueryStrings.Length;
			IQueryTranslator[] queries = new IQueryTranslator[length];
			for (int i = 0; i < length; i++)
			{
				queries[i] = settings.QueryTranslatorFactory.CreateQueryTranslator(hql, concreteQueryStrings[i], enabledFilters, this);
			}
			Put(cacheKey, queries);
			return queries;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private IFilterTranslator CreateFilterTranslator(string hql, string filterString, FilterCacheKey cacheKey, IDictionary<string, IFilter> enabledFilters)
		{
			IFilterTranslator filter =
				settings.QueryTranslatorFactory.CreateFilterTranslator(hql, filterString, enabledFilters, this);
			Put(cacheKey, filter);
			return filter;
		}

		public IQueryTranslator[] GetQuery(string queryString, bool shallow, IDictionary<string, IFilter> enabledFilters)
		{
			QueryCacheKey cacheKey = new QueryCacheKey(queryString, shallow, enabledFilters);

			// have to be careful to ensure that if the JVM does out-of-order execution
			// then another thread can't get an uncompiled QueryTranslator from the cache
			// we also have to be very careful to ensure that other threads can perform
			// compiled queries while another query is being compiled

			IQueryTranslator[] queries = (IQueryTranslator[]) Get(cacheKey);

			if (queries == null)
			{
				// a query that names an interface or unmapped class in the from clause
				// is actually executed as multiple queries
				string[] concreteQueryStrings = QuerySplitter.ConcreteQueries(queryString, this);
				queries = CreateQueryTranslators(queryString, concreteQueryStrings, cacheKey, enabledFilters);
			}

			foreach (IQueryTranslator q in queries)
			{
				q.Compile(settings.QuerySubstitutions, shallow);
			}
			// see comment above. note that QueryTranslator.compile() is synchronized

			return queries;
		}

		public IFilterTranslator GetFilter(string filterString, string collectionRole, bool scalar, IDictionary<string, IFilter> enabledFilters)
		{
			FilterCacheKey cacheKey = new FilterCacheKey(collectionRole, filterString, scalar);

			IFilterTranslator filter = (IFilterTranslator) Get(cacheKey);
			if (filter == null)
			{
				filter = CreateFilterTranslator(filterString, filterString, cacheKey, enabledFilters);
			}

			filter.Compile(collectionRole, settings.QuerySubstitutions, scalar);
			return filter;
		}

		private ISession OpenSession(IDbConnection connection, long timestamp, IInterceptor interceptor,
									 ConnectionReleaseMode connectionReleaseMode)
		{
			SessionImpl sessionImpl = new SessionImpl(connection, this, timestamp, interceptor, connectionReleaseMode);
			bool isSessionScopedInterceptor = this.interceptor!=interceptor;
			if(isSessionScopedInterceptor)
				interceptor.SetSession(sessionImpl);
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

		public ISession OpenSession(IDbConnection connection, ConnectionReleaseMode connectionReleaseMode)
		{
			return OpenSession(connection, Timestamper.Next(), interceptor, connectionReleaseMode);
		}

		public ISession OpenSession(
			IDbConnection connection,
			bool flushBeforeCompletionEnabled,
	        bool autoCloseSessionEnabled,
	        ConnectionReleaseMode connectionReleaseMode) 
		{
			return new SessionImpl(
					connection,
					this,
					true,
					settings.CacheProvider.NextTimestamp(),
					interceptor,
					settings.DefaultEntityMode,
					flushBeforeCompletionEnabled,
					autoCloseSessionEnabled,
					connectionReleaseMode);
		}

		public IEntityPersister GetEntityPersister(string entityName)
		{
			return GetEntityPersister(entityName, true);
		}

		public IEntityPersister GetEntityPersister(string entityName, bool throwIfNotFound)
		{
			// TODO: H2.1 has the code below, an equivalent for .NET would be useful
			//if( className.StartsWith( "[" ) )
			//{
			//	throw new MappingException( "No persister for array result, likely a broken query" );
			//}

			IEntityPersister result = classPersistersByName[entityName] as IEntityPersister;
			if (result == null && throwIfNotFound)
			{
				throw new MappingException("No persister for: " + entityName);
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="theClass"></param>
		/// <returns></returns>
		public IEntityPersister GetEntityPersister(System.Type theClass)
		{
			IEntityPersister result = classPersisters[theClass] as IEntityPersister;
			if (result == null)
			{
				throw new MappingException("Unknown entity class: " + theClass.FullName);
			}
			return result;
		}

		public ICollectionPersister GetCollectionPersister(string role)
		{
			ICollectionPersister result = collectionPersisters[role] as ICollectionPersister;
			if (result == null)
			{
				throw new MappingException("Unknown collection role: " + role);
			}
			return result;
		}

		public ISet GetCollectionRolesByEntityParticipant(string entityName)
		{
			return collectionRolesByEntityParticipant[entityName] as ISet;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IDatabinder OpenDatabinder()
		{
			throw new NotImplementedException("Have not coded Databinder yet.");
			//if( templates == null )
			//{
			//	throw new HibernateException(
			//		"No output stylesheet configured. Use the property output_stylesheet and ensure xalan.jar is in classpath"
			//	);
			//}
			//try
			//{
			//	return new XMLDatabinder( this, templates.NewTransformer() );
			//}
			//catch( Exception e )
			//{
			//	log.Error( "Could not open Databinder", e );
			//	throw new HibernateException( "Could not open Databinder", e );
			//}
		}

		/// <summary></summary>
		public HibernateDialect Dialect
		{
			get { return settings.Dialect; }
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

		/// <summary></summary>
		public bool IsScrollableResultSetsEnabled
		{
			get { return settings.IsScrollableResultSetsEnabled; }
		}

		/// <summary></summary>
		public bool IsGetGeneratedKeysEnabled
		{
			get { return settings.IsGetGeneratedKeysEnabled; }
		}

		/// <summary></summary>
		public bool IsOuterJoinedFetchEnabled
		{
			get { return settings.IsOuterJoinFetchEnabled; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
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
			return queryPlanCache.GetHQLQueryPlan(queryString, false, new CollectionHelper.EmptyMapClass<string, IFilter>()).ReturnMetadata.ReturnTypes;
		}

		/// <summary> Get the return aliases of a query</summary>
		public string[] GetReturnAliases(string queryString)
		{
			return queryPlanCache.GetHQLQueryPlan(queryString, false, new CollectionHelper.EmptyMapClass<string, IFilter>()).ReturnMetadata.ReturnAliases;
		}

		/// <summary></summary>
		public string DefaultSchema
		{
			get { return settings.DefaultSchemaName; }
		}

		public IClassMetadata GetClassMetadata(System.Type persistentClass)
		{
			return GetEntityPersister(persistentClass).ClassMetadata;
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
			System.Type clazz;
			try
			{
				clazz = ReflectHelper.ClassForFullName(className);
			}
			catch (Exception)
			{
				return new string[] { className }; //for a dynamic-class
			}

			ArrayList results = new ArrayList();
			foreach (IEntityPersister p in classPersisters.Values)
			{
				if (p is IQueryable)
				{
					IQueryable q = (IQueryable) p;
					string testClassName = q.EntityName;
					bool isMappedClass = className.Equals(testClassName);
					if (q.IsExplicitPolymorphism)
					{
						if (isMappedClass)
						{
							return new string[] {testClassName};
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
			return (string[]) results.ToArray(typeof(string));
		}

		public string GetImportedClassName(string className)
		{
			if (imports.ContainsKey(className))
				return imports[className];
			else
				return className;
		}

		/// <summary></summary>
		public IDictionary GetAllClassMetadata()
		{
			return classMetadata;
		}

		/// <summary></summary>
		public IDictionary GetAllCollectionMetadata()
		{
			return collectionMetadata;
		}

		private bool disposed;
	  private readonly IDictionary items = new Hashtable();

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

			foreach (IEntityPersister p in classPersisters.Values)
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

			if (IsQueryCacheEnabled)
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
			IEntityPersister p = GetEntityPersister(persistentClass);
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
			IEntityPersister p = GetEntityPersister(persistentClass);
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

		/// <summary></summary>
		public int MaximumFetchDepth
		{
			get { return settings.MaximumFetchDepth; }
		}

		public IType GetReferencedPropertyType(string className, string propertyName)
		{
			return GetEntityPersister(className).GetPropertyType(propertyName);
		}

		public bool HasNonIdentifierPropertyNamedId(string className)
		{
			return "id".Equals(GetIdentifierPropertyName(className));
		}

		/// <summary></summary>
		public IConnectionProvider ConnectionProvider
		{
			get { return settings.ConnectionProvider; }
		}

		public UpdateTimestampsCache UpdateTimestampsCache
		{
			get { return updateTimestampsCache; }
		}

		public IDictionary GetAllSecondLevelCacheRegions()
		{
			lock (allCacheRegions.SyncRoot)
			{
				return new Hashtable(allCacheRegions);
			}
		}

		public ICache GetSecondLevelCacheRegion(string regionName)
		{
			lock (allCacheRegions.SyncRoot)
			{
				return (ICache)allCacheRegions[regionName];
			}
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

			IQueryCache currentQueryCache = (IQueryCache) queryCaches[cacheRegion];
			if (currentQueryCache == null)
			{
				currentQueryCache = settings.QueryCacheFactory.GetQueryCache(
					cacheRegion,
					updateTimestampsCache,
					settings,
					properties
					);
				queryCaches[cacheRegion] = currentQueryCache;
			}
			return currentQueryCache;
		}

		public bool IsQueryCacheEnabled
		{
			get { return settings.IsQueryCacheEnabled; }
		}

		// TODO: isBatchVersionedData()
		// TODO: isWrapDataReadersEnabled()

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
			else if (queryCaches != null)
			{
				IQueryCache currentQueryCache = (IQueryCache) queryCaches[cacheRegion];
				if (currentQueryCache != null)
				{
					currentQueryCache.Clear();
				}
			}
		}

		/*
		// TODO: a better way to normalised the NamedSQLQUery aspect
		internal class InternalNamedSQLQuery
		{
			private readonly string query;
			private readonly string[ ] returnAliases;
			private readonly System.Type[ ] returnClasses;
			private readonly IList querySpaces;

			public InternalNamedSQLQuery( string query, string[ ] aliases, System.Type[ ] clazz, IList querySpaces )
			{
				this.returnClasses = clazz;
				this.returnAliases = aliases;
				this.query = query;
				this.querySpaces = querySpaces;
			}

			public string[ ] ReturnAliases
			{
				get { return returnAliases; }
			}

			public System.Type[ ] ReturnClasses
			{
				get { return returnClasses; }
			}

			public string QueryString
			{
				get { return query; }
			}

			public ICollection QuerySpaces
			{
				get { return querySpaces; }
			}
		}
		*/

		/// <summary></summary>
		public IsolationLevel Isolation
		{
			get { return settings.IsolationLevel; }
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

		public IIdentifierGenerator GetIdentifierGenerator(System.Type rootClass)
		{
			return (IIdentifierGenerator) identifierGenerators[rootClass];
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

	    public IDictionary Items
	    {
	        get { return items; }
	    }

	    public ISession GetCurrentSession()
		{
			if (currentSessionContext == null)
			{
				throw new HibernateException("No CurrentSessionContext configured (set the property " +
				                             Environment.CurrentSessionContextClass + ")!");
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
