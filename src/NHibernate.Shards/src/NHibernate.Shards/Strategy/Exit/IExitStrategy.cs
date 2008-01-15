namespace NHibernate.Shards.Strategy.Exit
{
	/// <summary>
	///   Classes implementing this interface gather results from operations that are
	/// executed across shards.  If you intend to use a specific implementation
	/// in conjunction with ParallelShardAccessStrategy that implementation must
	/// be threadsafe.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IExitStrategy<T>
	{
		/// <summary>
		/// Add the provided result and return whether or not the caller can halt
		/// processing.
		/// </summary>
		/// <param name="result">The result to add</param>
		/// <param name="shard"></param>
		/// <returns>Whether or not the caller can halt processing</returns>
		bool AddResult(T result, IShard shard);
		
		T CompileResults(IExitOperationsCollector exitOperationsCollector);
	}
}