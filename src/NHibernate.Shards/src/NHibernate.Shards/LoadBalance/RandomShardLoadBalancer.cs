using System;
using System.Collections.Generic;

namespace NHibernate.Shards.LoadBalance
{
	/// <summary>
	/// Random selection load balancing algorithm.
	/// </summary>
	public class RandomShardLoadBalancer : BaseShardLoadBalancer
	{
		private readonly Random rand = new Random();

		/// <summary>
		/// Construct a RandomShardLoadBalancer
		/// </summary>
		/// <param name="shardIds">the ShardIds that we're balancing across</param>
		public RandomShardLoadBalancer(IList<ShardId> shardIds) : base(shardIds)
		{
		}

		/// <summary>
		/// the index of the next ShardId we should return
		/// </summary>
		protected override int NextIndex
		{
			get { return rand.Next() % ShardIds.Count; }
		}
	}
}