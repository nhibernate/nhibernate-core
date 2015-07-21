using System;
using System.Collections;
using System.Collections.Generic;

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
				var enumerator = result.GetEnumerator();

				if (!enumerator.MoveNext())
				{
				    var defVal = default(T);
                    if (ExecuteOnEval != null)
                        defVal = (T)ExecuteOnEval.DynamicInvoke(defVal);
				    return defVal;
				}

                var val = enumerator.Current;

                if (ExecuteOnEval != null)
                    val = (T)ExecuteOnEval.DynamicInvoke(val);
				    
                return val;
            }
        }

        public Delegate ExecuteOnEval
        {
            get; set;
        }
    }
}