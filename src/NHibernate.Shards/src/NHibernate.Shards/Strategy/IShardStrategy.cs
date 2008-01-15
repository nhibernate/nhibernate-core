using NHibernate.Shards.Strategy.Access;
using NHibernate.Shards.Strategy.Resolution;
using NHibernate.Shards.Strategy.Selection;

namespace NHibernate.Shards.Strategy
{
	public interface IShardStrategy
	{
		IShardSelectionStrategy ShardSelectionStrategy { get; }

		IShardResolutionStrategy ShardResolutionStrategy { get; }

		IShardAccessStrategy ShardAccessStrategy { get; }
	}
}