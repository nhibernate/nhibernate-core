using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="ICriteria"/> interface
	/// </summary>
	[Serializable]
	public class CriteriaImpl : ICriteria
	{
		private IList criteria = new ArrayList();
		private IList orderEntries = new ArrayList();
		private IDictionary fetchModes = new Hashtable();
		private IDictionary lockModes = new Hashtable();
		private int maxResults = RowSelection.NoValue;
		private int firstResult;
		private int timeout = RowSelection.NoValue;
		private int fetchSize = RowSelection.NoValue;
		private System.Type persistentClass;
		private ISessionImplementor session;
		private IResultTransformer resultTransformer = CriteriaSpecification.RootEntity;
		private bool cacheable;
		private string cacheRegion;
		private CacheMode? cacheMode;
		private CacheMode? sessionCacheMode;

		private IList subcriteriaList = new ArrayList();
		private string rootAlias;

		private IDictionary subcriteriaByPath = new Hashtable();
		private IDictionary subcriteriaByAlias = new Hashtable();
		private readonly string entityOrClassName;

		// Projection Fields
		private IProjection projection;
		private ICriteria projectionCriteria;

		[Serializable]
		public sealed class Subcriteria : ICriteria
		{
			// Added to simulate Java-style inner class
			private CriteriaImpl root;

			private ICriteria parent;
			private string alias;
			private string path;
			private LockMode lockMode;
			private JoinType joinType;

			internal Subcriteria(CriteriaImpl root, ICriteria parent, string path, string alias, JoinType joinType)
			{
				this.root = root;
				this.parent = parent;
				this.alias = alias;
				this.path = path;
				this.joinType = joinType;

				root.subcriteriaList.Add(this);

				root.subcriteriaByPath[path] = this;
				if (alias != null)
					root.subcriteriaByAlias[alias] = this;
			}

			internal Subcriteria(CriteriaImpl root, ICriteria parent, string path, JoinType joinType)
				: this(root, parent, path, null, joinType)
			{
			}

			public ICriteria Add(ICriterion expression)
			{
				root.Add(this, expression);
				return this;
			}

			public ICriteria CreateAlias(string associationPath, string alias)
			{
				return CreateAlias(associationPath, alias, JoinType.InnerJoin);
			}

			public ICriteria CreateAlias(string associationPath, string alias, JoinType joinType)
			{
				new Subcriteria(root, this, associationPath, alias, joinType);
				return this;
			}

			public ICriteria CreateCriteria(string associationPath)
			{
				return CreateCriteria(associationPath, JoinType.InnerJoin);
			}

			public ICriteria CreateCriteria(string associationPath, JoinType joinType)
			{
				return new Subcriteria(root, this, associationPath, joinType);
			}

			public ICriteria AddOrder(Order order)
			{
				root.orderEntries.Add(new OrderEntry(order, this));
				return this;
			}

			public IList List()
			{
				return root.List();
			}

			public void List(IList results)
			{
				root.List(results);
			}

			public IList<T> List<T>()
			{
				return root.List<T>();
			}

			public T UniqueResult<T>()
			{
				object result = UniqueResult();
				if (result == null && typeof(T).IsValueType)
				{
					throw new InvalidCastException("UniqueResult<T>() cannot cast null result to value type. Call UniqueResult<T?>() instead");
				}
				else
				{
					return (T)UniqueResult();
				}
			}

			public object UniqueResult()
			{
				return root.UniqueResult();
			}

			public ICriteria SetFetchMode(string associationPath, FetchMode mode)
			{
				root.SetFetchMode(StringHelper.Qualify(path, associationPath), mode);
				return this;
			}

			public ICriteria SetFirstResult(int firstResult)
			{
				root.SetFirstResult(firstResult);
				return this;
			}

			public ICriteria SetMaxResults(int maxResults)
			{
				root.SetMaxResults(maxResults);
				return this;
			}

			public ICriteria SetTimeout(int timeout)
			{
				root.SetTimeout(timeout);
				return this;
			}

			public ICriteria SetFetchSize(int fetchSize)
			{
				root.SetFetchSize(fetchSize);
				return this;
			}

			public ICriteria CreateCriteria(string associationPath, string alias)
			{
				return CreateCriteria(associationPath, alias, JoinType.InnerJoin);
			}

			public ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType)
			{
				return new Subcriteria(root, this, associationPath, alias, joinType);
			}

			// Deprecated methods not ported: ReturnMaps, ReturnRootEntities

			public LockMode LockMode
			{
				get { return lockMode; }
			}

			public ICriteria SetLockMode(LockMode lockMode)
			{
				this.lockMode = lockMode;
				return this;
			}

			public ICriteria SetLockMode(string alias, LockMode lockMode)
			{
				root.SetLockMode(alias, lockMode);
				return this;
			}

			public JoinType JoinType
			{
				get { return joinType; }
			}

			public string Alias
			{
				get { return alias; }
				set
				{
					root.subcriteriaByAlias.Remove(alias);
					alias = value;
					root.subcriteriaByAlias[alias] = this;
				}
			}

			public ICriteria Parent
			{
				get { return parent; }
			}

			public string Path
			{
				get { return path; }
			}

			public ICriteria SetResultTransformer(IResultTransformer resultProcessor)
			{
				root.SetResultTransformer(resultProcessor);
				return this;
			}

			public ICriteria SetCacheable(bool cacheable)
			{
				root.SetCacheable(cacheable);
				return this;
			}

			public ICriteria SetCacheRegion(string cacheRegion)
			{
				root.SetCacheRegion(cacheRegion);
				return this;
			}

			public ICriteria SetProjection(IProjection projection)
			{
				root.SetProjection(projection);
				return this;
			}

			public ISessionImplementor Session
			{
				get { return root.Session;}
			}

			public ICriteria GetCriteriaByPath(string path)
			{
				return root.GetCriteriaByPath(path);
			}

			public ICriteria GetCriteriaByAlias(string alias)
			{
				return root.GetCriteriaByAlias(alias);
			}

			/// <summary> Override the cache mode for this particular query. </summary>
			/// <param name="cacheMode">The cache mode to use. </param>
			/// <returns> this (for method chaining) </returns>
			public ICriteria SetCacheMode(CacheMode cacheMode)
			{
				root.SetCacheMode(cacheMode);
				return this;
			}

			public int MaxResults
			{
				get { return root.MaxResults; }
			}

			public int FirstResult
			{
				get { return root.FirstResult; }
			}

			public int Timeout
			{
				get { return root.Timeout; }
			}

			public int FetchSize
			{
				get { return root.FetchSize; }
			}

			public System.Type CriteriaClass
			{
				get { return root.CriteriaClass; }
			}

			public IDictionary LockModes
			{
				get { return root.LockModes; }
			}

			public IResultTransformer ResultTransformer
			{
				get { return root.ResultTransformer; }
			}

			public bool Cacheable
			{
				get { return root.Cacheable; }
			}

			public string CacheRegion
			{
				get { return root.CacheRegion; }
			}

			public IProjection Projection
			{
				get { return root.Projection; }
			}

			public ICriteria ProjectionCriteria
			{
				get { return root.ProjectionCriteria; }
			}

			public IList Restrictions
			{
				get { return root.Restrictions; }
			}

			public IList Orders
			{
				get { return root.Orders; }
			}

			public IDictionary FetchModes
			{
				get { return root.FetchModes; }
			}

			public IList SubcriteriaList
			{
				get { return root.SubcriteriaList; }
			}

			public string RootAlias
			{
				get { return root.RootAlias; }
			}
		}

		protected internal void Before()
		{
			if (cacheMode.HasValue)
			{
				sessionCacheMode = Session.CacheMode;
				Session.CacheMode = cacheMode.Value;
			}
		}

		protected internal void After()
		{
			if (sessionCacheMode.HasValue)
			{
				Session.CacheMode = sessionCacheMode.Value;
				sessionCacheMode = null;
			}
		}

		public ICriteria SetMaxResults(int maxResults)
		{
			this.maxResults = maxResults;
			return this;
		}

		public ICriteria SetFirstResult(int firstResult)
		{
			this.firstResult = firstResult;
			return this;
		}

		public ICriteria SetTimeout(int timeout)
		{
			this.timeout = timeout;
			return this;
		}

		public ICriteria SetFetchSize(int fetchSize)
		{
			this.fetchSize = fetchSize;
			return this;
		}

		public ICriteria Add(ICriterion expression)
		{
			Add(this, expression);
			return this;
		}

		public int MaxResults
		{
			get { return maxResults; }
		}

		public int FirstResult
		{
			get { return firstResult; }
		}

		public int Timeout
		{
			get { return timeout; }
		}

		public int FetchSize
		{
			get { return fetchSize; }
		}

		public CriteriaImpl(System.Type persistentClass, ISessionImplementor session)
			: this(persistentClass, CriteriaUtil.RootAlias, session)
		{
		}

		public CriteriaImpl(System.Type persistentClass, string alias, ISessionImplementor session)
		{
			// TODO H3.2 EntityName in constructor instead persistentClass
			this.persistentClass = persistentClass;
			this.session = session;
			this.cacheable = false;
			this.rootAlias = alias;
			cacheMode = null;
			entityOrClassName = persistentClass.FullName;
			subcriteriaByAlias[alias] = this;
		}

		public IList List()
		{
			ArrayList results = new ArrayList();
			List(results);
			return results;
		}

		public void List(IList results)
		{
			Before();
			try
			{
				session.List(this, results);
			}
			finally
			{
				After();
			}
		}

		public IList<T> List<T>()
		{
			List<T> results = new List<T>();
			List(results);
			return results;
		}

		public T UniqueResult<T>()
		{
			object result = UniqueResult();
			if (result == null && typeof(T).IsValueType)
			{
				return default(T);
			}
			else
			{
				return (T)result;
			}
		}

		public IEnumerable IterateExpressionEntries()
		{
			return criteria;
		}

		public IEnumerable IterateOrderings()
		{
			return orderEntries;
		}

		public IEnumerable IterateSubcriteria()
		{
			return subcriteriaList;
		}

		public override string ToString()
		{
			bool first = true;
			StringBuilder builder = new StringBuilder();
			foreach (CriterionEntry criterionEntry in criteria)
			{
				if (!first)
					builder.Append(" and ");
				builder.Append(criterionEntry.ToString());
				first = false;
			}
			if (orderEntries.Count != 0)
			{
				builder.Append(Environment.NewLine);
			}
			first = true;
			foreach (OrderEntry orderEntry in orderEntries)
			{
				if (!first)
					builder.Append(" and ");
				builder.Append(orderEntry.ToString());
				first = false;
			}
			return builder.ToString();
		}

		public ICriteria AddOrder(Order ordering)
		{
			orderEntries.Add(new OrderEntry(ordering, this));
			return this;
		}

		public FetchMode GetFetchMode(string path)
		{
			object result = fetchModes[path];
			return result == null ? FetchMode.Default : (FetchMode) result;
		}

		public ICriteria SetFetchMode(string associationPath, FetchMode mode)
		{
			fetchModes[associationPath] = mode;
			return this;
		}

		public ICriteria CreateAlias(string associationPath, string alias)
		{
			CreateAlias(associationPath, alias, JoinType.InnerJoin);
			return this;
		}

		public ICriteria CreateAlias(string associationPath, string alias, JoinType joinType)
		{
			new Subcriteria(this, this, associationPath, alias, joinType);
			return this;
		}

		public string Alias
		{
			get { return rootAlias; }
		}

		public ICriteria Add(ICriteria criteriaInst, ICriterion expression)
		{
			criteria.Add(new CriterionEntry(expression, criteriaInst));
			return this;
		}

		public ICriteria CreateCriteria(string associationPath)
		{
			return CreateCriteria(associationPath, JoinType.InnerJoin);
		}

		public ICriteria CreateCriteria(string associationPath, JoinType joinType)
		{
			return new Subcriteria(this, this, associationPath, joinType);
		}

		public ICriteria CreateCriteria(string associationPath, string alias)
		{
			return CreateCriteria(associationPath, alias, JoinType.InnerJoin);
		}

		public ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType)
		{
			return new Subcriteria(this, this, associationPath, alias, joinType);
		}

		public object UniqueResult()
		{
			return AbstractQueryImpl.UniqueElement(List());
		}

		public System.Type CriteriaClass
		{
			get { return persistentClass; }
		}

		// Deprecated methods not ported: ReturnMaps, ReturnRootEntities

		public ICriteria SetLockMode(LockMode lockMode)
		{
			return SetLockMode(CriteriaUtil.RootAlias, lockMode);
		}

		public ICriteria SetLockMode(string alias, LockMode lockMode)
		{
			lockModes[alias] = lockMode;
			return this;
		}

		public IDictionary LockModes
		{
			get { return lockModes; }
		}

		public string EntityOrClassName
		{
			get { return entityOrClassName; }
		}

		public IResultTransformer ResultTransformer
		{
			get { return resultTransformer; }
		}

		public ICriteria SetResultTransformer(IResultTransformer tupleMapper)
		{
			resultTransformer = tupleMapper;
			return this;
		}

		public bool Cacheable
		{
			get { return cacheable; }
		}

		public ISessionImplementor Session
		{
			get { return session; }
			set { session = value; }
		}

		public string CacheRegion
		{
			get { return cacheRegion; }
		}

		public ICriteria SetCacheable(bool cacheable)
		{
			this.cacheable = cacheable;
			return this;
		}

		public ICriteria SetCacheRegion(string cacheRegion)
		{
			this.cacheRegion = cacheRegion.Trim();
			return this;
		}

		public bool IsLookupByNaturalKey()
		{
			// TODO H3:
			//			if ( projection != null ) 
			//			{
			//				return false;
			//			}
			if (subcriteriaList.Count > 0)
			{
				return false;
			}
			if (criteria.Count != 1)
			{
				return false;
			}

			return false;
			//			CriterionEntry ce = (CriterionEntry) criteria[ 0 ];
			//			return ce.Criterion is NaturalIdentifier;
		}

		public IProjection Projection
		{
			get { return projection; }
		}

		public ICriteria SetProjection(IProjection projection)
		{
			this.projection = projection;
			projectionCriteria = this;
			SetResultTransformer(CriteriaSpecification.Projection);
			return this;
		}

		internal void SetProjectionCriteria(ICriteria projectionCriteria)
		{
			this.projectionCriteria = projectionCriteria;
		}

		public ICriteria GetCriteriaByPath(string path)
		{
			return (ICriteria) subcriteriaByPath[path];
		}

		public ICriteria GetCriteriaByAlias(string alias)
		{
			return (ICriteria) subcriteriaByAlias[alias];
		}

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		public ICriteria SetCacheMode(CacheMode cacheMode)
		{
			this.cacheMode = cacheMode;
			return this;
		}

		public ICriteria ProjectionCriteria
		{
			get { return projectionCriteria; }
		}

		[Serializable]
		public sealed class CriterionEntry
		{
			private readonly ICriterion criterion;
			private readonly ICriteria criteria;

			internal CriterionEntry(ICriterion criterion, ICriteria criteria)
			{
				this.criterion = criterion;
				this.criteria = criteria;
			}

			public ICriterion Criterion
			{
				get { return criterion; }
			}

			public ICriteria Criteria
			{
				get { return criteria; }
			}

			public override string ToString()
			{
				return criterion.ToString();
			}
		}

		[Serializable]
		public sealed class OrderEntry
		{
			private readonly Order order;
			private readonly ICriteria criteria;

			internal OrderEntry(Order order, ICriteria criteria)
			{
				this.order = order;
				this.criteria = criteria;
			}

			public Order Order
			{
				get { return order; }
			}

			public ICriteria Criteria
			{
				get { return criteria; }
			}

			public override string ToString()
			{
				return order.ToString();
			}
		}

		public IList Restrictions
		{
			get { return criteria; }
		}

		public IList Orders
		{
			get { return orderEntries; }
		}

		public IDictionary FetchModes
		{
			get { return fetchModes; }
		}

		public string RootAlias
		{
			get { return rootAlias; }
		}

		public IList SubcriteriaList
		{
			get { return subcriteriaList; }
		}
	}
}
