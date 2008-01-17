using NHibernate.Shards.Strategy.Access;
using NHibernate.Shards.Strategy.Resolution;
using NHibernate.Shards.Strategy.Selection;
using NHibernate.Shards.Util;

namespace NHibernate.Shards.Strategy
{
	/// <summary>
	/// Actual Implementation of IShardStrategy
	/// </summary>
	public class ShardStrategyImpl
	{
		private readonly IShardAccessStrategy shardAccessStrategy;
		private readonly IShardResolutionStrategy shardResolutionStrategy;
		private readonly IShardSelectionStrategy shardSelectionStrategy;

		public ShardStrategyImpl(IShardSelectionStrategy shardSelectionStrategy,
		                         IShardResolutionStrategy shardResolutionStrategy,
		                         IShardAccessStrategy shardAccessStrategy)
		{
			Preconditions.CheckNotNull(shardSelectionStrategy);
			Preconditions.CheckNotNull(shardResolutionStrategy);
			Preconditions.CheckNotNull(shardAccessStrategy);

			this.shardSelectionStrategy = shardSelectionStrategy;
			this.shardResolutionStrategy = shardResolutionStrategy;
			this.shardAccessStrategy = shardAccessStrategy;
		}

		public IShardAccessStrategy ShardAccessStrategy
		{
			get { return shardAccessStrategy; }
		}

		public IShardResolutionStrategy ShardResolutionStrategy
		{
			get { return shardResolutionStrategy; }
		}

		public IShardSelectionStrategy ShardSelectionStrategy
		{
			get { return shardSelectionStrategy; }
		}
	}
}