using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Loader.Custom;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary> Functionality common to stateless and stateful sessions </summary>
	[Serializable]
	public abstract class AbstractSessionImpl : ISessionImplementor
	{
		[NonSerialized]
		protected internal SessionFactoryImpl factory;
		private bool closed = false;

		internal AbstractSessionImpl() {}

		protected internal AbstractSessionImpl(SessionFactoryImpl factory)
		{
			this.factory = factory;
		}

		#region ISessionImplementor Members

		public abstract void InitializeCollection(IPersistentCollection coolection, bool writing);
		public abstract object InternalLoad(string entityName, object id, bool eager, bool isNullable);
		public abstract object ImmediateLoad(string entityName, object id);
		public abstract long Timestamp { get; }

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}
		public abstract EntityMode EntityMode { get;}

		public abstract IBatcher Batcher { get; }

		public abstract IList List(string query, QueryParameters parameters);
		public abstract void List(string query, QueryParameters parameters, IList results);
		public abstract IList<T> List<T>(string query, QueryParameters queryParameters);
		public abstract IList<T> List<T>(CriteriaImpl criteria);
		public abstract void List(CriteriaImpl criteria, IList results);
		public abstract IList List(CriteriaImpl criteria);
		public abstract IEnumerable Enumerable(string query, QueryParameters parameters);
		public abstract IEnumerable<T> Enumerable<T>(string query, QueryParameters queryParameters);
		public abstract IList ListFilter(object collection, string filter, QueryParameters parameters);
		public abstract IList<T> ListFilter<T>(object collection, string filter, QueryParameters parameters);
		public abstract IEnumerable EnumerableFilter(object collection, string filter, QueryParameters parameters);
		public abstract IEnumerable<T> EnumerableFilter<T>(object collection, string filter, QueryParameters parameters);
		public abstract IEntityPersister GetEntityPersister(object obj);
		public abstract void AfterTransactionBegin(ITransaction tx);
		public abstract void BeforeTransactionCompletion(ITransaction tx);
		public abstract void AfterTransactionCompletion(bool successful, ITransaction tx);
		public abstract object GetContextEntityIdentifier(object obj);
		public abstract object GetEntityIdentifierIfNotUnsaved(object obj);
		public abstract bool IsSaved(object obj);
		public abstract object Instantiate(System.Type clazz, object id);
		public abstract IList List(NativeSQLQuerySpecification spec, QueryParameters queryParameters);
		public abstract void List(NativeSQLQuerySpecification spec, QueryParameters queryParameters, IList results);
		public abstract IList<T> List<T>(NativeSQLQuerySpecification spec, QueryParameters queryParameters);
		public abstract void ListCustomQuery(ICustomQuery customQuery, QueryParameters queryParameters, IList results);
		public abstract IList<T> ListCustomQuery<T>(ICustomQuery customQuery, QueryParameters queryParameters);
		public abstract object Copy(object obj, IDictionary copiedAlready);
		public abstract object GetFilterParameterValue(string filterParameterName);
		public abstract IType GetFilterParameterType(string filterParameterName);
		public abstract ISession GetSession();
		public abstract IDictionary<string, IFilter> EnabledFilters { get; }

		public virtual IQuery GetNamedSQLQuery(string name)
		{
			ErrorIfClosed();
			NamedSQLQueryDefinition nsqlqd = factory.GetNamedSQLQuery(name);
			if (nsqlqd == null)
			{
				throw new MappingException("Named SQL query not known: " + name);
			}
			IQuery query = new SqlQueryImpl(nsqlqd, this, factory.QueryPlanCache.GetSQLParameterMetadata(nsqlqd.QueryString));
			//query.SetComment("named native SQL query " + name);
			InitQuery(query, nsqlqd);
			return query;
		}

		public abstract IQueryTranslator[] GetQueries(string query, bool scalar);
		public abstract IInterceptor Interceptor { get; }
		public abstract EventListeners Listeners { get; }
		public abstract int DontFlushFromFind { get; }
		public abstract ConnectionManager ConnectionManager { get; }
		public abstract bool IsEventSource { get; }
		public abstract object GetEntityUsingInterceptor(EntityKey key);
		public abstract IPersistenceContext PersistenceContext { get; }
		public abstract CacheMode CacheMode { get; set; }
		public abstract bool IsOpen { get; }
		public abstract bool IsConnected { get; }
		public abstract FlushMode FlushMode { get; set; }
		public abstract string BestGuessEntityName(object entity);
		public abstract string GuessEntityName(object entity);
		public abstract IDbConnection Connection { get; }

		public virtual IQuery GetNamedQuery(string queryName)
		{
			ErrorIfClosed();
			NamedQueryDefinition nqd = factory.GetNamedQuery(queryName);
			IQuery query;
			if (nqd != null)
			{
				string queryString = nqd.QueryString;
				query = new QueryImpl(queryString, nqd.FlushMode, this, GetHQLQueryPlan(queryString, false).ParameterMetadata);
				//query.SetComment("named HQL query " + queryName);
			}
			else
			{
				NamedSQLQueryDefinition nsqlqd = factory.GetNamedSQLQuery(queryName);
				if (nsqlqd == null)
				{
					throw new MappingException("Named query not known: " + queryName);
				}
				query = new SqlQueryImpl(nsqlqd, this, factory.QueryPlanCache.GetSQLParameterMetadata(nsqlqd.QueryString));
				//query.SetComment("named native SQL query " + queryName);
				nqd = nsqlqd;
			}
			InitQuery(query, nqd);
			return query;
		}

		public bool IsClosed
		{
			get { return closed; }
		}

		protected internal virtual void ErrorIfClosed()
		{
			if (closed)
			{
				throw new SessionException("Session is closed!");
			}
		}

		public abstract void Flush();

		public abstract bool TransactionInProgress { get; }

		#endregion

		protected internal void SetClosed()
		{
			closed = true;
		}

		private void InitQuery(IQuery query, NamedQueryDefinition nqd)
		{
			query.SetCacheable(nqd.IsCacheable);
			query.SetCacheRegion(nqd.CacheRegion);
			if (nqd.Timeout != -1)
			{
				query.SetTimeout(nqd.Timeout);
			}
			//if (nqd.FetchSize != -1)
			//{
			//	query.SetFetchSize(nqd.FetchSize);
			//}
			if (nqd.CacheMode.HasValue) 
				query.SetCacheMode(nqd.CacheMode.Value);

			query.SetReadOnly(nqd.IsReadOnly);
			//if (nqd.Comment != null)
			//{
			//	query.SetComment(nqd.Comment);
			//}
		}

		public virtual IQuery CreateQuery(string queryString)
		{
			ErrorIfClosed();
			QueryImpl query = new QueryImpl(queryString, this, GetHQLQueryPlan(queryString, false).ParameterMetadata);
			//query.SetComment(queryString);
			return query;
		}

		public virtual ISQLQuery CreateSQLQuery(string sql)
		{
			ErrorIfClosed();
			SqlQueryImpl query = new SqlQueryImpl(sql, this, factory.QueryPlanCache.GetSQLParameterMetadata(sql));
			//query.SetComment("dynamic native SQL query");
			return query;
		}

		protected internal virtual HQLQueryPlan GetHQLQueryPlan(string query, bool shallow)
		{
			return factory.QueryPlanCache.GetHQLQueryPlan(query, shallow, EnabledFilters);
		}

		protected internal virtual NativeSQLQueryPlan GetNativeSQLQueryPlan(NativeSQLQuerySpecification spec)
		{
			return factory.QueryPlanCache.GetNativeSQLQueryPlan(spec);
		}

		protected static ADOException Convert(Exception sqlException, string message)
		{
			return ADOExceptionHelper.Convert( /*Factory.SQLExceptionConverter,*/ sqlException, message);
		}

	}
}
