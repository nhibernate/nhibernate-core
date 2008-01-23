namespace NHibernate.Shards.Strategy.Exit
{
	public class FirstNonNullResultExitStrategy<T> : IExitStrategy<T>
	{
		/// <summary>
		/// Add the provided result and return whether or not the caller can halt
		/// processing.
		/// </summary>
		/// <param name="result">The result to add</param>
		/// <param name="shard"></param>
		/// <returns>Whether or not the caller can halt processing</returns>
		public bool AddResult(T result, IShard shard)
		{
			throw new System.NotImplementedException();
		}

		public T CompileResults(IExitOperationsCollector exitOperationsCollector)
		{
			throw new System.NotImplementedException();
		}
	}
}