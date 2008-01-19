namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRunnableFuture<T> : IFuture<T>, IRunnable
	{
	}
}