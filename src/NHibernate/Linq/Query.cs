using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
    /// <summary>
    /// A default implementation of IQueryable for use with QueryProvider
    /// </summary>
    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly Expression _expression;
        private readonly IQueryProvider _provider;

        public Query(IQueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public Query(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof (IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            _provider = provider;
            _expression = expression;
        }

        #region IQueryable<T> Members

        public Expression Expression
        {
            get { return _expression; }
        }

        public System.Type ElementType
        {
            get { return typeof (T); }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            if (_expression.NodeType == ExpressionType.Constant &&
                ((ConstantExpression) _expression).Value == this)
            {
                return "Query(" + typeof (T) + ")";
            }
            
            return _expression.ToString();
        }
    }
}