using System.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// Support for pluggable caches
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Configure the cache
		/// </summary>
		/// <param name="regionName">the name of the cache region</param>
		/// <param name="properties">configuration settings</param>
		/// <returns></returns>
		ICache BuildCache(string regionName, IDictionary<string, string> properties);

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