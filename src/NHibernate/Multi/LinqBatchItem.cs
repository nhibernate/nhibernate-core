using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors;

namespace NHibernate.Multi
{
	public static class LinqBatchItem
	{
		public static LinqBatchItem<TResult> Create<T, TResult>(IQueryable<T> query, Expression<Func<IQueryable<T>, TResult>> selector)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			if (selector == null)
				throw new ArgumentNullException(nameof(selector));
			var expression = ReplacingExpressionVisitor
				.Replace(selector.Parameters.Single(), query.Expression, selector.Body);
			return GetForQuery<TResult>(query, expression);
		}

		public static LinqBatchItem<T> Create<T>(IQueryable<T> query)
		{
			return GetForQuery<T>(query, null);
		}

		private static LinqBatchItem<TResult> GetForQuery<TResult>(IQueryable query, Expression ex = null)
		{
			if (query == null)
				throw new ArgumentNullException(nameof(query));
			var prov = (ISupportFutureBatchNhQueryProvider) query.Provider;

			var q = prov.GetPreparedQuery(ex ?? query.Expression, out var linqEx);
			return new LinqBatchItem<TResult>(q, linqEx);
		}
	}

	/// <summary>
	/// Create instance via <see cref="LinqBatchItem.Create"/> methods
	/// </summary>
	/// <typeparam name="T">Result type</typeparam>
	public partial class LinqBatchItem<T> : QueryBatchItem<T>
	{
		private readonly Delegate _postExecuteTransformer;

		public LinqBatchItem(IQuery query) : base(query)
		{
		}

		internal LinqBatchItem(IQuery query, NhLinqExpression linq) : base(query)
		{
			_postExecuteTransformer = linq.ExpressionToHqlTranslationResults.PostExecuteTransformer;
		}

		protected override IList<T> GetResultsNonBatched()
		{
			if (_postExecuteTransformer == null)
			{
				return base.GetResultsNonBatched();
			}

			return GetTransformedResults(Query.List());
		}

		protected override List<T> DoGetResults()
		{
			if (_postExecuteTransformer != null)
			{
				var elementType = GetResultTypeIfChanged();

				IList transformerList = elementType == null
					? base.DoGetResults()
					: GetTypedResults(elementType);

				return GetTransformedResults(transformerList);
			}

			return base.DoGetResults();
		}

		private List<T> GetTransformedResults(IList transformerList)
		{
			var res = _postExecuteTransformer.DynamicInvoke(transformerList.AsQueryable());
			return new List<T>
			{
				(T) res
			};
		}

		private System.Type GetResultTypeIfChanged()
		{
			if (_postExecuteTransformer == null)
			{
				return null;
			}
			var elementType = _postExecuteTransformer.Method.GetParameters()[1].ParameterType.GetGenericArguments()[0];
			if (typeof(T).IsAssignableFrom(elementType))
			{
				return null;
			}

			return elementType;
		}

		private IList GetTypedResults(System.Type type)
		{
			var method = ReflectHelper.GetMethod(() => GetTypedResults<T>())
									.GetGenericMethodDefinition();
			var generic = method.MakeGenericMethod(type);
			return (IList) generic.Invoke(this, null);
		}
	}
}
