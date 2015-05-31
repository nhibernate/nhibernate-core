using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;

namespace NHibernate.Linq
{
    public class CollectionFilterQueryable<T> : IQueryable<T>
    {
        public Expression Expression { get; private set; }

        public System.Type ElementType { get { return typeof(T); } }

        public IQueryProvider Provider { get; private set; }

        public CollectionFilterQueryable(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }      

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IQueryable<T>)this).GetEnumerator();
        }
    }
}
