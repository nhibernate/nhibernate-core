namespace NHibernate.Shards.Criteria
{
	/// <summary>
	/// Interface for a shard-aware <see cref="ICriteria"/> implementation.
	/// <seealso cref="ICriteria"/> 
	/// </summary>
	public interface IShardedCriteria : ICriteria
	{
		/// <summary>
		/// CriteriaId of this ShardedCriteria instance.
		/// </summary>
		CriteriaId CriteriaId { get; }

		/// <summary>
		/// CriteriaFactory of this ShardedCriteria instance.
		/// </summary>
		ICriteriaFactory CriteriaFactory { get; }
	}
}