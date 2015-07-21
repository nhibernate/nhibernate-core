
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

		public QueryOverOrderBuilder(QueryOver<TRoot,TSubType> root, Expression<Func<object>> path, bool isAlias) : base(root, path, isAlias)
		{}

		public QueryOverOrderBuilder(QueryOver<TRoot,TSubType> root, ExpressionProcessor.ProjectionInfo projection) : base(root, projection)
		{}

	}

	public class IQueryOverOrderBuilder<TRoot,TSubType> : QueryOverOrderBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverOrderBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<TSubType, object>> path) : base(root, path)
		{}

		public IQueryOverOrderBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<object>> path, bool isAlias) : base(root, path, isAlias)
		{}

		public IQueryOverOrderBuilder(IQueryOver<TRoot,TSubType> root, ExpressionProcessor.ProjectionInfo projection) : base(root, projection)
		{}

	}

	public class QueryOverOrderBuilderBase<TReturn, TRoot, TSubType> where TReturn : IQueryOver<TRoot, TSubType>
	{

		protected TReturn root;
		protected LambdaExpression path;
		protected bool isAlias;
		protected ExpressionProcessor.ProjectionInfo projection;

		protected QueryOverOrderBuilderBase(TReturn root, Expression<Func<TSubType, object>> path)
		{
			this.root = root;
			this.path = path;
			this.isAlias = false;
		}

		protected QueryOverOrderBuilderBase(TReturn root, Expression<Func<object>> path, bool isAlias)
		{
			this.root = root;
			this.path = path;
			this.isAlias = isAlias;
		}

		protected QueryOverOrderBuilderBase(TReturn root, ExpressionProcessor.ProjectionInfo projection)
		{
			this.root = root;
			this.projection = projection;
		}

		private void AddOrder(Func<string, Order> orderStringDelegate, Func<IProjection, Order> orderProjectionDelegate)
		{
			if (projection != null)
				root.UnderlyingCriteria.AddOrder(projection.CreateOrder(orderStringDelegate, orderProjectionDelegate));
			else
				if (isAlias)
					root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, orderStringDelegate));
				else
					root.UnderlyingCriteria.AddOrder(ExpressionProcessor.ProcessOrder(path, orderStringDelegate, orderProjectionDelegate));
		}

		public TReturn Asc
		{
			get
			{
				AddOrder(Order.Asc, Order.Asc);
				return this.root;
			}
		}

		public TReturn Desc
		{
			get
			{
				AddOrder(Order.Desc, Order.Desc);
				return this.root;
			}
		}

	}

}
