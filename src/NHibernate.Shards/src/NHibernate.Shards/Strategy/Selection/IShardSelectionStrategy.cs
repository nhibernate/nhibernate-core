namespace NHibernate.Shards.Strategy.Selection
{
	public interface IShardSelectionStrategy
	{
		/// <summary>
		/// Determine the specific shard on which this object should reside
		/// </summary>
		/// <param name="obj">the new object for which we are selecting a shard</param>
		/// <returns>the id of the shard on which this object should live</returns>
		ShardId SelectShardIdForNewObject(object obj);
	}
}