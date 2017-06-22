using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate
{
	/// <summary>
	/// A deferred enumerable, which enumeration will trigger execution of all other pending futures.
	/// </summary>
	/// <typeparam name="T">The type of the enumerated elements.</typeparam>
	public interface IFutureEnumerable<T> : IEnumerable<T>
	{
		/// <summary>
		/// Asynchronously triggers the future query and all other pending future if the query was not already resolved, then
		/// returns a non-deferred enumerable of the query resulting items.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
		/// <returns>A non-deferred enumerable listing the resulting items of the future query.</returns>
		Task<IEnumerable<T>> GetEnumerableAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}
