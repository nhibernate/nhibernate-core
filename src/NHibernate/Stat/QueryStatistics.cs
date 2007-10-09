using System;
using System.Text;

namespace NHibernate.Stat
{
	/// <summary> Query statistics (HQL and SQL) </summary>
	/// <remarks>Note that for a cached query, the cache miss is equals to the db count</remarks>
	[Serializable]
	public class QueryStatistics : CategorizedStatistics
	{
		internal long cacheHitCount;
		internal long cacheMissCount;
		internal long cachePutCount;
		private long executionCount;
		private long executionRowCount;
		private long executionAvgTime;
		private long executionMaxTime;
		private long executionMinTime;

		public QueryStatistics(string categoryName) : base(categoryName) { }

		public long CacheHitCount
		{
			get { return cacheHitCount; }
		}

		public long CacheMissCount
		{
			get { return cacheMissCount; }
		}

		public long CachePutCount
		{
			get { return cachePutCount; }
		}

		public long ExecutionCount
		{
			get { return executionCount; }
		}

		public long ExecutionRowCount
		{
			get { return executionRowCount; }
		}

		public long ExecutionAvgTime
		{
			get { return executionAvgTime; }
		}

		public long ExecutionMaxTime
		{
			get { return executionMaxTime; }
		}

		public long ExecutionMinTime
		{
			get { return executionMinTime; }
		}

		/// <summary> Add statistics report of a DB query </summary>
		/// <param name="rows">rows count returned </param>
		/// <param name="time">time taken </param>
		internal void Executed(long rows, long time)
		{
			if (time < executionMinTime)
				executionMinTime = time;
			if (time > executionMaxTime)
				executionMaxTime = time;
			executionCount++;
			executionRowCount += rows;
			executionAvgTime = (executionAvgTime * executionCount + time) / executionCount;
		}

		public override string ToString()
		{
			return new StringBuilder()
				.Append("QueryStatistics[")
				.Append("cacheHitCount=").Append(cacheHitCount)
				.Append(",cacheMissCount=").Append(cacheMissCount)
				.Append(",cachePutCount=").Append(cachePutCount)
				.Append(",executionCount=").Append(executionCount)
				.Append(",executionRowCount=").Append(executionRowCount)
				.Append(",executionAvgTime=").Append(executionAvgTime)
				.Append(",executionMaxTime=").Append(executionMaxTime)
				.Append(",executionMinTime=").Append(executionMinTime)
				.Append(']').ToString();

		}
	}
}
