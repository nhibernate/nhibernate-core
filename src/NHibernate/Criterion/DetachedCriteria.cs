using System;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Some applications need to create criteria queries in "detached
	/// mode", where the Hibernate session is not available. This class
	/// may be instantiated anywhere, and then a <c>ICriteria</c>
	/// may be obtained by passing a session to 
	/// <c>GetExecutableCriteria()</c>. All methods have the
	/// same semantics and behavior as the corresponding methods of the
	/// <c>ICriteria</c> interface.
	/// </summary>
	[Serializable]
	public class DetachedCriteria
	{
		private readonly CriteriaImpl impl;
		private readonly ICriteria criteria;

		protected DetachedCriteria(System.Type entityType)
		{
			impl = new CriteriaImpl(entityType, null);
			criteria = impl;
		}

		protected DetachedCriteria(System.Type entityType, string alias)
		{
			impl = new CriteriaImpl(entityType, alias, null);
			criteria = impl;
		}

		protected DetachedCriteria(string entityName)
		{
			impl = new CriteriaImpl(entityName, null);
			criteria = impl;
		}

		protected DetachedCriteria(string entityName, string alias)
		{
			impl = new CriteriaImpl(entityName, alias, null);
			criteria = impl;
		}

		protected internal DetachedCriteria(CriteriaImpl impl, ICriteria criteria)
		{
			this.impl = impl;
			this.criteria = criteria;
		}

		internal DetachedCriteria(CriteriaImpl impl)
		{
			this.impl = impl;
			criteria = impl;
		}

		/// <summary>
		/// Get an executable instance of <c>Criteria</c>,
		/// to actually run the query.</summary>
		public ICriteria GetExecutableCriteria(ISession session)
		{
			impl.Session = session.GetSessionImplementation();
			return impl;
		}

		/// <summary>
		/// Get an executable instance of <c>Criteria</c>,
		/// to actually run the query.</summary>
		public ICriteria GetExecutableCriteria(IStatelessSession session)
		{
			impl.Session = session.GetSessionImplementation();
			return impl;
		}

		public static DetachedCriteria For(System.Type entityType)
		{
			return new DetachedCriteria(entityType);
		}

		public static DetachedCriteria For<T>()
		{
			return new DetachedCriteria(typeof(T));
		}

		public static DetachedCriteria For<T>(string alias)
		{
			return new DetachedCriteria(typeof(T), alias);
		}

		public static DetachedCriteria For(System.Type entityType, string alias)
		{
			return new DetachedCriteria(entityType, alias);
		}

		public static DetachedCriteria ForEntityName(string entityName)
		{
			return new DetachedCriteria(entityName);
		}

		public static DetachedCriteria ForEntityName(string entityName, string alias)
		{
			return new DetachedCriteria(entityName, alias);
		}

		public DetachedCriteria Add(ICriterion criterion)
		{
			criteria.Add(criterion);
			return this;
		}

		public DetachedCriteria AddOrder(Order order)
		{
			criteria.AddOrder(order);
			return this;
		}

		public DetachedCriteria CreateAlias(string associationPath, string alias)
		{
			criteria.CreateAlias(associationPath, alias);
			return this;
		}

		public DetachedCriteria CreateAlias(string associationPath, string alias, JoinType joinType)
		{
			criteria.CreateAlias(associationPath, alias, joinType);
			return this;
		}

		public DetachedCriteria CreateAlias(string associationPath, string alias, JoinType joinType, ICriterion withClause)
		{
			criteria.CreateAlias(associationPath, alias, joinType, withClause);
			return this;
		}

		public DetachedCriteria CreateCriteria(string associationPath, string alias)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath, alias));
		}

		public DetachedCriteria CreateCriteria(string associationPath)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath));
		}

		public DetachedCriteria CreateCriteria(string associationPath, JoinType joinType)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath, joinType));
		}

		public DetachedCriteria CreateCriteria(string associationPath, string alias, JoinType joinType)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath, alias, joinType));
		}

		public DetachedCriteria CreateCriteria(string associationPath, string alias, JoinType joinType, ICriterion withClause)
		{
			return new DetachedCriteria(impl, criteria.CreateCriteria(associationPath, alias, joinType, withClause));
		}

		public string Alias
		{
			get { return criteria.Alias; }
		}

		public string EntityOrClassName
		{
			get { return impl.EntityOrClassName; }
		}

		protected internal CriteriaImpl GetCriteriaImpl()
		{
			return impl;
		}

		public DetachedCriteria SetFetchMode(string associationPath, FetchMode mode)
		{
			criteria.SetFetchMode(associationPath, mode);
			return this;
		}

		public DetachedCriteria SetLockMode(LockMode lockMode)
		{
			criteria.SetLockMode(lockMode);
			return this;
		}

		public DetachedCriteria SetLockMode(string alias, LockMode lockMode)
		{
			criteria.SetLockMode(alias, lockMode);
			return this;
		}

		public DetachedCriteria SetCacheMode(CacheMode cacheMode)
		{
			criteria.SetCacheMode(cacheMode);
			return this;
		}

		public DetachedCriteria SetCacheRegion(string region)
		{
			criteria.SetCacheRegion(region);
			return this;
		}

		public DetachedCriteria SetCacheable(bool cacheable)
		{
			criteria.SetCacheable(cacheable);
			return this;
		}

		public DetachedCriteria SetProjection(IProjection projection)
		{
			criteria.SetProjection(projection);
			return this;
		}

		public DetachedCriteria SetResultTransformer(IResultTransformer resultTransformer)
		{
			criteria.SetResultTransformer(resultTransformer);
			return this;
		}

		public DetachedCriteria SetFirstResult(int firstResult)
		{
			criteria.SetFirstResult(firstResult);
			return this;
		}

		public DetachedCriteria SetMaxResults(int maxResults)
		{
			criteria.SetMaxResults(maxResults);
			return this;
		}

		public override string ToString()
		{
			return string.Format("DetachableCriteria({0})", criteria);
		}

		public DetachedCriteria GetCriteriaByPath(string path)
		{
			ICriteria tmpCrit = criteria.GetCriteriaByPath(path);
			if (tmpCrit == null)
			{
				return null;
			}
			return new DetachedCriteria(impl, tmpCrit);
		}

		public DetachedCriteria GetCriteriaByAlias(string alias)
		{
			ICriteria tmpCrit = criteria.GetCriteriaByAlias(alias);
			if (tmpCrit == null)
			{
				return null;
			}
			return new DetachedCriteria(impl, tmpCrit);
		}

		/// <summary>
		/// Gets the root entity type if available, throws otherwise
		/// </summary>
		/// <remarks>
		/// This is an NHibernate specific method, used by several dependent 
		/// frameworks for advance integration with NHibernate.
		/// </remarks>
		public System.Type GetRootEntityTypeIfAvailable()
		{
			return criteria.GetRootEntityTypeIfAvailable();
		}

		/// <summary>
		/// Clear all orders from criteria.
		/// </summary>
		public void ClearOrders()
		{
			criteria.ClearOrders();
		}
	}
}