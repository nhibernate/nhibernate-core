using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Runtime.CompilerServices;

using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.Ps;
using NHibernate.Transaction;
using NHibernate.Type;
using NHibernate.Util;

using HibernateDialect = NHibernate.Dialect.Dialect;



namespace NHibernate.Impl {
	
	/// <summary>
	///  Concrete implementation of a SessionFactory.
	/// </summary>
	/// <remarks>
	/// IMMUTABLE
	/// </remarks>
	public class SessionFactoryImpl : ISessionFactory, ISessionFactoryImplementor {
		
		private string name;
		private string uuid;

		private IDictionary classPersisters;
		private IDictionary classPersistersByName;
		private IDictionary collectionPersisters;
		private IDictionary namedQueries;
		private IDictionary imports;
		private IConnectionProvider connections;
		private IDictionary properties;
		private bool showSql;
		private bool useOuterJoin;
		private bool supportsLocking;
		private IDictionary querySubstitutions;
		private string[] queryImports;
		private Dialect.Dialect dialect;
		private PreparedStatementCache statementCache;
		private ITransactionFactory transactionFactory;
		private int adoBatchSize;

		private string defaultSchema;
		//private object statementFetchSize;
		private IInterceptor interceptor;

		private static IIdentifierGenerator uuidgen = new UUIDStringGenerator();

		private static System.Type[] PersisterConstructorArgs = new System.Type[] {
																					  typeof(PersistentClass), typeof(ISessionFactoryImplementor) };
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CollectionPersister));

	
		public SessionFactoryImpl(Configuration cfg, IDictionary properties, IInterceptor interceptor) {

			if ( log.IsDebugEnabled ) log.Debug("instantiating session factory with properties: " + properties);

			this.interceptor = interceptor;

			Dialect.Dialect dl = null;
			bool sl = false;
			try {
				//TODO: make this work
				//dl = Dialect.Dialect.GetDialect(properties);
				dl = HibernateDialect.GetDialect();
				IDictionary temp = new Hashtable();

				foreach(DictionaryEntry de in dl.DefaultProperties) {
					temp.Add(de.Key, de.Value);
				}
				foreach(DictionaryEntry de in properties) {
					temp.Add(de.Key, de.Value);
				}
				properties = temp;
			} catch (HibernateException he) {
				log.Warn( "No dialect set - using GenericDialect: " + he.Message );
				dl = new GenericDialect();
			}
			dialect = dl;
			supportsLocking = sl;

			connections = ConnectionProviderFactory.NewConnectionProvider(properties);

			int cacheSize = PropertiesHelper.GetInt( Cfg.Environment.StatementCacheSize, properties, 0);
			statementCache = ( cacheSize<1 || connections.IsStatementCache ) ? null : new PreparedStatementCache(cacheSize);

			//statementFetchSize = PropertiesHelper.GetInt( Cfg.Environment.StatementFetchSize, properties);
			//if (statementFetchSize!=null) log.Info("ado result set fetch size: " + statementFetchSize);

			useOuterJoin = PropertiesHelper.GetBoolean(Cfg.Environment.OuterJoin, properties);
			log.Info("use outer join fetching: " + useOuterJoin);

			//bool usrs = PropertiesHelper.GetBoolean(Cfg.Environment.UseScrollableResultSet, properties);
			//int batchSize = PropertiesHelper.GetInt(Cfg.Environment.statementBatchSize, properties, 0);

			try {
				IDbConnection conn = connections.GetConnection();
				try {
					//get meta data
					adoBatchSize = 0;
				} finally {
					connections.CloseConnection(conn);
				}
			} catch (Exception e) {
				log.Warn("could not obtain connection metadata", e);
			}
			
			defaultSchema = properties[Cfg.Environment.DefaultSchema] as string;
			if ( defaultSchema!=null) log.Info ("Default schema set to: " + defaultSchema);

			transactionFactory = BuildTransactionFactory(properties);

			showSql = PropertiesHelper.GetBoolean(Cfg.Environment.ShowSql, properties);
			if (showSql) log.Info("echoing all SQL to stdout");

			this.properties = properties;

			// Persisters:

			classPersisters = new Hashtable();
			classPersistersByName = new Hashtable();

			foreach(PersistentClass model in cfg.ClassMappings) {
				System.Type persisterClass = model.Persister;
				IClassPersister cp;
				if (persisterClass==null || persisterClass==typeof(EntityPersister)) {
					cp = new EntityPersister(model, this);
				} else if (persisterClass==typeof(NormalizedEntityPersister)) {
					cp = new NormalizedEntityPersister(model, this);
				} else {
					cp = InstantiatePersister(persisterClass, model);
				}
				classPersisters.Add( model.PersistentClazz, cp);
				classPersistersByName.Add( model.Name, cp );
			}

			collectionPersisters = new Hashtable();
			foreach( Mapping.Collection map in cfg.CollectionMappings ) {
				collectionPersisters.Add( map.Role, new CollectionPersister(map, cfg, this) );
			}

			foreach(IClassPersister persister in classPersisters.Values) {
				persister.PostInstantiate(this);
			}

			//TODO: Add for databinding

			name = (string) properties[ Cfg.Environment.SessionFactoryName ];

			try {
				uuid = (string) uuidgen.Generate(null, null);
			} catch (Exception) {
				throw new AssertionFailure("could not generate UUID");
			}

			// queries:

			querySubstitutions = PropertiesHelper.ToDictionary(Cfg.Environment.QuerySubstitutions, " ,=;:\n\t\r\f", properties);
			log.Info("Query language substitutions: " + querySubstitutions);

			queryImports = PropertiesHelper.ToStringArray(Cfg.Environment.QueryImports, " ,=;:\n\t\r\f", properties);
			if ( queryImports.Length!=0 ) log.Info( "Query language imports: " + StringHelper.ToString(queryImports) );

			namedQueries = cfg.NamedQueries;
			imports = new Hashtable( cfg.Imports );

			log.Debug("Instantiated session factory");

		}

		// Emulates constant time LRU/MRU algorithms for cache
		// It is better to hold strong references on some (LRU/MRU) queries
		private const int MaxStrongRefCount = 128;
		private readonly object[] strongRefs = new object[MaxStrongRefCount];
		private int strongRefIndex = 0;
		private readonly IDictionary softQueryCache = new Hashtable(); //TODO: make soft reference map

		//TODO: All
		private static readonly QueryCacheKeyFactory QueryKeyFactory;
		private static readonly FilterCacheKeyFactory FilterKeyFactory;
		static SessionFactoryImpl() {
			/*
			QueryKeyFactory = (QueryCacheKeyFactory) KeyFactory.Create(QueryCacheKeyFactory.GetType(), QueryCacheKeyFactory......);
			FilterKeyFactory = (FilterCacheKeyFactory) KeyFactory.Create(
			FilterCacheKeyFactory.class, FilterCacheKeyFactory.class.getClassLoader() 				);*/
			
			QueryKeyFactory = null;
			FilterKeyFactory = null;
		}
																												

		//returns generated class instance
		interface QueryCacheKeyFactory {
			// will not recalculate hashKey for constant queries
			object NewInstance(string query, bool scalar);
		}
																								//returns generated class instance
		interface FilterCacheKeyFactory {
			// will not recalculate hashKey for constant queries
			object NewInstance(string query, bool scalar);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private object Get(object key) {		
			object result = softQueryCache[key];
			if ( result != null ) {
				strongRefs[ ++strongRefIndex % MaxStrongRefCount ] = result;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Put(object key, object value) {
			softQueryCache[key] = value;
			strongRefs[ ++strongRefIndex % MaxStrongRefCount ] = value;
		}

		public QueryTranslator GetQuery(string query) {
			return GetQuery(query, false);
		}

		public QueryTranslator GetShallowQuery(string query) {
			return GetQuery(query, true);
		}

		private QueryTranslator GetQuery(string query, bool shallow) {
			/*object cacheKey = QueryKeyFactory.NewInstance(query, shallow);

			// have to be careful to ensure that if the JVM does out-of-order execution
			// then another thread can't get an uncompiled QueryTranslator from the cache
			// we also have to be very careful to ensure that other threads can perform
			// compiled queries while another query is being compiled

			QueryTranslator q = (QueryTranslator) Get(cacheKey);
			if ( q==null) {
				q = new QueryTranslator();
				Put(cacheKey, q);
			}
			*/
			QueryTranslator q = new QueryTranslator();
			q.Compile(this, query, querySubstitutions, shallow);
			
			return q;
		}

		public FilterTranslator GetFilter(string query, string collectionRole, bool scalar) {
			object cacheKey = FilterKeyFactory.NewInstance( query, scalar );

			FilterTranslator q = (FilterTranslator) Get(cacheKey);
			if ( q==null ) {
				q = new FilterTranslator();
				Put(cacheKey, q);
			}
			q.Compile(collectionRole, this, query, querySubstitutions, scalar);
			
			return q;
		}

		private ISession OpenSession(IDbConnection connection, bool autoClose, long timestamp, IInterceptor interceptor) {
			return new SessionImpl( connection, this, autoClose, timestamp, interceptor );
		}

		public ISession OpenSession(IDbConnection connection, IInterceptor interceptor) {
			return OpenSession( connection, false, long.MinValue, interceptor );
		}

		public ISession OpenSession(IInterceptor interceptor) {
			long timestamp = Timestamper.Next();
			return OpenSession( null, true, timestamp, interceptor );
		}

		public ISession OpenSession(IDbConnection connection) {
			return OpenSession(connection, interceptor);
		}

		public ISession OpenSession() {
			return OpenSession(interceptor);
		}

		public IDbConnection OpenConnection() {
			try {
				return connections.GetConnection();
			} catch (Exception sqle) {
				throw new ADOException("cannot open connection", sqle);
			}
		}
		
		public void CloseConnection(IDbConnection conn) {
			try {
				connections.CloseConnection(conn);
			} catch (Exception e) {
				throw new ADOException("cannot close connection", e);
			}
		}

		public IClassPersister GetPersister(string className) {
			IClassPersister result = (IClassPersister) classPersistersByName[className];
			if ( result==null) throw new MappingException( "No persister for: " + className );
			return result;
		}

		public IClassPersister GetPersister(System.Type theClass) {
			IClassPersister result = (IClassPersister) classPersisters[theClass];
			if ( result==null) throw new MappingException( "No persisters for: " + theClass.FullName );
			return result;
		}

		public CollectionPersister GetCollectionPersister(string role) {
			CollectionPersister result = (CollectionPersister) collectionPersisters[role];
			if ( result==null ) throw new MappingException( "No persisters for collection role: " + role );
			return result;
		}

		public IDatabinder OpenDatabinder() {
			return null;
			//TODO: this has to be implemented
		}

		public Dialect.Dialect Dialect {
			get { return dialect; }
		}

		private ITransactionFactory BuildTransactionFactory(IDictionary transactionProps) {
			return new TransactionFactory();
		}

		public ITransactionFactory TransactionFactory {
			get { return transactionFactory; }
		}

		
		public IDbCommand GetPreparedStatement(IDbConnection conn, string sql, bool scrollable) {

			if ( log.IsDebugEnabled ) log.Debug(
										  "prepared statement get: " + sql
										  );
			if ( showSql ) Console.WriteLine("Hibernate: " + sql);

			if ( statementCache != null ) {
				return statementCache.GetPreparedStatement(sql, conn);
			} else {
				try {
					log.Debug("preparing statement");
					IDbCommand retVal = conn.CreateCommand();
					retVal.CommandText = sql;
					retVal.CommandType = CommandType.Text;

					// Hack: force parameters to be created
					Impl.AdoHack.CreateParameters(dialect, retVal);
					// end-of Hack

					// Hack: disable Prepare() as long as the parameters have no datatypes!!
#if FALSE
					retVal.Prepare();
#endif
					// end-of Hack

					return retVal;
				} catch (Exception e) {
					throw e;
				}
			}
		}

		public void ClosePreparedStatement(IDbCommand ps) 
		{
			if ( statementCache != null ) {
				statementCache.ClosePreparedStatement(ps);
			} else {
				try {
					log.Debug("closing statement");
					ps.Dispose();
				} catch (Exception e) {
					throw e;
				}
			}
		}

		public bool UseAdoBatch {
			get { return adoBatchSize > 0; }
		}

		public int ADOBatchSize {
			get { return adoBatchSize; }
		}

		public bool EnableJoinedFetch {
			get { return useOuterJoin; }
		}

		public bool UseScrollableResultSets {
			get { return false; }
		}

		public string GetNamedQuery(string name) {
			string queryString = (string) namedQueries[name];
			if (queryString==null) throw new MappingException("Named query not known: " + name);
			return queryString;
		}

		public IType GetIdentifierType(System.Type objectClass) {
			return GetPersister(objectClass).IdentifierType;
		}

		//TODO: Serialization stuff

		public IType[] GetReturnTypes(string queryString) {
			string[] queries = QueryTranslator.ConcreteQueries(queryString, this);
			if ( queries.Length==0 ) throw new HibernateException("Query does not refer to any persistent classes: " + queryString);
			return GetShallowQuery( queries[0] ).ReturnTypes;
		}

		public ICollection GetNamedParameters(string queryString) {
			string[] queries = QueryTranslator.ConcreteQueries(queryString, this);
			if ( queries.Length==0 ) throw new HibernateException("Query does not refer to any persistent classes: " + queryString);
			return GetShallowQuery( queries[0] ).NamedParameters;
		}

		public string DefaultSchema {
			get { return defaultSchema; }
		}

		public void SetFetchSize(IDbCommand statement) {
			//if ( statementFetchSize!=null) s
		}

		private IClassPersister InstantiatePersister(System.Type persisterClass, PersistentClass model) {
			
			try {
				return (IClassPersister) Activator.CreateInstance( persisterClass, new object[] { model, this } );
			} catch (Exception e) {
				if ( e is HibernateException ) {
					throw (HibernateException) e;
				} else {
					throw new MappingException( "Could not instantiate persiser " + persisterClass.Name, e);
				}
			}
		}

		public IClassMetadata GetClassMetadata(System.Type persistentClass) {
			return GetPersister(persistentClass).ClassMetadata;
		}

		public ICollectionMetadata GetCollectionMetadata(string roleName) {
			return (ICollectionMetadata) GetCollectionPersister(roleName);
		}

		public string[] GetImplementors(System.Type clazz) {
			ArrayList results = new ArrayList();
			foreach(IClassPersister p in classPersisters.Values) {
				if ( p is IQueryable ) {
					IQueryable q = (IQueryable) p;
					string name = q.ClassName;
					bool isMappedClass = clazz.Equals( q.MappedClass );
					if ( q.IsExplicitPolymorphism ) {
						if (isMappedClass) return new string[] { name };
					} else {
						if ( isMappedClass ) {
							results.Add(name);
						} else if (
							clazz.IsAssignableFrom( q.MappedClass ) &&
							( !q.IsInherited || !clazz.IsAssignableFrom( q.MappedSuperclass ) ) ) {

							results.Add(name);
						}
					}
				}
			}
			return (string[]) results.ToArray(typeof(string));
		}

		/*public string[] Imports {
			get { return queryImports; }
		}*/
		public string GetImportedClassName(string name) {
			string result = (string) imports[name];
			return (result==null) ? name : result;
		}

		public IDictionary GetAllClassMetadata() {
			return classPersisters;
		}

		public IDictionary GetAllCollectionMetadata() {
			 return collectionPersisters;
		}

		public void Close() {
		//	if ( statementCache!= null ) 
				
				//
		}
	}
}
