using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate
{
	/// <summary>
	/// <para>A deferred query result. Accessing its enumerable result will trigger execution of all other pending futures.</para>
	/// <para>This interface is directly usable as a <see cref="IEnumerable{T}"/> for backward compatibility, but this will
	/// be dropped in a later version. Please get the <see cref="IEnumerable{T}"/> from <see cref="IFutureEnumerable{T}.GetEnumerable"/>
	/// or <see cref="IFutureEnumerable{T}.GetEnumerableAsync(CancellationToken)"/>.</para>
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

		/// <summary>
		/// Synchronously triggers the future query and all other pending future if the query was not already resolved, then
		/// returns a non-deferred enumerable of the query resulting items.
		/// </summary>
		/// <returns>A non-deferred enumerable listing the resulting items of the future query.</returns>
		IEnumerable<T> GetEnumerable();

		// Remove in 6.0, along with " : IEnumerable<T>" above.
		/// <summary>
		/// Synchronously triggers the future query and all other pending future if the query was not already resolved, then
		/// returns a non-deferred enumerator of the query resulting items.
		/// </summary>
		/// <returns>A non-deferred enumerator listing the resulting items of the future query.</returns>
		[Obsolete("Please use GetEnumerable() or GetEnumerableAsync(cancellationToken) instead")]
		new IEnumerator<T> GetEnumerator();
	}
}
