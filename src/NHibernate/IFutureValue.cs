using System.Threading;
using System.Threading.Tasks;

namespace NHibernate
{
	/// <summary>
	/// An object allowing to get at the value of a future query.
	/// </summary>
	/// <typeparam name="T">The type of the value returned by the query.</typeparam>
	public interface IFutureValue<T>
	{
		/// <summary>
		/// The value of the future query. If not already resolved, triggers all pending future query execution.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// Asynchronously get the value of the future query. If not already resolved, triggers all pending future query execution.
		/// Otherwise, this synchronously returns the already resolved value.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		/// <returns>The value of the future query.</returns>
		Task<T> GetValueAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
