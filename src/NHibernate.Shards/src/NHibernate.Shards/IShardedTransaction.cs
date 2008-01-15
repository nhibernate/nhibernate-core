namespace NHibernate.Shards
{
	/// <summary>
	/// Simple interface to represent a shard-aware <see cref="ITransaction"/> 
	/// </summary>
	public interface IShardedTransaction : ITransaction
	{
		/// <summary>
		/// If a new Session is opened while ShardedTransaction exists, this method is
		/// called with the Session as argument. Implementations should set up a
		/// transaction for this session and sync it with ShardedTransaction
		/// </summary>
		/// <param name="session">The Session on which the Transaction should be setup</param>
		void SetupTransaction(ISession session);
	}
}