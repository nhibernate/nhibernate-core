
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NHibernate.Impl;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion.Lambda
{

	public class QueryOverSubqueryBuilder<TRoot,TSubType> : QueryOverSubqueryBuilderBase<QueryOver<TRoot,TSubType>, TRoot, TSubType, QueryOverSubqueryPropertyBuilder<TRoot,TSubType>>
	{

		public QueryOverSubqueryBuilder(QueryOver<TRoot,TSubType> root)
			: base(root) { }

	}

	public class IQueryOverSubqueryBuilder<TRoot,TSubType> : QueryOverSubqueryBuilderBase<IQueryOver<TRoot,TSubType>, TRoot, TSubType, IQueryOverSubqueryPropertyBuilder<TRoot,TSubType>>
	{

		public IQueryOverSubqueryBuilder(IQueryOver<TRoot,TSubType> root)
			: base(root) { }

	}

	public class QueryOverSubqueryBuilderBase<TReturn, TRoot, TSubType, TBuilderType>
		where TReturn : IQueryOver<TRoot,TSubType>
		where TBuilderType : QueryOverSubqueryPropertyBuilderBase, new()
	{

		protected TReturn root;

		protected QueryOverSubqueryBuilderBase(TReturn root)
		{
			this.root = root;
		}

		/// <summary>
		/// Add an Exists subquery criterion
		/// </summary>
		public TReturn WhereExists<U>(QueryOver<U> detachedQuery)
		{
			root.And(Subqueries.Exists(detachedQuery.DetachedCriteria));
			return root;
		}

		/// <summary>
		/// Add a NotExists subquery criterion
		/// </summary>
		public TReturn WhereNotExists<U>(QueryOver<U> detachedQuery)
		{
			root.And(Subqueries.NotExists(detachedQuery.DetachedCriteria));
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .Where(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn Where(Expression<Func<TSubType, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<TSubType>(LambdaSubqueryType.Exact, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .Where(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn Where(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.Exact, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereAll(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn WhereAll(Expression<Func<TSubType, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<TSubType>(LambdaSubqueryType.All, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereAll(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn WhereAll(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.All, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereSome(t =&gt; t.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn WhereSome(Expression<Func<TSubType, bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery<TSubType>(LambdaSubqueryType.Some, expression);
			root.And(criterion);
			return root;
		}

		/// <summary>
		/// Subquery expression in the format
		/// .WhereSome(() =&gt; alias.Property [==, !=, >, etc.] detachedQueryOver.As&lt;propertyType&gt;())
		/// </summary>
		public TReturn WhereSome(Expression<Func<bool>> expression)
		{
			AbstractCriterion criterion = ExpressionProcessor.ProcessSubquery(LambdaSubqueryType.Some, expression);
			root.And(criterion);
			return root;
		}

		public TBuilderType WhereProperty(Expression<Func<TSubType, object>> expression)
		{
			string property = ExpressionProcessor.FindMemberExpression(expression.Body);
			return (TBuilderType)new TBuilderType().Set(root, property, null);
		}

		public TBuilderType WhereProperty(Expression<Func<object>> expression)
		{
			string property = ExpressionProcessor.FindMemberExpression(expression.Body);
			return (TBuilderType)new TBuilderType().Set(root, property, null);
		}

		public TBuilderType WhereValue(object value)
		{
			return (TBuilderType)new TBuilderType().Set(root, null, value);
		}

	}

}
