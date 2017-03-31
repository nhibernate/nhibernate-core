#if ASYNC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHibernate.Impl
{
	internal class FutureValueAsync<T> : IFutureValueAsync<T>, IDelayedValue
	{
		public delegate Task<IEnumerable<T>> GetResult();

		private readonly GetResult getResult;

		public FutureValueAsync(GetResult result)
		{
			getResult = result;
		}

		public async Task<T> GetValue()
		{
			var result = await getResult();
			if (ExecuteOnEval != null)
				// When not null, ExecuteOnEval is fetched with PostExecuteTransformer from IntermediateHqlTree
				// through ExpressionToHqlTranslationResults, which requires a IQueryable as input and directly
				// yields the scalar result when the query is scalar.
				return (T)ExecuteOnEval.DynamicInvoke(result.AsQueryable());
			var enumerator = result.GetEnumerator();

			if (!enumerator.MoveNext())
				return default(T);

			return enumerator.Current;
		}

		public Delegate ExecuteOnEval { get; set; }
	}
}
#endif