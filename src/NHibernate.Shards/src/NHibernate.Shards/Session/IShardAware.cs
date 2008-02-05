namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Describes an object that knows the id of the shard on which it lives.
	/// </summary>
	public interface IShardAware
	{
		ShardId ShardId { get; set; }
	}
}