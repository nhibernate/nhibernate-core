using System;
using System.Text;

namespace NHibernate.Stat
{
	/// <summary> Entity related statistics </summary>
	[Serializable]
	public class EntityStatistics : CategorizedStatistics
	{
		internal long loadCount;
		internal long updateCount;
		internal long insertCount;
		internal long deleteCount;
		internal long fetchCount;
		internal long optimisticFailureCount;

		internal EntityStatistics(string categoryName) : base(categoryName) { }

		public long LoadCount
		{
			get { return loadCount; }
		}

		public long UpdateCount
		{
			get { return updateCount; }
		}

		public long InsertCount
		{
			get { return insertCount; }
		}

		public long DeleteCount
		{
			get { return deleteCount; }
		}

		public long FetchCount
		{
			get { return fetchCount; }
		}

		public long OptimisticFailureCount
		{
			get { return optimisticFailureCount; }
		}

		public override string ToString()
		{
			return new StringBuilder()
				.Append("EntityStatistics[")
				.Append("loadCount=").Append(loadCount)
				.Append(",updateCount=").Append(updateCount)
				.Append(",insertCount=").Append(insertCount)
				.Append(",deleteCount=").Append(deleteCount)
				.Append(",fetchCount=").Append(fetchCount)
				.Append(",optimisticLockFailureCount=").Append(optimisticFailureCount)
				.Append(']').ToString();
		}
	}
}
