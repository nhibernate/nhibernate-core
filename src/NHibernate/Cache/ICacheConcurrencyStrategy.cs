using System;

namespace NHibernate.Cache {

	/// <summary>
	/// Implementors manage transactional access to cached data.
	/// </summary>
	/// <remarks>
	/// Transactions pass in a timestamp indicating transaction start time.
	/// </remarks>
	public interface ICacheConcurrencyStrategy {
		
		/// <summary>
		/// Attempt to cache an object
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="txTimestamp">A timestamp prior to the transaction start time</param>
		/// <returns>The cached object or <c>null</c></returns>
		/// <exception cref="CacheException"></exception>
		object Get(object key, long txTimestamp);

		/// <summary>
		/// Attempt to retrieve an object from the cache
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		/// <param name="txTimestamp">A timestamp prior to the transaction start time</param>
		/// <returns><c>true</c> if the object was successfully cached</returns>
		/// <exception cref="CacheException"></exception>
		bool Put(object key, object value, long txTimestamp);

		/// <summary>
		/// We are going to attempt to update/delete the keyed object
		/// </summary>
		/// <param name="key">The key</param>
		/// <exception cref="CacheException"></exception>
		void Lock(object key);

		/// <summary>
		/// We have finished the attempted update/delete (which may or may not have been successful)
		/// </summary>
		/// <param name="key">The key</param>
		/// <exception cref="CacheException"></exception>
		void Release(object key);
	}
}
