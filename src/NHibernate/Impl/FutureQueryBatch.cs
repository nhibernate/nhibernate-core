using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
    public class FutureQueryBatch
    {
        private readonly List<IQuery> queries = new List<IQuery>();
        private int index;
        private IList results;
        private readonly ISession session;

        public FutureQueryBatch(ISession session)
        {
            this.session = session;
        }

        public IList Results
        {
            get
            {
                if (results == null)
                {
                    var multiQuery = session.CreateMultiQuery();
                    foreach (var crit in queries)
                    {
                        multiQuery.Add(crit);
                    }
                    results = multiQuery.List();
                    ((SessionImpl)session).FutureQueryBatch = null;
                }
                return results;
            }
        }

        public void Add(IQuery query)
        {
            queries.Add(query);
            index = queries.Count - 1;
        }

        public IFutureValue<T> GetFutureValue<T>()
        {
            int currentIndex = index;
            return new FutureValue<T>(() => (IList)Results[currentIndex]);
        }

        public IEnumerable<T> GetEnumerator<T>()
        {
            int currentIndex = index;
            return new DelayedEnumerator<T>(() => (IList)Results[currentIndex]);
        }
    }
}
