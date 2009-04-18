using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	public class FutureCriteriaBatch
	{
		private readonly List<ICriteria> criterias = new List<ICriteria>();
		private readonly IList<System.Type> resultCollectionGenericType = new List<System.Type>();

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
					for (int i = 0; i < criterias.Count; i++)
					{
						multiCriteria.Add(resultCollectionGenericType[i], criterias[i]);
					}
					results = multiCriteria.List();
					((SessionImpl)session).FutureCriteriaBatch = null;
				}
				return results;
			}
		}

		public void Add<T>(ICriteria criteria)
		{
			criterias.Add(criteria);
			resultCollectionGenericType.Add(typeof(T));
			index = criterias.Count - 1;
		}

		public void Add(ICriteria criteria)
		{
			Add<object>(criteria);
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
