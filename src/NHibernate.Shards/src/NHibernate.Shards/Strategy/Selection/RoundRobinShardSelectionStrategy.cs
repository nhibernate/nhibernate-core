using NHibernate.Shards.LoadBalance;

namespace NHibernate.Shards.Strategy.Selection
{
	public class RoundRobinShardSelectionStrategy : LoadBalancedShardSelectionStrategy
	{
		public RoundRobinShardSelectionStrategy(RoundRobinShardLoadBalancer shardLoadBalancer) 
			: base(shardLoadBalancer)
		{
		}
	}
}