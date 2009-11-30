
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverOrderBuilder<TRoot,TSubType> : QueryOverOrderBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public QueryOverOrderBuilder(QueryOver<TRoot,TSubType> root, Expression<Func<TSubType, object>> path) : base(root, path)
		{}

		public QueryOverOrderBuilder(QueryOver<TRoot,TSubType> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class IQueryOverOrderBuilder<TRoot,TSubType> : QueryOverOrderBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverOrderBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<TSubType, object>> path) : base(root, path)
		{}

		public IQueryOverOrderBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<object>> path) : base(root, path)
		{}

	}

	public class QueryOverOrderBuilderBase<TReturn, TRoot, TSubType> where TReturn : IQueryOver<TRoot, TSubType>
	{

		protected TReturn root;
		protected LambdaExpression path;

		protected QueryOverOrderBuilderBase(TReturn root, Expression<Func<TSubType, object>> path)
		{
			this.root = root;
			this.path = path;
		}

		protected QueryOverOrderBuilderBase(TReturn root, Expression<Func<object>> path)
		{
			this.root = root;
			this.path = path;
		}

		public TReturn Asc
		{
			get
			{
				this.root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, Order.Asc));
				return this.root;
			}
		}

		public TReturn Desc
		{
			get
			{
				this.root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, Order.Desc));
				return this.root;
			}
		}

	}

}
