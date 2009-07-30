
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	/// <summary>
	/// Implementation of the <see cref="IQueryOver&lt;T&gt;"/> interface
	/// </summary>
	[Serializable]
	public class QueryOver<T> : IQueryOver<T>
	{

		private ICriteria		_criteria;
		private CriteriaImpl	_impl;

		public QueryOver()
		{
			_impl = new CriteriaImpl(typeof(T), null);
			_criteria = _impl;
		}

		public QueryOver(CriteriaImpl impl)
		{
			_impl = impl;
			_criteria = impl;
		}

		public QueryOver(CriteriaImpl rootImpl, ICriteria criteria)
		{
			_impl = rootImpl;
			_criteria = criteria;
		}

		public ICriteria UnderlyingCriteria
		{
			get { return _criteria; }
		}

		public QueryOver<T> And(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> And(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> Where(Expression<Func<T, bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> Where(Expression<Func<bool>> expression)
		{
			return Add(expression);
		}

		public QueryOver<T> Select(params Expression<Func<T, object>>[] projections)
		{
			List<IProjection> projectionList = new List<IProjection>();

			foreach (var projection in projections)
				projectionList.Add(Projections.Property(ExpressionProcessor.FindMemberExpression(projection.Body)));

			_criteria.SetProjection(projectionList.ToArray());
			return this;
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

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path)
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

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
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

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
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

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
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

		public QueryOver<T> Join(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return AddAlias(
				ExpressionProcessor.FindMemberExpression(path.Body),
				ExpressionProcessor.FindMemberExpression(alias.Body),
				JoinType.InnerJoin);
		}

		public QueryOver<T> Join(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
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


		ICriteria IQueryOver<T>.UnderlyingCriteria
		{ get { return UnderlyingCriteria; } }

		IQueryOver<T> IQueryOver<T>.And(Expression<Func<T, bool>> expression)
		{ return And(expression); }

		IQueryOver<T> IQueryOver<T>.And(Expression<Func<bool>> expression)
		{ return And(expression); }

		IQueryOver<T> IQueryOver<T>.Where(Expression<Func<T, bool>> expression)
		{ return Where(expression); }

		IQueryOver<T> IQueryOver<T>.Where(Expression<Func<bool>> expression)
		{ return Where(expression); }

		IQueryOver<T> IQueryOver<T>.Select(params Expression<Func<T, object>>[] projections)
		{ return Select(projections); }

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

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{ return JoinQueryOver(path); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{ return JoinQueryOver(path, alias); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, JoinType joinType)
		{ return JoinQueryOver(path, joinType); }

		IQueryOver<U> IQueryOver<T>.JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias, JoinType joinType)
		{ return JoinQueryOver(path, alias, joinType); }

		IQueryOver<T> IQueryOver<T>.Join(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{ return Join(path, alias); }

		IQueryOver<T> IQueryOver<T>.Join(Expression<Func<T, object>> path, Expression<Func<object>> alias, JoinType joinType)
		{ return Join(path, alias, joinType); }

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

	}

}
