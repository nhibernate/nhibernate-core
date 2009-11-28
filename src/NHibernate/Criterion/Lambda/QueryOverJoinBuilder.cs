
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{
	
	public class QueryOverJoinBuilder<S,T> : QueryOverJoinBuilderBase<QueryOver<S,T>, S, T>
	{
		public QueryOverJoinBuilder(QueryOver<S,T> root, JoinType joinType) : base(root, joinType) { }

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<S,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

	}

	public class IQueryOverJoinBuilder<S,T> : QueryOverJoinBuilderBase<IQueryOver<S,T>, S, T>
	{
		public IQueryOverJoinBuilder(IQueryOver<S,T> root, JoinType joinType) : base(root, joinType) { }

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<S,U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

	}

	public class QueryOverJoinBuilderBase<R, S, T> where R : IQueryOver<S,T>
	{

		protected R root;
		protected JoinType joinType;

		public QueryOverJoinBuilderBase(R root, JoinType joinType)
		{
			this.root = root;
			this.joinType = joinType;
		}

		public R JoinAlias(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return (R)root.JoinAlias(path, alias, joinType);
		}

		public R JoinAlias(Expression<Func<object>> path, Expression<Func<object>> alias)
		{
			return (R)root.JoinAlias(path, alias, joinType);
		}

	}

}
