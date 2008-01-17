namespace NHibernate.Shards.LoadBalance
{
	/// <summary>
	/// Describes a load balance for shards.
	/// Implementations are expected to be threadsafe.
	/// </summary>
	public interface IShardLoadBalancer
	{
		/// <summary>
		/// the next ShardId
		/// Expected to be threadsafe.
		/// </summary>
		ShardId NextShardId { get; }
	}
}