namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Gets ShardId of the shard given object lives on.
	/// </summary>
	public interface IShardIdResolver
	{
		/// <summary>
		/// Gets ShardId of the shard given object lives on.
		/// </summary>
		/// <param name="obj">Object whose Shard should be resolved</param>
		/// <returns>ShardId of the shard the object lives on; null if shard could not be resolved</returns>
		ShardId GetShardIdForObject(object obj);
	}
}