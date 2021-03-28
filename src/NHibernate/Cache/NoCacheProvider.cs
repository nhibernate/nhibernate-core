using System;
using System.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// A cache provider placeholder used when caching is disabled.
	/// </summary>
	public class NoCacheProvider : ICacheProvider
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(NoCacheProvider));

		public const string WarnMessage = "Second-level cache is enabled in a class, but no cache provider was selected. Fake cache used.";

		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		/// <summary>
		/// Configure the cache
		/// </summary>
		/// <param name="regionName">the name of the cache region</param>
		/// <param name="properties">configuration settings</param>
		/// <exception cref="CacheException" />
		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			// NH different behavior because NH-1093
			log.Warn(WarnMessage);
			return new FakeCache(regionName);
		}

		/// <summary>
		/// Generate a timestamp
		/// </summary>
		public long NextTimestamp()
		{
			// This is used by SessionFactoryImpl to hand to the generated SessionImpl;
			// was the only reason I could see that we cannot just use null as
			// Settings.CacheProvider
			return DateTime.UtcNow.Ticks / (100 * TimeSpan.TicksPerMillisecond);
		}

		/// <summary>
		/// Callback to perform any necessary initialization of the underlying cache implementation during SessionFactory
		/// construction.
		/// </summary>
		/// <param name="properties">current configuration settings.</param>
		public void Start(IDictionary<string, string> properties)
		{
			// this is called by SessionFactory irregardless; we just disregard here;
			// could also add a check to SessionFactory to only conditionally call start
		}

		/// <summary>
		/// Callback to perform any necessary cleanup of the underlying cache implementation during SessionFactory.close().
		/// </summary>
		public void Stop()
		{
			// this is called by SessionFactory irregardless; we just disregard here;
			// could also add a check to SessionFactory to only conditionally call stop
		}

		//public bool IsMinimalPutsEnabledByDefault
		//{
		//    // this is called from SettingsFactory irregardless; trivial to simply disregard
		//    get { return false; }
		//}
	}
}
