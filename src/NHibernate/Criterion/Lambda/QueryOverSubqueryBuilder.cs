
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverSubqueryBuilder<S,T> : QueryOverSubqueryBuilderBase<QueryOver<S,T>, S, T, QueryOverSubqueryPropertyBuilder<S,T>>
	{

		public QueryOverSubqueryBuilder(QueryOver<S,T> root)
			: base(root) { }

	}

	public class IQueryOverSubqueryBuilder<S,T> : QueryOverSubqueryBuilderBase<IQueryOver<S,T>, S, T, IQueryOverSubqueryPropertyBuilder<S,T>>
	{

		public IQueryOverSubqueryBuilder(IQueryOver<S,T> root)
			: base(root) { }

	}

	public class QueryOverSubqueryBuilderBase<R, S, T, B>
		where R : IQueryOver<S,T>
		where B : QueryOverSubqueryPropertyBuilderBase, new()
	{

		protected R root;

		protected QueryOverSubqueryBuilderBase(R root)
		{
			this.root = root;
		}

		/// <summary>
		/// Add an Exists subquery criterion
		/// </summary>
		public R WhereExists<U>(QueryOver<U> detachedQuery)
		{
			root.And(Subqueries.Exists(detachedQuery.DetachedCriteria));
			return root;
		}

		/// <summary>
		/// Add a NotExists subquery criterion
		/// </summary>
		public R WhereNotExists<U>(QueryOver<U> detachedQuery)
		{
			root.And(Subqueries.NotExists(detachedQuery.DetachedCriteria));
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .Where(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R Where(Expression<Func<T, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<T>(LambdaSubqueryType.Exact, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .Where(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R Where(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.Exact, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereAll(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R WhereAll(Expression<Func<T, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<T>(LambdaSubqueryType.All, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereAll(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R WhereAll(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.All, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereSome(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R WhereSome(Expression<Func<T, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<T>(LambdaSubqueryType.Some, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereSome(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public R WhereSome(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.Some, expression);
			root.And(criterion);
			return root;
		}

		public B WhereProperty(Expression<Func<T, object>> expression)
		{
			string property = ExpressionProcessor.FindMemberExpression(expression.Body);
			return (B)new B().Set(root, property, null);
		}

		public B WhereProperty(Expression<Func<object>> expression)
		{
			string property = ExpressionProcessor.FindMemberExpression(expression.Body);
			return (B)new B().Set(root, property, null);
		}

		public B WhereValue(object value)
		{
			return (B)new B().Set(root, null, value);
		}

	}

}
