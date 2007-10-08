using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Stat
{
	public class StatisticsImpl : IStatistics, IStatisticsImplementor
	{
		private readonly object syncRoot = new object();

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

		public void Clear()
		{
			lock (syncRoot)
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

		public EntityStatistics GetEntityStatistics(string entityName)
		{
			lock (syncRoot)
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

		public CollectionStatistics GetCollectionStatistics(string role)
		{
			lock (syncRoot)
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

		public SecondLevelCacheStatistics GetSecondLevelCacheStatistics(string regionName)
		{
			lock (syncRoot)
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

		public QueryStatistics GetQueryStatistics(string queryString)
		{
			lock (syncRoot)
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

		public void OpenSession()
		{
			lock (syncRoot)
			{
				sessionOpenCount++;
			}
		}

		public void CloseSession()
		{
			lock (syncRoot)
			{
				sessionCloseCount++;
			}
		}

		public void Flush()
		{
			lock (syncRoot)
			{
				flushCount++;
			}
		}

		public void Connect()
		{
			lock (syncRoot)
			{
				connectCount++;
			}
		}

		public void LoadEntity(string entityName)
		{
			lock (syncRoot)
			{
				entityLoadCount++;
				GetEntityStatistics(entityName).loadCount++;
			}
		}

		public void FetchEntity(string entityName)
		{
			lock (syncRoot)
			{
				entityFetchCount++;
				GetEntityStatistics(entityName).fetchCount++;
			}
		}

		public void UpdateEntity(string entityName)
		{
			lock (syncRoot)
			{
				entityUpdateCount++;
				GetEntityStatistics(entityName).updateCount++;
			}
		}

		public void InsertEntity(string entityName)
		{
			lock (syncRoot)
			{
				entityInsertCount++;
				GetEntityStatistics(entityName).insertCount++;
			}
		}

		public void DeleteEntity(string entityName)
		{
			lock (syncRoot)
			{
				entityDeleteCount++;
				GetEntityStatistics(entityName).deleteCount++;
			}
		}

		public void LoadCollection(string role)
		{
			lock (syncRoot)
			{
				collectionLoadCount++;
				GetCollectionStatistics(role).loadCount++;
			}
		}

		public void FetchCollection(string role)
		{
			lock (syncRoot)
			{
				collectionFetchCount++;
				GetCollectionStatistics(role).fetchCount++;
			}
		}

		public void UpdateCollection(string role)
		{
			lock (syncRoot)
			{
				collectionUpdateCount++;
				GetCollectionStatistics(role).updateCount++;
			}
		}

		public void RecreateCollection(string role)
		{
			lock (syncRoot)
			{
				collectionRecreateCount++;
				GetCollectionStatistics(role).recreateCount++;
			}
		}

		public void RemoveCollection(string role)
		{
			lock (syncRoot)
			{
				collectionRemoveCount++;
				GetCollectionStatistics(role).removeCount++;
			}
		}

		public void SecondLevelCachePut(string regionName)
		{
			lock (syncRoot)
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
			lock (syncRoot)
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
			lock (syncRoot)
			{
				SecondLevelCacheStatistics slc = GetSecondLevelCacheStatistics(regionName);
				if (slc != null)
				{
					secondLevelCacheMissCount++;
					slc.missCount++;
				}
			}
		}

		public void QueryExecuted(string hql, int rows, long time)
		{
			lock (syncRoot)
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

		public void QueryCacheHit(string hql, string regionName)
		{
			lock (syncRoot)
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

		public void QueryCacheMiss(string hql, string regionName)
		{
			lock (syncRoot)
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

		public void QueryCachePut(string hql, string regionName)
		{
			lock (syncRoot)
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
