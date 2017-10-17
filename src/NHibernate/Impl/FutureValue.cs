using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Util;

namespace NHibernate.Impl
{
	internal class FutureValue<T> : IFutureValue<T>, IDelayedValue
	{
		public delegate IEnumerable GetResult();

		public delegate Task<IEnumerable> GetResultAsync(CancellationToken cancellationToken);

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
					return InvokeExecuteOnEval(result);

				return result.Cast<T>().FirstOrDefault();
			}
		}

		public async Task<T> GetValueAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = await _getResultAsync(cancellationToken).ConfigureAwait(false);
			if (ExecuteOnEval != null)
				return InvokeExecuteOnEval(result);

			return result.Cast<T>().FirstOrDefault();
		}

		private T InvokeExecuteOnEval(IEnumerable result)
		{
			// We get the required element type of ExecuteOnEval and cast the result enumerable
			var elementType = ExecuteOnEval.Method.GetParameters()[1].ParameterType.GetGenericArguments()[0];

			// When not null, ExecuteOnEval is fetched with PostExecuteTransformer from IntermediateHqlTree
			// through ExpressionToHqlTranslationResults, which requires a IQueryable as input and directly
			// yields the scalar result when the query is scalar.
			return (T)ExecuteOnEval.DynamicInvoke(MakeQueryableHelper.MakeQueryable(result, elementType));
		}

		public Delegate ExecuteOnEval
		{
			get; set;
		}
	}
}
