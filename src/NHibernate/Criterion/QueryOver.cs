
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
			get { return _criteria; }
		}

		public DetachedCriteria DetachedCriteria
		{
			get { return new DetachedCriteria(_impl, _impl); }
		}

	}

	[Serializable]
	public class QueryOver<T> : QueryOver, IQueryOver<T>
	{

		private IList<T> List()
		{
			return _criteria.List<T>();
		}

		private IList<U> List<U>()
		{
			return _criteria.List<U>();
		}

		private T UniqueResult()
		{
			return _criteria.UniqueResult<T>();
		}

		private U UniqueResult<U>()
		{
			return _criteria.UniqueResult<U>();
		}

		private IEnumerable<T> Future()
		{
			return _criteria.Future<T>();
		}

		private IEnumerable<U> Future<U>()
		{
			return _criteria.Future<U>();
		}

		private IFutureValue<T> FutureValue()
		{
			return _criteria.FutureValue<T>();
		}

		private IFutureValue<U> FutureValue<U>()
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

		/// <summary>
		/// Method to allow comparison of detached query in Lambda expression
		/// e.g., p =&gt; p.Name == myQuery.As&lt;string&gt;
		/// </summary>
		/// <typeparam name="S">type returned by query</typeparam>
		/// <returns>throws an exception if evaluated directly at runtime.</returns>
		public S As<S>()
		{
			throw new HibernateException("Incorrect syntax;  .As<T> method is for use in Lambda expressions only.");
		}


		ICriteria IQueryOver<T>.UnderlyingCriteria
		{ get { return UnderlyingCriteria; } }

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

	/// <summary>
	/// Implementation of the <see cref="IQueryOver&lt;T&gt;"/> interface
	/// </summary>
	[Serializable]
	public class QueryOver<R,T> : QueryOver<R>, IQueryOver<R,T>
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

		public QueryOver<R,T> And(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> And(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> And(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> AndNot(Expression<Func<T, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<R,T> AndNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<R,T> AndRestrictionOn(Expression<Func<T, object>> expression)
		{
			return new QueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<R,T> AndRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<R,T> Where(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> Where(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> Where(ICriterion expression)
		{
			return Add(expression);
		}

		public QueryOver<R,T> WhereNot(Expression<Func<T, bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOver<R,T> WhereNot(Expression<Func<bool>> expression)
		{
			return AddNot(expression);
		}

		public QueryOverRestrictionBuilder<R,T> WhereRestrictionOn(Expression<Func<T, object>> expression)
		{
			return new QueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOverRestrictionBuilder<R,T> WhereRestrictionOn(Expression<Func<object>> expression)
		{
			return new QueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public QueryOver<R,T> Select(params Expression<Func<R, object>>[] projections)
		{
			List<IProjection> projectionList = new List<IProjection>();

			foreach (var projection in projections)
				projectionList.Add(Projections.Property(ExpressionProcessor.FindMemberExpression(projection.Body)));

			_criteria.SetProjection(projectionList.ToArray());
			return this;
		}

		public QueryOver<R,T> Select(params IProjection[] projections)
		{
			_criteria.SetProjection(projections);
			return this;
		}

		QueryOverProjectionBuilder<QueryOver<R,T>, R, T> SelectList
		{
			get { return new QueryOverProjectionBuilder<QueryOver<R,T>, R, T>(this, this); }
		}

		public QueryOverOrderBuilder<R,T> OrderBy(Expression<Func<T, object>> path)
		{
			return new QueryOverOrderBuilder<R,T>(this, path);
		}

		public QueryOverOrderBuilder<R,T> OrderBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<R,T>(this, path);
		}

		public QueryOverOrderBuilder<R,T> ThenBy(Expression<Func<T, object>> path)
		{
			return new QueryOverOrderBuilder<R,T>(this, path);
		}

		public QueryOverOrderBuilder<R,T> ThenBy(Expression<Func<object>> path)
		{
			return new QueryOverOrderBuilder<R,T>(this, path);
		}

		public QueryOver<R,T> Skip(int firstResult)
		{
			_criteria.SetFirstResult(firstResult);
			return this;
		}

		public QueryOver<R,T> Take(int maxResults)
		{
			_criteria.SetMaxResults(maxResults);
			return this;
		}

		public QueryOver<R,T> Cacheable()
		{
			_criteria.SetCacheable(true);
			return this;
		}

		public QueryOver<R,T> CacheMode(CacheMode cacheMode)
		{
			_criteria.SetCacheMode(cacheMode);
			return this;
		}

		public QueryOver<R,T> CacheRegion(string cacheRegion)
		{
			_criteria.SetCacheRegion(cacheRegion);
			return this;
		}

		public QueryOverSubqueryBuilder<R,T> WithSubquery
		{
			get { return new QueryOverSubqueryBuilder<R,T>(this); }
		}

		public QueryOverFetchBuilder<R,T> Fetch(Expression<Func<R, object>> path)
		{
			return new QueryOverFetchBuilder<R,T>(this, path);
		}

		public QueryOverLockBuilder<R,T> Lock()
		{
			return new QueryOverLockBuilder<R,T>(this, null);
		}

		public QueryOverLockBuilder<R,T> Lock(Expression<Func<object>> alias)
		{
			return new QueryOverLockBuilder<R,T>(this, alias);
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body)));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					ExpressionProcessor.FindMemberExpression(alias.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<R,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{
			return new QueryOver<R,U>(_impl,
				_criteria.CreateCriteria(
					ExpressionProcessor.FindMemberExpression(path.Body),
					joinType));
		}

		public QueryOver<R,T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<R,T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<R,T> JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOver<R,T> JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				joinType);
		}

		public QueryOverJoinBuilder<R,T> Inner
		{
			get { return new QueryOverJoinBuilder<R,T>(this, JoinType.InnerJoin); }
		}

		public QueryOverJoinBuilder<R,T> Left
		{
			get { return new QueryOverJoinBuilder<R,T>(this, JoinType.LeftOuterJoin); }
		}

		public QueryOverJoinBuilder<R,T> Right
		{
			get { return new QueryOverJoinBuilder<R,T>(this, JoinType.RightOuterJoin); }
		}

		public QueryOverJoinBuilder<R,T> Full
		{
			get { return new QueryOverJoinBuilder<R,T>(this, JoinType.FullJoin); }
		}

		private QueryOver<R,T> AddAlias(string path, string alias, JoinType joinType)
		{
			_criteria.CreateAlias(path, alias, joinType);
			return this;
		}

		private QueryOver<R,T> Add(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression<T>(expression));
			return this;
		}

		private QueryOver<R,T> Add(Expression<Func<bool>> expression)
		{
			_criteria.Add(ExpressionProcessor.ProcessExpression(expression));
			return this;
		}

		private QueryOver<R,T> Add(ICriterion expression)
		{
			_criteria.Add(expression);
			return this;
		}

		private QueryOver<R,T> AddNot(Expression<Func<T, bool>> expression)
		{
			_criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression<T>(expression)));
			return this;
		}

		private QueryOver<R,T> AddNot(Expression<Func<bool>> expression)
		{
			_criteria.Add(Restrictions.Not(ExpressionProcessor.ProcessExpression(expression)));
			return this;
		}


		IQueryOver<R,T> IQueryOver<R,T>.And(Expression<Func<T, bool>> expression)
		{ return And(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.And(Expression<Func<bool>> expression)
		{ return And(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.And(ICriterion expression)
		{ return And(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.AndNot(Expression<Func<T, bool>> expression)
		{ return AndNot(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.AndNot(Expression<Func<bool>> expression)
		{ return AndNot(expression); }

		IQueryOverRestrictionBuilder<R,T> IQueryOver<R,T>.AndRestrictionOn(Expression<Func<T, object>> expression)
		{ return new IQueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<R,T> IQueryOver<R,T>.AndRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<R,T> IQueryOver<R,T>.Where(Expression<Func<T, bool>> expression)
		{ return Where(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.Where(Expression<Func<bool>> expression)
		{ return Where(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.Where(ICriterion expression)
		{ return Where(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.WhereNot(Expression<Func<T, bool>> expression)
		{ return WhereNot(expression); }

		IQueryOver<R,T> IQueryOver<R,T>.WhereNot(Expression<Func<bool>> expression)
		{ return WhereNot(expression); }

		IQueryOverRestrictionBuilder<R,T> IQueryOver<R,T>.WhereRestrictionOn(Expression<Func<T, object>> expression)
		{ return new IQueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOverRestrictionBuilder<R,T> IQueryOver<R,T>.WhereRestrictionOn(Expression<Func<object>> expression)
		{ return new IQueryOverRestrictionBuilder<R,T>(this, ExpressionProcessor.FindMemberExpression(expression.Body)); }

		IQueryOver<R,T> IQueryOver<R,T>.Select(params Expression<Func<R, object>>[] projections)
		{ return Select(projections); }

		IQueryOver<R,T> IQueryOver<R,T>.Select(params IProjection[] projections)
		{ return Select(projections); }

		QueryOverProjectionBuilder<IQueryOver<R,T>, R, T> IQueryOver<R,T>.SelectList
		{ get { return new QueryOverProjectionBuilder<IQueryOver<R,T>,R,T>(this, this); } }

		IQueryOverOrderBuilder<R,T> IQueryOver<R,T>.OrderBy(Expression<Func<T, object>> path)
		{ return new IQueryOverOrderBuilder<R,T>(this, path); }

		IQueryOverOrderBuilder<R,T> IQueryOver<R,T>.OrderBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<R,T>(this, path); }

		IQueryOverOrderBuilder<R,T> IQueryOver<R,T>.ThenBy(Expression<Func<T, object>> path)
		{ return new IQueryOverOrderBuilder<R,T>(this, path); }

		IQueryOverOrderBuilder<R,T> IQueryOver<R,T>.ThenBy(Expression<Func<object>> path)
		{ return new IQueryOverOrderBuilder<R,T>(this, path); }

		IQueryOver<R,T> IQueryOver<R,T>.Skip(int firstResult)
		{ return Skip(firstResult); }

		IQueryOver<R,T> IQueryOver<R,T>.Take(int maxResults)
		{ return Take(maxResults); }

		IQueryOver<R,T> IQueryOver<R,T>.Cacheable()
		{ return Cacheable(); }

		IQueryOver<R,T> IQueryOver<R,T>.CacheMode(CacheMode cacheMode)
		{ return CacheMode(cacheMode); }

		IQueryOver<R,T> IQueryOver<R,T>.CacheRegion(string cacheRegion)
		{ return CacheRegion(cacheRegion); }

		IQueryOverSubqueryBuilder<R,T> IQueryOver<R,T>.WithSubquery
		{ get { return new IQueryOverSubqueryBuilder<R,T>(this); } }

		IQueryOverFetchBuilder<R,T> IQueryOver<R,T>.Fetch(Expression<Func<R, object>> path)
		{ return new IQueryOverFetchBuilder<R,T>(this, path); }

		IQueryOverLockBuilder<R,T> IQueryOver<R,T>.Lock()
		{ return new IQueryOverLockBuilder<R,T>(this, null); }

		IQueryOverLockBuilder<R,T> IQueryOver<R,T>.Lock(Expression<Func<object>> alias)
		{ return new IQueryOverLockBuilder<R,T>(this, alias); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<R,U> IQueryOver<R,T>.JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<R,T> IQueryOver<R,T>.JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<R,T> IQueryOver<R,T>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{ return JoinAlias(path, alias); }

		IQueryOver<R,T> IQueryOver<R,T>.JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOver<R,T> IQueryOver<R,T>.JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return JoinAlias(path, alias, joinType); }

		IQueryOverJoinBuilder<R,T> IQueryOver<R,T>.Inner
		{ get { return new IQueryOverJoinBuilder<R,T>(this, JoinType.InnerJoin); } }

		IQueryOverJoinBuilder<R,T> IQueryOver<R,T>.Left
		{ get { return new IQueryOverJoinBuilder<R,T>(this, JoinType.LeftOuterJoin); } }

		IQueryOverJoinBuilder<R,T> IQueryOver<R,T>.Right
		{ get { return new IQueryOverJoinBuilder<R,T>(this, JoinType.RightOuterJoin); } }

		IQueryOverJoinBuilder<R,T> IQueryOver<R,T>.Full
		{ get { return new IQueryOverJoinBuilder<R,T>(this, JoinType.FullJoin); } }

	}

}
