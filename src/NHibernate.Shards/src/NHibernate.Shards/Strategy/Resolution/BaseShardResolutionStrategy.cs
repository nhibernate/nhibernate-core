using System.Collections.Generic;
using NHibernate.Shards.Strategy.Selection;

namespace NHibernate.Shards.Strategy.Resolution
{
	public abstract class BaseShardResolutionStrategy : BaseHasShardIdList, IShardResolutionStrategy
	{
		protected BaseShardResolutionStrategy(IList<ShardId> shardIds) : base(shardIds)
		{
		}

		public abstract IList<ShardId> SelectShardIdsFromShardResolutionStrategyData(
			IShardResolutionStrategyData shardResolutionStrategyData);
	}
}