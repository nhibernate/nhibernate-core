using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

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
	/// Caches "compiled" mappings - ie. <see cref="IClassPersisters"/> 
	/// and <see cref="CollectionPersisters"/>
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
		
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SessionFactoryImpl));

		private string name;
		private string uuid;

		[NonSerialized] private IDictionary classPersisters;
		[NonSerialized] private IDictionary classPersistersByName;
		[NonSerialized] private IDictionary collectionPersisters;
		[NonSerialized] private IDictionary namedQueries;
		[NonSerialized] private IDictionary imports;
		[NonSerialized] private IConnectionProvider connectionProvider;
		[NonSerialized] private IDictionary properties;
		[NonSerialized] private bool showSql;
		[NonSerialized] private bool useOuterJoin;
		[NonSerialized] private IsolationLevel isolation;
		// TODO: figure out why this is commented out in nh and not h2.0.3
		//[NonSerialized] private Templates templates;
		[NonSerialized] private IDictionary querySubstitutions;
		[NonSerialized] private Dialect.Dialect dialect;
		[NonSerialized] private PreparedStatementCache statementCache;
		[NonSerialized] private ITransactionFactory transactionFactory;
		[NonSerialized] private int adoBatchSize;
		[NonSerialized] private bool useScrollableResultSets;

		[NonSerialized] private string defaultSchema;
		[NonSerialized] private object statementFetchSize;
		[NonSerialized] private IInterceptor interceptor;

		private static IIdentifierGenerator uuidgen = new UUIDHexGenerator();
	
		public SessionFactoryImpl(Configuration cfg, IDictionary properties, IInterceptor interceptor) 
		{

			log.Info("building session factory");
			if ( log.IsDebugEnabled ) 
			{
				StringBuilder sb = new StringBuilder("instantiating session factory with properties: ");
				foreach(DictionaryEntry entry in properties)
					sb.AppendFormat("{0}={1};", entry.Key, ((string)entry.Key).IndexOf("connection_string")>0?"***":entry.Value);
				log.Debug(sb.ToString());
			}

			this.interceptor = interceptor;

			Dialect.Dialect dl = null;
			
			try 
			{
				dl = HibernateDialect.GetDialect(properties);
				IDictionary temp = new Hashtable();
				
				foreach(DictionaryEntry de in dl.DefaultProperties) 
				{
					temp[de.Key] = de.Value;
				}
				foreach(DictionaryEntry de in properties) 
				{
					temp[de.Key] = de.Value;
				}
				properties = temp;
			} 
			catch (HibernateException he) 
			{
				log.Warn( "No dialect set - using GenericDialect: " + he.Message );
				dl = new GenericDialect();
			}
			dialect = dl;
			
			connectionProvider = ConnectionProviderFactory.NewConnectionProvider(properties);

			// TODO: DESIGNQUESTION: There are other points in the application that have questions about the
			// statementCache - I just don't see this as being needed yet.  
			int cacheSize = PropertiesHelper.GetInt32( Cfg.Environment.StatementCacheSize, properties, 0);
			statementCache = ( cacheSize<1 || connectionProvider.IsStatementCache ) ? null : new PreparedStatementCache(cacheSize);
			//statementCache = null;

			statementFetchSize = PropertiesHelper.GetInt32( Cfg.Environment.StatementFetchSize, properties, -1);
			if((int)statementFetchSize==-1) statementFetchSize = null;
			if (statementFetchSize!=null) log.Info("ado result set fetch size: " + statementFetchSize);

			useOuterJoin = PropertiesHelper.GetBoolean(Cfg.Environment.OuterJoin, properties);
			log.Info("use outer join fetching: " + useOuterJoin);

			// default the isolationLevel to Unspecified to indicate to our code that no isolation level 
			// has been set so just use the default of the DataProvider.
			string isolationString = PropertiesHelper.GetString( Cfg.Environment.Isolation, properties, String.Empty );
			if( isolationString!=String.Empty ) 
			{
				try 
				{
					isolation = (IsolationLevel)Enum.Parse( typeof(IsolationLevel), isolationString );
					log.Info( "Using Isolation Level: " + isolation.ToString() );
				}
				catch( ArgumentException ae ) 
				{
					log.Error( "error configuring IsolationLevel " + isolationString, ae );
					throw new HibernateException( 
						"The isolation level of " + isolationString + " is not a valid IsolationLevel.  Please " +
						"use one of the Member Names from the IsolationLevel.", ae );
				}
			}
			else 
			{
				isolation = IsolationLevel.Unspecified;
			}

			
			bool usrs = PropertiesHelper.GetBoolean(Cfg.Environment.UseScrollableResultSet, properties);
			int batchSize = PropertiesHelper.GetInt32(Cfg.Environment.StatementBatchSize, properties, 0);

			try 
			{
				IDbConnection conn = connectionProvider.GetConnection();
				try 
				{
					//get meta data
					usrs = false; // no scrollable results sets in .net -> forward only readers...
					batchSize = 0; // is this
				} 
				finally 
				{
					connectionProvider.CloseConnection(conn);
				}
			} 
			catch (Exception e) 
			{
				log.Warn("could not obtain connection metadata", e);
			}
			
			useScrollableResultSets = usrs;
			adoBatchSize = batchSize;

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
				cp = PersisterFactory.Create(model, this);
				classPersisters[model.PersistentClazz] = cp;
				
				// Adds the "Namespace.ClassName" (FullClassname) as a lookup to get to the Persiter.
				// Most of the internals of NHibernate use this method to get to the Persister since
				// Model.Name is used in so many places.  It would be nice to fix it up to be Model.TypeName
				// instead of just FullClassname
				classPersistersByName[model.Name] = cp ;
				
				// Add in the AssemblyQualifiedName (includes version) as a lookup to get to the Persister.  
				// In HQL the Imports are used to get from the Classname to the Persister.  The
				// Imports provide the ability to jump from the Classname to the AssemblyQualifiedName.
				classPersistersByName[model.PersistentClazz.AssemblyQualifiedName] = cp;
			}

			collectionPersisters = new Hashtable();
			foreach( Mapping.Collection map in cfg.CollectionMappings ) {
				collectionPersisters[map.Role] = new CollectionPersister(map, cfg, this) ;
			}

			foreach(IClassPersister persister in classPersisters.Values) {
				persister.PostInstantiate(this);
			}

			//TODO: Add for databinding

			name = (string) properties[ Cfg.Environment.SessionFactoryName ];

			try 
			{
				uuid = (string) uuidgen.Generate(null, null);
			} 
			catch (Exception) 
			{
				throw new AssertionFailure("could not generate UUID");
			}

			SessionFactoryObjectFactory.AddInstance(uuid, name, this, properties);
			// queries:

			querySubstitutions = PropertiesHelper.ToDictionary(Cfg.Environment.QuerySubstitutions, " ,=;:\n\t\r\f", properties);
			if ( log.IsInfoEnabled ) 
			{
				StringBuilder sb = new StringBuilder("Query language substitutions: ");
				foreach(DictionaryEntry entry in querySubstitutions)
					sb.AppendFormat("{0}={1};", entry.Key, entry.Value);
				log.Info(sb.ToString());
			}

			namedQueries = cfg.NamedQueries;
			imports = new Hashtable( cfg.Imports );

			log.Debug("Instantiated session factory");

		}

		// Emulates constant time LRU/MRU algorithms for cache
		// It is better to hold strong references on some (LRU/MRU) queries
		private const int MaxStrongRefCount = 128;
		[NonSerialized] private readonly object[] strongRefs = new object[MaxStrongRefCount];
		[NonSerialized] private int strongRefIndex = 0;
		[NonSerialized] private readonly IDictionary softQueryCache = new Hashtable(); //TODO: make soft reference map

		
		static SessionFactoryImpl() 
		{
			// used to do some CGLIB stuff in here for QueryKeyFactory and FilterKeyFactory
		}
																												

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
				if( other==null) return false;

				return Equals(other);
			}

			public bool Equals(QueryCacheKey obj) 
			{
				return this.Query.Equals(obj.Query) && this.Scalar==obj.Scalar;
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
				if( other==null) return false;

				return Equals(other);
			}

			public bool Equals(FilterCacheKey obj) 
			{
				return this.Role.Equals(obj.Role) && this.Query.Equals(obj.Query) && this.Scalar==obj.Scalar;
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
			if ( result != null ) 
			{
				strongRefs[ ++strongRefIndex % MaxStrongRefCount ] = result;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Put(object key, object value) 
		{
			softQueryCache[key] = value;
			strongRefs[ ++strongRefIndex % MaxStrongRefCount ] = value;
		}

		public IConnectionProvider ConnectionProvider 
		{
			get {return this.connectionProvider;}
		}

		public IsolationLevel Isolation 
		{
			get { return isolation; }
		}

		public QueryTranslator GetQuery(string query) 
		{
			return GetQuery(query, false);
		}

		public QueryTranslator GetShallowQuery(string query) 
		{
			return GetQuery(query, true);
		}

		private QueryTranslator GetQuery(string query, bool shallow) 
		{
			QueryCacheKey cacheKey = new QueryCacheKey(query, shallow);

			// have to be careful to ensure that if the JVM does out-of-order execution
			// then another thread can't get an uncompiled QueryTranslator from the cache
			// we also have to be very careful to ensure that other threads can perform
			// compiled queries while another query is being compiled

			QueryTranslator q = (QueryTranslator) Get(cacheKey);
			if ( q==null) 
			{
				q = new QueryTranslator(dialect);
				Put(cacheKey, q);
			}
			
			q.Compile(this, query, querySubstitutions, shallow);
			
			return q;
		}

		public FilterTranslator GetFilter(string query, string collectionRole, bool scalar) 
		{
			FilterCacheKey cacheKey = new FilterCacheKey( collectionRole, query, scalar );

			FilterTranslator q = (FilterTranslator) Get(cacheKey);
			if ( q==null ) 
			{
				q = new FilterTranslator(dialect);
				Put(cacheKey, q);
			}
			
			q.Compile(collectionRole, this, query, querySubstitutions, scalar);
			
			return q;
		}

		private ISession OpenSession(IDbConnection connection, bool autoClose, long timestamp, IInterceptor interceptor) 
		{
			return new SessionImpl( connection, this, autoClose, timestamp, interceptor );
		}

		public ISession OpenSession(IDbConnection connection, IInterceptor interceptor) 
		{
			//TODO: figure out why autoClose was set to false - diff in JDBC vs ADO.NET???
			return OpenSession( connection, false, long.MinValue, interceptor );
		}

		public ISession OpenSession(IInterceptor interceptor) 
		{
			long timestamp = Timestamper.Next();
			return OpenSession( null, true, timestamp, interceptor );
		}

		public ISession OpenSession(IDbConnection connection) 
		{
			return OpenSession(connection, interceptor);
		}

		public ISession OpenSession() 
		{
			return OpenSession(interceptor);
		}

		public IDbConnection OpenConnection() 
		{
			try 
			{
				return connectionProvider.GetConnection();
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
				connectionProvider.CloseConnection(conn);
			} 
			catch (Exception e) 
			{
				throw new ADOException("cannot close connection", e);
			}
		}

		public IClassPersister GetPersister(string className) 
		{
			//(IClassPersister) was replaced by as
			IClassPersister result = classPersistersByName[className] as IClassPersister;
			//TODO: not throwing this exception results in a 50% perf gain with hql - not sure
			// where else this code is being used and expecting an exception...
//			if ( result==null) throw new MappingException( "No persister for: " + className );
			return result;
		}

		public IClassPersister GetPersister(System.Type theClass) 
		{
			//IClassPersister result = (IClassPersister) classPersisters[theClass];
			IClassPersister result = classPersisters[theClass] as IClassPersister;
			if ( result==null) throw new MappingException( "No persisters for: " + theClass.FullName );
			return result;
		}

		public CollectionPersister GetCollectionPersister(string role) 
		{
			//CollectionPersister result = (CollectionPersister) collectionPersisters[role];
			CollectionPersister result = collectionPersisters[role] as CollectionPersister;
			if ( result==null ) throw new MappingException( "No persisters for collection role: " + role );
			return result;
		}

		public IDatabinder OpenDatabinder() 
		{
			throw new NotImplementedException("Have not coded Databinder yet.");
		}

		public Dialect.Dialect Dialect 
		{
			get { return dialect; }
		}

		private ITransactionFactory BuildTransactionFactory(IDictionary transactionProps) 
		{
			return new TransactionFactory();
		}

		public ITransactionFactory TransactionFactory 
		{
			get { return transactionFactory; }
		}

		//TODO: revisit if we want the SessionFactoryImpl to store the PreparedStatements considering
		// that ADO.NET handles prepared Commands differently depending on the provider
		public IDbCommand GetPreparedStatement(IDbConnection conn, string sql, bool scrollable) {

			if ( log.IsDebugEnabled ) log.Debug("prepared statement get: " + sql);
			if ( showSql ) log.Debug("Hibernate: " + sql);

			//TODO: what would be the implications of hooking up the PreparedStatment (IDbCommand) to
			// the IDbTransaction at this point.  I am a little nervous about this because the SessionFactory
			// is not specific to a Session.  So would the IDbCommand object be shared among different sessions?
			// Would that cause us to run into problems where one Session would be using the Transaction from 
			// a different Session??
			// NOTE: I have commented out the code that assigns the statement cache - so it will always
			// be null and we will be creating a new command each time.
					
			if ( statementCache != null ) 
			{
				return statementCache.GetPreparedStatement(sql, conn);
			} 
			else {
				try {
					log.Debug("preparing statement");
					IDbCommand retVal = conn.CreateCommand();
					retVal.CommandText = sql;
					retVal.CommandType = CommandType.Text;

					
					// Hack: disable Prepare() as long as the parameters have no datatypes!!
#if FALSE
					retVal.Prepare();
#endif
					// end-of Hack

					return retVal;
				} 
				catch (Exception e) {
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
					//TODO: there is some missing logic about when this gets called - with the OleDb driver
					// as soon as the Dispose is called the CommandText=="", with SqlServer driver that 
					// is not occurring - don't know why not???  This prevents a command from being called
					// more than 1 time in a row...
					// In H2.0.3 this is a Close - not a dispose.  It looks like each Provider implements
					// Dispose just a bit differently...
					//ps.Dispose();
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
			get { return useScrollableResultSets; }
		}

		public string GetNamedQuery(string name) 
		{
			string queryString = namedQueries[name] as string;
			if (queryString==null) throw new MappingException("Named query not known: " + name);
			return queryString;
		}

		public IType GetIdentifierType(System.Type objectClass) 
		{
			return GetPersister(objectClass).IdentifierType;
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
			if(result==null) 
			{
				// if we were deserialized into a different AppDomain, look for an instance with the
				// same name.
				result = SessionFactoryObjectFactory.GetNamedInstance(name);
				if(result==null) 
				{
					throw new NullReferenceException("Could not find a SessionFactory named " + name + " or identified by uuid " + uuid );
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


		public IType[] GetReturnTypes(string queryString) 
		{
			string[] queries = QueryTranslator.ConcreteQueries(queryString, this);
			if ( queries.Length==0 ) throw new HibernateException("Query does not refer to any persistent classes: " + queryString);
			return GetShallowQuery( queries[0] ).ReturnTypes;
		}

		public ICollection GetNamedParameters(string queryString) 
		{
			string[] queries = QueryTranslator.ConcreteQueries(queryString, this);
			if ( queries.Length==0 ) throw new HibernateException("Query does not refer to any persistent classes: " + queryString);
			return GetShallowQuery( queries[0] ).NamedParameters;
		}

		public string DefaultSchema 
		{
			get { return defaultSchema; }
		}

		public void SetFetchSize(IDbCommand statement) 
		{
			if ( statementFetchSize!=null) 
			{
				// nothing to do in ADO.NET
			}
		}

		public IClassMetadata GetClassMetadata(System.Type persistentClass) 
		{
			return GetPersister(persistentClass).ClassMetadata;
		}

		public ICollectionMetadata GetCollectionMetadata(string roleName) 
		{
			return (ICollectionMetadata) GetCollectionPersister(roleName);
		}

		public string[] GetImplementors(System.Type clazz) 
		{
			ArrayList results = new ArrayList();
			foreach(IClassPersister p in classPersisters.Values) 
			{
				if ( p is IQueryable ) 
				{
					IQueryable q = (IQueryable) p;
					string name = q.ClassName;
					bool isMappedClass = clazz.Equals( q.MappedClass );
					if ( q.IsExplicitPolymorphism ) 
					{
						if (isMappedClass) return new string[] { name };
					} 
					else 
					{
						if ( isMappedClass ) 
						{
							results.Add(name);
						} 
						else if (
							clazz.IsAssignableFrom( q.MappedClass ) &&
							( !q.IsInherited || !clazz.IsAssignableFrom( q.MappedSuperclass ) ) ) 
						{

							results.Add(name);
						}
					}
				}
			}
			return (string[]) results.ToArray(typeof(string));
		}

		
		public string GetImportedClassName(string name) 
		{
			string result = imports[name] as string;
			return (result==null) ? name : result;
		}

		public IDictionary GetAllClassMetadata() 
		{
			return classPersisters;
		}

		public IDictionary GetAllCollectionMetadata() 
		{
			 return collectionPersisters;
		}

		public void Close() 
		{
			log.Info("Closing");

			foreach(IClassPersister p in classPersisters.Values)
			{
				if ( p.HasCache ) p.Cache.Destroy();
			}
		
			foreach(CollectionPersister p in collectionPersisters.Values)
			{
				if ( p.HasCache ) p.CacheConcurrencyStrategy.Destroy();
			}

			//TODO: H2.0.3
			//if (statementCache!=null) statementCache.CloseAll();
			try 
			{
				connectionProvider.Close();
			}
			finally 
			{
				SessionFactoryObjectFactory.RemoveInstance(uuid, name, properties);
			} 
		}

		public void Evict(System.Type persistentClass, object id) 
		{
			IClassPersister p = GetPersister(persistentClass);
			if(p.HasCache) p.Cache.Release(id);
		}

		public void Evict(System.Type persistentClass) 
		{
			IClassPersister p = GetPersister(persistentClass);
			if(p.HasCache) p.Cache.Clear();
		}

		public void EvictCollection(string roleName, object id) 
		{
			CollectionPersister p = GetCollectionPersister(roleName);
			if(p.HasCache) p.CacheConcurrencyStrategy.Remove(id);
		}

		public void EvictCollection(string roleName) 
		{
			CollectionPersister p = GetCollectionPersister(roleName);
			if(p.HasCache) p.CacheConcurrencyStrategy.Clear();
		}
		
	}
}
