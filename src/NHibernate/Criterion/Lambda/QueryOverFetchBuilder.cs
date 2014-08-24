
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverFetchBuilder<TRoot,TSubType> : QueryOverFetchBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public QueryOverFetchBuilder(QueryOver<TRoot,TSubType> root, Expression<Func<TRoot, object>> path)
			: base(root, path) { }

	}

	public class IQueryOverFetchBuilder<TRoot,TSubType> : QueryOverFetchBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType>
	{

		public IQueryOverFetchBuilder(IQueryOver<TRoot,TSubType> root, Expression<Func<TRoot, object>> path)
			: base(root, path) { }

	}

	public class QueryOverFetchBuilderBase<TReturn, TRoot, TSubType> where TReturn : IQueryOver<TRoot,TSubType>
	{

		protected TReturn root;
		protected string path;

		protected QueryOverFetchBuilderBase(TReturn root, Expression<Func<TRoot, object>> path)
		{
			this.root = root;
			this.path = ExpressionProcessor.FindMemberExpression(path.Body);
		}

		public TReturn Eager
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Eager);
				return this.root;
			}
		}

		public TReturn Lazy
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Lazy);
				return this.root;
			}
		}

		public TReturn Default
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Default);
				return this.root;
			}
		}

	}

}
