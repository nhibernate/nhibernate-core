namespace NHibernate.Shards.Session
{
	/// <summary>
	/// The main runtime inteface between .Net application and NHibernate Shards.
	/// ShardedSession represents a logical transaction that might be spanning
	/// multiple shards. It follows the contract set by ISession API, and adds some
	/// shard-related methods. 
	/// </summary>
	public interface IShardedSession : ISession
	{
		/// <summary>
		/// Gets the non-sharded session with which the objects is associated.
		/// </summary>
		/// <param name="obj">the object for which we want the Session</param>
		/// <returns>
		///	The Session with which this object is associated, or null if the
		/// object is not associated with a session belonging to this ShardedSession
		/// </returns>
		ISession GetSessionForObject(object obj);

		/// <summary>
		///  Gets the ShardId of the shard with which the objects is associated.
		/// </summary>
		/// <param name="obj">the object for which we want the Session</param>
		/// <returns>
		/// the ShardId of the Shard with which this object is associated, or
		/// null if the object is not associated with a shard belonging to this
		/// ShardedSession
		/// </returns>
		ShardId GetShardIdForObject(object obj);

		/// <summary>
		/// Place the session into a state where every create operation takes place
		/// on the same shard.  Once the shard is locked on a session it cannot
		/// be unlocked.
		/// </summary>
		void LockShard();
	}
}