using System;
using System.Collections;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Cache Provider plugin for NHibernate that is configured by using
	/// <c>hibernate.cache.provider_class="NHibernate.Cache.HashtableCacheProvider"</c>
	/// </summary>
	public class HashtableCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		public ICache BuildCache(string regionName, ICollection properties)
		{
			return new HashtableCache( regionName );
		}

		public long NextTimestamp()
		{
			return Timestamper.Next();
		}

		#endregion
	}
}
