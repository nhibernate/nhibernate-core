using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	public class FutureCriteriaBatch
	{
		private readonly List<ICriteria> criterias = new List<ICriteria>();
		private int index;
		private IList results;
		private readonly ISession session;

		public FutureCriteriaBatch(ISession session)
		{
			this.session = session;
		}

		public IList Results
		{
			get
			{
				if (results == null)
				{
					var multiCriteria = session.CreateMultiCriteria();
					foreach (var crit in criterias)
					{
						multiCriteria.Add(crit);
					}
					results = multiCriteria.List();
					((SessionImpl)session).FutureCriteriaBatch = null;
				}
				return results;
			}
		}

		public void Add(ICriteria criteria)
		{
			criterias.Add(criteria);
			index = criterias.Count - 1;
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
