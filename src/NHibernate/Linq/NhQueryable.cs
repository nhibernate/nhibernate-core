using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;

namespace NHibernate.Linq
{
    /// <summary>
    /// Provides the main entry point to a LINQ query.
    /// </summary>
    public class NhQueryable<T> : QueryableBase<T>
    {
        private static IQueryExecutor CreateExecutor(ISession session)
        {
            return new NhQueryExecutor(session);
        }

        // This constructor is called by our users, create a new IQueryExecutor.
        public NhQueryable(ISession session)
            : base(CreateExecutor(session))
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        public NhQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}