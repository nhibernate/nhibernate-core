using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Hql;
using NHibernate.Id;
using NHibernate.Loader.Criteria;
using NHibernate.Loader.Custom;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	[Serializable]
	public class StatelessSessionImpl : AbstractSessionImpl, IStatelessSession
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(StatelessSessionImpl));
		[NonSerialized]
		private readonly ConnectionManager connectionManager;
		[NonSerialized]
		private readonly StatefulPersistenceContext temporaryPersistenceContext;

		internal StatelessSessionImpl(IDbConnection connection, SessionFactoryImpl factory)
			: base(factory)
		{
			temporaryPersistenceContext = new StatefulPersistenceContext(this);
			connectionManager = new ConnectionManager(this, connection, ConnectionReleaseMode.AfterTransaction, new EmptyInterceptor());
		}

		public override void InitializeCollection(IPersistentCollection coolection, bool writing)
		{
			throw new SessionException("collections cannot be fetched by a stateless session");
		}

		public override object InternalLoad(string entityName, object id, bool eager, bool isNullable)
		{
			ErrorIfClosed();
			IEntityPersister persister = Factory.GetEntityPersister(entityName);
			if (!eager && persister.HasProxy)
			{
				return persister.CreateProxy(id, this);
			}
			object loaded = temporaryPersistenceContext.GetEntity(new EntityKey(id, persister, EntityMode.Poco));
			//TODO: if not loaded, throw an exception
			return loaded ?? Get(entityName, id);
		}

		public override object ImmediateLoad(string entityName, object id)
		{
			throw new SessionException("proxies cannot be fetched by a stateless session");
		}

		public override long Timestamp
		{
			get { throw new NotSupportedException(); }
		}

		public override IBatcher Batcher
		{
			get
			{
				ErrorIfClosed();
				return connectionManager.Batcher;
			}
		}

		public override IList List(string query, QueryParameters parameters)
		{
			// TODO pull up
			IList results = new ArrayList();
			List(query, parameters, results);
			return results;
		}

		public override void List(string query, QueryParameters queryParameters, IList results)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, false);
			bool success = false;
			try
			{
				plan.PerformList(queryParameters, this, results);
				success = true;
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception e)
			{
				throw Convert(e, "Could not execute query");
			}
			finally
			{
				AfterOperation(success);
			}
			temporaryPersistenceContext.Clear();
		}

		private void AfterOperation(bool success)
		{
			// TODO pull up
			if (!connectionManager.IsInActiveTransaction)
			{
				connectionManager.AfterNonTransactionalQuery(success);
			}
		}

		public override IList<T> List<T>(string query, QueryParameters queryParameters)
		{
			// TODO pull up
			List<T> results = new List<T>();
			List(query, queryParameters, results);
			return results;
		}

		public override IList<T> List<T>(CriteriaImpl criteria)
		{
			// TODO pull up
			List<T> results = new List<T>();
			List(criteria, results);
			return results;
		}

		public override void List(CriteriaImpl criteria, IList results)
		{
			ErrorIfClosed();
			string[] implementors = factory.GetImplementors(criteria.EntityOrClassName);
			int size = implementors.Length;

			CriteriaLoader[] loaders = new CriteriaLoader[size];
			for (int i = 0; i < size; i++)
			{
				loaders[i] = new CriteriaLoader(GetOuterJoinLoadable(implementors[i]), factory,
					criteria, implementors[i], EnabledFilters);
			}

			bool success = false;
			try
			{
				for (int i = size - 1; i >= 0; i--)
				{
					ArrayHelper.AddAll(results, loaders[i].List(this));
				}
				success = true;
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw Convert(sqle, "Unable to perform find");
			}
			finally
			{
				AfterOperation(success);
			}
			temporaryPersistenceContext.Clear();
		}

		private IOuterJoinLoadable GetOuterJoinLoadable(string entityName)
		{
			// TODO pull up
			IEntityPersister persister = factory.GetEntityPersister(entityName);
			if (!(persister is IOuterJoinLoadable))
			{
				throw new MappingException("class persister is not IOuterJoinLoadable: " + entityName);
			}
			return (IOuterJoinLoadable)persister;
		}

		public override IList List(CriteriaImpl criteria)
		{
			// TODO pull up
			ArrayList results = new ArrayList();
			List(criteria, results);
			return results;
		}

		public override IEnumerable Enumerable(string query, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters)
		{
			throw new NotSupportedException();
		}

		public override IList ListFilter(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters)
		{
			throw new NotSupportedException();
		}

		public override IEntityPersister GetEntityPersister(object obj)
		{
			ErrorIfClosed();
			return factory.GetEntityPersister(GuessEntityName(obj));
			//if (entityName == null)
			//{
			//  return factory.GetEntityPersister(GuessEntityName(obj));
			//}
			//else
			//{
			//  return factory.GetEntityPersister(entityName).GetSubclassEntityPersister(obj, Factory);
			//}
		}

		public override void AfterTransactionBegin(ITransaction tx)
		{
		}

		public override void BeforeTransactionCompletion(ITransaction tx)
		{
		}

		public override void AfterTransactionCompletion(bool successful, ITransaction tx)
		{
			connectionManager.AfterTransaction();
		}

		public override object GetContextEntityIdentifier(object obj)
		{
			ErrorIfClosed();
			return null;
		}

		public override bool IsSaved(object obj)
		{
			throw new NotImplementedException();
		}

		public override object Instantiate(string clazz, object id)
		{
			ErrorIfClosed();
			return Factory.GetEntityPersister(clazz).Instantiate(id, EntityMode.Poco);
		}

		public override IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			// TODO pull up
			ArrayList results = new ArrayList();
			List(spec, queryParameters, results);
			return results;
		}

		public override void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results)
		{
			// TODO pull up
			SQLCustomQuery query = new SQLCustomQuery(
				spec.SqlQueryReturns,
				spec.QueryString,
				spec.QuerySpaces,
				factory);
			ListCustomQuery(query, queryParameters, results);
		}

		public override IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters)
		{
			// TODO pull up
			List<T> results = new List<T>();
			List(spec, queryParameters, results);
			return results;
		}

		public override void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results)
		{
			ErrorIfClosed();

			CustomLoader loader = new CustomLoader(customQuery, factory);

			bool success = false;
			try
			{
				ArrayHelper.AddAll(results, loader.List(this, queryParameters));
				success = true;
			}
			finally
			{
				AfterOperation(success);
			}
			temporaryPersistenceContext.Clear();
		}

		public override IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters)
		{
			// TODO pull up
			List<T> results = new List<T>();
			ListCustomQuery(customQuery, queryParameters, results);
			return results;
		}

		public override object Copy(object obj, IDictionary copiedAlready)
		{
			throw new NotSupportedException();
		}

		public override object GetFilterParameterValue(string filterParameterName)
		{
			throw new NotSupportedException();
		}

		public override IType GetFilterParameterType(string filterParameterName)
		{
			throw new NotSupportedException();
		}

		public override IDictionary<string, IFilter> EnabledFilters
		{
			get { return new CollectionHelper.EmptyMapClass<string, IFilter>(); }
		}

		public override IQueryTranslator[] GetQueries(string query, bool scalar)
		{
			// take the union of the query spaces (ie the queried tables)
			IQueryTranslator[] q = factory.GetQuery(query, scalar, EnabledFilters);
			return q;
		}

		public override IInterceptor Interceptor
		{
			get { return new EmptyInterceptor(); }
		}

		public override EventListeners Listeners
		{
			get { throw new NotSupportedException(); }
		}

		public override int DontFlushFromFind
		{
			get { return 0; }
		}

		public override ConnectionManager ConnectionManager
		{
			get { return connectionManager; }
		}

		public override bool IsEventSource
		{
			get { return false; }
		}

		public override object GetEntityUsingInterceptor(EntityKey key)
		{
			ErrorIfClosed();
			return null;
		}

		public override IPersistenceContext PersistenceContext
		{
			get { return temporaryPersistenceContext; }
		}

		public override bool IsOpen
		{
			get { return !IsClosed; }
		}

		public override bool IsConnected
		{
			get { return connectionManager.IsConnected; }
		}

		public override FlushMode FlushMode
		{
			get { return FlushMode.Commit; }
			set { throw new NotSupportedException(); }
		}

		public override string BestGuessEntityName(object entity)
		{
			INHibernateProxy proxy = entity as INHibernateProxy;
			if (proxy != null)
			{
				entity = proxy.HibernateLazyInitializer.GetImplementation();
			}
			return GuessEntityName(entity);
		}

		public override string GuessEntityName(object entity)
		{
			ErrorIfClosed();
			return entity.GetType().FullName;
		}

		public override IDbConnection Connection
		{
			get { return connectionManager.GetConnection(); }
		}

		public override void Flush()
		{
			ManagedFlush(); // NH Different behavior since ADOContext.Context is not implemented
		}

		public void ManagedFlush()
		{
			ErrorIfClosed();
			Batcher.ExecuteBatch();
		}

		public override bool TransactionInProgress
		{
			get { return Transaction.IsActive; }
		}

		#region IStatelessSession Members

		/// <summary> Get the current Hibernate transaction.</summary>
		public ITransaction Transaction
		{
			get { return connectionManager.Transaction; }
		}

		public override CacheMode CacheMode
		{
			get { return CacheMode.Ignore; }
			set { throw new NotSupportedException(); }
		}

		public override EntityMode EntityMode
		{
			get { return NHibernate.EntityMode.Poco; }
		}

		public override string FetchProfile
		{
			get { return null; }
			set { }
		}

		/// <summary> Close the stateless session and release the ADO.NET connection.</summary>
		public void Close()
		{
			ManagedClose();
		}

		public void ManagedClose()
		{
			if (IsClosed)
			{
				throw new SessionException("Session was already closed!");
			}
			ConnectionManager.Close();
			SetClosed();
		}

		/// <summary> Insert a entity.</summary>
		/// <param name="entity">A new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		public object Insert(object entity)
		{
			ErrorIfClosed();
			return Insert(null, entity);
		}

		/// <summary> Insert a row. </summary>
		/// <param name="entityName">The entityName for the entity to be inserted </param>
		/// <param name="entity">a new transient instance </param>
		/// <returns> the identifier of the instance </returns>
		public object Insert(string entityName, object entity)
		{
			ErrorIfClosed();
			IEntityPersister persister = GetEntityPersister(entity);
			object id = persister.IdentifierGenerator.Generate(this, entity);
			object[] state = persister.GetPropertyValues(entity, EntityMode.Poco);
			if (persister.IsVersioned)
			{
				object versionValue = state[persister.VersionProperty];
				bool substitute = Versioning.SeedVersion(state, persister.VersionProperty, persister.VersionType, persister.IsUnsavedVersion(versionValue), this);
				if (substitute)
				{
					persister.SetPropertyValues(entity, state, EntityMode.Poco);
				}
			}
			if (id == IdentifierGeneratorFactory.PostInsertIndicator)
			{
				id = persister.Insert(state, entity, this);
			}
			else
			{
				persister.Insert(id, state, entity, this);
			}
			persister.SetIdentifier(entity, id, EntityMode.Poco);
			return id;
		}

		/// <summary> Update a entity.</summary>
		/// <param name="entity">a detached entity instance </param>
		public void Update(object entity)
		{
			ErrorIfClosed();
			Update(null, entity);
		}

		/// <summary>Update a entity.</summary>
		/// <param name="entityName">The entityName for the entity to be updated </param>
		/// <param name="entity">a detached entity instance </param>
		public void Update(string entityName, object entity)
		{
			ErrorIfClosed();
			IEntityPersister persister = GetEntityPersister(entity);
			object id = persister.GetIdentifier(entity, EntityMode.Poco);
			object[] state = persister.GetPropertyValues(entity, EntityMode.Poco);
			object oldVersion;
			if (persister.IsVersioned)
			{
				oldVersion = persister.GetVersion(entity, EntityMode.Poco);
				object newVersion = Versioning.Increment(oldVersion, persister.VersionType, this);
				Versioning.SetVersion(state, newVersion, persister);
				persister.SetPropertyValues(entity, state, EntityMode.Poco);
			}
			else
			{
				oldVersion = null;
			}
			persister.Update(id, state, null, false, null, oldVersion, entity, null, this);
		}

		/// <summary> Delete a entity. </summary>
		/// <param name="entity">a detached entity instance </param>
		public void Delete(object entity)
		{
			ErrorIfClosed();
			Delete(null, entity);
		}

		/// <summary> Delete a entity. </summary>
		/// <param name="entityName">The entityName for the entity to be deleted </param>
		/// <param name="entity">a detached entity instance </param>
		public void Delete(string entityName, object entity)
		{
			ErrorIfClosed();
			IEntityPersister persister = GetEntityPersister(entity);
			object id = persister.GetIdentifier(entity, EntityMode.Poco);
			object version = persister.GetVersion(entity, EntityMode.Poco);
			persister.Delete(id, version, entity, this);
		}

		/// <summary> Retrieve a entity. </summary>
		/// <returns> a detached entity instance </returns>
		public object Get(string entityName, object id)
		{
			return Get(entityName, id, LockMode.None);
		}

		/// <summary> Retrieve a entity.
		/// 
		/// </summary>
		/// <returns> a detached entity instance
		/// </returns>
		public object Get<T>(object id)
		{
			return Get(typeof(T), id);
		}

		private object Get(System.Type persistentClass, object id)
		{
			return Get(persistentClass.FullName, id);
		}

		/// <summary> 
		/// Retrieve a entity, obtaining the specified lock mode. 
		/// </summary>
		/// <returns> a detached entity instance </returns>
		public object Get(string entityName, object id, LockMode lockMode)
		{
			ErrorIfClosed();
			object result = Factory.GetEntityPersister(entityName).Load(id, null, lockMode, this);
			temporaryPersistenceContext.Clear();
			return result;
		}

		/// <summary> 
		/// Retrieve a entity, obtaining the specified lock mode. 
		/// </summary>
		/// <returns> a detached entity instance </returns>
		public object Get<T>(object id, LockMode lockMode)
		{
			return Get(typeof(T).FullName, id, lockMode);
		}

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		public void Refresh(object entity)
		{
			Refresh(BestGuessEntityName(entity), entity, LockMode.None);
		}

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed.</param>
		public void Refresh(string entityName, object entity)
		{
			Refresh(entityName, entity, LockMode.None);
		}

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied.</param>
		public void Refresh(object entity, LockMode lockMode)
		{
			Refresh(BestGuessEntityName(entity), entity, lockMode);
		}

		/// <summary> 
		/// Refresh the entity instance state from the database. 
		/// </summary>
		/// <param name="entityName">The entityName for the entity to be refreshed. </param>
		/// <param name="entity">The entity to be refreshed. </param>
		/// <param name="lockMode">The LockMode to be applied. </param>
		public void Refresh(string entityName, object entity, LockMode lockMode)
		{
			IEntityPersister persister = GetEntityPersister(entity);
			object id = persister.GetIdentifier(entity, EntityMode);
			if (log.IsDebugEnabled)
			{
				log.Debug("refreshing transient " + MessageHelper.InfoString(persister, id, Factory));
			}
			//from H3.2 TODO : can this ever happen???
			//		EntityKey key = new EntityKey( id, persister, source.getEntityMode() );
			//		if ( source.getPersistenceContext().getEntry( key ) != null ) {
			//			throw new PersistentObjectException(
			//					"attempted to refresh transient instance when persistent " +
			//					"instance was already associated with the Session: " +
			//					MessageHelper.infoString( persister, id, source.getFactory() )
			//			);
			//		}

			if (persister.HasCache)
			{
				CacheKey ck = new CacheKey(id, persister.IdentifierType, persister.RootEntityName, EntityMode, Factory);
				persister.Cache.Remove(ck);
			}

			object result = persister.Load(id, entity, lockMode, this);
			UnresolvableObjectException.ThrowIfNull(result, id, persister.EntityName);
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class. 
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria<T>()
		{
			ErrorIfClosed();
			return new CriteriaImpl(typeof(T), this);
		}

		/// <summary>
		/// Create a new <see cref="ICriteria"/> instance, for the given entity class,
		/// or a superclass of an entity class, with the given alias. 
		/// </summary>
		/// <typeparam name="T">A class, which is persistent, or has persistent subclasses</typeparam>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria<T>(string alias)
		{
			ErrorIfClosed();
			return new CriteriaImpl(typeof(T), alias, this);
		}

		/// <summary> 
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name.
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria(string entityName)
		{
			throw new NotImplementedException();
		}

		/// <summary> 
		/// Create a new <see cref="ICriteria"/> instance, for the given entity name,
		/// with the given alias.  
		/// </summary>
		/// <param name="entityName">The entity name. </param>
		/// <param name="alias">The alias of the entity</param>
		/// <returns> The <see cref="ICriteria"/>. </returns>
		/// <remarks>Entities returned by the query are detached.</remarks>
		public ICriteria CreateCriteria(string entityName, string alias)
		{
			throw new NotImplementedException();
		}

		/// <summary> Begin a NHibernate transaction.</summary>
		public ITransaction BeginTransaction()
		{
			ErrorIfClosed();
			return connectionManager.BeginTransaction();
		}

		#endregion

		#region IDisposable Members
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~StatelessSessionImpl()
		{
			Dispose(false);
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			log.Debug("running IStatelessSession.Dispose()");
			Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the session if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				Close();
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}
		#endregion

		public override ISession GetSession()
		{
			// TODO: Verify the use of this method in NH.Search and remove it
			throw new NotSupportedException();
		}

		public override int ExecuteNativeUpdate(NativeSQLQuerySpecification nativeSQLQuerySpecification, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			NativeSQLQueryPlan plan = GetNativeSQLQueryPlan(nativeSQLQuerySpecification);

			bool success = false;
			int result;
			try
			{
				result = plan.PerformExecuteUpdate(queryParameters, this);
				success = true;
			}
			finally
			{
				AfterOperation(success);
			}
			temporaryPersistenceContext.Clear();
			return result;
		}

		public override int ExecuteUpdate(string query, QueryParameters queryParameters)
		{
			ErrorIfClosed();
			queryParameters.ValidateParameters();
			HQLQueryPlan plan = GetHQLQueryPlan(query, false);
			bool success = false;
			int result;
			try
			{
				result = plan.PerformExecuteUpdate(queryParameters, this);
				success = true;
			}
			finally
			{
				AfterOperation(success);
			}
			temporaryPersistenceContext.Clear();
			return result;
		}

		public override IEntityPersister GetEntityPersister(string entityName, object obj)
		{
			ErrorIfClosed();
			if (entityName == null)
			{
				return factory.GetEntityPersister(GuessEntityName(obj));
			}
			else
			{
				return factory.GetEntityPersister(entityName).GetSubclassEntityPersister(obj, Factory, EntityMode.Poco);
			}
		}
	}
}
