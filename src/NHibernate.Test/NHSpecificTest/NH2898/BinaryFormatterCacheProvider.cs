using System;
using System.Collections.Generic;
using NHibernate.Cache;

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	public class BinaryFormatterCacheProvider : ICacheProvider
	{
		#region ICacheProvider Members

		// Since 5.2
		[Obsolete]
		ICache ICacheProvider.BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return BuildCache(regionName, properties);
		}

		public CacheBase BuildCache(string regionName, IDictionary<string, string> properties)
		{
			return new BinaryFormatterCache(regionName);
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
