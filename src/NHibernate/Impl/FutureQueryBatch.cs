using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
    public class FutureQueryBatch
    {
        private readonly List<IQuery> queries = new List<IQuery>();
		private readonly IList<System.Type> resultCollectionGenericType = new List<System.Type>();

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
                	for (int i = 0; i < queries.Count; i++)
                	{
						multiQuery.Add(resultCollectionGenericType[i], queries[i]);
                	}
                	results = multiQuery.List();
                    ((SessionImpl)session).FutureQueryBatch = null;
                }
                return results;
            }
        }

		public void Add<T>(IQuery query)
		{
			queries.Add(query);
			resultCollectionGenericType.Add(typeof(T));
			index = queries.Count - 1;
		}

		public void Add(IQuery query)
        {
			Add<object>(query);
        }

        public IFutureValue<T> GetFutureValue<T>()
        {
            int currentIndex = index;
            return new FutureValue<T>(() => (IList<T>)Results[currentIndex]);
        }

        public IEnumerable<T> GetEnumerator<T>()
        {
            int currentIndex = index;
            return new DelayedEnumerator<T>(() => (IList<T>)Results[currentIndex]);
        }
    }
}
