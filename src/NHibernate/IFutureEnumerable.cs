using System.Collections.Generic;

namespace NHibernate
{
	/// <summary>
	/// A deferred enumerable, which enumeration will trigger execution of all other pending futures.
	/// </summary>
	/// <typeparam name="T">The type of the enumerated elements.</typeparam>
	public interface IFutureEnumerable<out T> : IEnumerable<T>
	{
		/// <summary>
		/// An <see cref="IAsyncEnumerable{T}"/> for enumerating the future asynchronously.
		/// </summary>
		IAsyncEnumerable<T> AsyncEnumerable { get; }
	}
}
