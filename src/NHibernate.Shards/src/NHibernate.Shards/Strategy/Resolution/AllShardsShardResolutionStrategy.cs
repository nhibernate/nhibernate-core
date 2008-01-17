using System.Collections.Generic;
using NHibernate.Shards.Strategy.Selection;

namespace NHibernate.Shards.Strategy.Resolution
{
	public class AllShardsShardResolutionStrategy : BaseShardResolutionStrategy
	{
		public AllShardsShardResolutionStrategy(IList<ShardId> shardIds) : base(shardIds)
		{
		}

		public override IList<ShardId> SelectShardIdsFromShardResolutionStrategyData(
			IShardResolutionStrategyData shardResolutionStrategyData)
		{
			return ShardIds;
		}
	}
}