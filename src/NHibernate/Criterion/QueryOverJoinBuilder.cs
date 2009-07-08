
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
	}

	public class IQueryOverJoinBuilder<T> : QueryOverJoinBuilderBase<IQueryOver<T>, T>
	{
		public IQueryOverJoinBuilder(IQueryOver<T> root, JoinType joinType) : base(root, joinType) { }
	}

	public class QueryOverJoinBuilderBase<R, T> where R : IQueryOver<T>
	{

		private R _root;
		private JoinType _joinType;

		public QueryOverJoinBuilderBase(R root, JoinType joinType)
		{
			_root = root;
			_joinType = joinType;
		}

		public R Join(Expression<Func<T, object>> path, Expression<Func<object>> alias)
		{
			return (R)_root.Join(path, alias, _joinType);
		}

	}

}
