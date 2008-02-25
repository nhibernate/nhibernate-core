using System.Collections.Generic;

namespace NHibernate.Cache
{
	/// <summary>
	/// Cache Provider plugin for NHibernate that is configured by using
	/// <c>cache.provider_class="NHibernate.Cache.HashtableCacheProvider"</c>
	/// </summary>
	public class HashtableCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		public ICache BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new HashtableCache(regionName);
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		public void Start(IDictionary<string, string> properties)
		{
		}

		public void Stop()
		{
		}

		#endregion
	}
}