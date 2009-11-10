
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	public class QueryOverFetchBuilder<T> : QueryOverFetchBuilderBase<QueryOver<T>, T>
	{

		public QueryOverFetchBuilder(QueryOver<T> root, Expression<Func<T, object>> path)
			: base(root, path) { }

	}

	public class IQueryOverFetchBuilder<T> : QueryOverFetchBuilderBase<IQueryOver<T>, T>
	{

		public IQueryOverFetchBuilder(IQueryOver<T> root, Expression<Func<T, object>> path)
			: base(root, path) { }

	}

	public class QueryOverFetchBuilderBase<R, T> where R : IQueryOver<T>
	{

		protected R root;
		protected string path;

		protected QueryOverFetchBuilderBase(R root, Expression<Func<T, object>> path)
		{
			this.root = root;
			this.path = ExpressionProcessor.FindMemberExpression(path.Body);
		}

		public R Eager
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Eager);
				return this.root;
			}
		}

		public R Lazy
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Lazy);
				return this.root;
			}
		}

		public R Default
		{
			get
			{
				this.root.UnderlyingCriteria.SetFetchMode(path, FetchMode.Default);
				return this.root;
			}
		}

	}

}
