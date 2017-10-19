using System;
using System.Collections;
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

		public T Value => _getResult().FirstOrDefault();

		public async Task<T> GetValueAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _getResultAsync(cancellationToken).ConfigureAwait(false);
			return result.FirstOrDefault();
		}

		public Delegate ExecuteOnEval { get; set; }

		public IList TransformList(IList collection)
		{
			if (ExecuteOnEval == null)
				return collection;


			// When not null on a future value, ExecuteOnEval is fetched with PostExecuteTransformer from
			// IntermediateHqlTree through ExpressionToHqlTranslationResults, which requires a IQueryable
			// as input and directly yields the scalar result when the query is scalar.
			var resultElement = (T) ExecuteOnEval.DynamicInvoke(collection.AsQueryable());

			return new List<T> {resultElement};
		}
	}
}
