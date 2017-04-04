
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion.Lambda;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Criterion
{

	[Serializable]
	public abstract class QueryOver
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

		public static QueryOver<T, T> Of<T>(string entityName)
		{
			return new QueryOver<T, T>(entityName);
		}

		public static QueryOver<T, T> Of<T>(string entityName, Expression<Func<T>> alias)
		{
			return new QueryOver<T, T>(entityName, alias);
		}

		public ICriteria UnderlyingCriteria
		{
			get { return criteria; }
		}

		public ICriteria RootCriteria
		{
			get { return impl; }
		}

		public DetachedCriteria DetachedCriteria
		{
			get { return new DetachedCriteria(impl, impl); }
		}

	}

	[Serializable]
	public abstract class QueryOver<TRoot> : QueryOver, IQueryOver<TRoot>
	{

		private IList<TRoot> List()
		{
			return criteria.List<TRoot>();
		}

		private IList<U> List<U>()
		{
			return criteria.List<U>();
		}

		private TRoot SingleOrDefault()
		{
			return criteria.UniqueResult<TRoot>();
		}

		private U SingleOrDefault<U>()
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

		private IAsyncEnumerable<TRoot> FutureAsync()
		{
			return criteria.FutureAsync<TRoot>();
		}

		private IAsyncEnumerable<U> FutureAsync<U>()
		{
			return criteria.FutureAsync<U>();
		}

		private IFutureValueAsync<TRoot> FutureValueAsync()
		{
			return criteria.FutureValueAsync<TRoot>();
		}

		private IFutureValueAsync<U> FutureValueAsync<U>()
		{
			return criteria.FutureValueAsync<U>();
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
		/// Clones the QueryOver, clears the orders and paging, and projects the RowCount
		/// </summary>
		/// <returns></returns>
		public QueryOver<TRoot,TRoot> ToRowCountQuery()
		{
			return (QueryOver<TRoot,TRoot>)
				Clone()
					.Select(Projections.RowCount())
					.ClearOrders()
					.Skip(0)
					.Take(RowSelection.NoValue);
		}

		/// <summary>
		/// Clones the QueryOver, clears the orders and paging, and projects the RowCount (Int64)
		/// </summary>
		/// <returns></returns>
		public QueryOver<TRoot,TRoot> ToRowCountInt64Query()
		{
			return (QueryOver<TRoot,TRoot>)
				Clone()
					.Select(Projections.RowCountInt64())
					.ClearOrders()
					.Skip(0)
					.Take(RowSelection.NoValue);
		}

		/// <summary>
		/// Creates an exact clone of the QueryOver
		/// </summary>
		public QueryOver<TRoot,TRoot> Clone()
		{
			return new QueryOver<TRoot,TRoot>((CriteriaImpl)criteria.Clone());
		}

		public QueryOver<TRoot> ClearOrders()
		{
			criteria.ClearOrders();
			return this;
		}

		public QueryOver<TRoot> Skip(int firstResult)
		{
			criteria.SetFirstResult(firstResult);
			return this;
		}

		public QueryOver<TRoot> Take(int maxResults)
		{
			criteria.SetMaxResults(maxResults);
			return this;
		}

		public QueryOver<TRoot> Cacheable()
		{
			criteria.SetCacheable(true);
			return this;
		}

		public QueryOver<TRoot> CacheMode(CacheMode cacheMode)
		{
			criteria.SetCacheMode(cacheMode);
			return this;
		}

		public QueryOver<TRoot> CacheRegion(string cacheRegion)
		{
			criteria.SetCacheRegion(cacheRegion);
			return this;
		}

		private QueryOver<TRoot> ReadOnly()
		{
			criteria.SetReadOnly(true);
			return this;
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


		IList<TRoot> IQueryOver<TRoot>.List()
		{ return List(); }

		IList<U> IQueryOver<TRoot>.List<U>()
		{ return List<U>(); }

		IQueryOver<TRoot,TRoot> IQueryOver<TRoot>.ToRowCountQuery()
		{ return ToRowCountQuery(); }

		IQueryOver<TRoot,TRoot> IQueryOver<TRoot>.ToRowCountInt64Query()
		{ return ToRowCountInt64Query(); }

		int IQueryOver<TRoot>.RowCount()
		{ return ToRowCountQuery().SingleOrDefault<int>(); }

		long IQueryOver<TRoot>.RowCountInt64()
		{ return ToRowCountInt64Query().SingleOrDefault<long>(); }

		TRoot IQueryOver<TRoot>.SingleOrDefault()
		{ return SingleOrDefault(); }

		U IQueryOver<TRoot>.SingleOrDefault<U>()
		{ return SingleOrDefault<U>(); }

		IEnumerable<TRoot> IQueryOver<TRoot>.Future()
		{ return Future(); }

		IEnumerable<U> IQueryOver<TRoot>.Future<U>()
		{ return Future<U>(); }

		IFutureValue<TRoot> IQueryOver<TRoot>.FutureValue()
		{ return FutureValue(); }

		IFutureValue<U> IQueryOver<TRoot>.FutureValue<U>()
		{ return FutureValue<U>(); }

		IAsyncEnumerable<TRoot> IQueryOver<TRoot>.FutureAsync()
		{ return FutureAsync(); }

		IAsyncEnumerable<U> IQueryOver<TRoot>.FutureAsync<U>()
		{ return FutureAsync<U>(); }

		IFutureValueAsync<TRoot> IQueryOver<TRoot>.FutureValueAsync()
		{ return FutureValueAsync(); }

		IFutureValueAsync<U> IQueryOver<TRoot>.FutureValueAsync<U>()
		{ return FutureValueAsync<U>(); }

		IQueryOver<TRoot,TRoot> IQueryOver<TRoot>.Clone()
		{ return Clone(); }

		IQueryOver<TRoot> IQueryOver<TRoot>.ClearOrders()
		{ return ClearOrders(); }

		IQueryOver<TRoot> IQueryOver<TRoot>.Skip(int firstResult)
		{ return Skip(firstResult); }

		IQueryOver<TRoot> IQueryOver<TRoot>.Take(int maxResults)
		{ return Take(maxResults); }

		IQueryOver<TRoot> IQueryOver<TRoot>.Cacheable()
		{ return Cacheable(); }

		IQueryOver<TRoot> IQueryOver<TRoot>.CacheMode(CacheMode cacheMode)
		{ return CacheMode(cacheMode); }

		IQueryOver<TRoot> IQueryOver<TRoot>.CacheRegion(string cacheRegion)
		{ return CacheRegion(cacheRegion); }

		IQueryOver<TRoot> IQueryOver<TRoot>.ReadOnly()
		{ return ReadOnly(); }

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

		protected internal QueryOver(string entityName)
		{
			impl = new CriteriaImpl(entityName, null);
			criteria = impl;
		}

		protected internal QueryOver(Expression<Func<TSubType>> alias)
		{
			string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
			impl = new CriteriaImpl(typeof(TRoot), aliasPath, null);
			criteria = impl;
		}

		protected internal QueryOver(string entityName, Expression<Func<TSubType>> alias)
		{
			string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
			impl = new CriteriaImpl(entityName, aliasPath, null);
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

		public QueryOver<TRoot, TSubType> AndNot(ICriterion expression)
		{
			return AddNot(expression);
		}
		
		public QueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<TSubType, object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body));
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> AndRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body));
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

		public QueryOver<TRoot, TSubType> WhereNot(ICriterion expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<TSubType, object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body));
		}

		public QueryOverRestrictionBuilder<TRoot,TSubType> WhereRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body));
		}

		public QueryOver<TRoot,TSubType> Select(params Expression<Func<TRoot, object>>[] projections)
		{
			List<IProjection> projectionList = new List<IProjection>();

			foreach (var projection in projections)
				projectionList.Add(ExpressionProcessor.FindMemberProjection(projection.Body).AsProjection());

			criteria.SetProjection(projectionList.ToArray());
			return this;
		}

		public QueryOver<TRoot,TSubType> Select(params IProjection[] projections)
		{
			criteria.SetProjection(projections);
			return this;
		}

		public QueryOver<TRoot, TSubType> SelectList(Func<QueryOverProjectionBuilder<TRoot>, QueryOverProjectionBuilder<TRoot>> list)
		{
			criteria.SetProjection(list(new QueryOverProjectionBuilder<TRoot>()).ProjectionList);
			return this;
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<TSubType, object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path, false);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderBy(IProjection projection)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, ExpressionProcessor.ProjectionInfo.ForProjection(projection));
		}

		public QueryOverOrderBuilder<TRoot,TSubType> OrderByAlias(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path, true);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<TSubType, object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path, false);
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenBy(IProjection projection)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, ExpressionProcessor.ProjectionInfo.ForProjection(projection));
		}

		public QueryOverOrderBuilder<TRoot,TSubType> ThenByAlias(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<TRoot,TSubType>(this, path, true);
		}

		public QueryOver<TRoot,TSubType> TransformUsing(IResultTransformer resultTransformer)
		{
			criteria.SetResultTransformer(resultTransformer);
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

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType,
					withClause));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType,
					withClause));
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

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType,
					withClause));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<TRoot,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return new QueryOver<TRoot,U>(impl,
				criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType,
					withClause));
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

		public QueryOver<TRoot,TSubType> JoinAlias<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType,
				withClause);
		}

		public QueryOver<TRoot,TSubType> JoinAlias<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType,
				withClause);
		}

		public QueryOver<TRoot,TSubType> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOver<TRoot,TSubType> JoinAlias<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType,
				withClause);
		}

		public QueryOver<TRoot,TSubType> JoinAlias<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType,
				withClause);
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

		private QueryOver<TRoot,TSubType> AddAlias(string path, string alias, JoinType joinType, ICriterion withClause)
		{
			criteria.CreateAlias(path, alias, joinType, withClause);
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

		private QueryOver<TRoot, TSubType> AddNot(ICriterion expression)
		{
			criteria.Add(Restrictions.Not(expression));
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

		IQueryOver<TRoot, TSubType> IQueryOver<TRoot, TSubType>.AndNot(Expression<Func<bool>> expression)
		{ return AndNot(expression); }

		IQueryOver<TRoot, TSubType> IQueryOver<TRoot, TSubType>.AndNot(ICriterion expression)
		{ return AndNot(expression); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndRestrictionOn(Expression<Func<TSubType, object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body)); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.AndRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body)); }

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

		IQueryOver<TRoot, TSubType> IQueryOver<TRoot, TSubType>.WhereNot(ICriterion expression)
		{ return WhereNot(expression); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereRestrictionOn(Expression<Func<TSubType, object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body)); }

		IQueryOverRestrictionBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.WhereRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<TRoot,TSubType>(this, ExpressionProcessor.FindMemberProjection(expression.Body)); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Select(params Expression<Func<TRoot, object>>[] projections)
		{ return Select(projections); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.Select(params IProjection[] projections)
		{ return Select(projections); }

		IQueryOver<TRoot, TSubType> IQueryOver<TRoot, TSubType>.SelectList(Func<QueryOverProjectionBuilder<TRoot>, QueryOverProjectionBuilder<TRoot>> list)
		{ return SelectList(list); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderBy(Expression<Func<TSubType, object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path, false); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderBy(IProjection projection)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, ExpressionProcessor.ProjectionInfo.ForProjection(projection)); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.OrderByAlias(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path, true); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenBy(Expression<Func<TSubType, object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path, false); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenBy(IProjection projection)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, ExpressionProcessor.ProjectionInfo.ForProjection(projection)); }

		IQueryOverOrderBuilder<TRoot,TSubType> IQueryOver<TRoot,TSubType>.ThenByAlias(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<TRoot,TSubType>(this, path, true); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.TransformUsing(IResultTransformer resultTransformer)
		{ return TransformUsing(resultTransformer); }

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

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinQueryOver(path, alias, joinType, withClause); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinQueryOver(path, alias, joinType, withClause); }

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

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinQueryOver(path, alias, joinType, withClause); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<TRoot,U> IQueryOver<TRoot,TSubType>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinQueryOver(path, alias, joinType, withClause); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<TSubType, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias<U>(Expression<Func<TSubType, U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinAlias(path, alias, joinType, withClause); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias<U>(Expression<Func<TSubType, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinAlias(path, alias, joinType, withClause); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinAlias(path, alias, joinType, withClause); }

		IQueryOver<TRoot,TSubType> IQueryOver<TRoot,TSubType>.JoinAlias<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType, ICriterion withClause)
		{ return JoinAlias(path, alias, joinType, withClause); }

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
