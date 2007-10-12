using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Stat
{
	public class StatisticsImpl : IStatistics, IStatisticsImplementor
	{
		private object _syncRoot;

		private static readonly ILog log = LogManager.GetLogger(typeof(StatisticsImpl));
		private readonly ISessionFactoryImplementor sessionFactory;
		private long entityDeleteCount;
		private long entityInsertCount;
		private long entityLoadCount;
		private long entityFetchCount;
		private long entityUpdateCount;
		private long queryExecutionCount;
		private long queryExecutionMaxTime;
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
		private bool isStatisticsEnabled;
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

		public StatisticsImpl()
		{
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

		public long QueryExecutionMaxTime
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

		public bool IsStatisticsEnabled
		{
			get { return isStatisticsEnabled; }
			set { isStatisticsEnabled = value; }
		}

		public string[] Queries
		{
			get { return ArrayHelper.ToStringArray(queryStatistics.Keys); }
		}

		public string[] EntityNames
		{
			get
			{
				if (sessionFactory == null)
				{
					return ArrayHelper.ToStringArray(entityStatistics.Keys);
				}
				else
				{
					return ArrayHelper.ToStringArray(sessionFactory.GetAllClassMetadata().Keys);
				}
			}
		}

		public string[] CollectionRoleNames
		{
			get
			{
				if (sessionFactory == null)
				{
					return ArrayHelper.ToStringArray(collectionStatistics.Keys);
				}
				else
				{
					return ArrayHelper.ToStringArray(sessionFactory.GetAllCollectionMetadata().Keys);
				}
			}
		}

		public string[] SecondLevelCacheRegionNames
		{
			get
			{
				if (sessionFactory == null)
				{
					return ArrayHelper.ToStringArray(secondLevelCacheStatistics.Keys);
				}
				else
				{
					return ArrayHelper.ToStringArray(sessionFactory.GetAllSecondLevelCacheRegions().Keys);
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
				queryExecutionMaxTime = 0;
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
				entityStatistics.TryGetValue(entityName, out es);
				if (es == null)
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
				collectionStatistics.TryGetValue(role, out cs);
				if (cs == null)
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
				secondLevelCacheStatistics.TryGetValue(regionName, out slcs);
				if (slcs == null)
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
				queryStatistics.TryGetValue(queryString, out qs);
				if (qs == null)
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
			log.Info(string.Format("start time: {0}", startTime));// TODO change format to show ms
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
			log.Info("max query time: " + queryExecutionMaxTime + "ms");
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
		public void LoadEntity(string entityName)
		{
			lock (SyncRoot)
			{
				entityLoadCount++;
				GetEntityStatistics(entityName).loadCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void FetchEntity(string entityName)
		{
			lock (SyncRoot)
			{
				entityFetchCount++;
				GetEntityStatistics(entityName).fetchCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateEntity(string entityName)
		{
			lock (SyncRoot)
			{
				entityUpdateCount++;
				GetEntityStatistics(entityName).updateCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void InsertEntity(string entityName)
		{
			lock (SyncRoot)
			{
				entityInsertCount++;
				GetEntityStatistics(entityName).insertCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DeleteEntity(string entityName)
		{
			lock (SyncRoot)
			{
				entityDeleteCount++;
				GetEntityStatistics(entityName).deleteCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void LoadCollection(string role)
		{
			lock (SyncRoot)
			{
				collectionLoadCount++;
				GetCollectionStatistics(role).loadCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void FetchCollection(string role)
		{
			lock (SyncRoot)
			{
				collectionFetchCount++;
				GetCollectionStatistics(role).fetchCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateCollection(string role)
		{
			lock (SyncRoot)
			{
				collectionUpdateCount++;
				GetCollectionStatistics(role).updateCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RecreateCollection(string role)
		{
			lock (SyncRoot)
			{
				collectionRecreateCount++;
				GetCollectionStatistics(role).recreateCount++;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RemoveCollection(string role)
		{
			lock (SyncRoot)
			{
				collectionRemoveCount++;
				GetCollectionStatistics(role).removeCount++;
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
		public void QueryExecuted(string hql, int rows, long time)
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
				slcs.hitCount++;
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
				slcs.missCount++;
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
				slcs.putCount++;
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
	}
}
