using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using log4net;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transaction;
using NHibernate.Type;
using NHibernate.Util;

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
	internal class SessionFactoryImpl : ISessionFactory, ISessionFactoryImplementor, IObjectReference
	{
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
		private readonly IDictionary identifierGenerators;

		[NonSerialized]
		private readonly IDictionary namedQueries;

		[NonSerialized]
		private readonly IDictionary namedSqlQueries;

		[NonSerialized]
		private readonly IDictionary sqlResultSetMappings;

        [NonSerialized]
        private readonly IDictionary filters;

		[NonSerialized]
		private readonly IDictionary imports;

		// templates are related to XmlDatabinder - nothing like that yet 
		// in NHibernate.
		//[NonSerialized] private Templates templates;

		[NonSerialized]
		private readonly IInterceptor interceptor;

		[NonSerialized]
		private readonly Settings settings;

		[NonSerialized]
		private readonly IDictionary properties;

		[NonSerialized]
		private SchemaExport schemaExport;

		[NonSerialized]
		private readonly IQueryCache queryCache;

		[NonSerialized]
		private readonly UpdateTimestampsCache updateTimestampsCache;

		[NonSerialized]
		private readonly IDictionary queryCaches;

		private static readonly IIdentifierGenerator UuidGenerator = new UUIDHexGenerator();

		private static readonly ILog log = LogManager.GetLogger(typeof(SessionFactoryImpl));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="settings"></param>
		public SessionFactoryImpl(Configuration cfg, IMapping mapping, Settings settings)
		{
			log.Info("building session factory");

			this.properties = cfg.Properties;
			this.interceptor = cfg.Interceptor;
			this.settings = settings;

			if (log.IsDebugEnabled)
			{
				log.Debug("instantiating session factory with properties: "
					+ CollectionPrinter.ToString(properties));
			}

			// Generators:
			identifierGenerators = new Hashtable();
			foreach (PersistentClass model in cfg.ClassMappings)
			{
				if (!model.IsInherited)
				{
					// TODO H3:
					IIdentifierGenerator generator = model.Identifier.CreateIdentifierGenerator(
						settings.Dialect);
					//					IIdentifierGenerator generator = model.Identifier.CreateIdentifierGenerator(
					//						settings.Dialect,
					//						settings.DefaultCatalogName,
					//						settings.DefaultSchemaName,
					//						(RootClass) model
					//						);
					identifierGenerators[model.MappedClass] = generator;
				}
			}


			// Persisters:

			classPersisters = new Hashtable();
			classPersistersByName = new Hashtable();
			IDictionary classMeta = new Hashtable();

			foreach (PersistentClass model in cfg.ClassMappings)
			{
				IEntityPersister cp = PersisterFactory.CreateClassPersister(model, this, mapping);
				classPersisters[model.MappedClass] = cp;

				// Adds the "Namespace.ClassName" (FullClassname) as a lookup to get to the Persiter.
				// Most of the internals of NHibernate use this method to get to the Persister since
				// Model.Name is used in so many places.  It would be nice to fix it up to be Model.TypeName
				// instead of just FullClassname
				classPersistersByName[model.Name] = cp;

				// Add in the AssemblyQualifiedName (includes version) as a lookup to get to the Persister.  
				// In HQL the Imports are used to get from the Classname to the Persister.  The
				// Imports provide the ability to jump from the Classname to the AssemblyQualifiedName.
				classPersistersByName[model.MappedClass.AssemblyQualifiedName] = cp;

				classMeta[model.MappedClass] = cp.ClassMetadata;
			}
			classMetadata = new Hashtable(classMeta);

			collectionPersisters = new Hashtable();
			foreach (Mapping.Collection map in cfg.CollectionMappings)
			{
				collectionPersisters[map.Role] = PersisterFactory
					.CreateCollectionPersister(map, this)
					.CollectionMetadata;
			}
			collectionMetadata = new Hashtable(collectionPersisters);

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

			namedQueries = new Hashtable(cfg.NamedQueries);
			namedSqlQueries = new Hashtable(cfg.NamedSQLQueries);
			sqlResultSetMappings = new Hashtable(cfg.SqlResultSetMappings);
            filters = new Hashtable(cfg.FilterDefinitions);

			imports = new Hashtable(cfg.Imports);

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

			if (settings.IsQueryCacheEnabled)
			{
				updateTimestampsCache = new UpdateTimestampsCache(settings.CacheProvider, properties);
				queryCache = settings.QueryCacheFactory
					.GetQueryCache(null, settings.CacheProvider, updateTimestampsCache, properties);
				queryCaches = Hashtable.Synchronized(new Hashtable());
			}
			else
			{
				updateTimestampsCache = null;
				queryCache = null;
				queryCaches = null;
			}
		}

		// Emulates constant time LRU/MRU algorithms for cache
		// It is better to hold strong references on some (LRU/MRU) queries
		private const int MaxStrongRefCount = 128;

		[NonSerialized]
		private readonly object[] strongRefs = new object[MaxStrongRefCount];

		[NonSerialized]
		private int strongRefIndex = 0;

		[NonSerialized]
		private readonly IDictionary softQueryCache = new WeakHashtable();
		// both keys and values may be soft since value keeps a hard ref to the key (and there is a hard ref to MRU values)

		/// <summary>
		/// A class that can be used as a Key in a Hashtable for 
		/// a Query Cache.
		/// </summary>
		private class QueryCacheKey
		{
			private string _query;
			private bool _scalar;

			internal QueryCacheKey(string query, bool scalar)
			{
				_query = query;
				_scalar = scalar;
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
				QueryCacheKey other = obj as QueryCacheKey;
				if (other == null)
				{
					return false;
				}

				return Equals(other);
			}

			public bool Equals(QueryCacheKey obj)
			{
				return this.Query.Equals(obj.Query) && this.Scalar == obj.Scalar;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return this.Query.GetHashCode() + this.Scalar.GetHashCode();
				}
			}

			#endregion
		}

		/// <summary>
		/// A class that can be used as a Key in a Hashtable for 
		/// a Query Cache.
		/// </summary>
		private class FilterCacheKey
		{
			private string _role;
			private string _query;
			private bool _scalar;

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
				return this.Role.Equals(obj.Role) && this.Query.Equals(obj.Query) && this.Scalar == obj.Scalar;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return this.Role.GetHashCode() + this.Query.GetHashCode() + this.Scalar.GetHashCode();
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
		private QueryTranslator[] CreateQueryTranslators(string[] concreteQueryStrings, QueryCacheKey cacheKey)
		{
			int length = concreteQueryStrings.Length;
			QueryTranslator[] queries = new QueryTranslator[length];
			for (int i = 0; i < length; i++)
			{
				queries[i] = new QueryTranslator(this, concreteQueryStrings[i], filters);
			}
			Put(cacheKey, queries);
			return queries;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private QueryTranslator CreateFilterTranslator(string filterString, FilterCacheKey cacheKey)
		{
			QueryTranslator filter = new QueryTranslator(this, filterString, filters);
			Put(cacheKey, filter);
			return filter;
		}

		public QueryTranslator[] GetQuery(string queryString, bool shallow)
		{
			QueryCacheKey cacheKey = new QueryCacheKey(queryString, shallow);

			// have to be careful to ensure that if the JVM does out-of-order execution
			// then another thread can't get an uncompiled QueryTranslator from the cache
			// we also have to be very careful to ensure that other threads can perform
			// compiled queries while another query is being compiled

			QueryTranslator[] queries = (QueryTranslator[]) Get(cacheKey);

			if (queries == null)
			{
				// a query that names an interface or unmapped class in the from clause
				// is actually executed as multiple queries
				string[] concreteQueryStrings = QueryTranslator.ConcreteQueries(queryString, this);
				queries = CreateQueryTranslators(concreteQueryStrings, cacheKey);
			}

			foreach (QueryTranslator q in queries)
			{
				q.Compile(settings.QuerySubstitutions, shallow);
			}
			// see comment above. note that QueryTranslator.compile() is synchronized

			return queries;
		}

		public QueryTranslator GetFilter(string filterString, string collectionRole, bool scalar)
		{
			FilterCacheKey cacheKey = new FilterCacheKey(collectionRole, filterString, scalar);

			QueryTranslator filter = (QueryTranslator) Get(cacheKey);
			if (filter == null)
			{
				filter = CreateFilterTranslator(filterString, cacheKey);
			}

			filter.Compile(collectionRole, settings.QuerySubstitutions, scalar);
			return filter;
		}

		private ISession OpenSession(IDbConnection connection, bool autoClose, long timestamp, IInterceptor interceptor)
		{
			return new SessionImpl(connection, this, autoClose, timestamp, interceptor);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		public ISession OpenSession(IDbConnection connection, IInterceptor interceptor)
		{
			// specify false for autoClose because the user has passed in an IDbConnection
			// and they assume responsibility for it.
			return OpenSession(connection, false, long.MinValue, interceptor);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		public ISession OpenSession(IInterceptor interceptor)
		{
			long timestamp = Timestamper.Next();
			// specify true for autoClose because NHibernate has responsibility for
			// the IDbConnection.
			return OpenSession(null, true, timestamp, interceptor);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public ISession OpenSession(IDbConnection connection)
		{
			return OpenSession(connection, interceptor);
		}

		/// <summary></summary>
		public ISession OpenSession()
		{
			return OpenSession(interceptor);
		}

		public IEntityPersister GetEntityPersister(string className)
		{
			return GetEntityPersister(className, true);
		}

		public IEntityPersister GetEntityPersister(string className, bool throwIfNotFound)
		{
			// TODO: H2.1 has the code below, an equivalent for .NET would be useful
			//if( className.StartsWith( "[" ) )
			//{
			//	throw new MappingException( "No persister for array result, likely a broken query" );
			//}

			IEntityPersister result = classPersistersByName[className] as IEntityPersister;
			if (result == null && throwIfNotFound)
			{
				throw new MappingException("No persister for: " + className);
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
			//		"No output stylesheet configured. Use the property hibernate.output_stylesheet and ensure xalan.jar is in classpath"
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
		public Dialect.Dialect Dialect
		{
			get { return settings.Dialect; }
		}

		/// <summary></summary>
		public ITransactionFactory TransactionFactory
		{
			get { return settings.TransactionFactory; }
		}

		// TransactionManager - not ported

		// TODO: SQLExceptionConverter
		// JNDI Reference - not ported

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
		public bool IsBatchUpdateEnabled
		{
			get { return settings.BatchSize > 0; }
		}

		/// <summary></summary>
		public int BatchSize
		{
			get { return settings.BatchSize; }
		}

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
		/// <param name="name">The name of that identifies the query.</param>
		/// <returns>
		/// A <c>hql</c> query or <c>null</c> if the named
		/// query does not exist.
		/// </returns>
		public NamedQueryDefinition GetNamedQuery(string name)
		{
			return (NamedQueryDefinition) namedQueries[name];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
		public NamedSQLQueryDefinition GetNamedSQLQuery(string queryName)
		{
			return namedSqlQueries[queryName] as NamedSQLQueryDefinition;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectClass"></param>
		/// <returns></returns>
		public IType GetIdentifierType(System.Type objectClass)
		{
			return GetEntityPersister(objectClass).IdentifierType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectClass"></param>
		/// <returns></returns>
		public string GetIdentifierPropertyName(System.Type objectClass)
		{
			return GetEntityPersister(objectClass).IdentifierPropertyName;
		}

		public IType[] GetReturnTypes(String queryString)
		{
			string[] queries = QueryTranslator.ConcreteQueries(queryString, this);
			if (queries.Length == 0)
			{
				throw new HibernateException("Query does not refer to any persistent classes: " + queryString);
			}
			return GetQuery(queries[0], true)[0].ReturnTypes;
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
		/// <param name="clazz"></param>
		/// <returns></returns>
		public string[] GetImplementors(System.Type clazz)
		{
			ArrayList results = new ArrayList();
			foreach (IEntityPersister p in classPersisters.Values)
			{
				if (p is IQueryable)
				{
					IQueryable q = (IQueryable) p;
					string className = q.ClassName;
					bool isMappedClass = clazz.Equals(q.MappedClass);
					if (q.IsExplicitPolymorphism)
					{
						if (isMappedClass)
						{
							return new string[] { className };
						}
					}
					else
					{
						if (isMappedClass)
						{
							results.Add(className);
						}
						else if (
							clazz.IsAssignableFrom(q.MappedClass) &&
								(!q.IsInherited || !clazz.IsAssignableFrom(q.MappedSuperclass)))
						{
							results.Add(className);
						}
					}
				}
			}
			return (string[]) results.ToArray(typeof(string));
		}

		/// <summary>
		/// Added to solve a problem with SessionImpl.Find( CriteriaImpl ),
		/// see the comment there for an explanation.
		/// </summary>
		/// <param name="clazz"></param>
		/// <returns></returns>
		public System.Type[] GetImplementorClasses(System.Type clazz)
		{
			ArrayList results = new ArrayList();
			foreach (IEntityPersister p in classPersisters.Values)
			{
				if (p is IQueryable)
				{
					IQueryable q = (IQueryable) p;
					bool isMappedClass = clazz.Equals(q.MappedClass);
					if (q.IsExplicitPolymorphism)
					{
						if (isMappedClass)
						{
							return new System.Type[] { q.MappedClass };
						}
					}
					else
					{
						if (isMappedClass)
						{
							results.Add(q.MappedClass);
						}
						else if (
							clazz.IsAssignableFrom(q.MappedClass) &&
							(!q.IsInherited || !clazz.IsAssignableFrom(q.MappedSuperclass)))
						{
							results.Add(q.MappedClass);
						}
					}
				}
			}
			return (System.Type[]) results.ToArray(typeof(System.Type));
		}

		public string GetImportedClassName(string className)
		{
			string result = imports[className] as string;
			return (result == null) ? className : result;
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
				p.Cache.Remove(id);
			}
		}

		public void Evict(System.Type persistentClass)
		{
			IEntityPersister p = GetEntityPersister(persistentClass);
			if (p.HasCache)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("evicting second-level cache: " + p.ClassName);
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
				p.Cache.Remove(id);
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

		public IType GetPropertyType(System.Type persistentClass, string propertyName)
		{
			return GetEntityPersister(persistentClass).GetPropertyType(propertyName);
		}

		/// <summary></summary>
		public bool IsShowSqlEnabled
		{
			get { return settings.IsShowSqlEnabled; }
		}

		/// <summary></summary>
		public int FetchSize
		{
			get { return settings.StatementFetchSize; }
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
					settings.CacheProvider,
					updateTimestampsCache,
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
		public bool PrepareSql
		{
			get { return settings.PrepareSql; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
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
			return (ResultSetMappingDefinition) sqlResultSetMappings[resultSetName];
		}

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            FilterDefinition def = (FilterDefinition)filters[filterName];
            if (def == null)
            {
                throw new HibernateException("No such filter configured [" + filterName + "]");
            }
            return def;
        }

        public ICollection DefinedFilterNames
        {
            get { return filters.Keys; }
        }
    }
}