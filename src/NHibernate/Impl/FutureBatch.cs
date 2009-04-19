using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	public abstract class FutureBatch<TQueryApproach, TMultiApproach>
	{
		private readonly List<TQueryApproach> queries = new List<TQueryApproach>();
		private readonly IList<System.Type> resultTypes = new List<System.Type>();
		private int index;
		private IList results;

		protected readonly SessionImpl session;

		protected FutureBatch(ISession session)
		{
			this.session = (SessionImpl)session;
		}

		public IList Results
		{
			get
			{
				if (results == null)
				{
					GetResults();
				}
				return results;
			}
		}

		public void Add<TResult>(TQueryApproach query)
		{
			queries.Add(query);
			resultTypes.Add(typeof(TResult));
			index = queries.Count - 1;
		}

		public void Add(TQueryApproach query)
		{
			Add<object>(query);
		}

		public IFutureValue<TResult> GetFutureValue<TResult>()
		{
			int currentIndex = index;
			return new FutureValue<TResult>(() => GetCurrentResult<TResult>(currentIndex));
		}

		public IEnumerable<TResult> GetEnumerator<TResult>()
		{
			int currentIndex = index;
			return new DelayedEnumerator<TResult>(() => GetCurrentResult<TResult>(currentIndex));
		}

		private void GetResults()
		{
			var multiApproach = CreateMultiApproach();
			for (int i = 0; i < queries.Count; i++)
			{
				AddTo(multiApproach, queries[i], resultTypes[i]);
			}
			results = GetResultsFrom(multiApproach);
			ClearCurrentFutureBatch();
		}

		private IList<TResult> GetCurrentResult<TResult>(int currentIndex)
		{
			return (IList<TResult>)Results[currentIndex];
		}

		protected abstract TMultiApproach CreateMultiApproach();
		protected abstract void AddTo(TMultiApproach multiApproach, TQueryApproach query, System.Type resultType);
		protected abstract IList GetResultsFrom(TMultiApproach multiApproach);
		protected abstract void ClearCurrentFutureBatch();
	}
}