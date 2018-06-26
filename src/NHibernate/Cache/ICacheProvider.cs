using System;
using System.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// Support for pluggable caches
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Build a cache.
		/// </summary>
		/// <param name="regionName">The name of the cache region.</param>
		/// <param name="properties">Configuration settings.</param>
		/// <returns>A cache.</returns>
		// 6.0 TODO: return a CacheBase instead
#pragma warning disable 618
		ICache BuildCache(string regionName, IDictionary<string, string> properties);
#pragma warning restore 618

		/// <summary>
		/// generate a timestamp
		/// </summary>
		/// <returns></returns>
		long NextTimestamp();

		/// <summary>
		/// Callback to perform any necessary initialization of the underlying cache implementation
		/// during ISessionFactory construction.
		/// </summary>
		/// <param name="properties">current configuration settings</param>
		void Start(IDictionary<string, string> properties);

		/// <summary>
		/// Callback to perform any necessary cleanup of the underlying cache implementation
		/// during <see cref="ISessionFactory.Close" />.
		/// </summary>
		void Stop();
	}
}
