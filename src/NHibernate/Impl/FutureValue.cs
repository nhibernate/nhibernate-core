using System.Collections;

namespace NHibernate.Impl
{
    internal class FutureValue<T> : IFutureValue<T>
    {
        public delegate IList GetResult();

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

                if (result.Count == 0)
                {
                    return default(T);
                }

                return (T)result[0];
            }
        }
    }
}