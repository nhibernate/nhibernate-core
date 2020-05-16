using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors;

namespace NHibernate.Multi
{
	interface ILinqBatchItem
	{
		List<TResult> GetTypedResults<TResult>();
	}

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
	public partial class LinqBatchItem<T> : QueryBatchItem<T>, ILinqBatchItem
	{
		private readonly PostResultTransformer _postExecuteTransformer;
		private readonly System.Type _resultTypeOverride;

		public LinqBatchItem(IQuery query) : base(query)
		{
		}

		internal LinqBatchItem(IQuery query, NhLinqExpression linq) : base(query)
		{
			_postExecuteTransformer = linq.ExpressionToHqlTranslationResults.PostResultTransformer;
			_resultTypeOverride = linq.ExpressionToHqlTranslationResults.ExecuteResultTypeOverride;
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
				IList transformerList = _resultTypeOverride == null
					? base.DoGetResults()
					//see LinqToFutureValueFixture tests that cover this scenario
					: LinqBatchReflectHelper.GetTypedResults(this, _resultTypeOverride);

				return GetTransformedResults(transformerList);
			}

			return base.DoGetResults();
		}

		private List<T> GetTransformedResults(IList transformerList)
		{
			var res = _postExecuteTransformer.Transform(transformerList.AsQueryable());
			return new List<T>
			{
				(T) res
			};
		}

		List<TResult> ILinqBatchItem.GetTypedResults<TResult>()
		{
			return GetTypedResults<TResult>();
		}
	}
}
