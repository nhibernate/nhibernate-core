using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
    internal class FutureValue<T> : IFutureValue<T>
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
                    return default(T);
                }

                return enumerator.Current;
            }
        }
    }
}