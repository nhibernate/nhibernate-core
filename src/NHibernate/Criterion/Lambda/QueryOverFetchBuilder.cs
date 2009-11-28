
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{

	public class QueryOverFetchBuilder<S,T> : QueryOverFetchBuilderBase<QueryOver<S,T>, S, T>
	{

		public QueryOverFetchBuilder(QueryOver<S,T> root, Expression<Func<S, object>> path)
			: base(root, path) { }

	}

	public class IQueryOverFetchBuilder<S,T> : QueryOverFetchBuilderBase<IQueryOver<S,T>, S, T>
	{

		public IQueryOverFetchBuilder(IQueryOver<S,T> root, Expression<Func<S, object>> path)
			: base(root, path) { }

	}

	public class QueryOverFetchBuilderBase<R, S, T> where R : IQueryOver<S,T>
	{

		protected R root;
		protected string path;

		protected QueryOverFetchBuilderBase(R root, Expression<Func<S, object>> path)
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
