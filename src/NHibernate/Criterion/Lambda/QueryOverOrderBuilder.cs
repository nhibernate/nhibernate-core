
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverOrderBuilder<S,T> : QueryOverOrderBuilderBase<QueryOver<S,T>, S, T>
	{

		public QueryOverOrderBuilder(QueryOver<S,T> root, Expression<Func<T, object>> path) : base(root, path)
		{}

		public QueryOverOrderBuilder(QueryOver<S,T> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class IQueryOverOrderBuilder<S,T> : QueryOverOrderBuilderBase<IQueryOver<S,T>, S, T>
	{

		public IQueryOverOrderBuilder(IQueryOver<S,T> root, Expression<Func<T, object>> path) : base(root, path)
		{}

		public IQueryOverOrderBuilder(IQueryOver<S,T> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class QueryOverOrderBuilderBase<R, S, T> where R : IQueryOver<S, T>
	{

		protected R root;
		protected LambdaExpression path;

		protected QueryOverOrderBuilderBase(R root, Expression<Func<T, object>> path)
		{
			this.root = root;
			this.path = path;
		}

		protected QueryOverOrderBuilderBase(R root, Expression<Func<object>> path)
		{
			this.root = root;
			this.path = path;
		}

		public R Asc
		{
			get
			{
				this.root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, Order.Asc));
				return this.root;
			}
		}

		public R Desc
		{
			get
			{
				this.root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, Order.Desc));
				return this.root;
			}
		}

	}

}
