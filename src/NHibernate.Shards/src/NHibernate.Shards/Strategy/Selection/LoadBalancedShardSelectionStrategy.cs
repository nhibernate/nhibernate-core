using NHibernate.Shards.LoadBalance;

namespace NHibernate.Shards.Strategy.Selection
{
	public class LoadBalancedShardSelectionStrategy : IShardSelectionStrategy
	{
		private readonly IShardLoadBalancer shardLoadBalancer;

		public LoadBalancedShardSelectionStrategy(IShardLoadBalancer shardLoadBalancer)
		{
			this.shardLoadBalancer = shardLoadBalancer;
		}

		public IShardLoadBalancer ShardLoadBalancer
		{
			get { return shardLoadBalancer; }
		}

		/// <summary>
		/// Determine the specific shard on which this object should reside
		/// taking into account the load balance strategy configurated.
		/// </summary>
		/// <param name="obj">the new object for which we are selecting a shard</param>
		/// <returns>the id of the shard on which this object should live</returns>
		public virtual ShardId SelectShardIdForNewObject(object obj)
		{
			return shardLoadBalancer.NextShardId;
		}
	}
}