using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
    internal class DelayedEnumerator<T> : IEnumerable<T>
    {
        public delegate IList<T> GetResult();

        private readonly GetResult result;

        public DelayedEnumerator(GetResult result)
        {
            this.result = result;
        }

        public IEnumerable<T> Enumerable
        {
            get
            {
                foreach (T item in result())
                {
                    yield return item;
                }
            }
        }

        #region IEnumerable<T> Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Enumerable).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.GetEnumerator();
        }

        #endregion
    }
}