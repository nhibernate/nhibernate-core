namespace NHibernate.Shards
{
	/// <summary>
	/// Simple interface used to reference something we can do against a <see cref="IShard"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IShardOperation<T>
	{
		/// <summary>
		/// The shard to Execute against
		/// </summary>
		/// <param name="shard"></param>
		/// <returns>the result of the operation</returns>
		T Execute(IShard shard);

		/// <summary>
		/// The name of the operation (useful for logging and debugging)
		/// </summary>
		string OperationName { get; }
	}
}