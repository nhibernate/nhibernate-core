using System;
using System.Text;

namespace NHibernate.Stat
{
	/// <summary> Collection related statistics </summary>
	[Serializable]
	public class CollectionStatistics : CategorizedStatistics
	{
		internal long loadCount;
		internal long fetchCount;
		internal long updateCount;
		internal long removeCount;
		internal long recreateCount;

		internal CollectionStatistics(string categoryName) : base(categoryName) { }

		public long LoadCount
		{
			get { return loadCount; }
		}

		public long FetchCount
		{
			get { return fetchCount; }
		}

		public long UpdateCount
		{
			get { return updateCount; }
		}

		public long RemoveCount
		{
			get { return removeCount; }
		}

		public long RecreateCount
		{
			get { return recreateCount; }
		}

		public override string ToString()
		{
			return new StringBuilder()
				.Append("CollectionStatistics[")
				.Append("loadCount=").Append(loadCount)
				.Append(",fetchCount=").Append(fetchCount)
				.Append(",recreateCount=").Append(recreateCount)
				.Append(",removeCount=").Append(removeCount)
				.Append(",updateCount=").Append(updateCount)
				.Append(']').ToString();

		}
	}
}
