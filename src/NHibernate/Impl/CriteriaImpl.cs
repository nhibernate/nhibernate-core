using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="ICriteria"/> interface
	/// </summary>
	[Serializable]
	public class CriteriaImpl : ICriteria
	{
		private readonly System.Type persistentClass;
		private readonly List<CriterionEntry> criteria = new List<CriterionEntry>();
		private readonly List<OrderEntry> orderEntries = new List<OrderEntry>(10);
		private readonly Dictionary<string, FetchMode> fetchModes = new Dictionary<string, FetchMode>();
		private readonly Dictionary<string, LockMode> lockModes = new Dictionary<string, LockMode>();
		private int maxResults = RowSelection.NoValue;
		private int firstResult;
		private int timeout = RowSelection.NoValue;
		private int fetchSize = RowSelection.NoValue;
		private ISessionImplementor session;
		private IResultTransformer resultTransformer = CriteriaSpecification.RootEntity;
		private bool cacheable;
		private string cacheRegion;
		private CacheMode? cacheMode;
		private CacheMode? sessionCacheMode;
		private string comment;
		private FlushMode? flushMode;
		private FlushMode? sessionFlushMode;
		private bool? readOnly;

		private readonly List<Subcriteria> subcriteriaList = new List<Subcriteria>();
		private readonly string rootAlias;

		private readonly Dictionary<string, ICriteria> subcriteriaByPath = new Dictionary<string, ICriteria>();
		private readonly Dictionary<string, ICriteria> subcriteriaByAlias = new Dictionary<string, ICriteria>();
		private readonly string entityOrClassName;

		// Projection Fields
		private IProjection projection;
		private ICriteria projectionCriteria;

		public CriteriaImpl(System.Type persistentClass, ISessionImplementor session)
			: this(persistentClass.FullName, CriteriaSpecification.RootAlias, session)
		{
			this.persistentClass = persistentClass;
		}

		public CriteriaImpl(System.Type persistentClass, string alias, ISessionImplementor session)
			: this(persistentClass.FullName, alias, session)
		{
			this.persistentClass = persistentClass;
		}

		public CriteriaImpl(string entityOrClassName, ISessionImplementor session)
			: this(entityOrClassName, CriteriaSpecification.RootAlias, session) {}

		public CriteriaImpl(string entityOrClassName, string alias, ISessionImplementor session)
		{
			this.session = session;
			this.entityOrClassName = entityOrClassName;
			cacheable = false;
			rootAlias = alias;
			subcriteriaByAlias[alias] = this;
		}

		public ISessionImplementor Session
		{
			get { return session; }
			set { session = value; }
		}

		public string EntityOrClassName
		{
			get { return entityOrClassName; }
		}

		public IDictionary<string, LockMode> LockModes
		{
			get { return lockModes; }
		}

		public ICriteria ProjectionCriteria
		{
			get { return projectionCriteria; }
		}

		public bool LookupByNaturalKey
		{
			get
			{
				if (projection != null)
				{
					return false;
				}
				if (subcriteriaList.Count > 0)
				{
					return false;
				}
				if (criteria.Count != 1)
				{
					return false;
				}

				CriterionEntry ce = criteria[0];
				return ce.Criterion is NaturalIdentifier;
			}
		}

		public string Alias
		{
			get { return rootAlias; }
		}

		public IProjection Projection
		{
			get { return projection; }
		}
		
		/// <inheritdoc />
		public bool IsReadOnlyInitialized
		{
			get { return (readOnly != null); }
		}
		
		/// <inheritdoc />
		public bool IsReadOnly
		{
			get
			{
				if (!IsReadOnlyInitialized && (Session == null))
					throw new InvalidOperationException("cannot determine readOnly/modifiable setting when it is not initialized and is not initialized and Session == null");

				return IsReadOnlyInitialized ? readOnly.Value : Session.PersistenceContext.DefaultReadOnly;
			}
		}

		public FetchMode GetFetchMode(string path)
		{
			FetchMode result;
			if (!fetchModes.TryGetValue(path, out result))
			{
				result = FetchMode.Default;
			}
			return result;
		}

		public IResultTransformer ResultTransformer
		{
			get { return resultTransformer; }
		}

		public int MaxResults
		{
			get { return maxResults; }
		}

		public int FirstResult
		{
			get { return firstResult; }
		}

		public int FetchSize
		{
			get { return fetchSize; }
		}

		public int Timeout
		{
			get { return timeout; }
		}

		public bool Cacheable
		{
			get { return cacheable; }
		}

		public string CacheRegion
		{
			get { return cacheRegion; }
		}

		public string Comment
		{
			get { return comment; }
		}

		protected internal void Before()
		{
			if (flushMode.HasValue)
			{
				sessionFlushMode = Session.FlushMode;
				Session.FlushMode = flushMode.Value;
			}
			if (cacheMode.HasValue)
			{
				sessionCacheMode = Session.CacheMode;
				Session.CacheMode = cacheMode.Value;
			}
		}

		protected internal void After()
		{
			if (sessionFlushMode.HasValue)
			{
				Session.FlushMode = sessionFlushMode.Value;
				sessionFlushMode = null;
			}
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

		public IList List()
		{
			var results = new List<object>();
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
			if (result == null && typeof (T).IsValueType)
			{
				return default(T);
			}
			else
			{
				return (T) result;
			}
		}

		public void ClearOrders()
		{
			orderEntries.Clear();
		}

		public IEnumerable<CriterionEntry> IterateExpressionEntries()
		{
			return criteria;
		}

		public IEnumerable<OrderEntry> IterateOrderings()
		{
			return orderEntries;
		}

		public IEnumerable<Subcriteria> IterateSubcriteria()
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
				{
					builder.Append(" and ");
				}
				builder.Append(criterionEntry.ToString());
				first = false;
			}
			if (orderEntries.Count != 0)
			{
				builder.AppendLine();
			}
			first = true;
			foreach (OrderEntry orderEntry in orderEntries)
			{
				if (!first)
				{
					builder.Append(" and ");
				}
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

		public ICriteria CreateAlias(string associationPath, string alias, JoinType joinType, ICriterion withClause)
		{
			new Subcriteria(this, this, associationPath, alias, joinType, withClause);
			return this;
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

		public ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType, ICriterion withClause)
		{
			return new Subcriteria(this, this, associationPath, alias, joinType, withClause);
		}

		public IFutureValue<T> FutureValue<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new FutureValue<T>(List<T>);
			}

			session.FutureCriteriaBatch.Add<T>(this);
			return session.FutureCriteriaBatch.GetFutureValue<T>();
		}

		public IEnumerable<T> Future<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return List<T>();
			}

			session.FutureCriteriaBatch.Add<T>(this);
			return session.FutureCriteriaBatch.GetEnumerator<T>();
		}

#if ASYNC
		public IFutureValueAsync<T> FutureValueAsync<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new FutureValueAsync<T>(async () => await ListAsync<T>());
			}

			session.FutureCriteriaBatch.Add<T>(this);
			return session.FutureCriteriaBatch.GetFutureValueAsync<T>();
		}

		public IAsyncEnumerable<T> FutureAsync<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new DelayedAsyncEnumerator<T>(async () => await ListAsync<T>());
			}

			session.FutureCriteriaBatch.Add<T>(this);
			return session.FutureCriteriaBatch.GetAsyncEnumerator<T>();
		}
#endif

		public object UniqueResult()
		{
			return AbstractQueryImpl.UniqueElement(List());
		}

		public ICriteria SetLockMode(LockMode lockMode)
		{
			return SetLockMode(CriteriaSpecification.RootAlias, lockMode);
		}

		public ICriteria SetLockMode(string alias, LockMode lockMode)
		{
			lockModes[alias] = lockMode;
			return this;
		}

		public ICriteria SetResultTransformer(IResultTransformer tupleMapper)
		{
			resultTransformer = tupleMapper;
			return this;
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

		public ICriteria SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		public ICriteria SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
			return this;
		}

		public ICriteria SetProjection(params IProjection[] projections)
		{
			if(projections==null)
				throw new ArgumentNullException("projections");
			if(projections.Length ==0)
				throw new ArgumentException("projections must contain a least one projection");

			if(projections.Length==1)
			{
				projection = projections[0];
			}
			else
			{
				var projectionList = new ProjectionList();
				foreach (var childProjection in projections)
				{
					projectionList.Add(childProjection);
				}
				projection = projectionList;
			}

			if (projection != null)
			{
				projectionCriteria = this;
				SetResultTransformer(CriteriaSpecification.Projection);
			}

			return this;
		}

		/// <inheritdoc />
		public ICriteria SetReadOnly(bool readOnly)
		{
			this.readOnly = readOnly;
			return this;
		}

		/// <summary> Override the cache mode for this particular query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		public ICriteria SetCacheMode(CacheMode cacheMode)
		{
			this.cacheMode = cacheMode;
			return this;
		}

		public object Clone()
		{
			CriteriaImpl clone;
			if (persistentClass != null)
			{
				clone = new CriteriaImpl(persistentClass, Alias, Session);
			}
			else
			{
				clone = new CriteriaImpl(entityOrClassName, Alias, Session);
			}
			CloneSubcriteria(clone);
			foreach (KeyValuePair<string, FetchMode> de in fetchModes)
			{
				clone.fetchModes.Add(de.Key, de.Value);
			}
			foreach (KeyValuePair<string, LockMode> de in lockModes)
			{
				clone.lockModes.Add(de.Key, de.Value);
			}
			clone.maxResults = maxResults;
			clone.firstResult = firstResult;
			clone.timeout = timeout;
			clone.fetchSize = fetchSize;
			clone.cacheable = cacheable;
			clone.cacheRegion = cacheRegion;
			clone.SetProjection(projection);
			CloneProjectCrtieria(clone);
			clone.SetResultTransformer(resultTransformer);
			clone.comment = comment;
			if (flushMode.HasValue)
			{
				clone.SetFlushMode(flushMode.Value);
			}
			if (cacheMode.HasValue)
			{
				clone.SetCacheMode(cacheMode.Value);
			}
			return clone;
		}

		private void CloneProjectCrtieria(CriteriaImpl clone)
		{
			if (projectionCriteria != null)
			{
				if (projectionCriteria == this)
				{
					clone.projectionCriteria = clone;
				}
				else
				{
					ICriteria clonedProjectionCriteria = (ICriteria) projectionCriteria.Clone();
					clone.projectionCriteria = clonedProjectionCriteria;
				}
			}
		}

		private void CloneSubcriteria(CriteriaImpl clone)
		{
			//we need to preserve the parent criteria, we rely on the ordering when creating the
			//subcriterias initially here, so we don't need to make more than a single pass
			Dictionary<ICriteria, ICriteria> newParents = new Dictionary<ICriteria, ICriteria>();
			newParents[this] = clone;

			foreach (Subcriteria subcriteria in IterateSubcriteria())
			{
				ICriteria currentParent;
				if (!newParents.TryGetValue(subcriteria.Parent, out currentParent))
				{
					throw new AssertionFailure(
						"Could not find parent for subcriteria in the previous subcriteria. If you see this error, it is a bug");
				}
				Subcriteria clonedSubCriteria =
					new Subcriteria(clone, currentParent, subcriteria.Path, subcriteria.Alias, subcriteria.JoinType, subcriteria.WithClause);
				clonedSubCriteria.SetLockMode(subcriteria.LockMode);
				newParents[subcriteria] = clonedSubCriteria;
			}

			// remap the orders
			foreach (OrderEntry orderEntry in IterateOrderings())
			{
				ICriteria currentParent;
				if (!newParents.TryGetValue(orderEntry.Criteria, out currentParent))
				{
					throw new AssertionFailure(
						"Could not find parent for order in the previous criteria. If you see this error, it is a bug");
				}
				currentParent.AddOrder(orderEntry.Order);
			}

			// remap the restrictions to appropriate criterias
			foreach (CriterionEntry criterionEntry in criteria)
			{
				ICriteria currentParent;
				if (!newParents.TryGetValue(criterionEntry.Criteria, out currentParent))
				{
					throw new AssertionFailure(
						"Could not find parent for restriction in the previous criteria. If you see this error, it is a bug.");
				}

				currentParent.Add(criterionEntry.Criterion);
			}
		}

		public ICriteria GetCriteriaByPath(string path)
		{
			ICriteria result;
			subcriteriaByPath.TryGetValue(path, out result);
			return result;
		}

		public ICriteria GetCriteriaByAlias(string alias)
		{
			ICriteria result;
			subcriteriaByAlias.TryGetValue(alias, out result);
			return result;
		}

		[Serializable]
		public sealed class Subcriteria : ICriteria
		{
			// Added to simulate Java-style inner class
			private readonly CriteriaImpl root;

			private readonly ICriteria parent;
			private string alias;
			private readonly string path;
			private LockMode lockMode;
			private readonly JoinType joinType;
			private ICriterion withClause;

			internal Subcriteria(CriteriaImpl root, ICriteria parent, string path, string alias, JoinType joinType, ICriterion withClause)
			{
				this.root = root;
				this.parent = parent;
				this.alias = alias;
				this.path = path;
				this.joinType = joinType;
				this.withClause = withClause;

				root.subcriteriaList.Add(this);

				root.subcriteriaByPath[path] = this;
				SetAlias(alias);
			}

			internal Subcriteria(CriteriaImpl root, ICriteria parent, string path, string alias, JoinType joinType)
				: this(root, parent, path, alias, joinType, null) {}

			internal Subcriteria(CriteriaImpl root, ICriteria parent, string path, JoinType joinType)
				: this(root, parent, path, null, joinType) { }

			public ICriterion WithClause
			{
				get { return withClause; }
			}

			public string Path
			{
				get { return path; }
			}

			public ICriteria Parent
			{
				get { return parent; }
			}

			public JoinType JoinType
			{
				get { return joinType; }
			}

			public string Alias
			{
				get { return alias; }
				set { SetAlias(value); }
			}

			public LockMode LockMode
			{
				get { return lockMode; }
			}

			public bool IsReadOnlyInitialized
			{
				get { return root.IsReadOnlyInitialized; }
			}
			
			public bool IsReadOnly
			{
				get { return root.IsReadOnly; }
			}
				
			public ICriteria SetLockMode(LockMode lockMode)
			{
				this.lockMode = lockMode;
				return this;
			}

			public ICriteria Add(ICriterion expression)
			{
				root.Add(this, expression);
				return this;
			}

			public ICriteria AddOrder(Order order)
			{
				root.orderEntries.Add(new OrderEntry(order, this));
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

			public ICriteria CreateAlias(string associationPath, string alias, JoinType joinType, ICriterion withClause)
			{
				new Subcriteria(root, this, associationPath, alias, joinType, withClause);
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

			public ICriteria CreateCriteria(string associationPath, string alias)
			{
				return CreateCriteria(associationPath, alias, JoinType.InnerJoin);
			}

			public ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType)
			{
				return new Subcriteria(root, this, associationPath, alias, joinType);
			}

			public ICriteria CreateCriteria(string associationPath, string alias, JoinType joinType, ICriterion withClause)
			{
				return new Subcriteria(root, this, associationPath, alias, joinType, withClause);
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

			public IList List()
			{
				return root.List();
			}

			public IFutureValue<T> FutureValue<T>()
			{
				return root.FutureValue<T>();
			}

			public IEnumerable<T> Future<T>()
			{
				return root.Future<T>();
			}

#if ASYNC
			public IFutureValueAsync<T> FutureValueAsync<T>()
			{
				return root.FutureValueAsync<T>();
			}

			public IAsyncEnumerable<T> FutureAsync<T>()
			{
				return root.FutureAsync<T>();
			}
#endif

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
				if (result == null && typeof (T).IsValueType)
				{
					throw new InvalidCastException(
						"UniqueResult<T>() cannot cast null result to value type. Call UniqueResult<T?>() instead");
				}
				else
				{
					return (T) result;
				}
			}

			public void ClearOrders()
			{
				root.ClearOrders();
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

			public ICriteria SetFlushMode(FlushMode flushMode)
			{
				root.SetFlushMode(flushMode);
				return this;
			}

			/// <summary> Override the cache mode for this particular query. </summary>
			/// <param name="cacheMode">The cache mode to use. </param>
			/// <returns> this (for method chaining) </returns>
			public ICriteria SetCacheMode(CacheMode cacheMode)
			{
				root.SetCacheMode(cacheMode);
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

			public ICriteria SetLockMode(string alias, LockMode lockMode)
			{
				root.SetLockMode(alias, lockMode);
				return this;
			}

			public ICriteria SetResultTransformer(IResultTransformer resultProcessor)
			{
				root.SetResultTransformer(resultProcessor);
				return this;
			}

			public ICriteria SetComment(string comment)
			{
				root.SetComment(comment);
				return this;
			}

			public ICriteria SetProjection(params IProjection[] projections)
			{
				root.SetProjection(projections);
				return this;
			}
			
			public ICriteria SetReadOnly(bool readOnly)
			{
				root.SetReadOnly(readOnly);
				return this;
			}
			
			public ICriteria GetCriteriaByPath(string path)
			{
				return root.GetCriteriaByPath(path);
			}

			public ICriteria GetCriteriaByAlias(string alias)
			{
				return root.GetCriteriaByAlias(alias);
			}

			public System.Type GetRootEntityTypeIfAvailable()
			{
				return root.GetRootEntityTypeIfAvailable();
			}

			/// <summary>
			/// The Clone is supported only by a root criteria.
			/// </summary>
			/// <returns>The clone of the root criteria.</returns>
			public object Clone()
			{
				// implemented only for compatibility with CriteriaTransformer
				return root.Clone();
			}

			private void SetAlias(string newAlias)
			{
				if (alias != null)
				{
					root.subcriteriaByAlias.Remove(alias);
				}
				if (newAlias != null)
				{
					root.subcriteriaByAlias[newAlias] = this;
				}
				alias = newAlias;
			}
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

		public System.Type GetRootEntityTypeIfAvailable()
		{
			if (persistentClass != null)
				return persistentClass;
			throw new HibernateException("Cannot provide root entity type because this criteria was initialized with an entity name.");
		}
	}
}
