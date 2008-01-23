using System.Collections.Generic;
using NHibernate.Shards.Threading;

namespace NHibernate.Shards.LoadBalance
{
	/// <summary>
	/// Round robin load balancing algorithm.
	/// </summary>
	public class RoundRobinShardLoadBalancer : BaseShardLoadBalancer
	{
		private readonly AtomicInteger nextIndex = new AtomicInteger(0);

		public RoundRobinShardLoadBalancer()
		{
		}

		/// <summary>
		/// Construct a RoundRobinShardLoadBalancer
		/// </summary>
		/// <param name="shardIds">the ShardIds that we're balancing across</param>
		public RoundRobinShardLoadBalancer(IList<ShardId> shardIds) : base(shardIds)
		{
		}

		/// <summary>
		/// The index of the next ShardId we should return
		/// </summary>
		protected override int NextIndex
		{
			get { return nextIndex.GetAndIncrement() % ShardIds.Count; }
		}
	}
}