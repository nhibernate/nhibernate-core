using System.Collections.Generic;

namespace NHibernate.Shards.LoadBalance
{
	/// <summary>
	/// Helpful base class for ShardLoadBalancer implementations.
	/// </summary>
	public abstract class BaseShardLoadBalancer : BaseHasShardIdList , IShardLoadBalancer
	{
		protected BaseShardLoadBalancer(IList<ShardId> shardIds) : base(shardIds)
		{
		}

		public BaseShardLoadBalancer()
		{
		}

		/// <summary>
		/// the next ShardId
		/// </summary>
		public ShardId NextShardId
		{
			get 
			{
				return shardIds[NextIndex];
			}
		}
		
		/// <summary>
		/// the index of the next ShardId we should return
		/// </summary>
		protected abstract int NextIndex { get; }
	}
}