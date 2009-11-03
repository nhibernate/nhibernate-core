
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	public class QueryOverJoinBuilder<T> : QueryOverJoinBuilderBase<QueryOver<T>, T>
	{
		public QueryOverJoinBuilder(QueryOver<T> root, JoinType joinType) : base(root, joinType) { }

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public QueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

	}

	public class IQueryOverJoinBuilder<T> : QueryOverJoinBuilderBase<IQueryOver<T>, T>
	{
		public IQueryOverJoinBuilder(IQueryOver<T> root, JoinType joinType) : base(root, joinType) { }

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<U>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path)
		{
			return root.JoinQueryOver<U>(path, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<T, IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

		public IQueryOver<U> JoinQueryOver<U>(Expression<Func<IEnumerable<U>>> path, Expression<Func<U>> alias)
		{
			return root.JoinQueryOver<U>(path, alias, joinType);
		}

	}

	public class QueryOverJoinBuilderBase<R, T> where R : IQueryOver<T>
	{

		protected R root;
		protected JoinType joinType;

		public QueryOverJoinBuilderBase(R root, JoinType joinType)
		{
			this.root = root;
			this.joinType = joinType;
		}

		public R Join(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return (R)root.Join(path, alias, joinType);
		}

		public R Join(Expression<Func<object>> path, Expression<Func<object>> alias)
		{
			return (R)root.Join(path, alias, joinType);
		}

	}

}
