using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Impl
{
	internal class FutureValue<T> : IFutureValue<T>, IDelayedValue
	{
		public delegate IEnumerable<T> GetResult();

		public delegate Task<IEnumerable<T>> GetResultAsync(CancellationToken cancellationToken);

		private readonly GetResult _getResult;

		private readonly GetResultAsync _getResultAsync;

		public FutureValue(GetResult result, GetResultAsync resultAsync)
		{
			_getResult = result;
			_getResultAsync = resultAsync;
		}

		public T Value
		{
			get
			{
				var result = _getResult();
				if (ExecuteOnEval != null)
					// When not null, ExecuteOnEval is fetched with PostExecuteTransformer from IntermediateHqlTree
					// through ExpressionToHqlTranslationResults, which requires a IQueryable as input and directly
					// yields the scalar result when the query is scalar.
					return (T)ExecuteOnEval.DynamicInvoke(result.AsQueryable());

				return result.FirstOrDefault();
			}
		}

		public async Task<T> GetValueAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _getResultAsync(cancellationToken).ConfigureAwait(false);
			if (ExecuteOnEval != null)
				// When not null, ExecuteOnEval is fetched with PostExecuteTransformer from IntermediateHqlTree
				// through ExpressionToHqlTranslationResults, which requires a IQueryable as input and directly
				// yields the scalar result when the query is scalar.
				return (T)ExecuteOnEval.DynamicInvoke(result.AsQueryable());
			return result.FirstOrDefault();
		}

		public Delegate ExecuteOnEval
		{
			get; set;
		}
	}
}