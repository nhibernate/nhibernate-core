
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Criterion.Lambda;
using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	[Serializable]
	public class QueryOver
	{

		protected ICriteria _criteria;
		protected CriteriaImpl _impl;

		protected QueryOver() { }

		public static QueryOver<T> Of<T>()
		{
			return new QueryOver<T>();
		}

		public static QueryOver<T> Of<T>(Expression<Func<T>> alias)
		{
			return new QueryOver<T>(alias);
		}

		public ICriteria UnderlyingCriteria
		{
			get { return _criteria; }
		}

		public DetachedCriteria DetachedCriteria
		{
			get { return new DetachedCriteria(_impl, _impl); }
		}

	}

	/// <summary>
	/// Implementation of the <see cref="IQueryOver&lt;T&gt;"/> interface
	/// </summary>
	[Serializable]
	public class QueryOver<T> : QueryOver, IQueryOver<T>
	{

		protected internal QueryOver()
		{
			_impl = new CriteriaImpl(typeof(T), null);
			_criteria = _impl;
		}

		protected internal QueryOver(Expression<Func<T>> alias)
		{
			string aliasPath = ExpressionProcessor.FindMemberExpression(alias.Body);
			_impl = new CriteriaImpl(typeof(T), aliasPath, null);
			_criteria = _impl;
		}

		protected internal QueryOver(CriteriaImpl impl)
		{
			_impl = impl;
			_criteria = impl;
		}

		protected internal QueryOver(CriteriaImpl rootImpl, ICriteria criteria)
		{
			_impl = rootImpl;
			_criteria = criteria;
		}

		/// <summary>
		/// Method to allow comparison of detached query in Lambda expression
		/// e.g., p =&gt; p.Name == myQuery.As&lt;string&gt;
		/// </summary>
		/// <typeparam name="R">type returned by query</typeparam>
		/// <returns>throws an exception if evaluated directly at runtime.</returns>
		public R As<R>()
		{
			throw new HibernateException("Incorrect syntax;  .As<T> method is for use in Lambda expressions only.");
		}

		public QueryOver<T> And(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> And(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> And(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<T> AndNot(Expression<Func<T, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<T> AndNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<T> AndRestrictionOn(Expression<Func<T, object>> expression)
		{
			return new QueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<T> AndRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<T> Where(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> Where(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> Where(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<T> WhereNot(Expression<Func<T, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<T> WhereNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<T> WhereRestrictionOn(Expression<Func<T, object>> expression)
		{
			return new QueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<T> WhereRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<T> Select(params Expression<Func<T, object>>[] projections)
		{
			List<IProjection> projectionList = new List<IProjection>();

			foreach (var projection in projections)
				projectionList.Add(Projections.Property(ExpressionProcessor.FindMemberExpression(projection.Body)));

			_criteria.SetProjection(projectionList.ToArray());
			return this;
		}

		public QueryOver<T> Select(params IProjection[] projections)
		{
			_criteria.SetProjection(projections);
			return this;
		}

		QueryOverProjectionBuilder<QueryOver<T>, T> SelectList
		{
			get { return new QueryOverProjectionBuilder<QueryOver<T>, T>(this, this); }
		}

		public QueryOverOrderBuilder<T> OrderBy(Expression<Func<T, object>> path)
		{
			return new QueryOverOrderBuilder<T>(this, path);
		}

		public QueryOverOrderBuilder<T> OrderBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<T>(this, path);
		}

		public QueryOverOrderBuilder<T> ThenBy(Expression<Func<T, object>> path)
		{
			return new QueryOverOrderBuilder<T>(this, path);
		}

		public QueryOverOrderBuilder<T> ThenBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<T>(this, path);
		}

		public QueryOver<T> Skip(int firstResult)
		{
			_criteria.SetFirstResult(firstResult);
			return this;
		}

		public QueryOver<T> Take(int maxResults)
		{
			_criteria.SetMaxResults(maxResults);
			return this;
		}

		public QueryOver<T> Cacheable()
		{
			_criteria.SetCacheable(true);
			return this;
		}

		public QueryOver<T> CacheMode(CacheMode cacheMode)
		{
			_criteria.SetCacheMode(cacheMode);
			return this;
		}

		public QueryOver<T> CacheRegion(string cacheRegion)
		{
			_criteria.SetCacheRegion(cacheRegion);
			return this;
		}

		public QueryOverSubqueryBuilder<T> WithSubquery
		{
			get { return new QueryOverSubqueryBuilder<T>(this); }
		}

		public QueryOverFetchBuilder<T> Fetch(Expression<Func<T, object>> path)
		{
			return new QueryOverFetchBuilder<T>(this, path);
		}

		public QueryOverLockBuilder<T> Lock()
		{
			return new QueryOverLockBuilder<T>(this, null);
		}

		public QueryOverLockBuilder<T> Lock(Expression<Func<object>> alias)
		{
			return new QueryOverLockBuilder<T>(this, alias);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOver<T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOverJoinBuilder<T> Inner
		{
			get { return new QueryOverJoinBuilder<T>(this, JoinType.InnerJoin); }
		}

		public QueryOverJoinBuilder<T> Left
		{
			get { return new QueryOverJoinBuilder<T>(this, JoinType.LeftOuterJoin); }
		}

		public QueryOverJoinBuilder<T> Right
		{
			get { return new QueryOverJoinBuilder<T>(this, JoinType.RightOuterJoin); }
		}

		public QueryOverJoinBuilder<T> Full
		{
			get { return new QueryOverJoinBuilder<T>(this, JoinType.FullJoin); }
		}

		public IList<T> List()
		{
			return _criteria.List<T>();
		}

		public IList<U> List<U>()
		{
			return _criteria.List<U>();
		}

		public T UniqueResult()
		{
			return _criteria.UniqueResult<T>();
		}

		public U UniqueResult<U>()
		{
			return _criteria.UniqueResult<U>();
		}

		IEnumerable<T> Future()
		{
			return _criteria.Future<T>();
		}

		IEnumerable<U> Future<U>()
		{
			return _criteria.Future<U>();
		}

		IFutureValue<T> FutureValue()
		{
			return _criteria.FutureValue<T>();
		}

		IFutureValue<U> FutureValue<U>()
		{
			return _criteria.FutureValue<U>();
		}

		/// <summary>
		/// Get an executable instance of <c>IQueryOver&lt;T&gt;</c>,
		/// to actually run the query.</summary>
		public IQueryOver<T> GetExecutableQueryOver(ISession session)
		{
			_impl.Session = session.GetSessionImplementation();
			return this;
		}

		private QueryOver<T> AddAlias(string path, string alias, JoinType joinType)
		{
			_criteria.CreateAlias(path, alias, joinType);
			return this;
		}

		private QueryOver<T> Add(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression<T>(expression));
			return this;
		}

		private QueryOver<T> Add(Expression<Func<bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression(expression));
			return this;
		}

		private QueryOver<T> Add(ICriterion expression)
		{
			_criteria.Add(expression);
			return this;
		}

		private QueryOver<T> AddNot(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression<T>(expression)));
			return this;
		}

		private QueryOver<T> AddNot(Expression<Func<bool>> expression)
		{
			_criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression(expression)));
			return this;
		}


		ICriteria IQueryOver<T>.UnderlyingCriteria
		{ get { return UnderlyingCriteria; } }

		IQueryOver<T> IQueryOver<T>.And(Expression<Func<T, bool>> expression)
		{ return And(expression); }

		IQueryOver<T> IQueryOver<T>.And(Expression<Func<bool>> expression)
		{ return And(expression); }

		IQueryOver<T> IQueryOver<T>.And(ICriterion expression)
		{ return And(expression); }

		IQueryOver<T> IQueryOver<T>.AndNot(Expression<Func<T, bool>> expression)
		{ return AndNot(expression); }

		IQueryOver<T> IQueryOver<T>.AndNot(Expression<Func<bool>> expression)
		{ return AndNot(expression); }

		IQueryOverRestrictionBuilder<T> IQueryOver<T>.AndRestrictionOn(Expression<Func<T, object>> expression)
		{ return new IQueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<T> IQueryOver<T>.AndRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<T> IQueryOver<T>.Where(Expression<Func<T, bool>> expression)
		{ return Where(expression); }

		IQueryOver<T> IQueryOver<T>.Where(Expression<Func<bool>> expression)
		{ return Where(expression); }

		IQueryOver<T> IQueryOver<T>.Where(ICriterion expression)
		{ return Where(expression); }

		IQueryOver<T> IQueryOver<T>.WhereNot(Expression<Func<T, bool>> expression)
		{ return WhereNot(expression); }

		IQueryOver<T> IQueryOver<T>.WhereNot(Expression<Func<bool>> expression)
		{ return WhereNot(expression); }

		IQueryOverRestrictionBuilder<T> IQueryOver<T>.WhereRestrictionOn(Expression<Func<T, object>> expression)
		{ return new IQueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<T> IQueryOver<T>.WhereRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<T> IQueryOver<T>.Select(params Expression<Func<T, object>>[] projections)
		{ return Select(projections); }

		IQueryOver<T> IQueryOver<T>.Select(params IProjection[] projections)
		{ return Select(projections); }

		QueryOverProjectionBuilder<IQueryOver<T>, T> IQueryOver<T>.SelectList
		{ get { return new QueryOverProjectionBuilder<IQueryOver<T>,T>(this, this); } }

		IQueryOverOrderBuilder<T> IQueryOver<T>.OrderBy(Expression<Func<T, object>> path)
		{ return new IQueryOverOrderBuilder<T>(this, path); }

		IQueryOverOrderBuilder<T> IQueryOver<T>.OrderBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<T>(this, path); }

		IQueryOverOrderBuilder<T> IQueryOver<T>.ThenBy(Expression<Func<T, object>> path)
		{ return new IQueryOverOrderBuilder<T>(this, path); }

		IQueryOverOrderBuilder<T> IQueryOver<T>.ThenBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<T>(this, path); }

		IQueryOver<T> IQueryOver<T>.Skip(int firstResult)
		{ return Skip(firstResult); }

		IQueryOver<T> IQueryOver<T>.Take(int maxResults)
		{ return Take(maxResults); }

		IQueryOver<T> IQueryOver<T>.Cacheable()
		{ return Cacheable(); }

		IQueryOver<T> IQueryOver<T>.CacheMode(CacheMode cacheMode)
		{ return CacheMode(cacheMode); }

		IQueryOver<T> IQueryOver<T>.CacheRegion(string cacheRegion)
		{ return CacheRegion(cacheRegion); }

		IQueryOverSubqueryBuilder<T> IQueryOver<T>.WithSubquery
		{ get { return new IQueryOverSubqueryBuilder<T>(this); } }

		IQueryOverFetchBuilder<T> IQueryOver<T>.Fetch(Expression<Func<T, object>> path)
		{ return new IQueryOverFetchBuilder<T>(this, path); }

		IQueryOverLockBuilder<T> IQueryOver<T>.Lock()
		{ return new IQueryOverLockBuilder<T>(this, null); }

		IQueryOverLockBuilder<T> IQueryOver<T>.Lock(Expression<Func<object>> alias)
		{ return new IQueryOverLockBuilder<T>(this, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<T> IQueryOver<T>.JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<T> IQueryOver<T>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<T> IQueryOver<T>.JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOver<T> IQueryOver<T>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOverJoinBuilder<T> IQueryOver<T>.Inner
		{ get { return new IQueryOverJoinBuilder<T>(this, JoinType.InnerJoin); } }

		IQueryOverJoinBuilder<T> IQueryOver<T>.Left
		{ get { return new IQueryOverJoinBuilder<T>(this, JoinType.LeftOuterJoin); } }

		IQueryOverJoinBuilder<T> IQueryOver<T>.Right
		{ get { return new IQueryOverJoinBuilder<T>(this, JoinType.RightOuterJoin); } }

		IQueryOverJoinBuilder<T> IQueryOver<T>.Full
		{ get { return new IQueryOverJoinBuilder<T>(this, JoinType.FullJoin); } }

		IList<T> IQueryOver<T>.List()
		{ return List(); }

		IList<U> IQueryOver<T>.List<U>()
		{ return List<U>(); }

		T IQueryOver<T>.UniqueResult()
		{ return UniqueResult(); }

		U IQueryOver<T>.UniqueResult<U>()
		{ return UniqueResult<U>(); }

		IEnumerable<T> IQueryOver<T>.Future()
		{ return Future(); }

		IEnumerable<U> IQueryOver<T>.Future<U>()
		{ return Future<U>(); }

		IFutureValue<T> IQueryOver<T>.FutureValue()
		{ return FutureValue(); }

		IFutureValue<U> IQueryOver<T>.FutureValue<U>()
		{ return FutureValue<U>(); }

	}

}
