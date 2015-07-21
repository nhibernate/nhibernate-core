using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Util;
using System.Linq;

namespace NHibernate.Stat
{
	public class StatisticsImpl : IStatistics, IStatisticsImplementor
	{
		private object _syncRoot;

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(StatisticsImpl));
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

		private object SyncRoot
		{
			get
			{
				if (_syncRoot == null)
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
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
				queryStatistics.Keys.CopyTo(result,0);
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Clear()
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public EntityStatistics GetEntityStatistics(string entityName)
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public CollectionStatistics GetCollectionStatistics(string role)
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public SecondLevelCacheStatistics GetSecondLevelCacheStatistics(string regionName)
		{
			lock (SyncRoot)
			{
				SecondLevelCacheStatistics slcs;

				if (!secondLevelCacheStatistics.TryGetValue(regionName, out slcs))
				{
					if (sessionFactory == null)
						return null;
					ICache cache = sessionFactory.GetSecondLevelCacheRegion(regionName);
					if (cache == null)
						return null;
					slcs = new SecondLevelCacheStatistics(cache);
					secondLevelCacheStatistics[regionName] = slcs;
				}
				return slcs;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public QueryStatistics GetQueryStatistics(string queryString)
		{
			lock (SyncRoot)
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
			log.Info(string.Format("start time: {0}", startTime.ToString("o")));
			log.Info("sessions opened: " + sessionOpenCount);
			log.Info("sessions closed: " + sessionCloseCount);
			log.Info("transactions: " + transactionCount);
			log.Info("successful transactions: " + commitedTransactionCount);
			log.Info("optimistic lock failures: " + optimisticFailureCount);
			log.Info("flushes: " + flushCount);
			log.Info("connections obtained: " + connectCount);
			log.Info("statements prepared: " + prepareStatementCount);
			log.Info("statements closed: " + closeStatementCount);
			log.Info("second level cache puts: " + secondLevelCachePutCount);
			log.Info("second level cache hits: " + secondLevelCacheHitCount);
			log.Info("second level cache misses: " + secondLevelCacheMissCount);
			log.Info("entities loaded: " + entityLoadCount);
			log.Info("entities updated: " + entityUpdateCount);
			log.Info("entities inserted: " + entityInsertCount);
			log.Info("entities deleted: " + entityDeleteCount);
			log.Info("entities fetched (minimize this): " + entityFetchCount);
			log.Info("collections loaded: " + collectionLoadCount);
			log.Info("collections updated: " + collectionUpdateCount);
			log.Info("collections removed: " + collectionRemoveCount);
			log.Info("collections recreated: " + collectionRecreateCount);
			log.Info("collections fetched (minimize this): " + collectionFetchCount);
			log.Info("queries executed to database: " + queryExecutionCount);
			log.Info("query cache puts: " + queryCachePutCount);
			log.Info("query cache hits: " + queryCacheHitCount);
			log.Info("query cache misses: " + queryCacheMissCount);
			log.Info("max query time: " + queryExecutionMaxTime.TotalMilliseconds.ToString("0") + " ms");
		}

		public TimeSpan OperationThreshold
		{
			get
			{
				return operationThreshold;
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				lock (SyncRoot)
				{
					operationThreshold = value;
				}
			}
		}

		#endregion

		#region IStatisticsImplementor Members

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void OpenSession()
		{
			lock (SyncRoot)
			{
				sessionOpenCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void CloseSession()
		{
			lock (SyncRoot)
			{
				sessionCloseCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Flush()
		{
			lock (SyncRoot)
			{
				flushCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Connect()
		{
			lock (SyncRoot)
			{
				connectCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void LoadEntity(string entityName, TimeSpan time)
		{
			lock (SyncRoot)
			{
				entityLoadCount++;
				GetEntityStatistics(entityName).loadCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoad, entityName, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void FetchEntity(string entityName, TimeSpan time)
		{
			lock (SyncRoot)
			{
				entityFetchCount++;
				GetEntityStatistics(entityName).fetchCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoad, entityName, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateEntity(string entityName, TimeSpan time)
		{
			lock (SyncRoot)
			{
				entityUpdateCount++;
				GetEntityStatistics(entityName).updateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationUpdate, entityName, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void InsertEntity(string entityName, TimeSpan time)
		{
			lock (SyncRoot)
			{
				entityInsertCount++;
				GetEntityStatistics(entityName).insertCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationInsert, entityName, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DeleteEntity(string entityName, TimeSpan time)
		{
			lock (SyncRoot)
			{
				entityDeleteCount++;
				GetEntityStatistics(entityName).deleteCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationDelete, entityName, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void LoadCollection(string role, TimeSpan time)
		{
			lock (SyncRoot)
			{
				collectionLoadCount++;
				GetCollectionStatistics(role).loadCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationLoadCollection, role, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void FetchCollection(string role, TimeSpan time)
		{
			lock (SyncRoot)
			{
				collectionFetchCount++;
				GetCollectionStatistics(role).fetchCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationFetchCollection, role, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateCollection(string role, TimeSpan time)
		{
			lock (SyncRoot)
			{
				collectionUpdateCount++;
				GetCollectionStatistics(role).updateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationUpdateCollection, role, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RecreateCollection(string role, TimeSpan time)
		{
			lock (SyncRoot)
			{
				collectionRecreateCount++;
				GetCollectionStatistics(role).recreateCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationRecreateCollection, role, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveCollection(string role, TimeSpan time)
		{
			lock (SyncRoot)
			{
				collectionRemoveCount++;
				GetCollectionStatistics(role).removeCount++;
			}
			if (operationThreshold < time)
			{
				LogOperation(OperationRecreateCollection, role, time);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SecondLevelCachePut(string regionName)
		{
			lock (SyncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCachePutCount++;
					slc.putCount++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SecondLevelCacheHit(string regionName)
		{
			lock (SyncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCacheHitCount++;
					slc.hitCount++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SecondLevelCacheMiss(string regionName)
		{
			lock (SyncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCacheMissCount++;
					slc.missCount++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void QueryExecuted(string hql, int rows, TimeSpan time)
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void QueryCacheHit(string hql, string regionName)
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void QueryCacheMiss(string hql, string regionName)
		{
			lock (SyncRoot)
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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void QueryCachePut(string hql, string regionName)
		{
			lock (SyncRoot)
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
				log.Info(operation + entityName + " " + time.Milliseconds + "ms");
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
