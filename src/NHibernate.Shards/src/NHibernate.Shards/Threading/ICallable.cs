namespace NHibernate.Shards.Threading
{
	/// <summary>
	/// A task that returns a result and may throw an exception. 
	/// Implementors define a single method with no arguments called <c>Call</c>.
	/// </summary>
	public interface ICallable<T>
	{
		/// <summary>
		/// Computes a result, or throws an exception if unable to do so.
		/// </summary>
		/// <returns></returns>
		T Call();
	}
}