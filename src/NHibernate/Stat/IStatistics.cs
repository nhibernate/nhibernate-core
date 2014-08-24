using System;
using System.Diagnostics;

namespace NHibernate.Stat
{
	/// <summary> 
	/// Statistics for a particular <see cref="ISessionFactory"/>.
	/// Beware of metrics, they are dependent of the <see cref="Stopwatch"/> precision:
	/// </summary>
	public interface IStatistics
	{
		/// <summary> Global number of entity deletes</summary>
		long EntityDeleteCount { get;}

		/// <summary> Global number of entity inserts</summary>
		long EntityInsertCount { get;}

		/// <summary> Global number of entity loads</summary>
		long EntityLoadCount { get;}

		/// <summary> Global number of entity fetchs</summary>
		long EntityFetchCount { get;}

		/// <summary> Global number of entity updates</summary>
		long EntityUpdateCount { get;}

		/// <summary> Global number of executed queries</summary>
		long QueryExecutionCount { get;}

		/// <summary> The <see cref="TimeSpan"/> of the slowest query.</summary>
		TimeSpan QueryExecutionMaxTime { get; }

		/// <summary> The query string for the slowest query.</summary>
		string QueryExecutionMaxTimeQueryString { get;}

		/// <summary> The global number of cached queries successfully retrieved from cache</summary>
		long QueryCacheHitCount { get;}

		/// <summary> The global number of cached queries *not* found in cache</summary>
		long QueryCacheMissCount { get;}

		/// <summary> The global number of cacheable queries put in cache</summary>
		long QueryCachePutCount { get;}

		/// <summary> Get the global number of flush executed by sessions (either implicit or explicit)</summary>
		long FlushCount { get;}

		/// <summary> 
		/// Get the global number of connections asked by the sessions
		/// (the actual number of connections used may be much smaller depending
		/// whether you use a connection pool or not)
		/// </summary>
		long ConnectCount { get;}

		/// <summary> Global number of cacheable entities/collections successfully retrieved from the cache</summary>
		long SecondLevelCacheHitCount { get;}

		/// <summary> Global number of cacheable entities/collections not found in the cache and loaded from the database.</summary>
		long SecondLevelCacheMissCount { get;}

		/// <summary> Global number of cacheable entities/collections put in the cache</summary>
		long SecondLevelCachePutCount { get;}

		/// <summary> Global number of sessions closed</summary>
		long SessionCloseCount { get;}

		/// <summary> Global number of sessions opened</summary>
		long SessionOpenCount { get;}

		/// <summary> Global number of collections loaded</summary>
		long CollectionLoadCount { get;}

		/// <summary> Global number of collections fetched</summary>
		long CollectionFetchCount { get;}

		/// <summary> Global number of collections updated</summary>
		long CollectionUpdateCount { get;}

		/// <summary> Global number of collections removed</summary>
		long CollectionRemoveCount { get;}

		/// <summary> Global number of collections recreated</summary>
		long CollectionRecreateCount { get;}

		/// <summary> Start time </summary>
		DateTime StartTime { get;}

		/// <summary> Enable/Disable statistics logs (this is a dynamic parameter)</summary>
		bool IsStatisticsEnabled { get;set;}

		/// <summary> All executed query strings</summary>
		string[] Queries { get;}

		/// <summary> The names of all entities</summary>
		string[] EntityNames { get;}

		/// <summary> The names of all collection roles</summary>
		string[] CollectionRoleNames { get;}

		/// <summary> Get all second-level cache region names</summary>
		string[] SecondLevelCacheRegionNames { get;}

		/// <summary> The number of transactions we know to have been successful</summary>
		long SuccessfulTransactionCount { get;}

		/// <summary> The number of transactions we know to have completed</summary>
		long TransactionCount { get;}

		/// <summary> The number of prepared statements that were acquired</summary>
		long PrepareStatementCount { get;}

		/// <summary> The number of prepared statements that were released</summary>
		long CloseStatementCount { get;}

		/// <summary> The number of <tt>StaleObjectStateException</tt>s  that occurred </summary>
		long OptimisticFailureCount { get;}

		/// <summary> Reset all statistics</summary>
		void Clear();

		/// <summary> Find entity statistics per name </summary>
		/// <param name="entityName">entity name </param>
		/// <returns> EntityStatistics object </returns>
		EntityStatistics GetEntityStatistics(string entityName);

		/// <summary> Get collection statistics per role </summary>
		/// <param name="role">collection role </param>
		/// <returns> CollectionStatistics </returns>
		CollectionStatistics GetCollectionStatistics(string role);

		/// <summary> Second level cache statistics per region </summary>
		/// <param name="regionName">region name </param>
		/// <returns> SecondLevelCacheStatistics </returns>
		SecondLevelCacheStatistics GetSecondLevelCacheStatistics(string regionName);

		/// <summary> Query statistics from query string (HQL or SQL) </summary>
		/// <param name="queryString">query string </param>
		/// <returns> QueryStatistics </returns>
		QueryStatistics GetQueryStatistics(string queryString);

		/// <summary> log in info level the main statistics</summary>
		void LogSummary();

		/// <summary> 
		/// The OperationThreshold to a value greater than <see cref="TimeSpan.MinValue"/> to enable logging of long running operations.
		/// </summary>
		/// <remarks>Operations that exceed the level will be logged.</remarks>
		TimeSpan OperationThreshold { get; set; }
	}
}
