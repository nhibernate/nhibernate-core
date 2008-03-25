namespace NHibernate.Cache
{
	/// <summary>
	/// Implementors define a caching algorithm.
	/// </summary>
	/// <remarks>
	/// <threadsafety instance="true" />
	/// <para>
	/// All implementations <em>must</em> be threadsafe.
	/// </para>
	/// <para>
	/// The key is the identifier of the object that is being cached and the 
	/// value is a <see cref="CachedItem"/>.
	/// </para>
	/// </remarks>
	public interface ICache
	{
		/// <summary>
		/// Get the object from the Cache
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Get(object key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void Put(object key, object value);

		/// <summary>
		/// Remove an item from the Cache.
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to remove.</param>
		/// <exception cref="CacheException"></exception>
		void Remove(object key);

		/// <summary>
		/// Clear the Cache
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void Clear();

		/// <summary>
		/// Clean up.
		/// </summary>
		/// <exception cref="CacheException"></exception>
		void Destroy();

		/// <summary>
		/// If this is a clustered cache, lock the item
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to lock.</param>
		/// <exception cref="CacheException"></exception>
		void Lock(object key);

		/// <summary>
		/// If this is a clustered cache, unlock the item
		/// </summary>
		/// <param name="key">The Key of the Item in the Cache to unlock.</param>
		/// <exception cref="CacheException"></exception>
		void Unlock(object key);

		/// <summary>
		/// Generate a timestamp
		/// </summary>
		/// <returns></returns>
		long NextTimestamp();

		/// <summary>
		/// Get a reasonable "lock timeout"
		/// </summary>
		int Timeout { get; }

		/// <summary>
		/// Gets the name of the cache region
		/// </summary>
		string RegionName { get; }
	}
}