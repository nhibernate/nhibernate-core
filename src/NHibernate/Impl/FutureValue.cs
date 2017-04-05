using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Impl
{
	internal class FutureValue<T> : IFutureValue<T>, IDelayedValue
	{
		public delegate IEnumerable<T> GetResult();

		private readonly GetResult getResult;

		public FutureValue(GetResult result)
		{
			getResult = result;
		}

		public T Value
		{
			get
			{
				var result = getResult();
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
		}

		public Delegate ExecuteOnEval
		{
			get; set;
		}
	}
}