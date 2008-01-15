using System.Collections.Generic;
using NHibernate;
using NHibernate.Shards.Strategy;

namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Shard-aware extension to <see cref="ISessionFactory"/>.  Similar to <see cref="ISessionFactory"/>,
	/// ShardedSessionFactory is threadsafe.
	/// </summary>
	public interface IShardedSessionFactory : ISessionFactory
	{
		/// <summary>
		/// All an unmodifiable list of the <see cref="ISessionFactory"/> objects contained within.
		/// </summary>
		IList<ISessionFactory> SessionFactories { get; }

		/// <summary>
		/// This method is provided to allow a client to work on a subset of
		/// shards or a specialized <see cref="IShardStrategyFactory"/>.  By providing
		/// the desired shardIds, the client can limit operations to these shards.
		/// Alternatively, this method can be used to create a ShardedSessionFactory
		/// with different strategies that might be appropriate for a specific operation.
		///
		/// The factory returned will not be stored as one of the factories that would
		/// be returned by a call to getSessionFactories.
		/// </summary>
		/// <param name="shardIds"></param>
		/// <param name="shardStrategyFactory"></param>
		/// <returns>specially configured ShardedSessionFactory</returns>
		IShardedSessionFactory GetSessionFactory(IList<ShardId> shardIds, IShardStrategyFactory shardStrategyFactory);

		/// <summary>
		/// Create database connection(s) and open a ShardedSession on it,
		/// specifying an interceptor.
		/// </summary>
		/// <param name="interceptor">a session-scoped interceptor</param>
		/// <returns></returns>
		/// Throws <see cref="HibernateException"/>
		new IShardedSession OpenSession(IInterceptor interceptor);

		/// <summary>
		/// Create database connection(s) and open a ShardedSession on it.
		/// </summary>
		/// <returns></returns>
		/// Throws <see cref="HibernateException"/>
		IShardedSession openSession();
	}
}