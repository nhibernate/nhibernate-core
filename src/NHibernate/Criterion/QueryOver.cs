
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion.Lambda;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	[Serializable]
	public class QueryOver
	{

		protected ICriteria criteria;
		protected CriteriaImpl impl;

		protected QueryOver() { }

		public static QueryOver<T,T> Of<T>()
		{
			return new QueryOver<T,T>();
		}

		public static QueryOver<T,T> Of<T>(Expression<Func<T>> alias)
		{
			return new QueryOver<T,T>(alias);
		}

		public ICriteria UnderlyingCriteria
		{
			get { return criteria; }
		}

		public DetachedCriteria DetachedCriteria
		{
			get { return new DetachedCriteria(impl, impl); }
		}

	}

	[Serializable]
	public class QueryOver<TRoot> : QueryOver, IQueryOver<TRoot>
	{

		private IList<TRoot> List()
		{
			return criteria.List<TRoot>();
		}

		private IList<U> List<U>()
		{
			return criteria.List<U>();
		}

		private TRoot UniqueResult()
		{
			return criteria.UniqueResult<TRoot>();
		}

		private U UniqueResult<U>()
		{
			return criteria.UniqueResult<U>();
		}

		private IEnumerable<TRoot> Future()
		{
			return criteria.Future<TRoot>();
		}

		private IEnumerable<U> Future<U>()
		{
			return criteria.Future<U>();
		}

		private IFutureValue<TRoot> FutureValue()
		{
			return criteria.FutureValue<TRoot>();
		}

		private IFutureValue<U> FutureValue<U>()
		{
			return criteria.FutureValue<U>();
		}

		/// <summary>
		/// Get an executable instance of <c>IQueryOver&lt;TRoot&gt;</c>,
		/// to actually run the query.</summary>
		public IQueryOver<TRoot,TRoot> GetExecutableQueryOver(ISession session)
		{
			impl.Session = session.GetSessionImplementation();
			return new QueryOver<TRoot,TRoot>(impl);
		}

		/// <summary>
		/// Get an executable instance of <c>IQueryOver&lt;TRoot&gt;</c>,
		/// to actually run the query.</summary>
		public IQueryOver<TRoot,TRoot> GetExecutableQueryOver(IStatelessSession session)
		{
			impl.Session = (ISessionImplementor)session;
			return new QueryOver<TRoot,TRoot>(impl);
		}

		/// <summary>
		/// Method to allow comparison of detached query in Lambda expression
		/// e.g., p =&gt; p.Name == myQuery.As&lt;string&gt;
		/// </summary>
		/// <typeparam name="S">type returned (projected) by query</typeparam>
		/// <returns>throws an exception if evaluated directly at runtime.</returns>
		public S As<S>()
		{
			throw new HibernateException("Incorrect syntax;  .As<T> method is for use in Lambda expressions only.");
		}


		ICriteria IQueryOver<TRoot>.UnderlyingCriteria
		{ get { return UnderlyingCriteria; } }

		IList<TRoot> IQueryOver<TRoot>.List()
		{ return List(); }

		IList<U> IQueryOver<TRoot>.List<U>()
		{ return List<U>(); }

		TRoot IQueryOver<TRoot>.UniqueResult()
		{ return UniqueResult(); }

		U IQueryOver<TRoot>.UniqueResult<U>()
		{ return UniqueResult<U>(); }

		IEnumerable<TRoot> IQueryOver<TRoot>.Future()
		{ return Future(); }

		IEnumerable<U> IQueryOver<TRoot>.Future<U>()
		{ return Future<U>(); }

		IFutureValue<TRoot> IQueryOver<TRoot>.FutureValue()
		{ return FutureValue(); }

		IFutureValue<U> IQueryOver<TRoot>.FutureValue<U>()
		{ return FutureValue<U>(); }

	}

	/// <summary>
	/// Implementation of the <see cref="IQueryOver&lt;TRoot, TSubType&gt;"/> interface
	/// </summary>
	[Serializable]
	public class QueryOver<TRoot,TSubType> : QueryOver<TRoot>, IQueryOver<TRoot,TSubType>
	{

		protected internal QueryOver()
		{
			impl = new CriteriaImpl(typeof(TRoot), null);
			criteria = impl;
		}

		protected internal QueryOver(Expression<Func<TSubType>> alias)
		{
			string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
			impl = new CriteriaImpl(typeof(TRoot), aliasPath, null);
			criteria = impl;
		}

		protected internal QueryOver(CriteriaImpl impl)
		{
			this.impl = impl;
			this.criteria = impl;
		}

		protected internal QueryOver(CriteriaImpl rootImpl, ICriteria criteria)
		{
			this.impl = rootImpl;
			this.criteria = criteria;
		}

		public QueryOver<TRoot,TSubType> And(Expression<Func<TSubType, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> And(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> And(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> AndNot(Expression<Func<TSubType, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<TRoot,TSubType> AndNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<TSubType, object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<TRoot,TSubType> Where(Expression<Func<TSubType, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> Where(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> Where(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<TRoot,TSubType> WhereNot(Expression<Func<TSubType, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<TRoot,TSubType> WhereNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<TSubType, object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<TRoot,TSubType> Select(params Expression<Func<TRoot, object>>[] projections)
		{
			List<IProjection> projectionList = new List<IProjection>();

			foreach (var projection in projections)
				projectionList.Add(Projections.Property(ExpressionProcessor.FindMemberExpression(projection.Body)));

			criteria.SetProjection(projectionList.ToArray());
			return this;
		}

		public QueryOver<TRoot,TSubType> Select(params IProjection[] projections)
		{
			criteria.SetProjection(projections);
			return this;
		}

		QueryOverProjectionBuilder<QueryOver<TRoot,TSubType>, TRoot, TSubType> SelectList
		{
			get { return new QueryOverProjectionBuilder<QueryOver<TRoot,TSubType>, TRoot, TSubType>(this, this); }
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<TSubType, object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<TSubType, object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOver<TRoot,TSubType> Skip(int firstResult)
		{
			criteria.SetFirstResult(firstResult);
			return this;
		}

		public QueryOver<TRoot,TSubType> Take(int maxResults)
		{
			criteria.SetMaxResults(maxResults);
			return this;
		}

		public QueryOver<TRoot,TSubType> Cacheable()
		{
			criteria.SetCacheable(true);
			return this;
		}

		public QueryOver<TRoot,TSubType> CacheMode(CacheMode cacheMode)
		{
			criteria.SetCacheMode(cacheMode);
			return this;
		}

		public QueryOver<TRoot,TSubType> CacheRegion(string cacheRegion)
		{
			criteria.SetCacheRegion(cacheRegion);
			return this;
		}

		public QueryOverSubqueryBuilder<TRoot,TSubType> WithSubquery
		{
			get { return new QueryOverSubqueryBuilder<TRoot,TSubType>(this); }
		}

		public QueryOverFetchBuilder<TRoot,TSubType> Fetch(Expression<Func<TRoot, object>> path)
		{
			return new QueryOverFetchBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverLockBuilder<TRoot,TSubType> Lock()
		{
			return new QueryOverLockBuilder<TRoot,TSubType>(this, null);
		}

		public QueryOverLockBuilder<TRoot,TSubType> Lock(Expression<Func<object>> alias)
		{
			return new QueryOverLockBuilder<TRoot,TSubType>(this, alias);
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<TRoot,TSubType> JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<TRoot,TSubType> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<TRoot,TSubType> JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOver<TRoot,TSubType> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOverJoinBuilder<TRoot,TSubType> Inner
		{
			get { return new QueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.InnerJoin); }
		}

		public QueryOverJoinBuilder<TRoot,TSubType> Left
		{
			get { return new QueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.LeftOuterJoin); }
		}

		public QueryOverJoinBuilder<TRoot,TSubType> Right
		{
			get { return new QueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.RightOuterJoin); }
		}

		public QueryOverJoinBuilder<TRoot,TSubType> Full
		{
			get { return new QueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.FullJoin); }
		}

		private QueryOver<TRoot,TSubType> AddAlias(string path, string alias, JoinType joinType)
		{
			criteria.CreateAlias(path, alias, joinType);
			return this;
		}

		private QueryOver<TRoot,TSubType> Add(Expression<Func<TSubType, bool>> expression)
		{
			criteria.Add(ExpressionProcessor.ProcessExpression<TSubType>(expression));
			return this;
		}

		private QueryOver<TRoot,TSubType> Add(Expression<Func<bool>> expression)
		{
			criteria.Add(ExpressionProcessor.ProcessExpression(expression));
			return this;
		}

		private QueryOver<TRoot,TSubType> Add(ICriterion expression)
		{
			criteria.Add(expression);
			return this;
		}

		private QueryOver<TRoot,TSubType> AddNot(Expression<Func<TSubType, bool>> expression)
		{
			criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression<TSubType>(expression)));
			return this;
		}

		private QueryOver<TRoot,TSubType> AddNot(Expression<Func<bool>> expression)
		{
			criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression(expression)));
			return this;
		}


		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.And(Expression<Func<TSubType, bool>> expression)
		{ return And(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.And(Expression<Func<bool>> expression)
		{ return And(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.And(ICriterion expression)
		{ return And(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndNot(Expression<Func<TSubType, bool>> expression)
		{ return AndNot(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndNot(Expression<Func<bool>> expression)
		{ return AndNot(expression); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndRestrictionOn(Expression<Func<TSubType, object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Where(Expression<Func<TSubType, bool>> expression)
		{ return Where(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Where(Expression<Func<bool>> expression)
		{ return Where(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Where(ICriterion expression)
		{ return Where(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereNot(Expression<Func<TSubType, bool>> expression)
		{ return WhereNot(expression); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereNot(Expression<Func<bool>> expression)
		{ return WhereNot(expression); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereRestrictionOn(Expression<Func<TSubType, object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Select(params Expression<Func<TRoot, object>>[] projections)
		{ return Select(projections); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Select(params IProjection[] projections)
		{ return Select(projections); }

		QueryOverProjectionBuilder<IQueryOver<TRoot,TSubType>, TRoot, TSubType> IQueryOver<TRoot,TSubType>.SelectList
		{ get { return new QueryOverProjectionBuilder<IQueryOver<TRoot,TSubType>,TRoot,TSubType>(this, this); } }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderBy(Expression<Func<TSubType, object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenBy(Expression<Func<TSubType, object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Skip(int firstResult)
		{ return Skip(firstResult); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Take(int maxResults)
		{ return Take(maxResults); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Cacheable()
		{ return Cacheable(); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.CacheMode(CacheMode cacheMode)
		{ return CacheMode(cacheMode); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.CacheRegion(string cacheRegion)
		{ return CacheRegion(cacheRegion); }

		IQueryOverSubqueryBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WithSubquery
		{ get { return new IQueryOverSubqueryBuilder<TRoot,TSubType>(this); } }

		IQueryOverFetchBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Fetch(Expression<Func<TRoot, object>> path)
		{ return new IQueryOverFetchBuilder<TRoot,TSubType>(this, path); }

		IQueryOverLockBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Lock()
		{ return new IQueryOverLockBuilder<TRoot,TSubType>(this, null); }

		IQueryOverLockBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Lock(Expression<Func<object>> alias)
		{ return new IQueryOverLockBuilder<TRoot,TSubType>(this, alias); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOverJoinBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Inner
		{ get { return new IQueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.InnerJoin); } }

		IQueryOverJoinBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Left
		{ get { return new IQueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.LeftOuterJoin); } }

		IQueryOverJoinBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Right
		{ get { return new IQueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.RightOuterJoin); } }

		IQueryOverJoinBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Full
		{ get { return new IQueryOverJoinBuilder<TRoot,TSubType>(this, JoinType.FullJoin); } }

	}

}
