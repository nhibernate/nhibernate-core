using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
    internal class DelayedEnumerator<T> : IEnumerable<T>, IDelayedValue
    {
        public delegate IEnumerable<T> GetResult();

        private readonly GetResult result;


        public Delegate ExecuteOnEval { get; set;}
        

        public DelayedEnumerator(GetResult result)
        {
            this.result = result;
        }

        public IEnumerable<T> Enumerable
        {
            get
            {
                var value = result();
                if(ExecuteOnEval != null)
                    value = (IEnumerable<T>)ExecuteOnEval.DynamicInvoke(value);
                foreach (T item in value)
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

    internal interface IDelayedValue
    {
        Delegate ExecuteOnEval { get; set; }
    }
}