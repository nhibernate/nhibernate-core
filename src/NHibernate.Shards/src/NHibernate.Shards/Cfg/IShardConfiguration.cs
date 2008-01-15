using NHibernate.Shards.Session;

namespace NHibernate.Shards.Cfg
{
	/// <summary>
	/// Describes the configuration properties that can vary across the <see cref="ISessionFactory"/>
	/// instances contained within your <see cref="IShardedSessionFactory"/>.
	/// </summary>
	public interface IShardConfiguration
	{
		/// <summary>
		/// the url of the shard.
		/// </summary>
		string ShardUrl { get; }

		/// <summary>
		/// the user that will be sent to the shard for authentication
		/// </summary>
		string ShardUser { get; }

		/// <summary>
		/// the password that will be sent to the shard for authentication
		/// </summary>
		string ShardPassword { get; }

		/// <summary>
		/// the name that the <see cref="ISessionFactory"/> created from this config will have
		/// </summary>
		string ShardSessionFactoryName { get; }

		/// <summary>
		/// unique id of the shard
		/// </summary>
		int ShardId { get; }

		/// <summary>
		/// the datasource for the shard
		/// </summary>
		string ShardDatasource { get; }

		/// <summary>
		/// the cache region prefix for the shard
		/// </summary>
		string ShardCacheRegionPrefix { get; }
	}
}