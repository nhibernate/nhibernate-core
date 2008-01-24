namespace NHibernate.Shards.Query
{
	/// <summary>
	/// Interface for events that can be lazily applied to a
	/// <see cref="IQuery"/>. Useful because we don't allocate a
	/// <see cref="IQuery"/> until we actually need it, and programmers
	/// might be calling a variety of methods against
	/// {@link org.hibernate.shards.query.ShardedQueryImpl}
	/// which need to be applied to the actual <see cref="IQuery"/> when
	/// it is allocated.
	/// </summary>
	public interface IQueryEvent
	{
		/// <summary>
		/// Apply the event
		/// </summary>
		/// <param name="query">the Query to Apply the event to</param>
		void OnEvent(IQuery query);
	}
}