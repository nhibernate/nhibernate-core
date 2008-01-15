namespace NHibernate.Shards.Query
{
	/// <summary>
	/// ShardedQuery extends the Query interface to provide the ability to query
	/// across shards
	/// </summary>
	public interface IShardedQuery : IQuery
	{
		/// <summary>
		/// Get the <see cref="QueryId"/> associated
		/// </summary>
		QueryId QueryId { get; }

		/// <summary>
		/// Get the <see cref="IQueryFactory"/> associated
		/// </summary>
		IQueryFactory QueryFactory { get; }
	}
}