using System.Collections.Generic;
using System.Linq;
using Remotion.Data.Linq;

namespace NHibernate.Linq
{
    public class NhQueryExecutor : IQueryExecutor
    {
        private readonly ISession _session;

        public NhQueryExecutor(ISession session)
        {
            _session = session;
        }

        // Executes a query with a scalar result, i.e. a query that ends with a result operator such as Count, Sum, or Average.
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).Single();
        }

        // Executes a query with a single result object, i.e. a query that ends with a result operator such as First, Last, Single, Min, or Max.
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return returnDefaultWhenEmpty ? ExecuteCollection<T>(queryModel).SingleOrDefault() : ExecuteCollection<T>(queryModel).Single();
        }

        // Executes a query with a collection result.
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var commandData = QueryModelVisitor.GenerateHqlQuery(queryModel);

            var query = commandData.CreateQuery(_session, typeof(T));

            // TODO - check which call on Query makes most sense...
            return (IEnumerable<T>) query.List();
        }
    }
}