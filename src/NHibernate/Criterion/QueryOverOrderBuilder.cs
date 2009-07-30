
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	public class QueryOverOrderBuilder<T> : QueryOverOrderBuilderBase<QueryOver<T>, T>
	{

		public QueryOverOrderBuilder(QueryOver<T> root, Expression<Func<T, object>> path) : base(root, path)
		{}

		public QueryOverOrderBuilder(QueryOver<T> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class IQueryOverOrderBuilder<T> : QueryOverOrderBuilderBase<IQueryOver<T>, T>
	{

		public IQueryOverOrderBuilder(IQueryOver<T> root, Expression<Func<T, object>> path) : base(root, path)
		{}

		public IQueryOverOrderBuilder(IQueryOver<T> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class QueryOverOrderBuilderBase<R, T> where R : IQueryOver<T>
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
