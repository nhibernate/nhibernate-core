using System.Collections.Generic;
using NHibernate.Shards.Strategy.Selection;

namespace NHibernate.Shards.Strategy.Resolution
{
	public interface IShardResolutionStrategy
	{
		IList<ShardId> SelectShardIdsFromShardResolutionStrategyData(
			IShardResolutionStrategyData shardResolutionStrategyData);
	}
}