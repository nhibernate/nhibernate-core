using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Stat
{
	public class StatisticsImpl : IStatistics, IStatisticsImplementor
	{
		private readonly object _syncRoot = new object();

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(StatisticsImpl));
		private readonly ISessionFactoryImplementor sessionFactory;
		private long entityDeleteCount;
		private long entityInsertCount;
		private long entityLoadCount;
		private long entityFetchCount;
		private long entityUpdateCount;
		// log operations that take longer than this value
		private TimeSpan operationThreshold = TimeSpan.MaxValue;
		private long queryExecutionCount;
		private TimeSpan queryExecutionMaxTime;
		private string queryExecutionMaxTimeQueryString;
		private long queryCacheHitCount;
		private long queryCacheMissCount;
		private long queryCachePutCount;
		private long flushCount;
		private long connectCount;
		private long secondLevelCacheHitCount;
		private long secondLevelCacheMissCount;
		private long secondLevelCachePutCount;
		private long sessionCloseCount;
		private long sessionOpenCount;
		private long collectionLoadCount;
		private long collectionFetchCount;
		private long collectionUpdateCount;
		private long collectionRemoveCount;
		private long collectionRecreateCount;
		private DateTime startTime;
		private long commitedTransactionCount;
		private long transactionCount;
		private long prepareStatementCount;
		private long closeStatementCount;
		private long optimisticFailureCount;
		// second level cache statistics per region 
		private readonly Dictionary<string, SecondLevelCacheStatistics> secondLevelCacheStatistics = new Dictionary<string, SecondLevelCacheStatistics>();
		// entity statistics per name 
		private readonly Dictionary<string, EntityStatistics> entityStatistics = new Dictionary<string, EntityStatistics>();
		// collection statistics per name 
		private readonly Dictionary<string, CollectionStatistics> collectionStatistics = new Dictionary<string, CollectionStatistics>();
		// entity statistics per query string (HQL or SQL) 
		private readonly Dictionary<string, QueryStatistics> queryStatistics = new Dictionary<string, QueryStatistics>();

		internal const string OperationLoad = "load ";
		internal const string OperationFetch = "fetch ";
		internal const string OperationUpdate = "update ";
		internal const string OperationInsert = "insert ";
		internal const string OperationDelete = "delete ";
		internal const string OperationLoadCollection = "loadCollection ";
		internal const string OperationFetchCollection = "fetchCollection ";
		internal const string OperationUpdateCollection = "updateCollection ";
		internal const string OperationRecreateCollection = "recreateCollection ";
		internal const string OperationRemoveCollection = "removeCollection ";
		internal const string OperationExecuteQuery = "executeQuery ";
		internal const string OperationEndTransaction = "endTransaction ";

		public StatisticsImpl()
		{
			Clear();
		}

		public StatisticsImpl(ISessionFactoryImplementor sessionFactory)
			: this()
		{
			this.sessionFactory = sessionFactory;
		}

		#region IStatistics Members

		public long EntityDeleteCount
		{
			get { return entityDeleteCount; }
		}

		public long EntityInsertCount
		{
			get { return entityInsertCount; }
		}

		public long EntityLoadCount
		{
			get { return entityLoadCount; }
		}

		public long EntityFetchCount
		{
			get { return entityFetchCount; }
		}

		public long EntityUpdateCount
		{
			get { return entityUpdateCount; }
		}

		public long QueryExecutionCount
		{
			get { return queryExecutionCount; }
		}

		public TimeSpan QueryExecutionMaxTime
		{
			get { return queryExecutionMaxTime; }
		}

		public string QueryExecutionMaxTimeQueryString
		{
			get { return queryExecutionMaxTimeQueryString; }
		}

		public long QueryCacheHitCount
		{
			get { return queryCacheHitCount; }
		}

		public long QueryCacheMissCount
		{
			get { return queryCacheMissCount; }
		}

		public long QueryCachePutCount
		{
			get { return queryCachePutCount; }
		}

		public long FlushCount
		{
			get { return flushCount; }
		}

		public long ConnectCount
		{
			get { return connectCount; }
		}

		public long SecondLevelCacheHitCount
		{
			get { return secondLevelCacheHitCount; }
		}

		public long SecondLevelCacheMissCount
		{
			get { return secondLevelCacheMissCount; }
		}

		public long SecondLevelCachePutCount
		{
			get { return secondLevelCachePutCount; }
		}

		public long SessionCloseCount
		{
			get { return sessionCloseCount; }
		}

		public long SessionOpenCount
		{
			get { return sessionOpenCount; }
		}

		public long CollectionLoadCount
		{
			get { return collectionLoadCount; }
		}

		public long CollectionFetchCount
		{
			get { return collectionFetchCount; }
		}

		public long CollectionUpdateCount
		{
			get { return collectionUpdateCount; }
		}

		public long CollectionRemoveCount
		{
			get { return collectionRemoveCount; }
		}

		public long CollectionRecreateCount
		{
			get { return collectionRecreateCount; }
		}

		public DateTime StartTime
		{
			get { return startTime; }
		}

		public bool IsStatisticsEnabled { get; set; }

		public string[] Queries
		{
			get
			{
				var result = new string[queryStatistics.Keys.Count];
				queryStatistics.Keys.CopyTo(result, 0);
				return result;
			}
		}

		public string[] EntityNames
		{
			get
			{
				if (sessionFactory == null)
				{
					var result = new string[entityStatistics.Keys.Count];
					entityStatistics.Keys.CopyTo(result, 0);
					return result;
				}
				else
				{
					return sessionFactory.GetAllClassMetadata().Keys.ToArray();
				}
			}
		}

		public string[] CollectionRoleNames
		{
			get
			{
				if (sessionFactory == null)
				{
					var result = new string[collectionStatistics.Keys.Count];
					collectionStatistics.Keys.CopyTo(result, 0);
					return result;
				}
				else
				{
					ICollection<string> kc = sessionFactory.GetAllCollectionMetadata().Keys;
					var result = new string[kc.Count];
					kc.CopyTo(result, 0);
					return result;
				}
			}
		}

		public string[] SecondLevelCacheRegionNames
		{
			get
			{
				if (sessionFactory == null)
				{
					var result = new string[secondLevelCacheStatistics.Keys.Count];
					secondLevelCacheStatistics.Keys.CopyTo(result, 0);
					return result;
				}
				else
				{
					return sessionFactory.GetAllSecondLevelCacheRegions().Keys.ToArray();
				}
			}
		}

		public long SuccessfulTransactionCount
		{
			get { return commitedTransactionCount; }
		}

		public long TransactionCount
		{
			get { return transactionCount; }
		}

		public long PrepareStatementCount
		{
			get { return prepareStatementCount; }
		}

		public long CloseStatementCount
		{
			get { return closeStatementCount; }
		}

		public long OptimisticFailureCount
		{
			get { return optimisticFailureCount; }
		}

		public void Clear()
		{
			lock (_syncRoot)
			{
				secondLevelCacheHitCount = 0;
				secondLevelCacheMissCount = 0;
				secondLevelCachePutCount = 0;

				sessionCloseCount = 0;
				sessionOpenCount = 0;
				flushCount = 0;
				connectCount = 0;

				prepareStatementCount = 0;
				closeStatementCount = 0;

				entityDeleteCount = 0;
				entityInsertCount = 0;
				entityUpdateCount = 0;
				entityLoadCount = 0;
				entityFetchCount = 0;

				collectionRemoveCount = 0;
				collectionUpdateCount = 0;
				collectionRecreateCount = 0;
				collectionLoadCount = 0;
				collectionFetchCount = 0;

				queryExecutionCount = 0;
				queryCacheHitCount = 0;
				queryExecutionMaxTime = TimeSpan.Zero;
				queryExecutionMaxTimeQueryString = null;
				queryCacheMissCount = 0;
				queryCachePutCount = 0;

				transactionCount = 0;
				commitedTransactionCount = 0;

				optimisticFailureCount = 0;

				secondLevelCacheStatistics.Clear();
				entityStatistics.Clear();
				collectionStatistics.Clear();
				queryStatistics.Clear();

				startTime = DateTime.Now;
			}
		}

		public EntityStatistics GetEntityStatistics(string entityName)
		{
			lock (_syncRoot)
			{
				EntityStatistics es;
				if (!entityStatistics.TryGetValue(entityName, out es))
				{
					es = new EntityStatistics(entityName);
					entityStatistics[entityName] = es;
				}
				return es;
			}
		}

		public CollectionStatistics GetCollectionStatistics(string role)
		{
			lock (_syncRoot)
			{
				CollectionStatistics cs;
				if (!collectionStatistics.TryGetValue(role, out cs))
				{
					cs = new CollectionStatistics(role);
					collectionStatistics[role] = cs;
				}
				return cs;
			}
		}

		public SecondLevelCacheStatistics GetSecondLevelCacheStatistics(string regionName)
		{
			lock (_syncRoot)
			{
				SecondLevelCacheStatistics slcs;

				if (!secondLevelCacheStatistics.TryGetValue(regionName, out slcs))
				{
					if (sessionFactory == null)
						return null;
					var cache = sessionFactory.GetSecondLevelCacheRegion(regionName);
					if (cache == null)
						return null;
					slcs = new SecondLevelCacheStatistics(cache);
					secondLevelCacheStatistics[regionName] = slcs;
				}
				return slcs;
			}
		}

		public QueryStatistics GetQueryStatistics(string queryString)
		{
			lock (_syncRoot)
			{
				QueryStatistics qs;
				if (!queryStatistics.TryGetValue(queryString, out qs))
				{
					qs = new QueryStatistics(queryString);
					queryStatistics[queryString] = qs;
				}
				return qs;
			}
		}

		public void LogSummary()
		{
			log.Info("Logging statistics....");
			log.Info("start time: {0:o}", startTime);
			log.Info("sessions opened: {0}", sessionOpenCount);
			log.Info("sessions closed: {0}", sessionCloseCount);
			log.Info("transactions: {0}", transactionCount);
			log.Info("successful transactions: {0}", commitedTransactionCount);
			log.Info("optimistic lock failures: {0}", optimisticFailureCount);
			log.Info("flushes: {0}", flushCount);
			log.Info("connections obtained: {0}", connectCount);
			log.Info("statements prepared: {0}", prepareStatementCount);
			log.Info("statements closed: {0}", closeStatementCount);
			log.Info("second level cache puts: {0}", secondLevelCachePutCount);
			log.Info("second level cache hits: {0}", secondLevelCacheHitCount);
			log.Info("second level cache misses: {0}", secondLevelCacheMissCount);
			log.Info("entities loaded: {0}", entityLoadCount);
			log.Info("entities updated: {0}", entityUpdateCount);
			log.Info("entities inserted: {0}", entityInsertCount);
			log.Info("entities deleted: {0}", entityDeleteCount);
			log.Info("entities fetched (minimize this): {0}", entityFetchCount);
			log.Info("collections loaded: {0}", collectionLoadCount);
			log.Info("collections updated: {0}", collectionUpdateCount);
			log.Info("collections removed: {0}", collectionRemoveCount);
			log.Info("collections recreated: {0}", collectionRecreateCount);
			log.Info("collections fetched (minimize this): {0}", collectionFetchCount);
			log.Info("queries executed to database: {0}", queryExecutionCount);
			log.Info("query cache puts: {0}", queryCachePutCount);
			log.Info("query cache hits: {0}", queryCacheHitCount);
			log.Info("query cache misses: {0}", queryCacheMissCount);
			log.Info("max query time: {0:0} ms", queryExecutionMaxTime.TotalMilliseconds);
		}

		public TimeSpan OperationThreshold
		{
			get
			{
				return operationThreshold;
			}
			set
			{
				lock (_syncRoot)
				{
					operationThreshold = value;
				}
			}
		}

		#endregion

		#region IStatisticsImplementor Members

		public void OpenSession()
		{
			lock (_syncRoot)
			{
				sessionOpenCount++;
			}
		}

		public void CloseSession()
		{
			lock (_syncRoot)
			{
				sessionCloseCount++;
			}
		}

		public void Flush()
		{
			lock (_syncRoot)
			{
				flushCount++;
			}
		}

		public void Connect()
		{
			lock (_syncRoot)
			{
				connectCount++;
			}
		}

		public void LoadEntity(string entityName, TimeSpan time)
		{
			lock (_syncRoot)
			{
				entityLoadCount++;
				GetEntityStatistics(entityName).loadCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoad, entityName, time);
			}
		}

		public void FetchEntity(string entityName, TimeSpan time)
		{
			lock (_syncRoot)
			{
				entityFetchCount++;
				GetEntityStatistics(entityName).fetchCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoad, entityName, time);
			}
		}

		public void UpdateEntity(string entityName, TimeSpan time)
		{
			lock (_syncRoot)
			{
				entityUpdateCount++;
				GetEntityStatistics(entityName).updateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationUpdate, entityName, time);
			}
		}

		public void InsertEntity(string entityName, TimeSpan time)
		{
			lock (_syncRoot)
			{
				entityInsertCount++;
				GetEntityStatistics(entityName).insertCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationInsert, entityName, time);
			}
		}

		public void DeleteEntity(string entityName, TimeSpan time)
		{
			lock (_syncRoot)
			{
				entityDeleteCount++;
				GetEntityStatistics(entityName).deleteCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationDelete, entityName, time);
			}
		}

		public void LoadCollection(string role, TimeSpan time)
		{
			lock (_syncRoot)
			{
				collectionLoadCount++;
				GetCollectionStatistics(role).loadCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoadCollection, role, time);
			}
		}

		public void FetchCollection(string role, TimeSpan time)
		{
			lock (_syncRoot)
			{
				collectionFetchCount++;
				GetCollectionStatistics(role).fetchCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationFetchCollection, role, time);
			}
		}

		public void UpdateCollection(string role, TimeSpan time)
		{
			lock (_syncRoot)
			{
				collectionUpdateCount++;
				GetCollectionStatistics(role).updateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationUpdateCollection, role, time);
			}
		}

		public void RecreateCollection(string role, TimeSpan time)
		{
			lock (_syncRoot)
			{
				collectionRecreateCount++;
				GetCollectionStatistics(role).recreateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationRecreateCollection, role, time);
			}
		}

		public void RemoveCollection(string role, TimeSpan time)
		{
			lock (_syncRoot)
			{
				collectionRemoveCount++;
				GetCollectionStatistics(role).removeCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationRemoveCollection, role, time);
			}
		}

		public void SecondLevelCachePut(string regionName)
		{
			lock (_syncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCachePutCount++;
					slc.putCount++;
				}
			}
		}

		public void SecondLevelCacheHit(string regionName)
		{
			lock (_syncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCacheHitCount++;
					slc.hitCount++;
				}
			}
		}

		public void SecondLevelCacheMiss(string regionName)
		{
			lock (_syncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCacheMissCount++;
					slc.missCount++;
				}
			}
		}

		public void QueryExecuted(string hql, int rows, TimeSpan time)
		{
			lock (_syncRoot)
			{
				queryExecutionCount++;
				if (queryExecutionMaxTime < time)
				{
					queryExecutionMaxTime = time;
					queryExecutionMaxTimeQueryString = hql;
				}
				if (hql != null)
				{
					QueryStatistics qs = GetQueryStatistics(hql);
					qs.Executed(rows, time);
				}
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationExecuteQuery, hql, time);
			}
		}

		public void QueryCacheHit(string hql, string regionName)
		{
			lock (_syncRoot)
			{
				queryCacheHitCount++;
				if (hql != null)
				{
					QueryStatistics qs = GetQueryStatistics(hql);
					qs.cacheHitCount++;
				}
				SecondLevelCacheStatistics slcs = GetSecondLevelCacheStatistics(regionName);
				if (slcs != null)
				{
					slcs.hitCount++;
				}
			}
		}

		public void QueryCacheMiss(string hql, string regionName)
		{
			lock (_syncRoot)
			{
				queryCacheMissCount++;
				if (hql != null)
				{
					QueryStatistics qs = GetQueryStatistics(hql);
					qs.cacheMissCount++;
				}
				SecondLevelCacheStatistics slcs = GetSecondLevelCacheStatistics(regionName);
				if (slcs != null)
				{
					slcs.missCount++;
				}
			}
		}

		public void QueryCachePut(string hql, string regionName)
		{
			lock (_syncRoot)
			{
				queryCachePutCount++;
				if (hql != null)
				{
					QueryStatistics qs = GetQueryStatistics(hql);
					qs.cachePutCount++;
				}
				SecondLevelCacheStatistics slcs = GetSecondLevelCacheStatistics(regionName);
				if (slcs != null)
				{
					slcs.putCount++;
				}
			}
		}

		public void EndTransaction(bool success)
		{
			transactionCount++;
			if (success)
				commitedTransactionCount++;
		}

		public void CloseStatement()
		{
			closeStatementCount++;
		}

		public void PrepareStatement()
		{
			prepareStatementCount++;
		}

		public void OptimisticFailure(string entityName)
		{
			optimisticFailureCount++;
			GetEntityStatistics(entityName).optimisticFailureCount++;
		}

		#endregion

		private static void LogOperation(string operation, string entityName, TimeSpan time)
		{
			if (entityName != null)
				log.Info("{0}{1} {2}ms", operation, entityName, time.Milliseconds);
			else
				log.Info(operation); // just log that the event occurred
		}

		public override string ToString()
		{
			return
				new StringBuilder().Append("Statistics[").Append("start time=").Append(startTime).Append(",sessions opened=").Append
					(sessionOpenCount).Append(",sessions closed=").Append(sessionCloseCount).Append(",transactions=").Append(
					transactionCount).Append(",successful transactions=").Append(commitedTransactionCount).Append(
					",optimistic lock failures=").Append(optimisticFailureCount).Append(",flushes=").Append(flushCount).Append(
					",connections obtained=").Append(connectCount).Append(",statements prepared=").Append(prepareStatementCount).Append
					(",statements closed=").Append(closeStatementCount).Append(",second level cache puts=").Append(
					secondLevelCachePutCount).Append(",second level cache hits=").Append(secondLevelCacheHitCount).Append(
					",second level cache misses=").Append(secondLevelCacheMissCount).Append(",entities loaded=").Append(entityLoadCount)
					.Append(",entities updated=").Append(entityUpdateCount).Append(",entities inserted=").Append(entityInsertCount).
					Append(",entities deleted=").Append(entityDeleteCount).Append(",entities fetched=").Append(entityFetchCount).Append
					(",collections loaded=").Append(collectionLoadCount).Append(",collections updated=").Append(collectionUpdateCount).
					Append(",collections removed=").Append(collectionRemoveCount).Append(",collections recreated=").Append(
					collectionRecreateCount).Append(",collections fetched=").Append(collectionFetchCount).Append(
					",queries executed to database=").Append(queryExecutionCount).Append(",query cache puts=").Append(
					queryCachePutCount).Append(",query cache hits=").Append(queryCacheHitCount).Append(",query cache misses=").Append(
					queryCacheMissCount).Append(",max query time=").Append(queryExecutionMaxTime).Append(']').ToString();
		}
	}
}
